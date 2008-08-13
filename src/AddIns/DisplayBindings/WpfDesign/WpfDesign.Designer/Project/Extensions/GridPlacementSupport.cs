// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using System.Windows;
using System.Diagnostics;
using System.Windows.Controls;
using System.Windows.Media;

using ICSharpCode.WpfDesign.Adorners;
using ICSharpCode.WpfDesign.Extensions;
using ICSharpCode.WpfDesign.Designer.Controls;

namespace ICSharpCode.WpfDesign.Designer.Extensions
{
	/// <summary>
	/// Provides <see cref="IPlacementBehavior"/> behavior for <see cref="Grid"/>.
	/// </summary>
	[ExtensionFor(typeof(Grid), OverrideExtension=typeof(DefaultPlacementBehavior))]
	public sealed class GridPlacementSupport : GuideLinePlacementBehavior
	{
		Grid grid;
		
		protected override void OnInitialized()
		{
			base.OnInitialized();
			grid = (Grid)this.ExtendedItem.Component;
		}
		
		public override Rect GetPosition(PlacementOperation operation, DesignItem child)
		{
			FrameworkElement obj = child.Component as FrameworkElement;
			if (obj == null) return new Rect();
			
			Thickness margin = obj.Margin;
			
			double left, width, right;
			switch (obj.HorizontalAlignment) {
				case HorizontalAlignment.Stretch:
					left = GetColumnOffset(Grid.GetColumn(obj)) + margin.Left;
					right = GetColumnOffset(Grid.GetColumn(obj) + Grid.GetColumnSpan(obj)) - margin.Right;
					width = right - left;
					break;
				case HorizontalAlignment.Left:
					left = GetColumnOffset(Grid.GetColumn(obj)) + margin.Left;
					width = ModelTools.GetWidth(obj);
					right = left + width;
					break;
				case HorizontalAlignment.Right:
					right = GetColumnOffset(Grid.GetColumn(obj) + Grid.GetColumnSpan(obj)) - margin.Right;
					width = ModelTools.GetWidth(obj);
					left = right - width;
					break;
				case HorizontalAlignment.Center:
					throw new NotImplementedException();
				default:
					throw new NotSupportedException();
			}
			
			double top, height, bottom;
			switch (obj.VerticalAlignment) {
				case VerticalAlignment.Stretch:
					top = GetRowOffset(Grid.GetRow(obj)) + margin.Top;
					bottom = GetRowOffset(Grid.GetRow(obj) + Grid.GetRowSpan(obj)) - margin.Bottom;
					height = bottom - top;
					break;
				case VerticalAlignment.Top:
					top = GetRowOffset(Grid.GetRow(obj)) + margin.Top;
					height = ModelTools.GetHeight(obj);
					bottom = top + height;
					break;
				case VerticalAlignment.Bottom:
					bottom = GetRowOffset(Grid.GetRow(obj) + Grid.GetRowSpan(obj)) - margin.Bottom;
					height = ModelTools.GetHeight(obj);
					top = bottom - height;
					break;
				case VerticalAlignment.Center:
					throw new NotImplementedException();
				default:
					throw new NotSupportedException();
			}
			return new Rect(left, top, Math.Max(0, width), Math.Max(0, height));
		}
		
		double GetColumnOffset(int index)
		{
			// when the grid has no columns, we still need to return 0 for index=0 and grid.Width for index=1
			if (index == 0)
				return 0;
			else if (index < grid.ColumnDefinitions.Count)
				return grid.ColumnDefinitions[index].Offset;
			else
				return grid.ActualWidth;
		}
		
		double GetRowOffset(int index)
		{
			if (index == 0)
				return 0;
			else if (index < grid.RowDefinitions.Count)
				return grid.RowDefinitions[index].Offset;
			else
				return grid.ActualHeight;
		}
		
		const double epsilon = 0.00000001;
		
		int GetColumnIndex(double x)
		{
			if (grid.ColumnDefinitions.Count == 0)
				return 0;
			for (int i = 1; i < grid.ColumnDefinitions.Count; i++) {
				if (x < grid.ColumnDefinitions[i].Offset - epsilon)
					return i - 1;
			}
			return grid.ColumnDefinitions.Count - 1;
		}
		
