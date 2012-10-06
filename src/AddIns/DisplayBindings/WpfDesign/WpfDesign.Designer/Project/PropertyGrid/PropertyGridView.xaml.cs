// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Controls.Primitives;
using ICSharpCode.WpfDesign.PropertyGrid;

namespace ICSharpCode.WpfDesign.Designer.PropertyGrid
{
	public partial class PropertyGridView
	{
		public PropertyGridView()
		{
			PropertyGrid = new PropertyGrid();
			DataContext = PropertyGrid;

			InitializeComponent();

			thumb.DragDelta += new DragDeltaEventHandler(thumb_DragDelta);
		}

		static PropertyContextMenu propertyContextMenu = new PropertyContextMenu();

		public PropertyGrid PropertyGrid { get; private set; }

		public static readonly DependencyProperty FirstColumnWidthProperty =
			DependencyProperty.Register("FirstColumnWidth", typeof(double), typeof(PropertyGridView),
			new PropertyMetadata(120.0));

		public double FirstColumnWidth {
			get { return (double)GetValue(FirstColumnWidthProperty); }
			set { SetValue(FirstColumnWidthProperty, value); }
		}

		public static readonly DependencyProperty SelectedItemsProperty =
			DependencyProperty.Register("SelectedItems", typeof(IEnumerable<DesignItem>), typeof(PropertyGridView));

		public IEnumerable<DesignItem> SelectedItems {
			get { return (IEnumerable<DesignItem>)GetValue(SelectedItemsProperty); }
			set { SetValue(SelectedItemsProperty, value); }
		}

		protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
		{
			base.OnPropertyChanged(e);
			if (e.Property == SelectedItemsProperty) {
				PropertyGrid.SelectedItems = SelectedItems;
			}
		}

		protected override void OnMouseRightButtonUp(MouseButtonEventArgs e)
		{
			var ancestors = (e.OriginalSource as DependencyObject).GetVisualAncestors();
			Border row = ancestors.OfType<Border>().FirstOrDefault(b => b.Name == "uxPropertyNodeRow");
			if (row == null) return;

			PropertyNode node = row.DataContext as PropertyNode;
			if (node.IsEvent) return;

			PropertyContextMenu contextMenu = new PropertyContextMenu();
			contextMenu.DataContext = node;
			contextMenu.Placement = PlacementMode.Bottom;
			contextMenu.HorizontalOffset = -30;
			contextMenu.PlacementTarget = row;
			contextMenu.IsOpen = true;
		}

		void clearButton_Click(object sender, RoutedEventArgs e)
		{
			PropertyGrid.ClearFilter();
		}

		void thumb_DragDelta(object sender, DragDeltaEventArgs e)
		{
			FirstColumnWidth = Math.Max(0, FirstColumnWidth + e.HorizontalChange);
		}
	}
}
