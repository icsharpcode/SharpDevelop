// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using ICSharpCode.WpfDesign.Designer.Controls;
using ICSharpCode.WpfDesign.PropertyEditor;

namespace ICSharpCode.WpfDesign.Designer
{
	/// <summary>
	/// Shows a list of properties; supports data binding.
	/// </summary>
	public partial class PropertyEditor : UserControl
	{
		/// <summary>
		/// Dependency property for <see cref="EditedObject"/>.
		/// </summary>
		public static readonly DependencyProperty EditedObjectProperty
			= DependencyProperty.Register("EditedObject", typeof(IPropertyEditorDataSource), typeof(PropertyEditor),
			                              new FrameworkPropertyMetadata(null, _OnEditedObjectPropertyChanged));
		
		/// <summary>
		/// Creates a new PropertyEditor instance.
		/// </summary>
		public PropertyEditor()
		{
			try {
				InitializeComponent();
			} catch (Exception ex) {
				Debug.WriteLine(ex.ToString());
				throw;
			}
		}
		
		/// <summary>
		/// Gets/Sets the object being edited.
		/// </summary>
		public IPropertyEditorDataSource EditedObject {
			get { return (IPropertyEditorDataSource)GetValue(EditedObjectProperty); }
			set { SetValue(EditedObjectProperty, value); }
		}
		
		/// <summary>
		/// Is raised when the value of the <see cref="EditedObject"/> property changes.
		/// </summary>
		public event EventHandler EditedObjectChanged;
		
		static void _OnEditedObjectPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
		{
			((PropertyEditor)obj).OnEditedObjectPropertyChanged(e);
		}
		
		void OnEditedObjectPropertyChanged(DependencyPropertyChangedEventArgs e)
		{
			IPropertyEditorDataSource dataSource = e.NewValue as IPropertyEditorDataSource;
			componentImage.Fill = dataSource != null ? dataSource.CreateThumbnailBrush() : null;
			ShowProperties(dataSource);
			if (EditedObjectChanged != null) {
				EditedObjectChanged(this, EventArgs.Empty);
			}
		}
		
		void ShowPropertiesButton_Click(object sender, RoutedEventArgs e)
		{
			ShowProperties(this.EditedObject);
		}
		
		void ShowEventsButton_Click(object sender, RoutedEventArgs e)
		{
			ShowEvents(this.EditedObject);
		}
		
		bool useCategories = false;
		
		void ShowProperties(IPropertyEditorDataSource dataSource)
		{
			contentStackPanel.Children.Clear();
			if (dataSource == null)
				return;
			
			if (useCategories) {
				List<PropertyEditorCategoryView> categories = new List<PropertyEditorCategoryView>();
				foreach (IPropertyEditorDataProperty p in dataSource.Properties.OrderBy(p2 => p2.Name)) {
					if (p.Name == "Name") {
						continue;
					}
					PropertyEditorCategoryView cv = GetOrCreateCategory(categories, p.Category);
					PropertyGridView grid = (PropertyGridView)cv.Content;
					grid.AddProperty(p);
				}
				// Sort category titles alphabetically
				categories.Sort(delegate (PropertyEditorCategoryView c1, PropertyEditorCategoryView c2) {
				                	return c1.Header.ToString().CompareTo(c2.Header.ToString());
				                });
				// Add categories to contentStackPanel
				foreach (PropertyEditorCategoryView c in categories) {
					contentStackPanel.Children.Add(c);
				}
			} else {
				PropertyGridView grid = new PropertyGridView();
				contentStackPanel.Children.Add(grid);
				foreach (IPropertyEditorDataProperty p in dataSource.Properties.OrderBy(p2 => p2.Name)) {
					if (p.Name == "Name") {
						continue;
					}
					grid.AddProperty(p);
				}
			}
		}
		
		void ShowEvents(IPropertyEditorDataSource dataSource)
		{
			contentStackPanel.Children.Clear();
			if (dataSource == null)
				return;
			
			PropertyGridView grid = new PropertyGridView();
			contentStackPanel.Children.Add(grid);
			foreach (IPropertyEditorDataEvent e in dataSource.Events.OrderBy(p => p.Name)) {
				grid.AddEvent(e);
			}
		}
		
		HashSet<string> expandedCategories = new HashSet<string>();
		
		PropertyEditorCategoryView GetOrCreateCategory(List<PropertyEditorCategoryView> categories, string category)
		{
			foreach (PropertyEditorCategoryView c in categories) {
				if (c.Header.ToString() == category)
					return c;
			}
			PropertyEditorCategoryView newCategory = new PropertyEditorCategoryView();
			newCategory.Header = category;
			newCategory.Content = new PropertyGridView();
			newCategory.IsExpanded = expandedCategories.Contains(category);
			newCategory.Expanded += delegate {
				expandedCategories.Add(category);
			};
			newCategory.Collapsed += delegate {
				expandedCategories.Remove(category);
			};
			categories.Add(newCategory);
			return newCategory;
		}
		
		/*
		void clearSearchButton_Click(object sender, RoutedEventArgs e)
		{
			searchTextBox.Text = "";
		}
		 */
		
		void nameTextBox_SizeChanged(object sender, RoutedEventArgs e)
		{
			// Display componentImage only if there is enough space left.
			const double minimalNameTextBoxWidth = 80;
			const double switchTreshold = 5;
			if (componentImage.Visibility != Visibility.Collapsed) {
				if (nameTextBox.ActualWidth < minimalNameTextBoxWidth - switchTreshold) {
					componentImage.Visibility = Visibility.Collapsed;
				}
			} else {
				if (nameTextBox.ActualWidth > minimalNameTextBoxWidth + componentImage.Width + switchTreshold) {
					componentImage.Visibility = Visibility.Visible;
				}
			}
		}
		
		void nameTextBox_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.Handled) return;
			if (e.Key == Key.Enter) {
				nameTextBox.GetBindingExpression(TextBox.TextProperty).UpdateSource();
			} else if (e.Key == Key.Escape) {
				nameTextBox.GetBindingExpression(TextBox.TextProperty).UpdateTarget();
			}
		}
		
		public TextBox NameTextBox {
			get { return nameTextBox; }
		}
	}
}

