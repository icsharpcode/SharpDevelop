// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under MIT X11 license (for details please see \doc\license.txt)
namespace ICSharpCode.NRefactory.Demo
{
	partial class MainForm
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
			this.tabControl1 = new System.Windows.Forms.TabControl();
			this.tabPage1 = new System.Windows.Forms.TabPage();
			this.splitContainer1 = new System.Windows.Forms.SplitContainer();
			this.csharpCodeTextBox = new System.Windows.Forms.TextBox();
			this.csharpTreeView = new System.Windows.Forms.TreeView();
			this.csharpGenerateCodeButton = new System.Windows.Forms.Button();
			this.csharpParseButton = new System.Windows.Forms.Button();
			this.tabPage2 = new System.Windows.Forms.TabPage();
			this.tabControl1.SuspendLayout();
			this.tabPage1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
			this.splitContainer1.Panel1.SuspendLayout();
			this.splitContainer1.Panel2.SuspendLayout();
			this.splitContainer1.SuspendLayout();
			this.SuspendLayout();
			// 
			// tabControl1
			// 
			this.tabControl1.Controls.Add(this.tabPage1);
			this.tabControl1.Controls.Add(this.tabPage2);
			this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tabControl1.Location = new System.Drawing.Point(0, 0);
			this.tabControl1.Name = "tabControl1";
			this.tabControl1.SelectedIndex = 0;
			this.tabControl1.Size = new System.Drawing.Size(515, 484);
			this.tabControl1.TabIndex = 0;
			// 
			// tabPage1
			// 
			this.tabPage1.Controls.Add(this.splitContainer1);
			this.tabPage1.Location = new System.Drawing.Point(4, 22);
			this.tabPage1.Name = "tabPage1";
			this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
			this.tabPage1.Size = new System.Drawing.Size(507, 458);
			this.tabPage1.TabIndex = 0;
			this.tabPage1.Text = "C#";
			this.tabPage1.UseVisualStyleBackColor = true;
			// 
			// splitContainer1
			// 
			this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.splitContainer1.Location = new System.Drawing.Point(3, 3);
			this.splitContainer1.Name = "splitContainer1";
			this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
			// 
			// splitContainer1.Panel1
			// 
			this.splitContainer1.Panel1.Controls.Add(this.csharpCodeTextBox);
			// 
			// splitContainer1.Panel2
			// 
			this.splitContainer1.Panel2.Controls.Add(this.csharpTreeView);
			this.splitContainer1.Panel2.Controls.Add(this.csharpGenerateCodeButton);
			this.splitContainer1.Panel2.Controls.Add(this.csharpParseButton);
			this.splitContainer1.Size = new System.Drawing.Size(501, 452);
			this.splitContainer1.SplitterDistance = 201;
			this.splitContainer1.TabIndex = 0;
			// 
			// csharpCodeTextBox
			// 
			this.csharpCodeTextBox.AcceptsReturn = true;
			this.csharpCodeTextBox.AcceptsTab = true;
			this.csharpCodeTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
			this.csharpCodeTextBox.Font = new System.Drawing.Font("Courier New", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.csharpCodeTextBox.HideSelection = false;
			this.csharpCodeTextBox.Location = new System.Drawing.Point(0, 0);
			this.csharpCodeTextBox.Multiline = true;
			this.csharpCodeTextBox.Name = "csharpCodeTextBox";
			this.csharpCodeTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Both;
			this.csharpCodeTextBox.Size = new System.Drawing.Size(501, 201);
			this.csharpCodeTextBox.TabIndex = 0;
			this.csharpCodeTextBox.Text = "using System;\r\nclass Test\r\n{\r\n    public void Main(string[] args)\r\n    {\r\n       " +
			"  System.Console.WriteLine(\"Hello, World\");\r\n    }\r\n}";
			this.csharpCodeTextBox.WordWrap = false;
			// 
			// csharpTreeView
			// 
			this.csharpTreeView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
									| System.Windows.Forms.AnchorStyles.Left) 
									| System.Windows.Forms.AnchorStyles.Right)));
			this.csharpTreeView.HideSelection = false;
			this.csharpTreeView.Location = new System.Drawing.Point(3, 28);
			this.csharpTreeView.Name = "csharpTreeView";
			this.csharpTreeView.Size = new System.Drawing.Size(493, 216);
			this.csharpTreeView.TabIndex = 2;
			this.csharpTreeView.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.CSharpTreeViewAfterSelect);
			// 
			// csharpGenerateCodeButton
			// 
			this.csharpGenerateCodeButton.Anchor = System.Windows.Forms.AnchorStyles.Top;
			this.csharpGenerateCodeButton.Location = new System.Drawing.Point(249, 3);
			this.csharpGenerateCodeButton.Name = "csharpGenerateCodeButton";
			this.csharpGenerateCodeButton.Size = new System.Drawing.Size(118, 23);
			this.csharpGenerateCodeButton.TabIndex = 1;
			this.csharpGenerateCodeButton.Text = "Generate Code";
			this.csharpGenerateCodeButton.UseVisualStyleBackColor = true;
			this.csharpGenerateCodeButton.Click += new System.EventHandler(this.CSharpGenerateCodeButtonClick);
			// 
			// csharpParseButton
			// 
			this.csharpParseButton.Anchor = System.Windows.Forms.AnchorStyles.Top;
			this.csharpParseButton.Location = new System.Drawing.Point(133, 3);
			this.csharpParseButton.Name = "csharpParseButton";
			this.csharpParseButton.Size = new System.Drawing.Size(110, 23);
			this.csharpParseButton.TabIndex = 0;
			this.csharpParseButton.Text = "Parse Code";
			this.csharpParseButton.UseVisualStyleBackColor = true;
			this.csharpParseButton.Click += new System.EventHandler(this.CSharpParseButtonClick);
			// 
			// tabPage2
			// 
			this.tabPage2.Location = new System.Drawing.Point(4, 22);
			this.tabPage2.Name = "tabPage2";
			this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
			this.tabPage2.Size = new System.Drawing.Size(507, 458);
			this.tabPage2.TabIndex = 1;
			this.tabPage2.Text = "VB";
			this.tabPage2.UseVisualStyleBackColor = true;
			// 
			// MainForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(515, 484);
			this.Controls.Add(this.tabControl1);
			this.Name = "MainForm";
			this.Text = "NRefactory Demo";
			this.tabControl1.ResumeLayout(false);
			this.tabPage1.ResumeLayout(false);
			this.splitContainer1.Panel1.ResumeLayout(false);
			this.splitContainer1.Panel1.PerformLayout();
			this.splitContainer1.Panel2.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
			this.splitContainer1.ResumeLayout(false);
			this.ResumeLayout(false);
		}
		private System.Windows.Forms.TreeView csharpTreeView;
		private System.Windows.Forms.Button csharpParseButton;
		private System.Windows.Forms.Button csharpGenerateCodeButton;
		private System.Windows.Forms.TextBox csharpCodeTextBox;
		private System.Windows.Forms.SplitContainer splitContainer1;
		private System.Windows.Forms.TabPage tabPage2;
		private System.Windows.Forms.TabPage tabPage1;
		private System.Windows.Forms.TabControl tabControl1;
	}
}
