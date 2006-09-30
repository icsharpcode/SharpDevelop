// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using ICSharpCode.TextEditor.Gui.CompletionWindow;
using ICSharpCode.XmlEditor;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Xml;
using System.Xml.Schema;

namespace ICSharpCode.WixBinding
{
	/// <summary>
	/// Wix schema completion data read from wix.xsd which is an embedded resource.
	/// </summary>
	public class WixSchemaCompletionData : XmlSchemaCompletionData
	{
		public WixSchemaCompletionData() : base(GetWixSchema())
		{
		}
		
		/// <summary>
		/// Returns the child element names for the specified element.
		/// </summary>
		/// <remarks>
		/// The method assumes the element is defined in the root of the schema
		/// (i.e. is accessible via the XmlSchema.Elements property).
		/// </remarks>
		public string[] GetChildElements(string elementName)
		{
			return GetItems(elementName, GetChildElementCompletionData).ToArray();
		}
		
		/// <summary>
		/// Gets the attributes for the specified element name.
		/// </summary>
		/// <remarks>
		/// The method assumes the element is defined in the root of the schema
		/// (i.e. is accessible via the XmlSchema.Elements property).
		/// </remarks>
		public string[] GetAttributeNames(string elementName)
		{
			List<string> attributes = GetItems(elementName, GetAttributeCompletionData);
			
			// Exclude deprecated attributes.
			List<string> deprecatedAttributes = FindDeprecatedAttributes(elementName);
			if (deprecatedAttributes.Count > 0) {
				attributes = RemoveDeprecatedAttributes(attributes, deprecatedAttributes);
			}
			return attributes.ToArray();
		}
		
		/// <summary>
		/// Gets the attributes for the specified element. Also adds any standard
		/// attributes for the element which are not set.
		/// </summary>
		public WixXmlAttributeCollection GetAttributes(XmlElement element)
		{
			WixXmlAttributeCollection attributes = new WixXmlAttributeCollection();
			string elementName = element.Name;
			foreach (XmlAttribute attribute in element.Attributes) {
				string attributeName = attribute.Name;
				WixXmlAttributeType type = GetWixAttributeType(elementName, attributeName);
				WixXmlAttribute wixAttribute = new WixXmlAttribute(attributeName, attribute.Value, type);
				attributes.Add(wixAttribute);
			}
			attributes.AddRange(GetMissingAttributes(elementName, attributes, GetAttributeNames(elementName)));
			return attributes;
		}
		
		/// <summary>
		/// Returns the deprecated attributes for the specified element name.
		/// </summary>
		public string[] GetDeprecatedAttributes(string elementName)
		{
			return FindDeprecatedAttributes(elementName).ToArray();
		}
		
		/// <summary>
		/// Returns the qualified name of the attribute's type.
		/// </summary>
		public QualifiedName GetAttributeType(string elementName, string attributeName)
		{
			XmlElementPath path = GetPath(elementName);
			XmlSchemaElement element = FindElement(path);
			if (element != null) {
				XmlSchemaComplexType complexType = GetElementAsComplexType(element);
				if (complexType != null) {
					foreach (XmlSchemaAttribute attribute in complexType.Attributes) {
						if (attribute.Name == attributeName) {
							XmlQualifiedName name = attribute.SchemaTypeName;
							if (name != null) {
								return new QualifiedName(name.Name, name.Namespace);
							}
							return null;
						}
					}
				}
			}
			return null;
		}
		
		/// <summary>
		/// Delegate that allows us to get a different type of completion data, but use
		/// the same GetItems method to create a list of strings.
		/// </summary>
		delegate ICompletionData[] GetCompletionData(XmlElementPath path);
		
		/// <summary>
		/// Gets the completion data text for the completion data items that are
		/// returned for the specified element.
		/// </summary>
		List<string> GetItems(string elementName, GetCompletionData getCompletionData)
		{
			List<string> items = new List<string>();
			XmlElementPath path = GetPath(elementName);
			foreach (ICompletionData completionData in getCompletionData(path)) {
				items.Add(completionData.Text);
			}
			return items;
		}
		
