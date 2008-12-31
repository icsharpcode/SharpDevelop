using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;

namespace SharpDevelop.XamlDesigner.Controls
{
	class DashedLine : FrameworkElement
	{
		public static readonly DependencyProperty Point1Property =
			DependencyProperty.Register("Point1", typeof(Point), typeof(DashedLine));

		public Point Point1
		{
			get { return (Point)GetValue(Point1Property); }
			set { SetValue(Point1Property, value); }
		}

		public static readonly DependencyProperty Point2Property =
			DependencyProperty.Register("Point2", typeof(Point), typeof(DashedLine));

		public Point Point2
		{
			get { return (Point)GetValue(Point2Property); }
			set { SetValue(Point2Property, value); }
		}

		public static readonly DependencyProperty BackgroundPenProperty =
			DependencyProperty.Register("BackgroundPen", typeof(Pen), typeof(DashedLine));

		public Pen BackgroundPen
		{
			get { return (Pen)GetValue(BackgroundPenProperty); }
			set { SetValue(BackgroundPenProperty, value); }
		}

		public static readonly DependencyProperty ForegroundPenProperty =
			DependencyProperty.Register("ForegroundPen", typeof(Pen), typeof(DashedLine));

		public Pen ForegroundPen
		{
			get { return (Pen)GetValue(ForegroundPenProperty); }
			set { SetValue(ForegroundPenProperty, value); }
		}

		protected override void OnRender(DrawingContext drawingContext)
		{
			if (BackgroundPen != null) {
				drawingContext.DrawLine(BackgroundPen, Point1, Point2);
			}
			if (ForegroundPen != null) {
				//var dashStyle = ForegroundPen.DashStyle.Clone();
				//dashStyle.Offset = Point1.X + Point1.Y;
				//ForegroundPen.DashStyle = dashStyle;
				drawingContext.DrawLine(ForegroundPen, Point1, Point2);
			}
		}
	}
}
