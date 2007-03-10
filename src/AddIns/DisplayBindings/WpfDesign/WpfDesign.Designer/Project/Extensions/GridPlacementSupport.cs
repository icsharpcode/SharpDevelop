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

using ICSharpCode.WpfDesign.Adorners;
using ICSharpCode.WpfDesign.Extensions;
using ICSharpCode.WpfDesign.Designer.Controls;

namespace ICSharpCode.WpfDesign.Designer.Extensions
{
	/// <summary>
	/// Provides <see cref="IPlacementBehavior"/> behavior for <see cref="Grid"/>.
	/// </summary>
	[ExtensionFor(typeof(Grid))]
	public sealed class GridPlacementSupport : BehaviorExtension, IPlacementBehavior
	{
		protected override void OnInitialized()
		{
			base.OnInitialized();
			this.ExtendedItem.AddBehavior(typeof(IPlacementBehavior), this);
		}
		
		public bool CanPlace(ICollection<DesignItem> childItems, PlacementType type, PlacementAlignment position)
		{
			return type == PlacementType.Delete;
		}
		
		public void BeginPlacement(PlacementOperation operation)
		{
		}
		
		public void EndPlacement(PlacementOperation operation)
		{
		}
		
		public Rect GetPosition(PlacementOperation operation, DesignItem child)
		{
			return new Rect();
		}
		
		public void SetPosition(PlacementInformation info)
		{
			throw new NotImplementedException();
		}
		
		public bool CanLeaveContainer(PlacementOperation operation)
		{
			return true;
		}
		
		public void LeaveContainer(PlacementOperation operation)
		{
			foreach (PlacementInformation info in operation.PlacedItems) {
				if (info.Item.ComponentType == typeof(ColumnDefinition)) {
					// TODO: combine the width of the deleted column with the previous column
					this.ExtendedItem.Properties["ColumnDefinitions"].CollectionElements.Remove(info.Item);
				} else if (info.Item.ComponentType == typeof(RowDefinition)) {
					this.ExtendedItem.Properties["RowDefinitions"].CollectionElements.Remove(info.Item);
				} else {
					throw new NotImplementedException();
				}
			}
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
