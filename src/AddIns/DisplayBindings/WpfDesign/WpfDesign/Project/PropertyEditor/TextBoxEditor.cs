// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Controls;
using System.Windows.Documents;

namespace ICSharpCode.WpfDesign.PropertyEditor
{
	/// <summary>
	/// Type editor used to edit properties using a text box and the type's default type converter.
	/// </summary>
	sealed class TextBoxEditor : TextBox
	{
		/// <summary>
		/// Creates a new TextBoxEditor instance.
		/// </summary>
		public TextBoxEditor(IPropertyEditorDataProperty property)
		{
			Binding b = PropertyEditorBindingHelper.CreateBinding(this, property);
			b.Converter = new ToStringConverter(property.TypeConverter);
			SetBinding(TextProperty, b);
		}
		
		sealed class ToStringConverter : IValueConverter
		{
			readonly TypeConverter converter;
			
			public ToStringConverter(TypeConverter converter)
			{
				this.converter = converter;
			}
			
			public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
			{
				return converter.ConvertToString(null, culture, value);
			}
			
			public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
			{
				return converter.ConvertFromString(null, culture, (string)value);
			}
		}
	}
}
