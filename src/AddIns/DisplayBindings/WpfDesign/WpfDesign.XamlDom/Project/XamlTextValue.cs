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
using System.Globalization;
using System.Text;
using System.Xml;

namespace ICSharpCode.WpfDesign.XamlDom
{
	/// <summary>
	/// A textual value in a .xaml file.
	/// </summary>
	public sealed class XamlTextValue : XamlPropertyValue
	{
		XamlDocument document;
		XmlAttribute attribute;
		XmlText textNode;
		XmlSpace xmlSpace;
		string textValue;
		XmlCDataSection cDataSection;
		
		internal XamlTextValue(XamlDocument document, XmlAttribute attribute)
		{
			this.document = document;
			this.attribute = attribute;
		}
		
		internal XamlTextValue(XamlDocument document, string textValue)
		{
			this.document = document;
			this.textValue = textValue;
		}
		
		internal XamlTextValue(XamlDocument document, XmlText textNode, XmlSpace xmlSpace)
		{
			this.document = document;
			this.xmlSpace = xmlSpace;
			this.textNode = textNode;
		}
		
		internal XamlTextValue(XamlDocument document, XmlCDataSection cDataSection, XmlSpace xmlSpace)
		{
			this.document = document;
			this.xmlSpace = xmlSpace;
			this.cDataSection = cDataSection;
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
				else if (cDataSection != null)
					return cDataSection.Value;
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
				else if (cDataSection != null)
					cDataSection.Value = value;
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
				return converter.ConvertFromString(document.GetTypeDescriptorContext(), CultureInfo.InvariantCulture, this.Text);
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
			else if (cDataSection != null)
				cDataSection.ParentNode.RemoveChild(cDataSection);
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
			} else if (cDataSection != null) {
				property.AddChildNodeToProperty(cDataSection);
			} else {
				property.AddChildNodeToProperty(textNode);
			}
		}
		
		internal override XmlNode GetNodeForCollection()
		{
			if (textNode != null)
				return textNode;
			else if (cDataSection != null)
				return cDataSection;
			else
				throw new NotImplementedException();
		}
	}
}
