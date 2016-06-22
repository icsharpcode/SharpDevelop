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
using System.IO;
using System.Windows.Controls;
using System.Xml;
using ICSharpCode.Core;
using ICSharpCode.XmlEditor;
using NUnit.Framework;
using XmlEditor.Tests.Utils;

namespace XmlEditor.Tests.Editor
{
	[TestFixture]
	public class XmlSchemasPanelTestFixture
	{
		XmlSchemasPanel schemasPanel;
		ListBox schemaListBox;
		ComboBox fileExtensionComboBox;
		Button removeSchemaButton;
		TextBox namespacePrefixTextBox;
		TextBox schemaNamespaceTextBox;
		XmlSchemaFileAssociations associations;
		RegisteredXmlSchemas registeredXmlSchemas;
		Properties properties;
		MockXmlSchemaCompletionDataFactory factory;
		XmlSchemaListItem schemaListItem;
		XmlSchemaFileAssociationListItem fileAssociationItem;
		
		[SetUp]
		public void Init()
		{
			factory = new MockXmlSchemaCompletionDataFactory();
			registeredXmlSchemas = new RegisteredXmlSchemas(new string[0], String.Empty, null, factory);
			properties = new Properties();
			DefaultXmlSchemaFileAssociations defaultAssociations = new DefaultXmlSchemaFileAssociations(new AddInTreeNode());
			associations = new XmlSchemaFileAssociations(properties, defaultAssociations, new XmlSchemaCompletionCollection());
			schemasPanel = new XmlSchemasPanel(registeredXmlSchemas, new string[0], associations, factory);
			
			schemaListBox = schemasPanel.FindName("schemaListBox") as ListBox;
			fileExtensionComboBox = schemasPanel.FindName("fileExtensionComboBox") as ComboBox;
			removeSchemaButton = schemasPanel.FindName("removeSchemaButton") as Button;
			namespacePrefixTextBox = schemasPanel.FindName("namespacePrefixTextBox") as TextBox;
			schemaNamespaceTextBox = schemasPanel.FindName("schemaNamespaceTextBox") as TextBox;

			schemaListItem = new XmlSchemaListItem("a");
			schemaListBox.Items.Add(schemaListItem);

			fileAssociationItem = new XmlSchemaFileAssociationListItem(".xsd", "ns", "a");
			schemasPanel.AddXmlSchemaFileAssociationListItem(fileAssociationItem);			
		}
				
		[Test]
		public void XmlSchemaListItemCountReturnsCountOfItemsInListBox()
		{
			Assert.AreEqual(1, schemasPanel.XmlSchemaListItemCount);
		}
		
		[Test]
		public void GetSelectedXmlSchemaListItemIndexReturnsListBoxSelectedItem()
		{
			schemaListBox.SelectedIndex = 0;
			
			Assert.AreSame(schemaListItem, schemasPanel.GetSelectedXmlSchemaListItem());
		}
		
		[Test]
		public void GetSelectedXmlSchemaListItemAtIndexReturnsCorrectListBoxItem()
		{
			Assert.AreSame(schemaListItem, schemasPanel.GetXmlSchemaListItem(0));
		}
		
		[Test]
		public void SelectXmlSchemaListItemIndexSetsListBoxSelectedIndex()
		{
			schemasPanel.SelectedXmlSchemaListItemIndex = 0;
			
			Assert.AreEqual(0, schemaListBox.SelectedIndex);
		}
		
		[Test]
		public void XmlSchemaFileAssociationsItemCountReturnsCountOfItemsInComboBox()
		{
			Assert.AreEqual(1, schemasPanel.XmlSchemaFileAssociationListItemCount);
		}

		[Test]
		public void SelectedXmlSchemaFileAssociationsIndexReturnsComboBoxSelectedIndex()
		{
			fileExtensionComboBox.SelectedIndex = 0;
			
			Assert.AreEqual(0, schemasPanel.SelectedXmlSchemaFileAssociationListItemIndex);
		}
		
		[Test]
		public void GetSelectedXmlSchemaFileAssociationReturnsCorrectFileAssociation()
		{
			fileExtensionComboBox.SelectedIndex = 0;
			
			Assert.AreSame(fileAssociationItem, schemasPanel.GetSelectedXmlSchemaFileAssociationListItem());
		}
		
