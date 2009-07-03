using System;
using System.Globalization;
using System.Windows.Data;

namespace ICSharpCode.ShortcutsManagement.Converters
{
    /// <summary>
    /// Converts null and empty string to true and all other values to false
    /// </summary>
    class NotNullOrEmptyConverter : IValueConverter
    {
        /// <summary>
        /// Converts null and empty string to true and all other values to false
        /// </summary>
        /// <param name="value">String or any other object</param>
        /// <param name="targetType">Convertion target type</param>
        /// <param name="parameter">Not used</param>
        /// <param name="culture">Not used</param>
        /// <returns>Bool value</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ((value is string && !string.IsNullOrEmpty((string)value)) || (!(value is string) && value != null))
            {
                return true;
            }

            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
