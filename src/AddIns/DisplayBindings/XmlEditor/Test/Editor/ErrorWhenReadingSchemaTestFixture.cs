// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using ICSharpCode.XmlEditor;
using NUnit.Framework;
using XmlEditor.Tests.Utils;

namespace XmlEditor.Tests.Editor
{
	[TestFixture]
	public class ErrorWhenReadingSchemaTestFixture
	{
		MockXmlSchemaCompletionDataFactory factory;
		RegisteredXmlSchemas registeredXmlSchemas;
		MockFileSystem fileSystem;
		ApplicationException exceptionThrownWhenReadingSchema;
		
		[SetUp]
		public void Init()
		{
			string[] schemaFolders = new string[] { @"d:\projects\schemas" };
			
			exceptionThrownWhenReadingSchema = new ApplicationException("Read schema failed.");
			
			factory = new MockXmlSchemaCompletionDataFactory();
			factory.ExceptionToThrowWhenCreateXmlSchemaCalled = exceptionThrownWhenReadingSchema;
			
			fileSystem = new MockFileSystem();
			fileSystem.AddExistingFolders(schemaFolders);
			
			string[] sharpDevelopSchemaFiles = new string[] { @"d:\projects\schemas\addin.xsd" };
			fileSystem.AddDirectoryFiles(@"d:\projects\schemas", sharpDevelopSchemaFiles);
			
			registeredXmlSchemas = new RegisteredXmlSchemas(schemaFolders, String.Empty, fileSystem, factory);
			registeredXmlSchemas.ReadSchemas();
		}
		
		[Test]
		public void OneErrorReportedWhenReadingSchemas()
		{
			string message = @"Unable to read schema 'd:\projects\schemas\addin.xsd'.";
			RegisteredXmlSchemaError error = new RegisteredXmlSchemaError(message, exceptionThrownWhenReadingSchema);
			RegisteredXmlSchemaError[] expectedErrors = new RegisteredXmlSchemaError[] { error };
			Assert.AreEqual(expectedErrors, registeredXmlSchemas.GetSchemaErrors());
		}
	}
}
