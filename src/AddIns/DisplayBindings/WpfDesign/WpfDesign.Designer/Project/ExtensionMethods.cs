// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace ICSharpCode.WpfDesign.Designer
{
	public static class ExtensionMethods
	{
		public static double Coerce(this double value, double min, double max)
		{
			return Math.Max(Math.Min(value, max), min);
		}

		public static void AddRange<T>(this ICollection<T> col, IEnumerable<T> items)
		{
			foreach (var item in items) {
				col.Add(item);
			}
		}

		static bool IsVisual(DependencyObject d)
		{
			return d is Visual || d is Visual3D;
		}
		
		/// <summary>
		/// Gets all ancestors in the visual tree (including <paramref name="visual"/> itself).
		/// Returns an empty list if <paramref name="visual"/> is null or not a visual.
		/// </summary>
		public static IEnumerable<DependencyObject> GetVisualAncestors(this DependencyObject visual)
		{
			if (IsVisual(visual)) {
				while (visual != null) {
					yield return visual;
					visual = VisualTreeHelper.GetParent(visual);
				}
			}
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
				e.Handled = true;
			};
			element.CommandBindings.Add(cb);
		}
	}
}
