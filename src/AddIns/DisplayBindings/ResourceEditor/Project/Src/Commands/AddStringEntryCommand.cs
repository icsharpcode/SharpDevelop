// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="none" email=""/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Windows.Forms;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Gui;

namespace ResourceEditor
{
	class AddStringCommand : AbstractMenuCommand
	{
		public override void Run()
		{
			ResourceEditorControl editor = ((ResourceEditWrapper)WorkbenchSingleton.Workbench.ActiveViewContent).ResourceEditor;
			
			if(editor.ResourceList.WriteProtected) {
				return;
			}
			
			int count = 1;
			string newNameBase = " new string entry ";
			string newName = newNameBase + count.ToString();
			string type = "System.String";
			
			while(editor.ResourceList.Resources.ContainsKey(newName)) {
				count++;
				newName = newNameBase + count.ToString();
			}
			
			ResourceItem item = new ResourceItem(newName, "");
			editor.ResourceList.Resources.Add(newName, item);
			ListViewItem lv = new ListViewItem(new string[] { newName, type, "" }, item.ImageIndex);
			editor.ResourceList.Items.Add(lv);
			editor.ResourceList.OnChanged();
			lv.BeginEdit();
		}
	}
}
