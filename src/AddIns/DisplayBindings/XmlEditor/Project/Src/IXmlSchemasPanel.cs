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
