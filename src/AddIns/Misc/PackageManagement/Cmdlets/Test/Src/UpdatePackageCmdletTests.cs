// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Management.Automation;
using ICSharpCode.PackageManagement;
using ICSharpCode.PackageManagement.Design;
using ICSharpCode.PackageManagement.Scripting;
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
		FakePackageManagementProject fakeProject;
		FakeUpdatePackageActionsFactory fakeUpdateActionsFactory;
		FakeUpdatePackageActions fakeUpdateAllPackagesInProject;
		
		void CreateCmdletWithoutActiveProject()
		{
			cmdlet = new TestableUpdatePackageCmdlet();
			fakeTerminatingError = cmdlet.FakeCmdletTerminatingError;
			fakeConsoleHost = cmdlet.FakePackageManagementConsoleHost;
			fakeProject = fakeConsoleHost.FakeProject;
			fakeUpdateActionsFactory = cmdlet.FakeUpdatePackageActionsFactory;
			fakeUpdateAllPackagesInProject = fakeUpdateActionsFactory.FakeUpdateAllPackagesInProject;
		}
				
		FakeUpdatePackageAction UpdatePackageInSingleProjectAction {
			get { return fakeProject.FirstFakeUpdatePackageActionCreated; }
		}
		
		FakeUpdatePackageAction FirstUpdateActionWhenUpdatingAllPackagesInProject {
			get { return GetUpdatingAllPackagesInProjectAction(0); }
		}
		
		FakeUpdatePackageAction GetUpdatingAllPackagesInProjectAction(int index)
		{
			return fakeUpdateActionsFactory.FakeUpdateAllPackagesInProject.FakeActions[index];
		}
		
		FakeUpdatePackageAction SecondUpdateActionWhenUpdatingAllPackagesInProject {
			get { return GetUpdatingAllPackagesInProjectAction(1); }
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
		
		void CreateTwoUpdateActionsWhenUpdatingAllPackagesInProject(string packageId1, string packageId2)
		{
			CreateUpdateActionWhenUpdatingAllPackagesInProject(packageId1);
			CreateUpdateActionWhenUpdatingAllPackagesInProject(packageId2);
		}
		
		void CreateUpdateActionWhenUpdatingAllPackagesInProject(string packageId)
		{
			var action = new FakeUpdatePackageAction(fakeProject);
			action.PackageId = packageId;
			fakeUpdateActionsFactory.FakeUpdateAllPackagesInProject.FakeActions.Add(action);
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
		public void ProcessRecord_PackageIdAndProjectNameSpecified_PackageIdUsedToUpdatePackage()
		{
			CreateCmdletWithActivePackageSourceAndProject();
			
			SetIdParameter("Test");
			SetProjectNameParameter("MyProject");
			RunCmdlet();
			
			string actualPackageId = UpdatePackageInSingleProjectAction.PackageId;
			 
			Assert.AreEqual("Test", actualPackageId);
		}
		
		[Test]
		public void ProcessRecord_PackageIdAndProjectNameSpecified_NullPackageSourceUsedToCreateProject()
		{
			CreateCmdletWithActivePackageSourceAndProject();
			SetProjectNameParameter("MyProject");
			SetIdParameter("Test");
			RunCmdlet();
			
			string actualPackageSource = fakeConsoleHost.PackageSourcePassedToGetProject;
			
			Assert.IsNull(actualPackageSource);
		}
		
		[Test]
		public void ProcessRecord_ProjectNameAndPackageIdSpecified_ProjectMatchingProjectNameUsedWhenUpdatingPackage()
		{
			CreateCmdletWithActivePackageSourceAndProject();
			
			SetIdParameter("Test");
			SetProjectNameParameter("MyProject");
			RunCmdlet();
			
			string actualProjectName = fakeConsoleHost.ProjectNamePassedToGetProject;
			
			Assert.AreEqual("MyProject", actualProjectName);
		}
		
		[Test]
		public void ProcessRecord_PackageIdSpecified_UpdatePackageActionIsExecuted()
		{
			CreateCmdletWithActivePackageSourceAndProject();
			SetIdParameter("Test");
			SetProjectNameParameter("MyProject");
			RunCmdlet();
			
			bool executed = UpdatePackageInSingleProjectAction.IsExecuted;
			
			Assert.IsTrue(executed);
		}
		
		[Test]
		public void ProcessRecord_PackageIdAndProjectNameAndIgnoreDependenciesParameterSet_UpdateDependenciesIsFalseWhenUpdatingPackage()
		{
			CreateCmdletWithActivePackageSourceAndProject();
			
			SetIdParameter("Test");
			SetProjectNameParameter("MyProject");
			EnableIgnoreDependenciesParameter();
			RunCmdlet();
			
			bool result = UpdatePackageInSingleProjectAction.UpdateDependencies;
			
			Assert.IsFalse(result);
		}
		
		[Test]
		public void ProcessRecord_PackageIdAndProjectNameAndIgnoreDependenciesParameterNotSet_UpdateDependenciesIsTrueWhenUpdatingPackage()
		{
			CreateCmdletWithActivePackageSourceAndProject();
			
			SetIdParameter("Test");
			SetProjectNameParameter("MyProject");
			RunCmdlet();
			
			bool result = UpdatePackageInSingleProjectAction.UpdateDependencies;
			
			Assert.IsTrue(result);
		}
		
		[Test]
		public void ProcessRecord_PackageIdAndProjectNameAndSourceParameterSet_CustomSourceUsedWhenUpdatingPackage()
		{
			CreateCmdletWithActivePackageSourceAndProject();
			
			SetIdParameter("Test");
			SetProjectNameParameter("MyProject");
			SetSourceParameter("http://sharpdevelop.net/packages");
			RunCmdlet();
			
			string expected = "http://sharpdevelop.net/packages";
			string actual = fakeConsoleHost.PackageSourcePassedToGetProject;
			
			Assert.AreEqual(expected, actual);
		}
		
		[Test]
		public void ProcessRecord_PackageIdAndProjectNameAndPackageVersionParameterSet_VersionUsedWhenUpdatingPackage()
		{
			CreateCmdletWithActivePackageSourceAndProject();
			
			SetIdParameter("Test");
			SetProjectNameParameter("MyProject");
			var version = new Version("1.0.1");
			SetVersionParameter(version);
			RunCmdlet();
			
			Version actualVersion = UpdatePackageInSingleProjectAction.PackageVersion;
			
			Assert.AreEqual(version, actualVersion);
		}
		
		[Test]
		public void ProcessRecord_PackageIdAndProjectNameSpecified_CmdletUsedAsScriptRunner()
		{
			CreateCmdletWithoutActiveProject();
			AddDefaultProjectToConsoleHost();
			AddPackageSourceToConsoleHost();
			SetIdParameter("Test");
			SetProjectNameParameter("MyProject");
			RunCmdlet();
			
			IPackageScriptRunner scriptRunner = UpdatePackageInSingleProjectAction.PackageScriptRunner;
			
			Assert.AreEqual(cmdlet, scriptRunner);
		}
		
		[Test]
		public void ProcessRecord_UpdateAllPackagesInProject_CreatesUpdateAllPackagesProject()
		{
			CreateCmdletWithActivePackageSourceAndProject();
			SetProjectNameParameter("MyProject");
			RunCmdlet();
			
			IPackageManagementProject project = 
				fakeUpdateActionsFactory.ProjectPassedToCreateUpdateAllPackagesInProject;
			
			Assert.AreEqual(fakeProject, project);
		}
				
		[Test]
		public void ProcessRecord_UpdateAllPackagesInProject_ProjectNameUsedToCreateProject()
		{
			CreateCmdletWithActivePackageSourceAndProject();
			SetProjectNameParameter("MyProject");
			RunCmdlet();
			
			string projectName = fakeConsoleHost.ProjectNamePassedToGetProject;
			
			Assert.AreEqual("MyProject", projectName);
		}
		
		[Test]
		public void ProcessRecord_UpdateAllPackagesInProjectWhenSourceSpecified_PackageSourceUsedToCreateProject()
		{
			CreateCmdletWithActivePackageSourceAndProject();
			SetProjectNameParameter("MyProject");
			SetSourceParameter("http://sharpdevelop.com");
			RunCmdlet();
			
			string packageSource = fakeConsoleHost.PackageSourcePassedToGetProject;
			
			Assert.AreEqual("http://sharpdevelop.com", packageSource);
		}
		
		[Test]
		public void ProcessRecord_UpdateAllPackagesInProjectWhenTwoPackagesInProject_TwoUpdateActionsAreExecuted()
		{
			CreateCmdletWithActivePackageSourceAndProject();
			SetProjectNameParameter("MyProject");
			CreateTwoUpdateActionsWhenUpdatingAllPackagesInProject("PackageA", "PackageB");
			RunCmdlet();
			
			bool[] executedActions = new bool[] {
				FirstUpdateActionWhenUpdatingAllPackagesInProject.IsExecuted,
				SecondUpdateActionWhenUpdatingAllPackagesInProject.IsExecuted
			};
			
			bool[] expectedExecutedActions = new bool[] {
				true,
				true
			};
			
			CollectionAssert.AreEqual(expectedExecutedActions, executedActions);
		}
		
		[Test]
		public void ProcessRecord_UpdateAllPackagesInProjectWhenOnePackageInProjectAndIgnoreDependenciesIsTrue_ActionDoesNotUpdateDependencies()
		{
			CreateCmdletWithActivePackageSourceAndProject();
			SetProjectNameParameter("MyProject");
			CreateUpdateActionWhenUpdatingAllPackagesInProject("PackageA");
			EnableIgnoreDependenciesParameter();
			RunCmdlet();
			
			bool update = fakeUpdateAllPackagesInProject.UpdateDependencies;
			
			Assert.IsFalse(update);
		}
		
		[Test]
		public void ProcessRecord_UpdateAllPackagesInProjectWhenOnePackageInProjectAndIgnoreDependenciesIsFalse_ActionUpdatesDependencies()
		{
			CreateCmdletWithActivePackageSourceAndProject();
			SetProjectNameParameter("MyProject");
			CreateUpdateActionWhenUpdatingAllPackagesInProject("PackageA");
			RunCmdlet();
			
			bool update = fakeUpdateAllPackagesInProject.UpdateDependencies;
			
			Assert.IsTrue(update);
		}
		
		[Test]
		public void ProcessRecord_UpdateAllPackagesInProjectWhenOnePackageInProject_ActionsUsesCmdletAsScriptRunner()
		{
			CreateCmdletWithActivePackageSourceAndProject();
			SetProjectNameParameter("MyProject");
			CreateUpdateActionWhenUpdatingAllPackagesInProject("PackageA");
			RunCmdlet();
			
			IPackageScriptRunner runner = fakeUpdateAllPackagesInProject.PackageScriptRunner;
			
			Assert.AreEqual(cmdlet, runner);
		}
	}
}
