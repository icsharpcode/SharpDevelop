// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Linq;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Schema;

using ICSharpCode.SharpDevelop.Editor.CodeCompletion;

namespace ICSharpCode.XmlEditor
{
	/// <summary>
	/// Holds the completion (intellisense) data for an xml schema.
	/// </summary>
	/// <remarks>
	/// The XmlSchema class throws an exception if we attempt to load
	/// the xhtml1-strict.xsd schema.  It does not like the fact that
	/// this schema redefines the xml namespace, even though this is
	/// allowed by the w3.org specification.
	/// </remarks>
	public class XmlSchemaCompletion
	{
		XmlSchema schema;
		string fileName = String.Empty;
		bool readOnly;
		XmlNamespace xmlNamespace = new XmlNamespace();
		
		/// <summary>
		/// Stores attributes that have been prohibited whilst the code
		/// generates the attribute completion data.
		/// </summary>
		XmlSchemaObjectCollection prohibitedAttributes = new XmlSchemaObjectCollection();
		
		public XmlSchemaCompletion()
		{
		}
		
		public string DefaultNamespacePrefix {
			get { return xmlNamespace.Prefix; }
			set { xmlNamespace.Prefix = value; }
		}
		
		/// <summary>
		/// Creates completion data from the schema passed in
		/// via the reader object.
		/// </summary>
		public XmlSchemaCompletion(TextReader reader)
		{
			ReadSchema(String.Empty, reader);
		}
		
		/// <summary>
		/// Creates the completion data from the specified schema file.
		/// </summary>
		public XmlSchemaCompletion(string fileName) : this(String.Empty, fileName)
		{
		}
		
		/// <summary>
		/// Creates the completion data from the specified schema file and uses
		/// the specified baseUri to resolve any referenced schemas.
		/// </summary>
		public XmlSchemaCompletion(string baseUri, string fileName)
		{
			StreamReader reader = new StreamReader(fileName, true);
			ReadSchema(baseUri, reader);
			this.fileName = fileName;
		}
		
		/// <summary>
		/// Gets the schema.
		/// </summary>
		public XmlSchema Schema {
			get { return schema; }
		}
		
		/// <summary>
		/// Read only schemas are those that are installed with
		/// SharpDevelop.
		/// </summary>
		public bool IsReadOnly {
			get { return readOnly; }
			set { readOnly = value; }
		}
		
		/// <summary>
		/// Gets or sets the schema's file name.
		/// </summary>
		public string FileName {
			get { return fileName; }
			set { fileName = value; }
		}
		
		/// <summary>
		/// Gets the namespace URI for the schema.
		/// </summary>
		public string NamespaceUri {
			get { return xmlNamespace.Name; }
		}

		public bool HasNamespaceUri {
			get { return !String.IsNullOrWhiteSpace(NamespaceUri); }
		}
		
		public XmlNamespace Namespace {
			get { return xmlNamespace; }
		}
		
		/// <summary>
		/// Converts the filename into a valid Uri.
		/// </summary>
		public static string GetUri(string fileName)
		{
			if (fileName != null) {
				if (fileName.Length > 0) {
					return String.Concat("file:///", fileName.Replace('\\', '/'));
				}
			}
			return String.Empty;
		}
		
		public XmlCompletionItemCollection GetRootElementCompletion()
		{
			return GetRootElementCompletion(DefaultNamespacePrefix);
		}
		
		public XmlCompletionItemCollection GetRootElementCompletion(string namespacePrefix)
		{
			XmlCompletionItemCollection items = new XmlCompletionItemCollection();
			
			foreach (XmlSchemaElement element in schema.Elements.Values) {
				if (element.Name != null) {
					AddElement(items, element.Name, namespacePrefix, element.Annotation);
				} else {
					// Do not add reference element.
				}
			}
			
			return items;
		}
		
		/// <summary>
		/// Gets the attribute completion data for the xml element that exists
		/// at the end of the specified path.
		/// </summary>
		public XmlCompletionItemCollection GetAttributeCompletion(XmlElementPath path)
		{
			// Locate matching element.
			XmlSchemaElement element = FindElement(path);
			
			// Get completion data.
			if (element != null) {
				prohibitedAttributes.Clear();
				return GetAttributeCompletion(element, path.NamespacesInScope);
			}
			
			return new XmlCompletionItemCollection();
		}
		
		/// <summary>
		/// Gets the child element completion data for the xml element that exists
		/// at the end of the specified path.
		/// </summary>
		public XmlCompletionItemCollection GetChildElementCompletion(XmlElementPath path)
		{
			XmlSchemaElement element = FindElement(path);	
			if (element != null) {
				return GetChildElementCompletion(element, path.Elements.GetLastPrefix());
			}
			
			return new XmlCompletionItemCollection();
		}
		
		public XmlCompletionItemCollection GetAttributeValueCompletion(XmlElementPath path, string attributeName)
		{
			XmlSchemaElement element = FindElement(path);
			if (element != null) {
				return GetAttributeValueCompletion(element, attributeName);
			}
			
			return new XmlCompletionItemCollection();
		}
		
		/// <summary>
		/// Finds the element that exists at the specified path.
		/// </summary>
		/// <remarks>This method is not used when generating completion data,
		/// but is a useful method when locating an element so we can jump
		/// to its schema definition.</remarks>
		/// <returns><see langword="null"/> if no element can be found.</returns>
		public XmlSchemaElement FindElement(XmlElementPath path)
		{
			XmlSchemaElement element = null;
			for (int i = 0; i < path.Elements.Count; ++i) {
				QualifiedName name = path.Elements[i];
				if (i == 0) {
					// Look for root element.
					element = FindRootElement(name);
					if (element == null) {
						return null;
					}
				} else {
					element = FindChildElement(element, name);
					if (element == null) {
						return null;
					}
				}
			}
			return element;
		}
		
