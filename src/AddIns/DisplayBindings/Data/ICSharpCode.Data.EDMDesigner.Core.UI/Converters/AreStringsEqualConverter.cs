// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

#region Usings

using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

#endregion

namespace ICSharpCode.Data.EDMDesigner.Core.UI.Converters
{
    public class AreStringsEqualConverter : IValueConverter
    {
        public bool ReturnVisibility { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null || DependencyProperty.UnsetValue.Equals(value))
                return false;

            if (value.ToString() == parameter.ToString())
            {
                if (ReturnVisibility)
                    return Visibility.Visible;
                else
                    return true;
            }
            else
            {
                if (ReturnVisibility)
                    return Visibility.Collapsed;
                else
                    return false;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Binding.DoNothing;
        }
    }
}