		int GetRowIndex(double y)
		{
			if (grid.RowDefinitions.Count == 0)
				return 0;
			for (int i = 1; i < grid.RowDefinitions.Count; i++) {
				if (y < grid.RowDefinitions[i].Offset - epsilon)
					return i - 1;
			}
			return grid.RowDefinitions.Count - 1;
		}
		
		int GetEndColumnIndex(double x)
		{
			if (grid.ColumnDefinitions.Count == 0)
				return 0;
			for (int i = 1; i < grid.ColumnDefinitions.Count; i++) {
				if (x <= grid.ColumnDefinitions[i].Offset + epsilon)
					return i - 1;
			}
			return grid.ColumnDefinitions.Count - 1;
		}
		
		int GetEndRowIndex(double y)
		{
			if (grid.RowDefinitions.Count == 0)
				return 0;
			for (int i = 1; i < grid.RowDefinitions.Count; i++) {
				if (y <= grid.RowDefinitions[i].Offset + epsilon)
					return i - 1;
			}
			return grid.RowDefinitions.Count - 1;
		}
		
		static void SetColumn(DesignItem item, int column, int columnSpan)
		{
			Debug.Assert(item != null && column >= 0 && columnSpan > 0);
			item.Properties.GetAttachedProperty(Grid.ColumnProperty).SetValue(column);
			if (columnSpan == 1) {
				item.Properties.GetAttachedProperty(Grid.ColumnSpanProperty).Reset();
			} else {
				item.Properties.GetAttachedProperty(Grid.ColumnSpanProperty).SetValue(columnSpan);
			}
		}
		
		static void SetRow(DesignItem item, int row, int rowSpan)
		{
			Debug.Assert(item != null && row >= 0 && rowSpan > 0);
			item.Properties.GetAttachedProperty(Grid.RowProperty).SetValue(row);
			if (rowSpan == 1) {
				item.Properties.GetAttachedProperty(Grid.RowSpanProperty).Reset();
			} else {
				item.Properties.GetAttachedProperty(Grid.RowSpanProperty).SetValue(rowSpan);
			}
		}
		
		static HorizontalAlignment SuggestHorizontalAlignment(Rect itemBounds, Rect availableSpaceRect)
		{
			if (itemBounds.Right < (availableSpaceRect.Left + availableSpaceRect.Right) / 2) {
				return HorizontalAlignment.Left;
			} else if (itemBounds.Left > (availableSpaceRect.Left + availableSpaceRect.Right) / 2) {
				return HorizontalAlignment.Right;
			} else {
				return HorizontalAlignment.Stretch;
			}
		}
		
		static VerticalAlignment SuggestVerticalAlignment(Rect itemBounds, Rect availableSpaceRect)
		{
			if (itemBounds.Bottom < (availableSpaceRect.Top + availableSpaceRect.Bottom) / 2) {
				return VerticalAlignment.Top;
			} else if (itemBounds.Top > (availableSpaceRect.Top + availableSpaceRect.Bottom) / 2) {
				return VerticalAlignment.Bottom;
			} else {
				return VerticalAlignment.Stretch;
			}
		}
		
