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
using ICSharpCode.XmlEditor;
using NUnit.Framework;

namespace XmlEditor.Tests.Utils.Tests
{
	[TestFixture]
	public class MockXmlSchemasPanelTests
	{
		MockXmlSchemasPanel panel;
		
		[SetUp]
		public void Init()
		{
			panel = new MockXmlSchemasPanel();
		}
		
		[Test]
		public void XmlSchemaListItemCanBeRetrievedAfterAddingToPanel()
		{
			XmlSchemaListItem item = new XmlSchemaListItem("namespace", true);
			panel.AddXmlSchemaListItem(item);
			
			Assert.AreSame(item, panel.GetXmlSchemaListItem(0));
		}		
		
		[Test]
		public void SchemaListItemsCanBeSorted()
		{
			panel.AddXmlSchemaListItem(new XmlSchemaListItem("b"));
			panel.AddXmlSchemaListItem(new XmlSchemaListItem("a"));
			panel.AddXmlSchemaListSortDescription("NamespaceUri", ListSortDirection.Ascending);
						
			Assert.AreEqual("a", panel.GetXmlSchemaListItem(0).NamespaceUri);
		}
		
		[Test]
		public void SchemaListItemCount()
		{
			panel.AddXmlSchemaListItem(new XmlSchemaListItem("b"));
			panel.AddXmlSchemaListItem(new XmlSchemaListItem("a"));
						
			Assert.AreEqual(2, panel.XmlSchemaListItemCount);
		}
		
		[Test]
		public void RemoveSchemaListItem()
		{
			XmlSchemaListItem itemToRemove = new XmlSchemaListItem("b");
			panel.AddXmlSchemaListItem(itemToRemove);
			panel.AddXmlSchemaListItem(new XmlSchemaListItem("a"));
			panel.RemoveXmlSchemaListItem(itemToRemove);
						
			Assert.AreEqual(1, panel.XmlSchemaListItemCount);
		}
		
		[Test]
		public void XmlSchemaFileAssociationCanBeRetrievedAfterAddingToPanel()
		{
			XmlSchemaFileAssociationListItem item = new XmlSchemaFileAssociationListItem(".xml", "namespace-uri", "prefix");
			int index = panel.AddXmlSchemaFileAssociationListItem(item);
			
			Assert.AreSame(item, panel.GetXmlSchemaFileAssociationListItem(index));
		}
		
		[Test]
		public void XmlSchemaFileAssociationCountMatchesNumItemsAddedToPanel()
		{
			XmlSchemaFileAssociationListItem item = new XmlSchemaFileAssociationListItem(".xml", "namespace-uri", "prefix");
			panel.AddXmlSchemaFileAssociationListItem(item);
			
			Assert.AreEqual(1, panel.XmlSchemaFileAssociationListItemCount);
		}		
		
		[Test]
		public void XmlSchemaFileAssociationsCanBeSorted()
		{
			panel.AddXmlSchemaFileAssociationListItem(new XmlSchemaFileAssociationListItem(".xml", "namespace-uri", "x"));
			panel.AddXmlSchemaFileAssociationListItem(new XmlSchemaFileAssociationListItem(".abc", "namespace-uri-abc", "abc"));
			panel.AddXmlSchemaFileAssociationListSortDescription("Extension", ListSortDirection.Ascending);
			
			XmlSchemaFileAssociationListItem listItem = panel.GetXmlSchemaFileAssociationListItem(0) as XmlSchemaFileAssociationListItem;
			Assert.AreEqual(".abc", listItem.Extension);
		}
		
		[Test]
		public void NothingSelectedInXmlSchemaFileAssocationListAtStart()
		{
			Assert.AreEqual(-1, panel.SelectedXmlSchemaFileAssociationListItemIndex);
		}
		
		[Test]
		public void SchemaNamespaceTextUpdated()
		{
			panel.SetSelectedSchemaNamespace("abc");
			Assert.AreEqual("abc", panel.GetSelectedSchemaNamespace());
		}
		
		[Test]
		public void SelectedXmlSchemaFileAssocationIndexChangesSelectedItem()
		{
			XmlSchemaFileAssociationListItem xmlSchemaAssociation = new XmlSchemaFileAssociationListItem(".xml", "namespace-uri", "x");
			panel.AddXmlSchemaFileAssociationListItem(xmlSchemaAssociation);	
			panel.SelectedXmlSchemaFileAssociationListItemIndex = 0;
			
			Assert.AreSame(xmlSchemaAssociation, panel.GetSelectedXmlSchemaFileAssociationListItem());
		}
		
