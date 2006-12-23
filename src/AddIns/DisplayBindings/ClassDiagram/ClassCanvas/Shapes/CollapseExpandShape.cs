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
	public class CollapseExpandShape : VectorShape
	{
		private bool collapsed;
		static LinearGradientBrush shade = new LinearGradientBrush(
				new PointF(0, 0), new PointF(0, 22),
				Color.White, Color.LightSteelBlue);
		
		static PointF[] arrowPoints = new PointF[]{
			new PointF(4.0f, 9.0f),
			new PointF(10.0f, 3.0f),
			new PointF(16.0f, 9.0f),
			new PointF(15.0f, 11.0f),
			new PointF(10.0f, 6.0f),
			new PointF(5.0f, 11.0f)
		};
		
		static GraphicsPath roundedButton = InitializeButtonShape();
		
		static Pen buttonPen = Pens.SteelBlue;
		static Brush arrowBrush = Brushes.DarkBlue;
		
		static GraphicsPath InitializeButtonShape()
		{
			GraphicsPath path = new GraphicsPath();
			path.AddArc(0, 0, 3, 3, 180, 90);
			path.AddArc(17, 0, 3, 3, 270, 90);
			path.AddArc(17, 17, 3, 3, 0, 90);
			path.AddArc(0, 17, 3, 3, 90, 90);
			path.CloseFigure();
			return path;
		}
		
		static protected void DrawButton (Graphics graphics)
		{
			if (graphics == null) return;
			graphics.FillPath(shade, roundedButton);
			graphics.DrawPath(buttonPen, roundedButton);			
		}
		
		static protected void DrawArrow (Graphics graphics)
		{
			if (graphics == null) return;
			graphics.FillPolygon (arrowBrush, arrowPoints);			
		}
		
		public bool Collapsed
		{
			get { return collapsed; }
			set { collapsed = value; }
		}
		
		public override void Draw(Graphics graphics)
		{
			if (graphics == null) return;
			GraphicsState state = graphics.Save();
			CollapseExpandShape.DrawButton(graphics);
			
			if (collapsed)
			{
				graphics.TranslateTransform(0, 21);
				graphics.ScaleTransform(1, -1);
			}

			CollapseExpandShape.DrawArrow(graphics);
			graphics.TranslateTransform(0, 6);
			CollapseExpandShape.DrawArrow(graphics);
			graphics.Restore(state);
		}
			
		public override float ShapeWidth
		{
			get { return 20; }
		}
		
		public override float ShapeHeight
		{
			get { return 20; }
		}
	}
}
