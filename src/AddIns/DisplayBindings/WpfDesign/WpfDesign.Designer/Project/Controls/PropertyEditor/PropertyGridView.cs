// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Windows;
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
			this.ColumnDefinitions[0].Width = new GridLength(0.45, GridUnitType.Star);
			this.ColumnDefinitions[0].MinWidth = 40;
			this.ColumnDefinitions[1].Width = new GridLength(0.55, GridUnitType.Star);
			this.ColumnDefinitions[2].Width = new GridLength(16);
		}
		
		/// <summary>
		/// Adds a new property to the PropertyGridView.
		/// </summary>
		public void AddProperty(IPropertyEditorDataProperty property)
		{
			this.RowDefinitions.Add(new RowDefinition());
			
			Label propertyNameLabel = new Label();
			propertyNameLabel.Content = property.Name;
			propertyNameLabel.HorizontalContentAlignment = HorizontalAlignment.Right;
			SetRow(propertyNameLabel, this.RowDefinitions.Count - 1);
			SetColumn(propertyNameLabel, 0);
			this.Children.Add(propertyNameLabel);
			
			DependencyPropertyDotButton dotButton = new DependencyPropertyDotButton();
			dotButton.VerticalAlignment = VerticalAlignment.Center;
			dotButton.HorizontalAlignment = HorizontalAlignment.Center;
			Binding binding = new Binding("IsSet");
			binding.Source = property;
			dotButton.SetBinding(DependencyPropertyDotButton.CheckedProperty, binding);
			SetRow(dotButton, this.RowDefinitions.Count - 1);
			SetColumn(dotButton, 2);
			this.Children.Add(dotButton);
		}
	}
}
