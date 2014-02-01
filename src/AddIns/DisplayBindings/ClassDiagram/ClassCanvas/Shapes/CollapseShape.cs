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
