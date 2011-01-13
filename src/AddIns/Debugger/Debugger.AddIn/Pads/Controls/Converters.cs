// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

namespace Debugger.AddIn.Pads.Controls
{
    public class TreeListViewConverter : IValueConverter
    {
        private const double INDENTATION_SIZE = 10;

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null) return null;
            if (targetType == typeof(double) && typeof(DependencyObject).IsAssignableFrom(value.GetType()))
            {
                DependencyObject element = value as DependencyObject;
                int level = -1;
                for (; element != null; element = VisualTreeHelper.GetParent(element)) {
                	if (typeof(TreeViewItem).IsAssignableFrom(element.GetType())) {
                        level++;
                	}
                }
                return INDENTATION_SIZE * level;
            }
            
            throw new NotSupportedException();
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotSupportedException("This method is not supported.");
        }
    }
	
	public class BoolToVisibilityConverter : IMultiValueConverter
	{
		public object Convert(object[] values, Type targetType,
			object parameter, CultureInfo culture)
		{
			bool val = bool.Parse(parameter.ToString());
			return val == (bool.Parse(values[0].ToString()) && bool.Parse(values[1].ToString())) ? Visibility.Visible : Visibility.Collapsed;
		}
	
		public object[] ConvertBack(object value, Type[] targetTypes,
			object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
