// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.ComponentModel;
using System.Windows.Markup;

namespace ICSharpCode.WpfDesign.XamlDom
{
	/// <summary>
	/// Represents a markup extension.
	/// </summary>
	public class XamlMarkupValue : XamlPropertyValue
	{
		XamlObject markupObject;
		
		internal XamlMarkupValue(XamlObject markupObject)
		{
			this.markupObject = markupObject;
		}
		
		internal override object GetValueFor(XamlPropertyInfo targetProperty)
		{
			return ((MarkupExtension)markupObject.Instance).ProvideValue(markupObject.OwnerDocument.ServiceProvider);
		}
		
		internal override void OnParentPropertyChanged()
		{
			base.OnParentPropertyChanged();
			markupObject.ParentProperty = this.ParentProperty;
		}
		
		internal override void RemoveNodeFromParent()
		{
			markupObject.RemoveNodeFromParent();
		}
		
		internal override void AddNodeTo(XamlProperty property)
		{
			markupObject.AddNodeTo(property);
		}
		
		internal override System.Xml.XmlNode GetNodeForCollection()
		{
			return markupObject.GetNodeForCollection();
		}
	}
}
