using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;
using System.Collections;

namespace SharpDevelop.XamlDesigner
{
	public static class Utils
	{
		public static string DoubleToInvariantString(double d)
		{
			return d.ToString(NumberFormatInfo.InvariantInfo);
		}

		public static bool TryParseDouble(string s, out double result)
		{
			if (!double.TryParse(s, NumberStyles.Float | NumberStyles.AllowThousands,
				NumberFormatInfo.CurrentInfo, out result)) {

				if (!double.TryParse(s, NumberStyles.Float | NumberStyles.AllowThousands,
					NumberFormatInfo.InvariantInfo, out result)) {
					return false;
				}
			}
			return true;
		}

		public static bool CamelFilter(string name, string filter)
		{
			if (string.IsNullOrEmpty(filter)) {
				return true;
			}
			for (int i = 0; i < name.Length; i++) {
				if (i == 0 || char.IsUpper(name[i])) {
					if (string.Compare(name, i, filter, 0, filter.Length, true) == 0) {
						return true;
					}
				}
			}
			return false;
		}

		public static bool IsCollection(Type type)
		{
			return typeof(IList).IsAssignableFrom(type);
		}
	}
}
