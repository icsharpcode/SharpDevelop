// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Christian Hornung" email="chhornung@googlemail.com"/>
//     <version>$Revision$</version>
// </file>
namespace ReflectorAddIn
{
	partial class SetReflectorPathDialog
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
			System.Windows.Forms.Label txtReflectorExplanation;
			System.Windows.Forms.LinkLabel linkReflector;
			System.Windows.Forms.GroupBox grpPath;
			System.Windows.Forms.Button pshBrowse;
			this.slePath = new System.Windows.Forms.TextBox();
			this.txtReason = new System.Windows.Forms.Label();
			pshOK = new System.Windows.Forms.Button();
			pshCancel = new System.Windows.Forms.Button();
			txtReflectorExplanation = new System.Windows.Forms.Label();
			linkReflector = new System.Windows.Forms.LinkLabel();
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
			// txtReflectorExplanation
			// 
			txtReflectorExplanation.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
									| System.Windows.Forms.AnchorStyles.Right)));
			txtReflectorExplanation.Location = new System.Drawing.Point(12, 45);
			txtReflectorExplanation.Name = "txtReflectorExplanation";
			txtReflectorExplanation.Size = new System.Drawing.Size(416, 46);
			txtReflectorExplanation.TabIndex = 3;
			txtReflectorExplanation.Text = "${res:ReflectorAddIn.SetReflectorPathDialog.ReflectorInfo}";
			// 
			// linkReflector
			// 
			linkReflector.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
									| System.Windows.Forms.AnchorStyles.Right)));
			linkReflector.Location = new System.Drawing.Point(12, 91);
			linkReflector.Name = "linkReflector";
			linkReflector.Size = new System.Drawing.Size(416, 23);
			linkReflector.TabIndex = 4;
			linkReflector.TabStop = true;
			linkReflector.Text = "http://www.aisto.com/roeder/dotnet/";
			linkReflector.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.LinkReflectorLinkClicked);
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
			grpPath.Text = "${res:ReflectorAddIn.SetReflectorPathDialog.PathToReflectorExe}";
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
			// txtReason
			// 
			this.txtReason.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
									| System.Windows.Forms.AnchorStyles.Right)));
			this.txtReason.Location = new System.Drawing.Point(12, 9);
			this.txtReason.Name = "txtReason";
			this.txtReason.Size = new System.Drawing.Size(416, 36);
			this.txtReason.TabIndex = 2;
			// 
			// SetReflectorPathDialog
			// 
			this.AcceptButton = pshOK;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = pshCancel;
			this.ClientSize = new System.Drawing.Size(440, 222);
			this.Controls.Add(grpPath);
			this.Controls.Add(linkReflector);
			this.Controls.Add(txtReflectorExplanation);
			this.Controls.Add(this.txtReason);
			this.Controls.Add(pshCancel);
			this.Controls.Add(pshOK);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "SetReflectorPathDialog";
			this.ShowIcon = false;
			this.ShowInTaskbar = false;
			this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			grpPath.ResumeLayout(false);
			grpPath.PerformLayout();
			this.ResumeLayout(false);
		}
		private System.Windows.Forms.TextBox slePath;
		private System.Windows.Forms.Label txtReason;
	}
}
