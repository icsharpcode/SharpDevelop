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

using System.Windows;
using ICSharpCode.WpfDesign.Adorners;
using ICSharpCode.WpfDesign.Designer.Extensions;

namespace ICSharpCode.WpfDesign.Designer.Controls
{
	/// <summary>
	/// Description of MultiPointThumb.
	/// </summary>
	internal class PointThumb : DesignerThumb
	{
		public PointThumb(Point point)
		{
			this.AdornerPlacement = new PointPlacementSupport(point);
		}

		public AdornerPlacement AdornerPlacement { get; private set; }

		public class PointPlacementSupport : AdornerPlacement
		{
			private Point p;
			public PointPlacementSupport(Point point)
			{
				this.p = point;
			}

			public override void Arrange(AdornerPanel panel, UIElement adorner, Size adornedElementSize)
			{
				double thumbsize = 7;
				adorner.Arrange(new Rect(p.X - thumbsize / 2, p.Y - thumbsize / 2, thumbsize, thumbsize));
			}
		}
	}
}
