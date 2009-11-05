// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
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
	public class RegisteredXmlSchemasEditorTestFixture
	{
		RegisteredXmlSchemasEditor schemasEditor;
		MockXmlSchemaCompletionDataFactory factory;
		XmlSchemaCompletionData fooSchemaData;
		XmlSchemaCompletionData barSchemaData;
		XmlSchemaCompletionData xmlSchemaData;
		XmlEditorOptions options;
		XmlSchemaCompletionDataCollection schemas;
		Properties properties;
		MockXmlSchemasPanel panel;
		XmlSchemaManager schemaManager;
		
		[SetUp]
		public void Init()
		{
			LoadBarSchema();
			LoadFooSchema();
			fooSchemaData.ReadOnly = true;
			LoadXmlSchema();
			
			schemas = new XmlSchemaCompletionDataCollection();
			schemas.Add(fooSchemaData);
			schemas.Add(barSchemaData);
			schemas.Add(xmlSchemaData);
			
			factory = new MockXmlSchemaCompletionDataFactory();
			schemaManager = new XmlSchemaManager(new string[0], String.Empty, new MockFileSystem(), factory);
			schemaManager.Schemas.AddRange(schemas);

			string[] xmlFileExtensions = new string[] { ".foo", ".bar", ".xml" };
			
			DefaultXmlSchemaFileAssociations defaultSchemaFileAssociations = new DefaultXmlSchemaFileAssociations();
			defaultSchemaFileAssociations.Add(new XmlSchemaFileAssociation(".foo", "http://foo"));
			defaultSchemaFileAssociations.Add(new XmlSchemaFileAssociation(".bar", "http://bar", "b"));
			
			properties = new Properties();
			options = new XmlEditorOptions(properties, defaultSchemaFileAssociations, new XmlSchemaCompletionDataCollection());
			options.SetSchemaFileAssociation(new XmlSchemaFileAssociation(".xml", "http://xml"));
			
			panel = new MockXmlSchemasPanel();
			schemasEditor = new RegisteredXmlSchemasEditor(schemaManager, xmlFileExtensions, options, panel, factory);
			schemasEditor.LoadOptions();	
		}
		
		void LoadFooSchema()
		{
			string schema = "<xs:schema xmlns:xs='http://www.w3.org/2001/XMLSchema' targetNamespace='http://foo' />";
			fooSchemaData = LoadSchema(schema);
		}
		
		void LoadBarSchema()
		{
			string schema = "<xs:schema xmlns:xs='http://www.w3.org/2001/XMLSchema' targetNamespace='http://bar' />";
			barSchemaData = LoadSchema(schema);
		}
		
		void LoadXmlSchema()
		{
			string schema = "<xs:schema xmlns:xs='http://www.w3.org/2001/XMLSchema' targetNamespace='http://xml' />";
			xmlSchemaData = LoadSchema(schema);
		}		
		
		XmlSchemaCompletionData LoadSchema(string xml)
		{
			return new XmlSchemaCompletionData(new StringReader(xml));
		}
		
		[Test]
		public void FirstSchemaInListIsBarSchema()
		{
			Assert.AreEqual("http://bar", panel.GetXmlSchemaListItem(0).NamespaceUri);
		}
				
		[Test]
		public void FirstXmlFileExtensionIsBar()
		{
			XmlSchemaFileAssociationListItem expectedItem = new XmlSchemaFileAssociationListItem(".bar", "http://bar", "b");
			Assert.AreEqual(expectedItem.ToString(), panel.GetXmlSchemaFileAssociationListItem(0).ToString());
		}
		
		[Test]
		public void LastXmlFileExtensionIsXml()
		{
			XmlSchemaFileAssociationListItem expectedItem = new XmlSchemaFileAssociationListItem(".xml", "http://xml", String.Empty);
			Assert.AreEqual(expectedItem.ToString(), panel.GetXmlSchemaFileAssociationListItem(2).ToString());
		}
		
		[Test]
		public void FirstFileExtensionIsSelected()
		{
			Assert.AreEqual(0, panel.SelectedXmlSchemaFileAssociationListItemIndex);
		}
		
		[Test]
		public void SchemaNamespaceTextBoxShowsBarSchemaNamespace()
		{
			schemasEditor.XmlSchemaFileAssociationFileExtensionSelectionChanged();
			Assert.AreEqual("http://bar", panel.GetSelectedSchemaNamespace());
		}
		
		[Test]
		public void NamespacePrefixTextBoxShowsBarSchemaNamespacePrefix()
		{
			schemasEditor.XmlSchemaFileAssociationFileExtensionSelectionChanged();
			Assert.AreEqual("b", panel.GetSelectedSchemaNamespacePrefix());
		}
		
		[Test]
		public void RemoveSchemaButtonDisabledWhenFooReadOnlySchemaSelected()
		{
			panel.SelectSchemaListItem("http://foo");
			schemasEditor.SchemaListSelectionChanged();
			
			Assert.IsFalse(panel.RemoveSchemaButtonEnabled);
		}
		
		[Test]
		public void RemoveSchemaButtonEnabledWhenBarSchemaSelected()
		{
			panel.SelectSchemaListItem("http://bar");
			schemasEditor.SchemaListSelectionChanged();

			Assert.IsTrue(panel.RemoveSchemaButtonEnabled);
		}
		
		[Test]
		public void FileExtensionDisplayedInFileExtensionComboBox()
		{
			Assert.AreEqual(".bar", panel.GetXmlSchemaFileAssociationListItem(0).ToString());
		}
		
		[Test]
		public void NothingToBeSavedAfterInitiallyLoadingXmlSchemasOptionsPanel()
		{
			Assert.IsFalse(schemasEditor.XmlFileAssociationsChanged);
		}
		
		[Test]
		public void NamespacePrefixTextBoxChangedModifiesXmlFileAssociations()
		{
			panel.SetSelectedSchemaNamespacePrefix("modified-prefix");
			schemasEditor.SchemaNamespacePrefixChanged();

			Assert.IsTrue(schemasEditor.XmlFileAssociationsChanged);
		}
		
		[Test]
		public void NamespacePrefixTextBoxChangedModifiesXmlFileAssociationNamespacePrefix()
		{
			panel.SetSelectedSchemaNamespacePrefix("modified-prefix");
			schemasEditor.SchemaNamespacePrefixChanged();

			XmlSchemaFileAssociationListItem item = panel.GetSelectedXmlSchemaFileAssociationListItem() as XmlSchemaFileAssociationListItem;
			Assert.AreEqual("modified-prefix", item.NamespacePrefix);
		}
		
		[Test]
		public void NamespacePrefixModifiesXmlSchemaFileAssociation()
		{
			panel.SetSelectedSchemaNamespacePrefix("modified-prefix");
			schemasEditor.SchemaNamespacePrefixChanged();
			schemasEditor.SaveOptions();
			XmlSchemaFileAssociation expectedSchemaAssociation = new XmlSchemaFileAssociation(".bar", "http://bar", "modified-prefix");
			
			Assert.AreEqual(expectedSchemaAssociation, options.GetSchemaFileAssociation(".bar"));
		}
		
		[Test]
		public void FooSchemaFileAssociationNotSavedWhenBarNamespacePrefixModified()
		{
			panel.SetSelectedSchemaNamespacePrefix("modified-prefix");
			schemasEditor.SchemaNamespacePrefixChanged();
			schemasEditor.SaveOptions();
			
			Assert.IsTrue(String.IsNullOrEmpty(properties.Get("ext.foo", String.Empty)));
		}
		
		[Test]
		public void NamespaceSelectedButNotModifiedShouldNotBeSavedInXmlEditorOptions()
		{
			panel.SelectedXmlSchemaFileAssociationListItemIndex = 0;
			panel.SetMethodToCallWhenSchemaNamespacePrefixChanged(schemasEditor.SchemaNamespacePrefixChanged);
			schemasEditor.XmlSchemaFileAssociationFileExtensionSelectionChanged();
			schemasEditor.SaveOptions();
			
			Assert.IsTrue(String.IsNullOrEmpty(properties.Get("ext.bar", String.Empty)));
		}
		
		[Test]
		public void XmlSchemasHaveNotChanged()
		{
			Assert.IsFalse(schemasEditor.XmlSchemasChanged);
		}
		
		[Test]
		public void NothingHapppensWhenSchemaNamespacePrefixChangedAndNoFileAssociationSelected()
		{
			panel.SelectedXmlSchemaFileAssociationListItemIndex = -1;
			schemasEditor.SchemaNamespacePrefixChanged();
			Assert.IsFalse(schemasEditor.XmlFileAssociationsChanged);
		}
		
		[Test]
		public void RemoveButtonDisableWhenSchemaSelectionChangedAndNoSchemaSelected()
		{
			panel.RemoveSchemaButtonEnabled = true;
			panel.SelectedXmlSchemaListItemIndex = -1;
			schemasEditor.SchemaListSelectionChanged();
			
			Assert.IsFalse(panel.RemoveSchemaButtonEnabled);
		}
	}
}
