// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.PackageManagement.Design;
using ICSharpCode.PackageManagement.Scripting;
using NUnit.Framework;
using PackageManagement.Tests.Helpers;

namespace PackageManagement.Tests.Scripting
{
	[TestFixture]
	public class PackageInitializeScriptTests
	{
		PackageInitializeScript script;
		PackageInitializeScriptFileName scriptFileName;
		FakeFileSystem fakeFileSystem;
		FakePackageScriptSession fakeSession;
		
		void CreateScript()
		{
			fakeFileSystem = new FakeFileSystem();
			fakeFileSystem.FileExistsReturnValue = true;
			fakeFileSystem.DirectoryExistsReturnValue = true;
			scriptFileName = new PackageInitializeScriptFileName(fakeFileSystem);
			
			fakeSession = new FakePackageScriptSession();
			
			script = new PackageInitializeScript(scriptFileName, fakeSession);
		}
		
		void CreateScriptWithPhysicalFileSystem()
		{
			CreateScriptWithPhysicalFileSystem(@"d:\projects\myproject\packages\test");
		}
		
		void CreateScriptWithPhysicalFileSystem(string packageInstallDirectory)
		{
			scriptFileName = new PackageInitializeScriptFileName(packageInstallDirectory);
			fakeSession = new FakePackageScriptSession();
			script = new PackageInitializeScript(scriptFileName, fakeSession);
		}
		
		void AssertSessionVariableIsRemoved(string variableName)
		{
			bool removed = fakeSession.VariablesRemoved.Contains(variableName);
			Assert.IsTrue(removed);			
		}
		
		[Test]
		public void Execute_ExistingEnvironmentPathIsEmptyString_PathToScriptAddedToEnvironmentPath()
		{
			CreateScript();
			fakeFileSystem.PathToReturnFromGetFullPath = @"d:\projects\myproject\packages\test";
			fakeSession.SetEnvironmentPath(String.Empty);
			
			script.Execute();
			
			string actualEnvironmentPath = fakeSession.GetEnvironmentPath();
			string expectedEnvironmentPath = @"d:\projects\myproject\packages\test";
			
			Assert.AreEqual(expectedEnvironmentPath, actualEnvironmentPath);
		}
		
		[Test]
		public void Execute_OneItemInOriginalEnvironmentPath_PathToScriptAppendedToEnvironmentPath()
		{
			CreateScript();
			fakeFileSystem.PathToReturnFromGetFullPath = @"d:\projects\myproject\packages\test";
			fakeSession.SetEnvironmentPath(@"c:\users\sharpdevelop\ps;");
			
			script.Execute();
			
			string actualEnvironmentPath = fakeSession.GetEnvironmentPath();
			string expectedEnvironmentPath = @"c:\users\sharpdevelop\ps;d:\projects\myproject\packages\test";
			
			Assert.AreEqual(expectedEnvironmentPath, actualEnvironmentPath);
		}
		
		[Test]
		public void Execute_OneItemInOriginalEnvironmentPathMissingSemiColonAtEnd_PathToScriptAppendedToEnvironmentPathWithSemiColonAtStart()
		{
			CreateScript();
			fakeFileSystem.PathToReturnFromGetFullPath = @"d:\projects\myproject\packages\test";
			fakeSession.SetEnvironmentPath(@"c:\users\sharpdevelop\ps");
			
			script.Execute();
			
			string actualEnvironmentPath = fakeSession.GetEnvironmentPath();
			string expectedEnvironmentPath = @"c:\users\sharpdevelop\ps;d:\projects\myproject\packages\test";
			
			Assert.AreEqual(expectedEnvironmentPath, actualEnvironmentPath);
		}
		
		[Test]
		public void Execute_OriginalEnvironmentPathIsNull_PathToScriptAppendedToEnvironmentPath()
		{
			CreateScript();
			fakeFileSystem.PathToReturnFromGetFullPath = @"d:\projects\myproject\packages\test";
			fakeSession.SetEnvironmentPath(null);
			
			script.Execute();
			
			string actualEnvironmentPath = fakeSession.GetEnvironmentPath();
			string expectedEnvironmentPath = @"d:\projects\myproject\packages\test";
			
			Assert.AreEqual(expectedEnvironmentPath, actualEnvironmentPath);
		}
		
