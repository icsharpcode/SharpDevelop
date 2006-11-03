// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>
namespace ICSharpCode.Svn
{
	partial class DiffPanel : System.Windows.Forms.UserControl
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
			System.Windows.Forms.ColumnHeader revisionColumnHeader;
			System.Windows.Forms.ColumnHeader columnHeader1;
			this.mainSplitContainer = new System.Windows.Forms.SplitContainer();
			this.topSplitContainer = new System.Windows.Forms.SplitContainer();
			this.leftListView = new System.Windows.Forms.ListView();
			this.authorColumnHeader = new System.Windows.Forms.ColumnHeader();
			this.dateColumnHeader = new System.Windows.Forms.ColumnHeader();
			this.commentColumnHeader = new System.Windows.Forms.ColumnHeader();
			this.label1 = new System.Windows.Forms.Label();
			this.rightListView = new System.Windows.Forms.ListView();
			this.columnHeader2 = new System.Windows.Forms.ColumnHeader();
			this.columnHeader3 = new System.Windows.Forms.ColumnHeader();
			this.columnHeader4 = new System.Windows.Forms.ColumnHeader();
			this.label2 = new System.Windows.Forms.Label();
			this.diffViewPanel = new System.Windows.Forms.Panel();
			this.diffLabel = new System.Windows.Forms.Label();
			revisionColumnHeader = new System.Windows.Forms.ColumnHeader();
			columnHeader1 = new System.Windows.Forms.ColumnHeader();
			this.mainSplitContainer.Panel1.SuspendLayout();
			this.mainSplitContainer.Panel2.SuspendLayout();
			this.mainSplitContainer.SuspendLayout();
			this.topSplitContainer.Panel1.SuspendLayout();
			this.topSplitContainer.Panel2.SuspendLayout();
			this.topSplitContainer.SuspendLayout();
			this.SuspendLayout();
			// 
			// revisionColumnHeader
			// 
			revisionColumnHeader.Text = "Revision";
			// 
			// columnHeader1
			// 
			columnHeader1.Text = "Revision";
			// 
			// mainSplitContainer
			// 
			this.mainSplitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
			this.mainSplitContainer.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
			this.mainSplitContainer.Location = new System.Drawing.Point(0, 0);
			this.mainSplitContainer.Name = "mainSplitContainer";
			this.mainSplitContainer.Orientation = System.Windows.Forms.Orientation.Horizontal;
			// 
			// mainSplitContainer.Panel1
			// 
			this.mainSplitContainer.Panel1.Controls.Add(this.topSplitContainer);
			this.mainSplitContainer.Panel1MinSize = 75;
			// 
			// mainSplitContainer.Panel2
			// 
			this.mainSplitContainer.Panel2.Controls.Add(this.diffViewPanel);
			this.mainSplitContainer.Panel2.Controls.Add(this.diffLabel);
			this.mainSplitContainer.Panel2MinSize = 75;
			this.mainSplitContainer.Size = new System.Drawing.Size(600, 400);
			this.mainSplitContainer.SplitterDistance = 177;
			this.mainSplitContainer.TabIndex = 0;
			// 
			// topSplitContainer
			// 
			this.topSplitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
			this.topSplitContainer.Location = new System.Drawing.Point(0, 0);
			this.topSplitContainer.Name = "topSplitContainer";
			// 
			// topSplitContainer.Panel1
			// 
			this.topSplitContainer.Panel1.Controls.Add(this.leftListView);
			this.topSplitContainer.Panel1.Controls.Add(this.label1);
			this.topSplitContainer.Panel1MinSize = 75;
			// 
			// topSplitContainer.Panel2
			// 
			this.topSplitContainer.Panel2.Controls.Add(this.rightListView);
			this.topSplitContainer.Panel2.Controls.Add(this.label2);
			this.topSplitContainer.Panel2MinSize = 75;
			this.topSplitContainer.Size = new System.Drawing.Size(600, 177);
			this.topSplitContainer.SplitterDistance = 297;
			this.topSplitContainer.TabIndex = 0;
			// 
			// leftListView
			// 
			this.leftListView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
									| System.Windows.Forms.AnchorStyles.Left) 
									| System.Windows.Forms.AnchorStyles.Right)));
			this.leftListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
									revisionColumnHeader,
									this.authorColumnHeader,
									this.dateColumnHeader,
									this.commentColumnHeader});
			this.leftListView.FullRowSelect = true;
			this.leftListView.GridLines = true;
			this.leftListView.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
			this.leftListView.HideSelection = false;
			this.leftListView.Location = new System.Drawing.Point(3, 25);
			this.leftListView.MultiSelect = false;
			this.leftListView.Name = "leftListView";
			this.leftListView.Size = new System.Drawing.Size(291, 149);
			this.leftListView.TabIndex = 1;
			this.leftListView.UseCompatibleStateImageBehavior = false;
			this.leftListView.View = System.Windows.Forms.View.Details;
			this.leftListView.SelectedIndexChanged += new System.EventHandler(this.LeftListViewSelectedIndexChanged);
			// 
			// authorColumnHeader
			// 
			this.authorColumnHeader.Text = "Author";
			this.authorColumnHeader.Width = 77;
			// 
			// dateColumnHeader
			// 
			this.dateColumnHeader.Text = "Date";
			this.dateColumnHeader.Width = 82;
			// 
			// commentColumnHeader
			// 
			this.commentColumnHeader.Text = "Comment";
			this.commentColumnHeader.Width = 144;
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(3, 9);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(100, 23);
			this.label1.TabIndex = 0;
			this.label1.Text = "Diff &from:";
			// 
			// rightListView
			// 
			this.rightListView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
									| System.Windows.Forms.AnchorStyles.Left) 
									| System.Windows.Forms.AnchorStyles.Right)));
			this.rightListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
									columnHeader1,
									this.columnHeader2,
									this.columnHeader3,
									this.columnHeader4});
			this.rightListView.FullRowSelect = true;
			this.rightListView.GridLines = true;
			this.rightListView.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
			this.rightListView.HideSelection = false;
			this.rightListView.Location = new System.Drawing.Point(5, 25);
			this.rightListView.MultiSelect = false;
			this.rightListView.Name = "rightListView";
			this.rightListView.Size = new System.Drawing.Size(291, 149);
			this.rightListView.TabIndex = 2;
			this.rightListView.UseCompatibleStateImageBehavior = false;
			this.rightListView.View = System.Windows.Forms.View.Details;
			this.rightListView.SelectedIndexChanged += new System.EventHandler(this.RightListViewSelectedIndexChanged);
			// 
			// columnHeader2
			// 
			this.columnHeader2.Text = "Author";
			this.columnHeader2.Width = 77;
			// 
			// columnHeader3
			// 
			this.columnHeader3.Text = "Date";
			this.columnHeader3.Width = 82;
			// 
			// columnHeader4
			// 
			this.columnHeader4.Text = "Comment";
			this.columnHeader4.Width = 144;
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(3, 9);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(100, 23);
			this.label2.TabIndex = 1;
			this.label2.Text = "Diff &to:";
			// 
			// diffViewPanel
			// 
			this.diffViewPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
									| System.Windows.Forms.AnchorStyles.Left) 
									| System.Windows.Forms.AnchorStyles.Right)));
			this.diffViewPanel.Location = new System.Drawing.Point(3, 21);
			this.diffViewPanel.Name = "diffViewPanel";
			this.diffViewPanel.Size = new System.Drawing.Size(594, 195);
			this.diffViewPanel.TabIndex = 1;
			// 
			// diffLabel
			// 
			this.diffLabel.Location = new System.Drawing.Point(3, 6);
			this.diffLabel.Name = "diffLabel";
			this.diffLabel.Size = new System.Drawing.Size(100, 23);
			this.diffLabel.TabIndex = 0;
			this.diffLabel.Text = "Diff:";
			// 
			// DiffPanel
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.mainSplitContainer);
			this.Name = "DiffPanel";
			this.Size = new System.Drawing.Size(600, 400);
			this.mainSplitContainer.Panel1.ResumeLayout(false);
			this.mainSplitContainer.Panel2.ResumeLayout(false);
			this.mainSplitContainer.ResumeLayout(false);
			this.topSplitContainer.Panel1.ResumeLayout(false);
			this.topSplitContainer.Panel2.ResumeLayout(false);
			this.topSplitContainer.ResumeLayout(false);
			this.ResumeLayout(false);
		}
		private System.Windows.Forms.Label diffLabel;
		private System.Windows.Forms.Panel diffViewPanel;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.ColumnHeader columnHeader4;
		private System.Windows.Forms.ColumnHeader columnHeader3;
		private System.Windows.Forms.ColumnHeader columnHeader2;
		private System.Windows.Forms.ListView rightListView;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.ColumnHeader commentColumnHeader;
		private System.Windows.Forms.ColumnHeader dateColumnHeader;
		private System.Windows.Forms.ColumnHeader authorColumnHeader;
		private System.Windows.Forms.ListView leftListView;
		private System.Windows.Forms.SplitContainer topSplitContainer;
		private System.Windows.Forms.SplitContainer mainSplitContainer;
	}
}
