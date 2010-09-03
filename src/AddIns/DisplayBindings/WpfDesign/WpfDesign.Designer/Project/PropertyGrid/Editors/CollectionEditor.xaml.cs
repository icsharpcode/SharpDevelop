// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using ICSharpCode.WpfDesign.Designer.OutlineView;

namespace ICSharpCode.WpfDesign.Designer.PropertyGrid.Editors
{
	public partial class CollectionEditor : Window
	{
		private static readonly Dictionary<Type, Type> TypeMappings = new Dictionary<Type, Type>();
		static CollectionEditor()
		{
			TypeMappings.Add(typeof(Menu), typeof(MenuItem));
			TypeMappings.Add(typeof(ListBox),typeof(ListBoxItem));
			TypeMappings.Add(typeof(ListView),typeof(ListViewItem));
			TypeMappings.Add(typeof(ComboBox),typeof(ComboBoxItem));
			TypeMappings.Add(typeof(TreeView),typeof(TreeViewItem));
		}
		
		private DesignItem _item;
		private Type _type;
		private IComponentService _componentService;
		public CollectionEditor()
		{
			InitializeComponent();
		}
		
		public void LoadItemsCollection(DesignItem item)
		{
			Debug.Assert(item.View is ItemsControl);
			_item=item;
			_componentService=item.Services.Component;
			item.Services.Selection.SelectionChanged+= delegate { PropertyGridView.SelectedItems=item.Services.Selection.SelectedItems;  };
			var control=item.View as ItemsControl;
			if(control!=null){
				TypeMappings.TryGetValue(control.GetType(), out _type);
				if (_type != null) {
					OutlineNode node = OutlineNode.Create(item);
					Outline.Root = node;
					PropertyGridView.PropertyGrid.SelectedItems = item.Services.Selection.SelectedItems;
				}
				else{
					PropertyGridView.IsEnabled=false;
					Outline.IsEnabled=false;
					AddItem.IsEnabled=false;
					RemoveItem.IsEnabled=false;
					MoveUpItem.IsEnabled=false;
					MoveDownItem.IsEnabled=false;
				}
			}
			
		}
		
		private void OnAddItemClicked(object sender, RoutedEventArgs e)
		{
			DesignItem newItem = _componentService.RegisterComponentForDesigner(Activator.CreateInstance(_type));
			DesignItem selectedItem = _item.Services.Selection.PrimarySelection;
			if(selectedItem.ContentProperty.IsCollection)
				selectedItem.ContentProperty.CollectionElements.Add(newItem);
			else
				selectedItem.ContentProperty.SetValue(newItem);
			_item.Services.Selection.SetSelectedComponents(new []{newItem});
		}

		private void OnRemoveItemClicked(object sender, RoutedEventArgs e)
		{
			DesignItem selectedItem = _item.Services.Selection.PrimarySelection;
			DesignItem parent = selectedItem.Parent;
			if(parent!=null && selectedItem!=_item){
				if(parent.ContentProperty.IsCollection)
					parent.ContentProperty.CollectionElements.Remove(selectedItem);
				else
					parent.ContentProperty.SetValue(null);
				_item.Services.Selection.SetSelectedComponents(new[] {parent});
			}
		}

		private void OnMoveItemUpClicked(object sender, RoutedEventArgs e)
		{
			DesignItem selectedItem = _item.Services.Selection.PrimarySelection;
			DesignItem parent = selectedItem.Parent;
			if(parent!=null && parent.ContentProperty.IsCollection){
				if(parent.ContentProperty.CollectionElements.Count!=1 && parent.ContentProperty.CollectionElements.IndexOf(selectedItem)!=0){
					int moveToIndex=parent.ContentProperty.CollectionElements.IndexOf(selectedItem)-1;
					var itemAtMoveToIndex=parent.ContentProperty.CollectionElements[moveToIndex];
					parent.ContentProperty.CollectionElements.RemoveAt(moveToIndex);
					if ((moveToIndex + 1) < (parent.ContentProperty.CollectionElements.Count+1))
						parent.ContentProperty.CollectionElements.Insert(moveToIndex+1,itemAtMoveToIndex);
				}
			}
		}

		private void OnMoveItemDownClicked(object sender, RoutedEventArgs e)
		{
			DesignItem selectedItem = _item.Services.Selection.PrimarySelection;
			DesignItem parent = selectedItem.Parent;
			if(parent!=null && parent.ContentProperty.IsCollection){
				var itemCount=parent.ContentProperty.CollectionElements.Count;
				if(itemCount!=1 && parent.ContentProperty.CollectionElements.IndexOf(selectedItem)!=itemCount){
					int moveToIndex=parent.ContentProperty.CollectionElements.IndexOf(selectedItem)+1;
					if(moveToIndex<itemCount){
					var itemAtMoveToIndex=parent.ContentProperty.CollectionElements[moveToIndex];
					parent.ContentProperty.CollectionElements.RemoveAt(moveToIndex);
					if(moveToIndex>0)
						parent.ContentProperty.CollectionElements.Insert(moveToIndex-1,itemAtMoveToIndex);
					}
				}
			}
		}
		
		protected override void OnClosing(CancelEventArgs e)
		{
			_item.Services.Selection.SetSelectedComponents(new []{_item});
		}
	}
}
