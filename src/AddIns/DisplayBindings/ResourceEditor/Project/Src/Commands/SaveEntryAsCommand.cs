// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Gui;

namespace ResourceEditor
{
	class SaveEntryAsCommand : AbstractMenuCommand
	{
		public override void Run()
		{
			ResourceEditorControl editor = ((ResourceEditWrapper)WorkbenchSingleton.Workbench.ActiveViewContent).ResourceEditor;
			ResourceList list = editor.ResourceList;
			
			if(list.SelectedItems.Count != 1) {
				return;
			}
			
			string key = list.SelectedItems[0].Text;
			if(! list.Resources.ContainsKey(key)) {
				return;
			}
			
			ResourceItem item = list.Resources[key];
			SaveFileDialog sdialog 	= new SaveFileDialog();
			sdialog.AddExtension = true;
			sdialog.FileName = key;
			
			if (item.ResourceValue is Bitmap) {
				sdialog.Filter 		= StringParser.Parse("${res:SharpDevelop.FileFilter.ImageFiles} (*.png)|*.png");
				sdialog.DefaultExt 	= ".png";
			} else if (item.ResourceValue is Icon) {
				sdialog.Filter 		= StringParser.Parse("${res:SharpDevelop.FileFilter.Icons}|*.ico");
				sdialog.DefaultExt 	= ".ico";
			} else if (item.ResourceValue is Cursor) {
				sdialog.Filter 		= StringParser.Parse("${res:SharpDevelop.FileFilter.CursorFiles} (*.cur)|*.cur");
				sdialog.DefaultExt 	= ".cur";
			} else if (item.ResourceValue is byte[]){
				sdialog.Filter      = StringParser.Parse("${res:SharpDevelop.FileFilter.BinaryFiles} (*.*)|*.*");
				sdialog.DefaultExt  = ".bin";
			} else {
				return;
			}
			
			DialogResult dr = sdialog.ShowDialog(ICSharpCode.SharpDevelop.Gui.WorkbenchSingleton.MainWin32Window);
			sdialog.Dispose();
			if (dr != DialogResult.OK) {
				return;
			}
			
			try {
				if (item.ResourceValue is Icon) {
					FileStream fstr = new FileStream(sdialog.FileName, FileMode.Create);
					((Icon)item.ResourceValue).Save(fstr);
					fstr.Close();
				} else if(item.ResourceValue is Image) {
					Image img = (Image)item.ResourceValue;
					img.Save(sdialog.FileName);
				} else {
					FileStream fstr = new FileStream(sdialog.FileName, FileMode.Create);
					BinaryWriter wr = new BinaryWriter(fstr);
					wr.Write((byte[])item.ResourceValue);
					fstr.Close();
				}
			} catch(Exception ex) {
				MessageBox.Show(ex.Message, "Can't save resource to " + sdialog.FileName, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
			}
		}
	}
}
