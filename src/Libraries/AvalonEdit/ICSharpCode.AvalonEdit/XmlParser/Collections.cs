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

namespace ICSharpCode.AvalonEdit.XmlParser
{
	/// <summary>
	/// Collection that is publicly read-only and has support 
	/// for adding/removing multiple items at a time.
	/// </summary>
	public class ChildrenCollection<T>: Collection<T>, INotifyCollectionChanged
	{
		/// <summary> Occurs when the collection is changed </summary>
		public event NotifyCollectionChangedEventHandler CollectionChanged;
		
		/// <summary> Raises <see cref="CollectionChanged"/> event </summary>
		// Do not inherit - it is not called if event is null
		void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
		{
			if (CollectionChanged != null) {
				CollectionChanged(this, e);
			}
		}
		
		/// <inheritdoc/>
		protected override void ClearItems()
		{
			throw new NotSupportedException();
		}
		
		/// <inheritdoc/>
		protected override void InsertItem(int index, T item)
		{
			throw new NotSupportedException();
		}
		
		/// <inheritdoc/>
		protected override void RemoveItem(int index)
		{
			throw new NotSupportedException();
		}
		
		/// <inheritdoc/>
		protected override void SetItem(int index, T item)
		{
			throw new NotSupportedException();
		}
		
		internal void InsertItemAt(int index, T item)
		{
			base.InsertItem(index, item);
			if (CollectionChanged != null)
				OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, new T[] { item }.ToList(), index));
		}
		
		internal void RemoveItemAt(int index)
		{
			T removed = this[index];
			base.RemoveItem(index);
			if (CollectionChanged != null)
				OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, new T[] { removed }.ToList(), index));
		}
		
		internal void InsertItemsAt(int index, IList<T> items)
		{
			for(int i = 0; i < items.Count; i++) {
				base.InsertItem(index + i, items[i]);
			}
			if (CollectionChanged != null)
				OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, (IList)items, index));
		}
		
		internal void RemoveItemsAt(int index, int count)
		{
			List<T> removed = new List<T>();
			for(int i = 0; i < count; i++) {
				removed.Add(this[index]);
				base.RemoveItem(index);
			}
			if (CollectionChanged != null)
				OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, (IList)removed, index));
		}
	}
	
	/// <summary>
	/// Specailized attribute collection with attribute name caching
	/// </summary>
	public class AttributeCollection: FilteredCollection<RawAttribute, ChildrenCollection<RawObject>>
	{
		/// <summary> Wrap the given collection.  Non-attributes are filtered </summary>
		public AttributeCollection(ChildrenCollection<RawObject> source): base(source) {}
		
		/// <summary> Wrap the given collection.  Non-attributes are filtered.  Items not matching the condition are filtered. </summary>
		public AttributeCollection(ChildrenCollection<RawObject> source, Predicate<object> condition): base(source, condition) {}
		
		Dictionary<string, List<RawAttribute>> hashtable = new Dictionary<string, List<RawAttribute>>();
		
		void AddToHashtable(RawAttribute attr)
		{
			string localName = attr.LocalName;
			if (!hashtable.ContainsKey(localName)) {
				hashtable[localName] = new List<RawAttribute>(1);
			}
			hashtable[localName].Add(attr);
		}
		
		void RemoveFromHashtable(RawAttribute attr)
		{
			string localName = attr.LocalName;
			hashtable[localName].Remove(attr);
		}
		
		static List<RawAttribute> NoAttributes = new List<RawAttribute>();
		
		/// <summary>
		/// Get all attributes with given local name.
		/// Hash table is used for lookup so this is cheap.
		/// </summary>
		public IEnumerable<RawAttribute> GetByLocalName(string localName)
		{
			if (hashtable.ContainsKey(localName)) {
				return hashtable[localName];
			} else {
				return NoAttributes;
			}
		}
		
		/// <inheritdoc/>
		protected override void ClearItems()
		{
			foreach(RawAttribute item in this) {
				RemoveFromHashtable(item);
				item.Changing -= item_Changing;
				item.Changed  -= item_Changed;
			}
			base.ClearItems();
		}
		
		/// <inheritdoc/>
		protected override void InsertItem(int index, RawAttribute item)
		{
			AddToHashtable(item);
			item.Changing += item_Changing;
			item.Changed  += item_Changed;
			base.InsertItem(index, item);
		}
		
		/// <inheritdoc/>
		protected override void RemoveItem(int index)
		{
			RemoveFromHashtable(this[index]);
			this[index].Changing -= item_Changing;
			this[index].Changed  -= item_Changed;
			base.RemoveItem(index);
		}
		
		/// <inheritdoc/>
		protected override void SetItem(int index, RawAttribute item)
		{
			throw new NotSupportedException();
		}
		
		// Every item in the collectoin should be registered to these handlers
		// so that we can handle renames
		
		void item_Changing(object sender, RawObjectEventArgs e)
		{
			RemoveFromHashtable((RawAttribute)e.Object);
		}
		
		void item_Changed(object sender, RawObjectEventArgs e)
		{
			AddToHashtable((RawAttribute)e.Object);
		}
	}
	
	/// <summary>
	/// Collection that presents only some items from the wrapped collection.
	/// It implicitely filters object that are not of type T (or derived).
	/// </summary>
	public class FilteredCollection<T, C>: ObservableCollection<T> where C: INotifyCollectionChanged, IList
	{
		C source;
		Predicate<object> condition;
		List<int> srcPtrs = new List<int>(); // Index to the original collection
		
		/// <summary> Wrap the given collection.  Items of type other then T are filtered </summary>
		public FilteredCollection(C source) : this (source, x => true) { }
		
		/// <summary> Wrap the given collection.  Items of type other then T are filtered.  Items not matching the condition are filtered. </summary>
		public FilteredCollection(C source, Predicate<object> condition)
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
				if (source[i] is T && condition(source[i])) {
					this.Add((T)source[i]);
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
						if (e.NewItems[i] is T && condition(e.NewItems[i])) {
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
	public class MergedCollection<T, C>: ObservableCollection<T> where C: INotifyCollectionChanged, IList<T>
	{
		C a;
		C b;
		
		/// <summary> Create a wrapper containing elements of 'a' and then 'b' </summary>
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
