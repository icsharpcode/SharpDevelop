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
			var packageReference = new PackageReference(packageId, version, null, null, false, false);
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
