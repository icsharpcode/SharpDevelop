// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Management.Automation;
using ICSharpCode.PackageManagement.Design;
using NUnit.Framework;
using PackageManagement.Cmdlets.Tests.Helpers;
using PackageManagement.Tests.Helpers;

namespace PackageManagement.Cmdlets.Tests
{
	[TestFixture]
	public class UpdatePackageCmdletTests : CmdletTestsBase
	{
		TestableUpdatePackageCmdlet cmdlet;
		FakeCmdletTerminatingError fakeTerminatingError;
		FakePackageManagementService fakePackageManagementService;
		
		void CreateCmdletWithoutActiveProject()
		{
			cmdlet = new TestableUpdatePackageCmdlet();
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
		public void ProcessRecord_PackageIdSpecified_PackageIdUsedToUpdatePackage()
		{
			CreateCmdletWithActivePackageSourceAndProject();
			
			SetIdParameter("Test");
			RunCmdlet();
			
			var actualPackageId = fakePackageManagementService.PackageIdPassedToUpdatePackage;
			 
			Assert.AreEqual("Test", actualPackageId);
		}
		
		[Test]
		public void ProcessRecord_PackageIdSpecified_ActivePackageSourceUsedToUpdatePackage()
		{
			CreateCmdletWithoutActiveProject();
			AddDefaultProjectToConsoleHost();
			var packageSource = AddPackageSourceToConsoleHost();
			SetIdParameter("Test");
			RunCmdlet();
			
			var actualPackageSource = fakePackageManagementService.PackageSourcePassedToUpdatePackage;
			
			Assert.AreEqual(packageSource, actualPackageSource);
		}
		
		[Test]
		public void ProcessRecord_PackageIdSpecified_ActiveProjectUsedToUpdatePackage()
		{
			CreateCmdletWithoutActiveProject();
			AddPackageSourceToConsoleHost();
			var project = AddDefaultProjectToConsoleHost();
			SetIdParameter("Test");
			RunCmdlet();
			
			var actualProject = fakePackageManagementService.ProjectPassedToUpdatePackage;
			
			Assert.AreEqual(project, actualProject);
		}
		
		[Test]
		public void ProcessRecord_IgnoreDependenciesParameterSet_UpdateDependenciesIsFalseWhenUpdatingPackage()
		{
			CreateCmdletWithActivePackageSourceAndProject();
			
			SetIdParameter("Test");
			EnableIgnoreDependenciesParameter();
			RunCmdlet();
			
			bool result = fakePackageManagementService.UpdateDependenciesPassedToUpdatePackage;
			
			Assert.IsFalse(result);
		}
		
		[Test]
		public void ProcessRecord_IgnoreDependenciesParameterNotSet_UpdateDependenciesIsTrueWhenUpdatingPackage()
		{
			CreateCmdletWithActivePackageSourceAndProject();
			
			SetIdParameter("Test");
			RunCmdlet();
			
			bool result = fakePackageManagementService.UpdateDependenciesPassedToUpdatePackage;
			
			Assert.IsTrue(result);
		}
		
		[Test]
		public void ProcessRecord_SourceParameterSet_CustomSourceUsedWhenUpdatingPackage()
		{
			CreateCmdletWithActivePackageSourceAndProject();
			
			SetIdParameter("Test");
			SetSourceParameter("http://sharpdevelop.net/packages");
			RunCmdlet();
			
			var expected = "http://sharpdevelop.net/packages";
			var actual = fakePackageManagementService.PackageSourcePassedToUpdatePackage.Source;
			
			Assert.AreEqual(expected, actual);
		}
		
		[Test]
		public void ProcessRecord_PackageVersionParameterSet_VersionUsedWhenUpdatingPackage()
		{
			CreateCmdletWithActivePackageSourceAndProject();
			
			SetIdParameter("Test");
			var version = new Version("1.0.1");
			SetVersionParameter(version);
			RunCmdlet();
			
			var actualVersion = fakePackageManagementService.VersionPassedToUpdatePackage;
			
			Assert.AreEqual(version, actualVersion);
		}
		
		[Test]
		public void ProcessRecord_PackageVersionParameterNotSet_VersionUsedWhenUpdatingPackageIsNull()
		{
			CreateCmdletWithActivePackageSourceAndProject();
			
			SetIdParameter("Test");
			RunCmdlet();
			
			var actualVersion = fakePackageManagementService.VersionPassedToUpdatePackage;
			
			Assert.IsNull(actualVersion);
		}
		
		[Test]
		public void ProcessRecord_ProjectNameSpecified_ProjectMatchingProjectNameUsedWhenUpdatingPackage()
		{
			CreateCmdletWithActivePackageSourceAndProject();
			fakePackageManagementService.FakeProjectToReturnFromGetProject = ProjectHelper.CreateTestProject();
			
			SetIdParameter("Test");
			SetProjectNameParameter("MyProject");
			RunCmdlet();
			
			var actualProject = fakePackageManagementService.ProjectPassedToUpdatePackage;
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
