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

namespace ICSharpCode.WpfDesign.Designer.PropertyGrid
{
	public partial class PropertyGridView
	{
		public PropertyGridView()
		{
			PropertyGrid = new PropertyGrid();
			DataContext = PropertyGrid;

			InitializeComponent();

			AddHandler(Thumb.DragDeltaEvent, new DragDeltaEventHandler(dragDeltaHandler));
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

		public IEnumerable<DesignItem> SelectedItems
		{
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

		void clearButton_Click(object sender, RoutedEventArgs e)
		{
			PropertyGrid.ClearFilter();
		}

		void dragDeltaHandler(object sender, DragDeltaEventArgs e)
		{
			Thumb thumb = e.OriginalSource as Thumb;
			if (thumb != null && thumb.Name == "PART_Thumb") {
				FirstColumnWidth = Math.Max(0, FirstColumnWidth + e.HorizontalChange);
			}
		}
	}
}
