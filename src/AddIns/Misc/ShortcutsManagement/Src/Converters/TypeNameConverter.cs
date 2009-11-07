using System;
using System.Globalization;
using System.Windows.Data;

namespace ICSharpCode.ShortcutsManagement.Converters
{
	/// <summary>
	/// Convert object to its type name
	/// </summary>
	class TypeNameConverter : IValueConverter
	{
		/// <summary>
		/// Convert object to type name
		/// </summary>
		/// <param name="value">Any object</param>
		/// <param name="targetType">Convertion target type (Only string is supported)</param>
		/// <param name="parameter">Not used</param>
		/// <param name="culture">Not used</param>
		/// <returns>Passed object type name</returns>
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return value.GetType().Name;
		}
		
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotSupportedException();
		}
	}
}
