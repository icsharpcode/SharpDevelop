using System;
using System.Windows;
using System.Windows.Data;

namespace ICSharpCode.SharpDevelop.Gui.OptionPanels
{
	[ValueConversion(typeof(Version), typeof(string))]
    public class VersionToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            try
            {
                var version = value as Version;
                if (version != null)
                    return version.ToString();

                return DependencyProperty.UnsetValue;
            }
            catch
            {
                return DependencyProperty.UnsetValue;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            try
            {
                var versionString = value as string;
                if (versionString != null)
                {
                    Version version;
                    if (Version.TryParse(versionString, out version))
                    {
                        return version;
                    }
                }

                return DependencyProperty.UnsetValue;
            }
            catch
            {
                return DependencyProperty.UnsetValue;
            }
        }
    }
}