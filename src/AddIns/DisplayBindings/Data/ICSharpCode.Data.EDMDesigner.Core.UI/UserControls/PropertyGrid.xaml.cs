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

#region Usings

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Shapes;

using ICSharpCode.Core.Presentation;
using ICSharpCode.Data.EDMDesigner.Core.EDMObjects.Common;
using ICSharpCode.Data.EDMDesigner.Core.EDMObjects.CSDL;
using ICSharpCode.Data.EDMDesigner.Core.EDMObjects.CSDL.Attributes;
using ICSharpCode.Data.EDMDesigner.Core.EDMObjects.CSDL.Type;
using ICSharpCode.Data.EDMDesigner.Core.EDMObjects.Designer.CSDL.Association;
using ICSharpCode.Data.EDMDesigner.Core.UI.Converters;
using ICSharpCode.Data.EDMDesigner.Core.UI.UserControls.Common;

#endregion

namespace ICSharpCode.Data.EDMDesigner.Core.UI.UserControls
{
    public partial class PropertyGrid : UserControl
    {
        private const double ROW_HEIGHT = 30;
        private const string EXTENDEDPANEL_STYLE = "ExtendedPanelStyle";
        private const string TEXTBLOCK_STYLE = "TextBlockStyle";
        private const string TEXTBOX_STYLE = "TextBoxStyle";
        private const string COMBOBOX_STYLE = "ComboBoxStyle";

        public PropertyGrid()
        {
            InitializeComponent();
        }

        public CSDLContainer CSDL
        {
            get { return (CSDLContainer)GetValue(CSDLProperty); }
            set { SetValue(CSDLProperty, value); }
        }
        public static readonly DependencyProperty CSDLProperty =
            DependencyProperty.Register("CSDL", typeof(CSDLContainer), typeof(PropertyGrid), new UIPropertyMetadata(null));

