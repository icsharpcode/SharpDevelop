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
