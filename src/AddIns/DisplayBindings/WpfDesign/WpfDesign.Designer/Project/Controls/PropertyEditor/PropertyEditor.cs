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
	/// Description of PropertyGrid.
	/// </summary>
	public partial class PropertyEditor : UserControl
	{
		/// <summary>
		/// Dependency property for <see cref="EditedObject"/>.
		/// </summary>
		public static readonly DependencyProperty EditedObjectProperty
			= DependencyProperty.Register("EditedObject", typeof(IPropertyEditorDataSource), typeof(PropertyEditor),
			                              new FrameworkPropertyMetadata(null, _OnEditedObjectPropertyChanged));
		
		#if XAML_DEFINITIONS
		// this is not compiled, but gives us code-completion inside SharpDevelop
		TextBox nameTextBox;
		Label typeLabel;
		Image componentImage;
		TextBox searchTextBox;
		StackPanel contentStackPanel;
		#endif
		
		/// <summary>
		/// Creates a new PropertyGrid instance.
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
			ShowProperties(e.NewValue as IPropertyEditorDataSource);
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
			
		}
		
		bool useCategories = false;
		
		void ShowProperties(IPropertyEditorDataSource dataSource)
		{
			contentStackPanel.Children.Clear();
			if (dataSource == null)
				return;
			
			if (useCategories) {
				List<PropertyEditorCategoryView> categories = new List<PropertyEditorCategoryView>();
				foreach (IPropertyEditorDataProperty p in Func.Sort(dataSource.Properties, ComparePropertyNames)) {
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
				foreach (IPropertyEditorDataProperty p in Func.Sort(dataSource.Properties, ComparePropertyNames)) {
					if (p.Name == "Name") {
						continue;
					}
					grid.AddProperty(p);
				}
			}
		}
		
		static int ComparePropertyNames(IPropertyEditorDataProperty p1, IPropertyEditorDataProperty p2)
		{
			return p1.Name.CompareTo(p2.Name);
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
	}
}