        public void Bind(object businessObject)
        {
            propertiesGrid.Children.Clear();

            if (businessObject == null)
            {
                Visibility = Visibility.Collapsed;
                return;
            }
            var namedElement = businessObject as EDMObjectBase;
            nameTextBlock.Text = namedElement == null ? "" : namedElement.Name;
            Visibility = Visibility.Visible;
            var type = businessObject.GetType();
            int rowIndex = 0;
            foreach (var property in from p in type.GetProperties()
                                     where p.CanRead && p.CanWrite && p.GetGetMethod() != null && p.GetGetMethod().IsPublic && p.GetSetMethod() != null && p.GetSetMethod().IsPublic
                                     let resourceName = GetPropertyName(p.Name)
                                     let displayNameAttribute = resourceName != null ? null : p.GetAttribute<DisplayNameAttribute>()
                                     let name = resourceName ?? (displayNameAttribute == null ? p.Name : displayNameAttribute.DisplayName)
                                     let descriptionAttribute = p.GetAttribute<DescriptionAttribute>()
                                     let description = GetPropertyDescription(p.Name) ?? (descriptionAttribute == null ? null : descriptionAttribute.Description)
                                     let isVisbleAttribute = p.GetAttribute<DisplayVisibleConditionAttribute>()
                                     let isEnabledAttribute = p.GetAttribute<DisplayEnabledConditionAttribute>()
                                     orderby name
                                     select new { Property = p, Name = name, Description = description, IsVisibleProperty = isVisbleAttribute == null ? null : isVisbleAttribute.ConditionPropertyName, IsEnabledProperty = isEnabledAttribute == null ? null : isEnabledAttribute.ConditionPropertyName })
            {
                propertiesGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

                var extendedPanel = new ExtendedPanel { Height = ROW_HEIGHT, Opacity = 0.2, Style = (Style)FindResource(EXTENDEDPANEL_STYLE) };
                propertiesGrid.Children.Add(extendedPanel);
                SetVisibleBinding(extendedPanel, businessObject, property.IsVisibleProperty);
                Grid.SetColumnSpan(extendedPanel, 2);
                Grid.SetRow(extendedPanel, rowIndex);

                var textBlock = new TextBlock { Text = property.Name, VerticalAlignment = VerticalAlignment.Center, Margin = new Thickness(5, 0, 5, 0), HorizontalAlignment = HorizontalAlignment.Stretch, Style = (Style)FindResource(TEXTBLOCK_STYLE) };
                if (property.IsEnabledProperty != null)
                    textBlock.SetBinding(TextBlock.IsEnabledProperty, new Binding(property.IsEnabledProperty) { Source = businessObject });
                SetVisibleBinding(textBlock, businessObject, property.IsVisibleProperty);
                textBlock.MouseDown += (sender, e) =>
                    {
                        extendedPanel.Focus();
                        e.Handled = true;
                    };
                textBlock.GotFocus += delegate { descriptionTextBlock.Text = property.Description; };
                propertiesGrid.Children.Add(textBlock);
                extendedPanel.AddLogicalChild(textBlock);
                Grid.SetColumn(textBlock, 0);
                Grid.SetRow(textBlock, rowIndex);

                FrameworkElement valueUIElement;
                var propertyType = property.Property.PropertyType;
                var valuePropertyName = property.Property.Name;
                if (propertyType.IsEnum)
                {
                    if (propertyType == typeof(Cardinality))
                        valueUIElement = CreateCardinalityComboBox(businessObject, valuePropertyName, property.IsVisibleProperty, property.IsEnabledProperty);
                    else
                        valueUIElement = CreateComboBox(businessObject, Enum.GetValues(propertyType), null, valuePropertyName, property.IsVisibleProperty, property.IsEnabledProperty);
                }
                else if (propertyType == typeof(bool))
                    valueUIElement = CreateCheckBox(businessObject, false, valuePropertyName, property.IsVisibleProperty, property.IsEnabledProperty);
                else if (propertyType == typeof(bool?))
                    valueUIElement = CreateCheckBox(businessObject, true, valuePropertyName, property.IsVisibleProperty, property.IsEnabledProperty);
                else if (propertyType == typeof(EntityType))
                {
                    IEnumerable<EntityType> entityTypes = CSDL.EntityTypes;
                    var entityType = businessObject as EntityType;
                    if (entityType != null)
                    {
                        if (property.Property.GetAttribute<ExcludeItselftAttribute>() != null)
                            entityTypes = entityTypes.Except(new[] { entityType });
                        if (property.Property.GetAttribute<ExcludeChildrenTypesAttribute>() != null)
                            entityTypes = entityTypes.Where(et => !entityType.IsBaseType(et));
                    }
                    if (property.Property.GetAttribute<AddNullAttribute>() != null)
                        valueUIElement = CreateComboBoxWithNull(businessObject, entityTypes, "Name", valuePropertyName, property.IsVisibleProperty, property.IsEnabledProperty);
                    else
                        valueUIElement = CreateComboBox(businessObject, entityTypes, "Name", valuePropertyName, property.IsVisibleProperty, property.IsEnabledProperty);
                }
                else
                    valueUIElement = CreateTextBox(businessObject, valuePropertyName, property.IsVisibleProperty, property.IsEnabledProperty);
                valueUIElement.Margin = new Thickness(0, 0, 5, 0);
                valueUIElement.VerticalAlignment = VerticalAlignment.Center;
                valueUIElement.GotFocus += delegate { descriptionTextBlock.Text = property.Description; };
                propertiesGrid.Children.Add(valueUIElement);
                extendedPanel.AddLogicalChild(valueUIElement);
                Grid.SetColumn(valueUIElement, 1);
                Grid.SetRow(valueUIElement, rowIndex++);

                propertiesGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
                var line = new Line { Height = 2, Margin = new Thickness(0, 3, 0, 1), Stroke = Brushes.Black, X1 = 0, Y1 = 0, Y2 = 0 };
                line.SetBinding(Line.X2Property, new Binding("ActualWidth") { Source = propertiesGrid });
                propertiesGrid.Children.Add(line);
                SetVisibleBinding(line, businessObject, property.IsVisibleProperty);
                Grid.SetColumnSpan(line, 2);
                Grid.SetRow(line, rowIndex++);
            }
        }

        private TextBox CreateTextBox(object businessObject, string bindingPropertyName, string isVisiblePropetyName, string isEnabledPropertyName)
        {
            var textBox = new TextBox { Style = (Style)FindResource(TEXTBOX_STYLE) };
            SetCommunBindings(textBox, TextBox.TextProperty, businessObject, bindingPropertyName, isVisiblePropetyName);
            if (isEnabledPropertyName != null)
                textBox.SetBinding(TextBox.IsReadOnlyProperty, new Binding(isEnabledPropertyName) { Source = businessObject, Converter = new NotBoolConverter() });
            return textBox;
        }

        private ComboBoxEditableWhenFocused CreateComboBox(object businessObject, IEnumerable items, string displayMemberPath, string bindingPropertyName, string isVisiblePropetyName, string isEnabledPropertyName)
        {
            var comboBox = new ComboBoxEditableWhenFocused { Style = (Style)FindResource(COMBOBOX_STYLE), ItemsSource = items, DisplayMemberPath = displayMemberPath };
            SetCommunBindings(comboBox, ComboBoxEditableWhenFocused.SelectedValueProperty, businessObject, bindingPropertyName, isVisiblePropetyName);
            if (isEnabledPropertyName != null)
                comboBox.SetBinding(ComboBoxEditableWhenFocused.IsReadOnlyProperty, new Binding(isEnabledPropertyName) { Source = businessObject, Converter = new NotBoolConverter() });
            return comboBox;
        }

