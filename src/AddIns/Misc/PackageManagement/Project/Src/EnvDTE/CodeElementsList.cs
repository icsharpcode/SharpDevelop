// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ICSharpCode.SharpDevelop.Dom;

namespace ICSharpCode.PackageManagement.EnvDTE
{
	public class CodeElementsList<T> : MarshalByRefObject, global::EnvDTE.CodeElements, IList<T>
		where T : global::EnvDTE.CodeElement
	{
		List<T> elements = new List<T>();
		
		public CodeElementsList()
		{
		}
		
		#region EnvDTE.CodeElements implementation
		public int Count {
			get { return elements.Count; }
		}
		
		public IEnumerator GetEnumerator()
		{
			return elements.GetEnumerator();
		}
		
		global::EnvDTE.CodeElement global::EnvDTE.CodeElements.Item(object index)
		{
			if (index is int) {
				return GetItem((int)index);
			}
			return GetItem((string)index);
		}
		
		global::EnvDTE.CodeElement GetItem(int index)
		{
			return elements[index - 1];
		}
		
		global::EnvDTE.CodeElement GetItem(string name)
		{
			return elements.Single(item => item.Name == name);
		}
		#endregion
		
		#region IList<T>
		public T this[int index] {
			get { return elements[index]; }
			set { elements[index] = value; }
		}
		
		bool ICollection<T>.IsReadOnly {
			get { return false; }
		}
		
		public int IndexOf(T item)
		{
			return elements.IndexOf(item);
		}
		
		public void Insert(int index, T item)
		{
			elements.Insert(index, item);
		}
		
		public void RemoveAt(int index)
		{
			elements.RemoveAt(index);
		}
		
		public void Add(T item)
		{
			elements.Add(item);
		}
		
		public void Clear()
		{
			elements.Clear();
		}
		
		public bool Contains(T item)
		{
			return elements.Contains(item);
		}
		
		public void CopyTo(T[] array, int arrayIndex)
		{
			elements.CopyTo(array, arrayIndex);
		}
		
		public bool Remove(T item)
		{
			return elements.Remove(item);
		}
		
		IEnumerator<T> IEnumerable<T>.GetEnumerator()
		{
			return elements.GetEnumerator();
		}
		#endregion
	}
}
