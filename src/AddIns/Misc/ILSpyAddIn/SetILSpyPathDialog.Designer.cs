// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

namespace ICSharpCode.ILSpyAddIn
{
	partial class SetILSpyPathDialog
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
			System.Windows.Forms.Button pshOK;
			System.Windows.Forms.Button pshCancel;
			System.Windows.Forms.Label txtILSpyExplanation;
			System.Windows.Forms.GroupBox grpPath;
			System.Windows.Forms.Button pshBrowse;
			this.slePath = new System.Windows.Forms.TextBox();
			this.linkILSpy = new System.Windows.Forms.LinkLabel();
			this.txtReason = new System.Windows.Forms.Label();
			pshOK = new System.Windows.Forms.Button();
			pshCancel = new System.Windows.Forms.Button();
			txtILSpyExplanation = new System.Windows.Forms.Label();
			grpPath = new System.Windows.Forms.GroupBox();
			pshBrowse = new System.Windows.Forms.Button();
			grpPath.SuspendLayout();
			this.SuspendLayout();
			// 
			// pshOK
			// 
			pshOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			pshOK.DialogResult = System.Windows.Forms.DialogResult.OK;
			pshOK.Location = new System.Drawing.Point(272, 187);
			pshOK.Name = "pshOK";
			pshOK.Size = new System.Drawing.Size(75, 23);
			pshOK.TabIndex = 0;
			pshOK.Text = "${res:Global.OKButtonText}";
			pshOK.UseVisualStyleBackColor = true;
			// 
			// pshCancel
			// 
			pshCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			pshCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			pshCancel.Location = new System.Drawing.Point(353, 187);
			pshCancel.Name = "pshCancel";
			pshCancel.Size = new System.Drawing.Size(75, 23);
			pshCancel.TabIndex = 1;
			pshCancel.Text = "${res:Global.CancelButtonText}";
			pshCancel.UseVisualStyleBackColor = true;
			// 
			// txtILSpyExplanation
			// 
			txtILSpyExplanation.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
									| System.Windows.Forms.AnchorStyles.Right)));
			txtILSpyExplanation.Location = new System.Drawing.Point(12, 45);
			txtILSpyExplanation.Name = "txtILSpyExplanation";
			txtILSpyExplanation.Size = new System.Drawing.Size(416, 46);
			txtILSpyExplanation.TabIndex = 3;
			txtILSpyExplanation.Text = "${res:ILSpyAddIn.SetILSpyPathDialog.ILSpyInfo}";
			// 
			// grpPath
			// 
			grpPath.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
									| System.Windows.Forms.AnchorStyles.Right)));
			grpPath.Controls.Add(pshBrowse);
			grpPath.Controls.Add(this.slePath);
			grpPath.Location = new System.Drawing.Point(12, 127);
			grpPath.Name = "grpPath";
			grpPath.Size = new System.Drawing.Size(416, 51);
			grpPath.TabIndex = 5;
			grpPath.TabStop = false;
			grpPath.Text = "${res:ILSpyAddIn.SetILSpyPathDialog.PathToILSpyExe}";
			// 
			// pshBrowse
			// 
			pshBrowse.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			pshBrowse.Location = new System.Drawing.Point(303, 17);
			pshBrowse.Name = "pshBrowse";
			pshBrowse.Size = new System.Drawing.Size(107, 23);
			pshBrowse.TabIndex = 1;
			pshBrowse.Text = "${res:Global.BrowseButtonText}";
			pshBrowse.UseVisualStyleBackColor = true;
			pshBrowse.Click += new System.EventHandler(this.PshBrowseClick);
			// 
			// slePath
			// 
			this.slePath.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
									| System.Windows.Forms.AnchorStyles.Right)));
			this.slePath.Location = new System.Drawing.Point(6, 19);
			this.slePath.Name = "slePath";
			this.slePath.Size = new System.Drawing.Size(291, 20);
			this.slePath.TabIndex = 0;
			// 
			// linkILSpy
			// 
			this.linkILSpy.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
									| System.Windows.Forms.AnchorStyles.Right)));
			this.linkILSpy.Location = new System.Drawing.Point(12, 91);
			this.linkILSpy.Name = "linkILSpy";
			this.linkILSpy.Size = new System.Drawing.Size(416, 23);
			this.linkILSpy.TabIndex = 4;
			this.linkILSpy.TabStop = true;
			this.linkILSpy.Text = "http://www.ilspy.net/";
			this.linkILSpy.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.LinkILSpyLinkClicked);
			// 
			// txtReason
			// 
			this.txtReason.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
									| System.Windows.Forms.AnchorStyles.Right)));
			this.txtReason.Location = new System.Drawing.Point(12, 9);
			this.txtReason.Name = "txtReason";
			this.txtReason.Size = new System.Drawing.Size(416, 36);
			this.txtReason.TabIndex = 2;
			// 
			// SetILSpyPathDialog
			// 
			this.AcceptButton = pshOK;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = pshCancel;
			this.ClientSize = new System.Drawing.Size(440, 222);
			this.Controls.Add(grpPath);
			this.Controls.Add(this.linkILSpy);
			this.Controls.Add(txtILSpyExplanation);
			this.Controls.Add(this.txtReason);
			this.Controls.Add(pshCancel);
			this.Controls.Add(pshOK);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "SetILSpyPathDialog";
			this.ShowIcon = false;
			this.ShowInTaskbar = false;
			this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "${res:ILSpyAddIn.SetILSpyPathDialogTitle}";
			grpPath.ResumeLayout(false);
			grpPath.PerformLayout();
			this.ResumeLayout(false);
		}
		private System.Windows.Forms.LinkLabel linkILSpy;
		private System.Windows.Forms.TextBox slePath;
		private System.Windows.Forms.Label txtReason;
	}
}
