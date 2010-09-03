// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Xml;
using System.Xml.Schema;

namespace ICSharpCode.XmlEditor
{
	public class XmlSchemaDefinition
	{
		public const string XmlSchemaNamespace = "http://www.w3.org/2001/XMLSchema";

		XmlSchemaCompletionCollection schemas;
		XmlSchemaCompletion currentSchema;
		SelectedXmlElement selectedElement;
		
		public XmlSchemaDefinition(XmlSchemaCompletionCollection schemas, XmlSchemaCompletion currentSchema)
		{
			this.schemas = schemas;
			this.currentSchema = currentSchema;
		}
		
		/// <summary>
		/// Determines whether the specified namespace is actually the W3C namespace for
		/// XSD files.
		/// </summary>
		public static bool IsXmlSchemaNamespace(string schemaNamespace)
		{
			return schemaNamespace == XmlSchemaNamespace;
		}
		
		public XmlSchemaObjectLocation GetSelectedSchemaObjectLocation(string xml, int index)
		{
			XmlSchemaObject schemaObject = GetSelectedSchemaObject(xml, index);
			return new XmlSchemaObjectLocation(schemaObject);
		}
		
		/// <summary>
		/// Gets the XmlSchemaObject that defines the currently selected xml element or
		/// attribute.
		/// </summary>
		/// <param name="text">The complete xml text.</param>
		/// <param name="index">The current cursor index.</param>
		/// <param name="currentSchemaCompletionData">This is the schema completion data for the
		/// schema currently being displayed. This can be null if the document is
		/// not a schema.</param>
		public XmlSchemaObject GetSelectedSchemaObject(string xml, int index)
		{
			FindSelectedElement(xml, index);
			return GetSelectedSchemaObject();
		}
		
		XmlSchemaObject GetSelectedSchemaObject()
		{
			XmlSchemaCompletion schemaForSelectedElement = FindSchemaForSelectedElement();
			if (schemaForSelectedElement == null) {
				return null;
			}
			
			XmlSchemaElement selectedSchemaElement = FindSchemaObjectForSelectedElement(schemaForSelectedElement);
			if (selectedSchemaElement == null) {
				return null;
			}

			if (selectedElement.HasSelectedAttribute) {
				XmlSchemaAttribute attribute = FindSchemaObjectForSelectedAttribute(schemaForSelectedElement, selectedSchemaElement);
				if (attribute == null) {
					return selectedSchemaElement;
				}
				
				if (selectedElement.HasSelectedAttributeValue) {
					XmlSchemaObject schemaObject = FindSchemaObjectReferencedByAttributeValue(selectedSchemaElement, attribute);
					if (schemaObject != null) {
						return schemaObject;
					}
				}
				return attribute;
			}
			return selectedSchemaElement;
		}
		
		void FindSelectedElement(string xml, int index)
		{
			selectedElement = new SelectedXmlElement(xml, index);
		}
		
		XmlSchemaCompletion FindSchemaForSelectedElement()
		{
			return schemas[selectedElement.Path.GetRootNamespace()];
		}
		
		XmlSchemaElement FindSchemaObjectForSelectedElement(XmlSchemaCompletion schemaForSelectedElement)
		{
			return schemaForSelectedElement.FindElement(selectedElement.Path);
		}
		
		XmlSchemaAttribute FindSchemaObjectForSelectedAttribute(XmlSchemaCompletion schemaForSelectedElement, XmlSchemaElement selectedSchemaElement)
		{
			return schemaForSelectedElement.FindAttribute(selectedSchemaElement, selectedElement.SelectedAttribute);
		}
		
