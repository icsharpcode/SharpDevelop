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
using System.Windows;
using System.Windows.Shapes;
using ICSharpCode.WpfDesign;
using ICSharpCode.WpfDesign.Adorners;

namespace ICSharpCode.WpfDesign.Designer.Extensions
{
	public class PointTrackerPlacementSupport : AdornerPlacement
	{
		private Shape shape;
		private PlacementAlignment alignment;

		public int Index
		{
			get; set;
		}
		
		public PointTrackerPlacementSupport(Shape s, PlacementAlignment align, int index)
		{
			shape = s;
			alignment = align;
			Index = index;
		}

		/// <summary>
		/// Arranges the adorner element on the specified adorner panel.
		/// </summary>
		public override void Arrange(AdornerPanel panel, UIElement adorner, Size adornedElementSize)
		{
			Point p = new Point(0, 0);
			if (shape is Line)
			{
				var s = shape as Line;
				double x, y;
				
				if (alignment == PlacementAlignment.BottomRight)
				{
					x = s.X2;
					y = s.Y2;
				}
				else
				{
					x = s.X1;
					y = s.Y1;
				}
				p = new Point(x, y);
			} else if (shape is Polygon) {
				var pg = shape as Polygon;
				p = pg.Points[Index];
			} else if (shape is Polyline) {
				var pg = shape as Polyline;
				p = pg.Points[Index];
			}

			var transform = shape.RenderedGeometry.Transform;
			p = transform.Transform(p);
			
			adorner.Arrange(new Rect(p.X - 3.5, p.Y - 3.5, 7, 7));
		}
	}
}
