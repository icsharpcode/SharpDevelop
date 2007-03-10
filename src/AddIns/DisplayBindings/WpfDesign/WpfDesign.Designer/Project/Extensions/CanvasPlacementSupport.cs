// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

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
	[ExtensionFor(typeof(Canvas))]
	public sealed class CanvasPlacementSupport : BehaviorExtension, IPlacementBehavior
	{
		protected override void OnInitialized()
		{
			base.OnInitialized();
			this.ExtendedItem.AddBehavior(typeof(IPlacementBehavior), this);
		}
		
		GrayOutDesignerExceptActiveArea grayOut;
		
		public bool CanPlace(ICollection<DesignItem> child, PlacementType type, PlacementAlignment position)
		{
			return type == PlacementType.Resize || type == PlacementType.Move
				|| type == PlacementType.Delete
				|| type == PlacementType.AddItem;
		}
		
		public Rect GetPosition(PlacementOperation operation, DesignItem childItem)
		{
			UIElement child = childItem.View;
			return new Rect(GetLeft(child), GetTop(child), ModelTools.GetWidth(child), ModelTools.GetHeight(child));
		}
		
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
		
		public void SetPosition(PlacementInformation info)
		{
			UIElement child = info.Item.View;
			Rect newPosition = info.Bounds;
			if (newPosition.Left != GetLeft(child)) {
				info.Item.Properties.GetAttachedProperty(Canvas.LeftProperty).SetValue(newPosition.Left);
			}
			if (newPosition.Top != GetTop(child)) {
				info.Item.Properties.GetAttachedProperty(Canvas.TopProperty).SetValue(newPosition.Top);
			}
			if (newPosition.Width != ModelTools.GetWidth(child)) {
				info.Item.Properties.GetProperty(FrameworkElement.WidthProperty).SetValue(newPosition.Right - newPosition.Left);
			}
			if (newPosition.Height != ModelTools.GetHeight(child)) {
				info.Item.Properties.GetProperty(FrameworkElement.HeightProperty).SetValue(newPosition.Bottom - newPosition.Top);
			}
		}
		
		public void BeginPlacement(PlacementOperation op)
		{
			if (op.Type == PlacementType.Move || op.Type == PlacementType.Resize) {
				GrayOutDesignerExceptActiveArea.Start(ref grayOut, this.Services, this.ExtendedItem.View);
			}
		}
		
		public void EndPlacement(PlacementOperation op)
		{
			GrayOutDesignerExceptActiveArea.Stop(ref grayOut);
		}
		
		public bool CanLeaveContainer(PlacementOperation operation)
		{
			return true;
		}
		
		public void LeaveContainer(PlacementOperation operation)
		{
			EndPlacement(operation);
			foreach (PlacementInformation info in operation.PlacedItems) {
				this.ExtendedItem.Properties["Children"].CollectionElements.Remove(info.Item);
				info.Item.Properties.GetAttachedProperty(Canvas.LeftProperty).Reset();
				info.Item.Properties.GetAttachedProperty(Canvas.TopProperty).Reset();
			}
		}
		
		public bool CanEnterContainer(PlacementOperation operation)
		{
			return true;
		}
		
		public void EnterContainer(PlacementOperation operation)
		{
			foreach (PlacementInformation info in operation.PlacedItems) {
				this.ExtendedItem.Properties["Children"].CollectionElements.Add(info.Item);
				SetPosition(info);
			}
			BeginPlacement(operation);
		}
	}
}
