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
	static class Linq
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
	}
}
