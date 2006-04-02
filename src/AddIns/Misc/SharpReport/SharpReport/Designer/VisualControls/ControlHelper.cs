/*
 * Created by SharpDevelop.
 * User: Forstmeier Helmut
 * Date: 03.03.2006
 * Time: 08:28
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;
using System.Drawing;
using System.Windows.Forms;
namespace SharpReport.Designer
{
	/// <summary>
	/// Description of ControlHelper.
	/// </summary>
	public class ControlHelper{
		Control control;
		
		public ControlHelper(Control control){
			if (control == null) {
				throw new ArgumentNullException("control");
			}
			this.control = control;
		}
		
		public Rectangle BuildFocusRectangle {
			get {
				return new Rectangle(this.control.ClientRectangle.Left,
				                     this.control.ClientRectangle.Top,
				                     this.control.ClientRectangle.Width -1,
				                     this.control.ClientRectangle.Height -1);
			}
		}
		
		
		
		public  void DrawEdges (PaintEventArgs e) {
			
			int arc = 5;
			Rectangle r = this.BuildFocusRectangle;
			using (Pen p = new Pen (Color.Black)) {
				
				e.Graphics.DrawRectangle (p,
				                          r);
			}
			
			using (Pen pb = new Pen(this.control.BackColor)){
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
		}
		
	}
}
