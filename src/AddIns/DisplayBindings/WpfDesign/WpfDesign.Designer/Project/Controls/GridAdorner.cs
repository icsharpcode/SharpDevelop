// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

using ICSharpCode.WpfDesign.Adorners;

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
		public const double RailDistance = 6;
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
				previewAdorner = new GridColumnSplitterAdorner(gridItem, null, null);
			} else { // vertical
				this.Width = RailSize;
				previewAdorner = new GridRowSplitterAdorner(gridItem, null, null);
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
				rp.XOffset = -(RailSize + RailDistance);
				rp.WidthOffset = RailSize + RailDistance;
				rp.WidthRelativeToContentWidth = 1;
				rp.HeightOffset = SplitterWidth;
				rp.YOffset = e.GetPosition(this).Y - SplitterWidth / 2;
			} else {
				rp.YOffset = -(RailSize + RailDistance);
				rp.HeightOffset = RailSize + RailDistance;
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
			Focus();
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
						gridItem.Services.Selection.SetSelectedComponents(new DesignItem[] { newRowDefinition }, SelectionTypes.Auto);
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
						gridItem.Services.Selection.SetSelectedComponents(new DesignItem[] { newColumnDefinition }, SelectionTypes.Auto);
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
	
	public abstract class GridSplitterAdorner : Control
	{
		public static readonly DependencyProperty IsPreviewProperty
			= DependencyProperty.Register("IsPreview", typeof(bool), typeof(GridSplitterAdorner), new PropertyMetadata(SharedInstances.BoxedFalse));
		
		protected readonly Grid grid;
		protected readonly DesignItem gridItem;
		protected readonly DesignItem firstRow, secondRow; // can also be columns
		
		internal GridSplitterAdorner(DesignItem gridItem, DesignItem firstRow, DesignItem secondRow)
		{
			Debug.Assert(gridItem != null);
			this.grid = (Grid)gridItem.Component;
			this.gridItem = gridItem;
			this.firstRow = firstRow;
			this.secondRow = secondRow;
		}
		
		public bool IsPreview {
			get { return (bool)GetValue(IsPreviewProperty); }
			set { SetValue(IsPreviewProperty, SharedInstances.Box(value)); }
		}
		
		ChangeGroup activeChangeGroup;
		double mouseStartPos;
		bool mouseIsDown;
		
		protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
		{
			e.Handled = true;
			if (CaptureMouse()) {
				Focus();
				gridItem.Services.Selection.SetSelectedComponents(new DesignItem[] { secondRow }, SelectionTypes.Auto);
				mouseStartPos = GetCoordinate(e.GetPosition(grid));
				mouseIsDown = true;
			}
		}
		
		protected override void OnMouseMove(MouseEventArgs e)
		{
			if (mouseIsDown) {
				double mousePos = GetCoordinate(e.GetPosition(grid));
				if (activeChangeGroup == null) {
					if (Math.Abs(mousePos - mouseStartPos)
					    >= GetCoordinate(new Point(SystemParameters.MinimumHorizontalDragDistance, SystemParameters.MinimumVerticalDragDistance)))
					{
						activeChangeGroup = gridItem.OpenGroup("Change grid row/column size");
						RememberOriginalSize();
					}
				}
				if (activeChangeGroup != null) {
					ChangeSize(mousePos - mouseStartPos);
				}
			}
		}
		
		protected GridLength original1, original2;
		protected double originalPixelSize1, originalPixelSize2;
		
		protected abstract double GetCoordinate(Point point);
		protected abstract void RememberOriginalSize();
		protected abstract DependencyProperty RowColumnSizeProperty { get; }
		
		void ChangeSize(double delta)
		{
			// delta = difference in pixels
			
			if (delta < -originalPixelSize1) delta = -originalPixelSize1;
			if (delta > originalPixelSize2) delta = originalPixelSize2;
			
			// replace Auto lengths with absolute lengths if necessary
			if (original1.IsAuto) original1 = new GridLength(originalPixelSize1);
			if (original2.IsAuto) original2 = new GridLength(originalPixelSize2);
			
			GridLength new1;
			if (original1.IsStar && originalPixelSize1 > 0)
				new1 = new GridLength(original1.Value * (originalPixelSize1 + delta) / originalPixelSize1, GridUnitType.Star);
			else
				new1 = new GridLength(original1.Value + delta);
			GridLength new2;
			if (original2.IsStar && originalPixelSize2 > 0)
				new2 = new GridLength(original2.Value * (originalPixelSize2 - delta) / originalPixelSize2, GridUnitType.Star);
			else
				new2 = new GridLength(original2.Value - delta);
			firstRow.Properties[RowColumnSizeProperty].SetValue(new1);
			secondRow.Properties[RowColumnSizeProperty].SetValue(new2);
			((UIElement)VisualTreeHelper.GetParent(this)).InvalidateArrange();
		}
		
		protected override void OnMouseUp(MouseButtonEventArgs e)
		{
			if (activeChangeGroup != null) {
				activeChangeGroup.Commit();
				activeChangeGroup = null;
			}
			Stop();
		}
		
		protected override void OnLostMouseCapture(MouseEventArgs e)
		{
			Stop();
		}
		
		protected override void OnKeyDown(KeyEventArgs e)
		{
			if (e.Key == Key.Escape) {
				e.Handled = true;
				Stop();
			}
		}
		
		protected void Stop()
		{
			ReleaseMouseCapture();
			mouseIsDown = false;
			if (activeChangeGroup != null) {
				activeChangeGroup.Abort();
				activeChangeGroup = null;
			}
		}
	}
	
	public class GridRowSplitterAdorner : GridSplitterAdorner
	{
		static GridRowSplitterAdorner()
		{
			DefaultStyleKeyProperty.OverrideMetadata(typeof(GridRowSplitterAdorner), new FrameworkPropertyMetadata(typeof(GridRowSplitterAdorner)));
			CursorProperty.OverrideMetadata(typeof(GridRowSplitterAdorner), new FrameworkPropertyMetadata(Cursors.SizeNS));
		}
		
		
		internal GridRowSplitterAdorner(DesignItem gridItem, DesignItem firstRow, DesignItem secondRow) : base(gridItem, firstRow, secondRow)
		{
		}
		
		protected override double GetCoordinate(Point point)
		{
			return point.Y;
		}
		
		protected override void RememberOriginalSize()
		{
			RowDefinition r1 = (RowDefinition)firstRow.Component;
			RowDefinition r2 = (RowDefinition)secondRow.Component;
			original1 = (GridLength)r1.GetValue(RowDefinition.HeightProperty);
			original2 = (GridLength)r2.GetValue(RowDefinition.HeightProperty);
			originalPixelSize1 = r1.ActualHeight;
			originalPixelSize2 = r2.ActualHeight;
		}
		
		protected override DependencyProperty RowColumnSizeProperty {
			get { return RowDefinition.HeightProperty; }
		}
	}
	
	public sealed class GridColumnSplitterAdorner : GridSplitterAdorner
	{
		static GridColumnSplitterAdorner()
		{
			DefaultStyleKeyProperty.OverrideMetadata(typeof(GridColumnSplitterAdorner), new FrameworkPropertyMetadata(typeof(GridColumnSplitterAdorner)));
			CursorProperty.OverrideMetadata(typeof(GridColumnSplitterAdorner), new FrameworkPropertyMetadata(Cursors.SizeWE));
		}
		
		internal GridColumnSplitterAdorner(DesignItem gridItem, DesignItem firstRow, DesignItem secondRow) : base(gridItem, firstRow, secondRow)
		{
		}
		
		protected override double GetCoordinate(Point point)
		{
			return point.X;
		}
		
		protected override void RememberOriginalSize()
		{
			ColumnDefinition r1 = (ColumnDefinition)firstRow.Component;
			ColumnDefinition r2 = (ColumnDefinition)secondRow.Component;
			original1 = (GridLength)r1.GetValue(ColumnDefinition.WidthProperty);
			original2 = (GridLength)r2.GetValue(ColumnDefinition.WidthProperty);
			originalPixelSize1 = r1.ActualWidth;
			originalPixelSize2 = r2.ActualWidth;
		}
		
		protected override DependencyProperty RowColumnSizeProperty {
			get { return ColumnDefinition.WidthProperty; }
		}
	}
}
