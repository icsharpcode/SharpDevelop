// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Siegfried Pammer" email="siegfriedpammer@gmail.com" />
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Refactoring;

namespace SharpRefactoring
{
	/// <summary>
	/// Description of ResolveExtensionMethod.
	/// </summary>
	public class ResolveExtensionMethod : IRefactoringMenuItemFactory
	{
		public MenuItem Create(RefactoringMenuContext context)
		{
			if (context.ExpressionResult.Context == ExpressionContext.Attribute)
				return null;
			if (!(context.ResolveResult is UnknownMethodResolveResult))
				return null;
			
			UnknownMethodResolveResult rr = context.ResolveResult as UnknownMethodResolveResult;
			
			MenuItem item = new MenuItem() {
				Header = string.Format(StringParser.Parse("${res:AddIns.SharpRefactoring.ResolveExtensionMethod}"), rr.CallName),
				Icon = ClassBrowserIconService.GotoArrow.CreateImage()
			};
			
			List<IClass> results = new List<IClass>();
			
			IProjectContent pc = rr.CallingClass.ProjectContent;
			
			SearchAllExtensionMethodsWithName(results, pc, rr.CallName);
			
			foreach (IProjectContent content in pc.ReferencedContents)
				SearchAllExtensionMethodsWithName(results, content, rr.CallName);
			
			if (!results.Any())
				return null;
			
			foreach (IClass c in results) {
				string newNamespace = c.Namespace;
				MenuItem subItem = new MenuItem();
				subItem.Header = "using " + newNamespace;
				subItem.Icon = ClassBrowserIconService.Namespace.CreateImage();
				item.Items.Add(subItem);
				subItem.Click += delegate {
					NamespaceRefactoringService.AddUsingDeclaration(rr.CallingClass.CompilationUnit, context.Editor.Document, newNamespace, true);
					ParserService.BeginParse(context.Editor.FileName, context.Editor.Document);
				};
			}
			
			return item;
		}
		
		void SearchAllExtensionMethodsWithName(List<IClass> searchResults, IProjectContent pc, string name)
		{
			foreach (IClass c in pc.Classes) {
				if (c.HasExtensionMethods && !searchResults.Any(cl => cl.Namespace == c.Namespace) &&
				    c.Methods.Any(m => m.IsExtensionMethod && m.Name == name))
					searchResults.Add(c);
			}
		}
	}
}
