// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Linq;
using ICSharpCode.PackageManagement;
using ICSharpCode.PackageManagement.Design;
using ICSharpCode.PackageManagement.Scripting;
using NuGet;
using NUnit.Framework;
using PackageManagement.Tests.Helpers;

namespace PackageManagement.Tests
{
	[TestFixture]
	public class UpdateAllPackagesInSolutionTests
	{
		UpdateAllPackagesInSolution updateAllPackagesInSolution;
		FakePackageManagementSolution fakeSolution;
		List<UpdatePackageAction> updateActions;
		FakePackageRepository fakeSourceRepository;
		
		void CreateUpdateAllPackagesInSolution()
		{
			fakeSolution = new FakePackageManagementSolution();
			fakeSourceRepository = new FakePackageRepository();
			updateAllPackagesInSolution = new UpdateAllPackagesInSolution(fakeSolution, fakeSourceRepository);
		}
		
		void AddPackageToSolution(string packageId)
		{
			var package = new FakePackage(packageId, "1.0");
			fakeSolution.FakePackagesInReverseDependencyOrder.Add(package);
		}
		
		FakePackageManagementProject AddProjectToSolution(string projectName)
		{
			return fakeSolution.AddFakeProject(projectName);
		}
		
		void CallCreateActions()
		{
			IEnumerable<UpdatePackageAction> actions = updateAllPackagesInSolution.CreateActions();
			updateActions = actions.ToList();
		}
		
		UpdatePackageAction FirstUpdateAction {
			get { return updateActions[0]; }
		}
		
		FakePackageManagementProject FirstProjectInSolution {
			get { return fakeSolution.FakeProjects[0]; }
		}
		
		[Test]
		public void CreateActions_OnePackageInSolutionWithOneProject_ReturnsOneAction()
		{
			CreateUpdateAllPackagesInSolution();
			AddProjectToSolution("MyProject");
			AddPackageToSolution("Test");
			CallCreateActions();
			
			Assert.AreEqual(1, updateActions.Count);
		}
		
		[Test]
		public void CreateActions_OnePackageInSolutionWithOneProject_UpdateActionCreatedFromProject()
		{
			CreateUpdateAllPackagesInSolution();
			AddProjectToSolution("MyProject");
			AddPackageToSolution("Test");
			CallCreateActions();
			
			UpdatePackageAction action = FirstUpdateAction;
			UpdatePackageAction expectedAction = FirstProjectInSolution.FirstFakeUpdatePackageActionCreated;
			
			Assert.AreEqual(expectedAction, action);
		}
		
		[Test]
		public void CreateActions_OnePackageInSolutionWithOneProject_UpdateActionCreatedUsingSourceRepositoryPassedInConstructor()
		{
			CreateUpdateAllPackagesInSolution();
			AddProjectToSolution("MyProject");
			AddPackageToSolution("Test");
			CallCreateActions();
			
			IPackageRepository repository = fakeSolution.SourceRepositoryPassedToGetProjects;
			FakePackageRepository expectedRepository = fakeSourceRepository;
			
			Assert.AreEqual(expectedRepository, repository);
		}
		
		[Test]
		public void CreateActions_NoPackagesInSolution_NoActionsReturned()
		{
			CreateUpdateAllPackagesInSolution();
			CallCreateActions();
			
			Assert.AreEqual(0, updateActions.Count);
		}
		
		[Test]
		public void CreateActions_TwoPackagesInSolutionWithOneProject_ReturnsTwoActions()
		{
			CreateUpdateAllPackagesInSolution();
			AddProjectToSolution("MyProject");
			AddPackageToSolution("Test1");
			AddPackageToSolution("Test2");
			CallCreateActions();
			
			Assert.AreEqual(2, updateActions.Count);
		}
		
		[Test]
		public void CreateActions_TwoPackagesInSolutionWithOneProject_ReturnsTwoActionsWithPackageIdsForTwoPackages()
		{
			CreateUpdateAllPackagesInSolution();
			FakePackageManagementProject project = AddProjectToSolution("MyProject");
			AddPackageToSolution("Test1");
			AddPackageToSolution("Test2");
			CallCreateActions();
			
			string[] packageIds = new string[] {
				project.FirstFakeUpdatePackageActionCreated.PackageId,
				project.SecondFakeUpdatePackageActionCreated.PackageId
			};
			
			string [] expectedPackageIds = new string[] {
				"Test1",
				"Test2"
			};
			
			Assert.AreEqual(expectedPackageIds, packageIds);
		}
		
