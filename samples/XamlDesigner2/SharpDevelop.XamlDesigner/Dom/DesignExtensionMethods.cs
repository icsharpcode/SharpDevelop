using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

namespace SharpDevelop.XamlDesigner.Dom
{
	public static class DesignExtensionMethods
	{
		public static IEnumerable<DesignItem> AncestorsAndSelf(this DesignItem item)
		{
			yield return item;
			foreach (var item2 in Ancestors(item)) {
				yield return item2;
			}
		}

		public static IEnumerable<DesignItem> Ancestors(this DesignItem item)
		{
			while (item.ParentItem != null) {
				yield return item.ParentItem;
				item = item.ParentItem;
			}
		}

		public static IEnumerable<DesignItem> DescendantsAndSelf(this DesignItem item)
		{
			yield return item;
			foreach (var child in Descendants(item)) {
				yield return child;
			}
		}

		public static IEnumerable<DesignItem> Descendants(this DesignItem item)
		{
			foreach (var child in Children(item)) {
				foreach (var child2 in DescendantsAndSelf(child)) {
					yield return child2;
				}
			}
		}

		public static IEnumerable<DesignItem> Children(this DesignItem item)
		{
			var content = item.Content;
			if (content != null) {
				if (content.Collection != null) {
					foreach (var child in content.Collection) {
						yield return child;
					}
				}
				else if (content.Value != null) {
					yield return content.Value;
				}
			}
		}

		// Filters an element list, dropping all elements that are not part of the xaml document
		// (e.g. because they were deleted).
		//public static IEnumerable<DesignItem> GetLiveElements(IEnumerable<DesignItem> items)
		//{
		//    foreach (DesignItem item in items) {
		//        if (IsInDocument(item) && CanSelectComponent(item)) {
		//            yield return item;
		//        }
		//    }
		//}

		public static bool CanAdd(this DesignItem container, IEnumerable<DesignItem> items, bool copy)
		{
			return container.View is Panel;
		}

		public static void Add(this DesignItem container, IEnumerable<DesignItem> items, bool copy)
		{
			foreach (var item in items) {
				(container.View as Panel).Children.Add(item.View);
			}
		}

		public static bool CanInsert(this DesignItem container, IEnumerable<DesignItem> items, DesignItem after, bool copy)
		{
			return true;
		}

		public static void Insert(this DesignItem container, IEnumerable<DesignItem> items, DesignItem after, bool copy)
		{
			
		}

		public static void Delete(this DesignItem item)
		{
			var panel = item.View.Parent as Panel;
			if (panel != null) {
				panel.Children.Remove(item.View);
			}
		}

		public static void Delete(this IEnumerable<DesignItem> items)
		{
			foreach (var item in items) {
				Delete(item);
			}
		}

		public static DesignItem Clone(this DesignItem item)
		{
			return null;
		}

		public static Rect GetLayoutSlot(this DesignItem item)
		{
			return LayoutInformation.GetLayoutSlot(item.View);
		}

		public static Rect GetBounds(this DesignItem item)
		{
			Point zero = new Point();
			if (item.View.Parent != null) {
				zero = item.View.TransformToAncestor(item.View.Parent as Visual).Transform(zero);
			}
			return new Rect(zero, item.View.RenderSize);
		}
	}
}
