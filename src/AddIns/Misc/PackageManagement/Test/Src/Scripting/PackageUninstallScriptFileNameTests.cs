// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.PackageManagement.Design;
using ICSharpCode.PackageManagement.Scripting;
using NUnit.Framework;

namespace PackageManagement.Tests.Scripting
{
	[TestFixture]
	public class PackageUninstallScriptFileNameTests
	{
		PackageUninstallScriptFileName scriptFileName;
		FakeFileSystem fakeFileSystem;

		void CreateFileSystem()
		{
			fakeFileSystem = new FakeFileSystem();
		}
		
		void CreateFileName(string installPath)
		{
			scriptFileName = new PackageUninstallScriptFileName(installPath);
		}
		
		void CreateFakeFileSystem()
		{
			fakeFileSystem = new FakeFileSystem();
		}
		
		void CreateFileNameWithFakeFileSystem()
		{
			scriptFileName = new PackageUninstallScriptFileName(fakeFileSystem);
		}
		
		[Test]
		public void ToString_InstallDirectoryPassed_ReturnsFullPathToInstallScript()
		{
			CreateFileName(@"d:\projects\MyProject\packages\Test");
			
			string actualFileName = scriptFileName.ToString();
			string expectedFileName = @"d:\projects\MyProject\packages\Test\tools\uninstall.ps1";
			
			Assert.AreEqual(expectedFileName, actualFileName);
		}
		
		[Test]
		public void FileExists_FileSystemHasScriptFile_ToolsScriptFileCheckedForExistence()
		{
			CreateFakeFileSystem();
			fakeFileSystem.FileExistsReturnValue = true;
			CreateFileNameWithFakeFileSystem();
			
			scriptFileName.FileExists();
			
			Assert.AreEqual(@"tools\uninstall.ps1", fakeFileSystem.PathPassedToFileExists);
		}
	}
}
