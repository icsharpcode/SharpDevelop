// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Georg Brandl" email="g.brandl@gmx.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Drawing;
using System.Windows.Forms;

namespace ICSharpCode.SharpDevelop.Gui
{

	public class ColorButton : Button
	{			
		Color centerColor;
		
		public ColorButton()
		{		
		}
		
		public Color CenterColor
		{
			get { return centerColor; }
			set { centerColor = value; }
		}		
		
		protected override void OnMouseEnter(EventArgs e)
		{
			base.OnMouseEnter(e);
			Invalidate();
		}
		
		protected override void OnMouseLeave(EventArgs e)
		{
			base.OnMouseLeave(e);
			Invalidate();
		}	
		
		protected override void OnMouseUp(MouseEventArgs e)
		{
			base.OnMouseUp(e);
			Invalidate();
		}
		
		protected override void OnEnabledChanged(EventArgs e)
		{
			base.OnEnabledChanged(e);
			Invalidate();
		}
		
    	protected override void OnClick(EventArgs e)
    	{
    		base.OnClick(e);
    		
    		Point p = new Point(0, Height);
    		p = PointToScreen(p);
			
			using (ColorPaletteDialog clDlg = new ColorPaletteDialog(p.X, p.Y)) {
		    	clDlg.ShowDialog(ICSharpCode.SharpDevelop.Gui.WorkbenchSingleton.MainForm);    	 
				if (clDlg.DialogResult == DialogResult.OK) {
		    		CenterColor = clDlg.Color;
				}
			}
	    		
    	}  	    
		
		protected override void OnPaint(PaintEventArgs e)
		{
			base.OnPaint(e);
			
			Graphics g = e.Graphics;
			
			Rectangle r = ClientRectangle;	
			
			byte border = 4;
			byte right_border = 15;
			
			Rectangle rc = new Rectangle(r.Left + border, r.Top + border,
			                             r.Width - border - right_border - 1, r.Height - border * 2 - 1);
			
			SolidBrush centerColorBrush = new SolidBrush( centerColor );
			g.FillRectangle(centerColorBrush, rc);	
			
			Pen pen = Pens.Black;
			g.DrawRectangle(pen, rc);
			
			Pen greyPen = new Pen(SystemColors.ControlDark);
			
			//draw the arrow
			Point p1 = new Point(r.Width - 9, r.Height / 2 - 1);
			Point p2 = new Point(r.Width - 5, r.Height / 2 - 1);		
			g.DrawLine(Enabled ? pen : greyPen, p1, p2);
			
			p1 = new Point(r.Width - 8, r.Height / 2);
			p2 = new Point(r.Width - 6, r.Height / 2);		
			g.DrawLine(Enabled ? pen : greyPen, p1, p2);
			
			p1 = new Point(r.Width - 7, r.Height / 2);
			p2 = new Point(r.Width - 7, r.Height / 2 + 1);		
			g.DrawLine(Enabled ? pen : greyPen, p1, p2);
			
			//draw the divider line
			pen = new Pen(SystemColors.ControlDark); 
			p1 = new Point(r.Width - 12, 4);
			p2 = new Point(r.Width - 12, r.Height - 5 );		
			g.DrawLine(pen, p1, p2);
			
			pen = new Pen(SystemColors.ControlLightLight); 
			p1 = new Point(r.Width - 11, 4);
			p2 = new Point(r.Width - 11, r.Height - 5 );		
			g.DrawLine(pen, p1, p2);
		} 
	}

}
