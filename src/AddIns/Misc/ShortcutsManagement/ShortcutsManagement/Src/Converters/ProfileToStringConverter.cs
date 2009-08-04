using System;
using System.Globalization;
using System.Windows.Data;
using ICSharpCode.Core;
using ICSharpCode.Core.Presentation;

namespace ICSharpCode.ShortcutsManagement.Converters
{
    /// <summary>
    /// Converts profile to representing string
    /// </summary>
    public class ProfileToStringConverter : IValueConverter
    {
        /// <summary>
        /// Converts profile to representing string
        /// </summary>
        /// <param name="value">UserGestureProfile instance</param>
        /// <param name="targetType">Convertion target type (only string is supported)</param>
        /// <param name="parameter">Not supported</param>
        /// <param name="culture">Not supported</param>
        /// <returns>String representing instance of UserGestureProfile</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var profile = value as UserGestureProfile;
            if(profile != null)
            {
                var readOnlyString = "";
                if(profile.ReadOnly)
                {
                    readOnlyString = string.Format(" ({0})", StringParser.Parse("${res:ShortcutsManagement.ReadOnly}"));
                }

                return profile.Text + readOnlyString;
            }

            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
