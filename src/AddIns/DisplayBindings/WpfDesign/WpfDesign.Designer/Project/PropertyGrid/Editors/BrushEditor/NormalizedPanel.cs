// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