		[Test]
		public void CreateActions_OnePackageInSolutionWithTwoProjects_ReturnsTwoActions()
		{
			CreateUpdateAllPackagesInSolution();
			AddProjectToSolution("MyProject1");
			AddProjectToSolution("MyProject2");
			AddPackageToSolution("Test");
			CallCreateActions();
			
			Assert.AreEqual(2, updateActions.Count);
		}
		
		[Test]
		public void CreateActions_OnePackageInSolutionWithTwoProjects_ReturnsTwoActionsCreatedFromProjects()
		{
			CreateUpdateAllPackagesInSolution();
			FakePackageManagementProject project1 = AddProjectToSolution("MyProject1");
			FakePackageManagementProject project2 = AddProjectToSolution("MyProject2");
			AddPackageToSolution("Test");
			CallCreateActions();
			
			var expectedActions = new UpdatePackageAction[] {
				project1.FirstFakeUpdatePackageActionCreated,
				project2.FirstFakeUpdatePackageActionCreated
			};
			
			Assert.AreEqual(expectedActions, updateActions);
		}
		
		[Test]
		public void CreateActions_OnePackageInSolutionWithOneProjectAndScriptRunnerIsSet_UpdateActionUsesSameScriptRunner()
		{
			CreateUpdateAllPackagesInSolution();
			AddProjectToSolution("MyProject");
			AddPackageToSolution("Test");
			var expectedRunner = new FakePackageScriptRunner();
			updateAllPackagesInSolution.PackageScriptRunner = expectedRunner;
			CallCreateActions();
			
			IPackageScriptRunner runner = FirstUpdateAction.PackageScriptRunner;
			
			Assert.AreEqual(expectedRunner, runner);
		}
		
		[Test]
		public void CreateActions_OnePackageInSolutionWithOneProject_UpdateActionDoesNotUpdatePackageIfProjectDoesNotHavePackage()
		{
			CreateUpdateAllPackagesInSolution();
			AddProjectToSolution("MyProject");
			AddPackageToSolution("Test");
			CallCreateActions();
			
			bool updateIfPackageDoesNotExist = FirstUpdateAction.UpdateIfPackageDoesNotExistInProject;
			
			Assert.IsFalse(updateIfPackageDoesNotExist);
		}
		
		[Test]
		public void CreateActions_OnePackageInSolutionWithOneProjectAndUpdateDependenciesIsFalse_UpdateActionDoesNotUpdateDependencies()
		{
			CreateUpdateAllPackagesInSolution();
			AddProjectToSolution("MyProject");
			AddPackageToSolution("Test");
			updateAllPackagesInSolution.UpdateDependencies = false;
			CallCreateActions();
			
			bool updateDependencies = FirstUpdateAction.UpdateDependencies;
			
			Assert.IsFalse(updateDependencies);
		}
		
		[Test]
		public void CreateActions_OnePackageInSolutionWithOneProjectAndUpdateDependenciesIsTrue_UpdateActionDoesUpdateDependencies()
		{
			CreateUpdateAllPackagesInSolution();
			AddProjectToSolution("MyProject");
			AddPackageToSolution("Test");
			updateAllPackagesInSolution.UpdateDependencies = true;
			CallCreateActions();
			
			bool updateDependencies = FirstUpdateAction.UpdateDependencies;
			
			Assert.IsTrue(updateDependencies);
		}
		
		[Test]
		public void CreateActions_OnePackageInSolutionWithOneProjectAndAllowPrereleaseVersionsIsFalse_UpdateActionDoesNotAllowPrereleases()
		{
			CreateUpdateAllPackagesInSolution();
			AddProjectToSolution("MyProject");
			AddPackageToSolution("Test");
			updateAllPackagesInSolution.AllowPrereleaseVersions = false;
			CallCreateActions();
			
			bool allowPrereleases = FirstUpdateAction.AllowPrereleaseVersions;
			
			Assert.IsFalse(allowPrereleases);
		}
		
		[Test]
		public void CreateActions_OnePackageInSolutionWithOneProjectAndAllowPrereleaseVersionsIsTrue_UpdateActionDoesAllowPrereleases()
		{
			CreateUpdateAllPackagesInSolution();
			AddProjectToSolution("MyProject");
			AddPackageToSolution("Test");
			updateAllPackagesInSolution.AllowPrereleaseVersions = true;
			CallCreateActions();
			
			bool allowPrereleases = FirstUpdateAction.AllowPrereleaseVersions;
			
			Assert.IsTrue(allowPrereleases);
		}
	}
}
