// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.TextEditor.Document;

namespace ICSharpCode.SharpDevelop.Refactoring
{
	public static class NamespaceRefactoringService
	{
		static int CompareUsings(IUsing a, IUsing b)
		{
			if (a.HasAliases != b.HasAliases)
				return a.HasAliases ? 1 : -1;
			if (a.Usings.Count != 0 && b.Usings.Count != 0) {
				string u1 = a.Usings[0];
				string u2 = b.Usings[0];
				if (u1.StartsWith("System.") || u1 == "System") {
					if (!(u2.StartsWith("System.") || u2 == "System"))
						return -1;
				} else {
					if (u2.StartsWith("System.") || u2 == "System")
						return 1;
				}
				return a.Usings[0].CompareTo(b.Usings[0]);
			}
			if (a.Aliases.Count != 0 && b.Aliases.Count != 0) {
				return a.Aliases.Keys[0].CompareTo(b.Aliases.Keys[0]);
			}
			return 0;
		}
		
		public static void ManageUsings(string fileName, IDocument document, bool sort, bool removedUnused)
		{
			ParseInformation info = ParserService.ParseFile(fileName, document.TextContent);
			if (info == null) return;
			ICompilationUnit cu = info.MostRecentCompilationUnit;
			
			List<IUsing> newUsings = new List<IUsing>(cu.Usings);
			if (sort) {
				newUsings.Sort(CompareUsings);
			}
			
			if (removedUnused) {
				IList<IUsing> decl = cu.ProjectContent.Language.RefactoringProvider.FindUnusedUsingDeclarations(fileName, document.TextContent, cu);
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
			
			cu.ProjectContent.Language.CodeGenerator.ReplaceUsings(document, cu.Usings, newUsings);
		}
	}
}
