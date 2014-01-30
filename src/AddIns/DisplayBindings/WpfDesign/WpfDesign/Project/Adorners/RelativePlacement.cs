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
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Media;

namespace ICSharpCode.WpfDesign.Adorners
{
	// We have to support the different coordinate spaces as explained in
	// http://myfun.spaces.live.com/blog/cns!AC1291870308F748!242.entry

	/// <summary>
	/// Placement class providing properties for different kinds of relative placements.
	/// </summary>
	public sealed class RelativePlacement : AdornerPlacement
	{
		/// <summary>
		/// Creates a new RelativePlacement instance. The default instance is a adorner with zero size, you
		/// have to set some properties to define the placement.
		/// </summary>
		public RelativePlacement()
		{
		}
		
		/// <summary>
		/// Creates a new RelativePlacement instance from the specified horizontal and vertical alignments.
		/// </summary>
		public RelativePlacement(HorizontalAlignment horizontal, VerticalAlignment vertical)
		{
			switch (horizontal) {
				case HorizontalAlignment.Left:
					widthRelativeToDesiredWidth = 1;
					xRelativeToAdornerWidth = -1;
					break;
				case HorizontalAlignment.Right:
					widthRelativeToDesiredWidth = 1;
					xRelativeToContentWidth = 1;
					break;
				case HorizontalAlignment.Center:
					widthRelativeToDesiredWidth = 1;
					xRelativeToContentWidth = 0.5;
					xRelativeToAdornerWidth = -0.5;
					break;
				case HorizontalAlignment.Stretch:
					widthRelativeToContentWidth = 1;
					break;
			}
			switch (vertical) {
				case VerticalAlignment.Top:
					heightRelativeToDesiredHeight = 1;
					yRelativeToAdornerHeight = -1;
					break;
				case VerticalAlignment.Bottom:
					heightRelativeToDesiredHeight = 1;
					yRelativeToContentHeight = 1;
					break;
				case VerticalAlignment.Center:
					heightRelativeToDesiredHeight = 1;
					yRelativeToContentHeight = 0.5;
					yRelativeToAdornerHeight = -0.5;
					break;
				case VerticalAlignment.Stretch:
					heightRelativeToContentHeight = 1;
					break;
			}
		}
		
		double widthRelativeToDesiredWidth, heightRelativeToDesiredHeight;
		
		/// <summary>
		/// Gets/Sets the width of the adorner as factor relative to the desired adorner width.
		/// </summary>
		public double WidthRelativeToDesiredWidth {
			get { return widthRelativeToDesiredWidth; }
			set { widthRelativeToDesiredWidth = value; }
		}
		
		/// <summary>
		/// Gets/Sets the height of the adorner as factor relative to the desired adorner height.
		/// </summary>
		public double HeightRelativeToDesiredHeight {
			get { return heightRelativeToDesiredHeight; }
			set { heightRelativeToDesiredHeight = value; }
		}
		
		double widthRelativeToContentWidth, heightRelativeToContentHeight;
		
		/// <summary>
		/// Gets/Sets the width of the adorner as factor relative to the width of the adorned item.
		/// </summary>
		public double WidthRelativeToContentWidth {
			get { return widthRelativeToContentWidth; }
			set { widthRelativeToContentWidth = value; }
		}
		
		/// <summary>
		/// Gets/Sets the height of the adorner as factor relative to the height of the adorned item.
		/// </summary>
		public double HeightRelativeToContentHeight {
			get { return heightRelativeToContentHeight; }
			set { heightRelativeToContentHeight = value; }
		}
		
		double widthOffset, heightOffset;
		
		/// <summary>
		/// Gets/Sets an offset that is added to the adorner width for the size calculation.
		/// </summary>
		public double WidthOffset {
			get { return widthOffset; }
			set { widthOffset = value; }
		}
		
		/// <summary>
		/// Gets/Sets an offset that is added to the adorner height for the size calculation.
		/// </summary>
		public double HeightOffset {
			get { return heightOffset; }
			set { heightOffset = value; }
		}
		
		Size CalculateSize(UIElement adorner, Size adornedElementSize)
		{
			return new Size(Math.Max(widthOffset
			                         + widthRelativeToDesiredWidth * adorner.DesiredSize.Width
			                         + widthRelativeToContentWidth * adornedElementSize.Width, 0),
			                Math.Max(heightOffset
			                         + heightRelativeToDesiredHeight * adorner.DesiredSize.Height
			                         + heightRelativeToContentHeight * adornedElementSize.Height, 0));
		}
		
		double xOffset, yOffset;
		
		/// <summary>
		/// Gets/Sets an offset that is added to the adorner position.
		/// </summary>
		public double XOffset {
			get { return xOffset; }
			set { xOffset = value; }
		}
		
		/// <summary>
		/// Gets/Sets an offset that is added to the adorner position.
		/// </summary>
		public double YOffset {
			get { return yOffset; }
			set { yOffset = value; }
		}
		
		double xRelativeToAdornerWidth, yRelativeToAdornerHeight;
		
		/// <summary>
		/// Gets/Sets the left border of the adorner element as factor relative to the width of the adorner.
		/// </summary>
		public double XRelativeToAdornerWidth {
			get { return xRelativeToAdornerWidth; }
			set { xRelativeToAdornerWidth = value; }
		}
		
		/// <summary>
		/// Gets/Sets the top border of the adorner element as factor relative to the height of the adorner.
		/// </summary>
		public double YRelativeToAdornerHeight {
			get { return yRelativeToAdornerHeight; }
			set { yRelativeToAdornerHeight = value; }
		}
		
		double xRelativeToContentWidth, yRelativeToContentHeight;
		
		/// <summary>
		/// Gets/Sets the left border of the adorner element as factor relative to the width of the adorned content.
		/// </summary>
		public double XRelativeToContentWidth {
			get { return xRelativeToContentWidth; }
			set { xRelativeToContentWidth = value; }
		}
		
		/// <summary>
		/// Gets/Sets the top border of the adorner element as factor relative to the height of the adorned content.
		/// </summary>
		public double YRelativeToContentHeight {
			get { return yRelativeToContentHeight; }
			set { yRelativeToContentHeight = value; }
		}
		
		Point CalculatePosition(Size adornedElementSize, Size adornerSize)
		{
			return new Point(xOffset
			                 + xRelativeToAdornerWidth * adornerSize.Width
			                 + xRelativeToContentWidth * adornedElementSize.Width,
			                 yOffset
			                 + yRelativeToAdornerHeight * adornerSize.Height
			                 + yRelativeToContentHeight * adornedElementSize.Height);
		}
		
		/// <summary>
		/// Arranges the adorner element on the specified adorner panel.
		/// </summary>
		public override void Arrange(AdornerPanel panel, UIElement adorner, Size adornedElementSize)
		{
			Size adornerSize = CalculateSize(adorner, adornedElementSize);
			adorner.Arrange(new Rect(CalculatePosition(adornedElementSize, adornerSize), adornerSize));
		}
	}
}
