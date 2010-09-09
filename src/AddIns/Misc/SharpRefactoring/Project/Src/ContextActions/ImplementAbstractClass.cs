// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Editor;
using ICSharpCode.SharpDevelop.Editor.AvalonEdit;
using ICSharpCode.SharpDevelop.Refactoring;

namespace SharpRefactoring.ContextActions
{
	/// <summary>
	/// Description of ImplementAbstractClass.
	/// </summary>
	public class ImplementAbstractClassProvider : ContextActionsProvider
	{
		public override IEnumerable<IContextAction> GetAvailableActions(EditorContext editorContext)
		{
			foreach (var targetClass in editorContext.GetClassDeclarationsOnCurrentLine().Where(c => c.ClassType == ClassType.Class).Select(c2 => c2.GetCurrentClassPart(editorContext.Editor.FileName))) {
				foreach (var implementAction in RefactoringService.GetImplementAbstractClassActions(targetClass)) {
					yield return implementAction;
				}
			}
		}
	}
}