		/// <summary>
		/// Finds an element in the schema.
		/// </summary>
		/// <remarks>
		/// Only looks at the elements that are defined in the
		/// root of the schema so it will not find any elements
		/// that are defined inside any complex types.
		/// </remarks>
		public XmlSchemaElement FindRootElement(QualifiedName name)
		{
			foreach (XmlSchemaElement element in schema.Elements.Values) {
				if (name.Equals(element.QualifiedName)) {
					return element;
				}
			}
			return null;
		}
		
		/// <summary>
		/// Finds the complex type with the specified name.
		/// </summary>
		public XmlSchemaComplexType FindComplexType(QualifiedName name)
		{
			XmlQualifiedName qualifiedName = new XmlQualifiedName(name.Name, name.Namespace);
			return FindNamedType(schema, qualifiedName);
		}
		
		/// <summary>
		/// Finds the specified attribute name given the element.
		/// </summary>
		/// <remarks>This method is not used when generating completion data,
		/// but is a useful method when locating an attribute so we can jump
		/// to its schema definition.</remarks>
		/// <returns><see langword="null"/> if no attribute can be found.</returns>
		public XmlSchemaAttribute FindAttribute(XmlSchemaElement element, string name)
		{
			XmlSchemaComplexType complexType = GetElementAsComplexType(element);
			if (complexType != null) {
				return FindAttribute(complexType, name);
			}
			return null;
		}
		
		/// <summary>
		/// Finds the attribute group with the specified name.
		/// </summary>
		public XmlSchemaAttributeGroup FindAttributeGroup(string name)
		{
			return FindAttributeGroup(schema, name);
		}
		
		/// <summary>
		/// Finds the simple type with the specified name.
		/// </summary>
		public XmlSchemaSimpleType FindSimpleType(string name)
		{
			XmlQualifiedName qualifiedName = new XmlQualifiedName(name, xmlNamespace.Name);
			return FindSimpleType(qualifiedName);
		}
		
		/// <summary>
		/// Finds the specified attribute in the schema. This method only checks
		/// the attributes defined in the root of the schema.
		/// </summary>
		public XmlSchemaAttribute FindAttribute(string name)
		{
			foreach (XmlSchemaAttribute attribute in schema.Attributes.Values) {
				if (attribute.Name == name) {
					return attribute;
				}
			}
			return null;
		}
		
		/// <summary>
		/// Finds the schema group with the specified name.
		/// </summary>
		public XmlSchemaGroup FindGroup(string name)
		{
			if (name != null) {
				foreach (XmlSchemaObject schemaObject in schema.Groups.Values) {
					XmlSchemaGroup schemaGroup = schemaObject as XmlSchemaGroup;
					if (schemaGroup != null) {
						if (schemaGroup.Name == name) {
							return schemaGroup;
						}
					}
				}
			}
			return null;
		}
		
		/// <summary>
		/// Takes the name and creates a qualified name using the namespace of this
		/// schema.
		/// </summary>
		/// <remarks>If the name is of the form myprefix:mytype then the correct
		/// namespace is determined from the prefix. If the name is not of this
		/// form then no prefix is added.</remarks>
		public QualifiedName CreateQualifiedName(string name)
		{
			QualifiedName qualifiedName = QualifiedName.FromString(name);
			if (qualifiedName.HasPrefix) {
				foreach (XmlQualifiedName xmlQualifiedName in schema.Namespaces.ToArray()) {
					if (xmlQualifiedName.Name == qualifiedName.Prefix) {
						qualifiedName.Namespace = xmlQualifiedName.Namespace;
						return qualifiedName;
					}
				}
			}
			
			// Default behaviour just return the name with the namespace uri.
			qualifiedName.Namespace = xmlNamespace.Name;
			return qualifiedName;
		}
		
		/// <summary>
		/// Converts the element to a complex type if possible.
		/// </summary>
		public XmlSchemaComplexType GetElementAsComplexType(XmlSchemaElement element)
		{
			XmlSchemaComplexType complexType = element.SchemaType as XmlSchemaComplexType;
			if (complexType == null) {
				if (element.SchemaTypeName.IsEmpty) {
					return GetComplexTypeFromSubstitutionGroup(element);
				}
				return FindNamedType(schema, element.SchemaTypeName);
			}
			return complexType;
		}
		
		XmlSchemaComplexType GetComplexTypeFromSubstitutionGroup(XmlSchemaElement element)
		{
			if (!element.SubstitutionGroup.IsEmpty) {
				XmlSchemaElement substitutedElement = FindElement(element.SubstitutionGroup);
				if (substitutedElement != null) {
					return GetElementAsComplexType(substitutedElement);
				}
			}
			return null;
		}
		
		/// <summary>
		/// Handler for schema validation errors.
		/// </summary>
		void SchemaValidation(object source, ValidationEventArgs e)
		{
			// Do nothing.
		}
		
		/// <summary>
		/// Loads the schema.
		/// </summary>
		void ReadSchema(XmlReader reader)
		{
			try {
				schema = XmlSchema.Read(reader, SchemaValidation);
				if (schema != null) {
					XmlSchemaSet schemaSet = new XmlSchemaSet();
					schemaSet.ValidationEventHandler += SchemaValidation;
					schemaSet.Add(schema);
					schemaSet.Compile();
					
					xmlNamespace.Name = schema.TargetNamespace;
				}
			} finally {
				reader.Close();
			}
		}
		
