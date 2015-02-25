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
using System.Diagnostics;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using ICSharpCode.WpfDesign.XamlDom;
using ICSharpCode.WpfDesign.Designer.Services;
using System.Collections.Specialized;

namespace ICSharpCode.WpfDesign.Designer.Xaml
{
	sealed class XamlModelCollectionElementsCollection : IList<DesignItem>, INotifyCollectionChanged
	{
		readonly XamlModelProperty modelProperty;
		readonly XamlProperty property;
		readonly XamlDesignContext context;
		
		public event NotifyCollectionChangedEventHandler CollectionChanged;
		
		public XamlModelCollectionElementsCollection(XamlModelProperty modelProperty, XamlProperty property)
		{
			this.modelProperty = modelProperty;
			this.property = property;
			this.context = (XamlDesignContext)modelProperty.DesignItem.Context;
		}
		
		public int Count {
			get {
				return property.CollectionElements.Count;
			}
		}
		
		public bool IsReadOnly {
			get {
				return false;
			}
		}
		
		public void Add(DesignItem item)
		{
			Insert(this.Count, item);
		}
		
		public void Clear()
		{
			while (this.Count > 0) {
				RemoveAt(this.Count - 1);
			}
			
			if (CollectionChanged != null)
				CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
		}
		
		public bool Contains(DesignItem item)
		{
			XamlDesignItem xitem = CheckItemNoException(item);
			if (xitem != null)
				return property.CollectionElements.Contains(xitem.XamlObject);
			else
				return false;
		}
		
		public int IndexOf(DesignItem item)
		{
			XamlDesignItem xitem = CheckItemNoException(item);
			if (xitem != null)
				return property.CollectionElements.IndexOf(xitem.XamlObject);
			else
				return -1;
		}
		
		public void CopyTo(DesignItem[] array, int arrayIndex)
		{
			for (int i = 0; i < this.Count; i++) {
				array[arrayIndex + i] = this[i];
			}
		}
		
		public bool Remove(DesignItem item)
		{
			int index = IndexOf(item);
			if (index < 0)
				return false;
			
			RemoveAt(index);
			
			return true;
		}
		
		public IEnumerator<DesignItem> GetEnumerator()
		{
			foreach (XamlPropertyValue val in property.CollectionElements) {
				var item = GetItem(val);
				if (item != null)
					yield return item;
			}
		}
		
		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}
		
		DesignItem GetItem(XamlPropertyValue val)
		{
			if (val is XamlObject) {
				return context._componentService.GetDesignItem( ((XamlObject)val).Instance );
			} else {
				return null; //	throw new NotImplementedException();
			}
		}
		
		XamlDesignItem CheckItem(DesignItem item)
		{
			if (item == null)
				throw new ArgumentNullException("item");
			if (item.Context != modelProperty.DesignItem.Context)
				throw new ArgumentException("The item must belong to the same context as this collection", "item");
			XamlDesignItem xitem = item as XamlDesignItem;
			Debug.Assert(xitem != null);
			return xitem;
		}
		
		XamlDesignItem CheckItemNoException(DesignItem item)
		{
			return item as XamlDesignItem;
		}
		
		public DesignItem this[int index] {
			get {
				return GetItem(property.CollectionElements[index]);
			}
			set {
				RemoveAt(index);
				Insert(index, value);
			}
		}
		
		public void Insert(int index, DesignItem item)
		{
			Execute(new InsertAction(this, index, CheckItem(item)));
		}
		
		public void RemoveAt(int index)
		{
			Execute(new RemoveAtAction(this, index, (XamlDesignItem)this[index]));
		}
		
		internal ITransactionItem CreateResetTransaction()
		{
			return new ResetAction(this);
		}
		
		void Execute(ITransactionItem item)
		{
			UndoService undoService = context.Services.GetService<UndoService>();
			if (undoService != null)
				undoService.Execute(item);
			else
				item.Do();
		}
		
