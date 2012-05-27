// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace ICSharpCode.SharpDevelop.Refactoring
{
	[ValueConversion(typeof(bool), typeof(Visibility))]
	[Obsolete]
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
