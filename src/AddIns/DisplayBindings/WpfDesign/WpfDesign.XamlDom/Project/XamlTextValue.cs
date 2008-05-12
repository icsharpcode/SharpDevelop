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
					if (attribute.Value.StartsWith("{}"))
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
		
		static readonly TypeConverter stringTypeConverter = TypeDescriptor.GetConverter(typeof(string));
		
		internal override object GetValueFor(XamlPropertyInfo targetProperty)
		{
			if (attribute != null) {
				return AttributeTextToObject(attribute.Value, attribute.OwnerElement, document,
				                             targetProperty != null ? targetProperty.TypeConverter : stringTypeConverter);
			}
			if (targetProperty == null)
				return this.Text;
			TypeConverter converter = targetProperty.TypeConverter;
			if (converter != null) {
				return converter.ConvertFromString(document.GetTypeDescriptorContext(), CultureInfo.InvariantCulture, this.Text);
			} else {
				return this.Text;
			}
		}
		
		internal static object AttributeTextToObject(string attributeText, XmlElement containingElement,
		                                             XamlDocument document, TypeConverter typeConverter)
		{
			if (typeConverter == null)
				typeConverter = stringTypeConverter;
			if (attributeText.StartsWith("{}")) {
				return typeConverter.ConvertFromString(
					document.GetTypeDescriptorContext(), CultureInfo.InvariantCulture,
					attributeText.Substring(2));
			} else if (attributeText.StartsWith("{")) {
				XamlTypeResolverProvider xtrp = new XamlTypeResolverProvider(document, containingElement, document.ServiceProvider);
				MarkupExtension extension = MarkupExtensionParser.ConstructMarkupExtension(
					attributeText, containingElement, document);
				return extension.ProvideValue(xtrp);
			} else {
				return typeConverter.ConvertFromString(
					document.GetTypeDescriptorContext(), CultureInfo.InvariantCulture,
					attributeText);
			}
		}
		
		sealed class XamlTypeResolverProvider : IXamlTypeResolver, IServiceProvider
		{
			XamlDocument document;
			XmlElement containingElement;
			IServiceProvider baseProvider;
			
			public XamlTypeResolverProvider(XamlDocument document, XmlElement containingElement, IServiceProvider baseProvider)
			{
				this.document = document;
				this.containingElement = containingElement;
				this.baseProvider = baseProvider;
			}
			
			public Type Resolve(string typeName)
			{
				string typeNamespaceUri;
				string typeLocalName;
				if (typeName.Contains(":")) {
					typeNamespaceUri = containingElement.GetNamespaceOfPrefix(typeName.Substring(0, typeName.IndexOf(':')));
					typeLocalName = typeName.Substring(typeName.IndexOf(':') + 1);
				} else {
					typeNamespaceUri = containingElement.NamespaceURI;
					typeLocalName = typeName;
				}
				if (string.IsNullOrEmpty(typeNamespaceUri))
					throw new XamlMarkupExtensionParseException("Unrecognized namespace prefix in type " + typeName);
				return document.TypeFinder.GetType(typeNamespaceUri, typeLocalName);
			}
			
			public object GetService(Type serviceType)
			{
				if (serviceType == typeof(IXamlTypeResolver))
					return this;
				else
					return baseProvider.GetService(serviceType);
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