		void ReadSchema(string baseUri, TextReader reader)
		{
			XmlTextReader xmlReader = new XmlTextReader(baseUri, reader);
			
			// Setting the resolver to null allows us to
			// load the xhtml1-strict.xsd without any exceptions if
			// the referenced dtds exist in the same folder as the .xsd
			// file.  If this is not set to null the dtd files are looked
			// for in the assembly's folder.
			xmlReader.XmlResolver = null;
			ReadSchema(xmlReader);
		}
		
		/// <summary>
		/// Finds an element in the schema.
		/// </summary>
		/// <remarks>
		/// Only looks at the elements that are defined in the
		/// root of the schema so it will not find any elements
		/// that are defined inside any complex types.
		/// </remarks>
		XmlSchemaElement FindElement(XmlQualifiedName name)
		{
			foreach (XmlSchemaElement element in schema.Elements.Values) {
				if (name.Equals(element.QualifiedName)) {
					return element;
				}
			}
			return null;
		}
		
		XmlCompletionItemCollection GetChildElementCompletion(XmlSchemaElement element, string prefix)
		{
			XmlSchemaComplexType complexType = GetElementAsComplexType(element);
			if (complexType != null) {
				return GetChildElementCompletion(complexType, prefix);
			}
			return new XmlCompletionItemCollection();
		}
		
		XmlCompletionItemCollection GetChildElementCompletion(XmlSchemaComplexType complexType, string prefix)
		{
			XmlSchemaSequence sequence = complexType.Particle as XmlSchemaSequence;
			XmlSchemaChoice choice = complexType.Particle as XmlSchemaChoice;
			XmlSchemaGroupRef groupRef = complexType.Particle as XmlSchemaGroupRef;
			XmlSchemaComplexContent complexContent = complexType.ContentModel as XmlSchemaComplexContent;
			XmlSchemaAll all = complexType.Particle as XmlSchemaAll;
			
			if (sequence != null) {
				return GetChildElementCompletion(sequence.Items, prefix);
			} else if (choice != null) {
				return GetChildElementCompletion(choice.Items, prefix);
			} else if (complexContent != null) {
				return GetChildElementCompletion(complexContent, prefix);
			} else if (groupRef != null) {
				return GetChildElementCompletion(groupRef, prefix);
			} else if (all != null) {
				return GetChildElementCompletion(all.Items, prefix);
			}
			return new XmlCompletionItemCollection();
		}
		
		XmlCompletionItemCollection GetChildElementCompletion(XmlSchemaObjectCollection items, string prefix)
		{
			XmlCompletionItemCollection completionItems = new XmlCompletionItemCollection();
			
			foreach (XmlSchemaObject schemaObject in items) {
				
				XmlSchemaElement childElement = schemaObject as XmlSchemaElement;
				XmlSchemaSequence childSequence = schemaObject as XmlSchemaSequence;
				XmlSchemaChoice childChoice = schemaObject as XmlSchemaChoice;
				XmlSchemaGroupRef groupRef = schemaObject as XmlSchemaGroupRef;
				
				if (childElement != null) {
					string name = childElement.Name;
					if (name == null) {
						name = childElement.RefName.Name;
						XmlSchemaElement element = FindElement(childElement.RefName);
						if (element != null) {
							if (element.IsAbstract) {
								AddSubstitionGroupElements(completionItems, element.QualifiedName, prefix);
							} else {
								AddElement(completionItems, name, prefix, element.Annotation);
							}
						} else {
							AddElement(completionItems, name, prefix, childElement.Annotation);
						}
					} else {
						AddElement(completionItems, name, prefix, childElement.Annotation);
					}
				} else if (childSequence != null) {
					AddElements(completionItems, GetChildElementCompletion(childSequence.Items, prefix));
				} else if (childChoice != null) {
					AddElements(completionItems, GetChildElementCompletion(childChoice.Items, prefix));
				} else if (groupRef != null) {
					AddElements(completionItems, GetChildElementCompletion(groupRef, prefix));
				}
			}
			
			return completionItems;
		}
		
		XmlCompletionItemCollection GetChildElementCompletion(XmlSchemaComplexContent complexContent, string prefix)
		{
			XmlSchemaComplexContentExtension extension = complexContent.Content as XmlSchemaComplexContentExtension;
			if (extension != null) {
				return GetChildElementCompletion(extension, prefix);
			} else {
				XmlSchemaComplexContentRestriction restriction = complexContent.Content as XmlSchemaComplexContentRestriction;
				if (restriction != null) {
					return GetChildElementCompletion(restriction, prefix);
				}
			}
			return new XmlCompletionItemCollection();
		}
		
		XmlCompletionItemCollection GetChildElementCompletion(XmlSchemaComplexContentExtension extension, string prefix)
		{
			XmlCompletionItemCollection completionItems;
			XmlSchemaComplexType complexType = FindNamedType(schema, extension.BaseTypeName);
			if (complexType != null) {
				completionItems = GetChildElementCompletion(complexType, prefix);
			} else {
				completionItems = new XmlCompletionItemCollection();
			}
			
			// Add any elements.
			if (extension.Particle != null) {
				XmlSchemaSequence sequence = extension.Particle as XmlSchemaSequence;
				XmlSchemaChoice choice = extension.Particle as XmlSchemaChoice;
				XmlSchemaGroupRef groupRef = extension.Particle as XmlSchemaGroupRef;
				
				if(sequence != null) {
					completionItems.AddRange(GetChildElementCompletion(sequence.Items, prefix));
				} else if (choice != null) {
					completionItems.AddRange(GetChildElementCompletion(choice.Items, prefix));
				} else if (groupRef != null) {
					completionItems.AddRange(GetChildElementCompletion(groupRef, prefix));
				}
			}
			
			return completionItems;
		}
		
