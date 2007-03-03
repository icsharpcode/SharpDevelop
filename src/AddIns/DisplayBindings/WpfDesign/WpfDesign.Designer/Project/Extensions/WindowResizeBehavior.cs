// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;
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
		public bool CanPlace(DesignItem child, PlacementType type, PlacementAlignment position)
		{
			return type == PlacementType.Resize &&
				(position == PlacementAlignments.Right
				 || position == PlacementAlignments.BottomRight
				 || position == PlacementAlignments.Bottom);
		}
		
		/// <inherits/>
		public void StartPlacement(PlacementOperation operation)
		{
			UIElement element = (UIElement)operation.PlacedItem.Component;
			operation.Left = 0;
			operation.Top = 0;
			operation.Right = GetWidth(element);
			operation.Bottom = GetHeight(element);
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
		public void UpdatePlacement(PlacementOperation operation)
		{
			UIElement element = (UIElement)operation.PlacedItem.Component;
			if (operation.Right != GetWidth(element)) {
				operation.PlacedItem.Properties[FrameworkElement.WidthProperty].SetValue(operation.Right);
			}
			if (operation.Bottom != GetHeight(element)) {
				operation.PlacedItem.Properties[FrameworkElement.HeightProperty].SetValue(operation.Bottom);
			}
		}
		
		/// <inherits/>
		public void FinishPlacement(PlacementOperation operation)
		{
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
