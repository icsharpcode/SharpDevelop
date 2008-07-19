/*
 * Created by SharpDevelop.
 * User: alperd1
 * Date: 1/31/2008
 * Time: 4:26 AM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
namespace ICSharpCode.SharpDevelop.Gui
{
	partial class ExtractInterfaceDialog
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
			this.lblInterfaceName = new System.Windows.Forms.Label();
			this.txtInterfaceName = new System.Windows.Forms.TextBox();
			this.lblGeneratedName = new System.Windows.Forms.Label();
			this.txtGeneratedName = new System.Windows.Forms.TextBox();
			this.lblNewFileName = new System.Windows.Forms.Label();
			this.txtNewFileName = new System.Windows.Forms.TextBox();
			this.btnSelectAll = new System.Windows.Forms.Button();
			this.selectMembersListBox = new System.Windows.Forms.CheckedListBox();
			this.specifyInterfaceGroupBox = new System.Windows.Forms.GroupBox();
			this.cbIncludeComments = new System.Windows.Forms.CheckBox();
			this.btnDeselectAll = new System.Windows.Forms.Button();
			this.btnCancel = new System.Windows.Forms.Button();
			this.btnOK = new System.Windows.Forms.Button();
			this.groupOptions = new System.Windows.Forms.GroupBox();
			this.cbAddToClass = new System.Windows.Forms.CheckBox();
			this.specifyInterfaceGroupBox.SuspendLayout();
			this.groupOptions.SuspendLayout();
			this.SuspendLayout();
			// 
			// lblInterfaceName
			// 
			this.lblInterfaceName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
									| System.Windows.Forms.AnchorStyles.Right)));
			this.lblInterfaceName.Location = new System.Drawing.Point(12, 9);
			this.lblInterfaceName.Name = "lblInterfaceName";
			this.lblInterfaceName.Size = new System.Drawing.Size(283, 23);
			this.lblInterfaceName.TabIndex = 0;
			this.lblInterfaceName.Text = "${res:Dialog.Refactoring.ExtractInterface.InterfaceName}:";
			this.lblInterfaceName.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			// 
			// txtInterfaceName
			// 
			this.txtInterfaceName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
									| System.Windows.Forms.AnchorStyles.Right)));
			this.txtInterfaceName.Location = new System.Drawing.Point(15, 35);
			this.txtInterfaceName.Name = "txtInterfaceName";
			this.txtInterfaceName.Size = new System.Drawing.Size(412, 20);
			this.txtInterfaceName.TabIndex = 6;
			this.txtInterfaceName.TextChanged += new System.EventHandler(this.TxtInterfaceNameTextChanged);
			// 
			// lblGeneratedName
			// 
			this.lblGeneratedName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
									| System.Windows.Forms.AnchorStyles.Right)));
			this.lblGeneratedName.Location = new System.Drawing.Point(12, 58);
			this.lblGeneratedName.Name = "lblGeneratedName";
			this.lblGeneratedName.Size = new System.Drawing.Size(289, 23);
			this.lblGeneratedName.TabIndex = 2;
			this.lblGeneratedName.Text = "${res:Dialog.Refactoring.ExtractInterface.GeneratedName}:";
			this.lblGeneratedName.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			// 
			// txtGeneratedName
			// 
			this.txtGeneratedName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
									| System.Windows.Forms.AnchorStyles.Right)));
			this.txtGeneratedName.Enabled = false;
			this.txtGeneratedName.Location = new System.Drawing.Point(15, 84);
			this.txtGeneratedName.Name = "txtGeneratedName";
			this.txtGeneratedName.Size = new System.Drawing.Size(412, 20);
			this.txtGeneratedName.TabIndex = 7;
			// 
			// lblNewFileName
			// 
			this.lblNewFileName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
									| System.Windows.Forms.AnchorStyles.Right)));
			this.lblNewFileName.Location = new System.Drawing.Point(12, 107);
			this.lblNewFileName.Name = "lblNewFileName";
			this.lblNewFileName.Size = new System.Drawing.Size(286, 23);
			this.lblNewFileName.TabIndex = 1;
			this.lblNewFileName.Text = "${res:Dialog.Refactoring.ExtractInterface.NewFileName}:";
			this.lblNewFileName.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			// 
			// txtNewFileName
			// 
			this.txtNewFileName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
									| System.Windows.Forms.AnchorStyles.Right)));
			this.txtNewFileName.Location = new System.Drawing.Point(15, 133);
			this.txtNewFileName.Name = "txtNewFileName";
			this.txtNewFileName.Size = new System.Drawing.Size(412, 20);
			this.txtNewFileName.TabIndex = 8;
			this.txtNewFileName.TextChanged += new System.EventHandler(this.TxtNewFileNameTextChanged);
			this.txtNewFileName.KeyDown += new System.Windows.Forms.KeyEventHandler(this.TxtNewFileNameKeyDown);
			// 
			// btnSelectAll
			// 
			this.btnSelectAll.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.btnSelectAll.Location = new System.Drawing.Point(321, 16);
			this.btnSelectAll.Name = "btnSelectAll";
			this.btnSelectAll.Size = new System.Drawing.Size(94, 23);
			this.btnSelectAll.TabIndex = 4;
			this.btnSelectAll.Text = "${res:Global.SelectAllButtonText}";
			this.btnSelectAll.UseVisualStyleBackColor = true;
			this.btnSelectAll.Click += new System.EventHandler(this.BtnSelectAllClick);
			// 
			// selectMembersListBox
			// 
			this.selectMembersListBox.CheckOnClick = true;
			this.selectMembersListBox.Dock = System.Windows.Forms.DockStyle.Left;
			this.selectMembersListBox.FormattingEnabled = true;
			this.selectMembersListBox.HorizontalScrollbar = true;
			this.selectMembersListBox.Location = new System.Drawing.Point(3, 16);
			this.selectMembersListBox.Name = "selectMembersListBox";
			this.selectMembersListBox.Size = new System.Drawing.Size(312, 154);
			this.selectMembersListBox.TabIndex = 0;
			this.selectMembersListBox.ThreeDCheckBoxes = true;
			// 
			// specifyInterfaceGroupBox
			// 
			this.specifyInterfaceGroupBox.Controls.Add(this.btnDeselectAll);
			this.specifyInterfaceGroupBox.Controls.Add(this.btnSelectAll);
			this.specifyInterfaceGroupBox.Controls.Add(this.selectMembersListBox);
			this.specifyInterfaceGroupBox.Location = new System.Drawing.Point(12, 231);
			this.specifyInterfaceGroupBox.Name = "specifyInterfaceGroupBox";
			this.specifyInterfaceGroupBox.Size = new System.Drawing.Size(421, 173);
			this.specifyInterfaceGroupBox.TabIndex = 3;
			this.specifyInterfaceGroupBox.TabStop = false;
			this.specifyInterfaceGroupBox.Text = "${res:Dialog.Refactoring.ExtractInterface.SelectPublicMembers}";
			// 
			// cbIncludeComments
			// 
			this.cbIncludeComments.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
									| System.Windows.Forms.AnchorStyles.Right)));
			this.cbIncludeComments.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.cbIncludeComments.Location = new System.Drawing.Point(6, 19);
			this.cbIncludeComments.Name = "cbIncludeComments";
			this.cbIncludeComments.Size = new System.Drawing.Size(410, 17);
			this.cbIncludeComments.TabIndex = 11;
			this.cbIncludeComments.Text = "${res:Dialog.Refactoring.ExtractInterface.IncludeComments}";
			this.cbIncludeComments.UseVisualStyleBackColor = true;
			// 
			// btnDeselectAll
			// 
			this.btnDeselectAll.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.btnDeselectAll.Location = new System.Drawing.Point(321, 45);
			this.btnDeselectAll.Name = "btnDeselectAll";
			this.btnDeselectAll.Size = new System.Drawing.Size(94, 23);
			this.btnDeselectAll.TabIndex = 5;
			this.btnDeselectAll.Text = "${res:Global.DeselectAllButtonText}";
			this.btnDeselectAll.UseVisualStyleBackColor = true;
			this.btnDeselectAll.Click += new System.EventHandler(this.BtnDeselectAllClick);
			// 
			// btnCancel
			// 
			this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.btnCancel.Location = new System.Drawing.Point(352, 414);
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.Size = new System.Drawing.Size(75, 23);
			this.btnCancel.TabIndex = 9;
			this.btnCancel.Text = "${res:Global.CancelButtonText}";
			this.btnCancel.UseVisualStyleBackColor = true;
			this.btnCancel.Click += new System.EventHandler(this.BtnCancelClick);
			// 
			// btnOK
			// 
			this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.btnOK.Location = new System.Drawing.Point(271, 414);
			this.btnOK.Name = "btnOK";
			this.btnOK.Size = new System.Drawing.Size(75, 23);
			this.btnOK.TabIndex = 10;
			this.btnOK.Text = "${res:Global.OKButtonText}";
			this.btnOK.UseVisualStyleBackColor = true;
			this.btnOK.Click += new System.EventHandler(this.BtnOKClick);
			// 
			// groupOptions
			// 
			this.groupOptions.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
									| System.Windows.Forms.AnchorStyles.Right)));
			this.groupOptions.Controls.Add(this.cbAddToClass);
			this.groupOptions.Controls.Add(this.cbIncludeComments);
			this.groupOptions.Location = new System.Drawing.Point(12, 159);
			this.groupOptions.Name = "groupOptions";
			this.groupOptions.Size = new System.Drawing.Size(421, 66);
			this.groupOptions.TabIndex = 11;
			this.groupOptions.TabStop = false;
			this.groupOptions.Text = "${res:Global.OptionsLabelText}";
			// 
			// cbAddToClass
			// 
			this.cbAddToClass.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
									| System.Windows.Forms.AnchorStyles.Right)));
			this.cbAddToClass.Checked = true;
			this.cbAddToClass.CheckState = System.Windows.Forms.CheckState.Checked;
			this.cbAddToClass.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.cbAddToClass.Location = new System.Drawing.Point(6, 42);
			this.cbAddToClass.Name = "cbAddToClass";
			this.cbAddToClass.Size = new System.Drawing.Size(410, 17);
			this.cbAddToClass.TabIndex = 12;
			this.cbAddToClass.Text = "${res:Dialog.Refactoring.ExtractInterface.AddInterfaceToClass}";
			this.cbAddToClass.UseVisualStyleBackColor = true;
			// 
			// ExtractInterfaceDialog
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.AutoSize = true;
			this.ClientSize = new System.Drawing.Size(445, 449);
			this.Controls.Add(this.groupOptions);
			this.Controls.Add(this.btnOK);
			this.Controls.Add(this.lblInterfaceName);
			this.Controls.Add(this.btnCancel);
			this.Controls.Add(this.txtInterfaceName);
			this.Controls.Add(this.txtNewFileName);
			this.Controls.Add(this.specifyInterfaceGroupBox);
			this.Controls.Add(this.lblGeneratedName);
			this.Controls.Add(this.txtGeneratedName);
			this.Controls.Add(this.lblNewFileName);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.HelpButton = true;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "ExtractInterfaceDialog";
			this.ShowIcon = false;
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "${res:SharpDevelop.Refactoring.ExtractInterfaceCommand}";
			this.specifyInterfaceGroupBox.ResumeLayout(false);
			this.groupOptions.ResumeLayout(false);
			this.ResumeLayout(false);
			this.PerformLayout();
		}
		private System.Windows.Forms.CheckBox cbAddToClass;
		private System.Windows.Forms.GroupBox groupOptions;
		private System.Windows.Forms.CheckBox cbIncludeComments;
		private System.Windows.Forms.Button btnOK;
		private System.Windows.Forms.Button btnCancel;
		private System.Windows.Forms.Button btnDeselectAll;
		private System.Windows.Forms.GroupBox specifyInterfaceGroupBox;
		private System.Windows.Forms.CheckedListBox selectMembersListBox;
		private System.Windows.Forms.Button btnSelectAll;
		private System.Windows.Forms.TextBox txtNewFileName;
		private System.Windows.Forms.Label lblNewFileName;
		private System.Windows.Forms.TextBox txtGeneratedName;
		private System.Windows.Forms.Label lblGeneratedName;
		private System.Windows.Forms.TextBox txtInterfaceName;
		private System.Windows.Forms.Label lblInterfaceName;
	}
}