		XmlCompletionItemCollection GetChildElementCompletion(XmlSchemaGroupRef groupRef, string prefix)
		{
			XmlSchemaGroup schemaGroup = FindGroup(groupRef.RefName.Name);
			if (schemaGroup != null) {
				XmlSchemaSequence sequence = schemaGroup.Particle as XmlSchemaSequence;
				XmlSchemaChoice choice = schemaGroup.Particle as XmlSchemaChoice;
				
				if(sequence != null) {
					return GetChildElementCompletion(sequence.Items, prefix);
				} else if (choice != null) {
					return GetChildElementCompletion(choice.Items, prefix);
				}
			}
			return new XmlCompletionItemCollection();
		}
		
		XmlCompletionItemCollection GetChildElementCompletion(XmlSchemaComplexContentRestriction restriction, string prefix)
		{
			// Add any elements.
			if (restriction.Particle != null) {
				XmlSchemaSequence sequence = restriction.Particle as XmlSchemaSequence;
				XmlSchemaChoice choice = restriction.Particle as XmlSchemaChoice;
				XmlSchemaGroupRef groupRef = restriction.Particle as XmlSchemaGroupRef;
				
				if(sequence != null) {
					return GetChildElementCompletion(sequence.Items, prefix);
				} else if (choice != null) {
					return GetChildElementCompletion(choice.Items, prefix);
				} else if (groupRef != null) {
					return GetChildElementCompletion(groupRef, prefix);
				}
			}
			return new XmlCompletionItemCollection();
		}
		
		/// <summary>
		/// Adds an element completion data to the collection if it does not
		/// already exist.
		/// </summary>
		static void AddElement(XmlCompletionItemCollection completionItems, string name, string prefix, string documentation)
		{
			if (!completionItems.Contains(name)) {
				if (prefix.Length > 0) {
					name = String.Concat(prefix, ":", name);
				}
				XmlCompletionItem item = new XmlCompletionItem(name, documentation);
				completionItems.Add(item);
			}
		}
		
		static string GetDocumentation(XmlSchemaAnnotation annotation)
		{
			return new SchemaDocumentation(annotation).ToString();
		}
		
		/// <summary>
		/// Adds an element completion data to the collection if it does not
		/// already exist.
		/// </summary>
		static void AddElement(XmlCompletionItemCollection completionItems, string name, string prefix, XmlSchemaAnnotation annotation)
		{
			string documentation = GetDocumentation(annotation);
			AddElement(completionItems, name, prefix, documentation);
		}
		
		/// <summary>
		/// Adds elements to the collection if it does not already exist.
		/// </summary>
		static void AddElements(XmlCompletionItemCollection lhs, XmlCompletionItemCollection rhs)
		{
			foreach (XmlCompletionItem item in rhs) {
				if (!lhs.Contains(item.Text)) {
					lhs.Add(item);
				}
			}
		}
		
		XmlCompletionItemCollection GetAttributeCompletion(XmlSchemaElement element, XmlNamespaceCollection namespacesInScope)
		{
			XmlCompletionItemCollection completionItems = new XmlCompletionItemCollection();
			
			XmlSchemaComplexType complexType = GetElementAsComplexType(element);
			if (complexType != null) {
				completionItems.AddRange(GetAttributeCompletion(complexType, namespacesInScope));
			}
			
			return completionItems;
		}
		
		XmlCompletionItemCollection GetAttributeCompletion(XmlSchemaComplexContentRestriction restriction, XmlNamespaceCollection namespacesInScope)
		{
			XmlCompletionItemCollection completionItems = new XmlCompletionItemCollection();
			
			completionItems.AddRange(GetAttributeCompletion(restriction.Attributes, namespacesInScope));
			completionItems.AddRange(GetBaseComplexTypeAttributeCompletion(restriction.BaseTypeName, namespacesInScope));
			
			return completionItems;
		}
		
		XmlCompletionItemCollection GetAttributeCompletion(XmlSchemaComplexType complexType, XmlNamespaceCollection namespacesInScope)
		{
			XmlCompletionItemCollection completionItems = GetAttributeCompletion(complexType.Attributes, namespacesInScope);

			// Add any complex content attributes.
			XmlSchemaComplexContent complexContent = complexType.ContentModel as XmlSchemaComplexContent;
			if (complexContent != null) {
				XmlSchemaComplexContentExtension extension = complexContent.Content as XmlSchemaComplexContentExtension;
				XmlSchemaComplexContentRestriction restriction = complexContent.Content as XmlSchemaComplexContentRestriction;
				if (extension != null) {
					completionItems.AddRange(GetAttributeCompletion(extension, namespacesInScope));
				} else if (restriction != null) {
					completionItems.AddRange(GetAttributeCompletion(restriction, namespacesInScope));
				}
			} else {
				XmlSchemaSimpleContent simpleContent = complexType.ContentModel as XmlSchemaSimpleContent;
				if (simpleContent != null) {
					completionItems.AddRange(GetAttributeCompletion(simpleContent, namespacesInScope));
				}
			}
			
			return completionItems;
		}
		
