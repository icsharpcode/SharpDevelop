using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Windows.Input;

namespace ICSharpCode.ShortcutsManagement.Converters
{
    public class InputGestureConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if(value is KeyGesture && targetType == typeof(string))
            {
                return new KeyGestureConverter().ConvertToInvariantString(value).Replace("+", " + ");
            } 
            
            if(value is MouseGesture && targetType == typeof(string))
            {
                return new MouseGestureConverter().ConvertToInvariantString(value).Replace("+", " + ");
            }

            return "";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new System.NotImplementedException();
        }
    }
}
