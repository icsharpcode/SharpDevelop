// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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

namespace ICSharpCode.SharpDevelop.Gui
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
