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
	public class MethodShape : VectorShape
	{
		static Brush brickBrush1 = new SolidBrush(Color.FromArgb(255, 192, 0, 192));
		static PointF[] brickPoints1 = new PointF[]{
			new PointF( 8.0f,  2.0f),
			new PointF(12.0f,  5.0f),
			new PointF( 9.0f,  8.0f),
			new PointF( 5.0f,  4.0f)
		};

		static Brush brickBrush2 = new SolidBrush(Color.FromArgb(255, 064, 0, 064));
		static PointF[] brickPoints2 = new PointF[]{
			new PointF( 9.0f,  8.0f),
			new PointF(12.0f,  5.0f),
			new PointF(12.0f,  7.0f),
			new PointF( 9.0f, 10.0f),
		};

		static Brush brickBrush3 = new SolidBrush(Color.FromArgb(255, 128, 0, 128));
		static PointF[] brickPoints3 = new PointF[]{
			new PointF( 5.0f,  4.0f),
			new PointF( 9.0f,  8.0f),
			new PointF( 9.0f, 10.0f),
			new PointF( 5.0f,  6.0f)
		};
		
		static protected void DrawBrick (Graphics graphics, Brush b1, Brush b2, Brush b3)
		{
			if (graphics == null) return;
			graphics.FillPolygon (b1, brickPoints1);
			graphics.FillPolygon (b2, brickPoints2);
			graphics.FillPolygon (b3, brickPoints3);
		}
		
		public override void Draw(Graphics graphics)
		{
			if (graphics == null) return;
			MethodShape.DrawBrick(graphics, brickBrush1, brickBrush2, brickBrush3);
			graphics.FillRectangle(Brushes.Gray, 1.0f, 4.5f, 3.5f, 0.5f);
			graphics.FillRectangle(Brushes.Gray, 0.0f, 6.5f, 3.5f, 0.5f);
			graphics.FillRectangle(Brushes.Gray, 2.0f, 8.5f, 3.5f, 0.5f);
		}
		
		public override float ShapeWidth
		{
			get { return 13.0f; }
		}
		
		public override float ShapeHeight
		{
			get { return 13.0f; }
		}
	}
}
