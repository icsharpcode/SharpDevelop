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
			if (!CollectionSupport.TryInsert(info.ReturnType, collection, item, index)) {
				CollectionSupport.AddToCollection(info.ReturnType, collection, item);
			}
			
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
