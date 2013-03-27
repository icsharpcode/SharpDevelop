// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
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
		
		void CreateScript()
		{
			fakeFileSystem = new FakeFileSystem();
			fakeFileSystem.FileExistsReturnValue = true;
			fakeFileSystem.DirectoryExistsReturnValue = true;
			scriptFileName = new PackageInstallScriptFileName(fakeFileSystem);
			
			fakeSession = new FakePackageScriptSession();
			fakePackage = new FakePackage();
			
			script = new PackageInstallScript(fakePackage, scriptFileName);
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
	}
}
