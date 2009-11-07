using System;
using System.Globalization;
using System.Windows.Data;

namespace ICSharpCode.ShortcutsManagement.Converters
{
	/// <summary>
	/// Converts double value to another double value by adding another value provided in converter parameter
	/// </summary>
	class AddidionConverter : IValueConverter
	{
		/// <summary>
		/// Convert double value to another double value by adding another value provided in converter parameter
		/// </summary>
		/// <param name="value">Original double value</param>
		/// <param name="targetType">Convertion target type</param>
		/// <param name="parameter">Another double value to be added to original value</param>
		/// <param name="culture">Not used</param>
		/// <returns>New double value</returns>
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			double additionParameter = 0;
			var additionParameterString = parameter as string;
			if (additionParameterString != null) {
				Double.TryParse(additionParameterString, out additionParameter);
			}
			
			if(value is double) {
				var doubleValue = (double)value;
				return doubleValue + additionParameter;
			}
			
			return value;
		}
		
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotSupportedException();
		}
	}
}
