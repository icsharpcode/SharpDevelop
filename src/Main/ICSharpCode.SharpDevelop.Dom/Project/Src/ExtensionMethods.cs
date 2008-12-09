// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <author name="Daniel Grunwald"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace ICSharpCode.SharpDevelop.Dom
{
	/// <summary>
	/// Description of ExtensionMethods.
	/// </summary>
	static class ExtensionMethods
	{
		public static void AddRange(this ArrayList arrayList, IEnumerable elements)
		{
			foreach (object o in elements)
				arrayList.Add(o);
		}
		
		public static void AddRange<T>(this ICollection<T> list, IEnumerable<T> elements)
		{
			foreach (T o in elements)
				list.Add(o);
		}
		
		public static IEnumerable<T> Flatten<T>(this IEnumerable<T> input, Func<T, IEnumerable<T>> recursion)
		{
			Stack<IEnumerator<T>> stack = new Stack<IEnumerator<T>>();
			try {
				stack.Push(input.GetEnumerator());
				while (stack.Count > 0) {
					while (stack.Peek().MoveNext()) {
						T element = stack.Peek().Current;
						yield return element;
						IEnumerable<T> children = recursion(element);
						if (children != null) {
							stack.Push(children.GetEnumerator());
						}
					}
					stack.Pop();
				}
			} finally {
				while (stack.Count > 0) {
					stack.Pop().Dispose();
				}
			}
		}
		
		public static IEnumerable<IUsing> GetAllUsings(this ICompilationUnit cu)
		{
			return (new[]{cu.UsingScope}).Flatten(s=>s.ChildScopes).SelectMany(s=>s.Usings);
		}
	}
}
