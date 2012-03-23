// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
