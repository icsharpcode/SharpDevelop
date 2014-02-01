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
using System.Threading;
using ICSharpCode.NRefactory.Utils;

namespace ICSharpCode.SharpDevelop.Dom
{
	/// <summary>
	/// Synchronizing wrapper around IMutableModelCollection.
	/// </summary>
	public class SynchronizedModelCollection<T> : IMutableModelCollection<T>
	{
		readonly IMutableModelCollection<T> underlyingCollection;
		readonly object syncRoot;
		
		public SynchronizedModelCollection(IMutableModelCollection<T> underlyingCollection)
			: this(underlyingCollection, new object())
		{
		}
		
		public SynchronizedModelCollection(IMutableModelCollection<T> underlyingCollection, object syncRoot)
		{
			if (underlyingCollection == null)
				throw new ArgumentNullException("underlyingCollection");
			if (syncRoot == null)
				throw new ArgumentNullException("syncRoot");
			this.underlyingCollection = underlyingCollection;
			this.syncRoot = syncRoot;
		}
		
		public event ModelCollectionChangedEventHandler<T> CollectionChanged {
			add {
				lock (syncRoot) {
					underlyingCollection.CollectionChanged += value;
				}
			}
			remove {
				lock (syncRoot) {
					underlyingCollection.CollectionChanged -= value;
				}
			}
		}

		#region IMutableModelCollection implementation

		public void Clear()
		{
			lock (syncRoot) {
				underlyingCollection.Clear();
			}
		}

		public void Add(T item)
		{
			lock (syncRoot) {
				underlyingCollection.Add(item);
			}
		}

		public void AddRange(IEnumerable<T> items)
		{
			lock (syncRoot) {
				underlyingCollection.AddRange(items);
			}
		}

		public bool Remove(T item)
		{
			lock (syncRoot) {
				return underlyingCollection.Remove(item);
			}
		}

		public int RemoveAll(Predicate<T> predicate)
		{
			lock (syncRoot) {
				return underlyingCollection.RemoveAll(predicate);
			}
		}

		public IDisposable BatchUpdate()
		{
			Monitor.Enter(syncRoot);
			IDisposable disposable = underlyingCollection.BatchUpdate();
			return new CallbackOnDispose(
				delegate {
					try {
						if (disposable != null)
							disposable.Dispose();
					} finally {
						Monitor.Exit(syncRoot);
					}
				});
		}

		#endregion

		#region ICollection implementation
		public IReadOnlyCollection<T> CreateSnapshot()
		{
			lock (syncRoot) {
				return underlyingCollection.CreateSnapshot();
			}
		}
		
		public bool Contains(T item)
		{
			lock (syncRoot) {
				return underlyingCollection.Contains(item);
			}
		}

		public void CopyTo(T[] array, int arrayIndex)
		{
			lock (syncRoot) {
				underlyingCollection.CopyTo(array, arrayIndex);
			}
		}
		
		public int Count {
			get {
				lock (syncRoot) {
					return underlyingCollection.Count;
				}
			}
		}

		public bool IsReadOnly {
			get {
				lock (syncRoot) {
					return underlyingCollection.IsReadOnly;
				}
			}
		}

		public IEnumerator<T> GetEnumerator()
		{
			IEnumerable<T> snapshot;
			lock (syncRoot) {
				snapshot = underlyingCollection.CreateSnapshot();
			}
			return snapshot.GetEnumerator();
		}

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
		#endregion
	}
}
