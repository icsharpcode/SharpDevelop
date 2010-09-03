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
	public class NoRegisteredSchemasTestFixture
	{
		RegisteredXmlSchemas registeredXmlSchemas;
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
			registeredXmlSchemas = new RegisteredXmlSchemas(readOnlySchemaFolders, @"c:\users\user\sharpdevelop\schemas", fileSystem, factory);
			fileSystem.AddExistingFolders(expectedSearchedFolders);
			registeredXmlSchemas.ReadSchemas();
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
			XmlSchemaCompletion schema = new XmlSchemaCompletion();
			registeredXmlSchemas.AddUserSchema(schema);
			
			Assert.AreEqual(0, fileSystem.CreatedFolders.Count);
		}
	}
}