		void RemoveInternal(int index, XamlDesignItem item)
		{
			NameScopeHelper.NameChanged(item.XamlObject, item.Name, null);

			Debug.Assert(property.CollectionElements[index] == item.XamlObject);
			property.CollectionElements.RemoveAt(index);
			
			if (CollectionChanged != null)
				CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, item, index));
		}
		
		void InsertInternal(int index, XamlDesignItem item)
		{
			property.CollectionElements.Insert(index, item.XamlObject);
			
			if (CollectionChanged != null)
				CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item, index));

			NameScopeHelper.NameChanged(item.XamlObject, null, item.Name);
		}
		
		sealed class InsertAction : ITransactionItem
		{
			readonly XamlModelCollectionElementsCollection collection;
			readonly int index;
			readonly XamlDesignItem item;
			
			public InsertAction(XamlModelCollectionElementsCollection collection, int index, XamlDesignItem item)
			{
				this.collection = collection;
				this.index = index;
				this.item = item;
			}
			
			public ICollection<DesignItem> AffectedElements {
				get {
					return new DesignItem[] { item };
				}
			}
			
			public string Title {
				get {
					return "Insert into collection";
				}
			}
			
			public void Do()
			{
				collection.InsertInternal(index, item);
				collection.modelProperty.XamlDesignItem.NotifyPropertyChanged(collection.modelProperty);
			}
			
			public void Undo()
			{
				collection.RemoveInternal(index, item);
				collection.modelProperty.XamlDesignItem.NotifyPropertyChanged(collection.modelProperty);
			}
			
			public bool MergeWith(ITransactionItem other)
			{
				return false;
			}
		}
		
		sealed class RemoveAtAction : ITransactionItem
		{
			readonly XamlModelCollectionElementsCollection collection;
			readonly int index;
			readonly XamlDesignItem item;
			
			public RemoveAtAction(XamlModelCollectionElementsCollection collection, int index, XamlDesignItem item)
			{
				this.collection = collection;
				this.index = index;
				this.item = item;
			}
			
			public ICollection<DesignItem> AffectedElements {
				get {
					return new DesignItem[] { collection.modelProperty.DesignItem };
				}
			}
			
			public string Title {
				get {
					return "Remove from collection";
				}
			}
			
			public void Do()
			{
				collection.RemoveInternal(index, item);
				collection.modelProperty.XamlDesignItem.NotifyPropertyChanged(collection.modelProperty);
			}
			
			public void Undo()
			{
				collection.InsertInternal(index, item);
				collection.modelProperty.XamlDesignItem.NotifyPropertyChanged(collection.modelProperty);
			}
			
			public bool MergeWith(ITransactionItem other)
			{
				return false;
			}
		}
		
		sealed class ResetAction : ITransactionItem
		{
			readonly XamlModelCollectionElementsCollection collection;
			readonly XamlDesignItem[] items;
			
			public ResetAction(XamlModelCollectionElementsCollection collection)
			{
				this.collection = collection;
				
				items = new XamlDesignItem[collection.Count];
				for (int i = 0; i < collection.Count; i++) {
					items[i] = (XamlDesignItem)collection[i];
				}
			}
			
			#region ITransactionItem implementation
			
			public void Do()
			{
				for (int i = items.Length - 1; i >= 0; i--) {
					collection.RemoveInternal(i, items[i]);
				}
				collection.modelProperty.XamlDesignItem.NotifyPropertyChanged(collection.modelProperty);
			}
			public void Undo()
			{
				for (int i = 0; i < items.Length; i++) {
					collection.InsertInternal(i, items[i]);
				}
				collection.modelProperty.XamlDesignItem.NotifyPropertyChanged(collection.modelProperty);
			}
			public bool MergeWith(ITransactionItem other)
			{
				return false;
			}
			
			#endregion
			
			#region IUndoAction implementation
			
			public ICollection<DesignItem> AffectedElements {
				get {
					return new DesignItem[] { collection.modelProperty.DesignItem };
				}
			}
			
			public string Title {
				get {
					return "Reset collection";
				}
			}
			
			#endregion
		}
	}
}
