// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Globalization;
using ICSharpCode.Reports.Core.Project.BaseClasses;

/// <summary>
/// This Class handles the formatting of Output Values depending on there
/// Type and DbValue
/// </summary>
namespace ICSharpCode.Reports.Core.BaseClasses.Printing
{
	
//	http://en.csharp-online.net/Create_List_Controls
	
	
	internal static class StandardFormatter
	{
		
		
		public static string FormatOutput(string valueToFormat,string format,
		                                     string dataType, string nullValue )
		{
			TypeCode typeCode = TypeHelpers.TypeCodeFromString(dataType);
			return FormatItem(valueToFormat,format,typeCode,nullValue);                                    
		}
		
		
		public static string FormatItem (string valueToFormat,string format,
		                         TypeCode typeCode,string nullValue)
		{
			string retValue = String.Empty;
			
			if (String.IsNullOrEmpty(format)) {
				retValue = valueToFormat;
				return retValue;
			}
			
			if (String.IsNullOrEmpty(valueToFormat)) {
				return nullValue;
			}
			
			switch (typeCode) {
				case TypeCode.Int16:
				case TypeCode.Int32:
					retValue = FormatIntegers (valueToFormat,format);
					break;
				case TypeCode.DateTime:
					retValue = FormatDate(valueToFormat,format);
					break;
				case TypeCode.Boolean:
					retValue = FormatBool (valueToFormat,format);
					break;
				case TypeCode.Decimal:
					retValue = FormatDecimal (valueToFormat,format);
					break;
					
				case TypeCode.Double:
				case TypeCode.Single:
					break;
					
				case TypeCode.String:
				case TypeCode.Char:
					retValue = valueToFormat;
					break;
				default:
					retValue = valueToFormat;
					break;
			}
			
			return retValue;
		}
		
		
		private static string FormatBool (string toFormat, string format)
		{
			if (CheckValue(toFormat)) {
				bool b = bool.Parse (toFormat);
				return b.ToString (CultureInfo.CurrentCulture);
			}
			return toFormat;
		}
	
		private static string FormatIntegers(string toFormat, string format)
		{
			string str = String.Empty;
			if (CheckValue (toFormat)) {
				try {
					int number = Int32.Parse (toFormat,
					                          System.Globalization.NumberStyles.Any,
					                          CultureInfo.CurrentCulture.NumberFormat);
					
					str = number.ToString (format,CultureInfo.CurrentCulture);
				} catch (System.FormatException e) {
					throw e;
				}
				return str;
			} else {
				str = (0.0M).ToString(CultureInfo.CurrentCulture);
			}
			return str;
		}
		
		
		private static string FormatDecimal(string toFormat, string format)
		{
			string str = String.Empty;
			if (CheckValue (toFormat)) {
				try {
					decimal dec =	Decimal.Parse(toFormat,
					                            System.Globalization.NumberStyles.Any,
					                            CultureInfo.CurrentCulture.NumberFormat);
					str = dec.ToString (format,CultureInfo.CurrentCulture);
					
				} catch (System.FormatException e) {
					throw e;
				}
				return str;
			} else {
				str = (0.0M).ToString(CultureInfo.CurrentCulture);
			}
			return str;
		}
		
		
		private static string FormatDate(string toFormat, string format)
		{
			try {
				DateTime date = DateTime.Parse (toFormat.Trim(),
				                                CultureInfo.CurrentCulture.DateTimeFormat);
				string str = date.ToString(format,
				                           DateTimeFormatInfo.CurrentInfo);
				
				return str.Trim();
			} catch (System.FormatException ) {
//					string s = String.Format("< {0} > {1}",toFormat,e.Message);
//					System.Console.WriteLine("\t\tDateValue {0}",s);
			}
			
			return toFormat.Trim();
		}
		
		private static bool CheckValue (string toFormat)
		{
			if (String.IsNullOrEmpty(toFormat)) {
				return false;
			}
			return true;
		}
	}
}
