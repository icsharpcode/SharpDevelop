// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using ICSharpCode.PackageManagement;
using ICSharpCode.PackageManagement.Design;
using NuGet;
using NUnit.Framework;
using PackageManagement.Tests.Helpers;

namespace PackageManagement.Tests
{
	[TestFixture]
	public class AddPackageReferenceViewModelTests
	{
		AddPackageReferenceViewModel viewModel;
		PackageManagementEvents packageManagementEvents;
		FakeLicenseAcceptanceService fakeLicenseAcceptanceSevice;
		FakePackageManagementSolution fakeSolution;
		FakeRegisteredPackageRepositories fakeRegisteredPackageRepositories;
		FakeTaskFactory taskFactory;
		List<FakePackage> packagesPassedToOnAcceptLicenses;
		
		void CreateSolution()
		{
			fakeSolution = new FakePackageManagementSolution();
			fakeRegisteredPackageRepositories = new FakeRegisteredPackageRepositories();
		}
		
		void CreateViewModel()
		{
			CreateSolution();
			CreateViewModel(fakeSolution);
		}
		
		void CreateViewModel(FakePackageManagementSolution solution)
		{
			taskFactory = new FakeTaskFactory();
			packageManagementEvents = new PackageManagementEvents();
			fakeLicenseAcceptanceSevice = new FakeLicenseAcceptanceService();
			viewModel = new AddPackageReferenceViewModel(
				solution,
				fakeRegisteredPackageRepositories,
				packageManagementEvents,
				fakeLicenseAcceptanceSevice,
				taskFactory);
			taskFactory.ExecuteAllFakeTasks();
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
			var propertyNamesChanged = RecordViewModelPropertiesChanged();
			RaisePackageOperationErrorEvent();
			return propertyNamesChanged;
		}
		
		void RaisePackageOperationsStartingEvent()
		{
			packageManagementEvents.OnPackageOperationsStarting();
		}
		
		List<string> RaisePackageOperationsStartingEventAndRecordPropertiesChanged()
		{
			var propertyNamesChanged = RecordViewModelPropertiesChanged();
			RaisePackageOperationsStartingEvent();
			return propertyNamesChanged;
		}
		
		bool RaiseAcceptLicensesEvent()
		{
			packagesPassedToOnAcceptLicenses = new List<FakePackage>();
			return packageManagementEvents.OnAcceptLicenses(packagesPassedToOnAcceptLicenses);
		}
		
		[Test]
		public void InstalledPackagesViewModel_ProjectHasOneInstalledPackage_HasOnePackageViewModel()
		{
			CreateSolution();
			var projectManager = new FakeProjectManager();
			fakeSolution.FakeActiveProjectManager = projectManager;
			FakePackage package = new FakePackage();
			projectManager.FakeLocalRepository.FakePackages.Add(package);
			CreateViewModel(fakeSolution);
			
			IEnumerable<IPackage> expectedPackages = projectManager.FakeLocalRepository.FakePackages;
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
				Version = new Version("1.0.0.0")
			};
			fakeSolution.AddPackageToProjectLocalRepository(oldPackage);
			
			var newPackage = new FakePackage() {
				Id = "Test",
				Version = new Version("2.0.0.0")
			};
			fakeRegisteredPackageRepositories.FakeAggregateRepository.FakePackages.Add(newPackage);
			
			CreateViewModel(fakeSolution);
			
			List<FakePackage> expectedPackages = fakeRegisteredPackageRepositories.FakeAggregateRepository.FakePackages;
			
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
			var ex = RaisePackageOperationErrorEvent();
			
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
		public void Dispose_ContainedViewModelsAreDisposed_InstaledPackagesViewModelIsDisposed()
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
			
			var actualPackages = fakeLicenseAcceptanceSevice.PackagesPassedToAcceptLicenses;
			var expectedPackages = packagesPassedToOnAcceptLicenses;
			
			Assert.AreEqual(expectedPackages, actualPackages);
		}
		
		[Test]
		public void AcceptLicenses_EventFiredAndUserAcceptsLicenses_AcceptLicensesReturnsTrue()
		{
			CreateViewModel();
			fakeLicenseAcceptanceSevice.AcceptLicensesReturnValue = true;
			bool result = RaiseAcceptLicensesEvent();
			
			Assert.IsTrue(result);
		}
		
		[Test]
		public void AcceptLicenses_EventFiredAndUserDoesNotAcceptLicenses_AcceptLicensesReturnsFalse()
		{
			CreateViewModel();
			fakeLicenseAcceptanceSevice.AcceptLicensesReturnValue = false;
			bool result = RaiseAcceptLicensesEvent();
			
			Assert.IsFalse(result);
		}
		
		[Test]
		public void Dispose_AcceptLicensesEventFired_UserIsNotPromptedToAcceptLicenses()
		{
			CreateViewModel();
			viewModel.Dispose();
			RaiseAcceptLicensesEvent();
			
			Assert.IsFalse(fakeLicenseAcceptanceSevice.IsAcceptLicensesCalled);
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
	}
}
