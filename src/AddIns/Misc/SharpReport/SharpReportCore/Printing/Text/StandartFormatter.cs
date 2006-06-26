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
	public class StandartFormatter : object {
		
		
		public StandartFormatter() {
		}
		
		///<summary>Looks witch formatting Class to use, call the approbiate formatter
		/// and update the DbValue with the formatted String value
		/// </summary>
		///<param name="item">A ReportDataItem</param>
		/// 
		public string FormatItem (BaseDataItem item) {
			
			if (item == null) {
				throw new ArgumentNullException("item");
			}
			string retValue = String.Empty;
			
			switch (item.DataType) {
					
				case "System.DateTime" :
					retValue = DateValues(item.DbValue,item.FormatString);
					break;
					
				case "System.Int16":
					retValue = IntegerValues ("16",item.DbValue,item.FormatString);
					break;
					
				case "System.Int32" :
					retValue = IntegerValues ("32",item.DbValue,item.FormatString);
					break;
				case "System.Decimal":
					retValue = DecimalValues (item.DbValue,item.FormatString);
					break;

				case "System.Boolean":
					retValue = BoolValue (item.DbValue,item.FormatString);
					break;
				default:
					retValue = item.DbValue;
					break;
			}
			return retValue;
		}
		
		public string BoolValue (string toFormat, string format){
			string str = String.Empty;
			if (StandartFormatter.CheckFormat(format) == true) {
				
				if (StandartFormatter.CheckValue (toFormat)) {
					try {
						bool b = bool.Parse (toFormat);
						str = b.ToString (CultureInfo.CurrentCulture);
						
					} catch (System.FormatException) {
//						string s = String.Format("\tBool Value < {0} > {1}",toFormat,e.Message);
//					System.Console.WriteLine("\t\t{0}",s);
					}
				}
			} else {
				str = toFormat;
			}
			return str;
		}
		
		public  string IntegerValues(string valueType,string toFormat, string format) {
			string str = String.Empty;
			if (StandartFormatter.CheckFormat(format) == true) {
				if (StandartFormatter.CheckValue (toFormat)) {
					try {
						int number;
						switch (valueType) {
							case "16":
								number = Int16.Parse (toFormat,
								                      System.Globalization.NumberStyles.Any,
								                      CultureInfo.CurrentCulture.NumberFormat);
								break;
							case "32" :
								number = Int32.Parse (toFormat,
								                      System.Globalization.NumberStyles.Any,
								                      CultureInfo.CurrentCulture.NumberFormat);
								break;
							default:
								throw new ArgumentException("DefaultFormater:IntegerValues Unknown intType ");
								
						}
						str = number.ToString (format,CultureInfo.CurrentCulture);
						
					} catch (System.FormatException) {
//						string s = String.Format("\tDecimalValue < {0} > {1}",toFormat,e.Message);
//						System.Console.WriteLine("\t{0}",s);
					}
					return str;
				} else {
					str = (0.0M).ToString(CultureInfo.CurrentCulture);
				}
				
			} else {
				str = toFormat;
			}
			return str;
		}
		
		public  string DecimalValues(string toFormat, string format) {
			string str = String.Empty;
			if (StandartFormatter.CheckFormat(format) == true) {
				
				if (StandartFormatter.CheckValue (toFormat)) {
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
				
			} else {
				str = toFormat;
			}
			return str;
		}
		
		public  string DateValues(string toFormat, string format) {
			
			if (StandartFormatter.CheckFormat(format) == true) {
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
			} else {
				return toFormat.Trim();
			}
			return toFormat.Trim();
		}
		
		private static bool CheckFormat (string format) {
			if (String.IsNullOrEmpty(format)) {
				return false;
			} 
			return true;
		}
		
		private static bool CheckValue (string toFormat) {
			if (String.IsNullOrEmpty(toFormat)) {
				return false;
			}
			return true;
		}
	}
}
