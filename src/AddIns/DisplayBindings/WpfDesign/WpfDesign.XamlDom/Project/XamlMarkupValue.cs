// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Windows.Markup;

namespace ICSharpCode.WpfDesign.XamlDom
{
	/// <summary>
	/// Represents a markup extension.
	/// </summary>
	public class XamlMarkupValue : XamlPropertyValue
	{
		readonly XamlDocument doc;
		XamlObject markupObject;
		
		internal XamlMarkupValue(XamlDocument doc, XamlObject markupObject)
		{
			this.doc = doc;
			this.markupObject = markupObject;
		}
		
		internal override object GetValueFor(XamlPropertyInfo targetProperty)
		{
			return ((MarkupExtension)markupObject.Instance).ProvideValue(doc.ServiceProvider);
		}
		
		internal override void RemoveNodeFromParent()
		{
			markupObject.RemoveNodeFromParent();
		}
		
		internal override void AddNodeTo(XamlProperty property)
		{
			markupObject.AddNodeTo(property);
		}
	}
}
