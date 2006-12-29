// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.Collections.Specialized;

namespace ICSharpCode.WpfDesign
{
	/// <summary>
	/// Represents a set of items. The set does not preserve the order of items and does not allow items to
	/// be added twice.
	/// It supports collection change notifications and is cloned by sharing the underlying
	/// data structure and delaying the actual copy until the next change.
	/// </summary>
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix")]
	public sealed class HashSet<T> : ICollection<T>, ICollection, ICloneable, INotifyCollectionChanged
		where T : class
	{
		Dictionary<T, object> _dict;
		bool _copyOnWrite;
		
		/// <summary>
		/// This event is raised whenever the collection changes.
		/// </summary>
		public event NotifyCollectionChangedEventHandler CollectionChanged;
		
		/// <summary>
		/// Creates a new, empty set.
		/// </summary>
		public HashSet()
		{
			_dict = new Dictionary<T, object>();
		}
		
		/// <summary>
		/// Creates a copy of the existing set.
		/// </summary>
		public HashSet(HashSet<T> existingSet)
		{
			existingSet._copyOnWrite = true;
			this._copyOnWrite = true;
			_dict = existingSet._dict;
		}
		
		/// <summary>
		/// Adds the item to the set.
		/// Trying to add <c>null</c> will return false without changing the collection.
		/// </summary>
		/// <returns>True when the item was added, false when it was not added because it already is in the set</returns>
		public bool Add(T item)
		{
			if (item == null)
				return false;
			if (_dict.ContainsKey(item)) {
				return false;
			} else {
				CopyIfRequired();
				_dict.Add(item, null);
				if (CollectionChanged != null) {
					CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item));
				}
				return true;
			}
		}
		
		/// <summary>
		/// Adds a list of items to the set. This is equivalent to calling <see cref="Add"/> for each item in <paramref name="items"/>.
		/// </summary>
		public void AddRange(IEnumerable<T> items)
		{
			foreach (T item in items) {
				Add(item);
			}
		}

		private void CopyIfRequired()
		{
			if (_copyOnWrite) {
				_copyOnWrite = false;
				_dict = new Dictionary<T, object>(_dict);
			}
		}

		/// <summary>
		/// Removes all items from the set.
		/// </summary>
		public void Clear()
		{
			_dict.Clear();
		}

		/// <summary>
		/// Tests if this set contains the specified item.
		/// Checking for <c>null</c> always returns false.
		/// </summary>
		public bool Contains(T item)
		{
			if (item == null)
				return false;
			else
				return _dict.ContainsKey(item);
		}

		/// <summary>
		/// Gets the number of items in the collection.
		/// </summary>
		public int Count
		{
			get { return _dict.Count; }
		}

		/// <summary>
		/// Removes an item from the set.
		/// Trying to remove <c>null</c> will return false without changing the collection.
		/// </summary>
		public bool Remove(T item)
		{
			if (item == null)
				return false;
			CopyIfRequired();
			if (_dict.Remove(item)) {
				if (CollectionChanged != null) {
					CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, item));
				}
				return true;
			} else {
				return false;
			}
		}

		/// <summary>
		/// Copy all items to the specified array.
		/// </summary>
		void ICollection<T>.CopyTo(T[] array, int arrayIndex)
		{
			_dict.Keys.CopyTo(array, arrayIndex);
		}

		void ICollection<T>.Add(T item)
		{
			this.Add(item);
		}

		bool ICollection<T>.IsReadOnly
		{
			get { return false; }
		}

		#region IEnumerable Members
		/// <summary>
		/// Gets an enumerator to enumerate the items in the set.
		/// </summary>
		public IEnumerator<T> GetEnumerator()
		{
			return _dict.Keys.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return _dict.Keys.GetEnumerator();
		}
		#endregion

		#region ICollection Members

		void ICollection.CopyTo(Array array, int index)
		{
			((ICollection)_dict).CopyTo(array, index);
		}

		bool ICollection.IsSynchronized
		{
			get { return false; }
		}

		object ICollection.SyncRoot
		{
			get { return null; }
		}

		#endregion

		#region ICloneable Members

		/// <summary>
		/// Create a copy of this set.
		/// </summary>
		public HashSet<T> Clone()
		{
			return new HashSet<T>(this);
		}

		object ICloneable.Clone()
		{
			return this.Clone();
		}

		#endregion
	}
}
