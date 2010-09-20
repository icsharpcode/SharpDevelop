// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
