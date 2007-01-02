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
using System.Windows.Input;

namespace ICSharpCode.WpfDesign.PropertyEditor
{
	/// <summary>
	/// Type editor used to edit properties using a text box and the type's default type converter.
	/// </summary>
	sealed class TextBoxEditor : TextBox
	{
		BindingExpressionBase bindingResult;
		
		/// <summary>
		/// Creates a new TextBoxEditor instance.
		/// </summary>
		public TextBoxEditor(IPropertyEditorDataProperty property)
		{
			Binding b = PropertyEditorBindingHelper.CreateBinding(this, property);
			b.Converter = new ToStringConverter(property.TypeConverter);
			bindingResult = SetBinding(TextProperty, b);
		}
		
		protected override void OnKeyDown(KeyEventArgs e)
		{
			base.OnKeyDown(e);
			if (!e.Handled && e.Key == Key.Enter) {
				string oldText = this.Text;
				bindingResult.UpdateSource();
				if (this.Text != oldText) {
					this.SelectAll();
				}
				e.Handled = true;
			}
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
