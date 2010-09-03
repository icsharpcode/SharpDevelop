// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
		XmlSchemaCompletion fooSchemaData;
		XmlSchemaCompletion barSchemaData;
		XmlSchemaCompletion xmlSchemaData;
		XmlSchemaFileAssociations associations;
		XmlSchemaCompletionCollection schemas;
		Properties properties;
		MockXmlSchemasPanel panel;
		RegisteredXmlSchemas registeredXmlSchemas;
		
		[SetUp]
		public void Init()
		{
			LoadBarSchema();
			LoadFooSchema();
			fooSchemaData.IsReadOnly = true;
			LoadXmlSchema();
			
			schemas = new XmlSchemaCompletionCollection();
			schemas.Add(fooSchemaData);
			schemas.Add(barSchemaData);
			schemas.Add(xmlSchemaData);
			
			factory = new MockXmlSchemaCompletionDataFactory();
			registeredXmlSchemas = new RegisteredXmlSchemas(new string[0], String.Empty, new MockFileSystem(), factory);
			registeredXmlSchemas.Schemas.AddRange(schemas);

			string[] xmlFileExtensions = new string[] { ".foo", ".bar", ".xml" };
			
			DefaultXmlSchemaFileAssociations defaultSchemaFileAssociations = new DefaultXmlSchemaFileAssociations();
			defaultSchemaFileAssociations.Add(new XmlSchemaFileAssociation(".foo", "http://foo"));
			defaultSchemaFileAssociations.Add(new XmlSchemaFileAssociation(".bar", "http://bar", "b"));
			
			properties = new Properties();
			associations = new XmlSchemaFileAssociations(properties, defaultSchemaFileAssociations, new XmlSchemaCompletionCollection());
			associations.SetSchemaFileAssociation(new XmlSchemaFileAssociation(".xml", "http://xml"));
			
			panel = new MockXmlSchemasPanel();
			schemasEditor = new RegisteredXmlSchemasEditor(registeredXmlSchemas, xmlFileExtensions, associations, panel, factory);
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
		
		XmlSchemaCompletion LoadSchema(string xml)
		{
			return new XmlSchemaCompletion(new StringReader(xml));
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
			
			Assert.AreEqual(expectedSchemaAssociation, associations.GetSchemaFileAssociation(".bar"));
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
