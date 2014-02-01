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
using System.IO;
using Microsoft.Win32;
using ICSharpCode.Core;

namespace ICSharpCode.XmlEditor
{
	public class RegisteredXmlSchemasEditor
	{
		RegisteredXmlSchemas registeredXmlSchemas;
		IXmlSchemasPanel schemasPanel;
		ICollection<string> xmlFileExtensions;
		XmlSchemaFileAssociations associations;
		IXmlSchemaCompletionDataFactory factory;
		bool ignoreNamespacePrefixTextChanges;
		bool schemasChanged;
		XmlSchemaCompletionCollection addedSchemas = new XmlSchemaCompletionCollection();
		List<string> removedSchemaNamespaces = new List<string>();
		
		public RegisteredXmlSchemasEditor(RegisteredXmlSchemas registeredXmlSchemas, 
			ICollection<string> xmlFileExtensions, 
			XmlSchemaFileAssociations associations,
			IXmlSchemasPanel schemasPanel,
			IXmlSchemaCompletionDataFactory factory)
		{
			this.registeredXmlSchemas = registeredXmlSchemas;
			this.xmlFileExtensions = xmlFileExtensions;
			this.associations = associations;
			this.schemasPanel = schemasPanel;
			this.factory = factory;
		}
		
		public void LoadOptions()
		{
			PopulateSchemaList();
			PopulateXmlSchemaFileAssociationList();
			SelectFirstXmlSchemaFileAssociation();
		}
		
		void PopulateSchemaList()
		{
			foreach (XmlSchemaCompletion schema in registeredXmlSchemas.Schemas) {
				XmlSchemaListItem item = new XmlSchemaListItem(schema.NamespaceUri, schema.IsReadOnly);
				schemasPanel.AddXmlSchemaListItem(item);
			}
			schemasPanel.AddXmlSchemaListSortDescription("NamespaceUri", ListSortDirection.Ascending);
		}

		void PopulateXmlSchemaFileAssociationList()
		{			
			foreach (string extension in xmlFileExtensions) {
				XmlSchemaFileAssociation association = associations.GetSchemaFileAssociation(extension);
				XmlSchemaFileAssociationListItem item = new XmlSchemaFileAssociationListItem(extension, association.NamespaceUri, association.NamespacePrefix);
				schemasPanel.AddXmlSchemaFileAssociationListItem(item);
			}			
			schemasPanel.AddXmlSchemaFileAssociationListSortDescription("Extension", ListSortDirection.Ascending);			
		}
		
		void SelectFirstXmlSchemaFileAssociation()
		{
			schemasPanel.SelectedXmlSchemaFileAssociationListItemIndex = 0;	
		}
		
		public void XmlSchemaFileAssociationFileExtensionSelectionChanged()
		{
			ignoreNamespacePrefixTextChanges = true;
			
			XmlSchemaFileAssociationListItem association = schemasPanel.GetSelectedXmlSchemaFileAssociationListItem();
			schemasPanel.SetSelectedSchemaNamespace(association.NamespaceUri);
			schemasPanel.SetSelectedSchemaNamespacePrefix(association.NamespacePrefix);

			ignoreNamespacePrefixTextChanges = false;
		}
		
		public void SchemaListSelectionChanged()
		{
			bool removeSchemaButtonEnabled = false;
			XmlSchemaListItem item = schemasPanel.GetSelectedXmlSchemaListItem();
			if (item != null) {
				removeSchemaButtonEnabled = !item.ReadOnly;
			}
			schemasPanel.RemoveSchemaButtonEnabled = removeSchemaButtonEnabled;
		}
		
		public bool XmlFileAssociationsChanged {
			get {
				for (int i = 0; i < schemasPanel.XmlSchemaFileAssociationListItemCount; ++i) {
					XmlSchemaFileAssociationListItem item = schemasPanel.GetXmlSchemaFileAssociationListItem(i);
					if (item.IsDirty) {
						return true;
					}
				}
				return false;
			}
		}
		
