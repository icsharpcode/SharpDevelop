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
			this.DrawEdges (e,this.BuildFocusRectangle);
		}
		
		public  void DrawEdges (PaintEventArgs e,Rectangle rectangle) {
			
			int arc = 5;
			using (Pen p = new Pen (Color.Black)) {
				e.Graphics.DrawRectangle (p,rectangle);
			}
			
			using (Pen pb = new Pen(this.control.BackColor)){
				//top

				int leftLine = rectangle.Left + arc;
				int rightLine = rectangle.Left + rectangle.Width - arc;
				int botLine = rectangle.Top + rectangle.Height;
				//top
				e.Graphics.DrawLine (pb,
				                     leftLine,rectangle.Top,
				                     rightLine, rectangle.Top);
				
				//bottom
				e.Graphics.DrawLine (pb,
				                     leftLine,botLine,
				                     rightLine,botLine);
				//left
				
				int top = rectangle.Top + arc;
				int down = rectangle.Top + rectangle.Height - arc;
				e.Graphics.DrawLine(pb,
				                    rectangle.Left,top,
				                    rectangle.Left,down);
				//right
				e.Graphics.DrawLine(pb,
				                    rectangle.Left + rectangle.Width,top,
				                    rectangle.Left + rectangle.Width,down);
				
			}
		
		}
		
	}
}
