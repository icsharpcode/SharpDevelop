// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Forms;

namespace ICSharpCode.SharpDevelop
{
	/// <summary>
	/// Extension methods used in SharpDevelop.
	/// </summary>
	public static class ExtensionMethods
	{
		/// <summary>
		/// Runs an action for all elements in the input.
		/// </summary>
		public static void Foreach<T>(this IEnumerable<T> input, Action<T> action)
		{
			if (input == null)
				throw new ArgumentNullException("input");
			foreach (T element in input) {
				action(element);
			}
		}
		
		public static ReadOnlyCollection<T> AsReadOnly<T>(this T[] arr)
		{
			return Array.AsReadOnly(arr);
		}
		
		public static string Join(this IEnumerable<string> input, string separator)
		{
			return string.Join(separator, input.ToArray());
		}
		
		public static IEnumerable<Control> GetRecursive(this Control.ControlCollection collection)
		{
			foreach (Control ctl in collection) {
				yield return ctl;
				foreach (Control subCtl in ctl.Controls.GetRecursive()) {
					yield return subCtl;
				}
			}
		}
	}
}
