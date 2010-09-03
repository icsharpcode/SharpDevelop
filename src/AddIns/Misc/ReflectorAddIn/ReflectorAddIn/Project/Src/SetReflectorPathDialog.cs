// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Drawing;
using System.Windows.Forms;

using ICSharpCode.Core;

namespace ReflectorAddIn
{
	/// <summary>
	/// Asks the user for the path to Reflector.exe.
	/// </summary>
	internal partial class SetReflectorPathDialog : Form
	{
		public SetReflectorPathDialog(string oldPath, string reason)
		{
			//
			// The InitializeComponent() call is required for Windows Forms designer support.
			//
			InitializeComponent();
			
			this.Text = ResourceService.GetString("ReflectorAddIn.SetReflectorPathDialogTitle");
			
			Translate(this);
			
			this.txtReason.Text = reason;
			if (!String.IsNullOrEmpty(oldPath)) {
				this.slePath.Text = oldPath;
			}
		}
		
		private static void Translate(Control container) {
			container.Text = StringParser.Parse(container.Text);
			foreach (Control c in container.Controls) {
				Translate(c);
			}
		}
		
		void LinkReflectorLinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			try {
				using(System.Diagnostics.Process.Start("http://www.aisto.com/roeder/dotnet/")) {
				}
			} catch (Exception ex) {
				MessageService.ShowError(ex.Message);
			}
		}
		
		void PshBrowseClick(object sender, EventArgs e)
		{
			using(OpenFileDialog dialog = new OpenFileDialog()) {
				
				dialog.Title = this.Text;
				dialog.DefaultExt = "exe";
				dialog.RestoreDirectory = true;
				dialog.Filter = "Reflector.exe|Reflector.exe";
				
				if (!String.IsNullOrEmpty(this.slePath.Text)) {
					dialog.FileName = this.slePath.Text;
				}
				
				if (dialog.ShowDialog(this) == DialogResult.OK) {
					this.slePath.Text = dialog.FileName;
				}
				
			}
		}
		
		public string SelectedFile {
			get { return this.slePath.Text; }
		}
	}
}
