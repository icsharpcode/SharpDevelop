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
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

using ICSharpCode.NRefactory.Utils;
using ICSharpCode.SharpDevelop;

namespace ICSharpCode.CppBinding.Project
{
	/// <summary>
	/// Dependency relation between arbitrary types.
	/// </summary>
	public class DependencyRelation<T1, T2>
	{
		public DependencyRelation()
			: this(new MultiDictionary<T1, T2>())
		{ }

		public DependencyRelation(MultiDictionary<T1, T2> innerDep)
		{
			dep = innerDep;

			revDep = new MultiDictionary<T2, T1>();
			foreach (var group in dep)
				foreach (var value in group)
					revDep.Add(value, group.Key);
		}

		/// <summary>
		/// Adds an information that <paramref name="item"/> does depend on <paramref name="dependency"/>
		/// </summary>
		public void Add(T1 item, T2 dependency)
		{
			dep.Add(item, dependency);
			revDep.Add(dependency, item);
		}

		/// <summary>
		/// Removes a dependency between <paramref name="item"/> and <paramref name="dependency"/>.
		/// </summary>
		public bool Remove(T1 item, T2 dependency)
		{
			if (dep.Remove(item, dependency))
			{
				Debug.Assert(revDep.Remove(dependency, item));
				return true;
			}
			return false;
		}

		/// <summary>
		/// Returns a list of items given element directly depend on.
		/// If there is no element in the dependency relation, returns an empty list.
		/// </summary>
		public IReadOnlyList<T2> DependOn(T1 element)
		{
			return dep[element];
		}

		/// <summary>
		/// Enumerates all items given element depend on including those that
		/// depend indirectly. If type parameters <typeparamref name="T1"/> and <typeparamref name="T2"/>
		/// are not equal then the result is the same as from DependOn method.
		/// </summary>
		/// <remarks>
		/// The enumeration may contain the <paramref name="element"/> if there is 
		/// a cyclic dependency. It is guaranteed that the enumaration contains an item
		/// at most once.
		/// </remarks>
		public IEnumerable<T2> DependencyTreeFrom(T1 element)
		{
			if (typeof(T1) != typeof(T2)) return DependOn(element);

			//this solves the case when T1 == T2
			return DependencyTree(dep, element).Cast<T2>();
		}

		/// <summary>
		/// Returns a list of elements that depend on given element.
		/// If the is no item that depend on given element, returns an empty list.
		/// </summary>
		/// <returns></returns>
		public IReadOnlyList<T1> DependingOn(T2 element)
		{
			return revDep[element];
		}

		/// <summary>
		/// Enumerates all items that are depending on given element including those 
		/// depending indirectly. If type parameters <typeparamref name="T1"/> and <typeparamref name="T2"/>
		/// are not equal then the result is the same as from DependingOn method.
		/// </summary>
		/// <remarks>
		/// The enumeration may contain the <paramref name="element"/> if there is 
		/// a cyclic dependency. It is guaranteed that the enumaration contains an item
		/// at most once.
		/// </remarks>
		public IEnumerable<T1> DependencyTreeTo(T2 element)
		{
			if (typeof(T1) != typeof(T2)) return DependingOn(element);

			//this solves the case when T1 == T2
			return DependencyTree(revDep, element).Cast<T1>();
		}

		IEnumerable<T> DependencyTree<T, U>(MultiDictionary<T, U> rel, T startItem)
		{
			HashSet<T> visited = new HashSet<T>();
			Stack<T> pendingItems = new Stack<T>();

			pendingItems.Push(startItem);
			while (pendingItems.Count > 0)
			{
				T currentItem = pendingItems.Pop();
				foreach (T depend in rel[currentItem].Cast<T>())
					if (!visited.Contains(depend))
					{
						visited.Add(depend);
						pendingItems.Push(depend);
						yield return depend;
					}
			}
		}

		//dependency relation
		MultiDictionary<T1, T2> dep;

		//reverse dependency relation
		MultiDictionary<T2, T1> revDep;
	}
}
