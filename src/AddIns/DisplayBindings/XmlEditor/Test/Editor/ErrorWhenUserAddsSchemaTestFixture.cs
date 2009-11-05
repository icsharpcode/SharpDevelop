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
	public class ErrorWhenUserAddsSchemaTestFixture
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
			XmlEditorOptions options = new XmlEditorOptions(new Properties(), new DefaultXmlSchemaFileAssociations(null), schemas);
			panel = new MockXmlSchemasPanel();
			
			schemasEditor = new RegisteredXmlSchemasEditor(schemaManager, new string[0], options, panel, factory);
			schemasEditor.LoadOptions();
						
			panel.OpenFileDialogFileNameToReturn = @"c:\temp\schema.xsd";
			panel.OpenFileDialogResultToReturn = true;
			
		}
		
		[Test]
		public void ErrorMessageDisplayedWhenSchemaHasEmptyNamespace()
		{
			AddSchema("<xs:schema xmlns:xs='http://www.w3.org/2001/XMLSchema' targetNamespace='' />");
			
			FormattedErrorMessage expectedErrorMessage = new FormattedErrorMessage("${res:ICSharpCode.XmlEditor.XmlSchemasPanel.NoTargetNamespace}", "schema.xsd");
			Assert.AreEqual(expectedErrorMessage, panel.FormattedErrorMessage);
		}

		void AddSchema(string schema)
		{
			factory.AddSchemaXml(panel.OpenFileDialogFileNameToReturn, schema);
			schemasEditor.AddSchemaFromFileSystem();
		}

		[Test]
		public void SchemaWithEmptyNamespaceNotAddedToSchemaManager()
		{
			ErrorMessageDisplayedWhenSchemaHasEmptyNamespace();
			schemasEditor.SaveOptions();
			
			Assert.AreEqual(0, schemaManager.Schemas.Count);
		}
				
		[Test]
		public void ErrorMessageDisplayedWhenSchemaAlreadyExistsInSchemaManager()
		{
			string xml = "<xs:schema xmlns:xs='http://www.w3.org/2001/XMLSchema' targetNamespace='http://test'/>";
			AddSchemaToSchemaManager(xml);
			AddSchema(xml);
		
			FormattedErrorMessage expectedErrorMessage = new FormattedErrorMessage("${res:ICSharpCode.XmlEditor.XmlSchemasPanel.NamespaceExists}", "http://test");
			Assert.AreEqual(expectedErrorMessage, panel.FormattedErrorMessage);
		}
		
		void AddSchemaToSchemaManager(string xml)
		{
			XmlSchemaCompletionData schema = new XmlSchemaCompletionData(new StringReader(xml));
			schemaManager.Schemas.Add(schema);
		}
		
		[Test]
		public void SchemaThatAlreadyExistsIsNotAddedToSchemaManager()
		{
			ErrorMessageDisplayedWhenSchemaAlreadyExistsInSchemaManager();
			schemasEditor.SaveOptions();
			
			Assert.AreEqual(1, schemaManager.Schemas.Count);
		}
		
		[Test]
		public void ErrorMessageDisplayedWhenSchemaWithSameNamespaceAddedTwice()
		{
			string xml = "<xs:schema xmlns:xs='http://www.w3.org/2001/XMLSchema' targetNamespace='http://test'/>";
			AddSchema(xml);
			panel.OpenFileDialogFileNameToReturn = @"c:\temp\schema2.xsd";
			AddSchema(xml);
		
			FormattedErrorMessage expectedErrorMessage = new FormattedErrorMessage("${res:ICSharpCode.XmlEditor.XmlSchemasPanel.NamespaceExists}", "http://test");
			Assert.AreEqual(expectedErrorMessage, panel.FormattedErrorMessage);
		}
		
		[Test]
		public void ExceptionThrownWhenLoadingSchemaFromFileSystem()
		{
			string xml = "<xs:schema xmlns:xs='http://www.w3.org/2001/XMLSchema' targetNamespace='http://test'/>";
			ApplicationException ex = new ApplicationException("message");
			factory.ExceptionToThrowWhenCreateXmlSchemaCalled = ex;
			AddSchema(xml);
			
			string message = "${res:ICSharpCode.XmlEditor.XmlSchemasPanel.UnableToAddSchema}\n\n" + ex.Message;
			Assert.AreEqual(message, panel.ErrorMessage);
		}
		
	}
}
