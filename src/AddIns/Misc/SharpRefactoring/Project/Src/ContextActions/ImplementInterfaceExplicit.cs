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
	public class ImplementInterfaceExplicitProvider : ContextActionsProvider
	{
		public override IEnumerable<IContextAction> GetAvailableActions(EditorContext editorContext)
		{
			foreach (var targetClass in editorContext.GetClassDeclarationsOnCurrentLine()
			         .Where(c => c.ClassType == ClassType.Class || c.ClassType == ClassType.Interface)
			         .Select(c2 => c2.GetCurrentClassPart(editorContext.Editor.FileName))) {
				foreach (var implementAction in RefactoringService.GetImplementInterfaceActions(targetClass, true)) {
					yield return implementAction;
				}
			}
		}
	}
}
