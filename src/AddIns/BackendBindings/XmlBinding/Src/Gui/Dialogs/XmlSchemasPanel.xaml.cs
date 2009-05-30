using System;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Shapes;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop;
using ICSharpCode.XmlEditor;
using Microsoft.Win32;

namespace ICSharpCode.XmlEditor.Gui.Dialogs
{
	/// <summary>
	/// Interaction logic for XmlSchemasPanel.xaml
	/// </summary>
	public partial class XmlSchemasPanel : UserControl, IOptionPanel
	{
		public XmlSchemasPanel()
		{
			InitializeComponent();
		}
			
		bool ignoreNamespacePrefixTextChanges;
		bool changed;
		XmlSchemaCompletionDataCollection addedSchemas = new XmlSchemaCompletionDataCollection();
		StringCollection removedSchemaNamespaces = new StringCollection();
		
		public object Owner { get; set; }
		
		public object Control {
			get {
				return this;
			}
		}
		
		public void LoadOptions()
		{
			PopulateSchemaListBox();
			PopulateFileExtensionComboBox();
		}
		
		/// <summary>
		/// Saves any changes to the configured schemas.
		/// </summary>
		public bool SaveOptions()
		{
			if (changed) {
				try {
					return SaveSchemaChanges();
				} catch (Exception ex) {
					MessageService.ShowError(ex, "${res:ICSharpCode.XmlEditor.XmlSchemasPanel.UnableToSaveChanges}");
					return false;
				}
			}

			// Update schema associations after we have added any new schemas to
			// the schema manager.
			UpdateSchemaAssociations();
		
			return true;
		}
		
		/// <summary>
		/// Updates the configured file extension to namespace mappings.
		/// </summary>
		void UpdateSchemaAssociations()
		{
			foreach (XmlSchemaAssociationListBoxItem item in fileExtensionComboBox.Items) {
				if (item.IsDirty) {
					XmlSchemaAssociation association = new XmlSchemaAssociation(item.Extension, item.NamespaceUri, item.NamespacePrefix);
					XmlEditorAddInOptions.SetSchemaAssociation(association);
				}
			}
		}
		
		/// <summary>
		/// Adds the schema namespaces to the list.
		/// </summary>
		void PopulateSchemaListBox()
		{
			foreach (XmlSchemaCompletionData schema in XmlSchemaManager.SchemaCompletionDataItems) {
				XmlSchemaListBoxItem item = new XmlSchemaListBoxItem(schema.NamespaceUri, schema.ReadOnly);
				schemaListBox.Items.Add(item);
			}
			
			schemaListBox.Items.SortDescriptions.Add(new SortDescription("NamespaceUri", ListSortDirection.Ascending));
		}
		
		/// <summary>
		/// Reads the configured xml file extensions and their associated namespaces.
		/// </summary>
		void PopulateFileExtensionComboBox()
		{
			string [] extensions = XmlDisplayBinding.GetXmlFileExtensions();
			
			foreach (string extension in extensions) {
				XmlSchemaAssociation association = XmlEditorAddInOptions.GetSchemaAssociation(extension);
				XmlSchemaAssociationListBoxItem item = new XmlSchemaAssociationListBoxItem(association.Extension, association.NamespaceUri, association.NamespacePrefix);
				fileExtensionComboBox.Items.Add(item);
			}
			
			fileExtensionComboBox.Items.SortDescriptions.Add(new SortDescription("Extension", ListSortDirection.Ascending));
			
			if (fileExtensionComboBox.Items.Count > 0) {
				fileExtensionComboBox.SelectedIndex = 0;
				FileExtensionComboBoxSelectionChanged(this, null);
			}
		}
		
		/// <summary>
		/// Shows the namespace associated with the selected xml file extension.
		/// </summary>
		void FileExtensionComboBoxSelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			schemaTextBox.Text = string.Empty;
			ignoreNamespacePrefixTextChanges = true;
			namespacePrefixTextBox.Text = string.Empty;
			
