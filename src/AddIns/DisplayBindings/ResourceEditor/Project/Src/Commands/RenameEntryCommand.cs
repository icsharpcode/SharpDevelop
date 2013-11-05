// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop;

namespace ResourceEditor
{
	class RenameEntryCommand : AbstractMenuCommand
	{
		public override void Run()
		{
			ResourceEditorControl editor = ((ResourceEditWrapper)SD.Workbench.ActiveViewContent).ResourceEditor;
			
			if(editor.ResourceList.SelectedItems.Count != 0) {
				editor.ResourceList.SelectedItems[0].BeginEdit();
			}
		}
	}
}
