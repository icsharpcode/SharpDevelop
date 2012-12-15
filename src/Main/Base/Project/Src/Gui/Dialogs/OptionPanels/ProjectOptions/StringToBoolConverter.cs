/*
 * Created by SharpDevelop.
 * User: Peter Forstmeier
 * Date: 05.05.2012
 * Time: 19:29
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
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
