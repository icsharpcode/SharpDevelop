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
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;

namespace ICSharpCode.Core.Presentation
{
	/// <summary>
	/// Cancels the desired size of the child elements. Use this control around scrolling containers (e.g. ListBox) used
	/// inside auto-scroll contexts.
	/// </summary>
	public class RestrictDesiredSize : Decorator
	{
		public static readonly DependencyProperty RestrictWidthProperty =
			DependencyProperty.Register("RestrictWidth", typeof(bool), typeof(RestrictDesiredSize),
			                            new FrameworkPropertyMetadata(true, FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsArrange));
		
		public bool RestrictWidth {
			get { return (bool)GetValue(RestrictWidthProperty); }
			set { SetValue(RestrictWidthProperty, value); }
		}
		
		public static readonly DependencyProperty RestrictHeightProperty =
			DependencyProperty.Register("RestrictHeight", typeof(bool), typeof(RestrictDesiredSize),
			                            new FrameworkPropertyMetadata(true, FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsArrange));
		
		public bool RestrictHeight {
			get { return (bool)GetValue(RestrictHeightProperty); }
			set { SetValue(RestrictHeightProperty, value); }
		}
		
		Size lastArrangeSize = new Size(double.PositiveInfinity, double.PositiveInfinity);
		Size lastMeasureSize = new Size(double.NaN, double.NaN);

		protected override Size MeasureOverride(Size constraint)
		{
			if (RestrictWidth && RestrictHeight)
				return new Size(0, 0);
			
			if (RestrictWidth && constraint.Width > lastArrangeSize.Width)
				constraint.Width = lastArrangeSize.Width;
			if (RestrictHeight && constraint.Height > lastArrangeSize.Height)
				constraint.Height = lastArrangeSize.Height;
			lastMeasureSize = constraint;
			Size baseSize = base.MeasureOverride(constraint);
			return new Size(RestrictWidth ? 0 : baseSize.Width, RestrictHeight ? 0 : baseSize.Height);
		}
		
		protected override Size ArrangeOverride(Size arrangeSize)
		{
			if (lastMeasureSize != arrangeSize) {
				lastMeasureSize = arrangeSize;
				base.MeasureOverride(arrangeSize);
			}
			lastArrangeSize = arrangeSize;
			return base.ArrangeOverride(arrangeSize);
		}
	}
}
