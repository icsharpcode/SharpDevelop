// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Gui;

namespace ResourceEditor
{
	class SaveEntryAsCommand : AbstractMenuCommand
	{
		public override void Run()
		{
			ResourceEditorControl editor = ((ResourceEditWrapper)SD.Workbench.ActiveViewContent).ResourceEditor;
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
			
			DialogResult dr = sdialog.ShowDialog(SD.WinForms.MainWin32Window);
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
