// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision: 3518 $</version>
// </file>

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
					}
					else {
						return 0;
					}
				}
			}
			return 0;
		}

		/// <summary>
		/// Gets if the specified design item is in the document it belongs to.
		/// </summary>
		/// <returns>True for live objects, false for deleted objects.</returns>
		public static bool IsInDocument(DesignItem item)
		{
			return item.Context.ModelService.Root == GetRoot(item);
		}

		public static DesignItem GetRoot(DesignItem item)
		{
			while (item.Parent != null) {
				item = item.Parent;
			}
			return item;
		}

		/// <summary>
		/// Gets if the specified components can be deleted.
		/// </summary>
		public static bool CanDeleteComponents(IEnumerable<DesignItem> items)
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
		public static void DeleteComponents(IEnumerable<DesignItem> items)
		{
			DesignItem parent = items.First().Parent;
			PlacementOperation operation = PlacementOperation.Start(items, PlacementType.Delete);
			try {
				ISelectionService selectionService = items.First().Context.SelectionService;
				selectionService.Select(items, SelectionTypes.Remove);
				// if the selection is empty after deleting some components, select the parent of the deleted component
				if (selectionService.SelectionCount == 0 && !items.Contains(parent)) {
					selectionService.Select(new DesignItem[] { parent });
				}
				operation.DeleteItemsAndCommit();
			}
			catch {
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

		//TODO: Share with Outline / PlacementBehavior
		public static IEnumerable<DesignItem> DescendantsAndSelf(DesignItem item)
		{
			yield return item;
			foreach (var child in Descendants(item)) {
				yield return child;
			}
		}

		public static IEnumerable<DesignItem> Descendants(DesignItem item)
		{
			foreach (var child in Children(item)) {
				foreach (var child2 in DescendantsAndSelf(child)) {
					yield return child2;
				}
			}
		}

		public static IEnumerable<DesignItem> Children(DesignItem item)
		{
			var content = item.Content;
			if (content != null) {
				if (content.IsCollection) {
					foreach (var child in content.CollectionElements) {
						yield return child;
					}
				}
				else {
					if (content.Value != null) {
						yield return content.Value;
					}
				}
			}
		}

		// Filters an element list, dropping all elements that are not part of the xaml document
		// (e.g. because they were deleted).
		public static IEnumerable<DesignItem> GetLiveElements(IEnumerable<DesignItem> items)
		{
			foreach (DesignItem item in items) {
				if (IsInDocument(item) && CanSelectComponent(item)) {
					yield return item;
				}
			}
		}
		public static bool CanAdd(DesignItem container, IEnumerable<DesignItem> items, bool copy)
		{
			throw new NotImplementedException();
		}

		public static void Add(DesignItem container, IEnumerable<DesignItem> items, bool copy)
		{
			throw new NotImplementedException();
		}

		public static bool CanInsert(DesignItem container, IEnumerable<DesignItem> items, DesignItem after, bool copy)
		{
			throw new NotImplementedException();
		}

		public static void Insert(DesignItem container, IEnumerable<DesignItem> items, DesignItem after, bool copy)
		{
			throw new NotImplementedException();
		}

		public static void Remove(DesignItem item)
		{
			throw new NotImplementedException();
		}

		public static DesignItem Clone(DesignItem item)
		{
			throw new NotImplementedException();
		}

		//public void Remove()
		//{
		//    if (ParentProperty != null)
		//    {
		//        if (ParentProperty.IsCollection)
		//        {
		//            ParentProperty.CollectionElements.Remove(this);
		//        }
		//        else
		//        {
		//            ParentProperty.Reset();
		//        }
		//    }
		//}

		// TODO: Outline and IPlacementBehavior must use the same logic (put it inside DesignItem)
		//public bool CanInsert(IEnumerable<OutlineNode> nodes, OutlineNode after, bool copy)
		//{
		//    if (DesignItem.ContentPropertyName == null) return false;

		//    if (DesignItem.Content.IsCollection) {
		//        foreach (var node in nodes) {
		//            if (!ICSharpCode.Xaml.CollectionSupport.CanCollectionAdd(DesignItem.Content.ReturnType,
		//                                                    node.DesignItem.ComponentType)) {
		//                return false;
		//            }
		//        }
		//        return true;
		//    }
		//    else {
		//        return after == null && nodes.Count() == 1 &&
		//            DesignItem.Content.DeclaringType.IsAssignableFrom(
		//                nodes.First().DesignItem.ComponentType);
		//    }
		//}

		//public void Insert(IEnumerable<OutlineNode> nodes, OutlineNode after, bool copy)
		//{
		//    if (copy) {
		//        nodes = nodes.Select(n => OutlineNode.Create(ModelTools.Clone(n.DesignItem)));
		//    }
		//    else {
		//        foreach (var node in nodes) {
		//            ModelTools.Remove(node.DesignItem);
		//        }
		//    }

		//    var index = after == null ? 0 : Children.IndexOf(after) + 1;

		//    var content = DesignItem.Content;
		//    if (content.IsCollection) {
		//        foreach (var node in nodes) {
		//            content.CollectionElements.Insert(index++, node.DesignItem);
		//        }
		//    }
		//    else {
		//        content.SetValue(nodes.First().DesignItem);
		//    }
		//}

		//void UpdateChildren()
		//{
		//    Children.Clear();

		//    if (DesignItem.ContentPropertyName != null) {
		//        var content = DesignItem.Content;
		//        if (content.IsCollection) {
		//            UpdateChildrenCore(content.CollectionElements);
		//        }
		//        else {
		//            if (content.Value != null) {
		//                UpdateChildrenCore(new[] { content.Value });
		//            }
		//        }
		//    }
		//}

		//void UpdateChildrenCore(IEnumerable<DesignItem> items)
		//{
		//    foreach (var item in items) {
		//        if (ModelTools.CanSelectComponent(item)) {
		//            var node = OutlineNode.Create(item);
		//            Children.Add(node);
		//        }
		//    }
		//}
	}
}