		[Test]
		public void GetXmlSchemaFileAssociationByIndexReturnsCorrectFileAssociation()
		{
			Assert.AreSame(fileAssociationItem, schemasPanel.GetXmlSchemaFileAssociationListItem(0));
		}		

		[Test]
		public void SelectXmlSchemaFileAssociationsIndexSetsComboBoxSelectedIndex()
		{
			schemasPanel.SelectedXmlSchemaFileAssociationListItemIndex = 0;
			
			Assert.AreEqual(0, fileExtensionComboBox.SelectedIndex);
		}
		
		[Test]
		public void RemoveSchemaButtonEnabledSetsButtonEnabledState()
		{
			removeSchemaButton.IsEnabled = false;
			schemasPanel.RemoveSchemaButtonEnabled = true;
			
			Assert.IsTrue(removeSchemaButton.IsEnabled);
		}
		
		[Test]
		public void GetRemoveSchemaButtonEnabledReturnsButtonEnabledState()
		{
			removeSchemaButton.IsEnabled = true;
			
			Assert.IsTrue(schemasPanel.RemoveSchemaButtonEnabled);
		}
		
		[Test]
		public void AddXmlSchemaListItemAddsItemToListBox()
		{
			XmlSchemaListItem newItem = new XmlSchemaListItem("new");
			int index = schemasPanel.AddXmlSchemaListItem(newItem);
			
			Assert.AreSame(newItem, schemaListBox.Items[index]);
		}
		
		[Test]
		public void RemoveXmlSchemaListItemIndexSRemovesItemFromListBox()
		{
			schemasPanel.RemoveXmlSchemaListItem(schemaListItem);
			
			Assert.AreEqual(0, schemaListBox.Items.Count);
		}
		
		[Test]
		public void AddXmlSchemaSortDescriptionAddsSortDescriptionToListBox()
		{			
			schemasPanel.AddXmlSchemaListSortDescription("NamespaceUri", ListSortDirection.Ascending);
			
			SortDescription sortDesc = schemaListBox.Items.SortDescriptions[0];
			Assert.AreEqual("NamespaceUri", sortDesc.PropertyName);
			Assert.AreEqual(ListSortDirection.Ascending, sortDesc.Direction);
		}
		
		[Test]
		public void AddXmlSchemaFileAssociationAddsItemToComboBox()
		{
			Assert.AreSame(fileAssociationItem, schemasPanel.GetXmlSchemaFileAssociationListItem(0));
		}		
		
		[Test]
		public void AddXmlSchemaFileAssociationSortDescriptionAddsSortDescriptionToComboBox()
		{
			schemasPanel.AddXmlSchemaFileAssociationListSortDescription("Extension", ListSortDirection.Ascending);
			
			SortDescription sortDesc = fileExtensionComboBox.Items.SortDescriptions[0];
			Assert.AreEqual("Extension", sortDesc.PropertyName);
			Assert.AreEqual(ListSortDirection.Ascending, sortDesc.Direction);
		}
		
		[Test]
		public void GetSelectedNamespaceReturnsNamespaceTextBoxText()
		{
			schemaNamespaceTextBox.Text = "abc";
			Assert.AreEqual("abc", schemasPanel.GetSelectedSchemaNamespace());
		}
	
		[Test]
		public void SetSelectedNamespaceSetsNamespaceTextBoxText()
		{
			schemasPanel.SetSelectedSchemaNamespace("abc");
			Assert.AreEqual("abc", schemaNamespaceTextBox.Text);
		}
		
		[Test]
		public void GetSelectedNamespacePrefixReturnsNamespacePrefixTextBoxText()
		{
			namespacePrefixTextBox.Text = "abc";
			Assert.AreEqual("abc", schemasPanel.GetSelectedSchemaNamespacePrefix());
		}
	
		[Test]
		public void SetSelectedNamespacePrefixSetsNamespacePrefixTextBoxText()
		{
			schemasPanel.SetSelectedSchemaNamespacePrefix("abc");
			Assert.AreEqual("abc", namespacePrefixTextBox.Text);
		}
	}
}