		XmlCompletionItemCollection GetAttributeCompletion(XmlSchemaComplexContentExtension extension, XmlNamespaceCollection namespacesInScope)
		{
			XmlCompletionItemCollection completionItems = new XmlCompletionItemCollection();
			
			completionItems.AddRange(GetAttributeCompletion(extension.Attributes, namespacesInScope));
			completionItems.AddRange(GetBaseComplexTypeAttributeCompletion(extension.BaseTypeName, namespacesInScope));
			
			return completionItems;
		}
		
		XmlCompletionItemCollection GetAttributeCompletion(XmlSchemaSimpleContent simpleContent, XmlNamespaceCollection namespacesInScope)
		{
			XmlCompletionItemCollection completionItems = new XmlCompletionItemCollection();
			
			XmlSchemaSimpleContentExtension extension = simpleContent.Content as XmlSchemaSimpleContentExtension;
			if (extension != null) {
				completionItems.AddRange(GetAttributeCompletion(extension, namespacesInScope));
			}
			
			return completionItems;
		}
		
		XmlCompletionItemCollection GetAttributeCompletion(XmlSchemaSimpleContentExtension extension, XmlNamespaceCollection namespacesInScope)
		{
			XmlCompletionItemCollection completionItems = new XmlCompletionItemCollection();
			
			completionItems.AddRange(GetAttributeCompletion(extension.Attributes, namespacesInScope));
			completionItems.AddRange(GetBaseComplexTypeAttributeCompletion(extension.BaseTypeName, namespacesInScope));
			
			return completionItems;
		}
		
		XmlCompletionItemCollection GetBaseComplexTypeAttributeCompletion(XmlQualifiedName baseTypeName, XmlNamespaceCollection namespacesInScope)
		{
			XmlSchemaComplexType baseComplexType = FindNamedType(schema, baseTypeName);
			if (baseComplexType != null) {
				return GetAttributeCompletion(baseComplexType, namespacesInScope);
			}
			return new XmlCompletionItemCollection();
		}
		
		XmlCompletionItemCollection GetAttributeCompletion(XmlSchemaObjectCollection attributes, XmlNamespaceCollection namespacesInScope)
		{
			XmlCompletionItemCollection completionItems = new XmlCompletionItemCollection();
			
			foreach (XmlSchemaObject schemaObject in attributes) {
				XmlSchemaAttribute attribute = schemaObject as XmlSchemaAttribute;
				XmlSchemaAttributeGroupRef attributeGroupRef = schemaObject as XmlSchemaAttributeGroupRef;
				if (attribute != null) {
					if (!IsProhibitedAttribute(attribute)) {
						AddAttribute(completionItems, attribute, namespacesInScope);
					} else {
						prohibitedAttributes.Add(attribute);
					}
				} else if (attributeGroupRef != null) {
					completionItems.AddRange(GetAttributeCompletion(attributeGroupRef, namespacesInScope));
				}
			}
			return completionItems;
		}
		
		/// <summary>
		/// Checks that the attribute is prohibited or has been flagged
		/// as prohibited previously.
		/// </summary>
		bool IsProhibitedAttribute(XmlSchemaAttribute attribute)
		{
			if (attribute.Use == XmlSchemaUse.Prohibited) {
				return true;
			} else {
				foreach (XmlSchemaAttribute prohibitedAttribute in prohibitedAttributes) {
					if (prohibitedAttribute.QualifiedName == attribute.QualifiedName) {
						return true;
					}
				}
			}
			return false;
		}
		
		/// <summary>
		/// Adds an attribute to the completion data collection.
		/// </summary>
		/// <remarks>
		/// Note the special handling of xml:lang attributes.
		/// </remarks>
		void AddAttribute(XmlCompletionItemCollection completionItems, XmlSchemaAttribute attribute, XmlNamespaceCollection namespacesInScope)
		{
			string name = attribute.Name;
			if (name == null) {
				if (attribute.RefName.Namespace == "http://www.w3.org/XML/1998/namespace") {
					name = String.Concat("xml:", attribute.RefName.Name);
				} else {
					string prefix = namespacesInScope.GetPrefix(attribute.RefName.Namespace);
					if (!String.IsNullOrEmpty(prefix)) {
						name = String.Concat(prefix, ":", attribute.RefName.Name);
					}
				}
			}
			
			if (name != null) {
				string documentation = GetDocumentation(attribute.Annotation);
				XmlCompletionItem item = new XmlCompletionItem(name, documentation, XmlCompletionItemType.XmlAttribute);
				completionItems.Add(item);
			}
		}
		
		/// <summary>
		/// Gets attribute completion data from a group ref.
		/// </summary>
		XmlCompletionItemCollection GetAttributeCompletion(XmlSchemaAttributeGroupRef groupRef, XmlNamespaceCollection namespacesInScope)
		{
			XmlSchemaAttributeGroup attributeGroup = FindAttributeGroup(schema, groupRef.RefName.Name);
			if (attributeGroup != null) {
				return GetAttributeCompletion(attributeGroup.Attributes, namespacesInScope);
			}
			return new XmlCompletionItemCollection();
		}
		
		static XmlSchemaComplexType FindNamedType(XmlSchema schema, XmlQualifiedName name)
		{
			if (name != null) {
				foreach (XmlSchemaObject schemaObject in schema.Items) {
					XmlSchemaComplexType complexType = schemaObject as XmlSchemaComplexType;
					if (complexType != null) {
						if (complexType.QualifiedName == name) {
							return complexType;
						}
					}
				}
				
				// Try included schemas.
				foreach (XmlSchemaExternal external in schema.Includes) {
					XmlSchemaInclude include = external as XmlSchemaInclude;
					if (include != null) {
						if (include.Schema != null) {
							XmlSchemaComplexType matchedComplexType = FindNamedType(include.Schema, name);
							if (matchedComplexType != null) {
								return matchedComplexType;
							}
						}
					}
				}
			}
			return null;
		}
		