		/// <summary>
		/// If the attribute value found references another item in the schema
		/// return this instead of the attribute schema object. For example, if the
		/// user can select the attribute value and the code will work out the schema object pointed to by the ref
		/// or type attribute:
		///
		/// xs:element ref="ref-name"
		/// xs:attribute type="type-name"
		/// </summary>
		/// <returns>
		/// The <paramref name="attribute"/> if no schema object was referenced.
		/// </returns>
		XmlSchemaObject FindSchemaObjectReferencedByAttributeValue(XmlSchemaElement element, XmlSchemaAttribute attribute)
		{
			if ((currentSchema != null) && IsXmlSchemaNamespaceElement(element)) {
				return GetSchemaObjectReferencedByAttributeValue(element.Name, attribute.Name);
			}
			return null;
		}
		
		XmlSchemaObject GetSchemaObjectReferencedByAttributeValue(string elementName, string attributeName)
		{
			if (attributeName == "ref") {
				return FindSchemaObjectReference(selectedElement.SelectedAttributeValue, elementName);
			} else if (attributeName == "type") {
				return FindSchemaObjectType(selectedElement.SelectedAttributeValue, elementName);
			}
			return null;
		}
		
		/// <summary>
		/// Attempts to locate the reference name in the specified schema.
		/// </summary>
		/// <param name="name">The reference to look up.</param>
		/// <param name="schemaCompletionData">The schema completion data to use to
		/// find the reference.</param>
		/// <param name="elementName">The element to determine what sort of reference it is
		/// (e.g. group, attribute, element).</param>
		/// <returns><see langword="null"/> if no match can be found.</returns>
		XmlSchemaObject FindSchemaObjectReference(string name, string elementName)
		{
			QualifiedName qualifiedName = currentSchema.CreateQualifiedName(name);
			XmlSchemaCompletion schema = GetSchemaForQualifiedName(qualifiedName);
			return FindSchemaObjectReference(qualifiedName, elementName, schema);
		}
		
		XmlSchemaObject FindSchemaObjectReference(QualifiedName qualifiedName, string elementName, XmlSchemaCompletion schema)
		{
			switch (elementName) {
				case "element":
					return schema.FindRootElement(qualifiedName);
				case "attribute":
					return schema.FindAttribute(qualifiedName.Name);
				case "group":
					return schema.FindGroup(qualifiedName.Name);
				case "attributeGroup":
					return schema.FindAttributeGroup(qualifiedName.Name);
			}
			return null;
		}
		
		XmlSchemaCompletion GetSchemaForQualifiedName(QualifiedName name)
		{
			XmlSchemaCompletion schema = schemas[name.Namespace];
			if (schema != null) {
				return schema;
			}
			return currentSchema;
		}
		
		/// <summary>
		/// Attempts to locate the type name in the specified schema.
		/// </summary>
		/// <param name="name">The type to look up.</param>
		/// <param name="elementName">The element to determine what sort of type it is
		/// (e.g. group, attribute, element).</param>
		/// <returns><see langword="null"/> if no match can be found.</returns>
		XmlSchemaObject FindSchemaObjectType(string name, string elementName)
		{
			QualifiedName qualifiedName = currentSchema.CreateQualifiedName(name);
			XmlSchemaCompletion schema = GetSchemaForQualifiedName(qualifiedName);
			return FindSchemaObjectType(qualifiedName, elementName, schema);
		}
		
		XmlSchemaObject FindSchemaObjectType(QualifiedName qualifiedName, string elementName, XmlSchemaCompletion schema)
		{
			switch (elementName) {
				case "element":
					return schema.FindComplexType(qualifiedName);
				case "attribute":
					return schema.FindSimpleType(qualifiedName.Name);
			}
			return null;
		}
		
		/// <summary>
		/// Checks whether the element belongs to the XSD namespace.
		/// </summary>
		static bool IsXmlSchemaNamespaceElement(XmlSchemaElement element)
		{
			XmlQualifiedName qualifiedName = element.QualifiedName;
			if (qualifiedName != null) {
				return IsXmlSchemaNamespace(qualifiedName.Namespace);
			}
			return false;
		}
	}
}
