// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.IO;
using System.Text;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using MSjogren.GacTool.FusionNative;
using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.Core;

namespace ICSharpCode.SharpDevelop.Gui
{
	public class AssemblyReferencePanel : Panel, IReferencePanel
	{
		SelectReferenceDialog selectDialog;
		
		public AssemblyReferencePanel(SelectReferenceDialog selectDialog)
		{
			this.selectDialog = selectDialog;
			
			Button browseButton   = new Button();
			browseButton.Location = new Point(10, 10);
			
			browseButton.Text     = StringParser.Parse("${res:Global.BrowseButtonText}");
			browseButton.Click   += new EventHandler(SelectReferenceDialog);
			browseButton.FlatStyle = FlatStyle.System;
			Controls.Add(browseButton);
		}
		
		void SelectReferenceDialog(object sender, EventArgs e)
		{
			using (OpenFileDialog fdiag  = new OpenFileDialog()) {
				fdiag.AddExtension    = true;
				
				fdiag.Filter = StringParser.Parse("${res:SharpDevelop.FileFilter.AssemblyFiles}|*.dll;*.exe|${res:SharpDevelop.FileFilter.AllFiles}|*.*");
				fdiag.Multiselect     = true;
				fdiag.CheckFileExists = true;
				
				if (fdiag.ShowDialog(ICSharpCode.SharpDevelop.Gui.WorkbenchSingleton.MainForm) == DialogResult.OK) {
					foreach (string file in fdiag.FileNames) {
						selectDialog.AddReference(ReferenceType.Assembly,
						                          Path.GetFileName(file),
						                          file,
						                          null);
						
					}
				}
			}
		}
		
		public void AddReference(object sender, EventArgs e)
		{
			MessageBox.Show("This panel will contain a file browser, but so long use the browse button :)");
		}
	}
}
