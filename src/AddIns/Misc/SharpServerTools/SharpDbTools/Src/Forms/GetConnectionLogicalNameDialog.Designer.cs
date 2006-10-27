// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Dickon Field" email=""/>
//     <version>$Revision$</version>
// </file>

namespace SharpDbTools.Forms
{
	partial class GetConnectionLogicalNameDialog : System.Windows.Forms.Form
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
			this.connectionName = new System.Windows.Forms.TextBox();
			this.connectionNameOKButton = new System.Windows.Forms.Button();
			this.connectionNameCancelButton = new System.Windows.Forms.Button();
			this.label1 = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// connectionName
			// 
			this.connectionName.Location = new System.Drawing.Point(12, 33);
			this.connectionName.MaxLength = 30;
			this.connectionName.Name = "connectionName";
			this.connectionName.Size = new System.Drawing.Size(292, 21);
			this.connectionName.TabIndex = 0;
			// 
			// connectionNameOKButton
			// 
			this.connectionNameOKButton.Location = new System.Drawing.Point(66, 70);
			this.connectionNameOKButton.Name = "connectionNameOKButton";
			this.connectionNameOKButton.Size = new System.Drawing.Size(75, 23);
			this.connectionNameOKButton.TabIndex = 1;
			this.connectionNameOKButton.Text = "OK";
			this.connectionNameOKButton.UseVisualStyleBackColor = true;
			this.connectionNameOKButton.Click += new System.EventHandler(this.ConnectionNameOKButtonClick);
			// 
			// connectionNameCancelButton
			// 
			this.connectionNameCancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.connectionNameCancelButton.Location = new System.Drawing.Point(171, 70);
			this.connectionNameCancelButton.Name = "connectionNameCancelButton";
			this.connectionNameCancelButton.Size = new System.Drawing.Size(75, 23);
			this.connectionNameCancelButton.TabIndex = 2;
			this.connectionNameCancelButton.Text = "Cancel";
			this.connectionNameCancelButton.UseVisualStyleBackColor = true;
			this.connectionNameCancelButton.Click += new System.EventHandler(this.ConnectionNameCancelButtonClick);
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(12, 9);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(259, 23);
			this.label1.TabIndex = 3;
			this.label1.Text = "Please provide the name for your db connection:";
			// 
			// GetConnectionLogicalNameDialog
			// 
			this.AcceptButton = this.connectionNameOKButton;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.connectionNameCancelButton;
			this.ClientSize = new System.Drawing.Size(316, 114);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.connectionNameCancelButton);
			this.Controls.Add(this.connectionNameOKButton);
			this.Controls.Add(this.connectionName);
			this.Name = "GetConnectionLogicalNameDialog";
			this.Text = "Connection Name";
			this.ResumeLayout(false);
			this.PerformLayout();
		}
		private System.Windows.Forms.TextBox connectionName;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Button connectionNameCancelButton;
		private System.Windows.Forms.Button connectionNameOKButton;
	}
}
