using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows;

namespace SharpDevelop.XamlDesigner.Controls
{
	public class LayersPanel : Grid
	{
		public static readonly DependencyProperty SelectedIndexProperty =
			DependencyProperty.Register("SelectedIndex", typeof(int), typeof(LayersPanel));

		public int SelectedIndex
		{
			get { return (int)GetValue(SelectedIndexProperty); }
			set { SetValue(SelectedIndexProperty, value); }
		}

		protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
		{
			base.OnPropertyChanged(e);
			if (e.Property == SelectedIndexProperty) {
				UpdateVisibility();
			}
		}

		protected override void OnVisualChildrenChanged(DependencyObject visualAdded, DependencyObject visualRemoved)
		{
			base.OnVisualChildrenChanged(visualAdded, visualRemoved);
			UpdateVisibility();
		}

		void UpdateVisibility()
		{
			for (int i = 0; i < Children.Count; i++) {
				if (i == SelectedIndex) {
					Children[i].Visibility = Visibility.Visible;
				}
				else {
					Children[i].Visibility = Visibility.Hidden;
				}
			}
		}
	}
}
