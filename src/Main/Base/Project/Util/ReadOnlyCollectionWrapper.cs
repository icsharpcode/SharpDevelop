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

namespace ICSharpCode.SharpDevelop
{
	/// <summary>
	/// Wraps any collection to make it read-only.
	/// </summary>
	[Obsolete("This class seems to be unused now; all uses I've seen have been replaced with IReadOnlyList<T>")]
	public sealed class ReadOnlyCollectionWrapper<T> : ICollection<T>, IReadOnlyCollection<T>
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
