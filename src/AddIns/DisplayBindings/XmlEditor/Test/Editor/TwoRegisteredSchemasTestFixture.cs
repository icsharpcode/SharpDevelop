// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.IO;

using ICSharpCode.XmlEditor;
using NUnit.Framework;
using XmlEditor.Tests.Utils;

namespace XmlEditor.Tests.Editor
{
	[TestFixture]
	public class TwoRegisteredSchemasTestFixture
	{
		XmlSchemaManager schemaManager;
		MockFileSystem fileSystem;
		MockXmlSchemaCompletionDataFactory factory;
		string addinSchemaFileName;
		string testSchemaFileName;
		int userSchemaRemovedEventFiredCount;
		
		[SetUp]
		public void Init()
		{
			string sharpDevelopSchemaFolder = @"c:\sharpdevelop\schemas";
			string[] schemaFolders = new string[] { @"c:\sharpdevelop\schemas" };
			string userDefinedSchemaFolder = @"c:\users\user\schemas";
			
			fileSystem = new MockFileSystem();
			factory = new MockXmlSchemaCompletionDataFactory();
			schemaManager = new XmlSchemaManager(schemaFolders, userDefinedSchemaFolder, fileSystem, factory);
			
			fileSystem.AddExistingFolders(schemaFolders);
			fileSystem.AddExistingFolder(userDefinedSchemaFolder);
			
			addinSchemaFileName = Path.Combine(sharpDevelopSchemaFolder, "addin.xsd");
			string[] sharpDevelopSchemaFiles = new string[] { addinSchemaFileName };
			fileSystem.AddDirectoryFiles(sharpDevelopSchemaFolder, sharpDevelopSchemaFiles);
			
			testSchemaFileName = Path.Combine(userDefinedSchemaFolder, "test.xsd");
			string[] userDefinedSchemaFiles = new string[] { testSchemaFileName };
			fileSystem.AddDirectoryFiles(userDefinedSchemaFolder, userDefinedSchemaFiles);
			
			factory.AddSchemaXml(addinSchemaFileName,
				"<xs:schema xmlns:xs='http://www.w3.org/2001/XMLSchema' targetNamespace='http://addin' />");
			factory.AddSchemaXml(testSchemaFileName,
				"<xs:schema xmlns:xs='http://www.w3.org/2001/XMLSchema' targetNamespace='http://test' />");
			
			fileSystem.AddExistingFile(addinSchemaFileName, true);
			fileSystem.AddExistingFile(testSchemaFileName, false);
			
			schemaManager.ReadSchemas();
			
			userSchemaRemovedEventFiredCount = 0;
			schemaManager.UserSchemaRemoved += UserSchemaRemoved;
		}
		
		[Test]
		public void TwoSchemasRegistered()
		{
			Assert.AreEqual(2, schemaManager.Schemas.Count);
		}
		
		[Test]
		public void SharpDevelopSchemaIsReadOnly()
		{
			Assert.IsTrue(schemaManager.Schemas["http://addin"].ReadOnly);
		}
		
		[Test]
		public void UserDefinedSchemaIsNotReadOnly()
		{
			Assert.IsFalse(schemaManager.Schemas["http://test"].ReadOnly);
		}
		
		[Test]
		public void BaseUriUsedWhenCreatingXmlSchemaCompletionData()
		{
			string baseUri = XmlSchemaCompletionData.GetUri(addinSchemaFileName);
			Assert.AreEqual(baseUri, factory.GetBaseUri(addinSchemaFileName));
		}
		
		[Test]
		public void RemoveAddInSchemaRemovesSchemaFromSchemaCollection()
		{
			schemaManager.RemoveUserSchema("http://addin");
			Assert.IsFalse(schemaManager.SchemaExists("http://addin"));
		}
		
		[Test]
		public void RemoveUnknownSchemaDoesNothing()
		{
			schemaManager.RemoveUserSchema("Unknown-schema-namespace");
		}
		
		[Test]
		public void RemoveAddInSchemaCausesUserDefinedSchemaFileToBeDeleted()
		{
			schemaManager.RemoveUserSchema("http://addin");
			string[] expectedDeletedFiles = new string[] { addinSchemaFileName };

			Assert.AreEqual(expectedDeletedFiles, fileSystem.DeletedFiles);
		}
		
		[Test]
		public void RemoveAddInSchemaChecksIfUserDefinedSchemaFileExists()
		{
			schemaManager.RemoveUserSchema("http://addin");
			string[] expectedFiles = new string[] { addinSchemaFileName };

			Assert.AreEqual(expectedFiles, fileSystem.FilesCheckedThatTheyExist);
		}
		
		[Test]
		public void RemoveTestSchemaDoesNotDeleteNonExistentFile()
		{
			schemaManager.RemoveUserSchema("http://test");
			Assert.AreEqual(0, fileSystem.DeletedFiles.Length);
		}
		
		[Test]
		public void RemoveTestSchemaRemovesSchemaFromSchemaCollection()
		{
			schemaManager.RemoveUserSchema("http://test");
			Assert.IsFalse(schemaManager.SchemaExists("http://test"));
		}
		
		[Test]
		public void RemoveTestSchemaFiresUserSchemaRemovedEvent()
		{
			schemaManager.RemoveUserSchema("http://test");			
			Assert.AreEqual(1, userSchemaRemovedEventFiredCount);
		}
		
		void UserSchemaRemoved(object source, EventArgs e)
		{
			userSchemaRemovedEventFiredCount++;
		}
	}
}
