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
using System.Collections.Generic;
using System.Windows;
using System.Linq;
using System.IO;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Xps.Serialization;
using ICSharpCode.WpfDesign.Designer.Xaml;

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
				foreach (var designItem in items) {
					designItem.Name = null;
				}
				operation.DeleteItemsAndCommit();
			} catch {
				operation.Abort();
				throw;
			}
		}
		
		internal static void CreateVisualTree(this UIElement element)
		{
			try {
				var fixedDoc = new FixedDocument();
				var pageContent = new PageContent();
				var fixedPage = new FixedPage();
				fixedPage.Children.Add(element);
				(pageContent as IAddChild).AddChild(fixedPage);
				fixedDoc.Pages.Add(pageContent);

				var f = new XpsSerializerFactory();
				var w = f.CreateSerializerWriter(new MemoryStream());
				w.Write(fixedDoc);

				fixedPage.Children.Remove(element);
			}
			catch (Exception)
			{ }
		}
		
		internal static Size GetDefaultSize(DesignItem createdItem)
		{
			CreateVisualTree(createdItem.View);
			
			var s = Metadata.GetDefaultSize(createdItem.ComponentType, false);

			if (double.IsNaN(s.Width) && createdItem.View.DesiredSize.Width > 0)
			{
				s.Width = createdItem.View.DesiredSize.Width;
			}
			if (double.IsNaN(s.Height) && createdItem.View.DesiredSize.Height > 0)
			{
				s.Height = createdItem.View.DesiredSize.Height;
			}

			var newS = Metadata.GetDefaultSize(createdItem.ComponentType, true);

			if (!(s.Width > 0))
				s.Width = newS.Width;

			if (!(s.Height > 0))
				s.Height = newS.Height;

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
		
		
		private class ItemPos
		{
			public HorizontalAlignment HorizontalAlignment{ get; set; }
			
			public VerticalAlignment VerticalAlignment{ get; set; }
			
			public double Xmin { get; set; }
			
			public double Xmax { get; set; }

			public double Ymin { get; set; }
			
			public double Ymax { get; set; }
			
			public DesignItem DesignItem { get; set; }
		}
		
		public static void WrapItemsNewContainer(IEnumerable<DesignItem> items, Type containerType)
		{
			var collection = items;
			
			var _context = collection.First().Context as XamlDesignContext;
			
			var oldContainer = collection.First().Parent;
			
			if (collection.Any(x => x.Parent != oldContainer))
				return;
			
			var newInstance = Activator.CreateInstance(containerType);
			DesignItem newPanel = _context.Services.Component.RegisterComponentForDesigner(newInstance);
			var changeGroup = newPanel.OpenGroup("Wrap in Container");
			
			List<ItemPos> itemList = new List<ItemPos>();
			
			foreach (var item in collection) {
				
				var itemPos = new ItemPos(){ DesignItem = item };
				itemList.Add(itemPos);
				
				if (oldContainer.Component is Canvas) {
					var canvas = oldContainer.View as Canvas;
					
					if (item.Properties.GetAttachedProperty(Canvas.RightProperty) != null && item.Properties.GetAttachedProperty(Canvas.RightProperty).IsSet) {
						itemPos.HorizontalAlignment = HorizontalAlignment.Right;
						itemPos.Xmax = canvas.ActualWidth - (double)item.Properties.GetAttachedProperty(Canvas.RightProperty).ValueOnInstance;
						itemPos.Xmin = itemPos.Xmax - ((FrameworkElement)item.View).ActualWidth;
					}
					else if (item.Properties.GetAttachedProperty(Canvas.LeftProperty) != null && item.Properties.GetAttachedProperty(Canvas.LeftProperty).IsSet) {
						itemPos.HorizontalAlignment = HorizontalAlignment.Left;
						itemPos.Xmin = (double)item.Properties.GetAttachedProperty(Canvas.LeftProperty).ValueOnInstance;
						itemPos.Xmax = itemPos.Xmin + ((FrameworkElement)item.View).ActualWidth;
					} else {
						itemPos.HorizontalAlignment = HorizontalAlignment.Left;
						itemPos.Xmax = itemPos.Xmin + ((FrameworkElement)item.View).ActualWidth;
					}
					
					if (item.Properties.GetAttachedProperty(Canvas.BottomProperty) != null && item.Properties.GetAttachedProperty(Canvas.BottomProperty).IsSet) {
						itemPos.VerticalAlignment = VerticalAlignment.Bottom;
						itemPos.Ymax = canvas.ActualHeight - (double)item.Properties.GetAttachedProperty(Canvas.BottomProperty).ValueOnInstance;
						itemPos.Ymin = itemPos.Ymax - ((FrameworkElement)item.View).ActualHeight;
					}
					else if (item.Properties.GetAttachedProperty(Canvas.TopProperty) != null && item.Properties.GetAttachedProperty(Canvas.TopProperty).IsSet) {
						itemPos.VerticalAlignment = VerticalAlignment.Top;
						itemPos.Ymin = (double)item.Properties.GetAttachedProperty(Canvas.TopProperty).ValueOnInstance;
						itemPos.Ymax = itemPos.Ymin + ((FrameworkElement)item.View).ActualHeight;
					} else {
						itemPos.VerticalAlignment = VerticalAlignment.Top;
						itemPos.Ymax = itemPos.Ymin + ((FrameworkElement)item.View).ActualHeight;
					}
					
					item.Properties.GetAttachedProperty(Canvas.RightProperty).Reset();
					item.Properties.GetAttachedProperty(Canvas.LeftProperty).Reset();
					item.Properties.GetAttachedProperty(Canvas.TopProperty).Reset();
					item.Properties.GetAttachedProperty(Canvas.BottomProperty).Reset();
				} else if (oldContainer.Component is Grid) {
					var grid = oldContainer.View as Grid;
					
					if ((HorizontalAlignment)item.Properties.GetProperty(FrameworkElement.HorizontalAlignmentProperty).ValueOnInstance == HorizontalAlignment.Right) {
						itemPos.HorizontalAlignment = HorizontalAlignment.Right;
						itemPos.Xmax = grid.ActualWidth - ((Thickness)item.Properties.GetProperty(FrameworkElement.MarginProperty).ValueOnInstance).Right;
						itemPos.Xmin = itemPos.Xmax - ((FrameworkElement)item.View).ActualWidth;
					} else {
						itemPos.HorizontalAlignment = HorizontalAlignment.Left;
						itemPos.Xmin = ((Thickness)item.Properties.GetProperty(FrameworkElement.MarginProperty).ValueOnInstance).Left;
						itemPos.Xmax = itemPos.Xmin + ((FrameworkElement)item.View).ActualWidth;
					}
					
					if ((VerticalAlignment)item.Properties.GetProperty(FrameworkElement.VerticalAlignmentProperty).ValueOnInstance == VerticalAlignment.Bottom) {
						itemPos.VerticalAlignment = VerticalAlignment.Bottom;
						itemPos.Ymax = grid.ActualHeight - ((Thickness)item.Properties.GetProperty(FrameworkElement.MarginProperty).ValueOnInstance).Bottom;
						itemPos.Ymin = itemPos.Ymax - ((FrameworkElement)item.View).ActualHeight;
					} else {
						itemPos.VerticalAlignment = VerticalAlignment.Top;
						itemPos.Ymin = ((Thickness)item.Properties.GetProperty(FrameworkElement.MarginProperty).ValueOnInstance).Top;
						itemPos.Ymax = itemPos.Ymin + ((FrameworkElement)item.View).ActualHeight;
					}
					
					item.Properties.GetProperty(FrameworkElement.HorizontalAlignmentProperty).Reset();
					item.Properties.GetProperty(FrameworkElement.VerticalAlignmentProperty).Reset();
					item.Properties.GetProperty(FrameworkElement.MarginProperty).Reset();
				}
				
				var parCol = item.ParentProperty.CollectionElements;
				parCol.Remove(item);
			}
			
			var xmin = itemList.Min(x => x.Xmin);
			var xmax = itemList.Max(x => x.Xmax);
			var ymin = itemList.Min(x => x.Ymin);
			var ymax = itemList.Max(x => x.Ymax);
			
			if (oldContainer.Component is Canvas) {
				newPanel.Properties.GetProperty(FrameworkElement.WidthProperty).SetValue(xmax - xmin);
				newPanel.Properties.GetProperty(FrameworkElement.HeightProperty).SetValue(ymax - ymin);
				newPanel.Properties.GetAttachedProperty(Canvas.LeftProperty).SetValue(xmin);
				newPanel.Properties.GetAttachedProperty(Canvas.TopProperty).SetValue(ymin);
			} else if (oldContainer.Component is Grid) {
				newPanel.Properties.GetProperty(FrameworkElement.HorizontalAlignmentProperty).SetValue(HorizontalAlignment.Left);
				newPanel.Properties.GetProperty(FrameworkElement.VerticalAlignmentProperty).SetValue(VerticalAlignment.Top);
				newPanel.Properties.GetProperty(FrameworkElement.MarginProperty).SetValue(new Thickness(xmin, ymin, 0, 0));
				newPanel.Properties.GetProperty(FrameworkElement.WidthProperty).SetValue(xmax - xmin);
				newPanel.Properties.GetProperty(FrameworkElement.HeightProperty).SetValue(ymax - ymin);
			}
			
			foreach (var item in itemList) {
				newPanel.ContentProperty.CollectionElements.Add(item.DesignItem);
				
				if (newPanel.Component is Canvas) {
					if (item.HorizontalAlignment == HorizontalAlignment.Right) {
						item.DesignItem.Properties.GetAttachedProperty(Canvas.RightProperty).SetValue(xmax - item.Xmax);
					} else {
						item.DesignItem.Properties.GetAttachedProperty(Canvas.LeftProperty).SetValue(item.Xmin - xmin);
					}
					
					if (item.VerticalAlignment == VerticalAlignment.Bottom) {
						item.DesignItem.Properties.GetAttachedProperty(Canvas.BottomProperty).SetValue(ymax - item.Ymax);
					} else {
						item.DesignItem.Properties.GetAttachedProperty(Canvas.TopProperty).SetValue(item.Ymin - ymin);
					}
				} else if (newPanel.Component is Grid) {
					Thickness thickness = new Thickness(0);
					if (item.HorizontalAlignment == HorizontalAlignment.Right) {
						item.DesignItem.Properties.GetProperty(FrameworkElement.HorizontalAlignmentProperty).SetValue(HorizontalAlignment.Right);
						thickness.Right = xmax - item.Xmax;
					} else {
						item.DesignItem.Properties.GetProperty(FrameworkElement.HorizontalAlignmentProperty).SetValue(HorizontalAlignment.Left);
						thickness.Left = item.Xmin - xmin;
					}
					
					if (item.VerticalAlignment == VerticalAlignment.Bottom) {
						item.DesignItem.Properties.GetProperty(FrameworkElement.VerticalAlignmentProperty).SetValue(VerticalAlignment.Bottom);
						thickness.Bottom = ymax - item.Ymax;
					} else {
						item.DesignItem.Properties.GetProperty(FrameworkElement.VerticalAlignmentProperty).SetValue(VerticalAlignment.Top);
						thickness.Top = item.Ymin - ymin;
					}
					
					item.DesignItem.Properties.GetProperty(FrameworkElement.MarginProperty).SetValue(thickness);
				}
			}
			
			oldContainer.ContentProperty.CollectionElements.Add(newPanel);
			
			changeGroup.Commit();
			
			_context.Services.Selection.SetSelectedComponents(new []{ newPanel });
		}
	}
}
