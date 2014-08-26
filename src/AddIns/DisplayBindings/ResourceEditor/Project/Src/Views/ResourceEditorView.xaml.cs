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
	/// TextBox implementation for in-place editing of resource item fields.
	/// </summary>
	public class InPlaceEditTextBox : TextBox
	{
		string textBeforeEditing;
		
		public static readonly DependencyProperty IsEditingProperty =
			DependencyProperty.Register("IsEditing", typeof(bool), typeof(InPlaceEditTextBox),
				new FrameworkPropertyMetadata());
		
		public bool IsEditing {
			get { return (bool) GetValue(IsEditingProperty); }
			set { SetValue(IsEditingProperty, value); }
		}
		
		public InPlaceEditTextBox() : base()
		{
			this.Visibility = Visibility.Collapsed;
		}
		
		protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
		{
			base.OnPropertyChanged(e);
			
			if (e.Property == IsEditingProperty) {
				if ((bool) e.NewValue) {
					this.Visibility = Visibility.Visible;
				} else {
					this.Visibility = Visibility.Collapsed;
				}
			}
			if (e.Property == VisibilityProperty) {
				if ((Visibility) e.NewValue == Visibility.Visible) {
					// Auto-select whole text as soon as TextBox becomes visible
					this.Focus();
					this.SelectAll();
					textBeforeEditing = this.Text;
				}
			}
		}
		
		protected override void OnKeyUp(KeyEventArgs e)
		{
			base.OnKeyUp(e);
			
			if (e.Key == Key.Enter) {
				IsEditing = false;
			} else if (e.Key == Key.Escape) {
				// Cancel editing and restore original text
				this.Text = textBeforeEditing;
				IsEditing = false;
			}
		}
		
		protected override void OnLostFocus(RoutedEventArgs e)
		{
			base.OnLostFocus(e);
			
			// When losing focus, also stop editing
			IsEditing = false;
		}
	}
	
	/// <summary>
	/// Interaction logic for ResourceEditorView.xaml
	/// </summary>
	public partial class ResourceEditorView : UserControl, IResourceEditorView
	{
		readonly CollectionViewSource itemCollectionViewSource;
		
		public event EventHandler SelectionChanged;
		public event EventHandler EditingStarted;
		public event EventHandler EditingFinished;
		public event EventHandler EditingCancelled;
		
		public ResourceEditorView()
		{
			InitializeComponent();
			itemCollectionViewSource = (CollectionViewSource) this.Resources["resourceItemListViewSource"];
		}
		
		public IList SelectedItems {
			get {
				return resourceItemsListView.SelectedItems;
			}
		}
		
		public void SetItemView(IResourceItemView view)
		{
			resourceItemViewGrid.Children.Clear();
			view.UIControl.Visibility = Visibility.Visible;
			resourceItemViewGrid.Children.Add(view.UIControl);
		}
		
		public Predicate<ResourceEditor.ViewModels.ResourceItem> FilterPredicate {
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
			
			var resourceItem = e.Item as ResourceEditor.ViewModels.ResourceItem;
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
			/*if (e.Key == Key.Enter) {
				if (EditingFinished != null) {
					EditingFinished(this, new EventArgs());
				}
			} else if (e.Key == Key.Escape) {
				if (EditingCancelled != null) {
					EditingCancelled(this, new EventArgs());
				}
			} else*/
			if (e.Key == Key.F2) {
				if (EditingStarted != null) {
					EditingStarted(this, new EventArgs());
				}
			}
			
			e.Handled = false;
		}
	}
}