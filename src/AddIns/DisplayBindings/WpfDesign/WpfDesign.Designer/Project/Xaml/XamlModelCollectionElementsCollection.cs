// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using ICSharpCode.WpfDesign.XamlDom;
using ICSharpCode.WpfDesign.Designer.Services;

namespace ICSharpCode.WpfDesign.Designer.Xaml
{
	sealed class XamlModelCollectionElementsCollection : IList<DesignItem>
	{
		readonly XamlModelProperty modelProperty;
		readonly XamlProperty property;
		readonly XamlDesignContext context;
		
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
				yield return GetItem(val);
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
				throw new NotImplementedException();
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
			Debug.Assert(property.CollectionElements[index] == item.XamlObject);
			property.CollectionElements.RemoveAt(index);
		}
		
		void InsertInternal(int index, XamlDesignItem item)
		{
			property.CollectionElements.Insert(index, item.XamlObject);
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
	}
}
