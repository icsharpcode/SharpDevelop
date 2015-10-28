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
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Text;
using System.Windows.Markup;
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
			if (textValue.StartsWith("{"))
				textValue = "{}" + textValue;
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
				if (attribute != null) {
					if (attribute.Value.StartsWith("{}", StringComparison.Ordinal))
						return attribute.Value.Substring(2);
					else
						return attribute.Value;
				} else if (textValue != null)
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
			if (ParentProperty == null)
				throw new InvalidOperationException("Cannot call GetValueFor while ParentProperty is null");
			
			if (targetProperty == null)
				return this.Text;

			return XamlParser.CreateObjectFromAttributeText(Text, targetProperty, ParentProperty.ParentObject);
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
				attribute = property.SetAttribute(textValue);
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