		[Test]
		public void SchemaNamespacePrefixTextUpdated()
		{
			panel.SetSelectedSchemaNamespacePrefix("abc");
			Assert.AreEqual("abc", panel.GetSelectedSchemaNamespacePrefix());
		}
		
		[Test]
		public void SelectXmlSchemaByNamespace()
		{
			panel.AddXmlSchemaListItem(new XmlSchemaListItem("a"));
			XmlSchemaListItem listItemToBeSelected = new XmlSchemaListItem("b");
			panel.AddXmlSchemaListItem(listItemToBeSelected);
			
			panel.SelectSchemaListItem("b");
			
			Assert.AreSame(listItemToBeSelected, panel.GetSelectedXmlSchemaListItem());
		}
		
		[Test]
		public void AfterKnownXmlSchemaSelectedSelectingAnUnknownXmlSchemaChangesTheSelectedXmlSchemaListItemToNull()
		{
			SelectXmlSchemaByNamespace();
			panel.SelectSchemaListItem("unknown");
			Assert.IsNull(panel.GetSelectedXmlSchemaListItem());
		}
		
		[Test]
		public void WhenSelectedSchemaNamespacePrefixModifiedSchemasEditorSchemaNamespacePrefixChangedMethodCalled()
		{
			XmlSchemaFileAssociationListItem xmlSchemaAssociation = new XmlSchemaFileAssociationListItem(".xml", "namespace-uri", "x");
			panel.AddXmlSchemaFileAssociationListItem(xmlSchemaAssociation);	
			panel.SelectedXmlSchemaFileAssociationListItemIndex = 0;

			bool methodCalled = false;
			panel.SetMethodToCallWhenSchemaNamespacePrefixChanged(delegate() { methodCalled = true; });
			panel.SetSelectedSchemaNamespacePrefix("test");
			
			Assert.IsTrue(methodCalled);
		}
		
		[Test]
		public void NothingSelectedInXmlSchemaListAtStart()
		{
			Assert.AreEqual(-1, panel.SelectedXmlSchemaListItemIndex);
		}
		
		[Test]
		public void SettingXmlSchemaListSelectedIndexChangesSelectedXmlSchemaListItem()
		{
			XmlSchemaListItem item = new XmlSchemaListItem("namespace", true);
			panel.AddXmlSchemaListItem(item);
			panel.SelectedXmlSchemaListItemIndex = 0;
			
			Assert.AreEqual("namespace", panel.GetSelectedXmlSchemaListItem().ToString());
		}
		
		[Test]
		public void AddingXmlSchemaListItemReturnsIndexOfItem()
		{
			panel.AddXmlSchemaListItem(new XmlSchemaListItem("namespace", true));
			int index = panel.AddXmlSchemaListItem(new XmlSchemaListItem("namespace2", false));
			Assert.AreEqual(1, index);
		}
		
		[Test]
		public void OpenFileDialogFileNameReturned()
		{
			string fileName = @"c:\projects\b.xsd";
			panel.OpenFileDialogFileNameToReturn = fileName;
			OpenFileDialog dialog = new OpenFileDialog();
			panel.ShowDialog(dialog);
			
			Assert.AreEqual(fileName, dialog.FileName);
		}
		
		[Test]
		public void OpenDialogPassedToShowDialogSaved()
		{
			OpenFileDialog dialog = new OpenFileDialog();
			panel.ShowDialog(dialog);
			
			Assert.AreSame(dialog, panel.OpenFileDialog);
		}
		
		[Test]
		public void ShowDialogReturnsFalse()
		{
			bool? result = false;
			panel.OpenFileDialogResultToReturn = result;
			OpenFileDialog dialog = new OpenFileDialog();
			
			Assert.AreEqual(result, panel.ShowDialog(dialog));
		}
		
		[Test]
		public void ShowDialogReturnsTrue()
		{
			bool? result = true;
			panel.OpenFileDialogResultToReturn = result;
			OpenFileDialog dialog = new OpenFileDialog();
			
			Assert.AreEqual(result, panel.ShowDialog(dialog));
		}
		
