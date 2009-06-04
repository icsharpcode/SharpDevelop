using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Input;
using ICSharpCode.Core.Presentation;

namespace ICSharpCode.ShortcutsManagement.Converters
{
    public class GesturesListConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is InputGestureCollection && (targetType == typeof(string) || targetType.IsSubclassOf(typeof(string))))
            {
                return new InputGestureCollectionConverter().ConvertToInvariantString(value);
            }

            if (value is ObservableCollection<InputGesture> && (targetType == typeof(string) || targetType.IsSubclassOf(typeof(string))))
            {
                var inputGestureCollection = new InputGestureCollection();
                foreach (var gesture in (ObservableCollection<InputGesture>)value)
                {
                    inputGestureCollection.Add(gesture);
                }
                return new InputGestureCollectionConverter().ConvertToInvariantString(inputGestureCollection);
            }

            return value.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException("Reverse convertion is not implemented:");
        }
    }
}
