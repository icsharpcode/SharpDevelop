// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;

namespace ICSharpCode.WpfDesign
{
	// Static helpers that should become extension methods in the future
	static class Func
	{
		public static T[] ToArray<T>(ICollection<T> collection) where T : class
		{
			T[] arr = new T[collection.Count];
			collection.CopyTo(arr, 0);
			return arr;
		}
		
		/// <summary>
		/// Outputs distinct elements only, filtering all duplicates.
		/// </summary>
		public static IEnumerable<T> Distinct<T>(IEnumerable<T> input) where T : class
		{
			// store elements already seen
			HashSet<T> elements = new HashSet<T>();
			
			foreach (T element in input) {
				if (elements.Add(element)) {
					yield return element;
				}
			}
		}
		
		/// <summary>
		/// Returns the first element from <paramref name="input"/>.
		/// </summary>
		public static T First<T>(IEnumerable<T> input)
		{
			if (input == null)
				throw new ArgumentNullException("input");
			foreach (T item in input) {
				return item;
			}
			throw new ArgumentException("input must not be an empty collection", "input");
		}
		
		/// <summary>
		/// Skips the first <paramref name="skipCount"/> items in input and returns the rest.
		/// </summary>
		public static IEnumerable<T> Skip<T>(IEnumerable<T> input, long skipCount)
		{
			if (input == null)
				throw new ArgumentNullException("input");
			if (skipCount < 0)
				throw new ArgumentOutOfRangeException("skipCount", skipCount, "skipCount must be non-negative.");
			using (IEnumerator<T> enumerator = input.GetEnumerator()) {
				if (skipCount != 0) {
					long i = 0;
					while (enumerator.MoveNext()) {
						// skip item
						if (++i == skipCount)
							break;
					}
					if (i != skipCount) yield break; // MoveNext returned false, don't call it again
				}
				while (enumerator.MoveNext()) {
					yield return enumerator.Current;
				}
			}
		}
	}
}
