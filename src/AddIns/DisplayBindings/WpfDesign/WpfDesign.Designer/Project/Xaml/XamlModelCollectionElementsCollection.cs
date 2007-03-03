// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using ICSharpCode.WpfDesign.XamlDom;

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
			property.CollectionElements.Add(CheckItem(item).XamlObject);
		}
		
		public void Clear()
		{
			property.CollectionElements.Clear();
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
			Func.ToArray(this).CopyTo(array, arrayIndex);
		}
		
		public bool Remove(DesignItem item)
		{
			XamlDesignItem xitem = CheckItemNoException(item);
			if (xitem != null)
				return property.CollectionElements.Remove(xitem.XamlObject);
			else
				return false;
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
				property.CollectionElements[index] = CheckItem(value).XamlObject;
			}
		}
		
		public void Insert(int index, DesignItem item)
		{
			property.CollectionElements.Insert(index, CheckItem(item).XamlObject);
		}
		
		public void RemoveAt(int index)
		{
			property.CollectionElements.RemoveAt(index);
		}
	}
}
