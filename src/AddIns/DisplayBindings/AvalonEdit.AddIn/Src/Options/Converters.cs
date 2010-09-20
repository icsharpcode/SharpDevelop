// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace ICSharpCode.AvalonEdit.AddIn.Options
{
	sealed class BooleanToBoldConverter : IValueConverter
	{
		public static readonly BooleanToBoldConverter Instance = new BooleanToBoldConverter();
		
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if ((bool)value)
				return FontWeights.Bold;
			else
				return FontWeights.Normal;
		}
		
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotSupportedException();
		}
	}
	
	sealed class BooleanToDefaultStringConverter : IValueConverter
	{
		public static readonly BooleanToDefaultStringConverter Instance = new BooleanToDefaultStringConverter();
		
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if ((bool)value)
				return "(Default)";
			else
				return null;
		}
		
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotSupportedException();
		}
	}
}
