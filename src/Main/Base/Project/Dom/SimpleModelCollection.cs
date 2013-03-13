// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)using System;

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
		}
		
		protected void CheckReentrancy()
		{
			if (isRaisingEvent)
				throw new InvalidOperationException("Cannot modify the collection from within the CollectionChanged event.");
		}
		
		protected virtual void ValidateItem(T item)
		{
		}
		
		#region CollectionChanged / BatchUpdate()
		public event ModelCollectionChangedEventHandler<T> CollectionChanged;
		
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
			var handler = CollectionChanged;
			if (handler != null)
				handler(removedItems, addedItems);
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
		
		public void Clear()
		{
			CheckReentrancy();
			addedItems = null;
			if (removedItems == null)
				removedItems = new List<T>();
			removedItems.AddRange(list);
			list.Clear();
			RaiseEventIfNotInBatch();
		}
		
		public void Add(T item)
		{
			CheckReentrancy();
			ValidateItem(item);
			if (removedItems != null)
				removedItems.Remove(item);
			if (addedItems == null)
				addedItems = new List<T>();
			addedItems.Add(item);
			list.Add(item);
			RaiseEventIfNotInBatch();
		}
		
		public void AddRange(IEnumerable<T> items)
		{
			if (items == null)
				throw new ArgumentNullException("items");
			CheckReentrancy();
			List<T> itemsList = items.ToList();
			for (int i = 0; i < itemsList.Count; i++) {
				ValidateItem(itemsList[i]);
			}
			if (removedItems != null) {
				for (int i = 0; i < itemsList.Count; i++) {
					removedItems.Remove(itemsList[i]);
				}
			}
			if (addedItems != null)
				addedItems.AddRange(itemsList);
			else
				addedItems = itemsList;
			list.AddRange(itemsList);
			RaiseEventIfNotInBatch();
		}
		
		public bool Remove(T item)
		{
			CheckReentrancy();
			if (list.Remove(item)) {
				if (addedItems != null)
					addedItems.Remove(item);
				if (removedItems == null)
					removedItems = new List<T>();
				removedItems.Add(item);
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
						if (addedItems != null)
							addedItems.Remove(obj);
						if (removedItems == null)
							removedItems = new List<T>();
						removedItems.Add(obj);
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

