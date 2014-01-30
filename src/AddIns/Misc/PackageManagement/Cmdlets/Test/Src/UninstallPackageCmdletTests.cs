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
using System.Management.Automation;
using ICSharpCode.PackageManagement.Design;
using ICSharpCode.PackageManagement.Scripting;
using NuGet;
using NUnit.Framework;
using PackageManagement.Cmdlets.Tests.Helpers;
using PackageManagement.Tests.Helpers;

namespace PackageManagement.Cmdlets.Tests
{
	[TestFixture]
	public class UninstallPackageCmdletTests : CmdletTestsBase
	{
		TestableUninstallPackageCmdlet cmdlet;
		FakeCmdletTerminatingError fakeTerminatingError;
		FakePackageManagementProject fakeProject;
		FakeUninstallPackageAction uninstallPackageAction;
		
		void CreateCmdletWithoutActiveProject()
		{
			cmdlet = new TestableUninstallPackageCmdlet();
			fakeTerminatingError = cmdlet.FakeCmdletTerminatingError;
			fakeConsoleHost = cmdlet.FakePackageManagementConsoleHost;
			fakeProject = fakeConsoleHost.FakeProject;
			uninstallPackageAction = fakeProject.FakeUninstallPackageAction;
		}
		
		void CreateCmdletWithActivePackageSourceAndProject()
		{
			CreateCmdletWithoutActiveProject();
			AddPackageSourceToConsoleHost();
			AddDefaultProjectToConsoleHost();
		}
		
		void RunCmdlet()
		{
			cmdlet.CallProcessRecord();
		}
		
		void SetIdParameter(string id)
		{
			cmdlet.Id = id;
		}
		
		void EnableForceParameter()
		{
			cmdlet.Force = new SwitchParameter(true);
		}
		
		void EnableRemoveDependenciesParameter()
		{
			cmdlet.RemoveDependencies = new SwitchParameter(true);
		}
		
		void SetVersionParameter(SemanticVersion version)
		{
			cmdlet.Version = version;
		}
		
		void SetProjectNameParameter(string name)
		{
			cmdlet.ProjectName = name;
		}

		[Test]
		public void ProcessRecord_NoActiveProject_ThrowsNoProjectOpenTerminatingError()
		{
			CreateCmdletWithoutActiveProject();
			AddPackageSourceToConsoleHost();
			SetIdParameter("Test");
			
			Assert.Throws(typeof(FakeCmdletTerminatingErrorException), () => RunCmdlet());
		}
		
		[Test]
		public void ProcessRecord_PackageIdSpecified_PackageIdUsedToUninstallPackage()
		{
			CreateCmdletWithActivePackageSourceAndProject();
			
			SetIdParameter("Test");
			RunCmdlet();
			
			var actualPackageId = uninstallPackageAction.PackageId;
			
			Assert.AreEqual("Test", actualPackageId);
		}
		
		[Test]
		public void ProcessRecord_ForceParameterSet_PackageForcefullyUninstalled()
		{
			CreateCmdletWithActivePackageSourceAndProject();
			
			SetIdParameter("Test");
			EnableForceParameter();
			RunCmdlet();
			
			bool result = uninstallPackageAction.ForceRemove;
			
			Assert.IsTrue(result);
		}
		
		[Test]
		public void ProcessRecord_ForceParameterNotSet_PackageIsNotForcefullyUninstalled()
		{
			CreateCmdletWithActivePackageSourceAndProject();
			
			SetIdParameter("Test");
			RunCmdlet();
			
			bool result = uninstallPackageAction.ForceRemove;
			
			Assert.IsFalse(result);
		}
		
		[Test]
		public void ProcessRecord_RemoveDependenciesParameterSet_PackageDependenciesUninstalled()
		{
			CreateCmdletWithActivePackageSourceAndProject();
			
			SetIdParameter("Test");
			EnableRemoveDependenciesParameter();
			RunCmdlet();
			
			bool result = uninstallPackageAction.RemoveDependencies;
			
			Assert.IsTrue(result);
		}
		
		[Test]
		public void ProcessRecord_RemoveDependenciesParameterNotSet_PackageDependenciesNotUninstalled()
		{
			CreateCmdletWithActivePackageSourceAndProject();
			
			SetIdParameter("Test");
			RunCmdlet();
			
			bool result = uninstallPackageAction.RemoveDependencies;
			
			Assert.IsFalse(result);
		}
		
		[Test]
		public void ProcessRecord_PackageVersionParameterSet_VersionUsedWhenUninstallingPackage()
		{
			CreateCmdletWithActivePackageSourceAndProject();
			
			SetIdParameter("Test");
			var version = new SemanticVersion("1.0.1");
			SetVersionParameter(version);
			RunCmdlet();
			
			SemanticVersion actualVersion = uninstallPackageAction.PackageVersion;
			
			Assert.AreEqual(version, actualVersion);
		}
		
		[Test]
		public void ProcessRecord_PackageVersionParameterNotSet_VersionUsedWhenUninstallingPackageIsNull()
		{
			CreateCmdletWithActivePackageSourceAndProject();
			
			SetIdParameter("Test");
			RunCmdlet();
			
			SemanticVersion actualVersion = uninstallPackageAction.PackageVersion;
			
			Assert.IsNull(actualVersion);
		}
		
		[Test]
		public void ProcessRecord_ProjectNameSpecified_ProjectMatchingProjectNameUsedWhenUninstallingPackage()
		{
			CreateCmdletWithActivePackageSourceAndProject();
			
			SetIdParameter("Test");
			SetProjectNameParameter("MyProject");
			RunCmdlet();
			
			string actualProjectName = fakeConsoleHost.ProjectNamePassedToGetProject;
			
			Assert.AreEqual("MyProject", actualProjectName);
		}
		
		[Test]
		public void ProcessRecord_ProjectNameSpecified_ProjectNameUsedToFindProject()
		{
			CreateCmdletWithActivePackageSourceAndProject();
			
			SetIdParameter("Test");
			SetProjectNameParameter("MyProject");
			RunCmdlet();
			
			string actual = fakeConsoleHost.ProjectNamePassedToGetProject;
			string expected = "MyProject";
			
			Assert.AreEqual(expected, actual);
		}
		
		[Test]
		public void ProcessRecord_PackageIdSpecified_PackageIsUninstalled()
		{
			CreateCmdletWithoutActiveProject();
			AddDefaultProjectToConsoleHost();
			PackageSource packageSource = AddPackageSourceToConsoleHost();
			SetIdParameter("Test");
			RunCmdlet();
						
			Assert.IsTrue(uninstallPackageAction.IsExecuted);
		}
		
		[Test]
		public void ProcessRecord_PackageIdSpecified_CmdletUsedAsScriptRunner()
		{
			CreateCmdletWithoutActiveProject();
			AddDefaultProjectToConsoleHost();
			PackageSource packageSource = AddPackageSourceToConsoleHost();
			SetIdParameter("Test");
			RunCmdlet();
			
			IPackageScriptRunner scriptRunner = uninstallPackageAction.PackageScriptRunner;
						
			Assert.AreEqual(cmdlet, scriptRunner);
		}
	}
}
