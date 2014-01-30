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
using System.ComponentModel;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Linq;
using ICSharpCode.WpfDesign.Designer.OutlineView;

namespace ICSharpCode.WpfDesign.Designer.PropertyGrid.Editors
{
	public partial class FlatCollectionEditor : Window
	{
		private static readonly Dictionary<Type, Type> TypeMappings = new Dictionary<Type, Type>();
		static FlatCollectionEditor()
		{
			TypeMappings.Add(typeof(ListBox),typeof(ListBoxItem));
			TypeMappings.Add(typeof(ListView),typeof(ListViewItem));
			TypeMappings.Add(typeof(ComboBox),typeof(ComboBoxItem));
			TypeMappings.Add(typeof(TabControl),typeof(TabItem));
			TypeMappings.Add(typeof(ColumnDefinitionCollection),typeof(ColumnDefinition));
			TypeMappings.Add(typeof(RowDefinitionCollection),typeof(RowDefinition));
		}
		
		private DesignItemProperty _itemProperty;
		private IComponentService _componentService;
		private Type _type;
		
		public FlatCollectionEditor()
		{
			InitializeComponent();
			
			this.Owner = Application.Current.MainWindow;
		}
		
		public void LoadItemsCollection(DesignItemProperty itemProperty)
		{
			_itemProperty = itemProperty;
			_componentService=_itemProperty.DesignItem.Services.Component;
			TypeMappings.TryGetValue(_itemProperty.ReturnType, out _type);
			if (_type == null) {
				PropertyGridView.IsEnabled=false;
				ListBox.IsEnabled=false;
				AddItem.IsEnabled=false;
				RemoveItem.IsEnabled=false;
				MoveUpItem.IsEnabled=false;
				MoveDownItem.IsEnabled=false;
			}
			
			ListBox.ItemsSource = _itemProperty.CollectionElements;
		}
		
		private void OnAddItemClicked(object sender, RoutedEventArgs e)
		{
			DesignItem newItem = _componentService.RegisterComponentForDesigner(Activator.CreateInstance(_type));
			_itemProperty.CollectionElements.Add(newItem);
		}

		private void OnRemoveItemClicked(object sender, RoutedEventArgs e)
		{
			var selItem = ListBox.SelectedItem as DesignItem;
			if (selItem != null)
				_itemProperty.CollectionElements.Remove(selItem);
		}
		
		private void OnMoveItemUpClicked(object sender, RoutedEventArgs e)
		{
			DesignItem selectedItem = ListBox.SelectedItem as DesignItem;
			if (selectedItem!=null) {
				if(_itemProperty.CollectionElements.Count!=1 && _itemProperty.CollectionElements.IndexOf(selectedItem)!=0){
					int moveToIndex=_itemProperty.CollectionElements.IndexOf(selectedItem)-1;
					var itemAtMoveToIndex=_itemProperty.CollectionElements[moveToIndex];
					_itemProperty.CollectionElements.RemoveAt(moveToIndex);
					if ((moveToIndex + 1) < (_itemProperty.CollectionElements.Count+1))
						_itemProperty.CollectionElements.Insert(moveToIndex+1,itemAtMoveToIndex);
				}
			}
		}

		private void OnMoveItemDownClicked(object sender, RoutedEventArgs e)
		{
			DesignItem selectedItem = ListBox.SelectedItem as DesignItem;
			if (selectedItem!=null) {
				var itemCount=_itemProperty.CollectionElements.Count;
				if(itemCount!=1 && _itemProperty.CollectionElements.IndexOf(selectedItem)!=itemCount){
					int moveToIndex=_itemProperty.CollectionElements.IndexOf(selectedItem)+1;
					if(moveToIndex<itemCount){
						var itemAtMoveToIndex=_itemProperty.CollectionElements[moveToIndex];
						_itemProperty.CollectionElements.RemoveAt(moveToIndex);
						if(moveToIndex>0)
							_itemProperty.CollectionElements.Insert(moveToIndex-1,itemAtMoveToIndex);
					}
				}
			}
		}
		
		void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			PropertyGridView.PropertyGrid.SelectedItems = ListBox.SelectedItems.Cast<DesignItem>();
		}
	}
}
