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
using System.Collections.ObjectModel;
using ICSharpCode.PackageManagement;
using ICSharpCode.PackageManagement.Design;
using NuGet;
using NUnit.Framework;
using PackageManagement.Tests.Helpers;
using Rhino.Mocks;

namespace PackageManagement.Tests
{
	[TestFixture]
	public class ManagePackagesViewModelTests
	{
		ManagePackagesViewModel viewModel;
		PackageManagementEvents packageManagementEvents;
		FakeLicenseAcceptanceService fakeLicenseAcceptanceService;
		FakeSelectProjectsService fakeSelectProjectsService;
		IFileConflictResolver fakeFileConflictResolver;
		FakePackageManagementSolution fakeSolution;
		FakeRegisteredPackageRepositories fakeRegisteredPackageRepositories;
		FakeTaskFactory fakeTaskFactory;
		List<FakePackage> packagesPassedToOnAcceptLicenses;
		FakePackageActionRunner fakeActionRunner;
		FakePackageManagementEvents fakeThreadSafeEvents;
		List<IPackageManagementSelectedProject> projectsPassedToOnSelectProjects;
		ManagePackagesUserPrompts userPrompts;
		PackagesViewModels packagesViewModels;
		ManagePackagesViewTitle viewTitle;
		
		void CreateSolution()
		{
			fakeSolution = new FakePackageManagementSolution();
			fakeRegisteredPackageRepositories = new FakeRegisteredPackageRepositories();
			fakeSolution.FakeActiveMSBuildProject = ProjectHelper.CreateTestProject();
		}
		
		void CreateViewModel()
		{
			CreateSolution();
			CreateViewModel(fakeSolution);
		}
		
		void CreateViewModel(FakePackageManagementSolution solution)
		{
			packageManagementEvents = new PackageManagementEvents();
			var threadSafeEvents = new ThreadSafePackageManagementEvents(packageManagementEvents, new FakePackageManagementWorkbench());
			CreateViewModel(fakeSolution, threadSafeEvents);
		}
		
		void CreateViewModel(
			FakePackageManagementSolution solution, 
			IThreadSafePackageManagementEvents packageManagementEvents)
		{
			fakeTaskFactory = new FakeTaskFactory();
			fakeLicenseAcceptanceService = new FakeLicenseAcceptanceService();
			fakeSelectProjectsService = new FakeSelectProjectsService();
			fakeFileConflictResolver = MockRepository.GenerateStub<IFileConflictResolver>();
			userPrompts = new ManagePackagesUserPrompts(
				packageManagementEvents,
				fakeLicenseAcceptanceService,
				fakeSelectProjectsService,
				fakeFileConflictResolver);
			fakeActionRunner = new FakePackageActionRunner();
			
			packagesViewModels = new PackagesViewModels(
				solution,
				fakeRegisteredPackageRepositories,
				packageManagementEvents,
				fakeActionRunner,
				fakeTaskFactory);
			
			viewTitle = new ManagePackagesViewTitle(solution);
			
			viewModel = new ManagePackagesViewModel(
				packagesViewModels,
				viewTitle,
				packageManagementEvents,
				userPrompts);
			fakeTaskFactory.ExecuteAllFakeTasks();
		}
		
		void CreateViewModelWithFakeThreadSafePackageManagementEvents()
		{
			CreateSolution();
			fakeThreadSafeEvents = new FakePackageManagementEvents();
			CreateViewModel(fakeSolution, fakeThreadSafeEvents);
		}
		
		List<string> RecordViewModelPropertiesChanged()
		{
			var propertyNamesChanged = new List<string>();
			viewModel.PropertyChanged += (sender, e) => propertyNamesChanged.Add(e.PropertyName);
			return propertyNamesChanged;
		}
		
		Exception RaisePackageOperationErrorEvent()
		{
			var ex = new Exception("Test");
			packageManagementEvents.OnPackageOperationError(ex);
			return ex;
		}
		
		List<string> RaisePackageOperationErrorEventAndRecordPropertiesChanged()
		{
			List<string> propertyNamesChanged = RecordViewModelPropertiesChanged();
			RaisePackageOperationErrorEvent();
			return propertyNamesChanged;
		}
		
		void RaisePackageOperationsStartingEvent()
		{
			packageManagementEvents.OnPackageOperationsStarting();
		}
		
