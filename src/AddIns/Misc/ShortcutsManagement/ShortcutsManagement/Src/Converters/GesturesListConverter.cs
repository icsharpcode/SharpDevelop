using System;
using System.Collections.Generic;
using System.Linq;
using System.Globalization;
using System.Windows.Data;

namespace ICSharpCode.ShortcutsManagement
{
    public class GesturesListConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is IEnumerable<string> && (targetType == typeof(string) || targetType.IsSubclassOf(typeof(string))))
            {
                var enumerableValue = (IEnumerable<string>)value;
                return string.Join(" | ", enumerableValue.ToArray());
            }

            return value.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException("Reverse convertion is not implemented:");
        }
    }
}
