// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision: 3337 $</version>
// </file>

using System;
using System.Collections.Generic;
using System.Windows;
using ICSharpCode.WpfDesign.Extensions;

namespace ICSharpCode.WpfDesign.Designer.Extensions
{
	/// <summary>
	/// Supports resizing a Window.
	/// </summary>
	[ExtensionFor(typeof(Window))]
	public class WindowResizeBehavior : BehaviorExtension, IRootPlacementBehavior
	{
		protected override void OnInitialized()
		{
			base.OnInitialized();
			this.ExtendedItem.AddBehavior(typeof(IRootPlacementBehavior), this);
		}
		
		public bool CanPlace(IEnumerable<DesignItem> children, PlacementType type, PlacementAlignment position)
		{
			return type == PlacementType.Resize &&
				(position == PlacementAlignment.Right
				 || position == PlacementAlignment.BottomRight
				 || position == PlacementAlignment.Bottom);
		}
		
		
		public void BeginPlacement(PlacementOperation operation)
		{
		}
		
		public void EndPlacement(PlacementOperation operation)
		{
		}
		
		public Rect GetPosition(PlacementOperation operation, DesignItem childItem)
		{
			UIElement child = childItem.View;
			return new Rect(0, 0, ModelTools.GetWidth(child), ModelTools.GetHeight(child));
		}

		public void BeforeSetPosition(PlacementOperation operation) 
		{
		}
		
		public void SetPosition(PlacementInformation info)
		{
			UIElement element = info.Item.View;
			Rect newPosition = info.Bounds;
			if (newPosition.Right != ModelTools.GetWidth(element)) {
				info.Item.Properties[FrameworkElement.WidthProperty].SetValue(newPosition.Right);
			}
			if (newPosition.Bottom != ModelTools.GetHeight(element)) {
				info.Item.Properties[FrameworkElement.HeightProperty].SetValue(newPosition.Bottom);
			}
		}
		
		public bool CanLeaveContainer(PlacementOperation operation)
		{
			return false;
		}
		
		public void LeaveContainer(PlacementOperation operation)
		{
			throw new NotSupportedException();
		}
		
		public bool CanEnterContainer(PlacementOperation operation)
		{
			return false;
		}
		
		public void EnterContainer(PlacementOperation operation)
		{
			throw new NotSupportedException();
		}
	}
}
