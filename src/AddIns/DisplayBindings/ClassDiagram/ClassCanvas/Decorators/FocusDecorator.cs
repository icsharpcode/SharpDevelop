/*
 * Created by SharpDevelop.
 * User: itai
 * Date: 28/09/2006
 * Time: 19:03
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;
using System.Collections.Generic;

using System.Drawing;
using System.Drawing.Drawing2D;

using System.Xml;
using System.Xml.XPath;

using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Project;

using Tools.Diagrams;

namespace ClassDiagram
{
	public class FocusDecorator : RectangleDecorator
	{
		public FocusDecorator (IRectangle rectangle) : base (rectangle) {}
		
		static Pen InitPen ()
		{
			Pen pen = new Pen(Color.Black);
			pen.DashStyle = DashStyle.Dot;
			return pen;
		}
		
		static Pen pen = InitPen();
		
		public override void DrawToGraphics(Graphics graphics)
		{
			if (graphics == null) return;

			graphics.DrawRectangle(pen,
			                       Rectangle.AbsoluteX - 4, Rectangle.AbsoluteY - 4,
			                       Rectangle.ActualWidth + 8, Rectangle.ActualHeight + 8);
		}
		
		public override void HandleMouseClick(PointF pos) { }
		public override void HandleMouseDown(PointF pos) { }
		public override void HandleMouseMove(PointF pos) { }
		public override void HandleMouseUp(PointF pos) { }
		public override void HandleMouseLeave() { }	
	}
}
