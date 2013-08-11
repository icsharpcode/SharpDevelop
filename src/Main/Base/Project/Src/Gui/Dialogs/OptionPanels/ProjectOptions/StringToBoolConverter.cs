// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Windows.Data;

namespace ICSharpCode.SharpDevelop.Gui.OptionPanels
{
	/// <summary>
	/// Description of StringToBoolConverter.
	/// </summary>
	public class StringToBoolConverter:IValueConverter
	{
		
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			
			if ("True".Equals(value.ToString(),StringComparison.OrdinalIgnoreCase)) {
				return true;
			}
			return false;
		}
		
		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			if ("True".Equals(value.ToString(),StringComparison.OrdinalIgnoreCase)) {
				return "True";
			}
			return "False";
		}
	}
}
