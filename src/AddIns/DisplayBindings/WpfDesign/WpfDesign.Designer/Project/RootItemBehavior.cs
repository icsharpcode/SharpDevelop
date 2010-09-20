// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Windows;
using System.Collections.Generic;
using System.Diagnostics;

namespace ICSharpCode.WpfDesign.Designer
{
	/// <summary>
	/// Intializes different behaviors for the Root item.
	/// <remarks>Could not be a extension since Root Item is can be of any type</remarks>
	/// </summary>
	public class RootItemBehavior : IRootPlacementBehavior
	{
		private DesignItem _rootItem;
		
		public void Intialize(DesignContext context)
		{
			Debug.Assert(context.RootItem!=null);
			this._rootItem=context.RootItem;
			_rootItem.AddBehavior(typeof(IRootPlacementBehavior),this);
		}
		
		public bool CanPlace(System.Collections.Generic.ICollection<DesignItem> childItems, PlacementType type, PlacementAlignment position)
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
		
		public System.Windows.Rect GetPosition(PlacementOperation operation, DesignItem childItem)
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
			throw new NotImplementedException();
		}
		
		public bool CanEnterContainer(PlacementOperation operation)
		{
			return false;
		}
		
		public void EnterContainer(PlacementOperation operation)
		{
			throw new NotImplementedException();
		}
	}
}
