// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using ICSharpCode.Core;
using NUnit.Framework;
using XmlEditor.Tests.Utils;

namespace XmlEditor.Tests.Utils.Tests
{
	[TestFixture]
	public class MockFileSystemTests
	{
		MockFileSystem fileSystem;
		string[] tempDirectoryFiles;
		
		[SetUp]
		public void Init()
		{
			fileSystem = new MockFileSystem();
			tempDirectoryFiles = new string[] { @"c:\temp\a.txt", @"c:\temp\b.txt" };
			fileSystem.AddDirectoryFiles(@"c:\temp", tempDirectoryFiles);	
			
			string[] existingFolders = new string[] { @"c:\temp", @"c:\projects" };
			fileSystem.AddExistingFolders(existingFolders);
		}
		
		[Test]
		public void GetFilesReturnsFilesSpecifiedInAddDirectoryFilesMethodCall()
		{
			Assert.AreEqual(tempDirectoryFiles, fileSystem.GetFiles(DirectoryName.Create(@"c:\temp"), String.Empty).Select(f => f.ToString()));
		}
		
		[Test]
		public void GetFilesReturnsEmptyArrayForUnknownDirectory()
		{
			Assert.AreEqual(new string[0], fileSystem.GetFiles(DirectoryName.Create(@"c:\unknown-dir"), String.Empty));
		}
		
		[Test]
		public void SearchedFoldersRecordedWhenCallingGetFilesMethod()
		{
			string[] expectedSearchedFolders = new string[] { @"c:\temp", @"c:\projects" };
			fileSystem.GetFiles(DirectoryName.Create(@"c:\temp"), String.Empty);
			fileSystem.GetFiles(DirectoryName.Create(@"c:\projects"), String.Empty);
			
			Assert.AreEqual(expectedSearchedFolders, fileSystem.SearchedFolders);
		}
		
		[Test]
		public void SearchPatternsRecordedWhenCallingGetFilesMethod()
		{
			string[] expectedSearchPatterns = new string[] { "*.*", @"*.xml" };
			fileSystem.GetFiles(DirectoryName.Create(@"c:\temp"), "*.*");
			fileSystem.GetFiles(DirectoryName.Create(@"c:\projects"), "*.xml");
			
			Assert.AreEqual(expectedSearchPatterns, fileSystem.SearchedForFileExtensions);
		}
	
		[Test]
		public void UnknownDirectoryDoesNotExist()
		{
			Assert.IsFalse(fileSystem.DirectoryExists(DirectoryName.Create(@"c:\unknown-dir")));
		}
		
		[Test]
		public void FoldersRecordedWhenDirectoryExistsMethodCalled()
		{
			string[] expectedFolders = new string[] { @"c:\temp", @"c:\projects" };
			fileSystem.DirectoryExists(DirectoryName.Create(@"c:\temp"));
			fileSystem.DirectoryExists(DirectoryName.Create(@"c:\projects"));
			
			Assert.AreEqual(expectedFolders, fileSystem.FoldersCheckedThatTheyExist);
		}
		
		[Test]
		public void TempFolderExists()
		{
			Assert.IsTrue(fileSystem.DirectoryExists(DirectoryName.Create(@"c:\temp")));
		}
		
		[Test]
		public void FilesCopiedAreRecorded()
		{
			string firstFileSource = @"c:\temp\a.xsd";
			string firstFileDestination = @"c:\projects\a.xsd";
			fileSystem.CopyFile(FileName.Create(firstFileSource), FileName.Create(firstFileDestination));
			
			string secondFileSource = @"c:\temp\b.xsd";
			string secondFileDestination = @"c:\projects\b.xsd";
			fileSystem.CopyFile(FileName.Create(secondFileSource), FileName.Create(secondFileDestination));
			
			NameValueCollection expectedNameValuePairs = new NameValueCollection();
			expectedNameValuePairs.Add(firstFileSource, firstFileDestination);
			expectedNameValuePairs.Add(secondFileSource, secondFileDestination);
			
			Assert.AreEqual(expectedNameValuePairs, fileSystem.CopiedFileLocations);
		}
		
		[Test]
		public void CopyFileMethodThrowsExceptionIfConfigured()
		{
			ApplicationException ex = new ApplicationException("message");
			fileSystem.ExceptionToThrowWhenCopyFileCalled = ex;
			
			Assert.Throws<ApplicationException>(delegate {	fileSystem.CopyFile(FileName.Create(@"c:\temp\a.xsd"), FileName.Create(@"c:\temp\b.xsd")); });
		}		
	}
}
