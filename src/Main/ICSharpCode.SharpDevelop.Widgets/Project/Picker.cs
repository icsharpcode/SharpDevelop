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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls.Primitives;
using System.Windows;
using System.Windows.Input;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Data;

namespace ICSharpCode.SharpDevelop.Widgets
{
	public class Picker : Grid
	{
		public Picker()
		{
			SizeChanged += delegate { UpdateValueOffset(); };
		}

		public static readonly DependencyProperty MarkerProperty =
			DependencyProperty.Register("Marker", typeof(UIElement), typeof(Picker));

		public UIElement Marker {
			get { return (UIElement)GetValue(MarkerProperty); }
			set { SetValue(MarkerProperty, value); }
		}

		public static readonly DependencyProperty ValueProperty =
			DependencyProperty.Register("Value", typeof(double), typeof(Picker),
			                            new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

		public double Value {
			get { return (double)GetValue(ValueProperty); }
			set { SetValue(ValueProperty, value); }
		}

		public static readonly DependencyProperty ValueOffsetProperty =
			DependencyProperty.Register("ValueOffset", typeof(double), typeof(Picker));

		public double ValueOffset {
			get { return (double)GetValue(ValueOffsetProperty); }
			set { SetValue(ValueOffsetProperty, value); }
		}

		public static readonly DependencyProperty OrientationProperty =
			DependencyProperty.Register("Orientation", typeof(Orientation), typeof(Picker));

		public Orientation Orientation {
			get { return (Orientation)GetValue(OrientationProperty); }
			set { SetValue(OrientationProperty, value); }
		}

		public static readonly DependencyProperty MinimumProperty =
			DependencyProperty.Register("Minimum", typeof(double), typeof(Picker));

		public double Minimum {
			get { return (double)GetValue(MinimumProperty); }
			set { SetValue(MinimumProperty, value); }
		}

		public static readonly DependencyProperty MaximumProperty =
			DependencyProperty.Register("Maximum", typeof(double), typeof(Picker),
			                            new FrameworkPropertyMetadata(100.0));

		public double Maximum {
			get { return (double)GetValue(MaximumProperty); }
			set { SetValue(MaximumProperty, value); }
		}

		protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
		{
			base.OnPropertyChanged(e);

			if (e.Property == MarkerProperty) {
				TranslateTransform t = Marker.RenderTransform as TranslateTransform;
				if (t == null) {
					t = new TranslateTransform();
					Marker.RenderTransform = t;
				}
				var property = Orientation == Orientation.Horizontal ? TranslateTransform.XProperty : TranslateTransform.YProperty;
				BindingOperations.SetBinding(t, property, new Binding("ValueOffset") {
				                             	Source = this
				                             });
			}
			else if (e.Property == ValueProperty) {
				UpdateValueOffset();
			}
		}

		bool isMouseDown;

		protected override void OnPreviewMouseDown(MouseButtonEventArgs e)
		{
			isMouseDown = true;
			CaptureMouse();
			UpdateValue();
		}

		protected override void OnPreviewMouseMove(MouseEventArgs e)
		{
			if (isMouseDown) {
				UpdateValue();
			}
		}

		protected override void OnPreviewMouseUp(MouseButtonEventArgs e)
		{
			isMouseDown = false;
			ReleaseMouseCapture();
		}

		void UpdateValue()
		{
			Point p = Mouse.GetPosition(this);
			double length = 0, pos = 0;
			
			if (Orientation == Orientation.Horizontal) {
				length = ActualWidth;
				pos = p.X;
			}
			else {
				length = ActualHeight;
				pos = p.Y;
			}

			pos = Math.Max(0, Math.Min(length, pos));
			Value = Minimum + (Maximum - Minimum) * pos / length;
		}

		void UpdateValueOffset()
		{
			var length = Orientation == Orientation.Horizontal ? ActualWidth : ActualHeight;
			ValueOffset = length * (Value - Minimum) / (Maximum - Minimum);
		}
	}
}
