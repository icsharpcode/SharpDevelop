// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;

namespace ICSharpCode.SharpDevelop.Dom
{
	/// <summary>
	/// A model collection that filters an input collection.
	/// </summary>
	public sealed class FilterModelCollection<T> : IModelCollection<T>
	{
		readonly IModelCollection<T> input;
		readonly Func<T, bool> predicate;
		bool isAttached;
		NotifyCollectionChangedEventHandler collectionChanged;
		
		public FilterModelCollection(IModelCollection<T> input, Func<T, bool> predicate)
		{
			if (input == null)
				throw new ArgumentNullException("input");
			if (predicate == null)
				throw new ArgumentNullException("predicate");
			this.input = input;
			this.predicate = predicate;
		}
		
		public event NotifyCollectionChangedEventHandler CollectionChanged {
			add {
				collectionChanged += value;
				if (collectionChanged != null && !isAttached) {
					input.CollectionChanged += input_CollectionChanged;
					isAttached = true;
				}
			}
			remove {
				collectionChanged -= value;
				if (collectionChanged == null && isAttached) {
					input.CollectionChanged -= input_CollectionChanged;
					isAttached = false;
				}
			}
		}

		void input_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			switch (e.Action) {
				case NotifyCollectionChangedAction.Add:
					collectionChanged(this, new NotifyCollectionChangedEventArgs(
						NotifyCollectionChangedAction.Add, ApplyFilter(e.NewItems)));
					break;
				case NotifyCollectionChangedAction.Remove:
					collectionChanged(this, new NotifyCollectionChangedEventArgs(
						NotifyCollectionChangedAction.Remove, ApplyFilter(e.OldItems)));
					break;
				case NotifyCollectionChangedAction.Replace:
					collectionChanged(this, new NotifyCollectionChangedEventArgs(
						NotifyCollectionChangedAction.Replace, ApplyFilter(e.OldItems), ApplyFilter(e.NewItems)));
					break;
				case NotifyCollectionChangedAction.Move:
					// this collection is unordered
					break;
				case NotifyCollectionChangedAction.Reset:
					collectionChanged(this, new NotifyCollectionChangedEventArgs(
						NotifyCollectionChangedAction.Reset));
					break;
				default:
					throw new NotSupportedException();
			}
		}
		
		IList ApplyFilter(IList inputItems)
		{
			if (inputItems == null)
				return null;
			List<T> outputItems = new List<T>();
			foreach (T item in inputItems) {
				if (predicate(item))
					outputItems.Add(item);
			}
			return outputItems;
		}
		
		public int Count {
			get {
				return input.Count(predicate);
			}
		}
		
		public IEnumerator<T> GetEnumerator()
		{
			return input.Where(predicate).GetEnumerator();
		}
		
		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
	}
}
