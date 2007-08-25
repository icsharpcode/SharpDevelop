// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Data;
using System.Windows.Controls;
using ICSharpCode.WpfDesign.PropertyEditor;

namespace ICSharpCode.WpfDesign.Designer.Controls
{
	/// <summary>
	/// Control used to view a property grid category.
	/// </summary>
	public sealed class PropertyGridView : Grid
	{
		static PropertyGridView()
		{
			DefaultStyleKeyProperty.OverrideMetadata(typeof(PropertyGridView), new FrameworkPropertyMetadata(typeof(PropertyGridView)));
		}
		
		/// <summary>
		/// Creates a new PropertyGridView instance.
		/// </summary>
		public PropertyGridView()
		{
			this.ColumnDefinitions.Add(new ColumnDefinition());
			this.ColumnDefinitions.Add(new ColumnDefinition());
			this.ColumnDefinitions.Add(new ColumnDefinition());
			this.ColumnDefinitions[0].Width = new GridLength(0.48, GridUnitType.Star);
			this.ColumnDefinitions[0].MinWidth = 40;
			this.ColumnDefinitions[1].Width = new GridLength(0.52, GridUnitType.Star);
			this.ColumnDefinitions[2].Width = new GridLength(16);
		}
		
		/// <summary>
		/// Adds a new property to the PropertyGridView.
		/// </summary>
		public void AddProperty(IPropertyEditorDataProperty property)
		{
			this.RowDefinitions.Add(new RowDefinition());
			
			// Column 0: name of the property
			PropertyNameTextBlock propertyNameText = new PropertyNameTextBlock(property);
			propertyNameText.Margin = new Thickness(0, 0, 2, 0);
			SetRow(propertyNameText, this.RowDefinitions.Count - 1);
			SetColumn(propertyNameText, 0);
			this.Children.Add(propertyNameText);
			
			// Column 1: the actual property editor
			UIElement editor = property.CreateEditor();
			SetRow(editor, this.RowDefinitions.Count - 1);
			SetColumn(editor, 1);
			this.Children.Add(editor);
			
			// Column 2: a "dot" button
			DependencyPropertyDotButton dotButton = new DependencyPropertyDotButton(property);
			dotButton.VerticalAlignment = VerticalAlignment.Center;
			dotButton.HorizontalAlignment = HorizontalAlignment.Center;
			SetRow(dotButton, this.RowDefinitions.Count - 1);
			SetColumn(dotButton, 2);
			this.Children.Add(dotButton);
			propertyNameText.ContextMenuProvider = dotButton;
		}
		
		/// <summary>
		/// Adds a new event to the PropertyGridView.
		/// </summary>
		public void AddEvent(IPropertyEditorDataEvent dataEvent)
		{
			this.RowDefinitions.Add(new RowDefinition());
			
			// Column 0: name of the event
			PropertyNameTextBlock propertyNameText = new PropertyNameTextBlock(dataEvent);
			propertyNameText.Margin = new Thickness(0, 0, 2, 0);
			SetRow(propertyNameText, this.RowDefinitions.Count - 1);
			SetColumn(propertyNameText, 0);
			this.Children.Add(propertyNameText);
			
			// Column 1: the actual property editor
			EventHandlerEditor editor = new EventHandlerEditor(dataEvent);
			SetRow(editor, this.RowDefinitions.Count - 1);
			SetColumn(editor, 1);
			this.Children.Add(editor);
			
			propertyNameText.MouseDown += delegate(object sender, MouseButtonEventArgs e) {
				if (e.ChangedButton == MouseButton.Left && e.ClickCount == 2) {
					e.Handled = true;
					editor.DoubleClick();
				}
			};
			
			// Column 2: empty
		}
	}
}
