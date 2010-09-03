// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
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
			Assert.AreEqual(tempDirectoryFiles, fileSystem.GetFilesInDirectory(@"c:\temp", String.Empty));
		}
		
		[Test]
		public void GetFilesReturnsEmptyArrayForUnknownDirectory()
		{
			Assert.AreEqual(new string[0], fileSystem.GetFilesInDirectory(@"c:\unknown-dir", String.Empty));
		}
		
		[Test]
		public void SearchedFoldersRecordedWhenCallingGetFilesMethod()
		{
			string[] expectedSearchedFolders = new string[] { @"c:\temp", @"c:\projects" };
			fileSystem.GetFilesInDirectory(@"c:\temp", String.Empty);
			fileSystem.GetFilesInDirectory(@"c:\projects", String.Empty);
			
			Assert.AreEqual(expectedSearchedFolders, fileSystem.SearchedFolders);
		}
		
		[Test]
		public void SearchPatternsRecordedWhenCallingGetFilesMethod()
		{
			string[] expectedSearchPatterns = new string[] { "*.*", @"*.xml" };
			fileSystem.GetFilesInDirectory(@"c:\temp", "*.*");
			fileSystem.GetFilesInDirectory(@"c:\projects", "*.xml");
			
			Assert.AreEqual(expectedSearchPatterns, fileSystem.SearchedForFileExtensions);
		}
	
		[Test]
		public void UnknownDirectoryDoesNotExist()
		{
			Assert.IsFalse(fileSystem.DirectoryExists(@"c:\unknown-dir"));
		}
		
		[Test]
		public void FoldersRecordedWhenDirectoryExistsMethodCalled()
		{
			string[] expectedFolders = new string[] { @"c:\temp", @"c:\projects" };
			fileSystem.DirectoryExists(@"c:\temp");
			fileSystem.DirectoryExists(@"c:\projects");
			
			Assert.AreEqual(expectedFolders, fileSystem.FoldersCheckedThatTheyExist);
		}
		
		[Test]
		public void TempFolderExists()
		{
			Assert.IsTrue(fileSystem.DirectoryExists(@"c:\temp"));
		}
		
		[Test]
		public void FilesCopiedAreRecorded()
		{
			string firstFileSource = @"c:\temp\a.xsd";
			string firstFileDestination = @"c:\projects\a.xsd";
			fileSystem.CopyFile(firstFileSource, firstFileDestination);
			
			string secondFileSource = @"c:\temp\b.xsd";
			string secondFileDestination = @"c:\projects\b.xsd";
			fileSystem.CopyFile(secondFileSource, secondFileDestination);
			
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
			
			Assert.Throws<ApplicationException>(delegate {	fileSystem.CopyFile(@"c:\temp\a.xsd", @"c:\temp\b.xsd"); });
		}		
	}
}
