// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.IO;
using ICSharpCode.Core;

namespace ICSharpCode.XmlEditor
{
	public class XmlSchemaFileAssociations
	{
		Properties properties;
		DefaultXmlSchemaFileAssociations defaultSchemaFileAssociations;
		XmlSchemaCompletionCollection schemas;
		
		public XmlSchemaFileAssociations(Properties properties, 
			DefaultXmlSchemaFileAssociations defaultSchemaFileAssociations,
			XmlSchemaCompletionCollection schemas)
		{
			this.properties = properties;
			this.defaultSchemaFileAssociations = defaultSchemaFileAssociations;
			this.schemas = schemas;
		}
		
		public XmlSchemaCompletionCollection Schemas {
			get { return schemas; }
		}
		
		/// <summary>
		/// Gets an association between a schema and a file extension.
		/// </summary>
		public XmlSchemaFileAssociation GetSchemaFileAssociation(string fileName)
		{
			string extension = Path.GetExtension(fileName).ToLowerInvariant();
			string property = properties.Get("ext" + extension, String.Empty);
			XmlSchemaFileAssociation schemaFileAssociation = XmlSchemaFileAssociation.ConvertFromString(property);
			if (schemaFileAssociation.IsEmpty) {
				return defaultSchemaFileAssociations.Find(extension);
			}
			return schemaFileAssociation;
		}
		
		public XmlSchemaCompletion GetSchemaCompletion(string fileName)
		{
			XmlSchemaFileAssociation association = GetSchemaFileAssociation(fileName);
			XmlSchemaCompletion schema = schemas[association.NamespaceUri];
			if (schema != null) {
				schema.DefaultNamespacePrefix = association.NamespacePrefix;
			}
			return schema;
		}
		
		public string GetNamespacePrefix(string fileName)
		{
			return GetSchemaFileAssociation(fileName).NamespacePrefix;
		}
		
		public void SetSchemaFileAssociation(XmlSchemaFileAssociation association)
		{
			properties.Set("ext" + association.FileExtension, association.ToString());
		}
	}
}
