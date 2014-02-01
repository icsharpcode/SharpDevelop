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

//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike KrÃƒÂ¼ger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing.Design;

namespace ICSharpCode.FormsDesigner.Services
{
	public class PropertyValueUIService : IPropertyValueUIService
	{
		PropertyValueUIHandler propertyValueUIHandler;

		public void AddPropertyValueUIHandler(PropertyValueUIHandler newHandler)
		{
			propertyValueUIHandler += newHandler;
		}

		public PropertyValueUIItem[] GetPropertyUIValueItems(ITypeDescriptorContext context, PropertyDescriptor propDesc)
		{
			// Let registered handlers have a chance to add their UIItems
			ArrayList propUIValues = new ArrayList();
			if (propertyValueUIHandler != null) {
				propertyValueUIHandler(context,propDesc,propUIValues);
			}
			PropertyValueUIItem[] values = new PropertyValueUIItem[propUIValues.Count];
			if (propUIValues.Count > 0) {
				propUIValues.CopyTo(values);
			}
			return values;
		}

		public void NotifyPropertyValueUIItemsChanged()
		{
			OnPropertyUIValueItemsChanged(EventArgs.Empty);
		}

		public void RemovePropertyValueUIHandler(PropertyValueUIHandler newHandler)
		{
			propertyValueUIHandler -= newHandler;
		}

		protected virtual void OnPropertyUIValueItemsChanged(EventArgs e)
		{
			if (PropertyUIValueItemsChanged != null) {
				PropertyUIValueItemsChanged(this, e);
			}
		}

		public event EventHandler PropertyUIValueItemsChanged;
	}
}
