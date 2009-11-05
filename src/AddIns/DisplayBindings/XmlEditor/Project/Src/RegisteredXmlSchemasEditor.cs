// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

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
		XmlSchemaManager schemaManager;
		IXmlSchemasPanel schemasPanel;
		ICollection<string> xmlFileExtensions;
		XmlEditorOptions xmlEditorOptions;
		IXmlSchemaCompletionDataFactory factory;
		bool ignoreNamespacePrefixTextChanges;
		bool schemasChanged;
		XmlSchemaCompletionDataCollection addedSchemas = new XmlSchemaCompletionDataCollection();
		List<string> removedSchemaNamespaces = new List<string>();
		
		public RegisteredXmlSchemasEditor(XmlSchemaManager schemaManager, 
			ICollection<string> xmlFileExtensions, 
			XmlEditorOptions xmlEditorOptions,
			IXmlSchemasPanel schemasPanel,
			IXmlSchemaCompletionDataFactory factory)
		{
			this.schemaManager = schemaManager;
			this.xmlFileExtensions = xmlFileExtensions;
			this.xmlEditorOptions = xmlEditorOptions;
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
			foreach (XmlSchemaCompletionData schema in schemaManager.Schemas) {
				XmlSchemaListItem item = new XmlSchemaListItem(schema.NamespaceUri, schema.ReadOnly);
				schemasPanel.AddXmlSchemaListItem(item);
			}
			schemasPanel.AddXmlSchemaListSortDescription("NamespaceUri", ListSortDirection.Ascending);
		}

		void PopulateXmlSchemaFileAssociationList()
		{			
			foreach (string extension in xmlFileExtensions) {
				XmlSchemaFileAssociation association = xmlEditorOptions.GetSchemaFileAssociation(extension);
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
					xmlEditorOptions.SetSchemaFileAssociation(association);
				}
			}
		}
		
		void SaveSchemaChanges()
		{
			RemoveSchemasFromSchemaManager();
			AddNewSchemasToSchemaManager();
		}

		void RemoveSchemasFromSchemaManager()
		{
			while (removedSchemaNamespaces.Count > 0) {
				schemaManager.RemoveUserSchema(removedSchemaNamespaces[0]);
				removedSchemaNamespaces.RemoveAt(0);
			}
		}		

		void AddNewSchemasToSchemaManager()
		{
			while (addedSchemas.Count > 0) {
				schemaManager.AddUserSchema(addedSchemas[0]);
				addedSchemas.RemoveAt(0);
			}
		}
		
		public void AddSchemaFromFileSystem()
		{
			try {
				string schemaFileName = BrowseForSchema();
				if (!String.IsNullOrEmpty(schemaFileName)) {
					
					XmlSchemaCompletionData schema = LoadSchema(schemaFileName);
					
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
		
		bool IsSchemaMissingNamespace(XmlSchemaCompletionData schema)
		{
			return !schema.HasNamespaceUri;
		}
		
		XmlSchemaCompletionData LoadSchema(string fileName)
		{
			string baseUri = XmlSchemaCompletionData.GetUri(fileName);
			return factory.CreateXmlSchemaCompletionData(baseUri, fileName);
		}

		void ShowSchemaMissingNamespaceErrorMessage(string fileName)
		{
			schemasPanel.ShowErrorFormatted("${res:ICSharpCode.XmlEditor.XmlSchemasPanel.NoTargetNamespace}", Path.GetFileName(fileName));
		}
		
		bool SchemaNamespaceExists(string namespaceUri)
		{
			if ((!schemaManager.SchemaExists(namespaceUri)) &&
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
		void AddSchema(XmlSchemaCompletionData schema)
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
			
			XmlSchemaCompletionData addedSchema = addedSchemas[namespaceUri];
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
