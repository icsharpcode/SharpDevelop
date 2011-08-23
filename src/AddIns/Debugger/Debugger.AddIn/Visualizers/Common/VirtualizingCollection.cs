// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the BSD license (for details please see \src\AddIns\Debugger\Debugger.AddIn\license.txt)

using System;
using System.Collections;
using System.Collections.Generic;

namespace Debugger.AddIn.Visualizers
{
	/// <summary>
	/// IList&lt;T&gt; with data vitualization - the indexer is lazy, uses IListValuesProvider to obtain values when needed.
	/// </summary>
	public class VirtualizingCollection<T> : IList<T>, IList
	{
		private IListValuesProvider<T> valueProvider;
		private Dictionary<int, T> itemCache = new Dictionary<int, T>();

		public VirtualizingCollection(IListValuesProvider<T> valueProvider)
		{
			this.valueProvider = valueProvider;
		}

		public int Count
		{
			get { return this.valueProvider.GetCount(); }
		}

		public T this[int index]
		{
			get
			{
				T cachedItem;
				if (!itemCache.TryGetValue(index, out cachedItem))
				{
					cachedItem = this.valueProvider.GetItemAt(index);
					this.itemCache[index] = cachedItem;
				}
				return cachedItem;
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		#region IList<T> Members

		public int IndexOf(T item)
		{
			return -1;
		}

		public void Insert(int index, T item)
		{
			throw new NotImplementedException();
		}

		public void RemoveAt(int index)
		{
			throw new NotImplementedException();
		}

		#endregion

		#region ICollection<T> Members

		public void Add(T item)
		{
			throw new NotImplementedException();
		}

		public void Clear()
		{
			throw new NotImplementedException();
		}

		public bool Contains(T item)
		{
			throw new NotImplementedException();
		}

		public void CopyTo(T[] array, int arrayIndex)
		{
			throw new NotImplementedException();
		}

		public bool IsReadOnly
		{
			get { return true; }
		}

		public bool Remove(T item)
		{
			throw new NotImplementedException();
		}

		#endregion

		#region IEnumerable<T> Members

		/// <summary>
		/// Should be avoided on large collections due to performance.
		/// </summary>
		/// <returns></returns>
		public IEnumerator<T> GetEnumerator()
		{
			for (int i = 0; i < this.Count; i++)
			{
				yield return this[i];
			}
		}

		#endregion

		#region IEnumerable Members

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		#endregion

		#region IList Members

		public int Add(object value)
		{
			throw new NotImplementedException();
		}

		public bool Contains(object value)
		{
			throw new NotImplementedException();
		}

		public int IndexOf(object value)
		{
			return IndexOf((T)value);
		}

		public void Insert(int index, object value)
		{
			throw new NotImplementedException();
		}

		public bool IsFixedSize
		{
			get { return false; }
		}

		public void Remove(object value)
		{
			throw new NotImplementedException();
		}

		object IList.this[int index]
		{
			get
			{
				return this[index];
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		#endregion

		#region ICollection Members

		public void CopyTo(Array array, int index)
		{
			throw new NotImplementedException();
		}

		public bool IsSynchronized
		{
			get { return false; }
		}

		public object SyncRoot
		{
			get { return this; }
		}

		#endregion
	}
}
