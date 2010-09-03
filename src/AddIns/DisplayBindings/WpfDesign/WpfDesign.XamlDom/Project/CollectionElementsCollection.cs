// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;

namespace ICSharpCode.WpfDesign.XamlDom
{
	/// <summary>
	/// The collection used by XamlProperty.CollectionElements
	/// </summary>
	sealed class CollectionElementsCollection : Collection<XamlPropertyValue>
	{
		XamlProperty property;
		
		internal CollectionElementsCollection(XamlProperty property)
		{
			this.property = property;
		}
		
		/// <summary>
		/// Used by parser to construct the collection without changing the XmlDocument.
		/// </summary>
		internal void AddInternal(XamlPropertyValue value)
		{
			base.InsertItem(this.Count, value);
		}
		
		protected override void ClearItems()
		{
			while (Count > 0) {
				RemoveAt(Count - 1);
			}
		}
		
		protected override void RemoveItem(int index)
		{
			XamlPropertyInfo info = property.propertyInfo;
			object collection = info.GetValue(property.ParentObject.Instance);
			if (!CollectionSupport.RemoveItemAt(info.ReturnType, collection, index)) {
				CollectionSupport.RemoveItem(info.ReturnType, collection, this[index].GetValueFor(info));
			}
			
			this[index].RemoveNodeFromParent();
			this[index].ParentProperty = null;
			base.RemoveItem(index);
		}
		
		protected override void InsertItem(int index, XamlPropertyValue item)
		{
			XamlPropertyInfo info = property.propertyInfo;
			object collection = info.GetValue(property.ParentObject.Instance);
			CollectionSupport.Insert(info.ReturnType, collection, item, index);
			
			item.ParentProperty = property;
			property.InsertNodeInCollection(item.GetNodeForCollection(), index);
			
			base.InsertItem(index, item);
		}
		
		protected override void SetItem(int index, XamlPropertyValue item)
		{
			RemoveItem(index);
			InsertItem(index, item);
		}
	}
}
