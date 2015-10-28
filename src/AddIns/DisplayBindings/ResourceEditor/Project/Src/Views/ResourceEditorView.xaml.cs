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
using System.Collections;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using ICSharpCode.Core.Presentation;
using ResourceEditor.ViewModels;

namespace ResourceEditor.Views
{
	/// <summary>
	/// Interaction logic for ResourceEditorView.xaml
	/// </summary>
	public partial class ResourceEditorView : UserControl, IResourceEditorView
	{
		readonly CollectionViewSource itemCollectionViewSource;
		
		public event EventHandler SelectionChanged;
		public event EventHandler EditingStartRequested;
		public event EventHandler AddingNewItemRequested;
		
		public ResourceEditorView()
		{
			InitializeComponent();
			itemCollectionViewSource = (CollectionViewSource)this.Resources["resourceItemListViewSource"];
		}
		
		public IList SelectedItems {
			get {
				return resourceItemsListView.SelectedItems;
			}
		}
		
		public void SelectItem(ResourceItem item)
		{
			SelectedItems.Clear();
			SelectedItems.Add(item);
			resourceItemsListView.ScrollIntoView(item);
		}
		
		public void SetItemView(IResourceItemView view)
		{
			resourceItemViewGrid.Children.Clear();
			if (view != null) {
				view.UIControl.Visibility = Visibility.Visible;
				resourceItemViewGrid.Children.Add(view.UIControl);
			}
		}
		
		public Predicate<ResourceItem> FilterPredicate {
			get;
			set;
		}

		void ListView_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
		{
			MenuService.ShowContextMenu(this, null, "/SharpDevelop/ResourceEditor/ResourceList/ContextMenu");
		}
		
		void ListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (SelectionChanged != null) {
				SelectionChanged(this, new EventArgs());
			}
		}
		
		void CollectionViewSource_Filter(object sender, FilterEventArgs e)
		{
			if (FilterPredicate == null) {
				// No filtering without predicate
				e.Accepted = true;
				return;
			}
			
			var resourceItem = e.Item as ResourceItem;
			if (resourceItem == null) {
				// Away with non-ResourceItems (shouldn't happen anyway)
				e.Accepted = false;
				return;
			}
			e.Accepted = FilterPredicate(resourceItem);
		}
		
		void FilterTextBox_KeyUp(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Enter) {
				// Apply filter text on Enter key
				UpdateFilter();
			} else if (e.Key == Key.Escape) {
				// Clear the filter text on Esc key
				searchTermTextBox.Clear();
				UpdateFilter();
			}
		}
		
		void UpdateFilterButton_Click(object sender, RoutedEventArgs e)
		{
			UpdateFilter();
		}
		
		void UpdateFilter()
		{
			// Update CollectionViewSource to re-evaluate filter predicate
			itemCollectionViewSource.View.Refresh();
		}
		
		void ResourceItemsListView_KeyUp(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.F2) {
				if (EditingStartRequested != null) {
					EditingStartRequested(this, new EventArgs());
				}
			}
			if (e.Key == Key.Insert) {
				if (AddingNewItemRequested != null) {
					AddingNewItemRequested(this, new EventArgs());
				}
			}
			if ((e.Key == Key.F) && Keyboard.Modifiers.HasFlag(ModifierKeys.Control)) {
				searchTermTextBox.Focus();
			}
			
			e.Handled = false;
		}
	}
}