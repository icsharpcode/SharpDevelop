// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections;
using System.Collections.Generic;

namespace ICSharpCode.SharpDevelop
{
	/// <summary>
	/// Wraps any collection to make it read-only.
	/// </summary>
	public sealed class ReadOnlyCollectionWrapper<T> : ICollection<T>
	{
		readonly ICollection<T> c;
		
		public ReadOnlyCollectionWrapper(ICollection<T> c)
		{
			if (c == null)
				throw new ArgumentNullException("c");
			this.c = c;
		}
		
		public int Count {
			get {
				return c.Count;
			}
		}
		
		public bool IsReadOnly {
			get {
				return true;
			}
		}
		
		void ICollection<T>.Add(T item)
		{
			throw new NotSupportedException();
		}
		
		void ICollection<T>.Clear()
		{
			throw new NotSupportedException();
		}
		
		public bool Contains(T item)
		{
			return c.Contains(item);
		}
		
		public void CopyTo(T[] array, int arrayIndex)
		{
			c.CopyTo(array, arrayIndex);
		}
		
		bool ICollection<T>.Remove(T item)
		{
			throw new NotSupportedException();
		}
		
		public IEnumerator<T> GetEnumerator()
		{
			return c.GetEnumerator();
		}
		
		IEnumerator IEnumerable.GetEnumerator()
		{
			return ((IEnumerable)c).GetEnumerator();
		}
	}
}
