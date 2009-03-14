// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Drawing;
using System.Windows.Forms;
using System.Reflection;

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
				return "\"" + propertyValue + "\"";
			} else if (propertyType == typeof(Size)) {
				Size size = (Size)propertyValue;
				return size.GetType().FullName + "(" + size.Width + ", " + size.Height + ")";
			} else if (propertyType.IsEnum) {
				return propertyType.FullName + "." + propertyValue.ToString();
			} else if (propertyType == typeof(Cursor)) {
				return GetCursorToString(propertyValue as Cursor);
			} else if (propertyType == typeof(Point)) {
				Point point = (Point)propertyValue;
				return point.GetType().FullName + "(" + point.X + ", " + point.Y + ")";
			}
			return propertyValue.ToString();
		}
		
		static string GetCursorToString(Cursor cursor)
		{
			foreach (PropertyInfo propertyInfo in typeof(Cursors).GetProperties(BindingFlags.Public | BindingFlags.Static)) {
				Cursor standardCursor = (Cursor)propertyInfo.GetValue(null, null);
				if (standardCursor == cursor) {
					return typeof(Cursors).FullName + "." + propertyInfo.Name;
				}
			}
			return String.Empty;
		}
	}
}
