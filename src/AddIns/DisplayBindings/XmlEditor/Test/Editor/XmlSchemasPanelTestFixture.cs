// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

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
		XmlEditorOptions options;
		XmlSchemaManager schemaManager;
		Properties properties;
		MockXmlSchemaCompletionDataFactory factory;
		XmlSchemaListItem schemaListItem;
		XmlSchemaFileAssociationListItem fileAssociationItem;
		
		[SetUp]
		public void Init()
		{
			factory = new MockXmlSchemaCompletionDataFactory();
			schemaManager = new XmlSchemaManager(new string[0], String.Empty, null, factory);
			properties = new Properties();
			DefaultXmlSchemaFileAssociations associations = new DefaultXmlSchemaFileAssociations(new AddInTreeNode());
			options = new XmlEditorOptions(properties, associations, new XmlSchemaCompletionDataCollection());
			schemasPanel = new XmlSchemasPanel(schemaManager, new string[0], options, factory);
			
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
