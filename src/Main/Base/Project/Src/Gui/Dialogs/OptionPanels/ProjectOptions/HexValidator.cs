// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

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
