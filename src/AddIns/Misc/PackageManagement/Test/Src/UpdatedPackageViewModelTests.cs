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
using ICSharpCode.PackageManagement;
using ICSharpCode.PackageManagement.Design;
using ICSharpCode.PackageManagement.Scripting;
using NuGet;
using NUnit.Framework;
using PackageManagement.Tests.Helpers;
using Rhino.Mocks;

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
		IPackageViewModelParent viewModelParent;
		
		void CreateViewModel()
		{
			CreateFakeSolution();
			CreateViewModel(fakeSolution);
		}
		
		void CreateViewModel(FakePackageManagementSolution fakeSolution)
		{
			viewModelParent = MockRepository.GenerateStub<IPackageViewModelParent>();
			viewModel = new TestableUpdatedPackageViewModel(viewModelParent, fakeSolution);
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
			
			IPackageAction actionExecuted = fakeActionRunner.ActionPassedToRun;

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
			List<IPackageAction> actions = fakeActionRunner.GetActionsRunInOneCallAsList();
			var updatePackageAction = actions[0] as UpdatePackageAction;
			IPackage actualPackage = updatePackageAction.Package;
			
			Assert.AreEqual(expectedPackage, actualPackage);
		}
		
		[Test]
		public void AddPackage_PackageRepositoryIsOperationAwareAndPackageAddedSuccessfully_UpdateOperationStartedForPackage()
		{
			CreateViewModel();
			var operationAwareRepository = new FakeOperationAwarePackageRepository();
			FakePackage fakePackage = viewModel.FakePackage;
			fakePackage.FakePackageRepository = operationAwareRepository;
			fakePackage.Id = "MyPackage";
			
			viewModel.AddPackage();
			
			operationAwareRepository.AssertOperationWasStartedAndDisposed(RepositoryOperationNames.Update, "MyPackage");
		}
	}
}
