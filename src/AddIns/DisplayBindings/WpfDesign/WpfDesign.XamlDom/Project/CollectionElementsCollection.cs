// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;

namespace ICSharpCode.WpfDesign.XamlDom
{
	partial class XamlProperty
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
			
			internal void AddByParser(XamlPropertyValue value)
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
				info.AddValue(collection, item);
				
				item.AddNodeTo(property);
				item.ParentProperty = property;
				
				base.InsertItem(index, item);
			}
		}
	}
}
