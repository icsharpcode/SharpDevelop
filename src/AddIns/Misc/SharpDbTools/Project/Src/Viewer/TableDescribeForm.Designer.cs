/*
 * User: dickon
 * Date: 30/08/2006
 * Time: 07:27
 * 
 */
namespace SharpDbTools.Viewer
{
	partial class TableDescribeForm : System.Windows.Forms.Form
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
			this.closeButton = new System.Windows.Forms.Button();
			this.tableInfoDataGridView = new System.Windows.Forms.DataGridView();
			((System.ComponentModel.ISupportInitialize)(this.tableInfoDataGridView)).BeginInit();
			this.SuspendLayout();
			// 
			// closeButton
			// 
			this.closeButton.Location = new System.Drawing.Point(108, 502);
			this.closeButton.Name = "closeButton";
			this.closeButton.Size = new System.Drawing.Size(79, 23);
			this.closeButton.TabIndex = 1;
			this.closeButton.Text = "Close";
			this.closeButton.UseVisualStyleBackColor = true;
			this.closeButton.Click += new System.EventHandler(this.CloseButtonClick);
			// 
			// dataGridView1
			// 
			this.tableInfoDataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this.tableInfoDataGridView.Location = new System.Drawing.Point(2, 1);
			this.tableInfoDataGridView.Name = "dataGridView1";
			this.tableInfoDataGridView.Size = new System.Drawing.Size(307, 495);
			this.tableInfoDataGridView.TabIndex = 2;
			// 
			// TableDescribeForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(312, 532);
			this.Controls.Add(this.tableInfoDataGridView);
			this.Controls.Add(this.closeButton);
			this.Name = "TableDescribeForm";
			this.Text = "Describe";
			((System.ComponentModel.ISupportInitialize)(this.tableInfoDataGridView)).EndInit();
			this.ResumeLayout(false);
		}
		private System.Windows.Forms.DataGridView tableInfoDataGridView;
		private System.Windows.Forms.Button closeButton;
		
		void CloseButtonClick(object sender, System.EventArgs e)
		{
			this.Close();
		}
	}
}
