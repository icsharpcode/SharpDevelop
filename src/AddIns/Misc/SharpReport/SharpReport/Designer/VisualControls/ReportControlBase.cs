/*
 * Created by SharpDevelop.
 * User: Fabio
 * Date: 09/10/2004
 * Time: 9.48
 *
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;
using System.Drawing;
using System.ComponentModel;
using System.Windows.Forms;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop;

namespace SharpReport.Designer{
	/// <summary>
	/// Base Class of all Visible Controls like Graphic or textbased Item's
	/// </summary>
	
	public abstract class ReportControlBase : ReportObjectControlBase{
		private System.Windows.Forms.Label lblTopLeft;
		private System.Windows.Forms.Label lblBottomRight;
		
		const string contextMenuPath = "/SharpReport/ContextMenu/Items";
		
		private enum SizeDirection{
			None,
			TopLeft,
			BottomRight,
		}
		
		
		private int xCoor;
		private int yCoor;
		private SizeDirection mouseDown;
		
		internal ReportControlBase(){
			InitializeComponent();
			this.SetStyle(ControlStyles.DoubleBuffer |
			              ControlStyles.UserPaint |
			              ControlStyles.AllPaintingInWmPaint |
			              ControlStyles.ResizeRedraw,
			              true);
			this.UpdateStyles();
			lblTopLeft.Visible = false;
			lblBottomRight.Visible = false;
		}
		
		private void ReportControlBaseEnter(object sender, System.EventArgs e){
			lblTopLeft.Visible = true;
			lblBottomRight.Visible = true;
			this.Refresh();
		}
		
		private void ReportControlBaseLeave(object sender, System.EventArgs e){
			lblTopLeft.Visible = false;
			lblBottomRight.Visible = false;
			this.Refresh();
		}
		
		private void SizeMouseDown(object sender, System.Windows.Forms.MouseEventArgs e){
			if (sender == lblTopLeft){
				mouseDown = SizeDirection.TopLeft;
			}
			if (sender == lblBottomRight){
				mouseDown = SizeDirection.BottomRight;
			}
			xCoor = e.X;
			yCoor = e.Y;
		}
		
		private void SizeMouseMove(object sender, System.Windows.Forms.MouseEventArgs e){
			if (mouseDown == SizeDirection.TopLeft){
				this.Top = this.Top + (e.Y - yCoor);
				this.Left = this.Left + (e.X - xCoor);
			}
			
			if (mouseDown == SizeDirection.BottomRight){
				this.Height = this.Height + (e.Y - yCoor);
				this.Width = this.Width + (e.X - xCoor);
			}
			
		}
		private void ReportControlBaseMouseUp(object sender, MouseEventArgs e){
			
			if (e.Button == MouseButtons.Right) {
				ContextMenuStrip ctMen = MenuService.CreateContextMenu (this,contextMenuPath);
				ctMen.Show (this,new Point (e.X,e.Y));
			} 
		}
		private void SizeMouseUp(object sender, System.Windows.Forms.MouseEventArgs e){
			mouseDown = SizeDirection.None;
			base.OnControlChanged();
		}
		
		private Rectangle BuildFocusRectangle(){
			return new Rectangle(this.ClientRectangle.Left,
			                            this.ClientRectangle.Top,
			                            this.ClientRectangle.Width -1,
			                            this.ClientRectangle.Height -1);
			
		}
		
		private void DrawDecorations(Graphics g){
			// it is not said that the
			// focused object in all the app
			// is the current report item!
			// So I don't check this.Focused.
			
			if (lblBottomRight.Visible){
				g.Clear(this.Body.BackColor);
				ControlPaint.DrawFocusRectangle(g,
				                                this.BuildFocusRectangle());
			}
		}
		
		protected override void OnPaint(System.Windows.Forms.PaintEventArgs e){
			base.OnPaint(e);
			int arc = 5;
			Rectangle r = this.BuildFocusRectangle();
			using (Pen p = new Pen (Color.Black)) {
				
				e.Graphics.DrawRectangle (p,
				                          r);
			}
			
			using (Pen pb = new Pen(this.BackColor)){
				//top

				int leftLine = r.Left + arc;
				int rightLine = r.Left + r.Width - arc;
				int botLine = r.Top + r.Height;
				//top
				e.Graphics.DrawLine (pb,
				                     leftLine,r.Top,
				                     rightLine, r.Top);
				
				//bottom
				e.Graphics.DrawLine (pb,
				                     leftLine,botLine,
				                     rightLine,botLine);
				//left
				
				int top = r.Top + arc;
				int down = r.Top + r.Height - arc;
				e.Graphics.DrawLine(pb,
				                    r.Left,top,
				                    r.Left,down);
				//right
				e.Graphics.DrawLine(pb,
				                    r.Left + r.Width,top,
				                    r.Left + r.Width,down);
				                    
			}
			this.DrawDecorations(e.Graphics);
		}
		
		protected override void OnResize(EventArgs e){		
			base.OnResize(e);
			this.Invalidate();
		}
		/*
		private Image Line(){
			Bitmap b = new Bitmap (8,1);
			using (Graphics g = Graphics.FromImage (b)){
				using (Pen p = new Pen(Color.Black,1)){
					g.DrawLine(p,0,0,8,0);
				}
			}
			return (Image)b;
		}
		*/
		
		#region Windows Forms Designer generated code
		/// <summary>
		/// This method is required for Windows Forms designer support.
		/// Do not change the method contents inside the source code editor. The Forms designer might
		/// not be able to load this method if it was changed manually.
		/// </summary>
		private void InitializeComponent() {
			this.lblBottomRight = new System.Windows.Forms.Label();
			this.lblTopLeft = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// lblBottomRight
			// 
			this.lblBottomRight.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.lblBottomRight.BackColor = System.Drawing.Color.Transparent;
			this.lblBottomRight.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.lblBottomRight.Cursor = System.Windows.Forms.Cursors.SizeNWSE;
			this.lblBottomRight.Location = new System.Drawing.Point(283, 47);
			this.lblBottomRight.Name = "lblBottomRight";
			this.lblBottomRight.Size = new System.Drawing.Size(8, 8);
			this.lblBottomRight.TabIndex = 0;
			this.lblBottomRight.MouseUp += new System.Windows.Forms.MouseEventHandler(this.SizeMouseUp);
			this.lblBottomRight.MouseMove += new System.Windows.Forms.MouseEventHandler(this.SizeMouseMove);
			this.lblBottomRight.MouseDown += new System.Windows.Forms.MouseEventHandler(this.SizeMouseDown);
			// 
			// lblTopLeft
			// 
			this.lblTopLeft.BackColor = System.Drawing.Color.Transparent;
			this.lblTopLeft.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.lblTopLeft.Cursor = System.Windows.Forms.Cursors.SizeAll;
			this.lblTopLeft.Location = new System.Drawing.Point(0, 0);
			this.lblTopLeft.Name = "lblTopLeft";
			this.lblTopLeft.Size = new System.Drawing.Size(8, 8);
			this.lblTopLeft.TabIndex = 1;
			this.lblTopLeft.MouseUp += new System.Windows.Forms.MouseEventHandler(this.SizeMouseUp);
			this.lblTopLeft.MouseMove += new System.Windows.Forms.MouseEventHandler(this.SizeMouseMove);
			this.lblTopLeft.MouseDown += new System.Windows.Forms.MouseEventHandler(this.SizeMouseDown);
			// 
			// ReportControlBase
			// 
			this.Controls.Add(this.lblTopLeft);
			this.Controls.Add(this.lblBottomRight);
			this.Name = "ReportControlBase";
			this.Size = new System.Drawing.Size(292, 56);
			this.Enter += new System.EventHandler(this.ReportControlBaseEnter);
			this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.ReportControlBaseMouseUp);
			this.Leave += new System.EventHandler(this.ReportControlBaseLeave);
			this.ResumeLayout(false);
		}
		#endregion
	}
}
