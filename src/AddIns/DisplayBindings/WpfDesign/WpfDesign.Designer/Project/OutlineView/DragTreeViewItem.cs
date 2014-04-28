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
		ContentPresenter part_header;
		
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
				ParentTree.FilterChanged += ParentTree_FilterChanged;
			}
		}

		void ParentTree_FilterChanged(string obj)
		{
			var v = ParentTree.ShouldItemBeVisible(this);
			if (v)
				part_header.Visibility = Visibility.Visible;
			else
				part_header.Visibility = Visibility.Collapsed;
		}

		void DragTreeViewItem_Unloaded(object sender, RoutedEventArgs e)
		{
			if (ParentTree != null) {
				ParentTree.ItemDetached(this);
				ParentTree.FilterChanged -= ParentTree_FilterChanged;
			}
			ParentTree = null;
		}
		
		
		public override void OnApplyTemplate()
		{
			base.OnApplyTemplate();
			
			part_header = GetTemplateChild("PART_Header") as ContentPresenter;
		}

		public new static readonly DependencyProperty IsSelectedProperty =
			Selector.IsSelectedProperty.AddOwner(typeof(DragTreeViewItem), new FrameworkPropertyMetadata(OnIsSelectedChanged));

		public new bool IsSelected {
			get { return (bool)GetValue(IsSelectedProperty); }
			set { SetValue(IsSelectedProperty, value); }
		}
		
		public static void OnIsSelectedChanged(DependencyObject s, DependencyPropertyChangedEventArgs e)
		{
			var el = s as DragTreeViewItem;
			if (el != null && el.IsSelected)
				el.BringIntoView();
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
			
			if (ParentTree != null) {
				ParentTree.ItemMouseUp(this);
			}
		}
	}
}
