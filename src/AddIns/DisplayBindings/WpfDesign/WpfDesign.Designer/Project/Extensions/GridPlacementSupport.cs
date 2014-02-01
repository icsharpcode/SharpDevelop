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
	public sealed class GridPlacementSupport : SnaplinePlacementBehavior
	{
		Grid grid;
		private bool enteredIntoNewContainer;
		
		protected override void OnInitialized()
		{
			base.OnInitialized();
			grid = (Grid)this.ExtendedItem.Component;
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
			bool isLeft = itemBounds.Left < availableSpaceRect.Left + availableSpaceRect.Width / 4;
			bool isRight = itemBounds.Right > availableSpaceRect.Right - availableSpaceRect.Width / 4;
			if (isLeft && isRight)
				return HorizontalAlignment.Stretch;
			else if (isRight)
				return HorizontalAlignment.Right;
			else
				return HorizontalAlignment.Left;
		}
		
		static VerticalAlignment SuggestVerticalAlignment(Rect itemBounds, Rect availableSpaceRect)
		{
			bool isTop = itemBounds.Top < availableSpaceRect.Top + availableSpaceRect.Height / 4;
			bool isBottom = itemBounds.Bottom > availableSpaceRect.Bottom - availableSpaceRect.Height / 4;
			if (isTop && isBottom)
				return VerticalAlignment.Stretch;
			else if (isBottom)
				return VerticalAlignment.Bottom;
			else
				return VerticalAlignment.Top;
		}
		
		public override void EnterContainer(PlacementOperation operation)
		{
			enteredIntoNewContainer=true;
			grid.UpdateLayout();
			base.EnterContainer(operation);
		}
		
		GrayOutDesignerExceptActiveArea grayOut;
		
		public override void EndPlacement(PlacementOperation operation)
		{
			GrayOutDesignerExceptActiveArea.Stop(ref grayOut);
			enteredIntoNewContainer=false;
			base.EndPlacement(operation);
		}
		
		public override void SetPosition(PlacementInformation info)
		{
			base.SetPosition(info);
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
			if (info.Item == Services.Selection.PrimarySelection) {
				// only for primary selection:
				if (grayOut != null) {
					grayOut.AnimateActiveAreaRectTo(availableSpaceRect);
				} else {
					GrayOutDesignerExceptActiveArea.Start(ref grayOut, this.Services, this.ExtendedItem.View, availableSpaceRect);
				}
			}
			
			HorizontalAlignment ha = (HorizontalAlignment)info.Item.Properties[FrameworkElement.HorizontalAlignmentProperty].ValueOnInstance;
			VerticalAlignment va = (VerticalAlignment)info.Item.Properties[FrameworkElement.VerticalAlignmentProperty].ValueOnInstance;
			if(enteredIntoNewContainer){
				ha = SuggestHorizontalAlignment(info.Bounds, availableSpaceRect);
				va = SuggestVerticalAlignment(info.Bounds, availableSpaceRect);
			}
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
			
			var widthIsSet = info.Item.Properties[FrameworkElement.WidthProperty].IsSet;
			var heightIsSet = info.Item.Properties[FrameworkElement.HeightProperty].IsSet;
			if (!widthIsSet)
			{
				if (ha == HorizontalAlignment.Stretch)
					info.Item.Properties[FrameworkElement.WidthProperty].Reset();
				else
					info.Item.Properties[FrameworkElement.WidthProperty].SetValue(info.Bounds.Width);
			}
			else {
				info.Item.Properties[FrameworkElement.WidthProperty].SetValue(info.Bounds.Width);
			}
			if (!heightIsSet)
			{
				if (va == VerticalAlignment.Stretch)
					info.Item.Properties[FrameworkElement.HeightProperty].Reset();
				else
					info.Item.Properties[FrameworkElement.HeightProperty].SetValue(info.Bounds.Height);
			}
			else {
				info.Item.Properties[FrameworkElement.HeightProperty].SetValue(info.Bounds.Height);
			}
		}
		
		public override void LeaveContainer(PlacementOperation operation)
		{
			GrayOutDesignerExceptActiveArea.Stop(ref grayOut);
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
