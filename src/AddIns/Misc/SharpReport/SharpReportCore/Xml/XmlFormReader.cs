// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Drawing;
using System.Globalization;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Xml;

namespace SharpReportCore {
	/// <summary>
	/// This class is able to generate a GUI definition out of a XML file.
	/// </summary>
	public class XmlFormReader	{
		readonly static Regex propertySet  = new Regex(@"(?<Property>[\w]+)\s*=\s*(?<Value>[\w\d]+)", RegexOptions.Compiled);
		
		public XmlFormReader(){
		}
		
		///<summary>
		///  Stolen from http://west-wind.com/weblog/posts/980.aspx
		/// Converts a String like "Verdana, 11.25pt, style=Bold,Italic,Underline,Strikeout"
		/// to a Font object
		/// </summary>
		public static object StringToTypedValue(string value,
		                                        Type targetType,
		                                        CultureInfo culture ){
			object Result = null;
			try {
				if ( targetType == typeof(string) )
					Result = value;
				
				else if (targetType == typeof(int))
					Result = int.Parse( value,
					                   System.Globalization.NumberStyles.Integer,
					                   culture.NumberFormat );
				
				else if (targetType  == typeof(byte) )
					Result = Convert.ToByte(value,CultureInfo.CurrentCulture);
				else if (targetType  == typeof(decimal))
					Result = Decimal.Parse(value,
					                       System.Globalization.NumberStyles.Any,
					                       culture.NumberFormat);
				
				else if (targetType  == typeof(double))
					Result = Double.Parse( value,
					                      System.Globalization.NumberStyles.Any,
					                      culture.NumberFormat);
				
				else if (targetType == typeof(bool))
				{
					if (value.ToLower(CultureInfo.CurrentCulture) == "true" || value.ToLower(CultureInfo.CurrentCulture) == "on" || value == "1")
						Result = true;
					else
						Result = false;
				}
				
				else if (targetType == typeof(DateTime))
					Result = Convert.ToDateTime(value,
					                            culture.DateTimeFormat);
				else if (targetType.IsEnum)
					
					Result = Enum.Parse(targetType,value);
				
				else{
					System.ComponentModel.TypeConverter converter =
						System.ComponentModel.TypeDescriptor.GetConverter(targetType);
					
					if (converter != null && converter.CanConvertFrom(typeof(string)) )
						Result =	converter.ConvertFromString(null,
						                                     System.Globalization.CultureInfo.InvariantCulture,
						                                     value );
					else{
						throw(new Exception("Type Conversion not handled in StringToTypedValue"));
					}
				}
			} catch (Exception) {
				IllegalFileFormatException ex = new IllegalFileFormatException("XmlFormreader:Wrong File Format");
				throw ex;
			}
			return Result;
		}

		///<summary>
		///  Stolen from http://west-wind.com/weblog/posts/980.aspx
		/// Converts a object like "Verdana, 11.25pt, style=Bold,Italic,Underline,Strikeout"
		/// to a Font object
		/// </summary>
		
		public static string TypedValueToString(object rawValue,
		                                        CultureInfo culture){
			
			Type ValueType = rawValue.GetType();
			string Return = null;
			
			if (ValueType == typeof(string) )
				Return = rawValue.ToString();
			
			else if ( ValueType == typeof(int) || ValueType == typeof(decimal) ||
			         
			         ValueType == typeof(double) || ValueType == typeof(float))
				
				Return = string.Format(culture.NumberFormat,"{0}",rawValue);
			
			else if(ValueType == typeof(DateTime))
				Return =  string.Format(culture.DateTimeFormat,"{0}",rawValue);
			
			else if(ValueType == typeof(bool))
				Return = rawValue.ToString();
			
			else if(ValueType == typeof(byte))
				Return = rawValue.ToString();
			
			else if(ValueType.IsEnum)
				Return = rawValue.ToString();
			
			else {
				
				// Any type that supports a type converter
				
				System.ComponentModel.TypeConverter converter =
					
					System.ComponentModel.TypeDescriptor.GetConverter( ValueType );
				
				if (converter != null && converter.CanConvertTo(typeof(string)) )
					
					Return = converter.ConvertToString( rawValue );
				
				else
					
					// Last resort - just call ToString() on unknown type
					Return = rawValue.ToString();
			}
			return Return;
		}
		
		public static void BuildFontElement (Font font, XmlElement fontElement) {

			System.ComponentModel.TypeConverter converter =
				System.ComponentModel.TypeDescriptor.GetConverter( typeof(Font));
			string fontString = converter.ConvertToInvariantString(font);
			
			XmlAttribute att = fontElement.OwnerDocument.CreateAttribute ("value");
			att.InnerText = fontString;
			fontElement.Attributes.Append(att);
		}
		
		
		public static Font MakeFont(string font) {
			System.ComponentModel.TypeConverter converter =
				System.ComponentModel.TypeDescriptor.GetConverter( typeof(Font));
			return (Font)converter.ConvertFromInvariantString(font);
		}
		/// <summary>
		/// Sets a property called propertyName in object <code>o</code> to <code>val</code>. This method performs
		/// all neccessary casts.
		/// </summary>
		public void SetValue(object o, string propertyName, string value)
		{
			try {
				PropertyInfo propertyInfo = o.GetType().GetProperty(propertyName);

				if (value.StartsWith("{") && value.EndsWith("}")) {
					value = value.Substring(1, value.Length - 2);
					object propertyObject = null;
	
					if (propertyInfo.CanWrite) {
						Type type = propertyInfo.GetType();				
						propertyObject = propertyInfo.GetValue(o, null);					
					} 
					else {
						propertyObject = propertyInfo.GetValue(o, null);
					}
					Match match = propertySet.Match(value);
					while (true) {
						if (!match.Success) {
							break;
						}
						SetValue(propertyObject, match.Result("${Property}"), match.Result("${Value}"));
						match = match.NextMatch();
					}
					
					if (propertyInfo.CanWrite) {
						propertyInfo.SetValue(o, propertyObject, null);
					}
				} else if (propertyInfo.PropertyType.IsEnum) {
					propertyInfo.SetValue(o, Enum.Parse(propertyInfo.PropertyType, value), null);
					
				} else if (propertyInfo.PropertyType == typeof(Color)) {
					string color = value.Substring(value.IndexOf('[') + 1).Replace("]", "");
					string[] argb = color.Split(',', '=');
					if (argb.Length > 1) {		
						CultureInfo ci = CultureInfo.CurrentCulture;
						propertyInfo.SetValue(o, Color.FromArgb(Int32.Parse(argb[1],ci), Int32.Parse(argb[3],ci), Int32.Parse(argb[5],ci), Int32.Parse(argb[7],ci)), null);
					} else {
						propertyInfo.SetValue(o, Color.FromName(color), null);
					}
				} else if (propertyInfo.PropertyType == typeof(Font)) {
					Font fnt = XmlFormReader.MakeFont(value);
					
					if (fnt != null) {
						propertyInfo.SetValue(o,fnt,null);
					} else {
						propertyInfo.SetValue(o, SystemInformation.MenuFont, null);
					}
				} 
				else {
					if (value.Length > 0) {
						if (propertyInfo.CanWrite) {
							propertyInfo.SetValue(o,
							                      Convert.ChangeType(value,
							                                            propertyInfo.PropertyType,
							                                           CultureInfo.CurrentCulture),
							                      null);
						}
					}
				}
			} catch (Exception) {
			}
		}
	}
}
