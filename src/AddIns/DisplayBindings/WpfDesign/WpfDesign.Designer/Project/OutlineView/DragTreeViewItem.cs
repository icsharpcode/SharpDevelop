// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Controls.Primitives;

namespace ICSharpCode.WpfDesign.Designer.OutlineView
{
	public class DragTreeViewItem : TreeViewItem
	{
		static DragTreeViewItem()
		{
			DefaultStyleKeyProperty.OverrideMetadata(typeof(DragTreeViewItem),
				new FrameworkPropertyMetadata(typeof(DragTreeViewItem)));
		}

		public DragTreeViewItem()
		{
			Loaded += new RoutedEventHandler(DragTreeViewItem_Loaded);
			Unloaded += new RoutedEventHandler(DragTreeViewItem_Unloaded);
		}

		void DragTreeViewItem_Loaded(object sender, RoutedEventArgs e)
		{
			ParentTree = this.GetVisualAncestors().OfType<DragTreeView>().FirstOrDefault();
			if (ParentTree != null) {
				ParentTree.ItemAttached(this);
			}
		}

		void DragTreeViewItem_Unloaded(object sender, RoutedEventArgs e)
		{
			if (ParentTree != null) {
				ParentTree.ItemDetached(this);
			}
			ParentTree = null;
		}

		public new static readonly DependencyProperty IsSelectedProperty =
			Selector.IsSelectedProperty.AddOwner(typeof(DragTreeViewItem));

		public new bool IsSelected {
			get { return (bool)GetValue(IsSelectedProperty); }
			set { SetValue(IsSelectedProperty, value); }
		}

		public static readonly DependencyProperty IsDragHoverProperty =
			DependencyProperty.Register("IsDragHover", typeof(bool), typeof(DragTreeViewItem));

		public bool IsDragHover {
			get { return (bool)GetValue(IsDragHoverProperty); }
			set { SetValue(IsDragHoverProperty, value); }
		}

		internal ContentPresenter HeaderPresenter {
			get { return (ContentPresenter)Template.FindName("PART_Header", this); }
		}

		public static readonly DependencyProperty LevelProperty =
			DependencyProperty.Register("Level", typeof(int), typeof(DragTreeViewItem));

		public int Level {
			get { return (int)GetValue(LevelProperty); }
			set { SetValue(LevelProperty, value); }
		}

		public DragTreeView ParentTree { get; private set; }

		protected override void OnVisualParentChanged(DependencyObject oldParent)
		{
			base.OnVisualParentChanged(oldParent);
			
			var parentItem = ItemsControl.ItemsControlFromItemContainer(this) as DragTreeViewItem;
			if (parentItem != null) Level = parentItem.Level + 1;
		}

		protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
		{
			base.OnPropertyChanged(e);
			if (e.Property == IsSelectedProperty) {
				if (ParentTree != null) {
					ParentTree.ItemIsSelectedChanged(this);
				}
			}
		}

		protected override DependencyObject GetContainerForItemOverride()
		{
			return new DragTreeViewItem();
		}

		protected override bool IsItemItsOwnContainerOverride(object item)
		{
			return item is DragTreeViewItem;
		}

		protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
		{
			base.OnMouseLeftButtonDown(e);
			if (e.Source is ToggleButton || e.Source is ItemsPresenter) return;
			ParentTree.ItemMouseDown(this);
		}

		protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
		{
			base.OnMouseLeftButtonUp(e);
			ParentTree.ItemMouseUp(this);
		}
	}
}
