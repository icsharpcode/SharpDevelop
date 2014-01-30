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
using System.Windows.Forms;

using ICSharpCode.Core;

namespace ICSharpCode.ILSpyAddIn
{
	/// <summary>
	/// Asks the user for the path to ILSpy.exe.
	/// </summary>
	internal partial class SetILSpyPathDialog : Form
	{
		public SetILSpyPathDialog(string oldPath, string reason)
		{
			//
			// The InitializeComponent() call is required for Windows Forms designer support.
			//
			InitializeComponent();
			
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
		
		void LinkILSpyLinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			try {
				using(System.Diagnostics.Process.Start(linkILSpy.Text)) {
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
				dialog.Filter = "ILSpy.exe|ILSpy.exe";
				
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
