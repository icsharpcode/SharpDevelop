using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Input;
using ICSharpCode.Core.Presentation;

namespace ICSharpCode.ShortcutsManagement.Converters
{
    /// <summary>
    /// Converts input gestures collection into string
    /// </summary>
    public class GesturesListConverter : IValueConverter
    {
        /// <summary>
        /// Convert collection of gestures to a string
        /// </summary>
        /// <param name="value">Collection of gestures</param>
        /// <param name="targetType">Convertion target type (only string is supported)</param>
        /// <param name="parameter">Not used</param>
        /// <param name="culture">Not used</param>
        /// <returns>String representing collection of gestures</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is InputGestureCollection && (targetType == typeof(string) || targetType.IsSubclassOf(typeof(string)))) {
                return new InputGestureCollectionConverter().ConvertToInvariantString(value);
            }

            if (value is ObservableCollection<InputGesture> && (targetType == typeof(string) || targetType.IsSubclassOf(typeof(string)))) {
                var inputGestureCollection = new InputGestureCollection();
                foreach (var gesture in (ObservableCollection<InputGesture>)value) {
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
