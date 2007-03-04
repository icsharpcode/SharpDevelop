// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
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
		/// <inherits/>
		protected override void OnInitialized()
		{
			base.OnInitialized();
			this.ExtendedItem.AddBehavior(typeof(IRootPlacementBehavior), this);
		}
		
		/// <inherits/>
		public bool CanPlace(ICollection<DesignItem> children, PlacementType type, PlacementAlignment position)
		{
			return type == PlacementType.Resize &&
				(position == PlacementAlignments.Right
				 || position == PlacementAlignments.BottomRight
				 || position == PlacementAlignments.Bottom);
		}
		
		
		/// <inherits/>
		public void BeginPlacement(PlacementOperation operation)
		{
		}
		
		/// <inherits/>
		public void EndPlacement(PlacementOperation operation)
		{
		}
		
		/// <inherits/>
		public Rect GetPosition(PlacementOperation operation, DesignItem childItem)
		{
			UIElement child = childItem.View;
			return new Rect(0, 0, GetWidth(child), GetHeight(child));
		}
		
		static double GetWidth(UIElement element)
		{
			double v = (double)element.GetValue(FrameworkElement.WidthProperty);
			if (double.IsNaN(v))
				return element.RenderSize.Width;
			else
				return v;
		}
		
		static double GetHeight(UIElement element)
		{
			double v = (double)element.GetValue(FrameworkElement.HeightProperty);
			if (double.IsNaN(v))
				return element.RenderSize.Height;
			else
				return v;
		}
		
		/// <inherits/>
		public void SetPosition(PlacementInformation info)
		{
			UIElement element = info.Item.View;
			Rect newPosition = info.Bounds;
			if (newPosition.Right != GetWidth(element)) {
				info.Item.Properties[FrameworkElement.WidthProperty].SetValue(newPosition.Right);
			}
			if (newPosition.Bottom != GetHeight(element)) {
				info.Item.Properties[FrameworkElement.HeightProperty].SetValue(newPosition.Bottom);
			}
		}
		
		/// <inherits/>
		public bool CanLeaveContainer(PlacementOperation operation)
		{
			return false;
		}
		
		/// <inherits/>
		public void LeaveContainer(PlacementOperation operation)
		{
			throw new NotSupportedException();
		}
		
		/// <inherits/>
		public bool CanEnterContainer(PlacementOperation operation)
		{
			return false;
		}
		
		/// <inherits/>
		public void EnterContainer(PlacementOperation operation)
		{
			throw new NotSupportedException();
		}
	}
}
