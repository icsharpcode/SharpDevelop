// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

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
			if (context.ProjectContent == null)
				return null;
			
			UnknownMethodResolveResult rr = context.ResolveResult as UnknownMethodResolveResult;
			
			MenuItem item = new MenuItem() {
				Header = string.Format(StringParser.Parse("${res:AddIns.SharpRefactoring.ResolveExtensionMethod}"), rr.CallName),
				Icon = ClassBrowserIconService.GotoArrow.CreateImage()
			};
			
			List<IClass> results = new List<IClass>();
			
			SearchAllExtensionMethodsWithName(results, context.ProjectContent, rr.CallName);
			
			foreach (IProjectContent content in context.ProjectContent.ThreadSafeGetReferencedContents())
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
					NamespaceRefactoringService.AddUsingDeclaration(context.CompilationUnit, context.Editor.Document, newNamespace, true);
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
