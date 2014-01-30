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
using System.Windows;
using System.Windows.Media;
using System.Windows.Controls;

namespace ICSharpCode.WpfDesign.Designer.PropertyGrid.Editors.BrushEditor
{
	public class NormalizedPanel : Panel
	{
		public static double GetX(DependencyObject obj)
		{
			return (double)obj.GetValue(XProperty);
		}

		public static void SetX(DependencyObject obj, double value)
		{
			obj.SetValue(XProperty, value);
		}

		public static readonly DependencyProperty XProperty =
			DependencyProperty.RegisterAttached("X", typeof(double), typeof(NormalizedPanel),
			                                    new PropertyMetadata(OnPositioningChanged));

		public static double GetY(DependencyObject obj)
		{
			return (double)obj.GetValue(YProperty);
		}

		public static void SetY(DependencyObject obj, double value)
		{
			obj.SetValue(YProperty, value);
		}

		public static readonly DependencyProperty YProperty =
			DependencyProperty.RegisterAttached("Y", typeof(double), typeof(NormalizedPanel),
			                                    new PropertyMetadata(OnPositioningChanged));

		static void OnPositioningChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			NormalizedPanel parent = VisualTreeHelper.GetParent(d) as NormalizedPanel;
			if (parent != null) {
				parent.InvalidateArrange();
			}
		}

		protected override Size MeasureOverride(Size availableSize)
		{
			foreach (UIElement item in Children) {
				item.Measure(availableSize);
			}
			return new Size();
		}

		protected override Size ArrangeOverride(Size finalSize)
		{
			foreach (UIElement item in Children) {
				Rect r = new Rect(item.DesiredSize);
				r.X = GetX(item) * finalSize.Width - item.DesiredSize.Width / 2;
				r.Y = GetY(item) * finalSize.Height - item.DesiredSize.Height / 2;
				item.Arrange(r);
			}
			return finalSize;
		}
	}
}
