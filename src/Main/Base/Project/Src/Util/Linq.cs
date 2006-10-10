// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;
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
	}
}