        private ComboBoxEditableWhenFocused CreateCardinalityComboBox(object businessObject, string bindingPropertyName, string isVisiblePropetyName, string isEnabledPropertyName)
        {
            var comboBox = new ComboBoxEditableWhenFocused { Style = (Style)FindResource(COMBOBOX_STYLE), ItemsSource = CardinalityList.Instance, DisplayMemberPath = "Text", SelectedValuePath = "Value" };
            SetCommunBindings(comboBox, ComboBoxEditableWhenFocused.SelectedValueProperty, businessObject, bindingPropertyName, isVisiblePropetyName);
            if (isEnabledPropertyName != null)
                comboBox.SetBinding(ComboBoxEditableWhenFocused.IsReadOnlyProperty, new Binding(isEnabledPropertyName) { Source = businessObject, Converter = new NotBoolConverter() });
            return comboBox;
        }

        private ComboBoxEditableWhenFocused CreateComboBoxWithNull<T>(object businessObject, IEnumerable<T> items, string displayMemberPath, string bindingPropertyName, string isVisiblePropetyName, string isEnabledPropertyName) where T : class
        {
            var style = (Style)FindResource(COMBOBOX_STYLE);
            var comboBox = new ComboBoxEditableWhenFocused();
            var itemType = items.GetType();
            var nullValues = NullValue.GetValues("(none)");
            comboBox.ItemsSource = new CompositeCollection() { new CollectionContainer() { Collection = nullValues }, new CollectionContainer() { Collection = items } };
            var newStyle = new Style(typeof(ComboBoxEditableWhenFocused));
            var fef = new FrameworkElementFactory(typeof(TextBlock));
            fef.SetBinding(TextBlock.TextProperty, new Binding(displayMemberPath));
            var styleResourceDictionary = new ResourceDictionary();
            foreach (var trigger in style.Triggers)
                newStyle.Triggers.Add(trigger);
            foreach (var resourceKey in style.Resources.Keys)
                styleResourceDictionary.Add(resourceKey, style.Resources[resourceKey]);
            styleResourceDictionary.Add(itemType.GetGenericArguments()[0], new DataTemplate(itemType.GetGenericArguments()[0]) { VisualTree = fef });
            newStyle.Resources = styleResourceDictionary;
            comboBox.Style = newStyle;
            SetCommunBindings(comboBox, ComboBoxEditableWhenFocused.SelectedValueProperty, new ComboBoxSelectedValueBindingWithNull<T>(businessObject.GetType().GetProperty(bindingPropertyName).GetValue(businessObject, null) ?? nullValues[0], (T value) => businessObject.GetType().GetProperty(bindingPropertyName).SetValue(businessObject, value, null)), "ComboSelectedValue", isVisiblePropetyName);
            if (isEnabledPropertyName != null)
                comboBox.SetBinding(ComboBoxEditableWhenFocused.IsReadOnlyProperty, new Binding(isEnabledPropertyName) { Source = businessObject, Converter = new NotBoolConverter() });
            return comboBox;
        }

        private CheckBox CreateCheckBox(object businessObject, bool threeState, string bindingPropertyName, string isVisiblePropetyName, string isEnabledPropertyName)
        {
            var checkBox = new CheckBox { IsThreeState = threeState, HorizontalAlignment = HorizontalAlignment.Left };
            SetCommunBindings(checkBox, CheckBox.IsCheckedProperty, businessObject, bindingPropertyName, isVisiblePropetyName);
            if (isEnabledPropertyName != null)
                checkBox.SetBinding(CheckBox.IsEnabledProperty, new Binding(isEnabledPropertyName) { Source = businessObject });
            return checkBox;
        }

        private void SetCommunBindings(FrameworkElement frameworkElement, DependencyProperty valueProperty, object businessObject, string bindingPropertyName, string isVisiblePropetyName)
        {
            frameworkElement.SetBinding(valueProperty, new Binding(bindingPropertyName) { Source = businessObject, Mode = BindingMode.TwoWay, UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged });
            SetVisibleBinding(frameworkElement, businessObject, isVisiblePropetyName);
        }

        private void SetVisibleBinding(FrameworkElement frameworkElement, object businessObject, string visibleBindingPropertyName)
        {
            if (visibleBindingPropertyName != null)
                frameworkElement.SetBinding(FrameworkElement.VisibilityProperty, new Binding(visibleBindingPropertyName) { Source = businessObject, Converter = new BoolToVisibilityConverter() });
        }

        private void PropertiesGrid_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            var scrollViewer = VisualTreeHelperUtil.GetControlAscendant<ScrollViewer>(this);
            var maxWidth = scrollViewer.ActualWidth - 10;
            if (propertiesGrid.ActualWidth > maxWidth)
                propertiesGrid.Width = maxWidth;
            else if (propertiesGrid.ActualWidth != maxWidth)
                propertiesGrid.Width = double.NaN;
        }
    }

    public static class PropertyInfoExtension
    {
        public static T GetAttribute<T>(this PropertyInfo pi) where T : Attribute
        {
            object[] attributes = pi.GetCustomAttributes(typeof(T), true);
            if (attributes.Length == 0)
                return null;
            return attributes[0] as T;
        }
    }

}
