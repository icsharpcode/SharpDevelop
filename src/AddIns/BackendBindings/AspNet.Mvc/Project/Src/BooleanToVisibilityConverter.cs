// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace ICSharpCode.AspNet.Mvc
{
	public class BooleanToVisibilityConverter : IValueConverter
	{
		public bool Hidden { get; set; }
		public bool IsReversed { get; set; }
		
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			bool visible = IsVisible((bool)value);
			if (visible) {
				return Visibility.Visible;
			} else if (Hidden) {
				return Visibility.Hidden;
			}
			return Visibility.Collapsed;
		}
		
		bool IsVisible(bool value)
		{
			if (IsReversed) {
				return !value;
			}
			return value;
		}
		
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return IsVisible((Visibility)value);
		}
		
		bool IsVisible(Visibility visibility)
		{
			bool visible = visibility == Visibility.Visible;
			return IsVisible(visible);
		}
	}
}
