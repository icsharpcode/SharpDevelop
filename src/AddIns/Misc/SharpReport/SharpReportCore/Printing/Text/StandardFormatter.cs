/*
 * Created by SharpDevelop.
 * User: Forstmeier Peter
 * Date: 26.06.2006
 * Time: 09:42
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;
using System.Globalization;
/// <summary>
/// This Class handles the formatting of Output Values depending on there
/// Type and DbValue
/// </summary>
namespace SharpReportCore {
	
	internal class StandardFormatter : object {
		
		private StandardFormatter() {
			
		}
		
		//TODO why not TypeCode tc = Type.GetTypeCode( Type.GetType(this.dataType));
		
		public static string FormatItem (string valueToFormat,string format,
		                         TypeCode typeCode,string nullValue) {
			string retValue = String.Empty;
			
			if (String.IsNullOrEmpty(format)) {
				retValue = valueToFormat;
				return retValue;
			}
			
			switch (typeCode) {
				case TypeCode.Int16:
				case TypeCode.Int32:
					retValue = StandardFormatter.IntegerValues (valueToFormat,format);
					break;
				case TypeCode.DateTime:
					retValue = StandardFormatter.DateValues(valueToFormat,format);
					break;
				case TypeCode.Boolean:
					retValue = StandardFormatter.BoolValue (valueToFormat,format);
					break;
				case TypeCode.Decimal:
					retValue = StandardFormatter.DecimalValues (valueToFormat,format);
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

		
		
		private static string BoolValue (string toFormat, string format){
			string str = String.Empty;
			try {
				bool b = bool.Parse (toFormat);
				str = b.ToString (CultureInfo.CurrentCulture);
			} catch (System.FormatException) {
//						string s = String.Format("\tBool Value < {0} > {1}",toFormat,e.Message);
//					System.Console.WriteLine("\t\t{0}",s);
			}
			return str;
		}
		
		private static string IntegerValues(string toFormat, string format) {
			string str = String.Empty;
			if (StandardFormatter.CheckValue (toFormat)) {
				try {
					int number = Int32.Parse (toFormat,
					                          System.Globalization.NumberStyles.Any,
					                          CultureInfo.CurrentCulture.NumberFormat);
					
					str = number.ToString (format,CultureInfo.CurrentCulture);
				} catch (System.FormatException) {
//						string s = String.Format("\tDecimalValue < {0} > {1}",toFormat,e.Message);
//						System.Console.WriteLine("\t{0}",s);
				}
				return str;
			} else {
				str = (0.0M).ToString(CultureInfo.CurrentCulture);
			}
			return str;
		}
		
		private static string DecimalValues(string toFormat, string format) {
			string str = String.Empty;
			if (StandardFormatter.CheckValue (toFormat)) {
				try {
					decimal dec =	Decimal.Parse(toFormat,
					                            System.Globalization.NumberStyles.Any,
					                            CultureInfo.CurrentCulture.NumberFormat);
					str = dec.ToString (format,CultureInfo.CurrentCulture);
					
				} catch (System.FormatException) {
//						string s = String.Format("\tDecimalValue < {0} > {1}",toFormat,e.Message);
//						System.Console.WriteLine("\t{0}",s);
				}
				return str;
			} else {
				str = (0.0M).ToString(CultureInfo.CurrentCulture);
			}
			return str;
		}
		
		private static string DateValues(string toFormat, string format) {
			try {
				DateTime date = DateTime.Parse (toFormat.Trim(),
				                                CultureInfo.CurrentCulture.DateTimeFormat);
				string str = date.ToString(format,
				                           DateTimeFormatInfo.CurrentInfo);
				
				return str.Trim();
			} catch (System.FormatException) {
//					string s = String.Format("< {0} > {1}",toFormat,e.Message);
//					System.Console.WriteLine("\t\tDateValue {0}",s);
			}
			
			return toFormat.Trim();
		}
		
		private static bool CheckValue (string toFormat) {
			if (String.IsNullOrEmpty(toFormat)) {
				return false;
			}
			return true;
		}
	}
}
