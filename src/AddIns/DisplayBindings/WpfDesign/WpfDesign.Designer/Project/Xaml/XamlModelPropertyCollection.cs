// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Windows;
using ICSharpCode.WpfDesign.XamlDom;

namespace ICSharpCode.WpfDesign.Designer.Xaml
{
	sealed class XamlModelPropertyCollection : DesignItemPropertyCollection
	{
		XamlDesignItem _item;
		
		public XamlModelPropertyCollection(XamlDesignItem item)
		{
			this._item = item;
		}
		
		public override DesignItemProperty this[DependencyProperty dependencyProperty] {
			get {
				//return new XamlModelProperty(_item, dependencyProperty);
				return this[dependencyProperty.Name];
			}
		}
		
		public override DesignItemProperty this[string name] {
			get {
				return new XamlModelProperty(_item, _item.XamlObject.FindOrCreateProperty(name));
			}
		}
		
		public override System.Collections.Generic.IEnumerator<DesignItemProperty> GetEnumerator()
		{
			throw new NotImplementedException();
		}
	}
}
