using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Globalization;

namespace ICSharpCode.WpfDesign.Designer.Controls.TypeEditors.BrushEditor
{
	public class IntFromEnumConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return (int)value;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return Enum.ToObject(targetType, (int)value);
		}
	}
}