		/// <summary>
		/// Finds an element that matches the specified <paramref name="name"/>
		/// from the children of the given <paramref name="element"/>.
		/// </summary>
		XmlSchemaElement FindChildElement(XmlSchemaElement element, QualifiedName name)
		{
			XmlSchemaComplexType complexType = GetElementAsComplexType(element);
			if (complexType != null) {
				return FindChildElement(complexType, name);
			}
			return null;
		}
		
		XmlSchemaElement FindChildElement(XmlSchemaComplexType complexType, QualifiedName name)
		{
			XmlSchemaSequence sequence = complexType.Particle as XmlSchemaSequence;
			XmlSchemaChoice choice = complexType.Particle as XmlSchemaChoice;
			XmlSchemaGroupRef groupRef = complexType.Particle as XmlSchemaGroupRef;
			XmlSchemaAll all = complexType.Particle as XmlSchemaAll;
			XmlSchemaComplexContent complexContent = complexType.ContentModel as XmlSchemaComplexContent;
			
			if (sequence != null) {
				return FindElement(sequence.Items, name);
			} else if (choice != null) {
				return FindElement(choice.Items, name);
			} else if (complexContent != null) {
				XmlSchemaComplexContentExtension extension = complexContent.Content as XmlSchemaComplexContentExtension;
				XmlSchemaComplexContentRestriction restriction = complexContent.Content as XmlSchemaComplexContentRestriction;
				if (extension != null) {
					return FindChildElement(extension, name);
				} else if (restriction != null) {
					return FindChildElement(restriction, name);
				}
			} else if (groupRef != null) {
				return FindElement(groupRef, name);
			} else if (all != null) {
				return FindElement(all.Items, name);
			}
			return null;
		}
		
		/// <summary>
		/// Finds the named child element contained in the extension element.
		/// </summary>
		XmlSchemaElement FindChildElement(XmlSchemaComplexContentExtension extension, QualifiedName name)
		{
			XmlSchemaComplexType complexType = FindNamedType(schema, extension.BaseTypeName);
			if (complexType != null) {
				XmlSchemaElement matchedElement = FindChildElement(complexType, name);
				if (matchedElement == null) {
					XmlSchemaSequence sequence = extension.Particle as XmlSchemaSequence;
					XmlSchemaChoice choice = extension.Particle as XmlSchemaChoice;
					XmlSchemaGroupRef groupRef = extension.Particle as XmlSchemaGroupRef;
					
					if (sequence != null) {
						return FindElement(sequence.Items, name);
					} else if (choice != null) {
						return FindElement(choice.Items, name);
					} else if (groupRef != null) {
						return FindElement(groupRef, name);
					}
				} else {
					return matchedElement;
				}
			}
			return null;
		}
		
		/// <summary>
		/// Finds the named child element contained in the restriction element.
		/// </summary>
		XmlSchemaElement FindChildElement(XmlSchemaComplexContentRestriction restriction, QualifiedName name)
		{
			XmlSchemaSequence sequence = restriction.Particle as XmlSchemaSequence;
			XmlSchemaGroupRef groupRef = restriction.Particle as XmlSchemaGroupRef;
			if (sequence != null) {
				return FindElement(sequence.Items, name);
			} else if (groupRef != null) {
				return FindElement(groupRef, name);
			}
			return null;
		}
		
		/// <summary>
		/// Finds the element in the collection of schema objects.
		/// </summary>
		XmlSchemaElement FindElement(XmlSchemaObjectCollection items, QualifiedName name)
		{
			foreach (XmlSchemaObject schemaObject in items) {
				XmlSchemaElement element = schemaObject as XmlSchemaElement;
				XmlSchemaSequence sequence = schemaObject as XmlSchemaSequence;
				XmlSchemaChoice choice = schemaObject as XmlSchemaChoice;
				XmlSchemaGroupRef groupRef = schemaObject as XmlSchemaGroupRef;
				
				XmlSchemaElement matchedElement = null;
				
				if (element != null) {
					if (element.Name != null) {
						if (name.Name == element.Name) {
							return element;
						}
					} else if (element.RefName != null) {
						if (name.Name == element.RefName.Name) {
							matchedElement = FindElement(element.RefName);
						} else {
							// Abstract element?
							XmlSchemaElement abstractElement = FindElement(element.RefName);
							if (abstractElement != null && abstractElement.IsAbstract) {
								matchedElement = FindSubstitutionGroupElement(abstractElement.QualifiedName, name);
							}
						}
					}
				} else if (sequence != null) {
					matchedElement = FindElement(sequence.Items, name);
				} else if (choice != null) {
					matchedElement = FindElement(choice.Items, name);
				} else if (groupRef != null) {
					matchedElement = FindElement(groupRef, name);
				}
				
				// Did we find a match?
				if (matchedElement != null) {
					return matchedElement;
				}
			}
			return null;
		}
		
		XmlSchemaElement FindElement(XmlSchemaGroupRef groupRef, QualifiedName name)
		{
			XmlSchemaGroup schemaGroup = FindGroup(groupRef.RefName.Name);
			if (schemaGroup != null) {
				XmlSchemaSequence sequence = schemaGroup.Particle as XmlSchemaSequence;
				XmlSchemaChoice choice = schemaGroup.Particle as XmlSchemaChoice;
				
				if(sequence != null) {
					return FindElement(sequence.Items, name);
				} else if (choice != null) {
					return FindElement(choice.Items, name);
				}
			}
			return null;
		}
		
