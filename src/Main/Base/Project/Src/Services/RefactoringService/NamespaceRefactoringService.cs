// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using ICSharpCode.SharpDevelop.Editor;
using System;
using System.Collections.Generic;
using System.Linq;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Dom.Refactoring;

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
		
		public static void ManageUsings(Gui.IProgressMonitor progressMonitor, string fileName, IDocument document, bool sort, bool removedUnused)
		{
			ParseInformation info = ParserService.ParseFile(fileName, document);
			if (info == null) return;
			ICompilationUnit cu = info.CompilationUnit;
			
			List<IUsing> newUsings = new List<IUsing>(cu.UsingScope.Usings);
			if (sort) {
				newUsings.Sort(CompareUsings);
			}
			
			if (removedUnused) {
				IList<IUsing> decl = cu.ProjectContent.Language.RefactoringProvider.FindUnusedUsingDeclarations(Gui.DomProgressMonitor.Wrap(progressMonitor), fileName, document.Text, cu);
				if (decl != null && decl.Count > 0) {
					foreach (IUsing u in decl) {
						string ns = null;
						for (int i = 0; i < u.Usings.Count; i++) {
							ns = u.Usings[i];
							if (ns == "System") break;
						}
						if (ns != "System") { // never remove "using System;"
							newUsings.Remove(u);
						}
					}
				}
			}
			
			// put empty line after last System namespace
			if (sort) {
				PutEmptyLineAfterLastSystemNamespace(newUsings);
			}
			
			cu.ProjectContent.Language.CodeGenerator.ReplaceUsings(new RefactoringDocumentAdapter(document), cu.UsingScope.Usings, newUsings);
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
		
		public static void AddUsingDeclaration(ICompilationUnit cu, IDocument document, string newNamespace, bool sortExistingUsings)
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
			
			List<IUsing> newUsings = new List<IUsing>(cu.UsingScope.Usings);
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
			cu.ProjectContent.Language.CodeGenerator.ReplaceUsings(new RefactoringDocumentAdapter(document), cu.UsingScope.Usings, newUsings);
		}
	}
}
