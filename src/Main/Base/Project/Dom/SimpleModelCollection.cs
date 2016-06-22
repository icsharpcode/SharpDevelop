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
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using ICSharpCode.NRefactory;
using ICSharpCode.NRefactory.Utils;

namespace ICSharpCode.SharpDevelop.Dom
{
	/// <summary>
	/// A model collection implementation.
	/// </summary>
	public class SimpleModelCollection<T> : IMutableModelCollection<T>
	{
		readonly ModelCollectionChangedEvent<T> collectionChangedEvent = new ModelCollectionChangedEvent<T>();
		readonly List<T> list;
		List<T> addedItems;
		List<T> removedItems;
		
		public SimpleModelCollection()
		{
			this.list = new List<T>();
		}
		
		public SimpleModelCollection(IEnumerable<T> items)
		{
			this.list = new List<T>(items);
			// Note: intentionally not using ValidateItem(), as calling a virtual method
			// from a constructor is problematic
		}
		
		protected void CheckReentrancy()
		{
			if (isRaisingEvent)
				throw new InvalidOperationException("Cannot modify the collection from within the CollectionChanged event.");
		}
		
		/// <summary>
		/// Called before an item
		/// </summary>
		protected virtual void ValidateItem(T item)
		{
		}
		
		#region CollectionChanged / BatchUpdate()
		public event ModelCollectionChangedEventHandler<T> CollectionChanged
		{
			add {
				collectionChangedEvent.AddHandler(value);
			}
			remove {
				collectionChangedEvent.RemoveHandler(value);
			}
		}
		
		bool isWithinBatchOperation;
		bool isRaisingEvent;
		
		void RaiseEventIfNotInBatch()
		{
			if (isWithinBatchOperation)
				return;
			IReadOnlyCollection<T> removed = this.removedItems;
			IReadOnlyCollection<T> added = this.addedItems;
			this.removedItems = null;
			this.addedItems = null;
			this.isRaisingEvent = true;
			try {
				OnCollectionChanged(removed ?? EmptyList<T>.Instance, added ?? EmptyList<T>.Instance);
			} finally {
				this.isRaisingEvent = false;
			}
		}
		
		protected virtual void OnCollectionChanged(IReadOnlyCollection<T> removedItems, IReadOnlyCollection<T> addedItems)
		{
			collectionChangedEvent.Fire(removedItems, addedItems);
		}
		
		public virtual IDisposable BatchUpdate()
		{
			if (isWithinBatchOperation)
				return null;
			isWithinBatchOperation = true;
			return new CallbackOnDispose(
				delegate {
					isWithinBatchOperation = false;
					if (removedItems != null || addedItems != null)
						RaiseEventIfNotInBatch();
				});
		}
		#endregion
		
		#region Read-Only list access
		
		public IReadOnlyCollection<T> CreateSnapshot()
		{
			return list.ToArray();
		}
		
		public int Count {
			get { return list.Count; }
		}
		
		public bool Contains(T item)
		{
			return list.Contains(item);
		}

		public void CopyTo(T[] array, int arrayIndex)
		{
			list.CopyTo(array, arrayIndex);
		}

		bool ICollection<T>.IsReadOnly {
			get { return false; }
		}

		public IEnumerator<T> GetEnumerator()
		{
			return list.GetEnumerator();
		}

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return list.GetEnumerator();
		}

		#endregion
		
		#region IMutableModelCollection implementation
		
		/// <summary>
		/// Called immediately when an item is removed; even within a batch.
		/// The collection may be in an invalid state while this method is called.
		/// </summary>
		protected virtual void OnRemove(T item)
		{
			if (addedItems != null && addedItems.Remove(item))
				return;
			if (removedItems == null)
				removedItems = new List<T>();
			removedItems.Add(item);
		}
		
		/// <summary>
		/// Called immediately when an item is added; even within a batch.
		/// The collection may be in an invalid state while this method is called.
		/// </summary>
		protected virtual void OnAdd(T item)
		{
			if (removedItems != null && removedItems.Remove(item))
				return;
			if (addedItems == null)
				addedItems = new List<T>();
			addedItems.Add(item);
		}
		
		public void Clear()
		{
			CheckReentrancy();
			addedItems = null;
			for (int i = 0; i < list.Count; i++) {
				OnRemove(list[i]);
			}
			list.Clear();
			RaiseEventIfNotInBatch();
		}
		
		public void Add(T item)
		{
			CheckReentrancy();
			ValidateItem(item);
			OnAdd(item);
			list.Add(item);
			RaiseEventIfNotInBatch();
		}
		
		public void AddRange(IEnumerable<T> items)
		{
			if (items == null)
				throw new ArgumentNullException("items");
			CheckReentrancy();
			try {
				foreach (T item in items) {
					// Add each item before validating the next,
					// this is necessary because ValidateItem() might be checking
					// for duplicates (e.g. KeyedModelCollection<,>)
					ValidateItem(item);
					OnAdd(item);
					list.Add(item);
				}
			} finally {
				// In case validation fails, we still need to raise the event
				// for the items that were added successfully.
				RaiseEventIfNotInBatch();
			}
		}
		
		public bool Remove(T item)
		{
			CheckReentrancy();
			if (list.Remove(item)) {
				OnRemove(item);
				RaiseEventIfNotInBatch();
				return true;
			} else {
				return false;
			}
		}
		
		public int RemoveAll(Predicate<T> predicate)
		{
			CheckReentrancy();
			int count = list.RemoveAll(
				delegate(T obj) {
					if (predicate(obj)) {
						OnRemove(obj);
						return true;
					} else {
						return false;
					}
				});
			if (count > 0)
				RaiseEventIfNotInBatch();
			return count;
		}

		#endregion
	}
	
	/// <summary>
	/// A model collection implementation that disallows null values.
	/// </summary>
	public class NullSafeSimpleModelCollection<T> : SimpleModelCollection<T> where T : class
	{
		protected override void ValidateItem(T item)
		{
			if (item == null)
				throw new ArgumentNullException("item");
			base.ValidateItem(item);
		}
	}
}
