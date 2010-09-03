// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

namespace HexEditor
{
	public partial class Editor
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
			this.VScrollBar = new System.Windows.Forms.VScrollBar();
			this.textView = new System.Windows.Forms.Panel();
			this.hexView = new System.Windows.Forms.Panel();
			this.side = new System.Windows.Forms.Panel();
			this.header = new System.Windows.Forms.Panel();
			this.SuspendLayout();
			// 
			// VScrollBar
			// 
			this.VScrollBar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
									| System.Windows.Forms.AnchorStyles.Right)));
			this.VScrollBar.Location = new System.Drawing.Point(670, 0);
			this.VScrollBar.Name = "VScrollBar";
			this.VScrollBar.Size = new System.Drawing.Size(18, 365);
			this.VScrollBar.TabIndex = 9;
			this.VScrollBar.Scroll += new System.Windows.Forms.ScrollEventHandler(this.VScrollBarScroll);
			// 
			// textView
			// 
			this.textView.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)));
			this.textView.Cursor = System.Windows.Forms.Cursors.IBeam;
			this.textView.Location = new System.Drawing.Point(561, 18);
			this.textView.MinimumSize = new System.Drawing.Size(1, 1);
			this.textView.Name = "textView";
			this.textView.Size = new System.Drawing.Size(108, 347);
			this.textView.TabIndex = 12;
			this.textView.MouseMove += new System.Windows.Forms.MouseEventHandler(this.TextViewMouseMove);
			this.textView.MouseClick += new System.Windows.Forms.MouseEventHandler(this.TextViewMouseClick);
			this.textView.MouseDown += new System.Windows.Forms.MouseEventHandler(this.TextViewMouseDown);
			this.textView.MouseUp += new System.Windows.Forms.MouseEventHandler(this.TextViewMouseUp);
			// 
			// hexView
			// 
			this.hexView.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
									| System.Windows.Forms.AnchorStyles.Left)));
			this.hexView.Cursor = System.Windows.Forms.Cursors.IBeam;
			this.hexView.Location = new System.Drawing.Point(92, 18);
			this.hexView.MinimumSize = new System.Drawing.Size(1, 1);
			this.hexView.Name = "hexView";
			this.hexView.Size = new System.Drawing.Size(463, 347);
			this.hexView.TabIndex = 11;
			this.hexView.MouseMove += new System.Windows.Forms.MouseEventHandler(this.HexViewMouseMove);
			this.hexView.MouseClick += new System.Windows.Forms.MouseEventHandler(this.HexViewMouseClick);
			this.hexView.MouseDown += new System.Windows.Forms.MouseEventHandler(this.HexViewMouseDown);
			this.hexView.MouseUp += new System.Windows.Forms.MouseEventHandler(this.HexViewMouseUp);
			// 
			// side
			// 
			this.side.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
									| System.Windows.Forms.AnchorStyles.Left)));
			this.side.Location = new System.Drawing.Point(0, 0);
			this.side.MinimumSize = new System.Drawing.Size(1, 1);
			this.side.Name = "side";
			this.side.Size = new System.Drawing.Size(76, 365);
			this.side.TabIndex = 10;
			// 
			// header
			// 
			this.header.Location = new System.Drawing.Point(92, 0);
			this.header.MinimumSize = new System.Drawing.Size(1, 1);
			this.header.Name = "header";
			this.header.Size = new System.Drawing.Size(463, 18);
			this.header.TabIndex = 13;
			// 
			// Editor
			// 
			this.BackColor = System.Drawing.Color.White;
			this.Controls.Add(this.header);
			this.Controls.Add(this.textView);
			this.Controls.Add(this.hexView);
			this.Controls.Add(this.side);
			this.Controls.Add(this.VScrollBar);
			this.DoubleBuffered = true;
			this.MinimumSize = new System.Drawing.Size(1, 1);
			this.Name = "Editor";
			this.Size = new System.Drawing.Size(688, 365);
			this.Paint += new System.Windows.Forms.PaintEventHandler(this.HexEditPaint);
			this.ContextMenuStripChanged += new System.EventHandler(this.HexEditControlContextMenuStripChanged);
			this.GotFocus += new System.EventHandler(this.HexEditGotFocus);
			this.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.HexEditKeyPress);
			this.SizeChanged += new System.EventHandler(this.HexEditSizeChanged);
			this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.HexEditKeyDown);
			this.ResumeLayout(false);
		}
		private System.Windows.Forms.Panel header;
		private System.Windows.Forms.Panel side;
		private System.Windows.Forms.Panel hexView;
		private System.Windows.Forms.Panel textView;
		private System.Windows.Forms.VScrollBar VScrollBar;
	}
}
