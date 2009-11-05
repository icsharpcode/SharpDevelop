// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Diagnostics;
using ICSharpCode.Core;
using System.Globalization;

namespace ICSharpCode.XmlEditor
{
	public class XmlEditorOptions
	{
		public static readonly string OptionsProperty = "XmlEditor.AddIn.Options";
		public static readonly string ShowAttributesWhenFoldedPropertyName = "ShowAttributesWhenFolded";
		public static readonly string ShowSchemaAnnotationPropertyName = "ShowSchemaAnnotation";
		
		Properties properties;
		DefaultXmlSchemaFileAssociations defaultSchemaFileAssociations;
		XmlSchemaCompletionDataCollection schemas;
		
		public XmlEditorOptions(Properties properties, 
			DefaultXmlSchemaFileAssociations defaultSchemaFileAssociations,
			XmlSchemaCompletionDataCollection schemas)
		{
			this.properties = properties;
			this.defaultSchemaFileAssociations = defaultSchemaFileAssociations;
			this.schemas = schemas;
		}

		public event PropertyChangedEventHandler PropertyChanged {
			add    { properties.PropertyChanged += value; }
			remove { properties.PropertyChanged -= value; }
		}
		
		/// <summary>
		/// Gets an association between a schema and a file extension.
		/// </summary>
		public XmlSchemaFileAssociation GetSchemaFileAssociation(string extension)
		{			
			extension = extension.ToLowerInvariant();
			string property = properties.Get("ext" + extension, String.Empty);
			XmlSchemaFileAssociation schemaFileAssociation = XmlSchemaFileAssociation.ConvertFromString(property);
			if (schemaFileAssociation.IsEmpty) {
				return defaultSchemaFileAssociations.Find(extension);
			}
			return schemaFileAssociation;
		}
		
		public XmlSchemaCompletionData GetSchemaCompletionData(string fileExtension)
		{
			XmlSchemaFileAssociation association = GetSchemaFileAssociation(fileExtension);
			return schemas[association.NamespaceUri];
		}		
		
		public string GetNamespacePrefix(string fileExtension)
		{
			return GetSchemaFileAssociation(fileExtension).NamespacePrefix;
		}		
		
		public void SetSchemaFileAssociation(XmlSchemaFileAssociation association)
		{
			properties.Set("ext" + association.FileExtension, association.ToString());
		}
		
		public bool ShowAttributesWhenFolded {
			get { return properties.Get(ShowAttributesWhenFoldedPropertyName, false); }
			set {  properties.Set(ShowAttributesWhenFoldedPropertyName, value); }
		}
		
		public bool ShowSchemaAnnotation {
			get { return properties.Get(ShowSchemaAnnotationPropertyName, true); }
			set { properties.Set(ShowSchemaAnnotationPropertyName, value); }
		}
	}
}
