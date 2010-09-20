// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

namespace ICSharpCode.SharpDevelop.Gui
{
	partial class AddOpenWithEntryDialog
	{
		/// <summary>
		/// Designer variable used to keep track of non-visual components.
		/// </summary>
		private System.ComponentModel.IContainer components = null;
		
		/// <summary>
		/// Disposes resources used by the form.
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
			this.label1 = new System.Windows.Forms.Label();
			this.programNameTextBox = new System.Windows.Forms.TextBox();
			this.browseForProgramButton = new System.Windows.Forms.Button();
			this.label2 = new System.Windows.Forms.Label();
			this.displayNameTextBox = new System.Windows.Forms.TextBox();
			this.okButton = new System.Windows.Forms.Button();
			this.cancelButton = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(12, 18);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(100, 23);
			this.label1.TabIndex = 0;
			this.label1.Text = "${res:Gui.ProjectBrowser.OpenWith.AddProgram.Program}";
			// 
			// programNameTextBox
			// 
			this.programNameTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
									| System.Windows.Forms.AnchorStyles.Right)));
			this.programNameTextBox.Location = new System.Drawing.Point(118, 15);
			this.programNameTextBox.Name = "programNameTextBox";
			this.programNameTextBox.Size = new System.Drawing.Size(250, 20);
			this.programNameTextBox.TabIndex = 1;
			this.programNameTextBox.TextChanged += new System.EventHandler(this.ProgramNameTextBoxTextChanged);
			// 
			// browseForProgramButton
			// 
			this.browseForProgramButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.browseForProgramButton.Location = new System.Drawing.Point(374, 13);
			this.browseForProgramButton.Name = "browseForProgramButton";
			this.browseForProgramButton.Size = new System.Drawing.Size(28, 23);
			this.browseForProgramButton.TabIndex = 2;
			this.browseForProgramButton.Text = "...";
			this.browseForProgramButton.UseVisualStyleBackColor = true;
			this.browseForProgramButton.Click += new System.EventHandler(this.BrowseForProgramButtonClick);
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(12, 50);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(100, 23);
			this.label2.TabIndex = 3;
			this.label2.Text = "${res:Gui.ProjectBrowser.OpenWith.AddProgram.DisplayName}";
			// 
			// displayNameTextBox
			// 
			this.displayNameTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
									| System.Windows.Forms.AnchorStyles.Right)));
			this.displayNameTextBox.Location = new System.Drawing.Point(118, 47);
			this.displayNameTextBox.Name = "displayNameTextBox";
			this.displayNameTextBox.Size = new System.Drawing.Size(284, 20);
			this.displayNameTextBox.TabIndex = 4;
			this.displayNameTextBox.TextChanged += new System.EventHandler(this.DisplayNameTextBoxTextChanged);
			// 
			// okButton
			// 
			this.okButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.okButton.Enabled = false;
			this.okButton.Location = new System.Drawing.Point(246, 73);
			this.okButton.Name = "okButton";
			this.okButton.Size = new System.Drawing.Size(75, 23);
			this.okButton.TabIndex = 5;
			this.okButton.Text = "${res:Global.OKButtonText}";
			this.okButton.UseVisualStyleBackColor = true;
			// 
			// cancelButton
			// 
			this.cancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.cancelButton.Location = new System.Drawing.Point(327, 73);
			this.cancelButton.Name = "cancelButton";
			this.cancelButton.Size = new System.Drawing.Size(75, 23);
			this.cancelButton.TabIndex = 6;
			this.cancelButton.Text = "${res:Global.CancelButtonText}";
			this.cancelButton.UseVisualStyleBackColor = true;
			// 
			// AddOpenWithEntryDialog
			// 
			this.AcceptButton = this.okButton;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.cancelButton;
			this.ClientSize = new System.Drawing.Size(414, 106);
			this.Controls.Add(this.cancelButton);
			this.Controls.Add(this.okButton);
			this.Controls.Add(this.displayNameTextBox);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.browseForProgramButton);
			this.Controls.Add(this.programNameTextBox);
			this.Controls.Add(this.label1);
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.MinimumSize = new System.Drawing.Size(225, 140);
			this.Name = "AddOpenWithEntryDialog";
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "${res:Gui.ProjectBrowser.OpenWith.AddProgram.DialogTitle}";
			this.ResumeLayout(false);
			this.PerformLayout();
		}
		private System.Windows.Forms.Button cancelButton;
		private System.Windows.Forms.Button okButton;
		private System.Windows.Forms.TextBox displayNameTextBox;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Button browseForProgramButton;
		private System.Windows.Forms.TextBox programNameTextBox;
		private System.Windows.Forms.Label label1;
	}
}
