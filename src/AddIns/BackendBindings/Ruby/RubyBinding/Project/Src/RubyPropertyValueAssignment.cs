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
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.ComponentModel.Design.Serialization;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;
using System.Reflection;
using System.Text;

namespace ICSharpCode.RubyBinding
{
	/// <summary>
	/// Represents a property value assignment in a form or control.
	/// </summary>
	public class RubyPropertyValueAssignment
	{
		RubyPropertyValueAssignment()
		{
		}
		
		/// <summary>
		/// Converts the property assignment to the equivalent Ruby string.
		/// </summary>
		/// <remarks>
		/// 1) Strings are returned surrounded by double quotes.
		/// 2) Characters are returned surrounded by double quotes.
		/// 3) Objects are returned with their full name (e.g. System::Windows::Forms::Size.new(100, 200)).
		/// 4) Enums are returned with their full name (e.g. System::Windows::Forms::AccessibleRole.None).
		/// 5) By default the ToString method is used on the property value.
		/// </remarks>
		public static string ToString(object propertyValue)
		{
			if (propertyValue == null) {
				return "nil";
			}
			
			Type propertyType = propertyValue.GetType();
			if (propertyType == typeof(bool)) {
				return ConvertTypeToString(propertyValue).ToLowerInvariant();
			} else if (propertyType == typeof(String)) {
				return GetQuotedString((string)propertyValue);
			} else if (propertyType == typeof(Char)) {
				return GetQuotedString(propertyValue.ToString());
			} else if (propertyType == typeof(AnchorStyles)) {
				AnchorStyles anchor = (AnchorStyles)propertyValue;
				return GetAnchorStyleAsString(anchor);
			}
			return ConvertTypeToString(propertyValue);
		}
								
		static string GetAnchorStyleAsString(AnchorStyles anchorStyles)
		{
			if (anchorStyles == AnchorStyles.None) {
				return typeof(AnchorStyles).FullName.Replace(".", "::") + "." + AnchorStyles.None;
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
						text.Append(typeof(AnchorStyles).FullName.Replace(".", "::"));
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
		/// If the string is multline triple quotes used.
		/// </summary>
		static string GetQuotedString(string text)
		{
			string quotes = "\"";
			if (text.Contains("\n")) {
				quotes = "\"\"\"";
			}
			return quotes + text.Replace(@"\", @"\\").Replace("\"", "\\\"") + quotes;
		}
		
		/// <summary>
		/// Looks for an instance descriptor for the property value's type and then tries to convert
		/// the object creation using this instance descriptor.
		/// </summary>
		static string ConvertTypeToString(object propertyValue)
		{
			TypeConverter converter = TypeDescriptor.GetConverter(propertyValue);
			if (converter.CanConvertTo(typeof(InstanceDescriptor))) {
				InstanceDescriptor instanceDescriptor = converter.ConvertTo(propertyValue, typeof(InstanceDescriptor)) as InstanceDescriptor;
				if (instanceDescriptor != null) {
					StringBuilder text = new StringBuilder();
					MemberInfo memberInfo = instanceDescriptor.MemberInfo;
					string fullName = memberInfo.DeclaringType.FullName.Replace('+', '.'); // Remove any + chars from enums.
					fullName = fullName.Replace(".", "::");
					text.Append(fullName);
					if (memberInfo.MemberType == MemberTypes.Constructor) {
						text.Append(".new");
					} else {
						text.Append('.');
						text.Append(memberInfo.Name);
					}
					
					// Append arguments.
					AppendArguments(text, instanceDescriptor.Arguments);
					return text.ToString();
				}
			}
			return converter.ConvertToString(null, CultureInfo.InvariantCulture, propertyValue);
		}
		
		static void AppendArguments(StringBuilder text, ICollection args)
		{
			if (args.Count > 0) {
				int i = 0;
				text.Append('(');
				foreach (object arg in args) {
					if (i > 0) {
						text.Append(", ");
					}
					text.Append(ToString(arg));
					++i;
				}			
				text.Append(')');
			}
		}
	}
}
