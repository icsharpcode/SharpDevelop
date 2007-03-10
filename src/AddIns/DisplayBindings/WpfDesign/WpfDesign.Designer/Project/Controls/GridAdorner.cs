// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using System.Diagnostics;
using ICSharpCode.WpfDesign.Adorners;
using ICSharpCode.WpfDesign.Extensions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Shapes;
using System.Windows.Media;
using System.Windows.Input;

namespace ICSharpCode.WpfDesign.Designer.Controls
{
	/// <summary>
	/// Adorner that displays the blue bar next to grids that can be used to create new rows/column.
	/// </summary>
	public class GridRailAdorner : Control
	{
		static GridRailAdorner()
		{
			DefaultStyleKeyProperty.OverrideMetadata(typeof(GridRailAdorner), new FrameworkPropertyMetadata(typeof(GridRailAdorner)));
		}
		
		readonly DesignItem gridItem;
		readonly Grid grid;
		readonly AdornerPanel adornerPanel;
		readonly GridSplitterAdorner previewAdorner;
		readonly Orientation orientation;
		
		public const double RailSize = 10;
		public const double SplitterWidth = 10;
		
		public GridRailAdorner(DesignItem gridItem, AdornerPanel adornerPanel, Orientation orientation)
		{
			Debug.Assert(gridItem != null);
			Debug.Assert(adornerPanel != null);
			
			this.gridItem = gridItem;
			this.grid = (Grid)gridItem.Component;
			this.adornerPanel = adornerPanel;
			this.orientation = orientation;
			
			if (orientation == Orientation.Horizontal) {
				this.Height = RailSize;
				previewAdorner = new GridColumnSplitterAdorner();
			} else { // vertical
				this.Width = RailSize;
				previewAdorner = new GridRowSplitterAdorner();
			}
			previewAdorner.IsPreview = true;
			previewAdorner.IsHitTestVisible = false;
		}
		
		#region Handle mouse events to add a new row/column
		protected override void OnMouseEnter(MouseEventArgs e)
		{
			base.OnMouseEnter(e);
			adornerPanel.Children.Add(previewAdorner);
		}
		
		protected override void OnMouseMove(MouseEventArgs e)
		{
			base.OnMouseMove(e);
			RelativePlacement rp = new RelativePlacement();
			if (orientation == Orientation.Vertical) {
				rp.XOffset = -RailSize;
				rp.WidthOffset = RailSize;
				rp.WidthRelativeToContentWidth = 1;
				rp.HeightOffset = SplitterWidth;
				rp.YOffset = e.GetPosition(this).Y - SplitterWidth / 2;
			} else {
				rp.YOffset = -RailSize;
				rp.HeightOffset = RailSize;
				rp.HeightRelativeToContentHeight = 1;
				rp.WidthOffset = SplitterWidth;
				rp.XOffset = e.GetPosition(this).X - SplitterWidth / 2;
			}
			AdornerPanel.SetPlacement(previewAdorner, rp);
		}
		
		protected override void OnMouseLeave(MouseEventArgs e)
		{
			base.OnMouseLeave(e);
			adornerPanel.Children.Remove(previewAdorner);
		}
		
		protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
		{
			base.OnMouseLeftButtonDown(e);
			e.Handled = true;
			adornerPanel.Children.Remove(previewAdorner);
			if (orientation == Orientation.Vertical) {
				using (ChangeGroup changeGroup = gridItem.OpenGroup("Split grid row")) {
					DesignItemProperty rowCollection = gridItem.Properties["RowDefinitions"];
					if (rowCollection.CollectionElements.Count == 0) {
						DesignItem firstRow = gridItem.Services.Component.RegisterComponentForDesigner(new RowDefinition());
						rowCollection.CollectionElements.Add(firstRow);
						grid.UpdateLayout(); // let WPF assign firstRow.ActualHeight
					}
					double insertionPosition = e.GetPosition(this).Y;
					for (int i = 0; i < grid.RowDefinitions.Count; i++) {
						RowDefinition row = grid.RowDefinitions[i];
						if (row.Offset > insertionPosition) continue;
						if (row.Offset + row.ActualHeight < insertionPosition) continue;
						
						// split row
						GridLength oldLength = (GridLength)row.GetValue(RowDefinition.HeightProperty);
						GridLength newLength1, newLength2;
						SplitLength(oldLength, insertionPosition - row.Offset, row.ActualHeight, out newLength1, out newLength2);
						DesignItem newRowDefinition = gridItem.Services.Component.RegisterComponentForDesigner(new RowDefinition());
						rowCollection.CollectionElements.Insert(i + 1, newRowDefinition);
						rowCollection.CollectionElements[i].Properties[RowDefinition.HeightProperty].SetValue(newLength1);
						newRowDefinition.Properties[RowDefinition.HeightProperty].SetValue(newLength2);
						changeGroup.Commit();
						break;
					}
				}
			} else {
				using (ChangeGroup changeGroup = gridItem.OpenGroup("Split grid column")) {
					DesignItemProperty columnCollection = gridItem.Properties["ColumnDefinitions"];
					if (columnCollection.CollectionElements.Count == 0) {
						DesignItem firstColumn = gridItem.Services.Component.RegisterComponentForDesigner(new ColumnDefinition());
						columnCollection.CollectionElements.Add(firstColumn);
						grid.UpdateLayout(); // let WPF assign firstColumn.ActualWidth
					}
					double insertionPosition = e.GetPosition(this).X;
					for (int i = 0; i < grid.ColumnDefinitions.Count; i++) {
						ColumnDefinition column = grid.ColumnDefinitions[i];
						if (column.Offset > insertionPosition) continue;
						if (column.Offset + column.ActualWidth < insertionPosition) continue;
						
						// split column
						GridLength oldLength = (GridLength)column.GetValue(ColumnDefinition.WidthProperty);
						GridLength newLength1, newLength2;
						SplitLength(oldLength, insertionPosition - column.Offset, column.ActualWidth, out newLength1, out newLength2);
						columnCollection.CollectionElements[i].Properties[ColumnDefinition.WidthProperty].SetValue(newLength1);
						DesignItem newColumnDefinition = gridItem.Services.Component.RegisterComponentForDesigner(new ColumnDefinition());
						newColumnDefinition.Properties[ColumnDefinition.WidthProperty].SetValue(newLength2);
						columnCollection.CollectionElements.Insert(i + 1, newColumnDefinition);
						changeGroup.Commit();
						break;
					}
				}
			}
		}
		
		void SplitLength(GridLength oldLength, double insertionPosition, double oldActualValue,
		                 out GridLength newLength1, out GridLength newLength2)
		{
			if (oldLength.IsAuto) {
				oldLength = new GridLength(oldActualValue);
			}
			double percentage = insertionPosition / oldActualValue;
			newLength1 = new GridLength(oldLength.Value * percentage, oldLength.GridUnitType);
			newLength2 = new GridLength(oldLength.Value - newLength1.Value, oldLength.GridUnitType);
		}
		#endregion
	}
	
	public class GridSplitterAdorner : Control
	{
		public static readonly DependencyProperty IsPreviewProperty
			= DependencyProperty.Register("IsPreview", typeof(bool), typeof(GridSplitterAdorner), new PropertyMetadata(SharedInstances.BoxedFalse));
		
		public bool IsPreview {
			get { return (bool)GetValue(IsPreviewProperty); }
			set { SetValue(IsPreviewProperty, SharedInstances.Box(value)); }
		}
	}
	
	public class GridRowSplitterAdorner : GridSplitterAdorner
	{
		static GridRowSplitterAdorner()
		{
			DefaultStyleKeyProperty.OverrideMetadata(typeof(GridRowSplitterAdorner), new FrameworkPropertyMetadata(typeof(GridRowSplitterAdorner)));
			CursorProperty.OverrideMetadata(typeof(GridRowSplitterAdorner), new FrameworkPropertyMetadata(Cursors.SizeNS));
		}
	}
	
	public class GridColumnSplitterAdorner : GridSplitterAdorner
	{
		static GridColumnSplitterAdorner()
		{
			DefaultStyleKeyProperty.OverrideMetadata(typeof(GridColumnSplitterAdorner), new FrameworkPropertyMetadata(typeof(GridColumnSplitterAdorner)));
			CursorProperty.OverrideMetadata(typeof(GridColumnSplitterAdorner), new FrameworkPropertyMetadata(Cursors.SizeWE));
		}
	}
}