		[Test]
		public void ShowFormattedErrorMethodSavesFormattedErrorMessageObject()
		{
			FormattedErrorMessage message = new FormattedErrorMessage("message", "param");
			panel.ShowErrorFormatted("message", "param");
			
			Assert.AreEqual(message, panel.FormattedErrorMessage);
		}
		
		[Test]
		public void ShowErrorMessageMethodSavesErrorMessage()
		{
			string message = "Error";
			panel.ShowError(message);
			
			Assert.AreEqual(message, panel.ErrorMessage);
		}
		
		[Test]
		public void ShowExceptionErrorMethodSavesExceptionErrorMessageObject()
		{
			ApplicationException ex = new ApplicationException("Error");
			ExceptionErrorMessage message = new ExceptionErrorMessage(ex, "message");
			panel.ShowExceptionError(ex, "message");
			
			Assert.AreEqual(message, panel.ExceptionErrorMessage);
		}
		
		[Test]
		public void ShowSelectXmlSchemaWindowDialogResultReturnsFalse()
		{
			bool? result = false;
			panel.SelectXmlSchemaWindowDialogResultToReturn = result;
			SelectXmlSchemaWindow dialog = new SelectXmlSchemaWindow(new string[0]);
			
			Assert.AreEqual(result, panel.ShowDialog(dialog));
		}
		
		[Test]
		public void ShowSelectXmlSchemaWindowDialogResultReturnsTrue()
		{
			bool? result = true;
			panel.SelectXmlSchemaWindowDialogResultToReturn = result;
			SelectXmlSchemaWindow dialog = new SelectXmlSchemaWindow(new string[0]);
			
			Assert.AreEqual(result, panel.ShowDialog(dialog));
		}

		[Test]
		public void SelectXmlSchemaWindowPassedToShowDialogSaved()
		{
			SelectXmlSchemaWindow dialog = new SelectXmlSchemaWindow(new string[0]);
			panel.ShowDialog(dialog);
			
			Assert.AreSame(dialog, panel.SelectXmlSchemaWindow);
		}
		
		[Test]
		public void SelectXmlSchemaWindowNamespaceReturned()
		{
			string namespaceUri = "http://test";
			SelectXmlSchemaWindow dialog = new SelectXmlSchemaWindow(new string[] { namespaceUri });
			panel.SelectXmlSchemaWindowNamespaceToReturn = namespaceUri;
			panel.ShowDialog(dialog);
			
			Assert.AreEqual(namespaceUri, dialog.SelectedNamespaceUri);
		}
		
		[Test]
		public void SelectXmlSchemaWindowNamespaceSavedWhenShowDialogCalled()
		{
			string namespaceUri = "http://test";
			SelectXmlSchemaWindow dialog = new SelectXmlSchemaWindow(new string[] { namespaceUri });
			dialog.SelectedNamespaceUri = namespaceUri;
			panel.ShowDialog(dialog);
			
			Assert.AreEqual(namespaceUri, panel.SelectXmlSchemaWindowNamespaceUriSelectedWhenShowDialogCalled);
		}
		
		[Test]
		public void GetXmlSchemaListItemNamespacesAsStringArray()
		{
			panel.AddXmlSchemaListItem(new XmlSchemaListItem("a"));
			panel.AddXmlSchemaListItem(new XmlSchemaListItem("b"));
			
			string[] expectedItems = new string[] { "a", "b" };

			Assert.AreEqual(expectedItems, panel.GetXmlSchemaListItemNamespaces());
		}
		
		[Test]
		public void AddingNewXmlSchemasAreSortedInList()
		{
			panel.AddXmlSchemaListSortDescription("NamespaceUri", ListSortDirection.Ascending);
			panel.AddXmlSchemaListItem(new XmlSchemaListItem("b"));
			panel.AddXmlSchemaListItem(new XmlSchemaListItem("a"));
			panel.RefreshXmlSchemaListItems();
			
			string[] expectedItems = new string[] { "a", "b" };

			Assert.AreEqual(expectedItems, panel.GetXmlSchemaListItemNamespaces());
		}
		
		[Test]
		public void ScrollIntoViewXmlSchemaListItemIsSaved()
		{
			XmlSchemaListItem item = new XmlSchemaListItem("a");
			panel.ScrollXmlSchemaListItemIntoView(item);
			Assert.AreSame(item, panel.XmlSchemaListItemScrolledIntoView);
		}
	}
}
