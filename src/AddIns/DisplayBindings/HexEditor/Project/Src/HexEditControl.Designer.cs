// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Siegfried Pammer" email="sie_pam@gmx.at"/>
//     <version>$Revision$</version>
// </file>

namespace HexEditor
{
	public partial class HexEditControl
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
			this.bWorker = new System.ComponentModel.BackgroundWorker();
			this.VScrollBar = new System.Windows.Forms.VScrollBar();
			this.TextView = new System.Windows.Forms.Panel();
			this.HexView = new System.Windows.Forms.Panel();
			this.Side = new System.Windows.Forms.Panel();
			this.Header = new System.Windows.Forms.Panel();
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
			// TextView
			// 
			this.TextView.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)));
			this.TextView.Cursor = System.Windows.Forms.Cursors.IBeam;
			this.TextView.Location = new System.Drawing.Point(561, 18);
			this.TextView.Name = "TextView";
			this.TextView.Size = new System.Drawing.Size(108, 347);
			this.TextView.TabIndex = 12;
			this.TextView.MouseDown += new System.Windows.Forms.MouseEventHandler(this.TextViewMouseDown);
			this.TextView.MouseMove += new System.Windows.Forms.MouseEventHandler(this.TextViewMouseMove);
			this.TextView.MouseClick += new System.Windows.Forms.MouseEventHandler(this.TextViewMouseClick);
			this.TextView.MouseUp += new System.Windows.Forms.MouseEventHandler(this.TextViewMouseUp);
			// 
			// HexView
			// 
			this.HexView.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
									| System.Windows.Forms.AnchorStyles.Left)));
			this.HexView.Cursor = System.Windows.Forms.Cursors.IBeam;
			this.HexView.Location = new System.Drawing.Point(92, 18);
			this.HexView.Name = "HexView";
			this.HexView.Size = new System.Drawing.Size(463, 347);
			this.HexView.TabIndex = 11;
			this.HexView.MouseDown += new System.Windows.Forms.MouseEventHandler(this.HexViewMouseDown);
			this.HexView.MouseMove += new System.Windows.Forms.MouseEventHandler(this.HexViewMouseMove);
			this.HexView.MouseClick += new System.Windows.Forms.MouseEventHandler(this.HexViewMouseClick);
			this.HexView.MouseUp += new System.Windows.Forms.MouseEventHandler(this.HexViewMouseUp);
			// 
			// Side
			// 
			this.Side.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
									| System.Windows.Forms.AnchorStyles.Left)));
			this.Side.Location = new System.Drawing.Point(0, 0);
			this.Side.Name = "Side";
			this.Side.Size = new System.Drawing.Size(76, 365);
			this.Side.TabIndex = 10;
			// 
			// Header
			// 
			this.Header.Location = new System.Drawing.Point(92, 0);
			this.Header.Name = "Header";
			this.Header.Size = new System.Drawing.Size(463, 18);
			this.Header.TabIndex = 13;
			// 
			// HexEditControl
			// 
			this.BackColor = System.Drawing.Color.White;
			this.Controls.Add(this.Header);
			this.Controls.Add(this.TextView);
			this.Controls.Add(this.HexView);
			this.Controls.Add(this.Side);
			this.Controls.Add(this.VScrollBar);
			this.DoubleBuffered = true;
			this.Name = "HexEditControl";
			this.Size = new System.Drawing.Size(688, 365);
			this.ContextMenuStripChanged += new System.EventHandler(this.HexEditControlContextMenuStripChanged);
			this.GotFocus += new System.EventHandler(this.HexEditGotFocus);
			this.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.HexEditKeyPress);
			this.Paint += new System.Windows.Forms.PaintEventHandler(this.HexEditPaint);
			this.SizeChanged += new System.EventHandler(this.HexEditSizeChanged);
			this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.HexEditKeyDown);
			this.ResumeLayout(false);
		}
		private System.Windows.Forms.Panel Header;
		private System.Windows.Forms.Panel Side;
		private System.Windows.Forms.Panel HexView;
		private System.Windows.Forms.Panel TextView;
		private System.ComponentModel.BackgroundWorker bWorker;
		private System.Windows.Forms.VScrollBar VScrollBar;
	}
}
