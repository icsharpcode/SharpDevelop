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
using System.Runtime.Versioning;
using ICSharpCode.PackageManagement.Design;
using ICSharpCode.PackageManagement.EnvDTE;
using ICSharpCode.PackageManagement.Scripting;
using NuGet;
using NUnit.Framework;
using PackageManagement.Tests.Helpers;

namespace PackageManagement.Tests.Scripting
{
	[TestFixture]
	public class PackageInstallScriptTests
	{
		PackageInstallScriptFileName scriptFileName;
		FakePackageScriptSession fakeSession;
		PackageInstallScript script;
		FakeFileSystem fakeFileSystem;
		FakePackage fakePackage;
		FakePackageManagementProject fakeProject;
		
		void CreateScript()
		{
			fakeFileSystem = new FakeFileSystem();
			fakeFileSystem.FileExistsReturnValue = true;
			fakeFileSystem.DirectoryExistsReturnValue = true;
			scriptFileName = new PackageInstallScriptFileName(fakeFileSystem);
			
			fakeSession = new FakePackageScriptSession();
			fakePackage = new FakePackage();
			
			fakeProject = new FakePackageManagementProject();
			
			script = new PackageInstallScript(fakePackage, scriptFileName);
			script.Project = fakeProject;
		}
		
		void ExecuteScript()
		{
			script.Run(fakeSession);
		}

		[Test]
		public void Execute_PackageInstallDirectoryIsSet_ProjectSessionVariableIsSet()
		{
			CreateScript();
			var expectedProject = new TestableDTEProject();
			var project = new FakePackageManagementProject();
			project.DTEProject = expectedProject;
			script.Project = project;
			ExecuteScript();
			
			var projectVariable = fakeSession.VariablesAdded["__project"];
			
			Assert.AreEqual(expectedProject, projectVariable);
		}
		
		[Test]
		public void Execute_ScriptDoesNotExist_ScriptIsNotExecuted()
		{
			CreateScript();
			fakeFileSystem.FileExistsReturnValue = false;
			ExecuteScript();
			
			bool executed = fakeSession.IsScriptExecuted;
			
			Assert.IsFalse(executed);
		}
		
		[Test]
		public void Exists_ScriptDoesNotExist_ReturnsFalse()
		{
			CreateScript();
			fakeFileSystem.FileExistsReturnValue = false;
			
			bool exists = script.Exists();
			
			Assert.IsFalse(exists);
		}
		
		[Test]
		public void Exists_ScriptDoesExist_ReturnsTrue()
		{
			CreateScript();
			fakeFileSystem.FileExistsReturnValue = true;
			
			bool exists = script.Exists();
			
			Assert.IsTrue(exists);
		}
		
		[Test]
		public void Execute_ScriptDoesNotExist_InstallScriptCheckedForExistence()
		{
			CreateScript();
			fakeFileSystem.FileExistsReturnValue = false;
			ExecuteScript();
			
			string fileChecked = fakeFileSystem.PathPassedToFileExists;
			
			Assert.AreEqual(@"tools\install.ps1", fileChecked);
		}
		
		[Test]
		public void Package_PackagePassedToConstructor_ReturnsPackagePassedToConstructor()
		{
			CreateScript();
			
			IPackage package = script.Package;
			
			Assert.AreEqual(fakePackage, package);
		}
		
		[Test]
		public void Run_PackageHasTargetFrameworkSpecificInstallPowerShellScript_InstallPowerShellScriptIsRun()
		{
			CreateScript();
			fakeFileSystem.Root = @"d:\projects\MyProject\packages\MyPackage.1.0";
			fakePackage.AddFile(@"tools\net40\Install.ps1");
			fakeProject.TargetFramework = new FrameworkName(".NETFramework", new Version("4.0"));
			
			script.Run(fakeSession);
			
			bool result = fakeSession.ScriptPassedToInvokeScript.StartsWith(
				@"& 'd:\projects\MyProject\packages\MyPackage.1.0\tools\net40\Install.ps1'");
			Assert.IsTrue(result, fakeSession.ScriptPassedToInvokeScript);
		}
		
		[Test]
		public void Exists_PackageHasTargetFrameworkSpecificInstallPowerShellScript_ReturnsTrue()
		{
			CreateScript();
			fakeFileSystem.Root = @"d:\projects\MyProject\packages\MyPackage.1.0";
			fakeFileSystem.ExistingFiles.Add(@"tools\net40\install.ps1");
			fakeFileSystem.FileExistsReturnValue = false;
			fakePackage.AddFile(@"tools\net40\install.ps1");
			fakeProject.TargetFramework = new FrameworkName(".NETFramework", new Version("4.0"));
			
			bool exists = script.Exists();
			
			Assert.IsTrue(exists);
		}
	}
}
