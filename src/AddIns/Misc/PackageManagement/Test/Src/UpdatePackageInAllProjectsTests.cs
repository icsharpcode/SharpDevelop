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
	public class UpdatePackageInAllProjectsTests
	{
		UpdatePackageInAllProjects updatePackageInAllProjects;
		FakePackageManagementSolution fakeSolution;
		FakePackageRepository fakeSourceRepository;
		List<UpdatePackageAction> updateActions;
		
		void CreateUpdatePackageInAllProjects(string packageId)
		{
			CreateUpdatePackageInAllProjects(packageId, new SemanticVersion("1.0"));
		}
		
		void CreateUpdatePackageInAllProjects(string packageId, SemanticVersion version)
		{
			fakeSolution = new FakePackageManagementSolution();
			fakeSourceRepository = new FakePackageRepository();
			var packageReference = new PackageReference(packageId, version, null, null);
			updatePackageInAllProjects = new UpdatePackageInAllProjects(packageReference, fakeSolution, fakeSourceRepository);
		}
		
		void CreateUpdatePackageInAllProjects()
		{
			CreateUpdatePackageInAllProjects("MyPackageId");
		}
		
		void CallCreateActions()
		{
			IEnumerable<UpdatePackageAction> actions = updatePackageInAllProjects.CreateActions();
			updateActions = actions.ToList();
		}
		
		UpdatePackageAction FirstUpdateAction {
			get { return updateActions[0]; }
		}
		
		FakePackageManagementProject AddProjectToSolution(string projectName)
		{
			return fakeSolution.AddFakeProject(projectName);
		}
		
		FakePackageManagementProject FirstProjectInSolution {
			get { return fakeSolution.FakeProjects[0]; }
		}
		
		[Test]
		public void CreateActions_SolutionHasOneProject_ReturnsOneAction()
		{
			CreateUpdatePackageInAllProjects();
			AddProjectToSolution("MyProject");
			CallCreateActions();
			
			Assert.AreEqual(1, updateActions.Count);
		}
		
		[Test]
		public void CreateActions_SolutionHasOneProject_UpdateActionCreatedFromProject()
		{
			CreateUpdatePackageInAllProjects();
			AddProjectToSolution("MyProject");
			CallCreateActions();
			
			UpdatePackageAction action = FirstUpdateAction;
			UpdatePackageAction expectedAction = FirstProjectInSolution.FirstFakeUpdatePackageActionCreated;
			
			Assert.AreEqual(expectedAction, action);
		}
		
		[Test]
		public void CreateActions_SolutionHasOneProject_UpdateActionCreatedUsingSourceRepositoryPassedInConstructor()
		{
			CreateUpdatePackageInAllProjects();
			AddProjectToSolution("MyProject");
			CallCreateActions();
			
			IPackageRepository repository = fakeSolution.SourceRepositoryPassedToGetProjects;
			FakePackageRepository expectedRepository = fakeSourceRepository;
			
			Assert.AreEqual(expectedRepository, repository);
		}
		
		[Test]
		public void CreateActions_NoPackagesInSolution_NoActionsReturned()
		{
			CreateUpdatePackageInAllProjects();
			CallCreateActions();
			
			Assert.AreEqual(0, updateActions.Count);
		}
		
		[Test]
		public void CreateActions_TwoProjectsInSolution_ReturnsTwoActions()
		{
			CreateUpdatePackageInAllProjects();
			AddProjectToSolution("MyProject1");
			AddProjectToSolution("MyProject2");
			CallCreateActions();
			
			Assert.AreEqual(2, updateActions.Count);
		}
		
		[Test]
		public void CreateActions_OnePackageInSolutionWithTwoProjects_ReturnsTwoActionsCreatedFromProjects()
		{
			CreateUpdatePackageInAllProjects();
			FakePackageManagementProject project1 = AddProjectToSolution("MyProject1");
			FakePackageManagementProject project2 = AddProjectToSolution("MyProject2");
			CallCreateActions();
			
			var expectedActions = new UpdatePackageAction[] {
				project1.FirstFakeUpdatePackageActionCreated,
				project2.FirstFakeUpdatePackageActionCreated
			};
			
			Assert.AreEqual(expectedActions, updateActions);
		}
		
		[Test]
		public void CreateActions_SolutionHasOneProject_PackageIdSetInUpdateAction()
		{
			CreateUpdatePackageInAllProjects("MyPackage");
			AddProjectToSolution("MyProject");
			CallCreateActions();
			
			string packageId = FirstUpdateAction.PackageId;
			
			Assert.AreEqual("MyPackage", packageId);
		}
		
		[Test]
		public void CreateActions_SolutionHasOneProject_PackageVersionSetInUpdateAction()
		{
			var expectedVersion = new SemanticVersion("1.2.3.4");
			CreateUpdatePackageInAllProjects("MyPackage", expectedVersion);
			AddProjectToSolution("MyProject");
			CallCreateActions();
			
			SemanticVersion version = FirstUpdateAction.PackageVersion;
			
			Assert.AreEqual(expectedVersion, version);
		}
		
		[Test]
		public void CreateActions_SolutionHasOneProjectAndScriptRunnerIsSet_UpdateActionUsesSameScriptRunner()
		{
			CreateUpdatePackageInAllProjects();
			AddProjectToSolution("MyProject");
			var expectedRunner = new FakePackageScriptRunner();
			updatePackageInAllProjects.PackageScriptRunner = expectedRunner;
			CallCreateActions();
			
			IPackageScriptRunner runner = FirstUpdateAction.PackageScriptRunner;
			
			Assert.AreEqual(expectedRunner, runner);
		}
		
		[Test]
		public void CreateActions_SolutionHasOneProject_UpdateActionDoesNotUpdatePackageIfProjectDoesNotHavePackage()
		{
			CreateUpdatePackageInAllProjects();
			AddProjectToSolution("MyProject");
			CallCreateActions();
			
			bool updateIfPackageDoesNotExist = FirstUpdateAction.UpdateIfPackageDoesNotExistInProject;
			
			Assert.IsFalse(updateIfPackageDoesNotExist);
		}
		
		[Test]
		public void CreateActions_SolutionHasOneProjectAndUpdateDependenciesIsFalse_UpdateActionDoesNotUpdateDependencies()
		{
			CreateUpdatePackageInAllProjects();
			AddProjectToSolution("MyProject");
			updatePackageInAllProjects.UpdateDependencies = false;
			CallCreateActions();
			
			bool updateDependencies = FirstUpdateAction.UpdateDependencies;
			
			Assert.IsFalse(updateDependencies);
		}
		
		[Test]
		public void CreateActions_SolutionHasOneProjectAndUpdateDependenciesIsTrue_UpdateActionDoesUpdateDependencies()
		{
			CreateUpdatePackageInAllProjects();
			AddProjectToSolution("MyProject");
			updatePackageInAllProjects.UpdateDependencies = true;
			CallCreateActions();
			
			bool updateDependencies = FirstUpdateAction.UpdateDependencies;
			
			Assert.IsTrue(updateDependencies);
		}
		
		[Test]
		public void CreateActions_SolutionHasOneProjectAndAllowPrereleaseVersionsIsFalse_UpdateActionDoesNotAllowPrereleaseVersions()
		{
			CreateUpdatePackageInAllProjects();
			AddProjectToSolution("MyProject");
			updatePackageInAllProjects.AllowPrereleaseVersions = false;
			CallCreateActions();
			
			bool allowPrereleases = FirstUpdateAction.AllowPrereleaseVersions;
			
			Assert.IsFalse(allowPrereleases);
		}
		
		[Test]
		public void CreateActions_SolutionHasOneProjectAndAllowPrereleaseVersionsIsTrue_UpdateActionDoesAllowPrereleaseVersions()
		{
			CreateUpdatePackageInAllProjects();
			AddProjectToSolution("MyProject");
			updatePackageInAllProjects.AllowPrereleaseVersions = true;
			CallCreateActions();
			
			bool allowPrereleases = FirstUpdateAction.AllowPrereleaseVersions;
			
			Assert.IsTrue(allowPrereleases);
		}
	}
}
