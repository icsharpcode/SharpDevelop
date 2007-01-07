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
		
		/// <summary>
			base.Dispose(disposing);
		}
		/// This method is required for Windows Forms designer support.
		/// Do not change the method contents inside the source code editor. The Forms designer might
		/// not be able to load this method if it was changed manually.
		/// </summary>
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			this.sqlToolTabControl = new System.Windows.Forms.TabControl();
			this.editorTab = new System.Windows.Forms.TabPage();
			this.statusStrip = new System.Windows.Forms.StatusStrip();
			this.queryToolStripProgressBar = new System.Windows.Forms.ToolStripProgressBar();
			this.resultTab = new System.Windows.Forms.TabPage();
			this.resultDataGridView = new System.Windows.Forms.DataGridView();
			this.messageTab = new System.Windows.Forms.TabPage();
			this.messageTextBox = new System.Windows.Forms.TextBox();
			this.progressTimer = new System.Windows.Forms.Timer(this.components);
			this.sqlToolTabControl.SuspendLayout();
			this.editorTab.SuspendLayout();
			this.statusStrip.SuspendLayout();
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
			this.sqlToolTabControl.Size = new System.Drawing.Size(759, 452);
			this.sqlToolTabControl.TabIndex = 0;
			// 
			// editorTab
			// 
			this.editorTab.Controls.Add(this.statusStrip);
			this.editorTab.Location = new System.Drawing.Point(4, 22);
			this.editorTab.Name = "editorTab";
			this.editorTab.Padding = new System.Windows.Forms.Padding(3);
			this.editorTab.Size = new System.Drawing.Size(751, 426);
			this.editorTab.TabIndex = 0;
			this.editorTab.Text = "Editor";
			this.editorTab.UseVisualStyleBackColor = true;
			// 
			// statusStrip
			// 
			this.statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
									this.queryToolStripProgressBar});
			this.statusStrip.Location = new System.Drawing.Point(3, 401);
			this.statusStrip.Name = "statusStrip";
			this.statusStrip.Size = new System.Drawing.Size(745, 22);
			this.statusStrip.TabIndex = 0;
			this.statusStrip.Text = "statusStrip1";
			// 
			// queryToolStripProgressBar
			// 
			this.queryToolStripProgressBar.Name = "queryToolStripProgressBar";
			this.queryToolStripProgressBar.Size = new System.Drawing.Size(100, 16);
			this.queryToolStripProgressBar.Visible = false;
			// 
			// resultTab
			// 
			this.resultTab.Controls.Add(this.resultDataGridView);
			this.resultTab.Location = new System.Drawing.Point(4, 22);
			this.resultTab.Name = "resultTab";
			this.resultTab.Padding = new System.Windows.Forms.Padding(3);
			this.resultTab.Size = new System.Drawing.Size(751, 426);
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
			this.resultDataGridView.Size = new System.Drawing.Size(745, 420);
			this.resultDataGridView.TabIndex = 0;
			// 
			// messageTab
			// 
			this.messageTab.Controls.Add(this.messageTextBox);
			this.messageTab.Location = new System.Drawing.Point(4, 22);
			this.messageTab.Name = "messageTab";
			this.messageTab.Padding = new System.Windows.Forms.Padding(3);
			this.messageTab.Size = new System.Drawing.Size(751, 426);
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
			this.messageTextBox.Size = new System.Drawing.Size(745, 420);
			this.messageTextBox.TabIndex = 0;
			// 
			// progressTimer
			// 
			this.progressTimer.Tick += new System.EventHandler(this.ProgressTimerTick);
			// 
			// SQLTool
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.sqlToolTabControl);
			this.Name = "SQLTool";
			this.Size = new System.Drawing.Size(759, 452);
			this.sqlToolTabControl.ResumeLayout(false);
			this.editorTab.ResumeLayout(false);
			this.editorTab.PerformLayout();
			this.statusStrip.ResumeLayout(false);
			this.statusStrip.PerformLayout();
			this.resultTab.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.resultDataGridView)).EndInit();
			this.messageTab.ResumeLayout(false);
			this.messageTab.PerformLayout();
			this.ResumeLayout(false);
		}
		private System.Windows.Forms.Timer progressTimer;
		private System.Windows.Forms.ToolStripProgressBar queryToolStripProgressBar;
		private System.Windows.Forms.StatusStrip statusStrip;
		private System.Windows.Forms.TextBox messageTextBox;
		private System.Windows.Forms.DataGridView resultDataGridView;
		private System.Windows.Forms.TabControl sqlToolTabControl;
		private System.Windows.Forms.TabPage messageTab;
		private System.Windows.Forms.TabPage resultTab;
		private System.Windows.Forms.TabPage editorTab;
		
	}
}
