// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.IO;

using ICSharpCode.XmlEditor;
using NUnit.Framework;
using XmlEditor.Tests.Utils;

namespace XmlEditor.Tests.Editor
{
	[TestFixture]
	public class RegisteredSchemaWithEmptyNamespaceTestFixture
	{
		RegisteredXmlSchemas registeredXmlSchemas;
		MockFileSystem fileSystem;
		MockXmlSchemaCompletionDataFactory factory;
		string testSchemaFileName;

		[SetUp]
		public void Init()
		{
			string userDefinedSchemaFolder = @"c:\users\user\schemas";
			
			fileSystem = new MockFileSystem();
			factory = new MockXmlSchemaCompletionDataFactory();
			registeredXmlSchemas = new RegisteredXmlSchemas(new string[0], userDefinedSchemaFolder, fileSystem, factory);
			
			fileSystem.AddExistingFolder(userDefinedSchemaFolder);
						
			testSchemaFileName = Path.Combine(userDefinedSchemaFolder, "test.xsd");
			string[] userDefinedSchemaFiles = new string[] { testSchemaFileName };
			fileSystem.AddDirectoryFiles(userDefinedSchemaFolder, userDefinedSchemaFiles);
			
			factory.AddSchemaXml(testSchemaFileName,
				"<xs:schema xmlns:xs='http://www.w3.org/2001/XMLSchema' />");
			
			fileSystem.AddExistingFile(testSchemaFileName, false);
			
			registeredXmlSchemas.ReadSchemas();
		}
		
		[Test]
		public void SchemaWithNoNamespaceRecordedAsError()
		{
			string message = "Ignoring schema with no namespace '" + testSchemaFileName + "'.";
			RegisteredXmlSchemaError error = new RegisteredXmlSchemaError(message);
			RegisteredXmlSchemaError[] expectedErrors = new RegisteredXmlSchemaError[] { error };
			Assert.AreEqual(expectedErrors, registeredXmlSchemas.GetSchemaErrors());
		}
	}
}
