#region Usings

using System;
using System.Windows.Data;

#endregion

namespace ICSharpCode.Data.EDMDesigner.Core.UI.Converters
{
	#region ExpanderHeightConverter

	public class ExpanderHeightConverter : IMultiValueConverter
	{
		#region IMultiValueConverter Members

		public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			double result = 1.0;

			for (int i = 0; i < values.Length; i++)
				result *= (double)values[i];

			return result;
		}

		public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
		{
			return null;
		}

		#endregion
	}

	#endregion
}
