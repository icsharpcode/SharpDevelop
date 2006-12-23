/*
 * Created by SharpDevelop.
 * User: itai
 * Date: 11/9/2006
 * Time: 4:57 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;
using System.Drawing;
using System.Drawing.Drawing2D;

using Tools.Diagrams;

namespace ClassDiagram
{
	/// <summary>
	/// Description of RouteInheritanceShape.
	/// </summary>
	public class RouteInheritanceShape : RouteShape
	{
		static GraphicsPath path = InitizlizePath();
		
		static GraphicsPath InitizlizePath()
		{
			GraphicsPath path = new GraphicsPath();
			path.AddLines( new PointF[]
			              {
							new PointF(0.0f, 0.0f),
							new PointF(5.0f, 9.0f),
							new PointF(-5.0f, 9.0f)
			              });
			path.CloseFigure();
			return path;
		}
		
		static Pen stroke = Pens.Black;
		static Brush fill = Brushes.White;
		
		protected override void Paint (Graphics graphics)
		{
			graphics.FillPath(fill, path);
			graphics.DrawPath(stroke, path);
		}
	}
}
