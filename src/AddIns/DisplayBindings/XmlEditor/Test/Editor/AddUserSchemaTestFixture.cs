// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.IO;
using System.Xml;

using ICSharpCode.XmlEditor;
using NUnit.Framework;
using XmlEditor.Tests.Utils;

namespace XmlEditor.Tests.Editor
{
	[TestFixture]
	public class AddUserSchemaTestFixture
	{
		RegisteredXmlSchemas registeredSchemas;
		string userDefinedSchemaFolder;
		XmlSchemaCompletion schema;
		int userSchemaAddedEventFiredCount;
		MockFileSystem fileSystem;
		MockXmlSchemaCompletionDataFactory factory;
		
		[SetUp]
		public void Init()
		{
			userDefinedSchemaFolder = @"c:\users\user\sharpdevelop\schemas";
			
			schema = LoadTestSchema();

			userSchemaAddedEventFiredCount = 0;
			fileSystem = new MockFileSystem();
			factory = new MockXmlSchemaCompletionDataFactory();
			registeredSchemas = new RegisteredXmlSchemas(new string[0], userDefinedSchemaFolder, fileSystem, factory);
			registeredSchemas.UserDefinedSchemaAdded += UserSchemaAdded;
			registeredSchemas.AddUserSchema(schema);
		}

		XmlSchemaCompletion LoadTestSchema()
		{
			string xml = "<xs:schema xmlns:xs='http://www.w3.org/2001/XMLSchema' targetNamespace='http://test'>\r\n" +
				"</xs:schema>";
			
			XmlSchemaCompletion schema = new XmlSchemaCompletion(new StringReader(xml));
			schema.FileName = @"c:\projects\schemas\test.xsd";
			return schema;
		}
		
		[Test]
		public void SchemaAddedToSchemaCollection()
		{
			Assert.AreEqual(schema, registeredSchemas.Schemas[0]);
		}
		
		[Test]
		public void SchemaFileNameChangedToNewLocationInUserDefinedSchemaFolder()
		{
			string expectedFileName = Path.Combine(userDefinedSchemaFolder, "test.xsd");
			Assert.AreEqual(expectedFileName, schema.FileName);
		}
		
		[Test]
		public void SchemaCopiedToUserDefinedSchemaFolder()
		{
			string fileName = Path.Combine(userDefinedSchemaFolder, "test.xsd");
			Assert.AreEqual(fileName, fileSystem.CopiedFileLocations[@"c:\projects\schemas\test.xsd"]);
		}
		
		[Test]
		public void UserDefinedSchemaFolderCheckedForExistenceBeforeCopyingFile()
		{
			string[] foldersChecked = new string[] { userDefinedSchemaFolder };
			Assert.AreEqual(foldersChecked, fileSystem.FoldersCheckedThatTheyExist);
		}
		
		[Test]
		public void UserDefinedSchemaFolderCreatedSinceFolderDoesNotExist()
		{
			Assert.AreEqual(userDefinedSchemaFolder, fileSystem.CreatedFolders[0]);
		}
		
		[Test]
		public void AddedSchemaNamespaceExistsInRegisteredSchemas()
		{
			Assert.IsTrue(registeredSchemas.SchemaExists("http://test"));
		}
		
		[Test]
		public void UserSchemaAddedEventFired()
		{
			Assert.AreEqual(1, userSchemaAddedEventFiredCount);
		}
		
		void UserSchemaAdded(object source, EventArgs e)
		{
			userSchemaAddedEventFiredCount++;
		}
	}
}
