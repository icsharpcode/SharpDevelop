using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ICSharpCode.WpfDesign.XamlDom
{
	public static class MarkupExtensionPrinter
	{
		public static bool CanPrint(XamlObject obj)
		{
			return true;
		}

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
