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
	public class PlusShape : SmallButtonShape
	{
		static PointF[] plusPoints = new PointF[]{
			new PointF(4.0f, 2.0f),
			new PointF(6.0f, 2.0f),
			new PointF(6.0f, 4.0f),
			new PointF(8.0f, 4.0f),
			new PointF(8.0f, 6.0f),
			new PointF(6.0f, 6.0f),
			new PointF(6.0f, 8.0f),
			new PointF(4.0f, 8.0f),
			new PointF(4.0f, 6.0f),
			new PointF(2.0f, 6.0f),
			new PointF(2.0f, 4.0f),
			new PointF(4.0f, 4.0f)
		};
		
		static Pen stroke = Pens.Black;
		static Brush fill = Brushes.White;
		
		public override void Draw(Graphics graphics)
		{
			if (graphics == null) return;
			base.Draw(graphics);
			graphics.FillPolygon (fill , plusPoints);
			graphics.DrawPolygon (stroke, plusPoints);
		}
	}
}
