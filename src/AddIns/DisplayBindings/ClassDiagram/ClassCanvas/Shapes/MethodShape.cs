// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

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
