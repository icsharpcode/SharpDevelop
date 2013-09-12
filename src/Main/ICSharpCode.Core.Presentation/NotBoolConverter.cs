// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

#region Usings

using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;

#endregion

namespace ICSharpCode.Core.Presentation
{
	public class NotBoolConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			if (value is bool && (bool)value)
				return false;
			else
				return true;
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			if (value is bool && (bool)value)
				return false;
			else
				return true;
		}
	}
	
	public class ContextMenuBuilder : IMultiValueConverter
	{
		public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
		{
			if (values.Length == 2 && values[0] is string) {
				return MenuService.CreateContextMenu(values[1], (string)values[0]);
			}
			throw new NotSupportedException();
		}

		public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
		{
			throw new NotSupportedException();
		}
	}
}
