// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
