// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

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

		public DependencyRelation(IMultiDictionary<T1, T2> innerDep)
		{
			dep = innerDep;

			revDep = new MultiDictionary<T2, T1>();
			foreach (KeyValuePair<T1, T2> item in dep)
				revDep.Add(item.Value, item.Key);
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
		public IList<T2> DependOn(T1 element)
		{
			if (dep.ContainsKey(element))
				return dep[element];
			return new List<T2>();
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
		public IList<T1> DependingOn(T2 element)
		{
			if (revDep.ContainsKey(element))
				return revDep[element];
			return new List<T1>();
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

		IEnumerable<T> DependencyTree<T, U>(IMultiDictionary<T, U> rel, T startItem)
		{
			HashSet<T> visited = new HashSet<T>();
			Stack<T> pendingItems = new Stack<T>();

			pendingItems.Push(startItem);
			while (pendingItems.Count > 0)
			{
				T currentItem = pendingItems.Pop();
				if (rel.ContainsKey(currentItem))
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
		IMultiDictionary<T1, T2> dep;

		//reverse dependency relation
		IMultiDictionary<T2, T1> revDep;
	}
}
