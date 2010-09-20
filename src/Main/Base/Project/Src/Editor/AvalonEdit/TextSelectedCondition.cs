// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Gui;

namespace ICSharpCode.SharpDevelop.Editor.AvalonEdit
{
	public class TextSelectedCondition : IConditionEvaluator
	{
		public bool IsValid(object owner, Condition condition)
		{
			ITextEditorProvider textEditorProvider = WorkbenchSingleton.Workbench.ActiveViewContent as ITextEditorProvider;
			if (textEditorProvider != null) {
				ITextEditor textEditor = textEditorProvider.TextEditor;
				return textEditor.SelectionLength > 0;
			}
			return false;
		}
	}
}
