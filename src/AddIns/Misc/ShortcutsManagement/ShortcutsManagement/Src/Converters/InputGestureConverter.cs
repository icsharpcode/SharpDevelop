using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Input;
using ICSharpCode.Core.Presentation;

namespace ICSharpCode.ShortcutsManagement.Converters
{
    /// <summary>
    /// Converts input gesture to string representation
    /// </summary>
    public class InputGestureConverter : IValueConverter
    {
        /// <summary>
        /// Convert input gesture to string
        /// </summary>
        /// <param name="value">Input gesture</param>
        /// <param name="targetType">Convertion target type (Only string is supported)</param>
        /// <param name="parameter">Not used</param>
        /// <param name="culture">Not used</param>
        /// <returns></returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is KeyGesture && targetType == typeof(string))
            {
                var gestures = new InputGestureCollection(new[] {value});
                return new InputGestureCollectionConverter().ConvertToInvariantString(gestures).Replace("+", " + ").Replace(",", ", ");
            } 
            
            if(value is MouseGesture && targetType == typeof(string))
            {
                return new MouseGestureConverter().ConvertToInvariantString(value);
            }

            return "";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
