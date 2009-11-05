// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.ComponentModel;
using Microsoft.Win32;

namespace ICSharpCode.XmlEditor
{
	public interface IXmlSchemasPanel
	{
		/// <summary>
		/// SchemaListBox.
		/// </summary>
		XmlSchemaListItem GetXmlSchemaListItem(int index);
		int AddXmlSchemaListItem(XmlSchemaListItem schemaListItem);
		void AddXmlSchemaListSortDescription(string propertyName, ListSortDirection sortDirection);
		XmlSchemaListItem GetSelectedXmlSchemaListItem();
		int XmlSchemaListItemCount { get; }
		void RemoveXmlSchemaListItem(XmlSchemaListItem schemaListItem);
		int SelectedXmlSchemaListItemIndex { get; set; }
		void RefreshXmlSchemaListItems();
		void ScrollXmlSchemaListItemIntoView(XmlSchemaListItem schemaListItem);
		
		/// <summary>
		/// XmlFileAssociationComboBox.
		/// </summary>
		XmlSchemaFileAssociationListItem GetXmlSchemaFileAssociationListItem(int index);
		XmlSchemaFileAssociationListItem GetSelectedXmlSchemaFileAssociationListItem();
		int AddXmlSchemaFileAssociationListItem(XmlSchemaFileAssociationListItem schemaFileAssociationListItem);
		void AddXmlSchemaFileAssociationListSortDescription(string propertyName, ListSortDirection sortDirection);
		int SelectedXmlSchemaFileAssociationListItemIndex { get; set; }
		int XmlSchemaFileAssociationListItemCount { get; }
		
		/// <summary>
		/// SchemaNamespaceTextBox.
		/// </summary>
		string GetSelectedSchemaNamespace();
		void SetSelectedSchemaNamespace(string schemaNamespace);
		
		/// <summary>
		/// SchemaNamespacePrefixTextBox.
		/// </summary>
		string GetSelectedSchemaNamespacePrefix();
		void SetSelectedSchemaNamespacePrefix(string namespacePrefix);
		
		bool RemoveSchemaButtonEnabled { get; set; }
		
		bool? ShowDialog(OpenFileDialog openFileDialog);
		bool? ShowDialog(SelectXmlSchemaWindow dialog);
		
		void ShowErrorFormatted(string format, string parameter);
		void ShowExceptionError(Exception ex, string message);
		void ShowError(string message);
	}
}
