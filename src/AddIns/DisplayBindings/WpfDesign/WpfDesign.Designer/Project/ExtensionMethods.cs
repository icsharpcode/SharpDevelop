using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Collections;
using System.Windows;

namespace ICSharpCode.WpfDesign.Designer
{
	public static class ExtensionMethods
	{
		public static double Coerce(this double d, double min, double max)
		{
			return Math.Max(Math.Min(d, max), min);
		}

		public static void AddRange<T>(this ICollection<T> col, IEnumerable<T> items)
		{
			foreach (var item in items) {
				col.Add(item);
			}
		}

		public static object GetDataContext(this RoutedEventArgs e)
		{
			var f = e.OriginalSource as FrameworkElement;
			if (f != null) {
				return f.DataContext;;
			}
			return null;
		}
	}
}