		XmlElementPath GetPath(string elementName)
		{
			XmlElementPath path = new XmlElementPath();
			path.Elements.Add(new QualifiedName(elementName, WixNamespaceManager.Namespace));
			return path;
		}
		                                        
		static XmlTextReader GetWixSchema()
		{
			Assembly assembly = Assembly.GetAssembly(typeof(WixSchemaCompletionData));
			string resourceName = "ICSharpCode.WixBinding.Resources.wix.xsd";
			Stream resourceStream = assembly.GetManifestResourceStream(resourceName);
			return new XmlTextReader(resourceStream);
		}
		
		/// <summary>
		/// Gets the attributes marked as deprecated in the Wix schema. 
		/// </summary>
		/// <remarks>
		/// The Wix schema marks the attributes as deprecated by using the xse:deprecated 
		/// tag in the attributes xs:documentation/xs:appinfo tag. The xse prefix is
		/// the Microsoft schema extension namespace:  
		/// "http://schemas.microsoft.com/wix/2005/XmlSchemaExtension".
		/// Thankfully the Wix schema does not use any complicated type definitions 
		/// (e.g. extensions, restrictions) so getting the attribute is a simple case of
		/// getting the element as a complex type and then looking at its attributes.
		/// </remarks>
		List<string> FindDeprecatedAttributes(string elementName)
		{
			XmlElementPath path = GetPath(elementName);
			XmlSchemaElement element = FindElement(path);
			if (element != null) {
				XmlSchemaComplexType complexType = GetElementAsComplexType(element);
				if (complexType != null) {
					return GetDeprecatedAttributes(complexType);
				}
			}
			return new List<string>();
		}
		
		List<string> GetDeprecatedAttributes(XmlSchemaComplexType complexType)
		{
			List<string> attributes = new List<string>();
			foreach (XmlSchemaAttribute attribute in complexType.Attributes) {
				if (IsDeprecated(attribute)) {
					attributes.Add(attribute.Name);
				}
			}
			return attributes;
		}
		
		bool IsDeprecated(XmlSchemaAttribute attribute)
		{
			XmlSchemaAnnotation annotation = attribute.Annotation;
			if (annotation != null) {
				foreach (XmlSchemaObject schemaObject in annotation.Items) {
					XmlSchemaAppInfo appInfo = schemaObject as XmlSchemaAppInfo;
					if (appInfo != null) {
						foreach (XmlNode node in appInfo.Markup) {
							XmlElement element = node as XmlElement;
							if (element.LocalName == "deprecated") {
								return true;
							}
						}
					}
				}
			}
			return false;
		}
		
		List<string> RemoveDeprecatedAttributes(List<string> allAttributes, List<string> deprecatedAttributes)
		{
			List<string> attributes = new List<string>();
			foreach (string attribute in allAttributes) {
				if (!deprecatedAttributes.Contains(attribute)) {
					attributes.Add(attribute);
				}
			}
			return attributes;
		}
		
		WixXmlAttributeType GetWixAttributeType(string elementName, string attributeName)
		{
			QualifiedName attributeTypeName = GetAttributeType(elementName, attributeName);
			if (attributeTypeName != null) {
				switch (attributeTypeName.Name) {
					case "autogenuuid":
						return WixXmlAttributeType.AutogenUuid;
					case "uuid":
						return WixXmlAttributeType.Uuid;
					case "ComponentGuid":
						return WixXmlAttributeType.ComponentGuid;
				}
			}
			return WixXmlAttributeType.Text;
		}
		
		/// <summary>
		/// Gets the attributes that have not been added to the 
		/// <paramref name="attributes"/>.
		/// </summary>		
		WixXmlAttributeCollection GetMissingAttributes(string elementName, WixXmlAttributeCollection existingAttributes, string[] attributes)
		{
			WixXmlAttributeCollection missingAttributes = new WixXmlAttributeCollection();
			foreach (string name in attributes) {
				if (existingAttributes[name] == null) {
					WixXmlAttributeType type = GetWixAttributeType(elementName, name);
					missingAttributes.Add(new WixXmlAttribute(name, type));
				}
			}
			return missingAttributes;
		}
	}
}
