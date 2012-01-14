// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Management.Automation;
using ICSharpCode.PackageManagement;
using ICSharpCode.PackageManagement.Design;
using ICSharpCode.PackageManagement.Scripting;
using NuGet;
using NUnit.Framework;
using PackageManagement.Cmdlets.Tests.Helpers;
using PackageManagement.Tests.Helpers;

namespace PackageManagement.Cmdlets.Tests
{
	[TestFixture]
	public class InstallPackageCmdletTests : CmdletTestsBase
	{
		TestableInstallPackageCmdlet cmdlet;
		FakeCmdletTerminatingError fakeTerminatingError;
		FakePackageManagementProject fakeProject;
		FakeInstallPackageAction fakeInstallPackageAction;
		
		void CreateCmdletWithoutActiveProject()
		{
			cmdlet = new TestableInstallPackageCmdlet();
			fakeTerminatingError = cmdlet.FakeCmdletTerminatingError;
			fakeConsoleHost = cmdlet.FakePackageManagementConsoleHost;
			fakeProject = fakeConsoleHost.FakeProject;
			fakeInstallPackageAction = fakeProject.FakeInstallPackageAction;
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
		
		void EnableIgnoreDependenciesParameter()
		{
			cmdlet.IgnoreDependencies = new SwitchParameter(true);
		}
		
		void EnablePrereleaseParameter()
		{
			cmdlet.IncludePrerelease = new SwitchParameter(true);
		}
		
		void SetSourceParameter(string source)
		{
			cmdlet.Source = source;
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
		public void ProcessRecord_PackageIdSpecified_PackageIdUsedToInstallPackage()
		{
			CreateCmdletWithActivePackageSourceAndProject();
			
			SetIdParameter("Test");
			RunCmdlet();
			
			var actualPackageId = fakeInstallPackageAction.PackageId;
			
			Assert.AreEqual("Test", actualPackageId);
		}
		
		[Test]
		public void ProcessRecord_IgnoreDependenciesParameterSet_IgnoreDependenciesIsTrueWhenInstallingPackage()
		{
			CreateCmdletWithActivePackageSourceAndProject();
			
			SetIdParameter("Test");
			EnableIgnoreDependenciesParameter();
			RunCmdlet();
			
			bool result = fakeInstallPackageAction.IgnoreDependencies;
			
			Assert.IsTrue(result);
		}
		
		[Test]
		public void ProcessRecord_IgnoreDependenciesParameterNotSet_IgnoreDependenciesIsFalseWhenInstallingPackage()
		{
			CreateCmdletWithActivePackageSourceAndProject();
			
			SetIdParameter("Test");
			RunCmdlet();
			
			bool result = fakeInstallPackageAction.IgnoreDependencies;
			
			Assert.IsFalse(result);
		}
		
		[Test]
		public void ProcessRecord_PrereleaseParameterSet_AllowPrereleaseVersionsIsTrueWhenInstallingPackage()
		{
			CreateCmdletWithActivePackageSourceAndProject();
			
			SetIdParameter("Test");
			EnablePrereleaseParameter();
			RunCmdlet();
			
			bool result = fakeInstallPackageAction.AllowPrereleaseVersions;
			
			Assert.IsTrue(result);
		}
		
		[Test]
		public void ProcessRecord_PrereleaseParameterNotSet_AllowPrereleaseVersionsIsFalseWhenInstallingPackage()
		{
			CreateCmdletWithActivePackageSourceAndProject();
			
			SetIdParameter("Test");
			RunCmdlet();
			
			bool result = fakeInstallPackageAction.AllowPrereleaseVersions;
			
			Assert.IsFalse(result);
		}
		
		[Test]
		public void ProcessRecord_SourceParameterSet_CustomSourceUsedWhenRetrievingProject()
		{
			CreateCmdletWithActivePackageSourceAndProject();
			
			SetIdParameter("Test");
			SetSourceParameter("http://sharpdevelop.net/packages");
			RunCmdlet();
			
			var expected = "http://sharpdevelop.net/packages";
			var actual = fakeConsoleHost.PackageSourcePassedToGetProject;
			
			Assert.AreEqual(expected, actual);
		}
		
		[Test]
		public void ProcessRecord_PackageVersionParameterSet_VersionUsedWhenInstallingPackage()
		{
			CreateCmdletWithActivePackageSourceAndProject();
			
			SetIdParameter("Test");
			var version = new SemanticVersion("1.0.1");
			SetVersionParameter(version);
			RunCmdlet();
			
			SemanticVersion actualVersion = fakeInstallPackageAction.PackageVersion;
			
			Assert.AreEqual(version, actualVersion);
		}
		
		[Test]
		public void ProcessRecord_PackageVersionParameterNotSet_VersionUsedWhenInstallingPackageIsNull()
		{
			CreateCmdletWithActivePackageSourceAndProject();
			
			SetIdParameter("Test");
			RunCmdlet();
			
			var actualVersion = fakeInstallPackageAction.PackageVersion;
			
			Assert.IsNull(actualVersion);
		}
		
		[Test]
		public void ProcessRecord_ProjectNameSpecified_ProjectMatchingProjectNameUsedWhenInstallingPackage()
		{
			CreateCmdletWithActivePackageSourceAndProject();
			
			SetIdParameter("Test");
			SetProjectNameParameter("MyProject");
			RunCmdlet();
			
			var actualProjectName = fakeConsoleHost.ProjectNamePassedToGetProject;
			
			Assert.AreEqual("MyProject", actualProjectName);
		}
		
		[Test]
		public void ProcessRecord_ProjectNameSpecified_ProjectNameUsedToFindProject()
		{
			CreateCmdletWithActivePackageSourceAndProject();
			
			SetIdParameter("Test");
			SetProjectNameParameter("MyProject");
			RunCmdlet();
			
			var actual = fakeConsoleHost.ProjectNamePassedToGetProject;
			var expected = "MyProject";
			
			Assert.AreEqual(expected, actual);
		}
		
		[Test]
		public void ProcessRecord_PackageIdSpecified_PackageIsInstalled()
		{
			CreateCmdletWithoutActiveProject();
			AddDefaultProjectToConsoleHost();
			var packageSource = AddPackageSourceToConsoleHost();
			SetIdParameter("Test");
			RunCmdlet();
			
			bool result = fakeInstallPackageAction.IsExecuteCalled;
			
			Assert.IsTrue(result);
		}
		
		[Test]
		public void ProcessRecord_PackageIdSpecified_CmdletUsedAsScriptRunner()
		{
			CreateCmdletWithoutActiveProject();
			AddDefaultProjectToConsoleHost();
			var packageSource = AddPackageSourceToConsoleHost();
			SetIdParameter("Test");
			RunCmdlet();
			
			IPackageScriptRunner scriptRunner = fakeInstallPackageAction.PackageScriptRunner;
			
			Assert.AreEqual(cmdlet, scriptRunner);
		}
	}
}
