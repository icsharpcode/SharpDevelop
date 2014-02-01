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
