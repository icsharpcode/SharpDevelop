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
	[TemplatePart(Name = "PART_Thumb", Type = typeof(Thumb))]
	public class PropertyGridView : Control
	{
		static PropertyGridView()
		{
			DefaultStyleKeyProperty.OverrideMetadata(typeof(PropertyGridView), new FrameworkPropertyMetadata(typeof(PropertyGridView)));
		}
		
		
		public PropertyGridView()
		{
			PropertyGrid = new PropertyGrid();
			DataContext = PropertyGrid;
		}
		
		private Thumb thumb;
		public override void OnApplyTemplate()
		{
			thumb = GetTemplateChild("PART_Thumb") as Thumb;

			thumb.DragDelta += new DragDeltaEventHandler(thumb_DragDelta);

			base.OnApplyTemplate();
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

		void thumb_DragDelta(object sender, DragDeltaEventArgs e)
		{
			FirstColumnWidth = Math.Max(0, FirstColumnWidth + e.HorizontalChange);
		}
	}
}
