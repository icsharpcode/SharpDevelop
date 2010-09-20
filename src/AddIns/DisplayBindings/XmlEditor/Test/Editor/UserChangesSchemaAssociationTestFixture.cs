// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
		RegisteredXmlSchemas registeredXmlSchemas;
		MockFileSystem fileSystem;

		[SetUp]
		public void Init()
		{
			fileSystem = new MockFileSystem();
			factory = new MockXmlSchemaCompletionDataFactory();
			registeredXmlSchemas = new RegisteredXmlSchemas(new string[0], @"c:\users\user\sharpdevelop\schemas", fileSystem, factory);
		
			string testSchemaXml = "<xs:schema xmlns:xs='http://www.w3.org/2001/XMLSchema' targetNamespace='http://test' />";
			XmlSchemaCompletion schema = new XmlSchemaCompletion(new StringReader(testSchemaXml));
			schema.IsReadOnly = false;
			registeredXmlSchemas.Schemas.Add(schema);
				
			XmlSchemaFileAssociations associations = new XmlSchemaFileAssociations(new Properties(), new DefaultXmlSchemaFileAssociations(null), registeredXmlSchemas.Schemas);
			associations.SetSchemaFileAssociation(new XmlSchemaFileAssociation(".test", "http://test"));
			panel = new MockXmlSchemasPanel();
			
			schemasEditor = new RegisteredXmlSchemasEditor(registeredXmlSchemas, new string[] { ".test" }, associations, panel, factory);
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
