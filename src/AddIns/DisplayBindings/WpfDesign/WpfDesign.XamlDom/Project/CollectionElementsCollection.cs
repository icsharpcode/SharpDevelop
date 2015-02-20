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
using System.Collections.Specialized;

namespace ICSharpCode.WpfDesign.XamlDom
{
	/// <summary>
	/// The collection used by XamlProperty.CollectionElements
	/// </summary>
	sealed class CollectionElementsCollection : Collection<XamlPropertyValue>, INotifyCollectionChanged
	{
		XamlProperty property;
		bool isClearing = false;
		
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
			isClearing = true;
			try {
				while (Count > 0) {
					RemoveAt(Count - 1);
				}
			} finally {
				isClearing = false;
			}
			
			if (CollectionChanged != null)
				CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
		}
		
		protected override void RemoveItem(int index)
		{
			XamlPropertyInfo info = property.propertyInfo;
			object collection = info.GetValue(property.ParentObject.Instance);
			if (!CollectionSupport.RemoveItemAt(info.ReturnType, collection, index)) {
				var propertyValue = this[index];
				CollectionSupport.RemoveItem(info.ReturnType, collection, propertyValue.GetValueFor(info), propertyValue);
			}
			
			var item = this[index];
			item.RemoveNodeFromParent();
			item.ParentProperty = null;
			base.RemoveItem(index);
			
			if (CollectionChanged != null && !isClearing)
				CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, item, index));
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
			
			if (CollectionChanged != null)
				CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item, index));
		}
		
		protected override void SetItem(int index, XamlPropertyValue item)
		{
			var oldItem = this[index];
			RemoveItem(index);
			InsertItem(index, item);
			
			if (CollectionChanged != null)
				CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, item, oldItem, index));
		}

		#region INotifyCollectionChanged implementation

		public event NotifyCollectionChangedEventHandler CollectionChanged;

		#endregion
	}
}
