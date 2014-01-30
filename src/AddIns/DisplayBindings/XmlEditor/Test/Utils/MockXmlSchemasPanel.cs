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
using System.Windows;
using System.Windows.Controls;
using Microsoft.Win32;
using ICSharpCode.XmlEditor;

namespace XmlEditor.Tests.Utils
{
	public class MockXmlSchemasPanel : IXmlSchemasPanel
	{
		ListBox schemaListBox = new ListBox();
		ComboBox schemaFileAssociationsComboBox = new ComboBox();
		string schemaNamespace;
		string schemaNamespacePrefix;
		bool removeSchemaButtonEnabled;
		OnNamespacePrefixChanged namespacePrefixChangedMethod;
		OpenFileDialog openFileDialog;
		string openFileDialogFileNameToReturn;
		bool? openFileDialogResultToReturn;
		bool? selectXmlSchemaWindowDialogResultToReturn;
		SelectXmlSchemaWindow selectXmlSchemaWindow;
		string selectXmlSchemaWindowNamespaceToReturn;
		string selectXmlSchemaWindowNamespaceUriSelectedWhenShowDialogCalled;			
		FormattedErrorMessage formattedErrorMessage;
		string errorMessage;
		ExceptionErrorMessage exceptionErrorMessage;
		XmlSchemaListItem xmlSchemaListItemScrolledIntoView;
		
		public delegate void OnNamespacePrefixChanged();
		
		public MockXmlSchemasPanel()
		{
		}
		
		public XmlSchemaListItem GetXmlSchemaListItem(int index)
		{
			return schemaListBox.Items[index] as XmlSchemaListItem;
		}
		
		public int XmlSchemaListItemCount {
			get { return schemaListBox.Items.Count; }
		}
		
		public int AddXmlSchemaListItem(XmlSchemaListItem schema)
		{
			return schemaListBox.Items.Add(schema);
		}
		
		public void RemoveXmlSchemaListItem(XmlSchemaListItem schema)
		{
			schemaListBox.Items.Remove(schema);
		}
		
		public void RefreshXmlSchemaListItems()
		{
			schemaListBox.Items.Refresh();
		}		
		
		public void AddXmlSchemaListSortDescription(string propertyName, ListSortDirection sortDirection)
		{
			schemaListBox.Items.SortDescriptions.Add(new SortDescription(propertyName, sortDirection));
		}
		
		public XmlSchemaFileAssociationListItem GetXmlSchemaFileAssociationListItem(int index)
		{
			return (XmlSchemaFileAssociationListItem)schemaFileAssociationsComboBox.Items[index];
		}
		
		public int XmlSchemaFileAssociationListItemCount {
			get { return schemaFileAssociationsComboBox.Items.Count; }
		}
		
		public int AddXmlSchemaFileAssociationListItem(XmlSchemaFileAssociationListItem item)
		{
			return schemaFileAssociationsComboBox.Items.Add(item);
		}
		
		public void AddXmlSchemaFileAssociationListSortDescription(string propertyName, ListSortDirection sortDirection)
		{
			schemaFileAssociationsComboBox.Items.SortDescriptions.Add(new SortDescription(propertyName, sortDirection));
		}
		
		public int SelectedXmlSchemaFileAssociationListItemIndex { 
			get { return schemaFileAssociationsComboBox.SelectedIndex; }
			set { schemaFileAssociationsComboBox.SelectedIndex = value; }
		}
		
		public string GetSelectedSchemaNamespace()
		{
			return schemaNamespace;
		}
		
		public void SetSelectedSchemaNamespace(string schemaNamespace)
		{
			this.schemaNamespace = schemaNamespace;
		}
		
		public XmlSchemaFileAssociationListItem GetSelectedXmlSchemaFileAssociationListItem()
		{
			return (XmlSchemaFileAssociationListItem)schemaFileAssociationsComboBox.SelectedItem;
		}
		
		public string GetSelectedSchemaNamespacePrefix()
		{
			return schemaNamespacePrefix;
		}
		
		public void SetSelectedSchemaNamespacePrefix(string namespacePrefix)
		{
			this.schemaNamespacePrefix = namespacePrefix;
			if (namespacePrefixChangedMethod != null) {
				namespacePrefixChangedMethod();
			}
		}
		
