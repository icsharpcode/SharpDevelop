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
using ICSharpCode.SharpDevelop.Gui.OptionPanels;

namespace ICSharpCode.SharpDevelop.Workbench
{
	/// <summary>
	/// This dialog is used to add a new program to the open with dialog.
	/// </summary>
	partial class AddOpenWithEntryDialog : Form
	{
		public AddOpenWithEntryDialog()
		{
			//
			// The InitializeComponent() call is required for Windows Forms designer support.
			//
			InitializeComponent();
			
			this.DialogResult = DialogResult.Cancel;
			
			foreach (Control ctl in this.Controls) {
				ctl.Text = StringParser.Parse(ctl.Text);
			}
			this.Text = StringParser.Parse(this.Text);
		}
		
		public string ProgramName {
			get { return programNameTextBox.Text; }
		}
		
		public string DisplayName {
			get { return displayNameTextBox.Text; }
		}
		
		bool userEditedDisplayName;
		
		void BrowseForProgramButtonClick(object sender, EventArgs e)
		{
			using (OpenFileDialog dlg = new OpenFileDialog()) {
				dlg.Filter = StringParser.Parse(ExternalToolPanel.ExecutableFilesFilter);
				if (dlg.ShowDialog(this) == DialogResult.OK) {
					programNameTextBox.Text = "\"" + dlg.FileName + "\"";
					if (!userEditedDisplayName) {
						displayNameTextBox.Text = Path.GetFileName(dlg.FileName);
					}
				}
			}
		}
		
		void ProgramNameTextBoxTextChanged(object sender, EventArgs e)
		{
			SetOkButtonEnabled();
		}
		
		void DisplayNameTextBoxTextChanged(object sender, EventArgs e)
		{
			userEditedDisplayName = true;
			SetOkButtonEnabled();
		}
		
		void SetOkButtonEnabled()
		{
			okButton.Enabled = !string.IsNullOrEmpty(programNameTextBox.Text)
				&& !string.IsNullOrEmpty(displayNameTextBox.Text);
		}
	}
}
