// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

namespace ICSharpCode.Svn
{
	partial class InfoPanel
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
			this.label1 = new System.Windows.Forms.Label();
			this.revisionListView = new System.Windows.Forms.ListView();
			this.columnHeader1 = new System.Windows.Forms.ColumnHeader();
			this.columnHeader2 = new System.Windows.Forms.ColumnHeader();
			this.columnHeader3 = new System.Windows.Forms.ColumnHeader();
			this.columnHeader4 = new System.Windows.Forms.ColumnHeader();
			this.splitContainer1 = new System.Windows.Forms.SplitContainer();
			this.changesListView = new System.Windows.Forms.ListView();
			this.columnHeader5 = new System.Windows.Forms.ColumnHeader();
			this.columnHeader6 = new System.Windows.Forms.ColumnHeader();
			this.columnHeader7 = new System.Windows.Forms.ColumnHeader();
			this.commentRichTextBox = new System.Windows.Forms.RichTextBox();
			this.splitContainer1.Panel1.SuspendLayout();
			this.splitContainer1.Panel2.SuspendLayout();
			this.splitContainer1.SuspendLayout();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
									| System.Windows.Forms.AnchorStyles.Right)));
			this.label1.Location = new System.Drawing.Point(3, 0);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(709, 23);
			this.label1.TabIndex = 0;
			this.label1.Text = "&Revision history:";
			// 
			// revisionListView
			// 
			this.revisionListView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
									| System.Windows.Forms.AnchorStyles.Left) 
									| System.Windows.Forms.AnchorStyles.Right)));
			this.revisionListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
									this.columnHeader1,
									this.columnHeader2,
									this.columnHeader3,
									this.columnHeader4});
			this.revisionListView.FullRowSelect = true;
			this.revisionListView.GridLines = true;
			this.revisionListView.HideSelection = false;
			this.revisionListView.Location = new System.Drawing.Point(3, 26);
			this.revisionListView.MultiSelect = false;
			this.revisionListView.Name = "revisionListView";
			this.revisionListView.Size = new System.Drawing.Size(709, 238);
			this.revisionListView.TabIndex = 1;
			this.revisionListView.UseCompatibleStateImageBehavior = false;
			this.revisionListView.View = System.Windows.Forms.View.Details;
			// 
			// columnHeader1
			// 
			this.columnHeader1.Text = "Revision";
			// 
			// columnHeader2
			// 
			this.columnHeader2.Text = "Author";
			this.columnHeader2.Width = 100;
			// 
			// columnHeader3
			// 
			this.columnHeader3.Text = "Date";
			this.columnHeader3.Width = 120;
			// 
			// columnHeader4
			// 
			this.columnHeader4.Text = "Comment";
			this.columnHeader4.Width = 400;
			// 
			// splitContainer1
			// 
			this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.splitContainer1.Location = new System.Drawing.Point(0, 0);
			this.splitContainer1.Name = "splitContainer1";
			this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
			// 
			// splitContainer1.Panel1
			// 
			this.splitContainer1.Panel1.Controls.Add(this.label1);
			this.splitContainer1.Panel1.Controls.Add(this.revisionListView);
			this.splitContainer1.Panel1MinSize = 75;
			// 
			// splitContainer1.Panel2
			// 
			this.splitContainer1.Panel2.Controls.Add(this.changesListView);
			this.splitContainer1.Panel2.Controls.Add(this.commentRichTextBox);
			this.splitContainer1.Panel2MinSize = 150;
			this.splitContainer1.Size = new System.Drawing.Size(715, 542);
			this.splitContainer1.SplitterDistance = 267;
			this.splitContainer1.TabIndex = 2;
			// 
			// changesListView
			// 
			this.changesListView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
									| System.Windows.Forms.AnchorStyles.Left) 
									| System.Windows.Forms.AnchorStyles.Right)));
			this.changesListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
									this.columnHeader5,
									this.columnHeader6,
									this.columnHeader7});
			this.changesListView.FullRowSelect = true;
			this.changesListView.Location = new System.Drawing.Point(3, 112);
			this.changesListView.Name = "changesListView";
			this.changesListView.Size = new System.Drawing.Size(709, 156);
			this.changesListView.TabIndex = 1;
			this.changesListView.UseCompatibleStateImageBehavior = false;
			this.changesListView.View = System.Windows.Forms.View.Details;
			// 
			// columnHeader5
			// 
			this.columnHeader5.Text = "Action";
			// 
			// columnHeader6
			// 
			this.columnHeader6.Text = "Path";
			// 
			// columnHeader7
			// 
			this.columnHeader7.Text = "Copy from";
			// 
			// commentRichTextBox
			// 
			this.commentRichTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
									| System.Windows.Forms.AnchorStyles.Right)));
			this.commentRichTextBox.BackColor = System.Drawing.SystemColors.Window;
			this.commentRichTextBox.Location = new System.Drawing.Point(3, 3);
			this.commentRichTextBox.Name = "commentRichTextBox";
			this.commentRichTextBox.ReadOnly = true;
			this.commentRichTextBox.Size = new System.Drawing.Size(709, 103);
			this.commentRichTextBox.TabIndex = 0;
			this.commentRichTextBox.Text = "";
			// 
			// InfoPanel
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.splitContainer1);
			this.Name = "InfoPanel";
			this.Size = new System.Drawing.Size(715, 542);
			this.splitContainer1.Panel1.ResumeLayout(false);
			this.splitContainer1.Panel2.ResumeLayout(false);
			this.splitContainer1.ResumeLayout(false);
			this.ResumeLayout(false);
		}
		private System.Windows.Forms.ColumnHeader columnHeader7;
		private System.Windows.Forms.ColumnHeader columnHeader6;
		private System.Windows.Forms.ColumnHeader columnHeader5;
		private System.Windows.Forms.ListView changesListView;
		private System.Windows.Forms.RichTextBox commentRichTextBox;
		private System.Windows.Forms.ColumnHeader columnHeader4;
		private System.Windows.Forms.ColumnHeader columnHeader3;
		private System.Windows.Forms.ColumnHeader columnHeader2;
		private System.Windows.Forms.ColumnHeader columnHeader1;
		private System.Windows.Forms.SplitContainer splitContainer1;
		private System.Windows.Forms.ListView revisionListView;
		private System.Windows.Forms.Label label1;
	}
}
