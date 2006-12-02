/*
 * User: dickon
 * Date: 21/11/2006
 * Time: 19:12
 * 
 */
namespace SharpDbTools.Forms
{
	partial class SQLTool : System.Windows.Forms.UserControl
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
			this.sqlToolTabControl = new System.Windows.Forms.TabControl();
			this.editorTab = new System.Windows.Forms.TabPage();
			this.resultTab = new System.Windows.Forms.TabPage();
			this.resultDataGridView = new System.Windows.Forms.DataGridView();
			this.messageTab = new System.Windows.Forms.TabPage();
			this.messageTextBox = new System.Windows.Forms.TextBox();
			this.sqlToolTabControl.SuspendLayout();
			this.resultTab.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.resultDataGridView)).BeginInit();
			this.messageTab.SuspendLayout();
			this.SuspendLayout();
			// 
			// sqlToolTabControl
			// 
			this.sqlToolTabControl.Controls.Add(this.editorTab);
			this.sqlToolTabControl.Controls.Add(this.resultTab);
			this.sqlToolTabControl.Controls.Add(this.messageTab);
			this.sqlToolTabControl.Dock = System.Windows.Forms.DockStyle.Fill;
			this.sqlToolTabControl.Location = new System.Drawing.Point(0, 0);
			this.sqlToolTabControl.Name = "sqlToolTabControl";
			this.sqlToolTabControl.SelectedIndex = 0;
			this.sqlToolTabControl.Size = new System.Drawing.Size(885, 522);
			this.sqlToolTabControl.TabIndex = 0;
			// 
			// editorTab
			// 
			this.editorTab.Location = new System.Drawing.Point(4, 24);
			this.editorTab.Name = "editorTab";
			this.editorTab.Padding = new System.Windows.Forms.Padding(3);
			this.editorTab.Size = new System.Drawing.Size(877, 494);
			this.editorTab.TabIndex = 0;
			this.editorTab.Text = "Editor";
			this.editorTab.UseVisualStyleBackColor = true;
			// 
			// resultTab
			// 
			this.resultTab.Controls.Add(this.resultDataGridView);
			this.resultTab.Location = new System.Drawing.Point(4, 24);
			this.resultTab.Name = "resultTab";
			this.resultTab.Padding = new System.Windows.Forms.Padding(3);
			this.resultTab.Size = new System.Drawing.Size(877, 494);
			this.resultTab.TabIndex = 1;
			this.resultTab.Text = "Results";
			this.resultTab.UseVisualStyleBackColor = true;
			// 
			// resultDataGridView
			// 
			this.resultDataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this.resultDataGridView.Dock = System.Windows.Forms.DockStyle.Fill;
			this.resultDataGridView.Location = new System.Drawing.Point(3, 3);
			this.resultDataGridView.Name = "resultDataGridView";
			this.resultDataGridView.Size = new System.Drawing.Size(871, 488);
			this.resultDataGridView.TabIndex = 0;
			// 
			// messageTab
			// 
			this.messageTab.Controls.Add(this.messageTextBox);
			this.messageTab.Location = new System.Drawing.Point(4, 24);
			this.messageTab.Name = "messageTab";
			this.messageTab.Padding = new System.Windows.Forms.Padding(3);
			this.messageTab.Size = new System.Drawing.Size(877, 494);
			this.messageTab.TabIndex = 2;
			this.messageTab.Text = "Messages";
			this.messageTab.UseVisualStyleBackColor = true;
			// 
			// messageTextBox
			// 
			this.messageTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
			this.messageTextBox.Location = new System.Drawing.Point(3, 3);
			this.messageTextBox.Multiline = true;
			this.messageTextBox.Name = "messageTextBox";
			this.messageTextBox.Size = new System.Drawing.Size(871, 488);
			this.messageTextBox.TabIndex = 0;
			// 
			// SQLTool
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.sqlToolTabControl);
			this.Name = "SQLTool";
			this.Size = new System.Drawing.Size(885, 522);
			this.sqlToolTabControl.ResumeLayout(false);
			this.resultTab.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.resultDataGridView)).EndInit();
			this.messageTab.ResumeLayout(false);
			this.messageTab.PerformLayout();
			this.ResumeLayout(false);
		}
		private System.Windows.Forms.TextBox messageTextBox;
		private System.Windows.Forms.DataGridView resultDataGridView;
		private System.Windows.Forms.TabControl sqlToolTabControl;
		private System.Windows.Forms.TabPage messageTab;
		private System.Windows.Forms.TabPage resultTab;
		private System.Windows.Forms.TabPage editorTab;
	}
}
