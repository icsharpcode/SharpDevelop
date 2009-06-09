using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace ICSharpCode.ShortcutsManagement.Converters
{
    /// <summary>
    /// Converts boolean value to <see cref="Visibility" /> enum
    /// </summary>
    public class BoolToVisibilityConverter : IValueConverter
    {
        /// <summary>
        /// Convert forward
        /// </summary>
        /// <param name="value">Boolean value</param>
        /// <param name="targetType">Target type (Only string is supported)</param>
        /// <param name="parameter">Hide type (Hidden - reserve space but don't show, Collapse - do not reserve space)</param>
        /// <param name="culture">Culture info (Invariant culture is used always)</param>
        /// <returns>Visibility value</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if(value is bool)
            {
                var hidden = parameter ?? "Hidden";
                return (bool)value ? "Visible" : hidden;
            }

            throw new NotImplementedException();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
