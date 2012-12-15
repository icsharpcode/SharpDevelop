/*
 * Created by SharpDevelop.
 * User: Peter Forstmeier
 * Date: 09.08.2012
 * Time: 20:10
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Diagnostics;
using System.Globalization;
using System.Windows.Controls;

namespace ICSharpCode.SharpDevelop.Gui.OptionPanels
{
	/// <summary>
	/// Description of HexValidator.
	/// </summary>
	public class  HexValidator :ValidationRule
	{
//		http://www.codeproject.com/Articles/15239/Validation-in-Windows-Presentation-Foundation
		public override ValidationResult Validate(object value, CultureInfo cultureInfo)
		{
			ValidationResult result = new ValidationResult(true, null);
			var str = value.ToString();
			var y = 0;
			var res = Int32.TryParse(str,NumberStyles.HexNumber,CultureInfo.InvariantCulture,out y);
			if (!res) {
				 result = new ValidationResult(false, "No valid Hex Digit");
			}
			return result;
		}
	}
	/*
	public class  BaseAdressValidator :ValidationRule
	{
		public override ValidationResult Validate(object value, CultureInfo cultureInfo)
		{
			Trace.WriteLine("-------------");
			ValidationResult result = new ValidationResult(true, null);
			string dllBaseAdress = value.ToString().Trim();
			NumberStyles style = NumberStyles.Integer;
			if (dllBaseAdress.StartsWith("0x")) {
				dllBaseAdress = dllBaseAdress.Substring(2);
				style = NumberStyles.HexNumber;
			}
			int val;
			if (!int.TryParse(dllBaseAdress, style, NumberFormatInfo.InvariantInfo, out val)) {
				result = new ValidationResult(false, "No valid Hex Digit");
			}
			return result;
		}
		
	}
	*/
}
