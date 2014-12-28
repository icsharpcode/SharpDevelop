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



		public PointTrackerPlacementSupport(Shape s, PlacementAlignment align)
		{
			shape = s;
			alignment = align;
			Index = -1;
		}

		/// <summary>
		/// Arranges the adorner element on the specified adorner panel.
		/// </summary>
		public override void Arrange(AdornerPanel panel, UIElement adorner, Size adornedElementSize)
		{
			Point p = new Point(0, 0);
			double thumbsize = 7;
			double distance = 0;// thumbsize / 2;
			if (shape as Line != null)
			{
				Line s = shape as Line;
				double x, y;
				//will give you the angle of the line if more than 180 degrees it becomes negative from
				Double theta = (180 / Math.PI) * Math.Atan2(s.Y2 - s.Y1, s.X2 - s.X1);

				//this will give you the x offset from the line x point in parts of half the size of the thumb
				double dx = Math.Cos(theta * (Math.PI / 180)) * distance;

				//this will give you the y offset from the line y point in parts of half the size of the thumb
				double dy = Math.Sin(theta * (Math.PI / 180)) * distance;
				
				if (alignment == PlacementAlignment.BottomRight)
				{
					//x offset is linear
					x = s.X2 - Math.Abs(theta) / (180 / thumbsize) + dx;
					//y offset is angular
					y = s.Y2 - ((.5 - Math.Sin(theta * (Math.PI / 180)) * .5) * thumbsize) + dy;
				}
				else
				{
					x = s.X1 - ((180 - Math.Abs(theta)) / (180 / thumbsize)) - dx;
					y = s.Y1 - ((.5 - Math.Sin(theta * (Math.PI / 180) + Math.PI) * .5) * thumbsize) - dy;
				}
				p = new Point(x, y);

			}
			Polygon pg = shape as Polygon;
			Polyline pl = shape as Polyline;
			if (pg != null || pl != null)
			{
				if (Index > 0)
				{
					p = pl != null ? pl.Points[Index] : pg.Points[Index];

				}
			}
			adorner.Arrange(new Rect(p, new Size(thumbsize, thumbsize)));
		}


	}
}
