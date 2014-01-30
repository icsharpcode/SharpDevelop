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

namespace ICSharpCode.FiletypeRegisterer
{
	partial class RegisterFiletypesPanel
	{
		/// <summary>
		/// Designer variable used to keep track of non-visual components.
		/// </summary>
		private System.ComponentModel.IContainer components = null;
		
		/// <summary>
		/// Disposes resources used by the control.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing) {
				if (components != null) {
					components.Dispose();
				}
			}
			base.Dispose(disposing);
		}
		
		/// <summary>
		/// This method is required for Windows Forms designer support.
		/// Do not change the method contents inside the source code editor. The Forms designer might
		/// not be able to load this method if it was changed manually.
		/// </summary>
		private void InitializeComponent()
		{
			this.captionLabel = new System.Windows.Forms.Label();
			this.fileTypesListBox = new System.Windows.Forms.CheckedListBox();
			this.SuspendLayout();
			// 
			// captionLabel
			// 
			this.captionLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
									| System.Windows.Forms.AnchorStyles.Right)));
			this.captionLabel.Location = new System.Drawing.Point(3, 0);
			this.captionLabel.Name = "captionLabel";
			this.captionLabel.Size = new System.Drawing.Size(328, 23);
			this.captionLabel.TabIndex = 0;
			this.captionLabel.Text = "${res:ICSharpCode.SharpDevelop.Gui.Dialogs.OptionPanels.RegisterFiletypesPanel.Ca" +
			"ptionLabel}";
			// 
			// fileTypesListBox
			// 
			this.fileTypesListBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
									| System.Windows.Forms.AnchorStyles.Left) 
									| System.Windows.Forms.AnchorStyles.Right)));
			this.fileTypesListBox.IntegralHeight = false;
			this.fileTypesListBox.Location = new System.Drawing.Point(3, 26);
			this.fileTypesListBox.Name = "fileTypesListBox";
			this.fileTypesListBox.Size = new System.Drawing.Size(328, 299);
			this.fileTypesListBox.TabIndex = 1;
			// 
			// RegisterFiletypesPanel
			// 
			this.Controls.Add(this.fileTypesListBox);
			this.Controls.Add(this.captionLabel);
			this.Name = "RegisterFiletypesPanel";
			this.Size = new System.Drawing.Size(334, 328);
			this.ResumeLayout(false);
		}
		private System.Windows.Forms.CheckedListBox fileTypesListBox;
		private System.Windows.Forms.Label captionLabel;
	}
}
