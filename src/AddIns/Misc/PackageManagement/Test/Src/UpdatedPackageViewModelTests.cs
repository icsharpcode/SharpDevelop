// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using ICSharpCode.PackageManagement;
using ICSharpCode.PackageManagement.Design;
using ICSharpCode.PackageManagement.Scripting;
using NuGet;
using NUnit.Framework;
using PackageManagement.Tests.Helpers;

namespace PackageManagement.Tests
{
	[TestFixture]
	public class UpdatedPackageViewModelTests
	{
		TestableUpdatedPackageViewModel viewModel;
		FakePackageManagementSolution fakeSolution;
		FakePackageManagementProject fakeProject;
		FakePackageActionRunner fakeActionRunner;
		FakePackageManagementEvents fakePackageManagementEvents;
		
		void CreateViewModel()
		{
			CreateFakeSolution();
			CreateViewModel(fakeSolution);
		}
		
		void CreateViewModel(FakePackageManagementSolution fakeSolution)
		{
			viewModel = new TestableUpdatedPackageViewModel(fakeSolution);
			fakeProject = fakeSolution.FakeProjectToReturnFromGetProject;
			fakeActionRunner = viewModel.FakeActionRunner;
			fakePackageManagementEvents = viewModel.FakePackageManagementEvents;
		}
		
		FakeUpdatePackageAction FirstUpdatePackageActionCreated {
			get { return fakeProject.FirstFakeUpdatePackageActionCreated; }
		}

		
		void CreateFakeSolution()
		{
			fakeSolution = new FakePackageManagementSolution();
			fakeSolution.FakeActiveMSBuildProject = ProjectHelper.CreateTestProject("MyProject");
		}
		
		void AddProjectToSolution()
		{
			TestableProject project = ProjectHelper.CreateTestProject();
			fakeSolution.FakeMSBuildProjects.Add(project);
		}
		
		void CreateViewModelWithTwoProjectsSelected(string projectName1, string projectName2)
		{
			CreateFakeSolution();
			AddProjectToSolution();
			AddProjectToSolution();
			fakeSolution.FakeMSBuildProjects[0].Name = projectName1;
			fakeSolution.FakeMSBuildProjects[1].Name = projectName2;
			fakeSolution.NoProjectsSelected();
			
			fakeSolution.AddFakeProjectToReturnFromGetProject(projectName1);
			fakeSolution.AddFakeProjectToReturnFromGetProject(projectName2);
			
			CreateViewModel(fakeSolution);
		}
		
		[Test]
		public void AddPackage_PackageAddedSuccessfully_ProjectCreatedUsingSourcePackageRepository()
		{
			CreateViewModel();
			viewModel.AddPackage();
						
			Assert.AreEqual(viewModel.FakePackage.Repository, fakeSolution.RepositoryPassedToGetProject);
		}
	
		[Test]
		public void AddPackage_PackageAddedSuccessfully_PackageUpdated()
		{
			CreateViewModel();
			viewModel.AddPackage();
			
			FakePackage expectedPackage = viewModel.FakePackage;
			IPackage actualPackage = FirstUpdatePackageActionCreated.Package;
						
			Assert.AreEqual(expectedPackage, actualPackage);
		}
		
		[Test]
		public void AddPackage_PackageAddedSuccessfully_PackageUpdatedUsingPackageOperations()
		{
			CreateViewModel();
			viewModel.AddPackage();
			
			IEnumerable<PackageOperation> expectedOperations = viewModel.FakePackageOperationResolver.PackageOperations;
			IEnumerable<PackageOperation> actualOperations = FirstUpdatePackageActionCreated.Operations;
						
			Assert.AreEqual(expectedOperations, actualOperations);
		}
		
		[Test]
		public void AddPackage_PackageAddedSuccessfully_PackageIsUpdated()
		{
			CreateViewModel();
			viewModel.AddPackage();
			
			ProcessPackageAction actionExecuted = fakeActionRunner.ActionPassedToRun;

			Assert.AreEqual(FirstUpdatePackageActionCreated, actionExecuted);
		}
		
		[Test]
		public void ManagePackage_OneProjectSelectedByUser_PackageIsUpdated()
		{
			CreateViewModelWithTwoProjectsSelected("Project A", "Project B");
			fakePackageManagementEvents.ProjectsToSelect.Add("Project B");
			fakePackageManagementEvents.OnSelectProjectsReturnValue = true;
			viewModel.ManagePackage();
			
			FakePackage expectedPackage = viewModel.FakePackage;
			List<ProcessPackageAction> actions = fakeActionRunner.GetActionsRunInOneCallAsList();
			var updatePackageAction = actions[0] as UpdatePackageAction;
			IPackage actualPackage = updatePackageAction.Package;
						
			Assert.AreEqual(expectedPackage, actualPackage);
		}
	}
}
