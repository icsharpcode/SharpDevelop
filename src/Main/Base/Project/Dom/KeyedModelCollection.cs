// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace ICSharpCode.SharpDevelop.Dom
{
	/// <summary>
	/// Observable KeyedCollection.
	/// </summary>
	public abstract class KeyedModelCollection<TKey, TItem> : KeyedCollection<TKey, TItem>, IModelCollection<TItem>
	{
		// TODO: do we still need this class? maybe we should remove it?
		
		public bool TryGetValue(TKey key, out TItem item)
		{
			return Dictionary.TryGetValue(key, out item);
		}
		
		protected override void ClearItems()
		{
			base.ClearItems();
			if (CollectionChanged != null)
				CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
		}
		
		protected override void InsertItem(int index, TItem item)
		{
			base.InsertItem(index, item);
			if (CollectionChanged != null)
				CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item, index));
		}
		
		protected override void RemoveItem(int index)
		{
			var oldItem = Items[index];
			base.RemoveItem(index);
			if (CollectionChanged != null)
				CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, oldItem, index));
		}
		
		protected override void SetItem(int index, TItem item)
		{
			var oldItem = Items[index];
			base.SetItem(index, item);
			if (CollectionChanged != null)
				CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, oldItem, index));
		}
		
		public event NotifyCollectionChangedEventHandler CollectionChanged;
	}
}
