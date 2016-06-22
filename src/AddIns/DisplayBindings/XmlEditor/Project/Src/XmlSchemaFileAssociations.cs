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
