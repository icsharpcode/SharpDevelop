// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

using System;
using ICSharpCode.PackageManagement.Design;
using ICSharpCode.PackageManagement.Scripting;
using NUnit.Framework;

namespace PackageManagement.Tests.Scripting
{
	[TestFixture]
	public class PackageInitializeScriptFileNameTests
	{
		PackageInitializeScriptFileName scriptFileName;
		FakeFileSystem fakeFileSystem;

		void CreateFileSystem()
		{
			fakeFileSystem = new FakeFileSystem();
		}
		
		void CreateFileName(string installPath)
		{
			scriptFileName = new PackageInitializeScriptFileName(installPath);
		}
		
		void CreateFakeFileSystem()
		{
			fakeFileSystem = new FakeFileSystem();
		}
		
		void CreateFileNameWithFakeFileSystem()
		{
			scriptFileName = new PackageInitializeScriptFileName(fakeFileSystem);
		}
		
		[Test]
		public void ToString_InstallDirectoryPassed_ReturnsFullPathToInitScript()
		{
			CreateFileName(@"d:\projects\MyProject\packages\Test");
			
			string actualFileName = scriptFileName.ToString();
			string expectedFileName = @"d:\projects\MyProject\packages\Test\tools\init.ps1";
			
			Assert.AreEqual(expectedFileName, actualFileName);
		}
		
		[Test]
		public void PackageInstallDirectory_InstallDirectoryPassed_ReturnsFullPathToInitScript()
		{
			CreateFileName(@"d:\projects\MyProject\packages\Test");
			
			string actualDirectory = scriptFileName.PackageInstallDirectory;
			string expectedDirectory = @"d:\projects\MyProject\packages\Test";
			
			Assert.AreEqual(expectedDirectory, actualDirectory);
		}
		
		[Test]
		public void ScriptDirectoryExists_FileSystemHasScriptDirectory_ReturnsTrue()
		{
			CreateFakeFileSystem();
			fakeFileSystem.DirectoryExistsReturnValue = true;
			CreateFileNameWithFakeFileSystem();
			
			bool exists = scriptFileName.ScriptDirectoryExists();
			
			Assert.IsTrue(exists);
		}
		
		[Test]
		public void ScriptDirectoryExists_FileSystemDoesNotHaveScriptDirectory_ReturnsFalse()
		{
			CreateFakeFileSystem();
			fakeFileSystem.DirectoryExistsReturnValue = false;
			CreateFileNameWithFakeFileSystem();
			
			bool exists = scriptFileName.ScriptDirectoryExists();
			
			Assert.IsFalse(exists);
		}
		
		[Test]
		public void ScriptDirectoryExists_FileSystemHasScriptDirectory_ToolsDirectoryCheckedForExistence()
		{
			CreateFakeFileSystem();
			fakeFileSystem.DirectoryExistsReturnValue = true;
			CreateFileNameWithFakeFileSystem();
			
			scriptFileName.ScriptDirectoryExists();
			
			Assert.AreEqual("tools", fakeFileSystem.PathPassedToDirectoryExists);
		}
		
		[Test]
		public void FileExists_FileSystemHasScriptFile_ReturnsTrue()
		{
			CreateFakeFileSystem();
			fakeFileSystem.FileExistsReturnValue = true;
			CreateFileNameWithFakeFileSystem();
			
			bool exists = scriptFileName.FileExists();
			
			Assert.IsTrue(exists);	
		}
		
		[Test]
		public void FileExists_FileSystemDoesNotHaveScriptFile_ReturnsFalse()
		{
			CreateFakeFileSystem();
			fakeFileSystem.FileExistsReturnValue = false;
			CreateFileNameWithFakeFileSystem();
			
			bool exists = scriptFileName.FileExists();
			
			Assert.IsFalse(exists);	
		}
		
		[Test]
		public void FileExists_FileSystemHasScriptFile_ToolsScriptFileCheckedForExistence()
		{
			CreateFakeFileSystem();
			fakeFileSystem.FileExistsReturnValue = true;
			CreateFileNameWithFakeFileSystem();
			
			scriptFileName.FileExists();
			
			Assert.AreEqual(@"tools\init.ps1", fakeFileSystem.PathPassedToFileExists);
		}
		
		[Test]
		public void GetScriptDirectory_InstallDirectoryPassed_ReturnsInitScriptDirectory()
		{
			CreateFileName(@"d:\projects\MyProject\packages\Test");
			
			string actualDirectory = scriptFileName.GetScriptDirectory();
			string expectedDirectory = @"d:\projects\MyProject\packages\Test\tools";
			
			Assert.AreEqual(expectedDirectory, actualDirectory);
		}
	}
}