		static XmlSchemaAttributeGroup FindAttributeGroup(XmlSchema schema, string name)
		{
			if (name != null) {
				foreach (XmlSchemaObject schemaObject in schema.Items) {
					
					XmlSchemaAttributeGroup attributeGroup = schemaObject as XmlSchemaAttributeGroup;
					if (attributeGroup != null) {
						if (attributeGroup.Name == name) {
							return attributeGroup;
						}
					}
				}
				
				// Try included schemas.
				foreach (XmlSchemaExternal external in schema.Includes) {
					XmlSchemaInclude include = external as XmlSchemaInclude;
					if (include != null) {
						if (include.Schema != null) {
							return FindAttributeGroup(include.Schema, name);
						}
					}
				}
			}
			return null;
		}
		
		XmlCompletionItemCollection GetAttributeValueCompletion(XmlSchemaElement element, string name)
		{
			XmlCompletionItemCollection completionItems = new XmlCompletionItemCollection();
			
			XmlSchemaComplexType complexType = GetElementAsComplexType(element);
			if (complexType != null) {
				XmlSchemaAttribute attribute = FindAttribute(complexType, name);
				if (attribute != null) {
					completionItems.AddRange(GetAttributeValueCompletion(attribute));
				}
			}
			
			return completionItems;
		}
		
		XmlCompletionItemCollection GetAttributeValueCompletion(XmlSchemaAttribute attribute)
		{
			XmlCompletionItemCollection completionItems = new XmlCompletionItemCollection();
			
			if (attribute.SchemaType != null) {
				XmlSchemaSimpleTypeRestriction simpleTypeRestriction = attribute.SchemaType.Content as XmlSchemaSimpleTypeRestriction;
				if (simpleTypeRestriction != null) {
					completionItems.AddRange(GetAttributeValueCompletion(simpleTypeRestriction));
				}
			} else if (attribute.AttributeSchemaType != null) {
				XmlSchemaSimpleType simpleType = attribute.AttributeSchemaType as XmlSchemaSimpleType;
				
				if (simpleType != null) {
					if (simpleType.Datatype.TypeCode == XmlTypeCode.Boolean) {
						completionItems.AddRange(GetBooleanAttributeValueCompletion());
					} else {
						completionItems.AddRange(GetAttributeValueCompletion(simpleType));
					}
				}
			}
			
			return completionItems;
		}
		
		static XmlCompletionItemCollection GetAttributeValueCompletion(XmlSchemaSimpleTypeRestriction simpleTypeRestriction)
		{
			XmlCompletionItemCollection completionItems = new XmlCompletionItemCollection();
			
			foreach (XmlSchemaObject schemaObject in simpleTypeRestriction.Facets) {
				XmlSchemaEnumerationFacet enumFacet = schemaObject as XmlSchemaEnumerationFacet;
				if (enumFacet != null) {
					AddAttributeValue(completionItems, enumFacet.Value, enumFacet.Annotation);
				}
			}

			return completionItems;
		}
		
		XmlCompletionItemCollection GetAttributeValueCompletion(XmlSchemaSimpleTypeUnion union)
		{
			XmlCompletionItemCollection completionItems = new XmlCompletionItemCollection();
			
			foreach (XmlSchemaObject schemaObject in union.BaseTypes) {
				XmlSchemaSimpleType simpleType = schemaObject as XmlSchemaSimpleType;
				if (simpleType != null) {
					completionItems.AddRange(GetAttributeValueCompletion(simpleType));
				}
			}
			
			if (union.BaseMemberTypes != null) {
				foreach (XmlSchemaSimpleType simpleType in union.BaseMemberTypes) {
					completionItems.AddRange(GetAttributeValueCompletion(simpleType));
				}
			}
			
			return completionItems;
		}
		
		XmlCompletionItemCollection GetAttributeValueCompletion(XmlSchemaSimpleType simpleType)
		{
			XmlCompletionItemCollection completionItems = new XmlCompletionItemCollection();
			
			XmlSchemaSimpleTypeRestriction simpleTypeRestriction = simpleType.Content as XmlSchemaSimpleTypeRestriction;
			XmlSchemaSimpleTypeUnion union = simpleType.Content as XmlSchemaSimpleTypeUnion;
			XmlSchemaSimpleTypeList list = simpleType.Content as XmlSchemaSimpleTypeList;
			
			if (simpleTypeRestriction != null) {
				completionItems.AddRange(GetAttributeValueCompletion(simpleTypeRestriction));
			} else if (union != null) {
				completionItems.AddRange(GetAttributeValueCompletion(union));
			} else if (list != null) {
				completionItems.AddRange(GetAttributeValueCompletion(list));
			}

			return completionItems;
		}
		
		XmlCompletionItemCollection GetAttributeValueCompletion(XmlSchemaSimpleTypeList list)
		{
			XmlCompletionItemCollection completionItems = new XmlCompletionItemCollection();
			
			if (list.ItemType != null) {
				completionItems.AddRange(GetAttributeValueCompletion(list.ItemType));
			} else if (list.ItemTypeName != null) {
				XmlSchemaSimpleType simpleType = FindSimpleType(list.ItemTypeName);
				if (simpleType != null) {
					completionItems.AddRange(GetAttributeValueCompletion(simpleType));
				}
			}
			
			return completionItems;
		}
		
