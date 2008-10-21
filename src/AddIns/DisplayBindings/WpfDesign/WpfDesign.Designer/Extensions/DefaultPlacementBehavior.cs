using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ICSharpCode.WpfDesign.Extensions;
using System.Windows.Controls;
using System.Windows;
using ICSharpCode.WpfDesign.Designer.Controls;
using System.Diagnostics;
using System.Windows.Media;

namespace ICSharpCode.WpfDesign.Designer.Extensions
{
	[ExtensionFor(typeof(UIElement))]
	public class DefaultPlacementBehavior : BehaviorExtension, IPlacementBehavior
	{
		protected override void OnInitialized()
		{
			base.OnInitialized();
			if (ExtendedItem.Content == null ||
				Metadata.IsPlacementDisabled(ExtendedItem.ComponentType)) 
				return;
			ExtendedItem.AddBehavior(typeof(IPlacementBehavior), this);
		}

		public virtual bool CanPlace(IEnumerable<DesignItem> childItems, PlacementType type, PlacementAlignment position)
		{
			return true;
		}

		public virtual void BeginPlacement(PlacementOperation operation)
		{
		}

		public virtual void EndPlacement(PlacementOperation operation)
		{
		}

		public virtual Rect GetPosition(PlacementOperation operation, DesignItem item)
		{
			var p = item.View.TranslatePoint(new Point(), operation.CurrentContainer.View);
			return new Rect(p, item.View.RenderSize);
		}

		public virtual void BeforeSetPosition(PlacementOperation operation) 
		{
		}

		public virtual void SetPosition(PlacementInformation info)
		{
			ModelTools.Resize(info.Item, info.Bounds.Width, info.Bounds.Height);
		}

		public virtual bool CanLeaveContainer(PlacementOperation operation)
		{
			return true;
		}

		public virtual void LeaveContainer(PlacementOperation operation)
		{
			if (ExtendedItem.Content.IsCollection) {
				foreach (var info in operation.PlacedItems) {
					ExtendedItem.Content.CollectionElements.Remove(info.Item);
				}
			} else {
				ExtendedItem.Content.Reset();
			}			
		}

		public virtual bool CanEnterContainer(PlacementOperation operation)
		{
			if (ExtendedItem.Content.IsCollection && 
				ICSharpCode.Xaml.CollectionSupport.CanCollectionAdd(ExtendedItem.Content.ReturnType, 
				operation.PlacedItems.Select(p => p.Item.Component))) 
				return true;
			return !ExtendedItem.Content.IsSet;
		}

		public virtual void EnterContainer(PlacementOperation operation)
		{
			if (ExtendedItem.Content.IsCollection) {
				foreach (var info in operation.PlacedItems) {
					ExtendedItem.Content.CollectionElements.Add(info.Item);
				}
			} else {
				ExtendedItem.Content.SetValue(operation.PlacedItems[0].Item);
			}
			if (operation.Type == PlacementType.AddItem) {
				foreach (var info in operation.PlacedItems) {
					SetPosition(info);
				}
			}
		}
	}
}
