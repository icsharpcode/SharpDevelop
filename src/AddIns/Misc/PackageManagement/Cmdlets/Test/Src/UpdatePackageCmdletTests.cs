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
	public class UpdatePackageCmdletTests : CmdletTestsBase
	{
		TestableUpdatePackageCmdlet cmdlet;
		FakeCmdletTerminatingError fakeTerminatingError;
		FakePackageManagementProject fakeProject;
		FakeUpdatePackageActionsFactory fakeUpdateActionsFactory;
		FakeUpdatePackageActions fakeUpdateAllPackagesInProject;
		FakePackageManagementSolution fakeSolution;
		FakeUpdatePackageActions fakeUpdateAllPackagesInSolution;
		
		void CreateCmdletWithoutActiveProject()
		{
			cmdlet = new TestableUpdatePackageCmdlet();
			fakeTerminatingError = cmdlet.FakeCmdletTerminatingError;
			fakeConsoleHost = cmdlet.FakePackageManagementConsoleHost;
			fakeSolution = fakeConsoleHost.FakeSolution;
			fakeProject = fakeConsoleHost.FakeProject;
			fakeUpdateActionsFactory = cmdlet.FakeUpdatePackageActionsFactory;
			fakeUpdateAllPackagesInProject = fakeUpdateActionsFactory.FakeUpdateAllPackagesInProject;
			fakeUpdateAllPackagesInSolution = fakeUpdateActionsFactory.FakeUpdateAllPackagesInSolution;
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
		
		FakeUpdatePackageAction FirstUpdateActionWhenUpdatingAllPackagesInSolution {
			get { return GetUpdatingAllPackagesInSolutionAction(0); }
		}
		
		FakeUpdatePackageAction GetUpdatingAllPackagesInSolutionAction(int index)
		{
			return fakeUpdateActionsFactory.FakeUpdateAllPackagesInSolution.FakeActions[index];
		}
		
		FakeUpdatePackageAction SecondUpdateActionWhenUpdatingAllPackagesInSolution {
			get { return GetUpdatingAllPackagesInSolutionAction(1); }
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
		
		void CreateTwoUpdateActionsWhenUpdatingAllPackagesInSolution(string packageId1, string packageId2)
		{
			CreateUpdateActionWhenUpdatingAllPackagesInSolution(packageId1);
			CreateUpdateActionWhenUpdatingAllPackagesInSolution(packageId2);
		}
		
		void CreateUpdateActionWhenUpdatingAllPackagesInSolution(string packageId)
		{
			var action = new FakeUpdatePackageAction(fakeProject);
			action.PackageId = packageId;
			fakeUpdateActionsFactory.FakeUpdateAllPackagesInSolution.FakeActions.Add(action);
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
		public void ProcessRecord_PackageIdAndProjectNameSpecified_UpdatePackageActionIsExecuted()
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
		public void ProcessRecord_UpdateAllPackagesInProject_CreatesUpdateAllPackagesInProject()
		{
			CreateCmdletWithActivePackageSourceAndProject();
			SetProjectNameParameter("MyProject");
			RunCmdlet();
			
			IPackageManagementProject project = 
				fakeUpdateActionsFactory.ProjectPassedToCreateUpdateAllPackagesInProject;
			
			Assert.AreEqual(fakeProject, project);
		}
		
		[Test]
		public void ProcessRecord_UpdateAllPackagesInProjectWhenPackageIdIsEmptyString_CreatesUpdateAllPackagesInProject()
		{
			CreateCmdletWithActivePackageSourceAndProject();
			SetProjectNameParameter("MyProject");
			SetIdParameter(String.Empty);
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
		public void ProcessRecord_UpdateAllPackagesInProjectWhenOnePackageInProject_ActionsUseCmdletAsScriptRunner()
		{
			CreateCmdletWithActivePackageSourceAndProject();
			SetProjectNameParameter("MyProject");
			CreateUpdateActionWhenUpdatingAllPackagesInProject("PackageA");
			RunCmdlet();
			
			IPackageScriptRunner runner = fakeUpdateAllPackagesInProject.PackageScriptRunner;
			
			Assert.AreEqual(cmdlet, runner);
		}
		
		[Test]
		public void ProcessRecord_UpdateAllPackagesInSolution_CreatesUpdateAllPackagesSolution()
		{
			CreateCmdletWithActivePackageSourceAndProject();
			RunCmdlet();
			
			IPackageManagementSolution solution = 
				fakeUpdateActionsFactory.SolutionPassedToCreateUpdateAllPackagesInSolution;
			
			Assert.AreEqual(fakeSolution, solution);
		}
		
		[Test]
		public void ProcessRecord_UpdateAllPackagesInSolutionWhenSourceSpecified_PackageSourceUsedToGetRepository()
		{
			CreateCmdletWithActivePackageSourceAndProject();
			SetSourceParameter("Test");
			RunCmdlet();
			
			PackageSource packageSource = fakeConsoleHost.PackageSourcePassedToGetRepository;
			PackageSource expectedPackageSource = fakeConsoleHost.PackageSourceToReturnFromGetActivePackageSource;
			
			Assert.AreEqual(expectedPackageSource, packageSource);
		}
		
		[Test]
		public void ProcessRecord_UpdateAllPackagesInSolutionWhenSourceSpecified_SourceUsedPassedGetActvePackageSourceFromConsoleHost()
		{
			CreateCmdletWithActivePackageSourceAndProject();
			SetSourceParameter("Test");
			RunCmdlet();
			
			string packageSource = fakeConsoleHost.PackageSourcePassedToGetActivePackageSource;
			
			Assert.AreEqual("Test", packageSource);
		}
		
		[Test]
		public void ProcessRecord_UpdateAllPackagesInSolution_RepositoryUsedToCreateUpdateAllPackagesInSolution()
		{
			CreateCmdletWithActivePackageSourceAndProject();
			RunCmdlet();
			
			IPackageRepository repository = fakeUpdateActionsFactory.SourceRepositoryPassedToCreateUpdateAllPackagesInSolution;
			IPackageRepository expectedRepository = fakeConsoleHost.FakePackageRepository;
			
			Assert.AreEqual(expectedRepository, repository);
		}
		
		[Test]
		public void ProcessRecord_UpdateAllPackagesInSolutionWhenTwoUpdateActionsReturned_TwoUpdateActionsAreExecuted()
		{
			CreateCmdletWithActivePackageSourceAndProject();
			CreateTwoUpdateActionsWhenUpdatingAllPackagesInSolution("PackageA", "PackageB");
			RunCmdlet();
			
			bool[] executedActions = new bool[] {
				FirstUpdateActionWhenUpdatingAllPackagesInSolution.IsExecuted,
				SecondUpdateActionWhenUpdatingAllPackagesInSolution.IsExecuted
			};
			
			bool[] expectedExecutedActions = new bool[] {
				true,
				true
			};
			
			CollectionAssert.AreEqual(expectedExecutedActions, executedActions);
		}
		
		[Test]
		public void ProcessRecord_UpdateAllPackagesInSolutionAndIgnoreDependenciesIsTrue_ActionDoesNotUpdateDependencies()
		{
			CreateCmdletWithActivePackageSourceAndProject();
			CreateUpdateActionWhenUpdatingAllPackagesInSolution("PackageA");
			EnableIgnoreDependenciesParameter();
			RunCmdlet();
			
			bool update = fakeUpdateAllPackagesInSolution.UpdateDependencies;
			
			Assert.IsFalse(update);
		}
		
		[Test]
		public void ProcessRecord_UpdateAllPackagesInSolutionAndIgnoreDependenciesIsFalse_ActionUpdatesDependencies()
		{
			CreateCmdletWithActivePackageSourceAndProject();
			CreateUpdateActionWhenUpdatingAllPackagesInSolution("PackageA");
			RunCmdlet();
			
			bool update = fakeUpdateAllPackagesInSolution.UpdateDependencies;
			
			Assert.IsTrue(update);
		}
		
		[Test]
		public void ProcessRecord_UpdateAllPackagesInSolution_ActionsUseCmdletAsScriptRunner()
		{
			CreateCmdletWithActivePackageSourceAndProject();
			RunCmdlet();
			
			IPackageScriptRunner runner = fakeUpdateAllPackagesInSolution.PackageScriptRunner;
			
			Assert.AreEqual(cmdlet, runner);
		}
	}
}
