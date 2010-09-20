// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

namespace ICSharpCode.SharpDevelop.Gui
{
	partial class SolutionConfigurationEditor : System.Windows.Forms.Form
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
			this.panel1 = new System.Windows.Forms.Panel();
			this.platformComboBox = new System.Windows.Forms.ComboBox();
			this.label2 = new System.Windows.Forms.Label();
			this.configurationComboBox = new System.Windows.Forms.ComboBox();
			this.label1 = new System.Windows.Forms.Label();
			this.grid = new System.Windows.Forms.DataGridView();
			this.projectNameColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.configurationColumn = new System.Windows.Forms.DataGridViewComboBoxColumn();
			this.platformColumn = new System.Windows.Forms.DataGridViewComboBoxColumn();
			this.panel2 = new System.Windows.Forms.Panel();
			this.closeButton = new System.Windows.Forms.Button();
			this.panel1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.grid)).BeginInit();
			this.panel2.SuspendLayout();
			this.SuspendLayout();
			// 
			// panel1
			// 
			this.panel1.Controls.Add(this.platformComboBox);
			this.panel1.Controls.Add(this.label2);
			this.panel1.Controls.Add(this.configurationComboBox);
			this.panel1.Controls.Add(this.label1);
			this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
			this.panel1.Location = new System.Drawing.Point(0, 0);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(504, 37);
			this.panel1.TabIndex = 0;
			// 
			// platformComboBox
			// 
			this.platformComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.platformComboBox.FormattingEnabled = true;
			this.platformComboBox.Location = new System.Drawing.Point(326, 6);
			this.platformComboBox.Name = "platformComboBox";
			this.platformComboBox.Size = new System.Drawing.Size(121, 21);
			this.platformComboBox.TabIndex = 3;
			this.platformComboBox.SelectedIndexChanged += new System.EventHandler(this.PlatformComboBoxSelectedIndexChanged);
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(265, 9);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(64, 23);
			this.label2.TabIndex = 2;
			this.label2.Text = "${res:Dialog.ProjectOptions.Platform}:";
			this.label2.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// configurationComboBox
			// 
			this.configurationComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.configurationComboBox.FormattingEnabled = true;
			this.configurationComboBox.Location = new System.Drawing.Point(138, 6);
			this.configurationComboBox.Name = "configurationComboBox";
			this.configurationComboBox.Size = new System.Drawing.Size(121, 21);
			this.configurationComboBox.TabIndex = 1;
			this.configurationComboBox.SelectedIndexChanged += new System.EventHandler(this.ConfigurationComboBoxSelectedIndexChanged);
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(3, 9);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(138, 23);
			this.label1.TabIndex = 0;
			this.label1.Text = "${res:Dialog.Options.CombineOptions.Configurations.SolutionConfiguration}";
			this.label1.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// grid
			// 
			this.grid.AllowUserToAddRows = false;
			this.grid.AllowUserToDeleteRows = false;
			this.grid.AllowUserToResizeRows = false;
			this.grid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this.grid.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
									this.projectNameColumn,
									this.configurationColumn,
									this.platformColumn});
			this.grid.Dock = System.Windows.Forms.DockStyle.Fill;
			this.grid.EditMode = System.Windows.Forms.DataGridViewEditMode.EditOnEnter;
			this.grid.Location = new System.Drawing.Point(0, 37);
			this.grid.Name = "grid";
			this.grid.Size = new System.Drawing.Size(504, 192);
			this.grid.TabIndex = 1;
			this.grid.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.GridCellValueChanged);
			this.grid.EditingControlShowing += new System.Windows.Forms.DataGridViewEditingControlShowingEventHandler(this.GridEditingControlShowing);
			this.grid.DataError += new System.Windows.Forms.DataGridViewDataErrorEventHandler(this.GridDataError);
			// 
			// projectNameColumn
			// 
			this.projectNameColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
			this.projectNameColumn.HeaderText = "${res:Dialog.SelectReferenceDialog.ProjectReferencePanel.NameHeader}";
			this.projectNameColumn.Name = "projectNameColumn";
			this.projectNameColumn.ReadOnly = true;
			// 
			// configurationColumn
			// 
			this.configurationColumn.HeaderText = "${res:Dialog.Options.CombineOptions.Configurations.ConfigurationColumnHeader}";
			this.configurationColumn.Name = "configurationColumn";
			// 
			// platformColumn
			// 
			this.platformColumn.HeaderText = "${res:Dialog.Options.CombineOptions.Configurations.PlatformColumnHeader}";
			this.platformColumn.Name = "platformColumn";
			// 
			// panel2
			// 
			this.panel2.Controls.Add(this.closeButton);
			this.panel2.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.panel2.Location = new System.Drawing.Point(0, 229);
			this.panel2.Name = "panel2";
			this.panel2.Size = new System.Drawing.Size(504, 30);
			this.panel2.TabIndex = 2;
			// 
			// closeButton
			// 
			this.closeButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.closeButton.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.closeButton.Location = new System.Drawing.Point(417, 3);
			this.closeButton.Name = "closeButton";
			this.closeButton.Size = new System.Drawing.Size(75, 23);
			this.closeButton.TabIndex = 0;
			this.closeButton.Text = "${res:Global.CloseButtonText}";
			this.closeButton.UseVisualStyleBackColor = true;
			// 
			// SolutionConfigurationEditor
			// 
			this.AcceptButton = this.closeButton;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(504, 259);
			this.Controls.Add(this.grid);
			this.Controls.Add(this.panel2);
			this.Controls.Add(this.panel1);
			this.MinimumSize = new System.Drawing.Size(457, 145);
			this.Name = "SolutionConfigurationEditor";
			this.ShowInTaskbar = false;
			this.Text = "${res:Dialog.Options.CombineOptions.Configurations.ConfigurationEditor}";
			this.panel1.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.grid)).EndInit();
			this.panel2.ResumeLayout(false);
			this.ResumeLayout(false);
		}
		private System.Windows.Forms.Button closeButton;
		private System.Windows.Forms.DataGridView grid;
		private System.Windows.Forms.ComboBox configurationComboBox;
		private System.Windows.Forms.ComboBox platformComboBox;
		private System.Windows.Forms.Panel panel2;
		private System.Windows.Forms.DataGridViewComboBoxColumn platformColumn;
		private System.Windows.Forms.DataGridViewComboBoxColumn configurationColumn;
		private System.Windows.Forms.DataGridViewTextBoxColumn projectNameColumn;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Panel panel1;
	}
}
