// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ICSharpCode.WpfDesign.XamlDom
{
	/// <summary>
	/// Static class that can generate XAML markup extension code ("{Binding Path=...}").
	/// </summary>
	public static class MarkupExtensionPrinter
	{
		/// <summary>
		/// Gets whether shorthand XAML markup extension code can be generated for the object.
		/// </summary>
		public static bool CanPrint(XamlObject obj)
		{
			return true;
		}
		
		/// <summary>
		/// Generates XAML markup extension code for the object.
		/// </summary>
		public static string Print(XamlObject obj)
		{
			StringBuilder sb = new StringBuilder();
			sb.Append("{");
			sb.Append(obj.GetNameForMarkupExtension());

			bool first = true;
			foreach (var property in obj.Properties) {
				if (!property.IsSet) continue;

				if (first)
					sb.Append(" ");
				else
					sb.Append(", ");
				first = false;

				sb.Append(property.GetNameForMarkupExtension());
				sb.Append("=");

				var value = property.PropertyValue;
				if (value is XamlTextValue) {
					sb.Append((value as XamlTextValue).Text);
				} else if (value is XamlObject) {
					sb.Append(Print(value as XamlObject));
				}
			}
			sb.Append("}");
			return sb.ToString();
		}
	}
}
