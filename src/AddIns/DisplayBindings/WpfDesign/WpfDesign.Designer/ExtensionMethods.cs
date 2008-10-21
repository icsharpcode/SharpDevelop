using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Collections;
using System.Windows;
using System.Windows.Media;
using System.Windows.Input;

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
				return f.DataContext;
			}
			return null;
		}

		public static T FindAncestor<T>(this DependencyObject d, string name) where T : class
		{
			while (true) {
				if (d == null) return null;
				if (d is T && d is FrameworkElement && (d as FrameworkElement).Name == name) return d as T;
				d = VisualTreeHelper.GetParent(d);
			}
		}

		public static T FindAncestor<T>(this DependencyObject d) where T : class
		{
			while (true) {
				if (d == null) return null;
				if (d is T) return d as T;
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
	}
}
