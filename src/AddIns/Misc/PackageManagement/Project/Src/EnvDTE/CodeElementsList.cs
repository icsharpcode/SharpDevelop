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
