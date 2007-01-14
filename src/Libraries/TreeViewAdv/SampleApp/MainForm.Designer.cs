namespace SampleApp
{
	partial class MainForm
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.tabControl1 = new System.Windows.Forms.TabControl();
			this.tabPage2 = new System.Windows.Forms.TabPage();
			this.simpleExample1 = new SampleApp.SimpleExample();
			this.tabPage1 = new System.Windows.Forms.TabPage();
			this.folderBrowser1 = new SampleApp.FolderBrowser();
			this.tabControl1.SuspendLayout();
			this.tabPage2.SuspendLayout();
			this.tabPage1.SuspendLayout();
			this.SuspendLayout();
			// 
			// tabControl1
			// 
			this.tabControl1.Controls.Add(this.tabPage2);
			this.tabControl1.Controls.Add(this.tabPage1);
			this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tabControl1.Location = new System.Drawing.Point(0, 0);
			this.tabControl1.Name = "tabControl1";
			this.tabControl1.SelectedIndex = 0;
			this.tabControl1.Size = new System.Drawing.Size(689, 413);
			this.tabControl1.TabIndex = 0;
			// 
			// tabPage2
			// 
			this.tabPage2.Controls.Add(this.simpleExample1);
			this.tabPage2.Location = new System.Drawing.Point(4, 22);
			this.tabPage2.Name = "tabPage2";
			this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
			this.tabPage2.Size = new System.Drawing.Size(681, 387);
			this.tabPage2.TabIndex = 1;
			this.tabPage2.Text = "Simple Example";
			this.tabPage2.UseVisualStyleBackColor = true;
			// 
			// simpleExample1
			// 
			this.simpleExample1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.simpleExample1.Location = new System.Drawing.Point(3, 3);
			this.simpleExample1.Name = "simpleExample1";
			this.simpleExample1.Size = new System.Drawing.Size(675, 381);
			this.simpleExample1.TabIndex = 0;
			// 
			// tabPage1
			// 
			this.tabPage1.Controls.Add(this.folderBrowser1);
			this.tabPage1.Location = new System.Drawing.Point(4, 22);
			this.tabPage1.Name = "tabPage1";
			this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
			this.tabPage1.Size = new System.Drawing.Size(681, 306);
			this.tabPage1.TabIndex = 0;
			this.tabPage1.Text = "Folder Browser";
			this.tabPage1.UseVisualStyleBackColor = true;
			// 
			// folderBrowser1
			// 
			this.folderBrowser1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.folderBrowser1.Location = new System.Drawing.Point(3, 3);
			this.folderBrowser1.Name = "folderBrowser1";
			this.folderBrowser1.Size = new System.Drawing.Size(675, 300);
			this.folderBrowser1.TabIndex = 0;
			// 
			// MainForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(689, 413);
			this.Controls.Add(this.tabControl1);
			this.Name = "MainForm";
			this.Text = "Sample Application";
			this.tabControl1.ResumeLayout(false);
			this.tabPage2.ResumeLayout(false);
			this.tabPage1.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.TabControl tabControl1;
		private System.Windows.Forms.TabPage tabPage1;
		private System.Windows.Forms.TabPage tabPage2;
		private FolderBrowser folderBrowser1;
		private SimpleExample simpleExample1;
	}
}

