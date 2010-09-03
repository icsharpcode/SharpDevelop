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
	public class RegisteredSchemaWithSameNamespaceAddedTwiceTestFixture
	{
		RegisteredXmlSchemas registeredXmlSchemas;
		MockFileSystem fileSystem;
		MockXmlSchemaCompletionDataFactory factory;
		string duplicateTestSchemaFileName;

		[SetUp]
		public void Init()
		{
			string userDefinedSchemaFolder = @"c:\users\user\schemas";
			
			fileSystem = new MockFileSystem();
			factory = new MockXmlSchemaCompletionDataFactory();
			registeredXmlSchemas = new RegisteredXmlSchemas(new string[0], userDefinedSchemaFolder, fileSystem, factory);
			
			fileSystem.AddExistingFolder(userDefinedSchemaFolder);
			
			string testSchemaFileName = Path.Combine(userDefinedSchemaFolder, "test.xsd");
			duplicateTestSchemaFileName = Path.Combine(userDefinedSchemaFolder, "test2.xsd");
			string[] userDefinedSchemaFiles = new string[] { testSchemaFileName, duplicateTestSchemaFileName };
			fileSystem.AddDirectoryFiles(userDefinedSchemaFolder, userDefinedSchemaFiles);
			
			factory.AddSchemaXml(testSchemaFileName,
				"<xs:schema xmlns:xs='http://www.w3.org/2001/XMLSchema' targetNamespace='http://test' />");
			
			factory.AddSchemaXml(duplicateTestSchemaFileName,
				"<xs:schema xmlns:xs='http://www.w3.org/2001/XMLSchema' targetNamespace='http://test' />");

			fileSystem.AddExistingFile(testSchemaFileName, false);
			fileSystem.AddExistingFile(duplicateTestSchemaFileName, false);
			
			registeredXmlSchemas.ReadSchemas();
		}
		
		[Test]
		public void NoSchemaErrorsRecordedForDuplicateSchemaNamespace()
		{
			Assert.AreEqual(0, registeredXmlSchemas.GetSchemaErrors().Length);
		}
	}
}
