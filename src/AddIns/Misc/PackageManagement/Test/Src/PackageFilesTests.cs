// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using ICSharpCode.PackageManagement;
using ICSharpCode.PackageManagement.Design;
using NuGet;
using NUnit.Framework;

namespace PackageManagement.Tests
{
	[TestFixture]
	public class PackageFilesTests
	{
		PackageFiles packageFiles;
		
		void CreatePackageFiles(FakePackage package)
		{
			packageFiles = new PackageFiles(package);
		}
		
		void CreatePackageFilesWithOneFile(string fileName)
		{
			var package = new FakePackage();
			package.AddFile(fileName);
			CreatePackageFiles(package);
		}
		
		void CreatePackageFilesWithTwoFiles(string fileName1, string fileName2)
		{
			var package = new FakePackage();
			package.AddFile(fileName1);
			package.AddFile(fileName2);
			CreatePackageFiles(package);
		}
		
		[Test]
		public void HasAnyPackageScripts_HasOnePowerShellInitScript_ReturnsTrue()
		{
			CreatePackageFilesWithOneFile(@"tools\init.ps1");
			
			bool hasScripts = packageFiles.HasAnyPackageScripts();
			
			Assert.IsTrue(hasScripts);
		}
		
		[Test]
		public void HasAnyPackageScripts_HasOneCSharpFile_ReturnsFalse()
		{
			CreatePackageFilesWithOneFile(@"src\test.cs");
			
			bool hasScripts = packageFiles.HasAnyPackageScripts();
			
			Assert.IsFalse(hasScripts);
		}
		
		[Test]
		public void HasAnyPackageScripts_HasOnePowerShellInitScriptWithDifferentParentFolder_ReturnsTrue()
		{
			CreatePackageFilesWithOneFile(@"parentfolder\init.ps1");
			
			bool hasScripts = packageFiles.HasAnyPackageScripts();
			
			Assert.IsTrue(hasScripts);
		}
		
		[Test]
		public void HasAnyPackageScripts_HasOnePowerShellInitScriptInUpperCase_ReturnsTrue()
		{
			CreatePackageFilesWithOneFile(@"tools\INIT.PS1");
			
			bool hasScripts = packageFiles.HasAnyPackageScripts();
			
			Assert.IsTrue(hasScripts);
		}
		
		[Test]
		public void HasAnyPackageScripts_HasOnePowerShellInstallScript_ReturnsTrue()
		{
			CreatePackageFilesWithOneFile(@"tools\install.ps1");
			
			bool hasScripts = packageFiles.HasAnyPackageScripts();
			
			Assert.IsTrue(hasScripts);
		}
		
		[Test]
		public void HasAnyPackageScripts_HasOnePowerShellInstallScriptInUpperCase_ReturnsTrue()
		{
			CreatePackageFilesWithOneFile(@"tools\INSTALL.PS1");
			
			bool hasScripts = packageFiles.HasAnyPackageScripts();
			
			Assert.IsTrue(hasScripts);
		}
		
		[Test]
		public void HasAnyPackageScripts_HasOnePowerShellUninstallScript_ReturnsTrue()
		{
			CreatePackageFilesWithOneFile(@"tools\uninstall.ps1");
			
			bool hasScripts = packageFiles.HasAnyPackageScripts();
			
			Assert.IsTrue(hasScripts);
		}
		
		[Test]
		public void HasAnyPackageScripts_HasOnePowerShellUninstallScriptInUpperCase_ReturnsTrue()
		{
			CreatePackageFilesWithOneFile(@"tools\UNINSTALL.PS1");
			
			bool hasScripts = packageFiles.HasAnyPackageScripts();
			
			Assert.IsTrue(hasScripts);
		}
		
		[Test]
		public void HasAnyPackageScripts_SecondFileIsPowerShellInitScript_ReturnsTrue()
		{
			CreatePackageFilesWithTwoFiles(@"src\test.cs", @"tools\init.ps1");
			
			bool hasScripts = packageFiles.HasAnyPackageScripts();
			
			Assert.IsTrue(hasScripts);
		}
		
		[Test]
		public void HasUninstallPackageScript_HasOnePowerShellUninstallScript_ReturnsTrue()
		{
			CreatePackageFilesWithOneFile(@"tools\uninstall.ps1");
			
			bool hasScript = packageFiles.HasUninstallPackageScript();
			
			Assert.IsTrue(hasScript);
		}
		
		[Test]
		public void HasUninstallPackageScript_HasOneCSharpFile_ReturnsFalse()
		{
			CreatePackageFilesWithOneFile(@"tools\test.cs");
			
			bool hasScript = packageFiles.HasUninstallPackageScript();
			
			Assert.IsFalse(hasScript);
		}
	}
}
