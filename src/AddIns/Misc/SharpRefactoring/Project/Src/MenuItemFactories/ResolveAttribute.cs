// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
	/// Description of ResolveAttribute.
	/// </summary>
	public class ResolveAttribute : IRefactoringMenuItemFactory
	{
		public MenuItem Create(RefactoringMenuContext context)
		{
			// TODO : [Test] above method is in Default context?
//			if (context.ExpressionResult.Context != ExpressionContext.Attribute)
//				return null;
			if (!(context.ResolveResult is UnknownIdentifierResolveResult || context.ResolveResult is UnknownMethodResolveResult))
				return null;

			List<IClass> results = new List<IClass>();
			
			ParseInformation info = ParserService.GetParseInformation(context.Editor.FileName);
			
			if (info == null || info.CompilationUnit == null || info.CompilationUnit.ProjectContent == null)
				return null;
			
			ICompilationUnit unit = info.CompilationUnit;
			IProjectContent pc = info.CompilationUnit.ProjectContent;
			
			string name = null;
			
			if (context.ResolveResult is UnknownMethodResolveResult) {
				var rr = context.ResolveResult as UnknownMethodResolveResult;
				SearchAttributesWithName(results, pc, rr.CallName);
				
				foreach (IProjectContent content in pc.ThreadSafeGetReferencedContents())
					SearchAttributesWithName(results, content, rr.CallName);
				
				name = rr.CallName;
			}
			
			if (context.ResolveResult is UnknownIdentifierResolveResult) {
				var rr = context.ResolveResult as UnknownIdentifierResolveResult;
				SearchAttributesWithName(results, pc, rr.Identifier);
				
				foreach (IProjectContent content in pc.ThreadSafeGetReferencedContents())
					SearchAttributesWithName(results, content, rr.Identifier);
				
				name = rr.Identifier;
			}
			
			if (!results.Any())
				return null;
			
			MenuItem item = new MenuItem() {
				Header = string.Format(StringParser.Parse("${res:AddIns.SharpRefactoring.ResolveAttribute}"), name),
				Icon = ClassBrowserIconService.GotoArrow.CreateImage()
			};
			
			foreach (IClass c in results) {
				string newNamespace = c.Namespace;
				MenuItem subItem = new MenuItem();
				subItem.Header = "using " + newNamespace;
				subItem.Icon = ClassBrowserIconService.Namespace.CreateImage();
				item.Items.Add(subItem);
				subItem.Click += delegate {
					NamespaceRefactoringService.AddUsingDeclaration(unit, context.Editor.Document, newNamespace, true);
					ParserService.BeginParse(context.Editor.FileName, context.Editor.Document);
				};
			}
			
			return item;
		}
		
		IClass baseClass = null;
		
		void SearchAttributesWithName(List<IClass> searchResults, IProjectContent pc, string name)
		{
			if (baseClass == null)
				baseClass = pc.GetClass("System.Attribute", 0);
			
			foreach (IClass c in pc.Classes) {
				if (c.IsTypeInInheritanceTree(baseClass) && (c.Name == name || c.Name == name + "Attribute"))
					searchResults.Add(c);
			}
		}
	}
}