		public bool XmlSchemasChanged {
			get { return schemasChanged; }
		}
		
		public void SchemaNamespacePrefixChanged()
		{
			if (!ignoreNamespacePrefixTextChanges) {
				XmlSchemaFileAssociationListItem item = schemasPanel.GetSelectedXmlSchemaFileAssociationListItem();
				if (item != null) {
					item.NamespacePrefix = schemasPanel.GetSelectedSchemaNamespacePrefix();
					item.IsDirty = true;
				}
			}
		}
		
		/// <summary>
		/// Saves any changes to the configured schemas.
		/// </summary>
		public bool SaveOptions()
		{
			if (schemasChanged) {
				try {
					SaveSchemaChanges();
				} catch (Exception ex) {
					schemasPanel.ShowExceptionError(ex, "${res:ICSharpCode.XmlEditor.XmlSchemasPanel.UnableToSaveChanges}");
					return false;
				}
			}

			UpdateXmlSchemaFileAssociations();
		
			return true;
		}
		
		void UpdateXmlSchemaFileAssociations()
		{
			for (int i = 0; i < schemasPanel.XmlSchemaFileAssociationListItemCount; ++i) {
				XmlSchemaFileAssociationListItem item = schemasPanel.GetXmlSchemaFileAssociationListItem(i);
				if (item.IsDirty) {
					XmlSchemaFileAssociation association = new XmlSchemaFileAssociation(item.Extension, item.NamespaceUri, item.NamespacePrefix);
					associations.SetSchemaFileAssociation(association);
				}
			}
		}
		
		void SaveSchemaChanges()
		{
			RemoveSchemasFromRegisteredSchemas();
			AddNewSchemasToRegisteredSchemas();
		}

		void RemoveSchemasFromRegisteredSchemas()
		{
			while (removedSchemaNamespaces.Count > 0) {
				registeredXmlSchemas.RemoveUserDefinedSchema(removedSchemaNamespaces[0]);
				removedSchemaNamespaces.RemoveAt(0);
			}
		}		

		void AddNewSchemasToRegisteredSchemas()
		{
			while (addedSchemas.Count > 0) {
				registeredXmlSchemas.AddUserSchema(addedSchemas[0]);
				addedSchemas.RemoveAt(0);
			}
		}
		
		public void AddSchemaFromFileSystem()
		{
			try {
				string schemaFileName = BrowseForSchema();
				if (!String.IsNullOrEmpty(schemaFileName)) {
					
					XmlSchemaCompletion schema = LoadSchema(schemaFileName);
					
					if (IsSchemaMissingNamespace(schema)) {
						ShowSchemaMissingNamespaceErrorMessage(schemaFileName);
						return;
					}
					
					if (SchemaNamespaceExists(schema.NamespaceUri)) {	
						ShowSchemaNamespaceAlreadyExistsErrorMessage(schema.NamespaceUri);
						return;
					}

					AddSchema(schema);
					SortSchemas();
					ScrollSelectedSchemaItemIntoView();
				}
			} catch (Exception ex) {
				schemasPanel.ShowError("${res:ICSharpCode.XmlEditor.XmlSchemasPanel.UnableToAddSchema}\n\n" + ex.Message);
			}
		}
		
		string BrowseForSchema()
		{			
			OpenFileDialog openFileDialog  = new OpenFileDialog();
			openFileDialog.Filter = StringParser.Parse("${res:SharpDevelop.FileFilter.XmlSchemaFiles}|*.xsd|${res:SharpDevelop.FileFilter.AllFiles}|*.*");
			
			if (schemasPanel.ShowDialog(openFileDialog) == true) {
				return openFileDialog.FileName;
			}
			return null;
		}
		
		bool IsSchemaMissingNamespace(XmlSchemaCompletion schema)
		{
			return !schema.HasNamespaceUri;
		}
		
		XmlSchemaCompletion LoadSchema(string fileName)
		{
			string baseUri = XmlSchemaCompletion.GetUri(fileName);
			return factory.CreateXmlSchemaCompletionData(baseUri, fileName);
		}

