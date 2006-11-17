// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections;
using System.Collections.Generic;

namespace ICSharpCode.SharpDevelop
{
	/// <summary>
	/// Set container class. Contains a sorted list of unique objects.
	/// When adding an object that is already in the set, it is not added again.
	/// Add, Remove and Contains are O(n log n)-operations.
	/// </summary>
	public sealed class Set<T> : ICollection<T>
	{
		SortedDictionary<T, object> dict;
		
		#region Constructors
		public Set()
		{
			dict = new SortedDictionary<T, object>();
		}
		
		public Set(IEnumerable<T> list)
			: this()
		{
			AddRange(list);
		}
		
		public Set(params T[] list)
			: this()
		{
			AddRange(list);
		}
		
		public Set(IComparer<T> comparer)
		{
			dict = new SortedDictionary<T, object>(comparer);
		}
		
		public Set(IEnumerable<T> list, IComparer<T> comparer)
			: this(comparer)
		{
			AddRange(list);
		}
		#endregion
		
		public void Add(T element)
		{
			dict[element] = null;
		}
		
		public void AddRange(IEnumerable<T> elements)
		{
			foreach (T element in elements) {
				Add(element);
			}
		}
		
		public bool Contains(T element)
		{
			return dict.ContainsKey(element);
		}
		
		public bool Remove(T element)
		{
			return dict.Remove(element);
		}
		
		public IEnumerator<T> GetEnumerator()
		{
			return dict.Keys.GetEnumerator();
		}
		
		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}
		
		public int Count {
			get {
				return dict.Count;
			}
		}
		
		bool ICollection<T>.IsReadOnly {
			get {
				return true;
			}
		}
		
		public void Clear()
		{
			dict.Clear();
		}
		
		public void CopyTo(T[] array, int arrayIndex)
		{
			dict.Keys.CopyTo(array, arrayIndex);
		}
		
		public T[] ToArray()
		{
			T[] arr = new T[dict.Count];
			dict.Keys.CopyTo(arr, 0);
			return arr;
		}
		
		public ReadOnlyCollectionWrapper<T> AsReadOnly()
		{
			return new ReadOnlyCollectionWrapper<T>(dict.Keys);
		}
	}
}
