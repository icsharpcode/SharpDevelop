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
			InitializeComponent();

			PropertyGrid = new PropertyGrid();
			uxDataContextHolder.DataContext = PropertyGrid;
			uxThumb.DragDelta += new DragDeltaEventHandler(thumb_DragDelta);
		}

		static PropertyContextMenu propertyContextMenu = new PropertyContextMenu();

		public PropertyGrid PropertyGrid { get; private set; }

		public static readonly DependencyProperty ContextProperty =
		   DesignSurface.ContextProperty.AddOwner(typeof(PropertyGridView));

		public DesignContext Context
		{
			get { return (DesignContext)GetValue(ContextProperty); }
			set { SetValue(ContextProperty, value); }
		}

		public static readonly DependencyProperty FirstColumnWidthProperty =
			DependencyProperty.Register("FirstColumnWidth", typeof(double), typeof(PropertyGridView),
			new PropertyMetadata(120.0));

		public double FirstColumnWidth {
			get { return (double)GetValue(FirstColumnWidthProperty); }
			set { SetValue(FirstColumnWidthProperty, value); }
		}

		protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
		{
			base.OnPropertyChanged(e);
			if (e.Property == ContextProperty) {
				OnContextChanged(e.OldValue as DesignContext, e.NewValue as DesignContext);
			}
		}

		void OnContextChanged(DesignContext oldContext, DesignContext newContext)
		{
			if (newContext != null) {
				newContext.SelectionService.SelectionChanged += SelectionService_SelectionChanged;
			}
			else if (oldContext != null) {
				oldContext.SelectionService.SelectionChanged -= SelectionService_SelectionChanged;
			}
		}

		void SelectionService_SelectionChanged(object sender, DesignItemCollectionEventArgs e)
		{
			PropertyGrid.SelectedItems = Context.SelectionService.SelectedItems;
		}

		protected override void OnMouseRightButtonDown(MouseButtonEventArgs e)
		{
			var row = (e.OriginalSource as DependencyObject).FindAncestor<Border>("uxPropertyNodeRow");
			if (row == null) return;

			var node = row.DataContext as PropertyNode;
			if (node.IsEvent) return;

			var contextMenu = new PropertyContextMenu();
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
