using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;
using System.Windows;
using System.Windows.Input;
using System.Globalization;
using System.ComponentModel;

namespace SharpDevelop.XamlDesigner
{
	public static class ExtensionMethods
	{
		public static T FindAncestor<T>(this DependencyObject d) where T : class
		{
			return AncestorsAndSelf(d).OfType<T>().FirstOrDefault();
		}

		public static IEnumerable<DependencyObject> AncestorsAndSelf(this DependencyObject d)
		{
			while (d != null) {
				yield return d;
				d = VisualTreeHelper.GetParent(d);
			}
		}

		public static T FindChild<T>(this DependencyObject d) where T : class
		{
			if (d is T) return d as T;
			int n = VisualTreeHelper.GetChildrenCount(d);
			for (int i = 0; i < n; i++) {
				var child = VisualTreeHelper.GetChild(d, i);
				var result = FindChild<T>(child);
				if (result != null) return result;
			}
			return null;
		}

		public static double Coerce(this double d, double min, double max)
		{
			return Math.Max(Math.Min(d, max), min);
		}

		public static void AddCommandHandler(this UIElement element, ICommand command, Action execute)
		{
			AddCommandHandler(element, command, execute, null);
		}

		public static void AddCommandHandler(this UIElement element, ICommand command, Action execute, Func<bool> canExecute)
		{
			var cb = new CommandBinding(command);
			if (canExecute != null) {
				cb.CanExecute += delegate(object sender, CanExecuteRoutedEventArgs e) {
					e.CanExecute = canExecute();
					e.Handled = true;
				};
			}
			cb.Executed += delegate(object sender, ExecutedRoutedEventArgs e) {
				execute();
			};
			element.CommandBindings.Add(cb);
		}

		public static void AddRange<T>(this ICollection<T> col, IEnumerable<T> items)
		{
			foreach (var item in items) {
				col.Add(item);
			}
		}

		public static string ToInvariantString(this double d)
		{
			return d.ToString(CultureInfo.InvariantCulture.NumberFormat);
		}

		public static T[] AsArray<T>(this T obj)
		{
			return new[] { obj };
		}

		public static T[] AsArrayOrDefault<T>(this T obj)
		{
			if (obj == null) {
				return null;
			}
			return new[] { obj };
		}

		public static object GetDataContext(this RoutedEventArgs e)
		{
			var f = e.OriginalSource as FrameworkElement;
			if (f != null) {
				return f.DataContext;
			}
			return null;
		}

		public static IEnumerable<string> Paths(this IDataObject data)
		{
			string[] paths = (string[])data.GetData(DataFormats.FileDrop);
			if (paths != null) {
				foreach (var path in paths) {
					yield return path;
				}
			}
		}

		public static Type GetComponentType(this MemberDescriptor descriptor)
		{
			if (descriptor is PropertyDescriptor) {
				return (descriptor as PropertyDescriptor).ComponentType;
			}
			return (descriptor as EventDescriptor).ComponentType;
		}

		public static IEnumerable<Type> GetChain(this Type type)
		{
			while (type != null) {
				yield return type;
				type = type.BaseType;
			}
		}
	}
}
