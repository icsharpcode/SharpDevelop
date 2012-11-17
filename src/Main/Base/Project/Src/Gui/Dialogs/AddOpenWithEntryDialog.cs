// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Drawing;
using System.Windows.Forms;
using System.IO;
using ICSharpCode.Core;

namespace ICSharpCode.SharpDevelop.Gui
{
	/// <summary>
	/// This dialog is used to add a new program to the open with dialog.
	/// </summary>
	public partial class AddOpenWithEntryDialog : Form
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
				dlg.Filter = StringParser.Parse(OptionPanels.ExternalToolPanel.ExecutableFilesFilter);
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
