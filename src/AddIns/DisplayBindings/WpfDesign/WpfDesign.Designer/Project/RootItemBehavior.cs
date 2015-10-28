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

		public bool CanPlace(IEnumerable<DesignItem> childItems, PlacementType type, PlacementAlignment position)
		{
			return type == PlacementType.Resize && (position == PlacementAlignment.Right || position == PlacementAlignment.BottomRight || position == PlacementAlignment.Bottom);
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
		
		public bool CanEnterContainer(PlacementOperation operation, bool shouldAlwaysEnter)
		{
			return false;
		}
		
		public void EnterContainer(PlacementOperation operation)
		{
			throw new NotImplementedException();
		}

		public Point PlacePoint(Point point)
		{
			return point;
		}
	}
}
