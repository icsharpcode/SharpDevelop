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
using System.Windows;
using ICSharpCode.WpfDesign.XamlDom;
using System.Collections;
using System.Collections.Generic;

namespace ICSharpCode.WpfDesign.Designer.Xaml
{
	sealed class XamlModelPropertyCollection : DesignItemPropertyCollection
	{
		XamlDesignItem _item;
		Dictionary<string, XamlModelProperty> propertiesDictionary = new Dictionary<string, XamlModelProperty>();
		
		public XamlModelPropertyCollection(XamlDesignItem item)
		{
			this._item = item;
		}
		
		public override DesignItemProperty GetProperty(string name)
		{
			XamlModelProperty property;
			if (propertiesDictionary.TryGetValue(name, out property))
				return property;
			property = new XamlModelProperty(_item, _item.XamlObject.FindOrCreateProperty(name));
			propertiesDictionary.Add(name, property);
			return property;
		}
		
		public override DesignItemProperty GetAttachedProperty(Type ownerType, string name)
		{
			return new XamlModelProperty(_item, _item.XamlObject.FindOrCreateAttachedProperty(ownerType, name));
		}
		
		public override System.Collections.Generic.IEnumerator<DesignItemProperty> GetEnumerator()
		{
			yield break;
		}
	}
}
