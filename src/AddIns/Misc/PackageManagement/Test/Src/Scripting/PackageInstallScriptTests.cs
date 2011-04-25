// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.PackageManagement.Design;
using ICSharpCode.PackageManagement.EnvDTE;
using ICSharpCode.PackageManagement.Scripting;
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
		
		void CreateScriptWithPhysicalFileSystem()
		{
			CreateScriptWithPhysicalFileSystem(@"d:\projects\myproject\packages\test");
		}
		
		void CreateScriptWithPhysicalFileSystem(string packageInstallDirectory)
		{
			scriptFileName = new PackageInstallScriptFileName(packageInstallDirectory);
			fakeSession = new FakePackageScriptSession();
			script = new PackageInstallScript(scriptFileName, fakeSession);
		}

		[Test]
		public void Execute_PackageInstallDirectoryIsSet_ProjectSessionVariableIsSet()
		{
			CreateScriptWithPhysicalFileSystem();
			var expectedProject = new Project();
			var project = new FakePackageManagementProject();
			project.DTEProject = expectedProject;
			script.Project = project;
			script.Execute();
			
			var projectVariable = fakeSession.VariablesAdded["__project"];
			
			Assert.AreEqual(expectedProject, projectVariable);
		}
	}
}
