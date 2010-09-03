// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.IO;
using ICSharpCode.Core;
using ICSharpCode.XmlEditor;
using NUnit.Framework;
using XmlEditor.Tests.Utils;

namespace XmlEditor.Tests.Editor
{
	[TestFixture]
	public class UserCancelsChangeSchemaDialogTestFixture
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
		
			XmlSchemaCompletionCollection schemas = new XmlSchemaCompletionCollection();
			string testSchemaXml = "<xs:schema xmlns:xs='http://www.w3.org/2001/XMLSchema' targetNamespace='http://test' />";
			XmlSchemaCompletion schema = new XmlSchemaCompletion(new StringReader(testSchemaXml));
			schema.IsReadOnly = false;
			schemas.Add(schema);
				
			XmlSchemaFileAssociations associations = new XmlSchemaFileAssociations(new Properties(), new DefaultXmlSchemaFileAssociations(null), schemas);
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
			
			panel.SelectXmlSchemaWindowDialogResultToReturn = false;
			schemasEditor.ChangeSchemaAssociation();
		}
		
		[Test]
		public void FileAssociationsChange()
		{
			Assert.IsFalse(schemasEditor.XmlFileAssociationsChanged);
		}		
	}
}
