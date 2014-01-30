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
	public class PropertyShape : VectorShape
	{
		static PointF[] handPoints = new PointF[]{
			new PointF( 3.0f,  7.0f),
			new PointF( 4.0f,  8.0f),
			new PointF( 6.0f,  6.0f),
			new PointF( 8.0f,  8.0f),
			new PointF( 9.0f,  7.0f),
			new PointF(10.0f,  8.0f),
			new PointF(12.0f,  6.0f),
			new PointF(13.0f,  6.0f),
			new PointF(13.0f,  2.0f),
			new PointF( 8.0f,  2.0f)
		};
		
		static Pen handPen = new Pen(Color.Brown, 0.5f);
		static Brush handBrush = Brushes.Yellow;
		static Pen sleevePen = Pens.DarkBlue;
		static Brush sleeveBrush = Brushes.Blue;
		static Brush shadowBrush = Brushes.Gray;
		static Brush linesBrush = Brushes.Black;
		static Brush panelBrush = Brushes.White;
		static Pen panelPen = Pens.SteelBlue;
		
		public override void Draw(Graphics graphics)
		{
			if (graphics == null) return;
			
			#region Panel
			graphics.FillRectangle(shadowBrush, 1, 14, 10.5f, 1.5f);
			graphics.FillRectangle(shadowBrush, 10, 6, 1.5f, 9);
			
			graphics.FillRectangle(panelBrush, 0, 5, 10, 9);
			graphics.DrawRectangle(panelPen, 0, 5, 10, 9);
			
			graphics.FillRectangle(linesBrush, 1.5f, 9, 2, 1);
			graphics.FillRectangle(linesBrush, 5, 9, 3, 1);

			graphics.FillRectangle(linesBrush, 1.5f, 11, 2, 1);
			graphics.FillRectangle(linesBrush, 5, 11, 3, 1);
			#endregion
			
			#region Hand
			//TODO - improve the hand, choose better colors
			graphics.FillPolygon(handBrush, handPoints);
			graphics.DrawPolygon(handPen, handPoints);
			graphics.DrawLine(handPen, 6, 6, 8, 4);
			graphics.DrawLine(handPen, 7, 7, 9.5f, 4.5f);
			graphics.DrawLine(handPen, 8, 8, 11, 5);
			graphics.FillRectangle(sleeveBrush, 13, 2, 2, 4);
			graphics.DrawRectangle(sleevePen, 13, 2, 2, 4);
			#endregion
		}
		
		public override float ShapeWidth
		{
			get { return 16.0f; }
		}
		
		public override float ShapeHeight
		{
			get { return 16.0f; }
		}
	}
}
