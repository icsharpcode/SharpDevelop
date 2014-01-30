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
using System.ComponentModel;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using ICSharpCode.Core;
using ICSharpCode.Core.Presentation;
using ICSharpCode.SharpDevelop.Widgets;

namespace ICSharpCode.SharpDevelop
{
	/// <summary>
	/// Used to bind ComboBoxes or RadioButtonGroups to enums.
	/// </summary>
	public static class EnumBinding
	{
		public static readonly DependencyProperty EnumTypeProperty =
			DependencyProperty.RegisterAttached("EnumType", typeof(Type), typeof(EnumBinding),
			                                    new FrameworkPropertyMetadata(OnEnumTypePropertyChanged));
		
		public static Type GetEnumType(Selector element)
		{
			return (Type)element.GetValue(EnumTypeProperty);
		}
		
		public static void SetEnumType(Selector element, Type enumType)
		{
			element.SetValue(EnumTypeProperty, enumType);
		}
		
		static string GetDescription(FieldInfo field)
		{
			foreach (DescriptionAttribute d in field.GetCustomAttributes(typeof(DescriptionAttribute), false))
				return d.Description;
			return field.Name;
		}
		
		static void OnEnumTypePropertyChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
		{
			Type enumType = e.NewValue as Type;
			if (enumType != null && enumType.IsEnum) {
				ComboBox comboBox = o as ComboBox;
				if (comboBox != null) {
					comboBox.SelectedValuePath = "Tag";
					comboBox.Items.Clear();
					foreach (FieldInfo field in enumType.GetFields()) {
						if (field.IsStatic) {
							ComboBoxItem item = new ComboBoxItem();
							item.Tag = field.GetValue(null);
							string description = GetDescription(field);
							item.SetValueToExtension(ComboBoxItem.ContentProperty, new StringParseExtension(description));
							comboBox.Items.Add(item);
						}
					}
				}
				RadioButtonGroup rbg = o as RadioButtonGroup;
				if (rbg != null) {
					rbg.Items.Clear();
					foreach (FieldInfo field in enumType.GetFields()) {
						if (field.IsStatic) {
							RadioButton b = new RadioButton();
							b.Tag = field.GetValue(null);
							string description = GetDescription(field);
							b.SetValueToExtension(RadioButton.ContentProperty, new StringParseExtension(description));
							rbg.Items.Add(b);
						}
					}
				}
			}
		}
	}
}
