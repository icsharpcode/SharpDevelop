// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
