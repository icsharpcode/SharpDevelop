// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

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
		XmlSchemaManager schemaManager;
		MockFileSystem fileSystem;

		[SetUp]
		public void Init()
		{
			fileSystem = new MockFileSystem();
			factory = new MockXmlSchemaCompletionDataFactory();
			schemaManager = new XmlSchemaManager(new string[0], @"c:\users\user\sharpdevelop\schemas", fileSystem, factory);
		
			XmlSchemaCompletionDataCollection schemas = new XmlSchemaCompletionDataCollection();
			string testSchemaXml = "<xs:schema xmlns:xs='http://www.w3.org/2001/XMLSchema' targetNamespace='http://test' />";
			XmlSchemaCompletionData schema = new XmlSchemaCompletionData(new StringReader(testSchemaXml));
			schema.ReadOnly = false;
			schemas.Add(schema);
				
			XmlEditorOptions options = new XmlEditorOptions(new Properties(), new DefaultXmlSchemaFileAssociations(null), schemas);
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
