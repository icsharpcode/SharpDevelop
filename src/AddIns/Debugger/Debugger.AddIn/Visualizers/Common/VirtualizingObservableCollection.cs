// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the BSD license (for details please see \src\AddIns\Debugger\Debugger.AddIn\license.txt)

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Debugger.AddIn.Visualizers.Common
{
	/// <summary>
	/// Wrapper of ObservableCollection&lt;IEvaluate&gt; 
	/// with lazy indexer, suitable for Controls that use indexer to query for data as needed (eg. ListView).
	/// </summary>
	public class VirtualizingObservableCollection<T> : ObservableCollection<T>, IList<T>, IList
	{
		ObservableCollection<T> lazifiedCollection;

		public VirtualizingObservableCollection(ObservableCollection<T> lazifiedCollection)
		{
			this.lazifiedCollection = lazifiedCollection;
			this.lazifiedCollection.CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(lazifiedCollection_CollectionChanged);
		}

		void lazifiedCollection_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
		{
			OnCollectionChanged(e);
		}

		public new int Count
		{
			get { return this.lazifiedCollection.Count; }
		}

		/// <summary>
		/// Lazy indexer.
		/// </summary>
		public new T this[int index]
		{
			get
			{
				var underlyingItem = this.lazifiedCollection[index];
				IEvaluate itemLazy = underlyingItem as IEvaluate;
				if (itemLazy != null)
				{
					if (!itemLazy.IsEvaluated)
					{
						itemLazy.Evaluate();
					}
				}
				// return item, evaluated if it was IEvaluate
				return underlyingItem;
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		#region IList<T> Members

		public new int IndexOf(T item)
		{
			return -1;
		}

		public new void Insert(int index, T item)
		{
			throw new NotImplementedException();
		}

		public new void RemoveAt(int index)
		{
			throw new NotImplementedException();
		}

		#endregion

		#region ICollection<T> Members

		public new void Add(T item)
		{
			throw new NotImplementedException();
		}

		public new void Clear()
		{
			throw new NotImplementedException();
		}

		public new bool Contains(T item)
		{
			throw new NotImplementedException();
		}

		public new void CopyTo(T[] array, int arrayIndex)
		{
			throw new NotImplementedException();
		}

		public bool IsReadOnly
		{
			get { return true; }
		}

		public new bool Remove(T item)
		{
			throw new NotImplementedException();
		}

		#endregion

		#region IEnumerable<T> Members

		/// <summary>
		/// Should be avoided on large collections due to performance.
		/// </summary>
		/// <returns></returns>
		public new IEnumerator<T> GetEnumerator()
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
