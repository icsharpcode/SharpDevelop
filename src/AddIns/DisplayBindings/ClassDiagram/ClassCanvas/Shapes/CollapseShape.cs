/*
 * Created by SharpDevelop.
 * User: itai
 * Date: 28/09/2006
 * Time: 19:04
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace ClassDiagram
{
	public class CollapseShape : VectorShape
	{
		static LinearGradientBrush shade = new LinearGradientBrush(
				new PointF(0, 0), new PointF(0, 22),
				Color.White, Color.LightSteelBlue);
		
		static PointF[] arrowPoints = new PointF[]{
			new PointF(5.0f, 10.0f),
			new PointF(11.0f, 4.0f),
			new PointF(17.0f, 10.0f),
			new PointF(16.0f, 12.0f),
			new PointF(11.0f, 7.0f),
			new PointF(6.0f, 12.0f)
		};
		
		static GraphicsPath roundedButton = InitializeButtonShape();
		
		static GraphicsPath InitializeButtonShape()
		{
			GraphicsPath path = new GraphicsPath();
			path.AddArc(3, 3, 3, 3, 180, 90);
			path.AddArc(18, 3, 3, 3, 270, 90);
			path.AddArc(18, 18, 3, 3, 0, 90);
			path.AddArc(3, 18, 3, 3, 90, 90);
			path.CloseFigure();
			return path;
		}
		
		static protected void DrawButton (Graphics graphics)
		{
			if (graphics == null) return;
			graphics.FillPath(shade, roundedButton);
			graphics.DrawPath(Pens.SteelBlue, roundedButton);			
		}
		
		static protected void DrawArrow (Graphics graphics)
		{
			if (graphics == null) return;
			graphics.FillPolygon (Brushes.DarkBlue, arrowPoints);			
		}
		
		public override void Draw(Graphics graphics)
		{
			if (graphics == null) return;
			CollapseShape.DrawButton(graphics);
			
			graphics.TranslateTransform(1, 1);
			CollapseShape.DrawArrow(graphics);
			graphics.TranslateTransform(0, 6);
			CollapseShape.DrawArrow(graphics);
		}
			
		public override float ShapeWidth
		{
			get { return 23; }
		}
		
		public override float ShapeHeight
		{
			get { return 23; }
		}
	}
}
