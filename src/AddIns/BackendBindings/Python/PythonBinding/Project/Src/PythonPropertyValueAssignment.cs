// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;
using System.Reflection;
using System.Text;

namespace ICSharpCode.PythonBinding
{
	/// <summary>
	/// Represents a property value assignment in a form or control.
	/// </summary>
	public class PythonPropertyValueAssignment
	{
		PythonPropertyValueAssignment()
		{
		}
		
		/// <summary>
		/// Converts the property assignment to the equivalent python string.
		/// </summary>
		/// <remarks>
		/// 1) Strings are returned surrounded by double quotes.
		/// 2) Objects are returned with their full name (e.g. System.Windows.Forms.Size(100, 200)).
		/// 3) Enums are returned with their full name (e.g. System.Windows.Forms.AccessibleRole.None).
		/// 4) By default the ToString method is used on the property value.
		/// </remarks>
		public static string ToString(object propertyValue)
		{
			Type propertyType = propertyValue.GetType();
			if (propertyType == typeof(String)) {
				return GetQuotedString((string)propertyValue);
			} else if (propertyType == typeof(Size)) {
				Size size = (Size)propertyValue;
				return size.GetType().FullName + "(" + size.Width + ", " + size.Height + ")";
			} else if (propertyType == typeof(SizeF)) {
				SizeF size = (SizeF)propertyValue;
				return size.GetType().FullName + "(" + size.Width + ", " + size.Height + ")";				
			} else if (propertyType == typeof(Cursor)) {
				return GetCursorAsString(propertyValue as Cursor);
			} else if (propertyType == typeof(Point)) {
				Point point = (Point)propertyValue;
				return point.GetType().FullName + "(" + point.X + ", " + point.Y + ")";
			} else if (propertyType == typeof(Padding)) {
				Padding padding = (Padding)propertyValue;
				return padding.GetType().FullName + "(" + padding.Left + ", " + padding.Top + ", " + padding.Right + ", " + padding.Bottom + ")";
			} else if (propertyType == typeof(Color)) {
				Color color = (Color)propertyValue;
				return GetColorAsString(color);
			} else if (propertyType == typeof(Font)) {
				Font font = (Font)propertyValue;
				return GetFontAsString(font);
			} else if (propertyType == typeof(AnchorStyles)) {
				AnchorStyles anchor = (AnchorStyles)propertyValue;
				return GetAnchorStyleAsString(anchor);
			} else if (propertyType.IsEnum) {
				return propertyType.FullName.Replace('+', '.') + "." + propertyValue.ToString();
			}
			return propertyValue.ToString();
		}
		
		static string GetCursorAsString(Cursor cursor)
		{
			TypeConverter converter = TypeDescriptor.GetConverter(typeof(Cursor));
			string cursorName = converter.ConvertToString(null, CultureInfo.InvariantCulture, cursor);
			return typeof(Cursors).FullName + "." + cursorName;
		}
		
		static string GetColorAsString(Color color)
		{
			if (color.IsSystemColor) {
				return GetColorAsString(color, typeof(SystemColors));
			} else if (color.IsNamedColor) {
				return GetColorAsString(color, typeof(Color));
			} 
			
			// Custom color.
			return color.GetType().FullName + ".FromArgb(" + color.R + ", " + color.G + ", " + color.B + ")";
		}
		
		static string GetColorAsString(Color color, Type type)
		{
			foreach (PropertyInfo property in type.GetProperties(BindingFlags.Public | BindingFlags.Static)) {
				Color standardColor = (Color)property.GetValue(null, null);
				if (color == standardColor) {
					return type.FullName + "." + standardColor.Name;
				}
			}
			return String.Empty;
		}
		
		static string GetFontAsString(Font font)
		{
			return String.Concat(font.GetType().FullName, "(\"", font.Name, "\", ", font.Size.ToString(CultureInfo.InvariantCulture), ", ", typeof(FontStyle).FullName, ".", font.Style, ", ", typeof(GraphicsUnit).FullName, ".", font.Unit, ", ", font.GdiCharSet, ")");
		}
		
		static string GetAnchorStyleAsString(AnchorStyles anchorStyles)
		{
			if (anchorStyles == AnchorStyles.None) {
				return typeof(AnchorStyles).FullName + "." + AnchorStyles.None;
			}
			
			StringBuilder text = new StringBuilder();
			bool firstStyle = true;
			foreach (AnchorStyles style in AnchorStyles.GetValues(typeof(AnchorStyles))) {
				if (style != AnchorStyles.None) {
					if ((anchorStyles & style) == style) {
						if (firstStyle) {
							firstStyle = false;
						} else {
							text.Append(" | ");
						}
						text.Append(typeof(AnchorStyles).FullName);
						text.Append('.');
						text.Append(style);
					}
				}
			}
			return text.ToString();
		}
		
		/// <summary>
		/// Returns the string enclosed by double quotes.
		/// Escapes any double quotes or backslashes in the string itself.
		/// </summary>
		static string GetQuotedString(string text)
		{
			return "\"" + text.Replace(@"\", @"\\").Replace("\"", "\\\"") + "\"";
		}
	}
}
