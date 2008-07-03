using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace ICSharpCode.WpfDesign.Designer.Controls.TypeEditors.BrushEditor
{
	public static class ExtensionMethods
	{
		//public static T[] GetValues<T>(this Type type)
		//{
		//    return type
		//        .GetProperties(BindingFlags.Static | BindingFlags.Public)
		//        .Select(p => p.GetValue(null, null)).OfType<T>().ToArray();
		//}

		public static double Coerce(this double d, double min, double max)
		{
			return Math.Max(Math.Min(d, max), min);
		}
	}
}
