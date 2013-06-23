// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop;

namespace ResourceEditor
{
	class EditCommentCommand : AbstractMenuCommand
	{
		public override void Run()
		{
			ResourceEditorControl editor = ((ResourceEditWrapper)SD.Workbench.ActiveViewContent).ResourceEditor;
			if (editor.ResourceList.SelectedItems.Count != 0) {
				var item = editor.ResourceList.SelectedItems[0].SubItems[3];
				string resourceName = editor.ResourceList.SelectedItems[0].Text;
				string newValue = SD.MessageService.ShowInputBox("${res:ResourceEditor.ResourceEdit.ContextMenu.EditComment}",
				                                                "${res:ResourceEditor.ResourceEdit.ContextMenu.EditCommentText}",
				                                                item.Text);
				if (newValue != null && newValue != item.Text) {
					editor.ResourceList.SetCommentValue(resourceName, newValue);
				}
			}
		}
	}
}


