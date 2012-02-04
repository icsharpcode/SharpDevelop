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
