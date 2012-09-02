// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

namespace ICSharpCode.IconEditor
{
	partial class IconPanel : System.Windows.Forms.UserControl
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
					this.Entry = null; // dispose temp. bitmaps
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
			this.components = new System.ComponentModel.Container();
			this.contextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.replaceWithImageToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.deleteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.compressedToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
			this.exportToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.exportANDMaskToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.exportXORMaskToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.emptyContextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.addImageToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.contextMenuStrip.SuspendLayout();
			this.emptyContextMenuStrip.SuspendLayout();
			this.SuspendLayout();
			// 
			// contextMenuStrip
			// 
			this.contextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
									this.replaceWithImageToolStripMenuItem,
									this.deleteToolStripMenuItem,
									this.compressedToolStripMenuItem,
									this.toolStripMenuItem1,
									this.exportToolStripMenuItem,
									this.exportANDMaskToolStripMenuItem,
									this.exportXORMaskToolStripMenuItem});
			this.contextMenuStrip.Name = "contextMenuStrip";
			this.contextMenuStrip.Size = new System.Drawing.Size(181, 164);
			// 
			// replaceWithImageToolStripMenuItem
			// 
			this.replaceWithImageToolStripMenuItem.Name = "replaceWithImageToolStripMenuItem";
			this.replaceWithImageToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
			this.replaceWithImageToolStripMenuItem.Text = "&Replace Image...";
			this.replaceWithImageToolStripMenuItem.Click += new System.EventHandler(this.ReplaceWithImageToolStripMenuItemClick);
			// 
			// deleteToolStripMenuItem
			// 
			this.deleteToolStripMenuItem.Name = "deleteToolStripMenuItem";
			this.deleteToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
			this.deleteToolStripMenuItem.Text = "&Delete";
			this.deleteToolStripMenuItem.Click += new System.EventHandler(this.DeleteToolStripMenuItemClick);
			// 
			// compressedToolStripMenuItem
			// 
			this.compressedToolStripMenuItem.Name = "compressedToolStripMenuItem";
			this.compressedToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
			this.compressedToolStripMenuItem.Text = "Use embedded &PNG";
			this.compressedToolStripMenuItem.Click += new System.EventHandler(this.CompressedToolStripMenuItemClick);
			// 
			// toolStripMenuItem1
			// 
			this.toolStripMenuItem1.Name = "toolStripMenuItem1";
			this.toolStripMenuItem1.Size = new System.Drawing.Size(177, 6);
			// 
			// exportToolStripMenuItem
			// 
			this.exportToolStripMenuItem.Name = "exportToolStripMenuItem";
			this.exportToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
			this.exportToolStripMenuItem.Text = "&Export...";
			this.exportToolStripMenuItem.Click += new System.EventHandler(this.ExportToolStripMenuItemClick);
			// 
			// exportANDMaskToolStripMenuItem
			// 
			this.exportANDMaskToolStripMenuItem.Name = "exportANDMaskToolStripMenuItem";
			this.exportANDMaskToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
			this.exportANDMaskToolStripMenuItem.Text = "Export &AND mask...";
			this.exportANDMaskToolStripMenuItem.Click += new System.EventHandler(this.ExportANDMaskToolStripMenuItemClick);
			// 
			// exportXORMaskToolStripMenuItem
			// 
			this.exportXORMaskToolStripMenuItem.Name = "exportXORMaskToolStripMenuItem";
			this.exportXORMaskToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
			this.exportXORMaskToolStripMenuItem.Text = "Export &XOR image...";
			this.exportXORMaskToolStripMenuItem.Click += new System.EventHandler(this.ExportXORMaskToolStripMenuItemClick);
			// 
			// emptyContextMenuStrip
			// 
			this.emptyContextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
									this.addImageToolStripMenuItem});
			this.emptyContextMenuStrip.Name = "contextMenuStrip";
			this.emptyContextMenuStrip.Size = new System.Drawing.Size(142, 26);
			// 
			// addImageToolStripMenuItem
			// 
			this.addImageToolStripMenuItem.Name = "addImageToolStripMenuItem";
			this.addImageToolStripMenuItem.Size = new System.Drawing.Size(141, 22);
			this.addImageToolStripMenuItem.Text = "&Add Image...";
			this.addImageToolStripMenuItem.Click += new System.EventHandler(this.ReplaceWithImageToolStripMenuItemClick);
			// 
			// IconPanel
			// 
			this.AllowDrop = true;
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
			this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
			this.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.ContextMenuStrip = this.emptyContextMenuStrip;
			this.Name = "IconPanel";
			this.Size = new System.Drawing.Size(173, 171);
			this.DragDrop += new System.Windows.Forms.DragEventHandler(this.IconPanelDragDrop);
			this.DragEnter += new System.Windows.Forms.DragEventHandler(this.IconPanelDragEnter);
			this.Paint += new System.Windows.Forms.PaintEventHandler(this.IconPanelPaint);
			this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.IconPanelMouseDown);
			this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.IconPanelMouseMove);
			this.contextMenuStrip.ResumeLayout(false);
			this.emptyContextMenuStrip.ResumeLayout(false);
			this.ResumeLayout(false);
		}
		private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
		private System.Windows.Forms.ToolStripMenuItem compressedToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem exportXORMaskToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem exportANDMaskToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem addImageToolStripMenuItem;
		private System.Windows.Forms.ContextMenuStrip emptyContextMenuStrip;
		private System.Windows.Forms.ToolStripMenuItem deleteToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem replaceWithImageToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem exportToolStripMenuItem;
		private System.Windows.Forms.ContextMenuStrip contextMenuStrip;
	}
}
