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
	
	public class ContextMenuBuilder : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value is string) {
				return MenuService.CreateContextMenu(parameter, (string)value);
			}
			throw new NotSupportedException();
		}
		
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotSupportedException();
		}
	}
}
