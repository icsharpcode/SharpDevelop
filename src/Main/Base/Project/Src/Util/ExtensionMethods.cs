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
using System.Text;
using System.Windows.Forms;

namespace ICSharpCode.SharpDevelop
{
	/// <summary>
	/// Extension methods used in SharpDevelop.
	/// </summary>
	public static class ExtensionMethods
	{
		/// <summary>
		/// Raises the event.
		/// Does nothing if eventHandler is null.
		/// Because the event handler is passed as parameter, it is only fetched from the event field one time.
		/// This makes
		/// <code>MyEvent.RaiseEvent(x,y);</code>
		/// thread-safe
		/// whereas
		/// <code>if (MyEvent != null) MyEvent(x,y);</code>
		/// would not be safe.
		/// </summary>
		public static void RaiseEvent(this EventHandler eventHandler, object sender, EventArgs e)
		{
			if (eventHandler != null) {
				eventHandler(sender, e);
			}
		}
		
		/// <summary>
		/// Raises the event.
		/// Does nothing if eventHandler is null.
		/// Because the event handler is passed as parameter, it is only fetched from the event field one time.
		/// This makes
		/// <code>MyEvent.RaiseEvent(x,y);</code>
		/// thread-safe
		/// whereas
		/// <code>if (MyEvent != null) MyEvent(x,y);</code>
		/// would not be safe.
		/// </summary>
		public static void RaiseEvent<T>(this EventHandler<T> eventHandler, object sender, T e) where T : EventArgs
		{
			if (eventHandler != null) {
				eventHandler(sender, e);
			}
		}
		
		/// <summary>
		/// Runs an action for all elements in the input.
		/// </summary>
		public static void ForEach<T>(this IEnumerable<T> input, Action<T> action)
		{
			if (input == null)
				throw new ArgumentNullException("input");
			foreach (T element in input) {
				action(element);
			}
		}
		
		[Obsolete("Use ForEach instead.")]
		public static void Foreach<T>(this IEnumerable<T> input, Action<T> action)
		{
			ForEach(input, action);
		}
		
		/// <summary>
		/// Adds all <paramref name="elements"/> to <paramref name="list"/>.
		/// </summary>
		public static void AddRange<T>(this ICollection<T> list, IEnumerable<T> elements)
		{
			foreach (T o in elements)
				list.Add(o);
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
		
		public static string Replace(this string original, string pattern, string replacement, StringComparison comparisonType)
		{
			if (original == null)
				throw new ArgumentNullException("original");
			if (pattern == null)
				throw new ArgumentNullException("pattern");
			if (pattern.Length == 0)
				throw new ArgumentException("String cannot be of zero length.", "pattern");
			if (comparisonType != StringComparison.Ordinal && comparisonType != StringComparison.OrdinalIgnoreCase)
				throw new NotSupportedException("Currently only ordinal comparisons are implemented.");
			
			StringBuilder result = new StringBuilder(original.Length);
			int currentPos = 0;
			int nextMatch = original.IndexOf(pattern, comparisonType);
			while (nextMatch >= 0) {
				result.Append(original, currentPos, nextMatch - currentPos);
				// The following line restricts this method to ordinal comparisons:
				// for non-ordinal comparisons, the match length might be different than the pattern length.
				currentPos = nextMatch + pattern.Length;
				result.Append(replacement);
				
				nextMatch = original.IndexOf(pattern, currentPos, comparisonType);
			}
			
			result.Append(original, currentPos, original.Length - currentPos);
			return result.ToString();
		}
		
		public static byte[] GetBytesWithPreamble(this Encoding encoding, string text)
		{
			byte[] encodedText = encoding.GetBytes(text);
			byte[] bom = encoding.GetPreamble();
			if (bom != null && bom.Length > 0) {
				byte[] result = new byte[bom.Length + encodedText.Length];
				bom.CopyTo(result, 0);
				encodedText.CopyTo(result, bom.Length);
				return result;
			} else {
				return encodedText;
			}
		}
	}
}
