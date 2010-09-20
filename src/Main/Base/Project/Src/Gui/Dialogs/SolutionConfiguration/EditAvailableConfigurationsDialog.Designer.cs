// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

namespace ICSharpCode.SharpDevelop.Gui
{
	partial class EditAvailableConfigurationsDialog : System.Windows.Forms.Form
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
			this.listBox = new System.Windows.Forms.ListBox();
			this.closeButton = new System.Windows.Forms.Button();
			this.removeButton = new System.Windows.Forms.Button();
			this.renameButton = new System.Windows.Forms.Button();
			this.addButton = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// listBox
			// 
			this.listBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
									| System.Windows.Forms.AnchorStyles.Left) 
									| System.Windows.Forms.AnchorStyles.Right)));
			this.listBox.FormattingEnabled = true;
			this.listBox.IntegralHeight = false;
			this.listBox.Location = new System.Drawing.Point(12, 12);
			this.listBox.Name = "listBox";
			this.listBox.Size = new System.Drawing.Size(204, 93);
			this.listBox.TabIndex = 0;
			// 
			// okButton
			// 
			this.closeButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.closeButton.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.closeButton.Location = new System.Drawing.Point(222, 111);
			this.closeButton.Name = "closeButton";
			this.closeButton.Size = new System.Drawing.Size(75, 23);
			this.closeButton.TabIndex = 1;
			this.closeButton.Text = "${res:Global.CloseButtonText}";
			this.closeButton.UseVisualStyleBackColor = true;
			// 
			// removeButton
			// 
			this.removeButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.removeButton.Location = new System.Drawing.Point(222, 12);
			this.removeButton.Name = "removeButton";
			this.removeButton.Size = new System.Drawing.Size(75, 23);
			this.removeButton.TabIndex = 2;
			this.removeButton.Text = "${res:Global.RemoveButtonText}";
			this.removeButton.UseVisualStyleBackColor = true;
			this.removeButton.Click += new System.EventHandler(this.RemoveButtonClick);
			// 
			// renameButton
			// 
			this.renameButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.renameButton.Location = new System.Drawing.Point(222, 41);
			this.renameButton.Name = "renameButton";
			this.renameButton.Size = new System.Drawing.Size(75, 23);
			this.renameButton.TabIndex = 3;
			this.renameButton.Text = "${res:Global.RenameButtonText}";
			this.renameButton.UseVisualStyleBackColor = true;
			this.renameButton.Click += new System.EventHandler(this.RenameButtonClick);
			// 
			// addButton
			// 
			this.addButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.addButton.Location = new System.Drawing.Point(222, 70);
			this.addButton.Name = "addButton";
			this.addButton.Size = new System.Drawing.Size(75, 23);
			this.addButton.TabIndex = 4;
			this.addButton.Text = "${res:Global.AddButtonText}...";
			this.addButton.UseVisualStyleBackColor = true;
			this.addButton.Click += new System.EventHandler(this.AddButtonClick);
			// 
			// EditAvailableConfigurationsDialog
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(309, 146);
			this.Controls.Add(this.addButton);
			this.Controls.Add(this.renameButton);
			this.Controls.Add(this.removeButton);
			this.Controls.Add(this.closeButton);
			this.Controls.Add(this.listBox);
			this.MinimumSize = new System.Drawing.Size(230, 165);
			this.Name = "EditAvailableConfigurationsDialog";
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "EditAvailableConfigurationsDialog";
			this.ResumeLayout(false);
		}
		private System.Windows.Forms.Button addButton;
		private System.Windows.Forms.Button renameButton;
		private System.Windows.Forms.Button removeButton;
		private System.Windows.Forms.ListBox listBox;
		private System.Windows.Forms.Button closeButton;
	}
}
