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
	public class EventShape : VectorShape
	{
		static PointF[] lightningPoints1 = new PointF[]{
			new PointF( 5.0f,  1.0f),
			new PointF( 9.0f,  1.0f),
			new PointF( 6.0f,  5.0f),
			new PointF( 8.0f,  5.0f),
			new PointF( 4.0f, 12.0f),
			new PointF( 6.0f,  6.0f),
			new PointF( 4.0f,  6.0f)
		};
		
		static PointF[] lightningPoints2 = new PointF[]{
			new PointF( 9.0f,  1.0f),
			new PointF(10.0f,  2.0f),
			new PointF( 8.0f,  5.0f),
			new PointF( 6.0f,  5.0f)
		};
		
		static PointF[] lightningPoints3 = new PointF[]{
			new PointF( 8.0f,  5.0f),
			new PointF( 9.0f,  6.0f),
			new PointF( 5.0f, 13.0f),
			new PointF( 4.0f, 12.0f)
		};
		
		static PointF[] lightningPoints4 = new PointF[]{
			new PointF( 4.0f,  6.0f),
			new PointF( 6.0f,  6.0f),
			new PointF( 6.0f,  7.0f),
			new PointF( 5.0f,  7.0f)
		};
		
		public override void Draw(Graphics graphics)
		{
			if (graphics == null) return;
			graphics.FillPolygon (Brushes.DarkGoldenrod, lightningPoints2);
			graphics.FillPolygon (Brushes.DarkGoldenrod, lightningPoints3);
			graphics.FillPolygon (Brushes.DarkGoldenrod, lightningPoints4);
			graphics.FillPolygon (Brushes.Gold, lightningPoints1);
		}
		
		public override float ShapeWidth
		{
			get { return 14.0f; }
		}
		
		public override float ShapeHeight
		{
			get { return 14.0f; }
		}
	}
}