		[Test]
		public void Execute_ScriptDirectoryDoesNotExist_PathIsNotUpdated()
		{
			CreateScript();
			fakeFileSystem.PathToReturnFromGetFullPath = @"d:\projects\myproject\packages\test";
			fakeFileSystem.DirectoryExistsReturnValue = false;
			fakeSession.SetEnvironmentPath(String.Empty);
			
			script.Execute();
			
			string actualEnvironmentPath = fakeSession.GetEnvironmentPath();
			string expectedEnvironmentPath = String.Empty;
			
			Assert.AreEqual(expectedEnvironmentPath, actualEnvironmentPath);
		}
		
		[Test]
		public void Execute_PackageIsSet_PackageSessionVariableIsSet()
		{
			CreateScriptWithPhysicalFileSystem();
			var expectedPackage = new FakePackage("Test");
			script.Package = expectedPackage;
			script.Execute();
			
			var actualPackage = fakeSession.VariablesAdded["__package"];
			
			Assert.AreEqual(expectedPackage, actualPackage);
		}
		
		[Test]
		public void Execute_PackageInstallDirectoryIsSet_RootPathSessionVariableIsSet()
		{
			string expectedRootPath = @"d:\projects\myproject\packages\test";
			CreateScriptWithPhysicalFileSystem(expectedRootPath);
			script.Execute();
			
			var rootPath = fakeSession.VariablesAdded["__rootPath"];
			
			Assert.AreEqual(expectedRootPath, rootPath);
		}
		
		[Test]
		public void Execute_PackageInstallDirectoryIsSet_ToolsPathSessionVariableIsSet()
		{
			CreateScriptWithPhysicalFileSystem(@"d:\projects\myproject\packages\test");
			script.Execute();
			
			var toolsPath = fakeSession.VariablesAdded["__toolsPath"];
			string expectedToolsPath = @"d:\projects\myproject\packages\test\tools";
			
			Assert.AreEqual(expectedToolsPath, toolsPath);
		}
		
		[Test]
		public void Execute_PackageInstallDirectoryIsSet_ProjectSessionVariableIsSet()
		{
			CreateScriptWithPhysicalFileSystem();
			script.Execute();
			
			var project = fakeSession.VariablesAdded["__project"];
			
			Assert.IsNull(project);
		}
		
		[Test]
		public void Execute_PackageInstallDirectoryIsSet_ScriptIsInvoked()
		{
			CreateScriptWithPhysicalFileSystem(@"d:\projects\myproject\packages\test");
			script.Execute();
			
			string actualScript = fakeSession.ScriptPassedToInvokeScript;
			
			string expectedScript = 
				"& 'd:\\projects\\myproject\\packages\\test\\tools\\init.ps1' $__rootPath $__toolsPath $__package $__project";
			
			Assert.AreEqual(expectedScript, actualScript);
		}
		
		[Test]
		public void Execute_PackageInstallDirectoryIsSet_PackageSessionVariableIsRemoved()
		{
			CreateScriptWithPhysicalFileSystem();
			script.Execute();
			
			AssertSessionVariableIsRemoved("__package");
		}
		
		[Test]
		public void Execute_PackageInstallDirectoryIsSet_RootPathSessionVariableIsRemoved()
		{
			CreateScriptWithPhysicalFileSystem();
			script.Execute();
			
			AssertSessionVariableIsRemoved("__rootPath");
		}
		
		[Test]
		public void Execute_PackageInstallDirectoryIsSet_ToolsPathSessionVariableIsRemoved()
		{
			CreateScriptWithPhysicalFileSystem();
			script.Execute();
			
			AssertSessionVariableIsRemoved("__toolsPath");
		}
		
		[Test]
		public void Execute_PackageInstallDirectoryIsSet_ProjectSessionVariableIsRemoved()
		{
			CreateScriptWithPhysicalFileSystem();
			script.Execute();
			
			AssertSessionVariableIsRemoved("__project");
		}
	}
}