		public void SetMethodToCallWhenSchemaNamespacePrefixChanged(OnNamespacePrefixChanged method)
		{
			namespacePrefixChangedMethod = method;
		}
		
		public void SelectSchemaListItem(string namespaceUri)
		{
			for (int index = 0; index < schemaListBox.Items.Count; ++index) {
				XmlSchemaListItem item = schemaListBox.Items[index] as XmlSchemaListItem;
				if (item.NamespaceUri == namespaceUri) {
					schemaListBox.SelectedIndex = index;
					return;
				}
			}
			schemaListBox.SelectedIndex = -1;
		}
		
		public bool RemoveSchemaButtonEnabled {
			get { return removeSchemaButtonEnabled; }
			set { removeSchemaButtonEnabled = value; }
		}
		
		public XmlSchemaListItem GetSelectedXmlSchemaListItem()
		{
			return (XmlSchemaListItem)schemaListBox.SelectedItem;
		}
		
		public bool? ShowDialog(OpenFileDialog openFileDialog)
		{
			this.openFileDialog = openFileDialog;
			openFileDialog.FileName = openFileDialogFileNameToReturn;
			
			return openFileDialogResultToReturn;
		}
		
		public OpenFileDialog OpenFileDialog {
			get { return openFileDialog; }
		}
		
		public string OpenFileDialogFileNameToReturn {
			get { return openFileDialogFileNameToReturn; }
			set { openFileDialogFileNameToReturn = value; }
		}
		
		public bool? OpenFileDialogResultToReturn {
			get { return openFileDialogResultToReturn; }
			set { openFileDialogResultToReturn = value; }
		}
		
		public bool? ShowDialog(SelectXmlSchemaWindow dialog)
		{
			selectXmlSchemaWindow = dialog;
			selectXmlSchemaWindowNamespaceUriSelectedWhenShowDialogCalled = dialog.SelectedNamespaceUri;
			
			dialog.SelectedNamespaceUri = selectXmlSchemaWindowNamespaceToReturn;
			return selectXmlSchemaWindowDialogResultToReturn;
		}
		
		public SelectXmlSchemaWindow SelectXmlSchemaWindow {
			get { return selectXmlSchemaWindow; }
		}
		
		public string SelectXmlSchemaWindowNamespaceUriSelectedWhenShowDialogCalled {
			get { return selectXmlSchemaWindowNamespaceUriSelectedWhenShowDialogCalled; }
		}
		
		public bool? SelectXmlSchemaWindowDialogResultToReturn {
			get { return selectXmlSchemaWindowDialogResultToReturn; }
			set { selectXmlSchemaWindowDialogResultToReturn = value; }
		}
		
		public string SelectXmlSchemaWindowNamespaceToReturn {
			get { return selectXmlSchemaWindowNamespaceToReturn; }
			set { selectXmlSchemaWindowNamespaceToReturn = value; }
		}
		
		public int SelectedXmlSchemaListItemIndex {
			get { return schemaListBox.SelectedIndex; }
			set { schemaListBox.SelectedIndex = value; }
		}
		
		public void ShowErrorFormatted(string format, string parameter)
		{
			formattedErrorMessage = new FormattedErrorMessage(format, parameter);
		}
		
		public FormattedErrorMessage FormattedErrorMessage {
			get { return formattedErrorMessage; }
		}
		
		public void ShowError(string message)
		{
			errorMessage = message;
		}
		
		public string ErrorMessage {
			get { return errorMessage; }
		}
		
		public void ShowExceptionError(Exception ex, string message)
		{
			exceptionErrorMessage = new ExceptionErrorMessage(ex, message);
		}
		
		public ExceptionErrorMessage ExceptionErrorMessage {
			get { return exceptionErrorMessage; }
		}
		
		public string[] GetXmlSchemaListItemNamespaces()
		{
			List<string> schemas = new List<string>();
			foreach (XmlSchemaListItem item in schemaListBox.Items) {
				schemas.Add(item.NamespaceUri);
			}			
			return schemas.ToArray();
		}
		
		public void ScrollXmlSchemaListItemIntoView(XmlSchemaListItem schemaListItem)
		{
			xmlSchemaListItemScrolledIntoView = schemaListItem;
		}
		
		public XmlSchemaListItem XmlSchemaListItemScrolledIntoView {
			get { return xmlSchemaListItemScrolledIntoView; }
		}
	}
}
