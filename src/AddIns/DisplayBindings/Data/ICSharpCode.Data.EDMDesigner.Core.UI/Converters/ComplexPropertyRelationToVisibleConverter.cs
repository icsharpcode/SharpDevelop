#region Usings

using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using ICSharpCode.Data.EDMDesigner.Core.UI.UserControls.Relations;

#endregion

namespace ICSharpCode.Data.EDMDesigner.Core.UI.Converters
{
    public class ComplexPropertyRelationToVisibleConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var complexPropertyRelation = ((ComplexPropertyRelation)value);
            if (parameter != null && (string)parameter == "3")
                return complexPropertyRelation.ThreePoints || complexPropertyRelation.FivePoints ? Visibility.Visible : Visibility.Collapsed;
            return complexPropertyRelation.FivePoints ? Visibility.Visible : Visibility.Collapsed;
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
