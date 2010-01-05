/*
 * Erstellt mit SharpDevelop.
 * Benutzer: Peter Forstmeier
 * Datum: 03.01.2010
 * Zeit: 17:43
 * 
 * Sie können diese Vorlage unter Extras > Optionen > Codeerstellung > Standardheader ändern.
 */
namespace SharpReportSamples
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
			System.Windows.Forms.TreeNode treeNode1 = new System.Windows.Forms.TreeNode("FormSheet");
			System.Windows.Forms.TreeNode treeNode2 = new System.Windows.Forms.TreeNode("PullModel");
			System.Windows.Forms.TreeNode treeNode3 = new System.Windows.Forms.TreeNode("PushModel");
			System.Windows.Forms.TreeNode treeNode4 = new System.Windows.Forms.TreeNode("Sample Reports", new System.Windows.Forms.TreeNode[] {
									treeNode1,
									treeNode2,
									treeNode3});
			this.splitContainer1 = new System.Windows.Forms.SplitContainer();
			this.treeView1 = new System.Windows.Forms.TreeView();
			this.label1 = new System.Windows.Forms.Label();
			this.previewControl1 = new ICSharpCode.Reports.Core.ReportViewer.PreviewControl();
			this.menuStrip2 = new System.Windows.Forms.MenuStrip();
			this.splitContainer1.Panel1.SuspendLayout();
			this.splitContainer1.Panel2.SuspendLayout();
			this.splitContainer1.SuspendLayout();
			this.SuspendLayout();
			// 
			// splitContainer1
			// 
			this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.splitContainer1.Location = new System.Drawing.Point(0, 24);
			this.splitContainer1.Name = "splitContainer1";
			// 
			// splitContainer1.Panel1
			// 
			this.splitContainer1.Panel1.Controls.Add(this.treeView1);
			this.splitContainer1.Panel1.Padding = new System.Windows.Forms.Padding(10);
			// 
			// splitContainer1.Panel2
			// 
			this.splitContainer1.Panel2.Controls.Add(this.label1);
			this.splitContainer1.Panel2.Controls.Add(this.previewControl1);
			this.splitContainer1.Panel2.Padding = new System.Windows.Forms.Padding(10, 10, 10, 5);
			this.splitContainer1.Size = new System.Drawing.Size(841, 464);
			this.splitContainer1.SplitterDistance = 152;
			this.splitContainer1.TabIndex = 0;
			// 
			// treeView1
			// 
			this.treeView1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.treeView1.Location = new System.Drawing.Point(10, 10);
			this.treeView1.Margin = new System.Windows.Forms.Padding(10);
			this.treeView1.Name = "treeView1";
			treeNode1.Name = "FormSheetItem";
			treeNode1.Text = "FormSheet";
			treeNode2.Name = "PullModelItem";
			treeNode2.Text = "PullModel";
			treeNode3.Name = "PushModelItem";
			treeNode3.Text = "PushModel";
			treeNode4.Name = "Knoten0";
			treeNode4.Text = "Sample Reports";
			this.treeView1.Nodes.AddRange(new System.Windows.Forms.TreeNode[] {
									treeNode4});
			this.treeView1.Size = new System.Drawing.Size(132, 444);
			this.treeView1.TabIndex = 0;
			this.treeView1.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.TreeView1MouseDoubleClick);
			// 
			// label1
			// 
			this.label1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.label1.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.label1.Location = new System.Drawing.Point(10, 436);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(665, 23);
			this.label1.TabIndex = 3;
			this.label1.Text = "label1";
			// 
			// previewControl1
			// 
			this.previewControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
									| System.Windows.Forms.AnchorStyles.Left) 
									| System.Windows.Forms.AnchorStyles.Right)));
			this.previewControl1.BackColor = System.Drawing.SystemColors.GradientActiveCaption;
			this.previewControl1.Location = new System.Drawing.Point(13, 9);
			this.previewControl1.Messages = null;
			this.previewControl1.Name = "previewControl1";
			this.previewControl1.Padding = new System.Windows.Forms.Padding(5);
			this.previewControl1.Size = new System.Drawing.Size(660, 424);
			this.previewControl1.TabIndex = 2;
			// 
			// menuStrip2
			// 
			this.menuStrip2.Location = new System.Drawing.Point(0, 0);
			this.menuStrip2.Name = "menuStrip2";
			this.menuStrip2.Size = new System.Drawing.Size(841, 24);
			this.menuStrip2.TabIndex = 1;
			this.menuStrip2.Text = "menuStrip2";
			// 
			// MainForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(841, 488);
			this.Controls.Add(this.splitContainer1);
			this.Controls.Add(this.menuStrip2);
			this.Name = "MainForm";
			this.Text = "SharpReportSamples";
			this.splitContainer1.Panel1.ResumeLayout(false);
			this.splitContainer1.Panel2.ResumeLayout(false);
			this.splitContainer1.ResumeLayout(false);
			this.ResumeLayout(false);
			this.PerformLayout();
		}
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.MenuStrip menuStrip2;
		private ICSharpCode.Reports.Core.ReportViewer.PreviewControl previewControl1;
		private System.Windows.Forms.TreeView treeView1;
		private System.Windows.Forms.SplitContainer splitContainer1;
	}
}
