// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
	public sealed class CanvasPlacementSupport : SnaplinePlacementBehavior
	{
		static double GetLeft(UIElement element)
		{
			double v = (double)element.GetValue(Canvas.LeftProperty);
			if (double.IsNaN(v))
				return 0;
			else
				return v;
		}
		
		static double GetTop(UIElement element)
		{
			double v = (double)element.GetValue(Canvas.TopProperty);
			if (double.IsNaN(v))
				return 0;
			else
				return v;
		}
		
		public override void SetPosition(PlacementInformation info)
		{
			base.SetPosition(info);
			info.Item.Properties[FrameworkElement.MarginProperty].Reset();

			UIElement child = info.Item.View;
			Rect newPosition = info.Bounds;

			if (newPosition.Left != GetLeft(child)) {
				info.Item.Properties.GetAttachedProperty(Canvas.LeftProperty).SetValue(newPosition.Left);
			}
			if (newPosition.Top != GetTop(child)) {
				info.Item.Properties.GetAttachedProperty(Canvas.TopProperty).SetValue(newPosition.Top);
			}
		}
		
		public override void LeaveContainer(PlacementOperation operation)
		{
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
	}
}
