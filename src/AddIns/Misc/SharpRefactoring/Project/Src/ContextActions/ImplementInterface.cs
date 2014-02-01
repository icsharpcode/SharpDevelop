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
using System.Windows;
using ICSharpCode.NRefactory;
using Ast = ICSharpCode.NRefactory.Ast;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Dom.NRefactoryResolver;
using ICSharpCode.SharpDevelop.Editor;
using ICSharpCode.SharpDevelop.Editor.AvalonEdit;
using ICSharpCode.SharpDevelop.Refactoring;

namespace SharpRefactoring.ContextActions
{
	/// <summary>
	/// Description of ImplementInterface.
	/// </summary>
	public class ImplementInterfaceProvider : ContextActionsProvider
	{
		public override IEnumerable<IContextAction> GetAvailableActions(EditorContext editorContext)
		{
			// Using CurrentLineAST is basically OK, but when the "class" keyword is on different line than class name,
			// parsing only one line never tells us that we are looking at TypeDeclaration
			// Alternative solution could be to try to resolve also IdentifierExpression to see if it is class declaration.
			foreach (var targetClass in editorContext.GetClassDeclarationsOnCurrentLine()
			         .Where(c => c.ClassType == ClassType.Class || c.ClassType == ClassType.Interface)
			         .Select(c2 => c2.GetCurrentClassPart(editorContext.Editor.FileName))) {
				foreach (var implementAction in RefactoringService.GetImplementInterfaceActions(targetClass)) {
					yield return implementAction;
				}
			}
		}
	}
}
