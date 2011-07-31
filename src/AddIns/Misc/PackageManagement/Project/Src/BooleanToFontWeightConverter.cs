// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace ICSharpCode.PackageManagement
{
	[ValueConversion(typeof(Boolean), typeof(FontWeight))]
	public class BooleanToFontWeightConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value is Boolean) {
				Boolean fontIsBold = (Boolean)value;
				return ConvertToFontWeight(fontIsBold);
			}
			return DependencyProperty.UnsetValue;
		}
		
		FontWeight ConvertToFontWeight(Boolean bold)
		{
			if (bold) {
				return FontWeights.Bold;
			}
			return FontWeights.Normal;
		}
		
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value is FontWeight) {
				FontWeight fontWeight = (FontWeight)value;
				return ConvertToBoolean(fontWeight);
			}
			return DependencyProperty.UnsetValue;
		}
		
		bool ConvertToBoolean(FontWeight fontWeight)
		{
			return fontWeight == FontWeights.Bold;
		}
	}
}
