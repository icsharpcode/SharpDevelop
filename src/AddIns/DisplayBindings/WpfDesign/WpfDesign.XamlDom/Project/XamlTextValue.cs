// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Text;
using System.Xml;

namespace ICSharpCode.WpfDesign.XamlDom
{
	/// <summary>
	/// A textual value in a .xaml file.
	/// </summary>
	public sealed class XamlTextValue : XamlPropertyValue
	{
		XmlAttribute attribute;
		XmlText textNode;
		XmlSpace xmlSpace;
		string textValue;
		
		internal XamlTextValue(XmlAttribute attribute)
		{
			this.attribute = attribute;
		}
		
		internal XamlTextValue(string textValue)
		{
			this.textValue = textValue;
		}
		
		internal XamlTextValue(XmlText textNode, XmlSpace xmlSpace)
		{
			this.xmlSpace = xmlSpace;
			this.textNode = textNode;
		}
		
		/// <summary>
		/// The text represented by the value.
		/// </summary>
		public string Text {
			get {
				if (attribute != null)
					return attribute.Value;
				else if (textValue != null)
					return textValue;
				else
					return NormalizeWhitespace(textNode.Value);
			}
			set {
				if (value == null)
					throw new ArgumentNullException("value");
				
				if (attribute != null)
					attribute.Value = value;
				else if (textValue != null)
					textValue = value;
				else
					textNode.Value = value;
			}
		}
		
		string NormalizeWhitespace(string text)
		{
			if (xmlSpace == XmlSpace.Preserve) {
				return text.Replace("\r", "");
			}
			StringBuilder b = new StringBuilder();
			bool wasWhitespace = true;
			foreach (char c in text) {
				if (char.IsWhiteSpace(c)) {
					if (!wasWhitespace) {
						b.Append(' ');
					}
					wasWhitespace = true;
				} else {
					wasWhitespace = false;
					b.Append(c);
				}
			}
			if (b.Length > 0 && wasWhitespace)
				b.Length -= 1;
			return b.ToString();
		}
		
		internal override object GetValueFor(XamlPropertyInfo targetProperty)
		{
			if (targetProperty == null)
				return this.Text;
			TypeConverter converter = targetProperty.TypeConverter;
			if (converter != null) {
				return converter.ConvertFromInvariantString(this.Text);
			} else {
				return this.Text;
			}
		}
		
		internal override void RemoveNodeFromParent()
		{
			if (attribute != null)
				attribute.OwnerElement.RemoveAttribute(attribute.Name);
			else if (textNode != null)
				textNode.ParentNode.RemoveChild(textNode);
		}
		
		internal override void AddNodeTo(XamlProperty property)
		{
			if (attribute != null) {
				property.ParentObject.XmlElement.Attributes.Append(attribute);
			} else if (textValue != null) {
				string ns = property.ParentObject.OwnerDocument.GetNamespaceFor(property.PropertyTargetType);
				string name;
				if (property.IsAttached)
					name = property.PropertyTargetType.Name + "." + property.PropertyName;
				else
					name = property.PropertyName;
				if (property.ParentObject.XmlElement.GetPrefixOfNamespace(ns) == "") {
					property.ParentObject.XmlElement.SetAttribute(name, textValue);
					attribute = property.ParentObject.XmlElement.GetAttributeNode(name);
				} else {
					property.ParentObject.XmlElement.SetAttribute(name, ns, textValue);
					attribute = property.ParentObject.XmlElement.GetAttributeNode(name, ns);
				}
				textValue = null;
			} else {
				property.AddChildNodeToProperty(textNode);
			}
		}
		
		internal override XmlNode GetNodeForCollection()
		{
			if (textNode != null)
				return textNode;
			else
				throw new NotImplementedException();
		}
	}
}