		List<string> RaisePackageOperationsStartingEventAndRecordPropertiesChanged()
		{
			List<string> propertyNamesChanged = RecordViewModelPropertiesChanged();
			RaisePackageOperationsStartingEvent();
			return propertyNamesChanged;
		}
		
		bool RaiseAcceptLicensesEvent()
		{
			packagesPassedToOnAcceptLicenses = new List<FakePackage>();
			return packageManagementEvents.OnAcceptLicenses(packagesPassedToOnAcceptLicenses);
		}
		
		bool RaiseSelectProjectsEvent()
		{
			projectsPassedToOnSelectProjects = new List<IPackageManagementSelectedProject>();
			return packageManagementEvents.OnSelectProjects(projectsPassedToOnSelectProjects);
		}
		
		[Test]
		public void InstalledPackagesViewModel_ProjectHasOneInstalledPackage_HasOnePackageViewModel()
		{
			CreateSolution();
			FakePackage package = new FakePackage();
			fakeSolution.FakeActiveProject.FakePackages.Add(package);
			CreateViewModel(fakeSolution);
			
			IEnumerable<IPackage> expectedPackages = fakeSolution.FakeActiveProject.FakePackages;
			IEnumerable<PackageViewModel> actualPackageViewModels = viewModel.InstalledPackagesViewModel.PackageViewModels;
			
			PackageCollectionAssert.AreEqual(expectedPackages, actualPackageViewModels);
		}
		
		[Test]
		public void AvailablePackagesViewModel_ActiveRepositoryHasOnePackage_HasOnePackageViewModel()
		{
			CreateSolution();
			var package = new FakePackage();
			package.Id = "Test";
			fakeRegisteredPackageRepositories.FakeActiveRepository.FakePackages.Add(package);
			CreateViewModel(fakeSolution);

			List<FakePackage> expectedPackages = fakeRegisteredPackageRepositories.FakeActiveRepository.FakePackages;
			
			PackageCollectionAssert.AreEqual(expectedPackages, viewModel.AvailablePackagesViewModel.PackageViewModels);
		}
		
		[Test]
		public void PackageUpdatesViewModel_OneUpdatedPackageVersion_HasOnePackageViewModel()
		{
			CreateSolution();
			
			var oldPackage = new FakePackage() {
				Id = "Test",
				Version = new SemanticVersion("1.0.0.0")
			};
			fakeSolution.FakeProjectToReturnFromGetProject.FakePackages.Add(oldPackage);
			
			var newPackage = new FakePackage() {
				Id = "Test",
				Version = new SemanticVersion("2.0.0.0")
			};
			fakeRegisteredPackageRepositories.FakeActiveRepository.FakePackages.Add(newPackage);
			
			CreateViewModel(fakeSolution);
			
			List<FakePackage> expectedPackages = fakeRegisteredPackageRepositories.FakeActiveRepository.FakePackages;
			
			PackageCollectionAssert.AreEqual(expectedPackages, viewModel.UpdatedPackagesViewModel.PackageViewModels);
		}
		
		[Test]
		public void RecentPackagesViewModel_RecentRepositoryHasOnePackage_HasOnePackageViewModel()
		{
			CreateSolution();
			var package = new FakePackage();
			package.Id = "Test";
			fakeRegisteredPackageRepositories.FakeRecentPackageRepository.FakePackages.Add(package);
			CreateViewModel(fakeSolution);

			List<FakePackage> expectedPackages = fakeRegisteredPackageRepositories.FakeRecentPackageRepository.FakePackages;
			
			PackageCollectionAssert.AreEqual(expectedPackages, viewModel.RecentPackagesViewModel.PackageViewModels);
		}
		
		[Test]
		public void Message_PackageManagementErrorEventFires_ExceptionMessageUpdatesViewModelMessage()
		{
			CreateViewModel();
			Exception ex = RaisePackageOperationErrorEvent();
			
			Assert.AreEqual("Test", viewModel.Message);
		}
		
		[Test]
		public void Message_PackageManagementErrorEventFires_MessagePropertyIsChanged()
		{
			CreateViewModel();
			List<string> propertyNamesChanged = RaisePackageOperationErrorEventAndRecordPropertiesChanged();
			
			bool result = propertyNamesChanged.Contains("Message");
			
			Assert.IsTrue(result);
		}
		
