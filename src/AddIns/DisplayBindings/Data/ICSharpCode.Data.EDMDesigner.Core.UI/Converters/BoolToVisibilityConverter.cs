// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

#region Usings

using System;
using System.Windows.Data;
using System.Globalization;
using System.Windows;

#endregion

namespace ICSharpCode.Data.EDMDesigner.Core.UI.Converters
{
    public class BoolToVisibilityConverter : DependencyObject, IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ((bool)value)
                return Visibility.Visible;
            return Hidden;
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (Visibility)value == Visibility.Visible;
        }

        public Visibility Hidden
        {
            get { return (Visibility)GetValue(HiddenProperty); }
            set { SetValue(HiddenProperty, value); }
        }
        public static readonly DependencyProperty HiddenProperty =
            DependencyProperty.Register("Hidden", typeof(Visibility), typeof(BoolToVisibilityConverter), new UIPropertyMetadata(Visibility.Collapsed));
    }
}
