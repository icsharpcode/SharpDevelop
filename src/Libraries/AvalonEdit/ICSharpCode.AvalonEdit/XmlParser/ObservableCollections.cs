// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;

// Missing XML comment
#pragma warning disable 1591

namespace ICSharpCode.AvalonEdit.XmlParser
{
	public class FilteredObservableCollection<T>: ObservableCollection<T>
	{
		ObservableCollection<T> source;
		Predicate<T> condition;
		List<int> srcPtrs = new List<int>();
		
		public FilteredObservableCollection(ObservableCollection<T> source, Predicate<T> condition)
		{
			this.source = source;
			this.condition = condition;
			
			for(int i = 0; i < source.Count; i++) {
				if (condition(source[i])) {
					int index = srcPtrs.Count;
					this.InsertItem(index, source[i]);
					srcPtrs.Insert(index, i);
				}
			}
			
			this.source.CollectionChanged += new NotifyCollectionChangedEventHandler(FilteredObservableCollection_CollectionChanged);
		}

		void FilteredObservableCollection_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			if (e.Action == NotifyCollectionChangedAction.Remove) {
				// Remove the item from our collection
				if (condition((T)e.OldItems[0])) {
					int index = srcPtrs.IndexOf(e.OldStartingIndex);
					this.RemoveAt(index);
					srcPtrs.RemoveAt(index);
				}
				// Update pointers
				for(int i = 0; i < srcPtrs.Count; i++) {
					if (srcPtrs[i] > e.OldStartingIndex) {
						srcPtrs[i]--;
					}
				}
			}
			if (e.Action == NotifyCollectionChangedAction.Add) {
				// Update pointers
				for(int i = 0; i < srcPtrs.Count; i++) {
					if (srcPtrs[i] >= e.NewStartingIndex) {
						srcPtrs[i]++;
					}
				}
				// Add item to collection
				if (condition((T)e.NewItems[0])) {
					int index = srcPtrs.FindIndex(srcPtr => srcPtr >= e.NewStartingIndex);
					if (index == -1) index = srcPtrs.Count;
					this.InsertItem(index, (T)e.NewItems[0]);
					srcPtrs.Insert(index, e.NewStartingIndex);
				}
			}
		}
	}
	
	public class MergedObservableCollection<T>: ObservableCollection<T>
	{
		ObservableCollection<T> a;
		ObservableCollection<T> b;
		
		public MergedObservableCollection(ObservableCollection<T> a, ObservableCollection<T> b)
		{
			this.a = a;
			this.b = b;
			
			foreach(T item in a) this.Add(item);
			foreach(T item in b) this.Add(item);
			
			this.a.CollectionChanged += new NotifyCollectionChangedEventHandler(MergedObservableCollection_CollectionAChanged);
			this.b.CollectionChanged += new NotifyCollectionChangedEventHandler(MergedObservableCollection_CollectionBChanged);
		}

		void MergedObservableCollection_CollectionAChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			if (e.Action == NotifyCollectionChangedAction.Remove) {
				this.RemoveAt(e.OldStartingIndex);
			}
			if (e.Action == NotifyCollectionChangedAction.Add) {
				this.InsertItem(e.NewStartingIndex, (T)e.NewItems[0]);
			}
		}
		
		void MergedObservableCollection_CollectionBChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			if (e.Action == NotifyCollectionChangedAction.Remove) {
				this.RemoveAt(e.OldStartingIndex + a.Count);
			}
			if (e.Action == NotifyCollectionChangedAction.Add) {
				this.InsertItem(e.NewStartingIndex + a.Count, (T)e.NewItems[0]);
			}
		}
	}
}
