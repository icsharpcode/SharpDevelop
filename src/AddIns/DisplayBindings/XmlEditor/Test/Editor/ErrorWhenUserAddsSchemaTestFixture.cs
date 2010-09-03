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
	public class ErrorWhenUserAddsSchemaTestFixture
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
			XmlSchemaFileAssociations associations = new XmlSchemaFileAssociations(new Properties(), new DefaultXmlSchemaFileAssociations(null), schemas);
			panel = new MockXmlSchemasPanel();
			
			schemasEditor = new RegisteredXmlSchemasEditor(registeredXmlSchemas, new string[0], associations, panel, factory);
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
		public void SchemaWithEmptyNamespaceNotAddedToRegisteredXmlSchemas()
		{
			ErrorMessageDisplayedWhenSchemaHasEmptyNamespace();
			schemasEditor.SaveOptions();
			
			Assert.AreEqual(0, registeredXmlSchemas.Schemas.Count);
		}
				
		[Test]
		public void ErrorMessageDisplayedWhenSchemaAlreadyExistsInRegisteredXmlSchemas()
		{
			string xml = "<xs:schema xmlns:xs='http://www.w3.org/2001/XMLSchema' targetNamespace='http://test'/>";
			AddSchemaToRegisteredXmlSchemas(xml);
			AddSchema(xml);
		
			FormattedErrorMessage expectedErrorMessage = new FormattedErrorMessage("${res:ICSharpCode.XmlEditor.XmlSchemasPanel.NamespaceExists}", "http://test");
			Assert.AreEqual(expectedErrorMessage, panel.FormattedErrorMessage);
		}
		
		void AddSchemaToRegisteredXmlSchemas(string xml)
		{
			XmlSchemaCompletion schema = new XmlSchemaCompletion(new StringReader(xml));
			registeredXmlSchemas.Schemas.Add(schema);
		}
		
		[Test]
		public void SchemaThatAlreadyExistsIsNotAddedToRegisteredXmlSchemas()
		{
			ErrorMessageDisplayedWhenSchemaAlreadyExistsInRegisteredXmlSchemas();
			schemasEditor.SaveOptions();
			
			Assert.AreEqual(1, registeredXmlSchemas.Schemas.Count);
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
