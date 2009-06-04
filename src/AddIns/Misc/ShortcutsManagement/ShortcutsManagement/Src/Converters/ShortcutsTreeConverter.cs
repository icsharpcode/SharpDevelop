using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows.Data;

namespace ICSharpCode.ShortcutsManagement.Converters
{
    public class ShortcutsTreeConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            var items = new List<object>();
            items.AddRange(((IEnumerable<ShortcutCategory>)values[0]).Cast<object>());
            items.AddRange(((IEnumerable<Shortcut>)values[1]).Cast<object>());

            return items;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new System.NotImplementedException();
        }
    }
}
