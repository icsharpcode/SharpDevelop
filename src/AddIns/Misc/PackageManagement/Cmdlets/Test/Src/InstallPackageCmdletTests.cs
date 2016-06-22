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
		
		void CreateCmdletWithoutActiveProject()
		{
			cmdlet = new TestableInstallPackageCmdlet();
			fakeTerminatingError = cmdlet.FakeCmdletTerminatingError;
			fakeConsoleHost = cmdlet.FakePackageManagementConsoleHost;
			fakeProject = fakeConsoleHost.FakeProject;
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
			
			string actualPackageId = fakeProject.LastInstallPackageCreated.PackageId;
			Assert.AreEqual("Test", actualPackageId);
		}
		
		[Test]
		public void ProcessRecord_IgnoreDependenciesParameterSet_IgnoreDependenciesIsTrueWhenInstallingPackage()
		{
			CreateCmdletWithActivePackageSourceAndProject();
			
			SetIdParameter("Test");
			EnableIgnoreDependenciesParameter();
			RunCmdlet();
			
			bool result = fakeProject.LastInstallPackageCreated.IgnoreDependencies;
			Assert.IsTrue(result);
		}
		
		[Test]
		public void ProcessRecord_IgnoreDependenciesParameterNotSet_IgnoreDependenciesIsFalseWhenInstallingPackage()
		{
			CreateCmdletWithActivePackageSourceAndProject();
			
			SetIdParameter("Test");
			RunCmdlet();
			
			bool result = fakeProject.LastInstallPackageCreated.IgnoreDependencies;
			Assert.IsFalse(result);
		}
		
		[Test]
		public void ProcessRecord_PrereleaseParameterSet_AllowPrereleaseVersionsIsTrueWhenInstallingPackage()
		{
			CreateCmdletWithActivePackageSourceAndProject();
			
			SetIdParameter("Test");
			EnablePrereleaseParameter();
			RunCmdlet();
			
			bool result = fakeProject.LastInstallPackageCreated.AllowPrereleaseVersions;
			Assert.IsTrue(result);
		}
		
		[Test]
		public void ProcessRecord_PrereleaseParameterNotSet_AllowPrereleaseVersionsIsFalseWhenInstallingPackage()
		{
			CreateCmdletWithActivePackageSourceAndProject();
			
			SetIdParameter("Test");
			RunCmdlet();
			
			bool result = fakeProject.LastInstallPackageCreated.AllowPrereleaseVersions;
			Assert.IsFalse(result);
		}
		
		[Test]
		public void ProcessRecord_SourceParameterSet_CustomSourceUsedWhenRetrievingProject()
		{
			CreateCmdletWithActivePackageSourceAndProject();
			
			SetIdParameter("Test");
			SetSourceParameter("http://sharpdevelop.net/packages");
			RunCmdlet();
			
			string expected = "http://sharpdevelop.net/packages";
			string actual = fakeConsoleHost.PackageSourcePassedToGetProject;
			
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
			
			SemanticVersion actualVersion = fakeProject.LastInstallPackageCreated.PackageVersion;
			Assert.AreEqual(version, actualVersion);
		}
		
		[Test]
		public void ProcessRecord_PackageVersionParameterNotSet_VersionUsedWhenInstallingPackageIsNull()
		{
			CreateCmdletWithActivePackageSourceAndProject();
			
			SetIdParameter("Test");
			RunCmdlet();
			
			SemanticVersion actualVersion = fakeProject.LastInstallPackageCreated.PackageVersion;
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
			
			bool result = fakeProject.LastInstallPackageCreated.IsExecuteCalled;
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
			
			IPackageScriptRunner scriptRunner = fakeProject.LastInstallPackageCreated.PackageScriptRunner;
			Assert.AreEqual(cmdlet, scriptRunner);
		}
		
		[Test]
		public void ProcessRecord_FileConflictActionIsOverwrite_FileConflictResolverCreatedWithOverwriteAction()
		{
			CreateCmdletWithoutActiveProject();
			AddDefaultProjectToConsoleHost();
			AddPackageSourceToConsoleHost();
			SetIdParameter("Test");
			cmdlet.FileConflictAction = FileConflictAction.Overwrite;
			
			RunCmdlet();
			
			Assert.AreEqual(FileConflictAction.Overwrite, fakeConsoleHost.LastFileConflictActionUsedWhenCreatingResolver);
		}
		
		[Test]
		public void ProcessRecord_FileConflictActionIsIgnore_FileConflictResolverCreatedWithIgnoreAction()
		{
			CreateCmdletWithoutActiveProject();
			AddDefaultProjectToConsoleHost();
			AddPackageSourceToConsoleHost();
			SetIdParameter("Test");
			cmdlet.FileConflictAction = FileConflictAction.Ignore;
			
			RunCmdlet();
			
			Assert.AreEqual(FileConflictAction.Ignore, fakeConsoleHost.LastFileConflictActionUsedWhenCreatingResolver);
		}
		
		[Test]
		public void ProcessRecord_FileConflictActionNotSet_FileConflictResolverCreatedWithNoneFileConflictAction()
		{
			CreateCmdletWithoutActiveProject();
			AddDefaultProjectToConsoleHost();
			AddPackageSourceToConsoleHost();
			SetIdParameter("Test");
			
			RunCmdlet();
			
			Assert.AreEqual(FileConflictAction.None, fakeConsoleHost.LastFileConflictActionUsedWhenCreatingResolver);
		}
		
		[Test]
		public void ProcessRecord_PackageIdSpecified_FileConflictResolverIsDisposed()
		{
			CreateCmdletWithoutActiveProject();
			AddDefaultProjectToConsoleHost();
			AddPackageSourceToConsoleHost();
			SetIdParameter("Test");
			
			RunCmdlet();
			
			fakeConsoleHost.AssertFileConflictResolverIsDisposed();
		}
		
		[Test]
		public void ProcessRecord_SourceRepositoryIsOperationAware_InstallOperationStartedForPackageAndDisposed()
		{
			CreateCmdletWithoutActiveProject();
			AddDefaultProjectToConsoleHost();
			var operationAwareRepository = new FakeOperationAwarePackageRepository();
			fakeConsoleHost.FakeProject.FakeSourceRepository = operationAwareRepository;
			AddPackageSourceToConsoleHost();
			SetIdParameter("Test");
			
			RunCmdlet();
			
			operationAwareRepository.AssertOperationWasStartedAndDisposed(RepositoryOperationNames.Install, "Test");
		}
		
		[Test]
		public void ProcessRecord_PackageIdSpecified_ConsoleHostLoggerIsDisposed()
		{
			CreateCmdletWithoutActiveProject();
			AddDefaultProjectToConsoleHost();
			AddPackageSourceToConsoleHost();
			SetIdParameter("Test");
			
			RunCmdlet();
			
			fakeConsoleHost.AssertLoggerIsDisposed();
			Assert.AreEqual(cmdlet, fakeConsoleHost.CmdletLoggerUsedToCreateLogger);
		}
		
		[Test]
		public void ProcessRecord_DependencyVersionSetToHighest_DependencyVersionUsedWhenInstallingPackage()
		{
			CreateCmdletWithActivePackageSourceAndProject();
			cmdlet.DependencyVersion = DependencyVersion.Highest;
			
			RunCmdlet();
			
			DependencyVersion actualDependencyVersion = fakeProject.LastInstallPackageCreated.DependencyVersion;
			Assert.AreEqual(DependencyVersion.Highest, actualDependencyVersion);
		}
		
		[Test]
		public void ProcessRecord_DependencyVersionNotSett_DependencyVersionSetToLowestWhenInstallingPackage()
		{
			CreateCmdletWithActivePackageSourceAndProject();
			
			RunCmdlet();
			
			DependencyVersion actualDependencyVersion = fakeProject.LastInstallPackageCreated.DependencyVersion;
			Assert.AreEqual(DependencyVersion.Lowest, actualDependencyVersion);
		}
	}
}