		public override void SetPosition(PlacementInformation info)
		{
			base.SetPosition(info);
			if (info.Operation.Type == PlacementType.AddItem) {
				SetColumn(info.Item, GetColumnIndex(info.Bounds.Left), 1);
				SetRow(info.Item, GetRowIndex(info.Bounds.Top), 1);
			} else {
				int leftColumnIndex = GetColumnIndex(info.Bounds.Left);
				int rightColumnIndex = GetEndColumnIndex(info.Bounds.Right);
				if (rightColumnIndex < leftColumnIndex) rightColumnIndex = leftColumnIndex;
				SetColumn(info.Item, leftColumnIndex, rightColumnIndex - leftColumnIndex + 1);
				int topRowIndex = GetRowIndex(info.Bounds.Top);
				int bottomRowIndex = GetEndRowIndex(info.Bounds.Bottom);
				if (bottomRowIndex < topRowIndex) bottomRowIndex = topRowIndex;
				SetRow(info.Item, topRowIndex, bottomRowIndex - topRowIndex + 1);
				
				Rect availableSpaceRect = new Rect(
					new Point(GetColumnOffset(leftColumnIndex), GetRowOffset(topRowIndex)),
					new Point(GetColumnOffset(rightColumnIndex + 1), GetRowOffset(bottomRowIndex + 1))
				);
				
				HorizontalAlignment ha = (HorizontalAlignment)info.Item.Properties[FrameworkElement.HorizontalAlignmentProperty].ValueOnInstance;
				VerticalAlignment va = (VerticalAlignment)info.Item.Properties[FrameworkElement.VerticalAlignmentProperty].ValueOnInstance;
				ha = SuggestHorizontalAlignment(info.Bounds, availableSpaceRect);
				va = SuggestVerticalAlignment(info.Bounds, availableSpaceRect);
				
				info.Item.Properties[FrameworkElement.HorizontalAlignmentProperty].SetValue(ha);
				info.Item.Properties[FrameworkElement.VerticalAlignmentProperty].SetValue(va);
				
				Thickness margin = new Thickness(0, 0, 0, 0);
				if (ha == HorizontalAlignment.Left || ha == HorizontalAlignment.Stretch)
					margin.Left = info.Bounds.Left - GetColumnOffset(leftColumnIndex);
				if (va == VerticalAlignment.Top || va == VerticalAlignment.Stretch)
					margin.Top = info.Bounds.Top - GetRowOffset(topRowIndex);
				if (ha == HorizontalAlignment.Right || ha == HorizontalAlignment.Stretch)
					margin.Right = GetColumnOffset(rightColumnIndex + 1) - info.Bounds.Right;
				if (va == VerticalAlignment.Bottom || va == VerticalAlignment.Stretch)
					margin.Bottom = GetRowOffset(bottomRowIndex + 1) - info.Bounds.Bottom;
				info.Item.Properties[FrameworkElement.MarginProperty].SetValue(margin);
				
				if (ha == HorizontalAlignment.Stretch)
					info.Item.Properties[FrameworkElement.WidthProperty].Reset();
				else
					info.Item.Properties[FrameworkElement.WidthProperty].SetValue(info.Bounds.Width);
				if (va == VerticalAlignment.Stretch)
					info.Item.Properties[FrameworkElement.HeightProperty].Reset();
				else
					info.Item.Properties[FrameworkElement.HeightProperty].SetValue(info.Bounds.Height);
			}
		}
		
		public override void LeaveContainer(PlacementOperation operation)
		{
			base.LeaveContainer(operation);
			foreach (PlacementInformation info in operation.PlacedItems) {
				if (info.Item.ComponentType == typeof(ColumnDefinition)) {
					// TODO: combine the width of the deleted column with the previous column
					this.ExtendedItem.Properties["ColumnDefinitions"].CollectionElements.Remove(info.Item);
				} else if (info.Item.ComponentType == typeof(RowDefinition)) {
					this.ExtendedItem.Properties["RowDefinitions"].CollectionElements.Remove(info.Item);
				} else {
					info.Item.Properties.GetAttachedProperty(Grid.RowProperty).Reset();
					info.Item.Properties.GetAttachedProperty(Grid.ColumnProperty).Reset();
					info.Item.Properties.GetAttachedProperty(Grid.RowSpanProperty).Reset();
					info.Item.Properties.GetAttachedProperty(Grid.ColumnSpanProperty).Reset();

					HorizontalAlignment ha = (HorizontalAlignment)info.Item.Properties[FrameworkElement.HorizontalAlignmentProperty].ValueOnInstance;
					VerticalAlignment va = (VerticalAlignment)info.Item.Properties[FrameworkElement.VerticalAlignmentProperty].ValueOnInstance;

					if (ha == HorizontalAlignment.Stretch)
						info.Item.Properties[FrameworkElement.WidthProperty].SetValue(info.Bounds.Width);
					if (va == VerticalAlignment.Stretch)
						info.Item.Properties[FrameworkElement.HeightProperty].SetValue(info.Bounds.Height);
				}
			}
		}
	}
}
