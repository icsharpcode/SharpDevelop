using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Input;
using System.Windows.Controls.Primitives;

namespace ICSharpCode.WpfDesign.Designer.PropertyGrid
{
	public class PropertyTreeView : TreeView
	{
		//static Dictionary<object, PropertyTreeViewItem> containersCache = new Dictionary<object, PropertyTreeViewItem>();

		protected override DependencyObject GetContainerForItemOverride()
		{
			//return new Decorator();
			return new PropertyTreeViewItem();
		}

		protected override bool IsItemItsOwnContainerOverride(object item)
		{
			return item is PropertyTreeViewItem;
		}

		//protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
		//{
		//    PrepareContainer(element, item);
		//}

		//internal void PrepareContainer(DependencyObject element, object item)
		//{
		//    PropertyTreeViewItem container;
		//    if (!containersCache.TryGetValue(item, out container))
		//    {
		//        container = new PropertyTreeViewItem();
		//        containersCache[item] = container;
		//    }
		//    base.PrepareContainerForItemOverride(container, item);
		//    (element as Decorator).Child = container;
		//}
	}

	public class PropertyTreeViewItem : TreeViewItem
	{
		//public PropertyTreeView ParentTree { get; private set; }

		public static readonly DependencyProperty LevelProperty =
			DependencyProperty.Register("Level", typeof(int), typeof(PropertyTreeViewItem));

		public int Level {
			get { return (int)GetValue(LevelProperty); }
			set { SetValue(LevelProperty, value); }
		}

		protected override DependencyObject GetContainerForItemOverride()
		{
			//return new Decorator();
			return new PropertyTreeViewItem();
		}

		protected override bool IsItemItsOwnContainerOverride(object item)
		{
			return item is PropertyTreeViewItem;
		}

		//protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
		//{
		//    ParentTree.PrepareContainer(element, item);
		//}

		protected override void OnVisualParentChanged(DependencyObject oldParent)
		{
			base.OnVisualParentChanged(oldParent);

			var parent = ItemsControl.ItemsControlFromItemContainer(this);
			//ParentTree = parent as PropertyTreeView;

			var parentItem = parent as PropertyTreeViewItem;
			if (parentItem != null) {
				Level = parentItem.Level + 1;
				//ParentTree = parentItem.ParentTree;
			}
		}

		protected override void OnMouseDown(MouseButtonEventArgs e)
		{
			if (e.ChangedButton == MouseButton.Right) {
				e.Handled = true;

				var contextMenu = new PropertyContextMenu();
				contextMenu.DataContext = DataContext;
				contextMenu.Placement = PlacementMode.Bottom;
				contextMenu.HorizontalOffset = -30;
				contextMenu.PlacementTarget = this;
				contextMenu.IsOpen = true;
			}
		}
	}
}
