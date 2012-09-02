// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace ICSharpCode.SharpDevelop.Widgets
{
	[ValueConversion(typeof(bool), typeof(Visibility))]
	public class BoolToVisibilityConverter : IValueConverter
	{
		public Visibility TrueValue { get; set; }
		public Visibility FalseValue { get; set; }
		
		public BoolToVisibilityConverter()
		{
			this.TrueValue = Visibility.Visible;
			this.FalseValue = Visibility.Collapsed;
		}
		
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if ((value as bool?) == true)
				return TrueValue;
			else
				return FalseValue;
		}
		
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return value is Visibility && (Visibility)value == TrueValue;
		}
	}
}
