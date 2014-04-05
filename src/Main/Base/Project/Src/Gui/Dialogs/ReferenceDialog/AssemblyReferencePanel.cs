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
using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.Core;

namespace ICSharpCode.SharpDevelop.Gui
{
	public class AssemblyReferencePanel : Panel, IReferencePanel
	{
		ISelectReferenceDialog selectDialog;
		Button browseButton;
		Button browseProjectButton;
		Button browseSolutionButton;
		
		public AssemblyReferencePanel(ISelectReferenceDialog selectDialog)
		{
			this.selectDialog = selectDialog;
			AutoSize = true;
			
			browseButton   = new Button();
			browseButton.Location = new Point(10, 10);
			browseButton.Name = "browseButton"; // requested by HP for UI automation purposes
			browseButton.Size = new Size(120, 23);
			
			browseButton.Text     = StringParser.Parse("${res:Dialog.SelectReferenceDialog.BrowseLastButton}");
			browseButton.Click   += new EventHandler(SelectReferenceDialog);
			browseButton.FlatStyle = FlatStyle.System;
			Controls.Add(browseButton);

			browseProjectButton   = new Button();
			browseProjectButton.Location = new Point(150, 10);
			browseProjectButton.Name = "browseProjectButton"; // requested by HP for UI automation purposes
			browseProjectButton.Size = new Size(120, 23);

			browseProjectButton.Text     = StringParser.Parse("${res:Dialog.SelectReferenceDialog.BrowseProjectButton}");
			browseProjectButton.Click   += new EventHandler(SelectReferenceDialog);
			browseProjectButton.FlatStyle = FlatStyle.System;
			Controls.Add(browseProjectButton);

			browseSolutionButton   = new Button();
			browseSolutionButton.Location = new Point(290, 10);
			browseSolutionButton.Name = "browseSolutionButton"; // requested by HP for UI automation purposes
			browseSolutionButton.Size = new Size(120, 23);

			browseSolutionButton.Text     = StringParser.Parse("${res:Dialog.SelectReferenceDialog.BrowseSolutionButton}");
			browseSolutionButton.Click   += new EventHandler(SelectReferenceDialog);
			browseSolutionButton.FlatStyle = FlatStyle.System;
			Controls.Add(browseSolutionButton);
		}
		
		void SelectReferenceDialog(object sender, EventArgs e)
		{
			using (OpenFileDialog fdiag  = new OpenFileDialog()) {
				fdiag.AddExtension    = true;
				fdiag.InitialDirectory = 
					sender == browseProjectButton ? selectDialog.ConfigureProject.Directory.ToString()
					: sender == browseSolutionButton ? selectDialog.ConfigureProject.ParentSolution.Directory.ToString()
					: "";
				
				fdiag.Filter = StringParser.Parse("${res:SharpDevelop.FileFilter.AssemblyFiles}|*.dll;*.exe|${res:SharpDevelop.FileFilter.AllFiles}|*.*");
				fdiag.Multiselect     = true;
				fdiag.CheckFileExists = true;
				
				if (fdiag.ShowDialog(SD.WinForms.MainWin32Window) == DialogResult.OK) {
					foreach (string file in fdiag.FileNames) {
						ReferenceProjectItem assemblyReference = new ReferenceProjectItem(selectDialog.ConfigureProject);
						assemblyReference.Include = Path.GetFileNameWithoutExtension(file);
						assemblyReference.HintPath = FileUtility.GetRelativePath(selectDialog.ConfigureProject.Directory, file);
						
						selectDialog.AddReference(
							Path.GetFileName(file), "Assembly", file,
							assemblyReference
						);
					}
				}
			}
		}
		
		public void AddReference()
		{
			SelectReferenceDialog(null, null);
		}
	}
}