		[Test]
		public void Message_PackageManagementErrorEventFires_HasErrorIsTrue()
		{
			CreateViewModel();
			RaisePackageOperationErrorEvent();
			
			Assert.IsTrue(viewModel.HasError);
		}
		
		[Test]
		public void Message_PackageManagementErrorEventFires_HasErrorPropertyIsChanged()
		{
			CreateViewModel();
			List<string> propertyNamesChanged = RaisePackageOperationErrorEventAndRecordPropertiesChanged();
			
			bool result = propertyNamesChanged.Contains("HasError");
			
			Assert.IsTrue(result);
		}
		
		[Test]
		public void Message_ErrorMessageCurrentlyDisplayedWhenPackageOperationsStartingEventFired_MessageIsCleared()
		{
			CreateViewModel();
			viewModel.Message = "test";
			packageManagementEvents.OnPackageOperationsStarting();
			
			Assert.IsNull(viewModel.Message);
		}
		
		[Test]
		public void Message_ErrorMessageCurrentlyDisplayedWhenOnPackageOperationsStartingEventFired_MessagePropertyIsChanged()
		{
			CreateViewModel();
			List<string> propertyNamesChanged = RaisePackageOperationsStartingEventAndRecordPropertiesChanged();
			
			bool result = propertyNamesChanged.Contains("Message");
			
			Assert.IsTrue(result);
		}
		
		[Test]
		public void HasError_ErrorMessageCurrentlyDisplayedWhenOnPackageOperationsStartingEventFired_HasErrorPropertyIsChanged()
		{
			CreateViewModel();
			List<string> propertyNamesChanged = RaisePackageOperationsStartingEventAndRecordPropertiesChanged();
			
			bool result = propertyNamesChanged.Contains("HasError");
			
			Assert.IsTrue(result);
		}
		
		[Test]
		public void HasError_ErrorMessageCurrentlyDisplayedWhenOnPackageOperationsStartingEventFired_ReturnsFalse()
		{
			CreateViewModel();
			viewModel.HasError = true;
			RaisePackageOperationsStartingEvent();
			
			Assert.IsFalse(viewModel.HasError);
		}
		
		[Test]
		public void Dispose_ContainedViewModelsAreDisposed_AvailablePackagesViewModelIsDisposed()
		{
			CreateViewModel();
			viewModel.Dispose();
			
			bool disposed = viewModel.AvailablePackagesViewModel.IsDisposed;
			
			Assert.IsTrue(disposed);
		}
		
		[Test]
		public void Dispose_ContainedViewModelsAreDisposed_InstalledPackagesViewModelIsDisposed()
		{
			CreateViewModel();
			viewModel.Dispose();
			
			bool disposed = viewModel.InstalledPackagesViewModel.IsDisposed;
			
			Assert.IsTrue(disposed);
		}
		
		[Test]
		public void Dispose_ContainedViewModelsAreDisposed_UpdatedPackagesViewModelIsDisposed()
		{
			CreateViewModel();
			viewModel.Dispose();
			
			bool disposed = viewModel.UpdatedPackagesViewModel.IsDisposed;
			
			Assert.IsTrue(disposed);
		}
		
		[Test]
		public void Dispose_ContainedViewModelsAreDisposed_RecentPackagesViewModelIsDisposed()
		{
			CreateViewModel();
			viewModel.Dispose();
			
			bool disposed = viewModel.RecentPackagesViewModel.IsDisposed;
			
			Assert.IsTrue(disposed);
		}
		
		[Test]
		public void AcceptLicenses_EventFired_PackagesPassedToUserToAcceptLicenses()
		{
			CreateViewModel();
			RaiseAcceptLicensesEvent();
			
			IEnumerable<IPackage> actualPackages = fakeLicenseAcceptanceService.PackagesPassedToAcceptLicenses;
			IEnumerable<IPackage> expectedPackages = packagesPassedToOnAcceptLicenses;
			
			Assert.AreEqual(expectedPackages, actualPackages);
		}
		
		[Test]
		public void AcceptLicenses_EventFiredAndUserAcceptsLicenses_AcceptLicensesReturnsTrue()
		{
			CreateViewModel();
			fakeLicenseAcceptanceService.AcceptLicensesReturnValue = true;
			bool result = RaiseAcceptLicensesEvent();
			
			Assert.IsTrue(result);
		}
		
