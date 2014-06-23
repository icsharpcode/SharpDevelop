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
using System.Windows.Controls;

using ICSharpCode.WpfDesign.Designer.Controls;
using ICSharpCode.WpfDesign.Extensions;

namespace ICSharpCode.WpfDesign.Designer.Extensions
{
	/// <summary>
	/// Provides <see cref="IPlacementBehavior"/> behavior for <see cref="Canvas"/>.
	/// </summary>
	[ExtensionFor(typeof(Canvas), OverrideExtension=typeof(DefaultPlacementBehavior))]
	public class CanvasPlacementSupport : SnaplinePlacementBehavior
	{
		GrayOutDesignerExceptActiveArea grayOut;
		FrameworkElement extendedComponent;
		FrameworkElement extendedView;
		
		static double GetCanvasProperty(UIElement element, DependencyProperty d)
		{
			double v = (double)element.GetValue(d);
			if (double.IsNaN(v))
				return 0;
			else
				return v;
		}
		
		static bool IsPropertySet(UIElement element, DependencyProperty d)
		{
			return element.ReadLocalValue(d) != DependencyProperty.UnsetValue;
		}
		
		protected override void OnInitialized()
		{
			base.OnInitialized();

			extendedComponent = (FrameworkElement)ExtendedItem.Component;
			extendedView = (FrameworkElement)this.ExtendedItem.View;
		}
		
		public override Rect GetPosition(PlacementOperation operation, DesignItem item)
		{
			UIElement child = item.View;

			if (child == null)
				return Rect.Empty;

			double x, y;

			if (IsPropertySet(child, Canvas.LeftProperty) || !IsPropertySet(child, Canvas.RightProperty)) {
				x = GetCanvasProperty(child, Canvas.LeftProperty);
			} else {
				x = extendedComponent.ActualWidth - GetCanvasProperty(child, Canvas.RightProperty) - child.RenderSize.Width;
			}


			if (IsPropertySet(child, Canvas.TopProperty) || !IsPropertySet(child, Canvas.BottomProperty)) {
				y = GetCanvasProperty(child, Canvas.TopProperty);
			} else {
				y = extendedComponent.ActualHeight - GetCanvasProperty(child, Canvas.BottomProperty) - child.RenderSize.Height;
			}

			var p = new Point(x, y);
			return new Rect(p, child.RenderSize);
		}
		
		public override void SetPosition(PlacementInformation info)
		{
			base.SetPosition(info);
			info.Item.Properties[FrameworkElement.MarginProperty].Reset();

			UIElement child = info.Item.View;
			Rect newPosition = info.Bounds;

			if (IsPropertySet(child, Canvas.LeftProperty) || !IsPropertySet(child, Canvas.RightProperty)) {
				if (newPosition.Left != GetCanvasProperty(child, Canvas.LeftProperty)) {
					info.Item.Properties.GetAttachedProperty(Canvas.LeftProperty).SetValue(newPosition.Left);
				}
			} else {
				var newR = extendedComponent.ActualWidth - newPosition.Right;
				if (newR != GetCanvasProperty(child, Canvas.RightProperty))
					info.Item.Properties.GetAttachedProperty(Canvas.RightProperty).SetValue(newR);
			}


			if (IsPropertySet(child, Canvas.TopProperty) || !IsPropertySet(child, Canvas.BottomProperty)) {
				if (newPosition.Top != GetCanvasProperty(child, Canvas.TopProperty)) {
					info.Item.Properties.GetAttachedProperty(Canvas.TopProperty).SetValue(newPosition.Top);
				}
			} else {
				var newB = extendedComponent.ActualHeight - newPosition.Bottom;
				if (newB != GetCanvasProperty(child, Canvas.BottomProperty))
					info.Item.Properties.GetAttachedProperty(Canvas.BottomProperty).SetValue(newB);
			}

			if (info.Item == Services.Selection.PrimarySelection)
			{
				var b = new Rect(0, 0, extendedView.ActualWidth, extendedView.ActualHeight);
				// only for primary selection:
				if (grayOut != null)
				{
					grayOut.AnimateActiveAreaRectTo(b);
				}
				else
				{
					GrayOutDesignerExceptActiveArea.Start(ref grayOut, this.Services, this.ExtendedItem.View, b);
				}
			}
		}
		
		public override void LeaveContainer(PlacementOperation operation)
		{
			GrayOutDesignerExceptActiveArea.Stop(ref grayOut);
			base.LeaveContainer(operation);
			foreach (PlacementInformation info in operation.PlacedItems) {
				info.Item.Properties.GetAttachedProperty(Canvas.LeftProperty).Reset();
				info.Item.Properties.GetAttachedProperty(Canvas.TopProperty).Reset();
			}
		}
		
		public override void EnterContainer(PlacementOperation operation)
		{
			base.EnterContainer(operation);
			foreach (PlacementInformation info in operation.PlacedItems) {
				info.Item.Properties[FrameworkElement.HorizontalAlignmentProperty].Reset();
				info.Item.Properties[FrameworkElement.VerticalAlignmentProperty].Reset();
				info.Item.Properties[FrameworkElement.MarginProperty].Reset();
			}
		}

		public override void EndPlacement(PlacementOperation operation)
		{
			GrayOutDesignerExceptActiveArea.Stop(ref grayOut);
			base.EndPlacement(operation);
		}
	}
}
