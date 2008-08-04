// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="none" email=""/>
//     <version>$Revision$</version>
// </file>

using System;
using ICSharpCode.Core;
using ICSharpCode.Core.WinForms;
using ICSharpCode.SharpDevelop.Gui;

namespace ResourceEditor
{
	class CopyResourceNameCommand : AbstractMenuCommand
	{
		public override void Run()
		{
			ResourceEditorControl editor = ((ResourceEditWrapper)WorkbenchSingleton.Workbench.ActiveViewContent).ResourceEditor;
			
			if(editor.ResourceList.SelectedItems.Count > 0) {
				ClipboardWrapper.SetText(editor.ResourceList.SelectedItems[0].Text);
			}
		}
	}
}
