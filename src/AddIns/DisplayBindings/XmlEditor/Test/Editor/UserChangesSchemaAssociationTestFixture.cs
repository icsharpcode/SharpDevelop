// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using System.IO;
using ICSharpCode.Core;
using ICSharpCode.XmlEditor;
using NUnit.Framework;
using XmlEditor.Tests.Utils;

namespace XmlEditor.Tests.Editor
{
	[TestFixture]
	public class UserChangesSchemaAssociationTestFixture
	{
		MockXmlSchemasPanel panel;
		RegisteredXmlSchemasEditor schemasEditor;
		MockXmlSchemaCompletionDataFactory factory;
		XmlSchemaManager schemaManager;
		MockFileSystem fileSystem;

		[SetUp]
		public void Init()
		{
			fileSystem = new MockFileSystem();
			factory = new MockXmlSchemaCompletionDataFactory();
			schemaManager = new XmlSchemaManager(new string[0], @"c:\users\user\sharpdevelop\schemas", fileSystem, factory);
		
			string testSchemaXml = "<xs:schema xmlns:xs='http://www.w3.org/2001/XMLSchema' targetNamespace='http://test' />";
			XmlSchemaCompletionData schema = new XmlSchemaCompletionData(new StringReader(testSchemaXml));
			schema.ReadOnly = false;
			schemaManager.Schemas.Add(schema);
				
			XmlEditorOptions options = new XmlEditorOptions(new Properties(), new DefaultXmlSchemaFileAssociations(null), schemaManager.Schemas);
			options.SetSchemaFileAssociation(new XmlSchemaFileAssociation(".test", "http://test"));
			panel = new MockXmlSchemasPanel();
			
			schemasEditor = new RegisteredXmlSchemasEditor(schemaManager, new string[] { ".test" }, options, panel, factory);
			schemasEditor.LoadOptions();
			
			string newXmlSchemaFileName = @"c:\projects\new.xsd";
			string newSchemaXml = "<xs:schema xmlns:xs='http://www.w3.org/2001/XMLSchema' targetNamespace='http://new' />";
			factory.AddSchemaXml(newXmlSchemaFileName, newSchemaXml);
			
			panel.OpenFileDialogFileNameToReturn = newXmlSchemaFileName;
			panel.OpenFileDialogResultToReturn = true;
			
			// Add schema from file system to ensure that the list of schemas shown to the
			// user is from the list of schemas in the list box when changing the association 
			// to a file extension
			schemasEditor.AddSchemaFromFileSystem();
			
			panel.SelectedXmlSchemaFileAssociationListItemIndex = 0;
			schemasEditor.XmlSchemaFileAssociationFileExtensionSelectionChanged();
			
			panel.SelectXmlSchemaWindowDialogResultToReturn = true;
			panel.SelectXmlSchemaWindowNamespaceToReturn = "http://new";
			schemasEditor.ChangeSchemaAssociation();
		}
		
		[Test]
		public void FileAssociationsChange()
		{
			Assert.IsTrue(schemasEditor.XmlFileAssociationsChanged);
		}
		
		[Test]
		public void FileAssociationNamespaceUpdated()
		{
			XmlSchemaFileAssociationListItem item = panel.GetSelectedXmlSchemaFileAssociationListItem() as XmlSchemaFileAssociationListItem;
			Assert.AreEqual("http://new", item.NamespaceUri);
		}
		
		[Test]
		public void TestSchemaSelectedInSelectXmlSchemaWindowWhenFirstOpened()
		{
			Assert.AreEqual("http://test", panel.SelectXmlSchemaWindowNamespaceUriSelectedWhenShowDialogCalled);
		}

		[Test]
		public void SchemaNamespaceTextBoxUpdated()
		{
			Assert.AreEqual("http://new", panel.GetSelectedSchemaNamespace());
		}
	}
}
