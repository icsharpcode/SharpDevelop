// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Martin Konicek" email="martin.konicek@gmail.com"/>
//     <version>$Revision: $</version>
// </file>
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace ICSharpCode.SharpDevelop.Refactoring
{
	[ValueConversion(typeof(bool), typeof(Visibility))]
	public class BoolToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ((bool)value)
                return Visibility.Visible;
            return Visibility.Hidden;
        }
        
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
        	throw new NotSupportedException();
        }
    }
}
