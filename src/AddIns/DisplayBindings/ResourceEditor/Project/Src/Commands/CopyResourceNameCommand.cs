// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Windows.Forms;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Gui;

namespace ResourceEditor
{
	class CopyResourceNameCommand : AbstractMenuCommand
	{
		public override void Run()
		{
			ResourceEditorControl editor = ((ResourceEditWrapper)SD.Workbench.ActiveViewContent).ResourceEditor;
			
			if(editor.ResourceList.SelectedItems.Count > 0) {
				Clipboard.SetText(editor.ResourceList.SelectedItems[0].Text);
			}
		}
	}
}
