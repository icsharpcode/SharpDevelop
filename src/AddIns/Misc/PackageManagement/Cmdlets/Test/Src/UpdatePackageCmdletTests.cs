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
using System.Collections.Generic;
using System.Linq;
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
		FakeUpdatePackageActions fakeUpdatePackageInAllProjects;
		
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
			fakeUpdatePackageInAllProjects = fakeUpdateActionsFactory.FakeUpdatePackageInAllProjects;
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
		
		FakeUpdatePackageAction FirstUpdateActionWhenUpdatingPackageInAllProjects {
			get { return GetUpdatingPackageInAllProjectsAction(0); }
		}
		
		FakeUpdatePackageAction GetUpdatingPackageInAllProjectsAction(int index)
		{
			return fakeUpdateActionsFactory.FakeUpdatePackageInAllProjects.FakeActions[index];
		}
		
		FakeUpdatePackageAction SecondUpdateActionWhenUpdatingPackageInAllProjects {
			get { return GetUpdatingPackageInAllProjectsAction(1); }
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
		
		FakeUpdatePackageAction CreateUpdateActionWhenUpdatingAllPackagesInSolution(string packageId)
		{
			var action = new FakeUpdatePackageAction(fakeProject);
			action.PackageId = packageId;
			fakeUpdateActionsFactory.FakeUpdateAllPackagesInSolution.FakeActions.Add(action);
			return action;
		}
		
		void CreateTwoUpdateActionsWhenUpdatingPackageInAllProjects(string packageId1, string packageId2)
		{
			CreateUpdateActionWhenUpdatingPackageInAllProjects(packageId1);
			CreateUpdateActionWhenUpdatingPackageInAllProjects(packageId2);
		}
		
		FakeUpdatePackageAction CreateUpdateActionWhenUpdatingPackageInAllProjects(string packageId)
		{
			var action = new FakeUpdatePackageAction(fakeProject);
			action.PackageId = packageId;
			fakeUpdateActionsFactory.FakeUpdatePackageInAllProjects.FakeActions.Add(action);
			return action;
		}
		
		FakeReinstallPackageAction ReinstallPackageInSingleProjectAction {
			get { return fakeProject.FakeReinstallPackageActionsCreated.First(); }
		}
		
		List<FakeReinstallPackageAction> ReinstallPackageActionsCreated {
			get { return fakeProject.FakeReinstallPackageActionsCreated; }
		}
		
		FakePackage ProjectHasPackageInstalled(string packageId, string version)
		{
			return ProjectHasPackageInstalled(fakeProject, packageId, version);
		}
		
		static FakePackage ProjectHasPackageInstalled(FakePackageManagementProject project, string packageId, string version)
		{
			FakePackage package = project.FakeLocalRepository.AddFakePackageWithVersion(packageId, version);
			project.FakePackages.Add(package);
			return package;
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
		public void ProcessRecord_PackageIdAndProjectNameAndPreleaseParameterNotSet_AllowPrereleaseVersionsIsFalseWhenUpdatingPackage()
		{
			CreateCmdletWithActivePackageSourceAndProject();
			
			SetIdParameter("Test");
			SetProjectNameParameter("MyProject");
			RunCmdlet();
			
			bool result = UpdatePackageInSingleProjectAction.AllowPrereleaseVersions;
			
			Assert.IsFalse(result);
		}
		
		[Test]
		public void ProcessRecord_PackageIdAndProjectNameAndPreleaseParameterSet_AllowPrereleaseVersionsIsTrueWhenUpdatingPackage()
		{
			CreateCmdletWithActivePackageSourceAndProject();
			
			SetIdParameter("Test");
			SetProjectNameParameter("MyProject");
			EnablePrereleaseParameter();
			RunCmdlet();
			
			bool result = UpdatePackageInSingleProjectAction.AllowPrereleaseVersions;
			
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
			var version = new SemanticVersion("1.0.1");
			SetVersionParameter(version);
			RunCmdlet();
			
			SemanticVersion actualVersion = UpdatePackageInSingleProjectAction.PackageVersion;
			
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
		public void ProcessRecord_UpdateAllPackagesInProjectWhenOnePackageInProjectAndPrereleaseIsTrue_ActionAllowsPrereleaseVersions()
		{
			CreateCmdletWithActivePackageSourceAndProject();
			SetProjectNameParameter("MyProject");
			CreateUpdateActionWhenUpdatingAllPackagesInProject("PackageA");
			EnablePrereleaseParameter();
			RunCmdlet();
			
			bool allow = fakeUpdateAllPackagesInProject.AllowPrereleaseVersions;
			
			Assert.IsTrue(allow);
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
		public void ProcessRecord_UpdateAllPackagesInSolutionWhenProjectNameIsEmptyString_CreatesUpdateAllPackagesSolution()
		{
			CreateCmdletWithActivePackageSourceAndProject();
			SetProjectNameParameter(String.Empty);
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
		
		[Test]
		public void ProcessRecord_UpdatePackageInAllProjects_CreatesUpdatePackageInAllProjects()
		{
			CreateCmdletWithActivePackageSourceAndProject();
			SetIdParameter("MyPackage");
			RunCmdlet();
			
			IPackageManagementSolution solution = 
				fakeUpdateActionsFactory.SolutionPassedToCreateUpdatePackageInAllProjects;
			
			Assert.AreEqual(fakeSolution, solution);
		}
		
		[Test]
		public void ProcessRecord_UpdatePackageInAllProjectsWhenSourceSpecified_PackageSourceUsedToGetRepository()
		{
			CreateCmdletWithActivePackageSourceAndProject();
			SetIdParameter("MyPackage");
			RunCmdlet();
			
			PackageSource packageSource = fakeConsoleHost.PackageSourcePassedToGetRepository;
			PackageSource expectedPackageSource = fakeConsoleHost.PackageSourceToReturnFromGetActivePackageSource;
			
			Assert.AreEqual(expectedPackageSource, packageSource);
		}
		
		[Test]
		public void ProcessRecord_UpdatePackageInAllProjectsWhenSourceSpecified_SourceUsedPassedGetActvePackageSourceFromConsoleHost()
		{
			CreateCmdletWithActivePackageSourceAndProject();
			SetIdParameter("MyPackage");
			SetSourceParameter("Test");
			RunCmdlet();
			
			string packageSource = fakeConsoleHost.PackageSourcePassedToGetActivePackageSource;
			
			Assert.AreEqual("Test", packageSource);
		}
		
		[Test]
		public void ProcessRecord_UpdatePackageInAllProjects_RepositoryUsedToCreateUpdatePackageInAllProjects()
		{
			CreateCmdletWithActivePackageSourceAndProject();
			SetIdParameter("MyPackage");
			RunCmdlet();
			
			IPackageRepository repository = fakeUpdateActionsFactory.SourceRepositoryPassedToCreateUpdatePackageInAllProjects;
			IPackageRepository expectedRepository = fakeConsoleHost.FakePackageRepository;
			
			Assert.AreEqual(expectedRepository, repository);
		}
		
		[Test]
		public void ProcessRecord_UpdatePackageInAllProjectsWhenTwoUpdateActionsReturned_TwoUpdateActionsAreExecuted()
		{
			CreateCmdletWithActivePackageSourceAndProject();
			CreateTwoUpdateActionsWhenUpdatingPackageInAllProjects("PackageA", "PackageB");
			SetIdParameter("MyPackage");
			RunCmdlet();
			
			bool[] executedActions = new bool[] {
				FirstUpdateActionWhenUpdatingPackageInAllProjects.IsExecuted,
				SecondUpdateActionWhenUpdatingPackageInAllProjects.IsExecuted
			};
			
			bool[] expectedExecutedActions = new bool[] {
				true,
				true
			};
			
			CollectionAssert.AreEqual(expectedExecutedActions, executedActions);
		}
		
		[Test]
		public void ProcessRecord_UpdatePackageInAllProjectsAndIgnoreDependenciesIsTrue_ActionDoesNotUpdateDependencies()
		{
			CreateCmdletWithActivePackageSourceAndProject();
			CreateUpdateActionWhenUpdatingPackageInAllProjects("PackageA");
			SetIdParameter("PackageA");
			EnableIgnoreDependenciesParameter();
			RunCmdlet();
			
			bool update = fakeUpdatePackageInAllProjects.UpdateDependencies;
			
			Assert.IsFalse(update);
		}
		
		[Test]
		public void ProcessRecord_UpdatePackageInAllProjectsAndIgnoreDependenciesIsFalse_ActionUpdatesDependencies()
		{
			CreateCmdletWithActivePackageSourceAndProject();
			CreateUpdateActionWhenUpdatingPackageInAllProjects("PackageA");
			SetIdParameter("PackageA");
			RunCmdlet();
			
			bool update = fakeUpdatePackageInAllProjects.UpdateDependencies;
			
			Assert.IsTrue(update);
		}
		
		[Test]
		public void ProcessRecord_UpdatePackageInAllProjects_ActionsUseCmdletAsScriptRunner()
		{
			CreateCmdletWithActivePackageSourceAndProject();
			SetIdParameter("MyPackage");
			RunCmdlet();
			
			IPackageScriptRunner runner = fakeUpdatePackageInAllProjects.PackageScriptRunner;
			
			Assert.AreEqual(cmdlet, runner);
		}
		
		[Test]
		public void ProcessRecord_UpdatePackageInAllProjects_PackageIdPassedToAction()
		{
			CreateCmdletWithActivePackageSourceAndProject();
			SetIdParameter("MyPackage");
			RunCmdlet();
			
			PackageReference packageReference = 
				fakeUpdateActionsFactory.PackageReferencePassedToCreateUpdatePackageInAllProjects;
			
			Assert.AreEqual("MyPackage", packageReference.Id);
		}
		
		[Test]
		public void ProcessRecord_UpdatePackageInAllProjects_PackageVersionPassedToAction()
		{
			CreateCmdletWithActivePackageSourceAndProject();
			SetIdParameter("MyPackage");
			var expectedVersion = new SemanticVersion("1.0");
			SetVersionParameter(expectedVersion);
			RunCmdlet();
			
			PackageReference packageReference = 
				fakeUpdateActionsFactory.PackageReferencePassedToCreateUpdatePackageInAllProjects;
			
			Assert.AreEqual(expectedVersion, packageReference.Version);
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
		public void ProcessRecord_PackageIdAndProjectNameSpecifiedSpecifiedAndSourceRepositoryIsOperationAware_UpdateOperationStartedAndDisposedForPackage()
		{
			CreateCmdletWithActivePackageSourceAndProject();
			SetProjectNameParameter("TestProject");
			SetIdParameter("Test");
			var operationAwareRepository = new FakeOperationAwarePackageRepository();
			fakeProject.FakeSourceRepository = operationAwareRepository;
			
			RunCmdlet();
			
			operationAwareRepository.AssertOperationWasStartedAndDisposed(RepositoryOperationNames.Update, "Test");
		}
		
		[Test]
		public void ProcessRecord_PackageIdSpecifiedAndSourceRepositoryIsOperationAware_UpdateOperationStartedAndDisposedForPackage()
		{
			CreateCmdletWithoutActiveProject();
			AddDefaultProjectToConsoleHost();
			AddPackageSourceToConsoleHost();
			SetIdParameter("Test");
			FakeUpdatePackageAction action = CreateUpdateActionWhenUpdatingPackageInAllProjects("Test");
			var operationAwareRepository = new FakeOperationAwarePackageRepository();
			action.FakeProject.FakeSourceRepository = operationAwareRepository;
			
			RunCmdlet();
			
			operationAwareRepository.AssertOperationWasStartedAndDisposed(RepositoryOperationNames.Update, "Test");
		}
		
		[Test]
		public void ProcessRecord_UpdateAllPackagesIsSolutionAndTwoUpdateActionsAndSourceRepositoryIsOperationAware_UpdateOperationStartedAndDisposedForSecondPackage()
		{
			CreateCmdletWithoutActiveProject();
			AddDefaultProjectToConsoleHost();
			AddPackageSourceToConsoleHost();
			CreateUpdateActionWhenUpdatingAllPackagesInSolution("Test1");
			FakeUpdatePackageAction action = CreateUpdateActionWhenUpdatingAllPackagesInSolution("Test2");
			var operationAwareRepository = new FakeOperationAwarePackageRepository();
			action.FakeProject.FakeSourceRepository = operationAwareRepository;
			
			RunCmdlet();
			
			operationAwareRepository.AssertOperationWasStartedAndDisposed(RepositoryOperationNames.Update, "Test2");
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
		public void ProcessRecord_ReinstallWhenPackageIdAndProjectNameSpecified_ReinstallPackageActionIsExecuted()
		{
			CreateCmdletWithActivePackageSourceAndProject();
			SetIdParameter("Test");
			SetProjectNameParameter("MyProject");
			cmdlet.Reinstall = true; 
			FakePackage testPackage = fakeProject.FakeLocalRepository.AddFakePackageWithVersion("Test", "1.2.3");
			
			RunCmdlet();
			
			Assert.IsTrue(ReinstallPackageInSingleProjectAction.IsExecuted);
			Assert.AreEqual(cmdlet, ReinstallPackageInSingleProjectAction.PackageScriptRunner);
			Assert.AreEqual("Test", ReinstallPackageInSingleProjectAction.PackageId);
			Assert.AreEqual(testPackage.Version, ReinstallPackageInSingleProjectAction.PackageVersion);
			Assert.IsFalse(ReinstallPackageInSingleProjectAction.AllowPrereleaseVersions);
			Assert.IsTrue(ReinstallPackageInSingleProjectAction.UpdateDependencies);
		}
		
		[Test]
		public void ProcessRecord_ReinstallPackageIdIntoProjectWhenPackageIdNotFound_ExceptionThrownAboutMissingPackage()
		{
			CreateCmdletWithActivePackageSourceAndProject();
			SetIdParameter("UnknownPackageId");
			SetProjectNameParameter("MyProject");
			cmdlet.Reinstall = true; 
			FakePackage testPackage = fakeProject.FakeLocalRepository.AddFakePackageWithVersion("Test", "1.2.3");
			
			var ex = Assert.Throws<InvalidOperationException>(RunCmdlet);
			
			Assert.AreEqual("Unable to find package 'UnknownPackageId'.", ex.Message);
		}
		
		[Test]
		public void ProcessRecord_ReinstallWhenPackageIdAndProjectNameSpecifiedAndSourceRepositoryIsOperationAware_ReinstallOperationStartedAndDisposedForPackage()
		{
			CreateCmdletWithActivePackageSourceAndProject();
			SetIdParameter("Test");
			SetProjectNameParameter("MyProject");
			fakeProject.FakeLocalRepository.AddFakePackage("Test");
			var operationAwareRepository = new FakeOperationAwarePackageRepository();
			fakeProject.FakeSourceRepository = operationAwareRepository;
			cmdlet.Reinstall = true;
			
			RunCmdlet();
			
			operationAwareRepository.AssertOperationWasStartedAndDisposed(RepositoryOperationNames.Reinstall, "Test");
		}
		
		[Test]
		public void ProcessRecord_ReinstallWhenPackageIdAndProjectNameSpecifiedAndIncludePrerelease_PrereleasesAllowedForReinstall()
		{
			CreateCmdletWithActivePackageSourceAndProject();
			SetIdParameter("Test");
			SetProjectNameParameter("MyProject");
			cmdlet.Reinstall = true;
			cmdlet.IncludePrerelease = true;
			fakeProject.FakeLocalRepository.AddFakePackageWithVersion("Test", "1.2.3");
			
			RunCmdlet();
			
			Assert.IsTrue(ReinstallPackageInSingleProjectAction.IsExecuted);
			Assert.IsTrue(ReinstallPackageInSingleProjectAction.AllowPrereleaseVersions);
		}
		
		[Test]
		public void ProcessRecord_ReinstallWhenPackageIdAndProjectNameSpecifiedAndLocalPackageIsPrerelease_PrereleasesAllowedForReinstall()
		{
			CreateCmdletWithActivePackageSourceAndProject();
			SetIdParameter("Test");
			SetProjectNameParameter("MyProject");
			cmdlet.Reinstall = true;
			cmdlet.IncludePrerelease = false;
			fakeProject.FakeLocalRepository.AddFakePackageWithVersion("Test", "1.2.3-alpha1");
			
			RunCmdlet();
			
			Assert.IsTrue(ReinstallPackageInSingleProjectAction.IsExecuted);
			Assert.IsTrue(ReinstallPackageInSingleProjectAction.AllowPrereleaseVersions);
		}
		
		[Test]
		public void ProcessRecord_ReinstallWhenPackageIdAndProjectNameSpecifiedAndIgnoreDependenciesIsTrue_DoNotUpdateDepenenciesOnReinstall()
		{
			CreateCmdletWithActivePackageSourceAndProject();
			SetIdParameter("Test");
			SetProjectNameParameter("MyProject");
			cmdlet.Reinstall = true;
			cmdlet.IgnoreDependencies = true;
			fakeProject.FakeLocalRepository.AddFakePackageWithVersion("Test", "1.2.3");
			
			RunCmdlet();
			
			Assert.IsTrue(ReinstallPackageInSingleProjectAction.IsExecuted);
			Assert.IsFalse(ReinstallPackageInSingleProjectAction.UpdateDependencies);
		}
		
		[Test]
		public void ProcessRecord_ReinstallAllPackagesInProjectWithTwoPackages_BothPackagesReinstalled()
		{
			CreateCmdletWithActivePackageSourceAndProject();
			SetProjectNameParameter("MyProject");
			cmdlet.Reinstall = true;
			cmdlet.IgnoreDependencies = false;
			FakePackage packageA = ProjectHasPackageInstalled("PackageA", "1.1");
			FakePackage packageB = ProjectHasPackageInstalled("PackageB", "1.2");
			
			RunCmdlet();
			
			Assert.AreEqual(2, ReinstallPackageActionsCreated.Count);
			Assert.IsTrue(ReinstallPackageActionsCreated[0].IsExecuted);
			Assert.IsTrue(ReinstallPackageActionsCreated[1].IsExecuted);
			Assert.AreEqual(cmdlet, ReinstallPackageActionsCreated[0].PackageScriptRunner);
			Assert.AreEqual(cmdlet, ReinstallPackageActionsCreated[1].PackageScriptRunner);
			Assert.AreEqual("PackageA", ReinstallPackageActionsCreated[0].PackageId);
			Assert.AreEqual("PackageB", ReinstallPackageActionsCreated[1].PackageId);
			Assert.AreEqual(packageA.Version, ReinstallPackageActionsCreated[0].PackageVersion);
			Assert.AreEqual(packageB.Version, ReinstallPackageActionsCreated[1].PackageVersion);
			Assert.IsFalse(ReinstallPackageActionsCreated[0].AllowPrereleaseVersions);
			Assert.IsFalse(ReinstallPackageActionsCreated[1].AllowPrereleaseVersions);
			Assert.IsFalse(ReinstallPackageActionsCreated[0].UpdateDependencies);
			Assert.IsFalse(ReinstallPackageActionsCreated[1].UpdateDependencies);
		}
		
		[Test]
		public void ProcessRecord_ReinstallAllPackagesInProjectWhenSourceRepositoryIsOperationAware_ReinstallOperationStartedAndDisposedForEachPackage()
		{
			CreateCmdletWithActivePackageSourceAndProject();
			SetProjectNameParameter("MyProject");
			cmdlet.Reinstall = true;
			ProjectHasPackageInstalled("PackageA", "1.1");
			ProjectHasPackageInstalled("PackageB", "1.2");
			var operationAwareRepository = new FakeOperationAwarePackageRepository();
			fakeProject.FakeSourceRepository = operationAwareRepository;
			cmdlet.Reinstall = true;
			
			RunCmdlet();
			
			Assert.AreEqual(2, operationAwareRepository.OperationsStarted.Count);
			operationAwareRepository.OperationsStarted[0].AssertOperationWasStartedAndDisposed(RepositoryOperationNames.Reinstall, "PackageA");
			operationAwareRepository.OperationsStarted[1].AssertOperationWasStartedAndDisposed(RepositoryOperationNames.Reinstall, "PackageB");
		}
		
		[Test]
		public void ProcessRecord_ReinstallPackageInAllProjectsWhenThreeProjectsButOnlyTwoHavePackage_PackageIsReinstalledInProjects()
		{
			CreateCmdletWithActivePackageSourceAndProject();
			AddPackageSourceToConsoleHost();
			SetSourceParameter("Test");
			SetIdParameter("B");
			cmdlet.Reinstall = true;
			cmdlet.IgnoreDependencies = false;
			FakePackageManagementProject project1 = fakeSolution.AddFakeProject("Project1");
			FakePackageManagementProject project2 = fakeSolution.AddFakeProject("Project2");
			FakePackageManagementProject project3 = fakeSolution.AddFakeProject("Project3");
			ProjectHasPackageInstalled(project1, "A", "1.1");
			ProjectHasPackageInstalled(project1, "B", "1.2");
			ProjectHasPackageInstalled(project3, "A", "1.1");
			ProjectHasPackageInstalled(project3, "B", "1.3");
			
			RunCmdlet();
			
			FakeReinstallPackageAction project1ReinstallAction = project1.FakeReinstallPackageActionsCreated.Single();
			FakeReinstallPackageAction project3ReinstallAction = project3.FakeReinstallPackageActionsCreated.Single();
			Assert.AreEqual(0, project2.FakeReinstallPackageActionsCreated.Count);
			Assert.IsTrue(project1ReinstallAction.IsExecuted);
			Assert.IsTrue(project3ReinstallAction.IsExecuted);
			Assert.AreEqual(project1, project1ReinstallAction.Project);
			Assert.AreEqual(project3, project3ReinstallAction.Project);
			Assert.AreEqual(cmdlet, project1ReinstallAction.PackageScriptRunner);
			Assert.AreEqual(cmdlet, project3ReinstallAction.PackageScriptRunner);
			Assert.AreEqual("B", project1ReinstallAction.PackageId);
			Assert.AreEqual("B", project3ReinstallAction.PackageId);
			Assert.AreEqual("1.2", project1ReinstallAction.PackageVersion.ToString());
			Assert.AreEqual("1.3", project3ReinstallAction.PackageVersion.ToString());
			Assert.IsFalse(project1ReinstallAction.AllowPrereleaseVersions);
			Assert.IsFalse(project3ReinstallAction.AllowPrereleaseVersions);
			Assert.IsTrue(project1ReinstallAction.UpdateDependencies);
			Assert.IsTrue(project3ReinstallAction.UpdateDependencies);
			Assert.AreEqual("Test", fakeConsoleHost.PackageSourcePassedToGetActivePackageSource);
			Assert.AreEqual(fakeConsoleHost.FakePackageRepository, fakeSolution.SourceRepositoryPassedToGetProjects);
		}
		
		[Test]
		public void ProcessRecord_ReinstallPackageInAllProjects_ReinstallOperationStartedAndDisposedForEachPackage()
		{
			CreateCmdletWithActivePackageSourceAndProject();
			AddPackageSourceToConsoleHost();
			SetSourceParameter("Test");
			SetIdParameter("B");
			cmdlet.Reinstall = true;
			cmdlet.IgnoreDependencies = false;
			FakePackageManagementProject project1 = fakeSolution.AddFakeProject("Project1");
			FakePackageManagementProject project2 = fakeSolution.AddFakeProject("Project2");
			ProjectHasPackageInstalled(project1, "A", "1.1");
			ProjectHasPackageInstalled(project1, "B", "1.2");
			ProjectHasPackageInstalled(project2, "A", "1.1");
			ProjectHasPackageInstalled(project2, "B", "1.3");
			var operationAwareRepository1 = new FakeOperationAwarePackageRepository();
			project1.FakeSourceRepository = operationAwareRepository1;
			var operationAwareRepository2 = new FakeOperationAwarePackageRepository();
			project2.FakeSourceRepository = operationAwareRepository2;
			
			RunCmdlet();
			
			operationAwareRepository1.OperationsStarted.Single().AssertOperationWasStartedAndDisposed(RepositoryOperationNames.Reinstall, "B");
			operationAwareRepository2.OperationsStarted.Single().AssertOperationWasStartedAndDisposed(RepositoryOperationNames.Reinstall, "B");
		}
		
		[Test]
		public void ProcessRecord_ReinstallPackageInAllProjectsButPackageNotFound_ExceptionThrown()
		{
			CreateCmdletWithActivePackageSourceAndProject();
			AddPackageSourceToConsoleHost();
			SetSourceParameter("Test");
			SetIdParameter("UnknownPackageId");
			cmdlet.Reinstall = true;
			FakePackageManagementProject project1 = fakeSolution.AddFakeProject("Project1");
			FakePackageManagementProject project2 = fakeSolution.AddFakeProject("Project2");
			FakePackageManagementProject project3 = fakeSolution.AddFakeProject("Project3");
			
			var ex = Assert.Throws<InvalidOperationException>(RunCmdlet);
			
			Assert.AreEqual("Unable to find package 'UnknownPackageId'.", ex.Message);
		}
		
		[Test]
		public void ProcessRecord_ReinstallAllPackagesInAllProjects_PackagesAreReinstalledInAllProjects()
		{
			CreateCmdletWithActivePackageSourceAndProject();
			AddPackageSourceToConsoleHost();
			SetSourceParameter("Test");
			cmdlet.Reinstall = true;
			cmdlet.IgnoreDependencies = false;
			FakePackageManagementProject project1 = fakeSolution.AddFakeProject("Project1");
			FakePackageManagementProject project2 = fakeSolution.AddFakeProject("Project2");
			ProjectHasPackageInstalled(project1, "A", "1.1");
			ProjectHasPackageInstalled(project1, "B", "1.2");
			ProjectHasPackageInstalled(project2, "A", "1.4");
			
			RunCmdlet();
			
			FakeReinstallPackageAction project2ReinstallAction = project2.FakeReinstallPackageActionsCreated.Single();
			Assert.AreEqual(2, project1.FakeReinstallPackageActionsCreated.Count);
			Assert.IsTrue(project1.FakeReinstallPackageActionsCreated[0].IsExecuted);
			Assert.IsTrue(project1.FakeReinstallPackageActionsCreated[1].IsExecuted);
			Assert.IsTrue(project2ReinstallAction.IsExecuted);
			Assert.AreEqual(project1, project1.FakeReinstallPackageActionsCreated[0].Project);
			Assert.AreEqual(project1, project1.FakeReinstallPackageActionsCreated[1].Project);
			Assert.AreEqual(project2, project2ReinstallAction.Project);
			Assert.AreEqual(cmdlet, project1.FakeReinstallPackageActionsCreated[0].PackageScriptRunner);
			Assert.AreEqual(cmdlet, project1.FakeReinstallPackageActionsCreated[1].PackageScriptRunner);
			Assert.AreEqual(cmdlet, project2ReinstallAction.PackageScriptRunner);
			Assert.AreEqual("A", project1.FakeReinstallPackageActionsCreated[0].PackageId);
			Assert.AreEqual("B", project1.FakeReinstallPackageActionsCreated[1].PackageId);
			Assert.AreEqual("A", project2ReinstallAction.PackageId);
			Assert.AreEqual("1.1", project1.FakeReinstallPackageActionsCreated[0].PackageVersion.ToString());
			Assert.AreEqual("1.2", project1.FakeReinstallPackageActionsCreated[1].PackageVersion.ToString());
			Assert.AreEqual("1.4", project2ReinstallAction.PackageVersion.ToString());
			Assert.IsFalse(project1.FakeReinstallPackageActionsCreated[0].AllowPrereleaseVersions);
			Assert.IsFalse(project1.FakeReinstallPackageActionsCreated[1].AllowPrereleaseVersions);
			Assert.IsFalse(project2ReinstallAction.AllowPrereleaseVersions);
			Assert.IsFalse(project1.FakeReinstallPackageActionsCreated[0].UpdateDependencies);
			Assert.IsFalse(project1.FakeReinstallPackageActionsCreated[1].UpdateDependencies);
			Assert.IsFalse(project2ReinstallAction.UpdateDependencies);
			Assert.AreEqual("Test", fakeConsoleHost.PackageSourcePassedToGetActivePackageSource);
			Assert.AreEqual(fakeConsoleHost.FakePackageRepository, fakeSolution.SourceRepositoryPassedToGetProjects);
		}
		
		[Test]
		public void ProcessRecord_ReinstallAllPackagesInAllProjects_ReinstallOperationStartedAndDisposedForEachPackage()
		{
			CreateCmdletWithActivePackageSourceAndProject();
			AddPackageSourceToConsoleHost();
			SetSourceParameter("Test");
			cmdlet.Reinstall = true;
			FakePackageManagementProject project1 = fakeSolution.AddFakeProject("Project1");
			FakePackageManagementProject project2 = fakeSolution.AddFakeProject("Project2");
			ProjectHasPackageInstalled(project1, "B", "1.2");
			ProjectHasPackageInstalled(project2, "B", "1.3");
			var operationAwareRepository1 = new FakeOperationAwarePackageRepository();
			project1.FakeSourceRepository = operationAwareRepository1;
			var operationAwareRepository2 = new FakeOperationAwarePackageRepository();
			project2.FakeSourceRepository = operationAwareRepository2;
			
			RunCmdlet();
			
			operationAwareRepository1.OperationsStarted.Single().AssertOperationWasStartedAndDisposed(RepositoryOperationNames.Reinstall, "B");
			operationAwareRepository2.OperationsStarted.Single().AssertOperationWasStartedAndDisposed(RepositoryOperationNames.Reinstall, "B");
		}
	}
}
