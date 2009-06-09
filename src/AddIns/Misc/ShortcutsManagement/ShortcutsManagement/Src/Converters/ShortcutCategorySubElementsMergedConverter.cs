using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows.Data;

namespace ICSharpCode.ShortcutsManagement.Converters
{
    /// <summary>
    /// Merges category's sub-categories and shortcuts into one list
    /// </summary>
    public class ShortcutCategorySubElementsMergedConverter : IMultiValueConverter
    {
        /// <summary>
        /// Merge category's sub-categories and shortcuts into one list
        /// </summary>
        /// <param name="values">List of sub-categories and list of shortcuts</param>
        /// <param name="targetType">Not used</param>
        /// <param name="parameter">Not used</param>
        /// <param name="culture">Not used</param>
        /// <returns>List of objects where objects are either a shortcut category or a shortcut</returns>
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            var items = new List<object>();
            items.AddRange(((IEnumerable<ShortcutCategory>)values[0]).Cast<object>());
            items.AddRange(((IEnumerable<Shortcut>)values[1]).Cast<object>());

            return items;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
