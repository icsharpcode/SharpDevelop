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
	sealed class XamlModelCollectionElementsCollection : Collection<DesignItem>
	{
		readonly XamlModelProperty modelProperty;
		readonly XamlProperty property;
		
		public XamlModelCollectionElementsCollection(XamlModelProperty modelProperty, XamlProperty property)
		{
			this.modelProperty = modelProperty;
			this.property = property;
			
			XamlDesignContext context = (XamlDesignContext)modelProperty.DesignItem.Context;
			foreach (XamlPropertyValue val in property.CollectionElements) {
				//context._componentService.GetDesignItem(val);
				if (val is XamlObject) {
					base.InsertItem(this.Count, context._componentService.GetDesignItem( ((XamlObject)val).Instance ));
				}
			}
		}
		
		protected override void ClearItems()
		{
			base.ClearItems();
			property.CollectionElements.Clear();
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
		
		protected override void InsertItem(int index, DesignItem item)
		{
			XamlDesignItem xitem = CheckItem(item);
			property.CollectionElements.Insert(index, xitem.XamlObject);
			
			base.InsertItem(index, item);
		}
		
		protected override void RemoveItem(int index)
		{
			XamlDesignItem item = (XamlDesignItem)this[index];
			
			property.CollectionElements.RemoveAt(index);
			base.RemoveItem(index);
		}
		
		protected override void SetItem(int index, DesignItem item)
		{
			XamlDesignItem xitem = CheckItem(item);
			property.CollectionElements[index] = xitem.XamlObject;
			
			
			base.SetItem(index, item);
		}
	}
}
