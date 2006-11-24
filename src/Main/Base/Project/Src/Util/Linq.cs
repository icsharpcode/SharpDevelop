// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections;
using System.Collections.Generic;

namespace ICSharpCode.SharpDevelop
{
	/// <summary>
	/// A set of methods that replicate some of the LINQ functionality.
	/// </summary>
	/// <remarks>
	/// Will be removed when SharpDevelop is compiled with C# 3.0.
	/// </remarks>
	public static class Linq
	{
		/// <summary>
		/// Applies a conversion function to all elements in the input.
		/// </summary>
		public static IEnumerable<S> Select<T, S>(IEnumerable<T> input, Converter<T, S> converter)
		{
			foreach (T element in input) {
				yield return converter(element);
			}
		}
		
		/// <summary>
		/// Returns only the elements in input for which filter is true.
		/// </summary>
		public static IEnumerable<T> Where<T>(IEnumerable<T> input, Predicate<T> filter)
		{
			foreach (T element in input) {
				if (filter(element))
					yield return element;
			}
		}
		
		/// <summary>
		/// Returns the elements of type T inside input by running
		/// "if (element is T) yield return (T)element;" on each element.
		/// </summary>
		public static IEnumerable<T> OfType<T>(IEnumerable input)
		{
			foreach (object element in input) {
				if (element is T)
					yield return (T)element;
			}
		}
		
		/// <summary>
		/// Casts a non-generic enumeration into a generic enumeration.
		/// </summary>
		public static IEnumerable<T> CastTo<T>(IEnumerable input)
		{
			foreach (object element in input) {
				yield return (T)element;
			}
		}
		
		/// <summary>
		/// Returns the first element in input for which filter is true.
		/// Returns default(T) if no element matches the filter.
		/// </summary>
		public static T Find<T>(IEnumerable<T> input, Predicate<T> filter)
		{
			foreach (T element in input) {
				if (filter(element))
					return element;
			}
			return default(T);
		}
		
		public static List<T> ToList<T>(IEnumerable<T> input)
		{
			return new List<T>(input);
		}
		
		public static T[] ToArray<T>(IEnumerable<T> input)
		{
			if (input is ICollection<T>) {
				ICollection<T> c = (ICollection<T>)input;
				T[] arr = new T[c.Count];
				c.CopyTo(arr, 0);
				return arr;
			} else {
				return new List<T>(input).ToArray();
			}
		}
		
		public static int Count<T>(IEnumerable<T> input)
		{
			if (input is ICollection<T>) {
				return ((ICollection<T>)input).Count;
			}
			int count = 0;
			using (IEnumerator<T> e = input.GetEnumerator()) {
				while (e.MoveNext())
					count++;
			}
			return count;
		}
	}
}
