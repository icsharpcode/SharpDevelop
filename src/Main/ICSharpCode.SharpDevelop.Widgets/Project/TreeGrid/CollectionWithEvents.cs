// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace ICSharpCode.SharpDevelop.Widgets.TreeGrid
{
	public class CollectionItemEventArgs<T> : EventArgs
	{
		T item;
		
		public T Item {
			get {
				return item;
			}
		}
		
		public CollectionItemEventArgs(T item)
		{
			this.item = item;
		}
	}
	
	/// <summary>
	/// A collection that fires events when items are added or removed.
	/// </summary>
	public sealed class CollectionWithEvents<T> : IList<T>
	{
		List<T> list = new List<T>();
		public event EventHandler<CollectionItemEventArgs<T>> Added;
		public event EventHandler<CollectionItemEventArgs<T>> Removed;
		
		void OnAdded(T item)
		{
			if (Added != null)
				Added(this, new CollectionItemEventArgs<T>(item));
		}
		void OnRemoved(T item)
		{
			if (Removed != null)
				Removed(this, new CollectionItemEventArgs<T>(item));
		}
		
		public T this[int index] {
			get {
				return list[index];
			}
			set {
				T oldValue = list[index];
				if (!object.Equals(oldValue, value)) {
					list[index] = value;
					OnRemoved(oldValue);
					OnAdded(value);
				}
			}
		}
		
		public int Count {
			[DebuggerStepThrough]
			get {
				return list.Count;
			}
		}
		
		public bool IsReadOnly {
			get {
				return false;
			}
		}
		
		public int IndexOf(T item)
		{
			return list.IndexOf(item);
		}
		
		public void Insert(int index, T item)
		{
			list.Insert(index, item);
			OnAdded(item);
		}
		
		public void RemoveAt(int index)
		{
			T item = list[index];
			list.RemoveAt(index);
			OnRemoved(item);
		}
		
		public void Add(T item)
		{
			list.Add(item);
			OnAdded(item);
		}
		
		public void AddRange(IEnumerable<T> range)
		{
			foreach(T t in range) {
				Add(t);
			}
		}
		
		public void Clear()
		{
			List<T> oldList = list;
			list = new List<T>();
			foreach (T item in oldList) {
				OnRemoved(item);
			}
		}
		
		public bool Contains(T item)
		{
			return list.Contains(item);
		}
		
		public void CopyTo(T[] array, int arrayIndex)
		{
			list.CopyTo(array, arrayIndex);
		}
		
		public bool Remove(T item)
		{
			if (list.Remove(item)) {
				OnRemoved(item);
				return true;
			}
			return false;
		}
		
		[DebuggerStepThrough]
		public IEnumerator<T> GetEnumerator()
		{
			return list.GetEnumerator();
		}
		
		[DebuggerStepThrough]
		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return list.GetEnumerator();
		}
	}
}