			try {
				XmlSchemaAssociationListBoxItem association = fileExtensionComboBox.SelectedItem as XmlSchemaAssociationListBoxItem;
				if (association != null) {
					schemaTextBox.Text = association.NamespaceUri;
					namespacePrefixTextBox.Text = association.NamespacePrefix;
				}
			} finally {
				ignoreNamespacePrefixTextChanges = false;			
			}
		}
		
		/// <summary>
		/// Enables the remove button if a list box item is selected.
		/// </summary>
		void SchemaListBoxSelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			XmlSchemaListBoxItem item = schemaListBox.SelectedItem as XmlSchemaListBoxItem;
			if (item != null) {
				if (item.ReadOnly) {
					btnRemove.IsEnabled = false;
				} else {
					btnRemove.IsEnabled = true;
				}
			} else {
				btnRemove.IsEnabled = false;
			}
		}
		
		void BtnAddClick(object sender, RoutedEventArgs e)
		{
			try {
				// Browse for schema.
				
				string schemaFileName = BrowseForSchema();
				
				// Add schema if the namespace does not already exist.
				
				if (schemaFileName.Length > 0) {
					changed = AddSchema(schemaFileName);
				}
			} catch (Exception ex) {
				MessageService.ShowError("${res:ICSharpCode.XmlEditor.XmlSchemasPanel.UnableToAddSchema}\n\n" + ex.Message);
			}
		}
		
		void BtnRemoveClick(object sender, RoutedEventArgs e)
		{
			// Remove selected schema.
			XmlSchemaListBoxItem item = schemaListBox.SelectedItem as XmlSchemaListBoxItem;
			if (item != null) {
				RemoveSchema(item.NamespaceUri);
				changed = true;
			}
		}
		
		/// <summary>
		/// Saves any changes to the configured schemas.
		/// </summary>
		/// <returns></returns>
		bool SaveSchemaChanges()
		{
			RemoveUserSchemas();
			AddUserSchemas();

			return true;
		}
		
		/// <summary>
		/// Allows the user to browse the file system for a schema.
		/// </summary>
		/// <returns>The schema file name the user selected; otherwise an 
		/// empty string.</returns>
		string BrowseForSchema()
		{
			string fileName = String.Empty;
			
			OpenFileDialog openFileDialog  = new OpenFileDialog();
			openFileDialog.CheckFileExists = true;
			openFileDialog.Multiselect = false;
			openFileDialog.Filter = StringParser.Parse("${res:SharpDevelop.FileFilter.XmlSchemaFiles}|*.xsd|${res:SharpDevelop.FileFilter.AllFiles}|*.*");
			
			if (openFileDialog.ShowDialog() ?? false) {
				fileName = openFileDialog.FileName;
			}
			
			return fileName;
		}
		
		/// <summary>
		/// Loads the specified schema and adds it to an internal collection.
		/// </summary>
		/// <remarks>The schema file is not copied to the user's schema folder
		/// until they click the OK button.</remarks>
		/// <returns><see langword="true"/> if the schema namespace 
		/// does not already exist; otherwise <see langword="false"/>
		/// </returns>
		bool AddSchema(string fileName)
		{			
			// Load the schema.
			XmlSchemaCompletionData schema = new XmlSchemaCompletionData(fileName);
			
			// Make sure the schema has a target namespace.
			if (schema.NamespaceUri == null) {
				MessageService.ShowErrorFormatted("${res:ICSharpCode.XmlEditor.XmlSchemasPanel.NoTargetNamespace}", System.IO.Path.GetFileName(schema.FileName));
				return false;
			}
			
			// Check that the schema does not exist.
			if (SchemaNamespaceExists(schema.NamespaceUri)) {	
				MessageService.ShowErrorFormatted("${res:ICSharpCode.XmlEditor.XmlSchemasPanel.NamespaceExists}", schema.NamespaceUri);
				return false;
			} 

			// Store the schema so we can add it later.
			int index = schemaListBox.Items.Add(new XmlSchemaListBoxItem(schema.NamespaceUri));
			schemaListBox.SelectedIndex = index;
			addedSchemas.Add(schema);
			if (removedSchemaNamespaces.Contains(schema.NamespaceUri)) {
				removedSchemaNamespaces.Remove(schema.NamespaceUri);
			}
			
			return true;
		}
		
		/// <summary>
		/// Checks that the schema namespace does not already exist.
		/// </summary>
		bool SchemaNamespaceExists(string namespaceURI)
		{
			bool exists = true;
			if ((XmlSchemaManager.SchemaCompletionDataItems[namespaceURI] == null) &&
			    (addedSchemas[namespaceURI] == null)){
				exists = false;
			} else {
				// Makes sure it has not been flagged for removal.
				exists = !removedSchemaNamespaces.Contains(namespaceURI);
			}
			return exists;
		}
		
		/// <summary>
		/// Schedules the schema for removal.
		/// </summary>
		void RemoveSchema(string namespaceUri)
		{
			RemoveListBoxItem(namespaceUri);
			
			XmlSchemaCompletionData addedSchema = addedSchemas[namespaceUri];
			if (addedSchema != null) {
				addedSchemas.Remove(addedSchema);
			} else {
				removedSchemaNamespaces.Add(namespaceUri);
			}
		}
		
		void RemoveListBoxItem(string namespaceUri)
		{
			foreach (XmlSchemaListBoxItem item in schemaListBox.Items) {
				if (item.NamespaceUri == namespaceUri) {
					schemaListBox.Items.Remove(item);
					break;
				}
			}
		}
		
		/// <summary>
		/// Removes the schemas from the schema manager.
		/// </summary>
		void RemoveUserSchemas()
		{
			while (removedSchemaNamespaces.Count > 0) {
				XmlSchemaManager.RemoveUserSchema(removedSchemaNamespaces[0]);
				removedSchemaNamespaces.RemoveAt(0);
			}
		}
		
		/// <summary>
		/// Adds the schemas to the schema manager.
		/// </summary>
		void AddUserSchemas()
		{
			while (addedSchemas.Count > 0) {
				XmlSchemaManager.AddUserSchema(addedSchemas[0]);
				addedSchemas.RemoveAt(0);
			}
		}
		
		/// <summary>
		/// User has changed the namespace prefix.
		/// </summary>
		void NamespacePrefixTextBoxTextChanged(object sender, TextChangedEventArgs e)
		{
			if (!ignoreNamespacePrefixTextChanges) {
				XmlSchemaAssociationListBoxItem item = fileExtensionComboBox.SelectedItem as XmlSchemaAssociationListBoxItem;
				if (item != null) {
					item.NamespacePrefix = namespacePrefixTextBox.Text;
					item.IsDirty = true;
				}
			}
		}

		/// <summary>
		/// Returns an array of schema namespace strings that will be displayed
		/// when the user chooses to associated a namespace to a file extension
		/// by default.
		/// </summary>
		string[] GetSchemaListBoxNamespaces()
		{
			string[] namespaces = new string[schemaListBox.Items.Count];
			
			for (int i = 0; i < schemaListBox.Items.Count; ++i) {
				XmlSchemaListBoxItem item = (XmlSchemaListBoxItem)schemaListBox.Items[i];
				namespaces[i] = item.NamespaceUri;
			}
			
			return namespaces;
		}
		
		/// <summary>
		/// Allows the user to change the schema associated with an xml file 
		/// extension.
		/// </summary>
		void BtnChangeSchemaClick(object sender, RoutedEventArgs e)
		{
			string[] namespaces = GetSchemaListBoxNamespaces();
			SelectXmlSchema form = new SelectXmlSchema(namespaces);
			form.SelectedNamespaceUri = schemaTextBox.Text;
			if (form.ShowDialog() ?? false) {
				schemaTextBox.Text = form.SelectedNamespaceUri;
				XmlSchemaAssociationListBoxItem item = (XmlSchemaAssociationListBoxItem)fileExtensionComboBox.SelectedItem;
				item.NamespaceUri = form.SelectedNamespaceUri;
				item.IsDirty = true;
			}
		}
	}
}