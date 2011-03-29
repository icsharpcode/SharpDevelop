// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
	public class InstallPackageCmdletTests : PackageManagementCmdletTests
	{
		TestableInstallPackageCmdlet cmdlet;
		FakeCmdletTerminatingError fakeTerminatingError;
		FakePackageManagementService fakePackageManagementService;
		
		void CreateCmdletWithoutActiveProject()
		{
			cmdlet = new TestableInstallPackageCmdlet();
			fakeTerminatingError = cmdlet.FakeCmdletTerminatingError;
			fakeConsoleHost = cmdlet.FakePackageManagementConsoleHost;
			fakePackageManagementService = cmdlet.FakePackageManagementService;
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
		
		void SetSourceParameter(string source)
		{
			cmdlet.Source = source;
		}
		
		void SetVersionParameter(Version version)
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
			RunCmdlet();
						
			Assert.IsTrue(fakeTerminatingError.IsThrowNoProjectOpenErrorCalled);
		}
		
		[Test]
		public void ProcessRecord_ProjectIsActiveInConsoleHost_NoTerminatingErrorThrown()
		{
			CreateCmdletWithActivePackageSourceAndProject();
			
			SetIdParameter("Test");
			RunCmdlet();
			
			Assert.IsFalse(fakeTerminatingError.IsThrowNoProjectOpenErrorCalled);
		}
		
		[Test]
		public void ProcessRecord_PackageIdSpecified_PackageIdUsedToInstallPackage()
		{
			CreateCmdletWithActivePackageSourceAndProject();
			
			SetIdParameter("Test");
			RunCmdlet();
			
			var actualPackageId = fakePackageManagementService.PackageIdPassedToInstallPackage;
			
			Assert.AreEqual("Test", actualPackageId);
		}
		
		[Test]
		public void ProcessRecord_PackageIdSpecified_ActivePackageSourceUsedToInstallPackage()
		{
			CreateCmdletWithoutActiveProject();
			AddDefaultProjectToConsoleHost();
			var packageSource = AddPackageSourceToConsoleHost();
			SetIdParameter("Test");
			RunCmdlet();
			
			var actualPackageSource = fakePackageManagementService.PackageSourcePassedToInstallPackage;
			
			Assert.AreEqual(packageSource, actualPackageSource);
		}
		
		[Test]
		public void ProcessRecord_PackageIdSpecified_ActiveProjectUsedToInstallPackage()
		{
			CreateCmdletWithoutActiveProject();
			AddPackageSourceToConsoleHost();
			var project = AddDefaultProjectToConsoleHost();
			SetIdParameter("Test");
			RunCmdlet();
			
			var actualProject = fakePackageManagementService.ProjectPassedToInstallPackage;
			
			Assert.AreEqual(project, actualProject);
		}
		
		[Test]
		public void ProcessRecord_IgnoreDependenciesParameterSet_IgnoreDependenciesIsTrueWhenInstallingPackage()
		{
			CreateCmdletWithActivePackageSourceAndProject();
			
			SetIdParameter("Test");
			EnableIgnoreDependenciesParameter();
			RunCmdlet();
			
			bool result = fakePackageManagementService.IgnoreDependenciesPassedToInstallPackage;
			
			Assert.IsTrue(result);
		}
		
		[Test]
		public void ProcessRecord_IgnoreDependenciesParameterNotSet_IgnoreDependenciesIsFalseWhenInstallingPackage()
		{
			CreateCmdletWithActivePackageSourceAndProject();
			
			SetIdParameter("Test");
			RunCmdlet();
			
			bool result = fakePackageManagementService.IgnoreDependenciesPassedToInstallPackage;
			
			Assert.IsFalse(result);
		}
		
		[Test]
		public void ProcessRecord_SourceParameterSet_CustomSourceUsedWhenInstallingPackage()
		{
			CreateCmdletWithActivePackageSourceAndProject();
			
			SetIdParameter("Test");
			SetSourceParameter("http://sharpdevelop.net/packages");
			RunCmdlet();
			
			var expected = "http://sharpdevelop.net/packages";
			var actual = fakePackageManagementService.PackageSourcePassedToInstallPackage.Source;
			
			Assert.AreEqual(expected, actual);
		}
		
		[Test]
		public void ProcessRecord_PackageVersionParameterSet_VersionUsedWhenInstallingPackage()
		{
			CreateCmdletWithActivePackageSourceAndProject();
			
			SetIdParameter("Test");
			var version = new Version("1.0.1");
			SetVersionParameter(version);
			RunCmdlet();
			
			var actualVersion = fakePackageManagementService.VersionPassedToInstallPackage;
			
			Assert.AreEqual(version, actualVersion);
		}
		
		[Test]
		public void ProcessRecord_PackageVersionParameterNotSet_VersionUsedWhenInstallingPackageIsNull()
		{
			CreateCmdletWithActivePackageSourceAndProject();
			
			SetIdParameter("Test");
			RunCmdlet();
			
			var actualVersion = fakePackageManagementService.VersionPassedToInstallPackage;
			
			Assert.IsNull(actualVersion);
		}
		
		[Test]
		public void ProcessRecord_ProjectNameSpecified_ProjectMatchingProjectNameUsedWhenInstallingPackage()
		{
			CreateCmdletWithActivePackageSourceAndProject();
			fakePackageManagementService.FakeProjectToReturnFromGetProject = ProjectHelper.CreateTestProject();
			
			SetIdParameter("Test");
			SetProjectNameParameter("MyProject");
			RunCmdlet();
			
			var actualProject = fakePackageManagementService.ProjectPassedToInstallPackage;
			var expectedProject = fakePackageManagementService.FakeProjectToReturnFromGetProject;
			
			Assert.AreEqual(expectedProject, actualProject);
		}
		
		[Test]
		public void ProcessRecord_ProjectNameSpecified_ProjectNameUsedToFindProject()
		{
			CreateCmdletWithActivePackageSourceAndProject();
			
			SetIdParameter("Test");
			SetProjectNameParameter("MyProject");
			RunCmdlet();
			
			var actual = fakePackageManagementService.NamePassedToGetProject;
			var expected = "MyProject";
			
			Assert.AreEqual(expected, actual);
		}
	}
}