		void ShowSchemaMissingNamespaceErrorMessage(string fileName)
		{
			schemasPanel.ShowErrorFormatted("${res:ICSharpCode.XmlEditor.XmlSchemasPanel.NoTargetNamespace}", Path.GetFileName(fileName));
		}
		
		bool SchemaNamespaceExists(string namespaceUri)
		{
			if ((!registeredXmlSchemas.SchemaExists(namespaceUri)) &&
			    (addedSchemas[namespaceUri] == null)){
				return false;
			} 
			return !removedSchemaNamespaces.Contains(namespaceUri);
		}
		
		void ShowSchemaNamespaceAlreadyExistsErrorMessage(string namespaceUri)
		{
			schemasPanel.ShowErrorFormatted("${res:ICSharpCode.XmlEditor.XmlSchemasPanel.NamespaceExists}", namespaceUri);			
		}
		
		/// <summary>
		/// Loads the specified schema and adds it to an internal collection.
		/// </summary>
		/// <remarks>The schema file is not copied to the user's schema folder
		/// until they click the OK button.</remarks>
		void AddSchema(XmlSchemaCompletion schema)
		{
			XmlSchemaListItem schemaListItem = new XmlSchemaListItem(schema.NamespaceUri, false);
			int index = schemasPanel.AddXmlSchemaListItem(schemaListItem);
			schemasPanel.SelectedXmlSchemaListItemIndex = index;
			addedSchemas.Add(schema);
			
			schemasChanged = true;
		}
		
		void SortSchemas()
		{
			schemasPanel.RefreshXmlSchemaListItems();
		}
		
		void ScrollSelectedSchemaItemIntoView()
		{
			schemasPanel.ScrollXmlSchemaListItemIntoView(schemasPanel.GetSelectedXmlSchemaListItem());
		}

		public void RemoveSelectedSchema()
		{
			XmlSchemaListItem item = schemasPanel.GetSelectedXmlSchemaListItem();
			if (item != null) {
				RemoveSchema(item.NamespaceUri);
			}
		}
		
		void RemoveSchema(string namespaceUri)
		{
			RemoveSchemaListItem(namespaceUri);
			
			XmlSchemaCompletion addedSchema = addedSchemas[namespaceUri];
			if (addedSchema != null) {
				addedSchemas.Remove(addedSchema);
			} else {
				removedSchemaNamespaces.Add(namespaceUri);
			}
			schemasChanged = true;
		}
		
		void RemoveSchemaListItem(string namespaceUri)
		{		
			for (int i = 0; i < schemasPanel.XmlSchemaListItemCount; ++i) {
				XmlSchemaListItem item = schemasPanel.GetXmlSchemaListItem(i);
				if (item.NamespaceUri == namespaceUri) {
					schemasPanel.RemoveXmlSchemaListItem(item);
					break;
				}
			}
		}
		
		public void ChangeSchemaAssociation()
		{
			string[] namespaces = GetSchemaListNamespaces();
			SelectXmlSchemaWindow window = new SelectXmlSchemaWindow(namespaces);
			window.SelectedNamespaceUri = schemasPanel.GetSelectedSchemaNamespace();
			if (schemasPanel.ShowDialog(window) == true) {
				schemasPanel.SetSelectedSchemaNamespace(window.SelectedNamespaceUri);
				XmlSchemaFileAssociationListItem item = schemasPanel.GetSelectedXmlSchemaFileAssociationListItem();
				item.NamespaceUri = window.SelectedNamespaceUri;
				item.IsDirty = true;
			}
		}
		
		string[] GetSchemaListNamespaces()
		{
			List<string> namespaces = new List<string>();
			for (int i = 0; i < schemasPanel.XmlSchemaListItemCount; ++i) {
				namespaces.Add(schemasPanel.GetXmlSchemaListItem(i).NamespaceUri);
			}
			return namespaces.ToArray();
		}		
	}
}