		[Test]
		public void AcceptLicenses_EventFiredAndUserDoesNotAcceptLicenses_AcceptLicensesReturnsFalse()
		{
			CreateViewModel();
			fakeLicenseAcceptanceService.AcceptLicensesReturnValue = false;
			bool result = RaiseAcceptLicensesEvent();
			
			Assert.IsFalse(result);
		}
		
		[Test]
		public void Dispose_AcceptLicensesEventFired_UserIsNotPromptedToAcceptLicenses()
		{
			CreateViewModel();
			viewModel.Dispose();
			RaiseAcceptLicensesEvent();
			
			Assert.IsFalse(fakeLicenseAcceptanceService.IsAcceptLicensesCalled);
		}
		
		[Test]
		public void Dispose_PackageOperationErrorEventFires_ViewModelMessageIsNotUpdated()
		{
			CreateViewModel();
			viewModel.Dispose();
			RaisePackageOperationErrorEvent();
			
			Assert.IsNull(viewModel.Message);
		}
		
		[Test]
		public void Dispose_PackageOperationsStartingEventFires_ViewModelMessageIsNotUpdated()
		{
			CreateViewModel();
			viewModel.Message = "Test";
			viewModel.Dispose();
			RaisePackageOperationsStartingEvent();
			
			Assert.AreEqual("Test", viewModel.Message);
		}
		
		[Test]
		public void Dispose_MethodCalled_DisposesThreadSafePackageManagementEvents()
		{
			CreateViewModelWithFakeThreadSafePackageManagementEvents();
			viewModel.Dispose();
			
			bool disposed = fakeThreadSafeEvents.IsDisposed;
			
			Assert.IsTrue(disposed);
		}
		
		[Test]
		public void Title_ProjectSelected_ReturnsProjectInTitle()
		{
			CreateSolution();
			TestableProject project = ProjectHelper.CreateTestProject("Test");
			fakeSolution.FakeActiveMSBuildProject = project;
			fakeSolution.FakeActiveProject.Name = "Test";
			CreateViewModel(fakeSolution);
			
			string title = viewModel.Title;
			
			string expectedTitle = "Test - Manage Packages";
			
			Assert.AreEqual(expectedTitle, title);
		}
		
		[Test]
		public void Title_SolutionSelectedButNoProjectSelected_ReturnsSolutionFileNameInTitle()
		{
			CreateSolution();
			fakeSolution.NoProjectsSelected();
			fakeSolution.FileName = @"d:\projects\MySolution.sln";
			CreateViewModel(fakeSolution);
			
			string title = viewModel.Title;
			
			string expectedTitle = "MySolution.sln - Manage Packages";
			
			Assert.AreEqual(expectedTitle, title);
		}
		
		[Test]
		public void SelectProjects_EventFired_ProjectsPassedToUserToSelect()
		{
			CreateViewModel();
			RaiseSelectProjectsEvent();
			
			IEnumerable<IPackageManagementSelectedProject> projects = fakeSelectProjectsService.ProjectsPassedToSelectProjects;
			List<IPackageManagementSelectedProject> expectedProjects = projectsPassedToOnSelectProjects;
			
			Assert.AreEqual(expectedProjects, projects);
		}
		
		[Test]
		public void SelectProjects_EventFiredAndUserAcceptsSelectedProjects_SelectProjectsReturnsTrue()
		{
			CreateViewModel();
			fakeSelectProjectsService.SelectProjectsReturnValue = true;
			bool result = RaiseSelectProjectsEvent();
			
			Assert.IsTrue(result);
		}
		
		[Test]
		public void SelectProjects_EventFiredAndUserDoesNotAcceptLicenses_SelectProjectsReturnsFalse()
		{
			CreateViewModel();
			fakeSelectProjectsService.SelectProjectsReturnValue = false;
			bool result = RaiseSelectProjectsEvent();
			
			Assert.IsFalse(result);
		}
		
		[Test]
		public void Dispose_SelectProjectsEventFired_UserIsNotPromptedToSelectProjects()
		{
			CreateViewModel();
			viewModel.Dispose();
			RaiseSelectProjectsEvent();
			
			Assert.IsFalse(fakeSelectProjectsService.IsSelectProjectsCalled);
		}
	}
}
