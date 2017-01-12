// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Linq;

using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Dom.Refactoring;
using ICSharpCode.SharpDevelop.Editor;

namespace ICSharpCode.SharpDevelop.Refactoring
{
	public static class NamespaceRefactoringService
	{
		internal static bool IsSystemNamespace(string ns)
		{
			return ns.StartsWith("System.") || ns == "System";
		}
		
		static int CompareUsings(IUsing a, IUsing b)
		{
			if (a.HasAliases != b.HasAliases)
				return a.HasAliases ? 1 : -1;
			if (a.Usings.Count != 0 && b.Usings.Count != 0) {
				string u1 = a.Usings[0];
				string u2 = b.Usings[0];
				if (IsSystemNamespace(u1) && !IsSystemNamespace(u2)) {
					return -1;
				} else if (!IsSystemNamespace(u1) && IsSystemNamespace(u2)) {
					return 1;
				}
				return u1.CompareTo(u2);
			}
			if (a.Aliases.Count != 0 && b.Aliases.Count != 0) {
				return a.Aliases.Keys.First().CompareTo(b.Aliases.Keys.First());
			}
			return 0;
		}
		
		public static void ManageUsings(Gui.IProgressMonitor progressMonitor, string fileName, IDocument document, bool sort, bool removeUnused)
		{
			ParseInformation info = ParserService.ParseFile(fileName, document);
			if (info == null) return;
			var compilationUnit = info.CompilationUnit;
			IEnumerable<IUsing> unusedDeclarations = removeUnused ? compilationUnit.ProjectContent.Language.RefactoringProvider.FindUnusedUsingDeclarations(Gui.DomProgressMonitor.Wrap(progressMonitor), fileName, document.Text, compilationUnit) : Enumerable.Empty<IUsing>();
			var refactoringDocument = new RefactoringDocumentAdapter(document);
			var codeGenerator = compilationUnit.ProjectContent.Language.CodeGenerator;
			ManageUsingsForScope(compilationUnit.UsingScope, sort, unusedDeclarations, codeGenerator, refactoringDocument);
		}
		
		private static void ManageUsingsForScope(IUsingScope usingScope, bool sort, IEnumerable<IUsing> unusedDeclarations, CodeGenerator codeGenerator, IRefactoringDocument refactoringDocument)
		{
			List<IUsing> newUsings = new List<IUsing>(usingScope.Usings);
			if (sort) {
				newUsings.Sort(CompareUsings);
			}
			
			unusedDeclarations.Where(u => !u.Usings.Any(usingName => usingName == "System")).ForEach(u => newUsings.Remove(u));

			foreach(var childScope in usingScope.ChildScopes) {
				ManageUsingsForScope(childScope, sort, unusedDeclarations, codeGenerator, refactoringDocument);
			}
			
			if (sort) {
				PutEmptyLineAfterLastSystemNamespace(newUsings);
			}
			
			codeGenerator.ReplaceUsings(refactoringDocument, usingScope.Usings, newUsings);
		}
		
		static void PutEmptyLineAfterLastSystemNamespace(List<IUsing> newUsings)
		{
			if (newUsings.Count > 1 && newUsings[0].Usings.Count > 0) {
				bool inSystem = IsSystemNamespace(newUsings[0].Usings[0]);
				int inSystemCount = 1;
				for (int i = 1; inSystem && i < newUsings.Count; i++) {
					inSystem = newUsings[i].Usings.Count > 0 && IsSystemNamespace(newUsings[i].Usings[0]);
					if (inSystem) {
						inSystemCount++;
					} else {
						if (inSystemCount > 2) { // only use empty line when there are more than 2 system namespaces
							newUsings.Insert(i, null);
						}
					}
				}
			}
		}
		
		public static void AddUsingDeclaration(ICompilationUnit cu, IDocument document, IUsingScope usingScope, string newNamespace, bool sortExistingUsings)
		{
			if (cu == null)
				throw new ArgumentNullException("cu");
			if (document == null)
				throw new ArgumentNullException("document");
			if (newNamespace == null)
				throw new ArgumentNullException("newNamespace");
			
			ParseInformation info = ParserService.ParseFile(cu.FileName, document);
			if (info != null)
				cu = info.CompilationUnit;
			
			IUsing newUsingDecl = new DefaultUsing(cu.ProjectContent);
			newUsingDecl.Usings.Add(newNamespace);
			
			while (!usingScope.Usings.Any() && usingScope.Parent != null) {
				usingScope = usingScope.Parent;
			}
			
			List<IUsing> newUsings = new List<IUsing>(usingScope.Usings);
			if (sortExistingUsings) {
				newUsings.Sort(CompareUsings);
			}
			bool inserted = false;
			for (int i = 0; i < newUsings.Count; i++) {
				if (CompareUsings(newUsingDecl, newUsings[i]) <= 0) {
					newUsings.Insert(i, newUsingDecl);
					inserted = true;
					break;
				}
			}
			if (!inserted) {
				newUsings.Add(newUsingDecl);
			}
			if (sortExistingUsings) {
				PutEmptyLineAfterLastSystemNamespace(newUsings);
			}
			cu.ProjectContent.Language.CodeGenerator.ReplaceUsings(new RefactoringDocumentAdapter(document), usingScope.Usings, newUsings);
		}
	}
}
