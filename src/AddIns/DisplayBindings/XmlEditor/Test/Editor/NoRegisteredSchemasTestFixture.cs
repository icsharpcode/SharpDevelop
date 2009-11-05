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
	public class NoRegisteredSchemasTestFixture
	{
		XmlSchemaManager schemaManager;
		MockFileSystem fileSystem;
		MockXmlSchemaCompletionDataFactory factory;
		string[] expectedSearchedFolders;
	
		[SetUp]
		public void Init()
		{
			expectedSearchedFolders = new string[] { @"c:\sharpdevelop\data-dir\schemas",
				@"c:\users\user\sharpdevelop\schemas" };

			string[] readOnlySchemaFolders = new String[] { @"c:\folder-does-not-exist",
				@"c:\sharpdevelop\data-dir\schemas" };
			
			fileSystem = new MockFileSystem();
			factory = new MockXmlSchemaCompletionDataFactory();
			schemaManager = new XmlSchemaManager(readOnlySchemaFolders, @"c:\users\user\sharpdevelop\schemas", fileSystem, factory);
			fileSystem.AddExistingFolders(expectedSearchedFolders);
			schemaManager.ReadSchemas();
		}
		
		[Test]
		public void FoldersSearchedForSchemas()
		{
			Assert.AreEqual(expectedSearchedFolders, fileSystem.SearchedFolders);
		}
		
		[Test]
		public void FoldersCheckedThatTheyExist()
		{
			string[] expectedFoldersChecked = new string[] { @"c:\folder-does-not-exist",
				@"c:\sharpdevelop\data-dir\schemas",
				@"c:\users\user\sharpdevelop\schemas" };
			
			Assert.AreEqual(expectedFoldersChecked, fileSystem.FoldersCheckedThatTheyExist);
		}
		
		[Test]
		public void XsdFilesAreSearchedForInReadOnlyFoldersAndAllFilesInUserDefinedSchema()
		{
			string[] expectedExtensions = new string[] { "*.xsd", "*.*" };
			Assert.AreEqual(expectedExtensions, fileSystem.SearchedForFileExtensions);
		}
		
		[Test]
		public void AddingNewUserSchemaDoesNotCreateUserDefinedSchemaDirectory()
		{
			XmlSchemaCompletionData schema = new XmlSchemaCompletionData();
			schemaManager.AddUserSchema(schema);
			
			Assert.AreEqual(0, fileSystem.CreatedFolders.Count);
		}
	}
}
