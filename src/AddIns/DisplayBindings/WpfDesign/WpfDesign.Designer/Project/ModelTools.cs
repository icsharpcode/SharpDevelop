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
using System.Windows.Shapes;
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
		
		public static void CreateVisualTree(this UIElement element)
		{
			try
			{
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
			var defS = Metadata.GetDefaultSize(createdItem.ComponentType, false);
			if (defS != null)
				return defS.Value;

			CreateVisualTree(createdItem.View);

			var s = createdItem.View.DesiredSize;
			
			var newS = Metadata.GetDefaultSize(createdItem.ComponentType, true);

			if (newS.HasValue)
			{
				if (!(s.Width > 5) && newS.Value.Width > 0)
					s.Width = newS.Value.Width;

				if (!(s.Height > 5) && newS.Value.Height > 0)
					s.Height = newS.Value.Height;
			}

			if (double.IsNaN(s.Width) && GetWidth(createdItem.View) > 0) {
				s.Width = GetWidth(createdItem.View);
			}
			if (double.IsNaN(s.Height) && GetWidth(createdItem.View) > 0) {
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
				if(double.IsNaN(newWidth))
					item.Properties.GetProperty(FrameworkElement.WidthProperty).Reset();
				else
					item.Properties.GetProperty(FrameworkElement.WidthProperty).SetValue(newWidth);
			}
			if (newHeight != GetHeight(item.View)) {
				if (double.IsNaN(newHeight))
					item.Properties.GetProperty(FrameworkElement.HeightProperty).Reset();
				else
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

		private static ItemPos GetItemPos(PlacementOperation operation, DesignItem designItem)
		{
			var itemPos = new ItemPos() {DesignItem = designItem};

			var pos = operation.CurrentContainerBehavior.GetPosition(operation, designItem);
			itemPos.Xmin = pos.X;
			itemPos.Xmax = pos.X + pos.Width;
			itemPos.Ymin = pos.Y;
			itemPos.Ymax = pos.Y + pos.Height;

			return itemPos;
		}

		public static Tuple<DesignItem, Rect> WrapItemsNewContainer(IEnumerable<DesignItem> items, Type containerType, bool doInsert = true)
		{
			var collection = items;
			
			var _context = collection.First().Context as XamlDesignContext;
			
			var container = collection.First().Parent;
			
			if (collection.Any(x => x.Parent != container))
				return null;

			//Change Code to use the Placment Operation!
			var placement = container.Extensions.OfType<IPlacementBehavior>().FirstOrDefault();
			if (placement == null)
				return null;

			var operation = PlacementOperation.Start(items.ToList(), PlacementType.Move);

			var newInstance = _context.Services.ExtensionManager.CreateInstanceWithCustomInstanceFactory(containerType, null);
			DesignItem newPanel = _context.Services.Component.RegisterComponentForDesigner(newInstance);
			
			List<ItemPos> itemList = new List<ItemPos>();
			
			foreach (var item in collection) {
				itemList.Add(GetItemPos(operation, item));
				//var pos = placement.GetPosition(null, item);
				if (container.Component is Canvas) {
					item.Properties.GetAttachedProperty(Canvas.RightProperty).Reset();
					item.Properties.GetAttachedProperty(Canvas.LeftProperty).Reset();
					item.Properties.GetAttachedProperty(Canvas.TopProperty).Reset();
					item.Properties.GetAttachedProperty(Canvas.BottomProperty).Reset();
				} else if (container.Component is Grid) {
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

			foreach (var item in itemList) {
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

					newPanel.ContentProperty.CollectionElements.Add(item.DesignItem);

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

					newPanel.ContentProperty.CollectionElements.Add(item.DesignItem);

				} else if (newPanel.Component is Viewbox) {
					newPanel.ContentProperty.SetValue(item.DesignItem);
				}
			}

			if (doInsert)
			{
				PlacementOperation operation2 = PlacementOperation.TryStartInsertNewComponents(
					container,
					new[] {newPanel},
					new[] {new Rect(xmin, ymin, xmax - xmin, ymax - ymin).Round()},
					PlacementType.AddItem
				);

				operation2.Commit();

				_context.Services.Selection.SetSelectedComponents(new[] {newPanel});
			}

			operation.Commit();

			return new Tuple<DesignItem, Rect>(newPanel, new Rect(xmin, ymin, xmax - xmin, ymax - ymin).Round());
		}

		public static void ArrangeItems(IEnumerable<DesignItem> items, ArrangeDirection arrangeDirection)
		{
			var collection = items;

			var _context = collection.First().Context as XamlDesignContext;

			var container = collection.First().Parent;

			if (collection.Any(x => x.Parent != container))
				return;

			var placement = container.Extensions.OfType<IPlacementBehavior>().FirstOrDefault();
			if (placement == null)
				return;

			var operation = PlacementOperation.Start(items.ToList(), PlacementType.Move);
			
			//var changeGroup = container.OpenGroup("Arrange Elements");

			List<ItemPos> itemList = new List<ItemPos>();
			foreach (var item in collection)
			{
				itemList.Add(GetItemPos(operation, item));
			}

			var xmin = itemList.Min(x => x.Xmin);
			var xmax = itemList.Max(x => x.Xmax);
			var mpos = (xmax - xmin) / 2 + xmin;
			var ymin = itemList.Min(x => x.Ymin);
			var ymax = itemList.Max(x => x.Ymax);
			var ympos = (ymax - ymin) / 2 + ymin;

			foreach (var item in collection)
			{
				switch (arrangeDirection)
				{
					case ArrangeDirection.Left:
						{
							if (container.Component is Canvas)
							{
								if (!item.Properties.GetAttachedProperty(Canvas.RightProperty).IsSet)
								{
									item.Properties.GetAttachedProperty(Canvas.LeftProperty).SetValue(xmin);
								}
								else
								{
									var pos = (double)((Panel)item.Parent.Component).ActualWidth - (xmin + (double) ((FrameworkElement) item.Component).ActualWidth);
									item.Properties.GetAttachedProperty(Canvas.RightProperty).SetValue(pos);
								}
							}
							else if (container.Component is Grid)
							{
								if ((HorizontalAlignment)item.Properties.GetProperty(FrameworkElement.HorizontalAlignmentProperty).ValueOnInstance != HorizontalAlignment.Right)
								{
									var margin = (Thickness)item.Properties.GetProperty(FrameworkElement.MarginProperty).ValueOnInstance;
									margin.Left = xmin;
									item.Properties.GetProperty(FrameworkElement.MarginProperty).SetValue(margin);
								}
								else
								{
									var pos = (double)((Panel)item.Parent.Component).ActualWidth - (xmin + (double)((FrameworkElement)item.Component).ActualWidth);
									var margin = (Thickness)item.Properties.GetProperty(FrameworkElement.MarginProperty).ValueOnInstance;
									margin.Right = pos;
									item.Properties.GetProperty(FrameworkElement.MarginProperty).SetValue(margin);
								}
							}
						}
						break;
					case ArrangeDirection.HorizontalMiddle:
						{
							if (container.Component is Canvas)
							{
								if (!item.Properties.GetAttachedProperty(Canvas.RightProperty).IsSet)
								{
									if (!item.Properties.GetAttachedProperty(Canvas.RightProperty).IsSet)
									{
										item.Properties.GetAttachedProperty(Canvas.LeftProperty).SetValue(mpos - (((FrameworkElement)item.Component).ActualWidth) / 2);
									}
									else
									{
										var pp = mpos - (((FrameworkElement)item.Component).ActualWidth) / 2;
										var pos = (double)((Panel)item.Parent.Component).ActualWidth - pp - (((FrameworkElement)item.Component).ActualWidth);
										item.Properties.GetAttachedProperty(Canvas.RightProperty).SetValue(pos);
									}
								}
							}
							else if (container.Component is Grid)
							{
								if ((HorizontalAlignment)item.Properties.GetProperty(FrameworkElement.HorizontalAlignmentProperty).ValueOnInstance != HorizontalAlignment.Right)
								{
									var margin = (Thickness)item.Properties.GetProperty(FrameworkElement.MarginProperty).ValueOnInstance;
									margin.Left = mpos - (((FrameworkElement)item.Component).ActualWidth) / 2;
									item.Properties.GetProperty(FrameworkElement.MarginProperty).SetValue(margin);
								}
								else
								{
									var pp = mpos - (((FrameworkElement)item.Component).ActualWidth) / 2;
									var pos = (double)((Panel)item.Parent.Component).ActualWidth - pp - (((FrameworkElement)item.Component).ActualWidth);
									var margin = (Thickness)item.Properties.GetProperty(FrameworkElement.MarginProperty).ValueOnInstance;
									margin.Right = pos;
									item.Properties.GetProperty(FrameworkElement.MarginProperty).SetValue(margin);
								}
							}
						}
						break;
					case ArrangeDirection.Right:
						{
							if (container.Component is Canvas)
							{
								if (!item.Properties.GetAttachedProperty(Canvas.RightProperty).IsSet)
								{
									var pos = xmax - (double)((FrameworkElement)item.Component).ActualWidth;
									item.Properties.GetAttachedProperty(Canvas.LeftProperty).SetValue(pos);
								}
								else
								{
									var pos = (double)((Panel)item.Parent.Component).ActualWidth - xmax;
									item.Properties.GetAttachedProperty(Canvas.RightProperty).SetValue(pos);
								}
							}
							else if (container.Component is Grid)
							{
								if ((HorizontalAlignment)item.Properties.GetProperty(FrameworkElement.HorizontalAlignmentProperty).ValueOnInstance != HorizontalAlignment.Right)
								{
									var pos = xmax - (double)((FrameworkElement)item.Component).ActualWidth;
									var margin = (Thickness)item.Properties.GetProperty(FrameworkElement.MarginProperty).ValueOnInstance;
									margin.Left = pos;
									item.Properties.GetProperty(FrameworkElement.MarginProperty).SetValue(margin);
								}
								else
								{
									var pos = (double)((Panel)item.Parent.Component).ActualWidth - xmax;
									var margin = (Thickness)item.Properties.GetProperty(FrameworkElement.MarginProperty).ValueOnInstance;
									margin.Right = pos;
									item.Properties.GetProperty(FrameworkElement.MarginProperty).SetValue(margin);
								}
							}
						}
						break;
					case ArrangeDirection.Top:
						{
							if (container.Component is Canvas)
							{
								if (!item.Properties.GetAttachedProperty(Canvas.BottomProperty).IsSet)
								{
									item.Properties.GetAttachedProperty(Canvas.TopProperty).SetValue(ymin);
								}
								else
								{
									var pos = (double)((Panel)item.Parent.Component).ActualHeight - (ymin + (double)((FrameworkElement)item.Component).ActualHeight);
									item.Properties.GetAttachedProperty(Canvas.BottomProperty).SetValue(pos);
								}
							}
							else if (container.Component is Grid)
							{
								if ((VerticalAlignment)item.Properties.GetProperty(FrameworkElement.VerticalAlignmentProperty).ValueOnInstance != VerticalAlignment.Bottom)
								{
									item.Properties.GetAttachedProperty(Canvas.TopProperty).SetValue(ymin);
									var margin = (Thickness)item.Properties.GetProperty(FrameworkElement.MarginProperty).ValueOnInstance;
									margin.Top = ymin;
									item.Properties.GetProperty(FrameworkElement.MarginProperty).SetValue(margin);
								}
								else
								{
									var pos = (double)((Panel)item.Parent.Component).ActualHeight - (ymin + (double)((FrameworkElement)item.Component).ActualHeight);
									var margin = (Thickness)item.Properties.GetProperty(FrameworkElement.MarginProperty).ValueOnInstance;
									margin.Bottom = pos;
									item.Properties.GetProperty(FrameworkElement.MarginProperty).SetValue(margin);
								}
							}
						}
						break;
					case ArrangeDirection.VerticalMiddle:
						{
							if (container.Component is Canvas)
							{
								if (!item.Properties.GetAttachedProperty(Canvas.BottomProperty).IsSet)
								{
									item.Properties.GetAttachedProperty(Canvas.TopProperty).SetValue(ympos - (((FrameworkElement)item.Component).ActualHeight) / 2);
								}
								else
								{
									var pp = mpos - (((FrameworkElement)item.Component).ActualHeight) / 2;
									var pos = (double)((Panel)item.Parent.Component).ActualHeight - pp - (((FrameworkElement)item.Component).ActualHeight);
									item.Properties.GetAttachedProperty(Canvas.BottomProperty).SetValue(pos);
								}
							}
							else if (container.Component is Grid)
							{
								if ((VerticalAlignment)item.Properties.GetProperty(FrameworkElement.VerticalAlignmentProperty).ValueOnInstance != VerticalAlignment.Bottom)
								{
									var margin = (Thickness)item.Properties.GetProperty(FrameworkElement.MarginProperty).ValueOnInstance;
									margin.Top = ympos - (((FrameworkElement)item.Component).ActualHeight) / 2;
									item.Properties.GetProperty(FrameworkElement.MarginProperty).SetValue(margin);
								}
								else
								{
									var pp = mpos - (((FrameworkElement)item.Component).ActualHeight) / 2;
									var pos = (double)((Panel)item.Parent.Component).ActualHeight - pp - (((FrameworkElement)item.Component).ActualHeight);
									var margin = (Thickness)item.Properties.GetProperty(FrameworkElement.MarginProperty).ValueOnInstance;
									margin.Bottom = pos;
									item.Properties.GetProperty(FrameworkElement.MarginProperty).SetValue(margin);
								}
							}
						}
						break;
					case ArrangeDirection.Bottom:
						{
							if (container.Component is Canvas)
							{
								if (!item.Properties.GetAttachedProperty(Canvas.BottomProperty).IsSet)
								{
									var pos = ymax - (double)((FrameworkElement)item.Component).ActualHeight;
									item.Properties.GetAttachedProperty(Canvas.TopProperty).SetValue(pos);
								}
								else
								{
									var pos = (double)((Panel)item.Parent.Component).ActualHeight - ymax;
									item.Properties.GetAttachedProperty(Canvas.BottomProperty).SetValue(pos);
								}
							}
							else if (container.Component is Grid)
							{
								if ((VerticalAlignment)item.Properties.GetProperty(FrameworkElement.VerticalAlignmentProperty).ValueOnInstance != VerticalAlignment.Bottom)
								{
									var pos = ymax - (double)((FrameworkElement)item.Component).ActualHeight;
									var margin = (Thickness)item.Properties.GetProperty(FrameworkElement.MarginProperty).ValueOnInstance;
									margin.Top = pos;
									item.Properties.GetProperty(FrameworkElement.MarginProperty).SetValue(margin);
								}
								else
								{
									var pos = (double)((Panel)item.Parent.Component).ActualHeight - ymax;
									var margin = (Thickness)item.Properties.GetProperty(FrameworkElement.MarginProperty).ValueOnInstance;
									margin.Bottom = pos;
									item.Properties.GetProperty(FrameworkElement.MarginProperty).SetValue(margin);
								}
							}
						}
						break;
				}
			}

			operation.Commit();
		}
		
//		public static class Path {
//			
//			public static PathGeometry ConvertToPathGeometry(this TextBlock textBlock)
//			{
//				//var ft = new FormatedText();
//				return null;
//			}
//			
//			public static PathGeometry ConvertToPathGeometry(this Rectangle rectangle)
//			{
//				return null;
//			}
//			
//			public static PathGeometry ConvertToPathGeometry(this Ellipse ellipse)
//			{
//				return null;
//			}
//		}
	}
}
