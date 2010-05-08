// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Siegfried Pammer" email="sie_pam@gmx.at"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace ICSharpCode.Profiler.Controls
{
	public class PercentBar : FrameworkElement
	{
		protected override void OnRender(DrawingContext drawingContext)
		{
			base.OnRender(drawingContext);
			
			if (this.RenderSize.Height > 0 && this.RenderSize.Width > 0) {
				drawingContext.DrawRectangle(new LinearGradientBrush(Colors.Red, Colors.Orange, 0), new Pen(Brushes.Black, 1), new Rect(new Point(0, 2), new Size(this.RenderSize.Width * this.Value, this.RenderSize.Height - 4)));
			}
		}
		
		public static readonly DependencyProperty ValueProperty =
			DependencyProperty.Register("Value", typeof(double), typeof(PercentBar),
			                            new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsMeasure));

		
		public double Value
		{
			get { return (double)GetValue(ValueProperty); }
			set { SetValue(ValueProperty, value); }
		}
	}
}