		/// <summary>
		/// Gets the set of attribute values for an xs:boolean type.
		/// </summary>
		static XmlCompletionItemCollection GetBooleanAttributeValueCompletion()
		{
			XmlCompletionItemCollection completionItems = new XmlCompletionItemCollection();
			
			AddAttributeValue(completionItems, "0");
			AddAttributeValue(completionItems, "1");
			AddAttributeValue(completionItems, "true");
			AddAttributeValue(completionItems, "false");
			
			return completionItems;
		}
		
		XmlSchemaAttribute FindAttribute(XmlSchemaComplexType complexType, string name)
		{
			XmlSchemaAttribute matchedAttribute = FindAttribute(complexType.Attributes, name);
			if (matchedAttribute == null) {
				XmlSchemaComplexContent complexContent = complexType.ContentModel as XmlSchemaComplexContent;
				if (complexContent != null) {
					return FindAttribute(complexContent, name);
				}
			}
			return matchedAttribute;
		}
		
		XmlSchemaAttribute FindAttribute(XmlSchemaObjectCollection schemaObjects, string name)
		{
			foreach (XmlSchemaObject schemaObject in schemaObjects) {
				XmlSchemaAttribute attribute = schemaObject as XmlSchemaAttribute;
				XmlSchemaAttributeGroupRef groupRef = schemaObject as XmlSchemaAttributeGroupRef;
				
				if (attribute != null) {
					if (attribute.Name == name) {
						return attribute;
					}
				} else if (groupRef != null) {
					XmlSchemaAttribute matchedAttribute = FindAttribute(groupRef, name);
					if (matchedAttribute != null) {
						return matchedAttribute;
					}
				}
			}
			return null;
		}
		
		XmlSchemaAttribute FindAttribute(XmlSchemaAttributeGroupRef groupRef, string name)
		{
			if (groupRef.RefName != null) {
				XmlSchemaAttributeGroup attributeGroup = FindAttributeGroup(schema, groupRef.RefName.Name);
				if (attributeGroup != null) {
					return FindAttribute(attributeGroup.Attributes, name);
				}
			}
			return null;
		}
		
		XmlSchemaAttribute FindAttribute(XmlSchemaComplexContent complexContent, string name)
		{
			XmlSchemaComplexContentExtension extension = complexContent.Content as XmlSchemaComplexContentExtension;
			XmlSchemaComplexContentRestriction restriction = complexContent.Content as XmlSchemaComplexContentRestriction;
			
			if (extension != null) {
				return FindAttribute(extension, name);
			} else if (restriction != null) {
				return FindAttribute(restriction, name);
			}
			return null;
		}
		
		XmlSchemaAttribute FindAttribute(XmlSchemaComplexContentExtension extension, string name)
		{
			return FindAttribute(extension.Attributes, name);
		}
		
		XmlSchemaAttribute FindAttribute(XmlSchemaComplexContentRestriction restriction, string name)
		{
			XmlSchemaAttribute matchedAttribute = FindAttribute(restriction.Attributes, name);
			if (matchedAttribute == null) {
				XmlSchemaComplexType complexType = FindNamedType(schema, restriction.BaseTypeName);
				if (complexType != null) {
					return FindAttribute(complexType, name);
				}
			}
			return matchedAttribute;
		}
		
		/// <summary>
		/// Adds an attribute value to the completion data collection.
		/// </summary>
		static void AddAttributeValue(XmlCompletionItemCollection completionItems, string valueText)
		{
			XmlCompletionItem item = new XmlCompletionItem(valueText, XmlCompletionItemType.XmlAttributeValue);
			completionItems.Add(item);
		}
		
		/// <summary>
		/// Adds an attribute value to the completion data collection.
		/// </summary>
		static void AddAttributeValue(XmlCompletionItemCollection completionItems, string valueText, XmlSchemaAnnotation annotation)
		{
			string documentation = GetDocumentation(annotation);
			XmlCompletionItem item = new XmlCompletionItem(valueText, documentation, XmlCompletionItemType.XmlAttributeValue);
			completionItems.Add(item);
		}
		
		/// <summary>
		/// Adds an attribute value to the completion data collection.
		/// </summary>
		static void AddAttributeValue(XmlCompletionItemCollection completionItems, string valueText, string description)
		{
			XmlCompletionItem item = new XmlCompletionItem(valueText, description, XmlCompletionItemType.XmlAttributeValue);
			completionItems.Add(item);
		}
		
		XmlSchemaSimpleType FindSimpleType(XmlQualifiedName name)
		{
			foreach (XmlSchemaObject schemaObject in schema.SchemaTypes.Values) {
				XmlSchemaSimpleType simpleType = schemaObject as XmlSchemaSimpleType;
				if (simpleType != null) {
					if (simpleType.QualifiedName == name) {
						return simpleType;
					}
				}
			}
			return null;
		}

		/// <summary>
		/// Adds any elements that have the specified substitution group.
		/// </summary>
		void AddSubstitionGroupElements(XmlCompletionItemCollection completionItems, XmlQualifiedName groupName, string prefix)
		{
			foreach (XmlSchemaElement element in schema.Elements.Values) {
				if (element.SubstitutionGroup == groupName) {
					AddElement(completionItems, element.Name, prefix, element.Annotation);
				}
			}
		}
		
		/// <summary>
		/// Looks for the substitution group element of the specified name.
		/// </summary>
		XmlSchemaElement FindSubstitutionGroupElement(XmlQualifiedName groupName, QualifiedName name)
		{
			foreach (XmlSchemaElement element in schema.Elements.Values) {
				if (element.SubstitutionGroup == groupName) {
					if (element.Name != null) {
						if (element.Name == name.Name) {
							return element;
						}
					}
				}
			}
			return null;
		}
	}
}
