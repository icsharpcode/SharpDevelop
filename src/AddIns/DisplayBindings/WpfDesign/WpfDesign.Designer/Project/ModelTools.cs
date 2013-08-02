// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Windows;
using System.Linq;

namespace ICSharpCode.WpfDesign.Designer
{
	/// <summary>
	/// Static helper methods for working with the designer DOM.
	/// </summary>
	public static class ModelTools
	{
		/// <summary>
		/// Compares the positions of a and b in the model file.
		/// </summary>
		public static int ComparePositionInModelFile(DesignItem a, DesignItem b)
		{
			// first remember all parent properties of a
			HashSet<DesignItemProperty> aProps = new HashSet<DesignItemProperty>();
			DesignItem tmp = a;
			while (tmp != null) {
				aProps.Add(tmp.ParentProperty);
				tmp = tmp.Parent;
			}
			
			// now walk up b's parent tree until a matching property is found
			tmp = b;
			while (tmp != null) {
				DesignItemProperty prop = tmp.ParentProperty;
				if (aProps.Contains(prop)) {
					if (prop.IsCollection) {
						return prop.CollectionElements.IndexOf(a).CompareTo(prop.CollectionElements.IndexOf(b));
					} else {
						return 0;
					}
				}
				tmp = tmp.Parent;
			}
			return 0;
		}
		
		/// <summary>
		/// Gets if the specified design item is in the document it belongs to.
		/// </summary>
		/// <returns>True for live objects, false for deleted objects.</returns>
		public static bool IsInDocument(DesignItem item)
		{
			DesignItem rootItem = item.Context.RootItem;
			while (item != null) {
				if (item == rootItem) return true;
				item = item.Parent;
			}
			return false;
		}
		
		/// <summary>
		/// Gets if the specified components can be deleted.
		/// </summary>
		public static bool CanDeleteComponents(ICollection<DesignItem> items)
		{
			IPlacementBehavior b = PlacementOperation.GetPlacementBehavior(items);
			return b != null
				&& b.CanPlace(items, PlacementType.Delete, PlacementAlignment.Center);
		}

		public static bool CanSelectComponent(DesignItem item)
		{
			return item.View != null;
		}
		
		/// <summary>
		/// Deletes the specified components from their parent containers.
		/// If the deleted components are currently selected, they are deselected before they are deleted.
		/// </summary>
		public static void DeleteComponents(ICollection<DesignItem> items)
		{
			DesignItem parent = items.First().Parent;
			PlacementOperation operation = PlacementOperation.Start(items, PlacementType.Delete);
			try {
				ISelectionService selectionService = items.First().Services.Selection;
				selectionService.SetSelectedComponents(items, SelectionTypes.Remove);
				// if the selection is empty after deleting some components, select the parent of the deleted component
				if (selectionService.SelectionCount == 0 && !items.Contains(parent)) {
					selectionService.SetSelectedComponents(new DesignItem[] { parent });
				}
				operation.DeleteItemsAndCommit();
			} catch {
				operation.Abort();
				throw;
			}
		}
		
		internal static Size GetDefaultSize(DesignItem createdItem)
		{
            var s = Metadata.GetDefaultSize(createdItem.ComponentType);
            if (double.IsNaN(s.Width)) {
                s.Width = GetWidth(createdItem.View);
            }
            if (double.IsNaN(s.Height)) {
                s.Height = GetHeight(createdItem.View);
            }
			return s;
		}
		
		internal static double GetWidth(UIElement element)
		{
			double v = (double)element.GetValue(FrameworkElement.WidthProperty);
			if (double.IsNaN(v))
				return element.RenderSize.Width;
			else
				return v;
		}
		
		internal static double GetHeight(UIElement element)
		{
			double v = (double)element.GetValue(FrameworkElement.HeightProperty);
			if (double.IsNaN(v))
				return element.RenderSize.Height;
			else
				return v;
		}

		public static void Resize(DesignItem item, double newWidth, double newHeight)
		{
			if (newWidth != GetWidth(item.View)) {
				item.Properties.GetProperty(FrameworkElement.WidthProperty).SetValue(newWidth);
			}
			if (newHeight != GetHeight(item.View)) {
				item.Properties.GetProperty(FrameworkElement.HeightProperty).SetValue(newHeight);
			}
		}
	}
}
