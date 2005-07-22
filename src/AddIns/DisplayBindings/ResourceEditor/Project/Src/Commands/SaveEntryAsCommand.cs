// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="none" email=""/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Windows.Forms;
using System.IO;
using System.Drawing;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop;

namespace ResourceEditor
{
	class SaveEntryAsCommand : AbstractMenuCommand
	{
		public override void Run()
		{
			IWorkbenchWindow window = WorkbenchSingleton.Workbench.ActiveWorkbenchWindow;
			ResourceEditorControl editor = (ResourceEditorControl)window.ViewContent.Control;
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
				sdialog.Filter 		= "Bitmap files (*.bmp)|*.bmp";
				sdialog.DefaultExt 	= ".bmp";
			} else if (item.ResourceValue is Icon) {
				sdialog.Filter 		= "Icon files (*.ico)|*.ico";
				sdialog.DefaultExt 	= ".ico";
			} else if (item.ResourceValue is Cursor) {
				sdialog.Filter 		= "Cursor files (*.cur)|*.cur";
				sdialog.DefaultExt 	= ".cur";
			} else if (item.ResourceValue is byte[]){
				sdialog.Filter      = "Binary files (*.*)|*.*";
				sdialog.DefaultExt  = ".bin";
			} else {
				return;
			}
			
			DialogResult dr = sdialog.ShowDialog(ICSharpCode.SharpDevelop.Gui.WorkbenchSingleton.MainForm);
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
