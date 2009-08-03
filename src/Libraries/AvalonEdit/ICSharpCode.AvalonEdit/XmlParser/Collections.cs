// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;

// Missing XML comment
#pragma warning disable 1591

namespace ICSharpCode.AvalonEdit.XmlParser
{
	/// <summary>
	/// Collection that is publicly read-only and has support 
	/// for adding/removing multiple items at a time.
	/// </summary>
	public class ChildrenCollection<T>: Collection<T>, INotifyCollectionChanged
	{
		public event NotifyCollectionChangedEventHandler CollectionChanged;
		
		protected virtual void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
		{
			if (CollectionChanged != null) {
				CollectionChanged(this, e);
			}
		}
		
		protected override void ClearItems()
		{
			throw new NotSupportedException();
		}
		
		protected override void InsertItem(int index, T item)
		{
			throw new NotSupportedException();
		}
		
		protected override void RemoveItem(int index)
		{
			throw new NotSupportedException();
		}
		
		protected override void SetItem(int index, T item)
		{
			throw new NotSupportedException();
		}
		
		internal void InsertItems(int index, IList<T> items)
		{
			for(int i = 0; i < items.Count; i++) {
				base.InsertItem(index + i, items[i]);
			}
			OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, (IList)items, index));
		}
		
		internal void RemoveItems(int index, int count)
		{
			List<T> removed = new List<T>();
			for(int i = 0; i < count; i++) {
				removed.Add(this[index]);
				base.RemoveItem(index);
			}
			OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, (IList)removed, index));
		}
	}
	
	/// <summary>
	/// Collection that presents only some items from the wrapped collection
	/// </summary>
	public class FilteredCollection<C, T>: ObservableCollection<T> where C: INotifyCollectionChanged, IList<T>
	{
		C source;
		Predicate<T> condition;
		List<int> srcPtrs = new List<int>(); // Index to the original collection
		
		public FilteredCollection(C source, Predicate<T> condition)
		{
			this.source = source;
			this.condition = condition;
			
			this.source.CollectionChanged += SourceCollectionChanged;
			
			Reset();
		}
		
		void Reset()
		{
			this.Clear();
			srcPtrs.Clear();
			for(int i = 0; i < source.Count; i++) {
				if (condition(source[i])) {
					this.Add(source[i]);
					srcPtrs.Add(i);
				}
			}
		}

		void SourceCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			switch(e.Action) {
				case NotifyCollectionChangedAction.Add:
					// Update pointers
					for(int i = 0; i < srcPtrs.Count; i++) {
						if (srcPtrs[i] >= e.NewStartingIndex) {
							srcPtrs[i] += e.NewItems.Count;
						}
					}
					// Find where to add items
					int addIndex = srcPtrs.FindIndex(srcPtr => srcPtr >= e.NewStartingIndex);
					if (addIndex == -1) addIndex = this.Count;
					// Add items to collection
					for(int i = 0; i < e.NewItems.Count; i++) {
						if (condition((T)e.NewItems[i])) {
							this.InsertItem(addIndex, (T)e.NewItems[i]);
							srcPtrs.Insert(addIndex, e.NewStartingIndex + i);
							addIndex++;
						}
					}
					break;
				case NotifyCollectionChangedAction.Remove:
					// Remove the item from our collection
					for(int i = 0; i < e.OldItems.Count; i++) {
						// Anyone points to the removed item?
						int removeIndex = srcPtrs.IndexOf(e.OldStartingIndex + i);
						// Remove
						if (removeIndex != -1) {
							this.RemoveAt(removeIndex);
							srcPtrs.RemoveAt(removeIndex);
						}
					}
					// Update pointers
					for(int i = 0; i < srcPtrs.Count; i++) {
						if (srcPtrs[i] >= e.OldStartingIndex) {
							srcPtrs[i] -= e.OldItems.Count;
						}
					}
					break;
				case NotifyCollectionChangedAction.Reset:
					Reset();
					break;
				default:
					throw new NotSupportedException(e.Action.ToString());
			}
		}
	}
	
	/// <summary>
	/// Two collections in sequence
	/// </summary>
	public class MergedCollection<C, T>: ObservableCollection<T> where C: INotifyCollectionChanged, IList<T>
	{
		C a;
		C b;
		
		public MergedCollection(C a, C b)
		{
			this.a = a;
			this.b = b;
			
			this.a.CollectionChanged += SourceCollectionAChanged;
			this.b.CollectionChanged += SourceCollectionBChanged;
			
			Reset();
		}
		
		void Reset()
		{
			this.Clear();
			foreach(T item in a) this.Add(item);
			foreach(T item in b) this.Add(item);
		}

		void SourceCollectionAChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			SourceCollectionChanged(0, e);
		}
		
		void SourceCollectionBChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			SourceCollectionChanged(a.Count, e);
		}
		
		void SourceCollectionChanged(int collectionStart, NotifyCollectionChangedEventArgs e)
		{
			switch(e.Action) {
				case NotifyCollectionChangedAction.Add:
					for (int i = 0; i < e.NewItems.Count; i++) {
						this.InsertItem(collectionStart + e.NewStartingIndex + i, (T)e.NewItems[i]);
					}
					break;
				case NotifyCollectionChangedAction.Remove:
					for (int i = 0; i < e.OldItems.Count; i++) {
						this.RemoveAt(collectionStart + e.OldStartingIndex);
					}
					break;
				case NotifyCollectionChangedAction.Reset:
					Reset();
					break;
				default:
					throw new NotSupportedException(e.Action.ToString());
			}
		}
	}
}
