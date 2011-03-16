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
		FakePackageManagementService fakePackageManagementService;
		FakeTaskFactory taskFactory;
		
		void CreatePackageManagementService()
		{
			fakePackageManagementService = new FakePackageManagementService();
		}
		
		void CreateViewModel()
		{
			CreatePackageManagementService();
			CreateViewModel(fakePackageManagementService);
		}
		
		void CreateViewModel(FakePackageManagementService packageManagementService)
		{
			taskFactory = new FakeTaskFactory();
			viewModel = new AddPackageReferenceViewModel(packageManagementService, taskFactory);
			taskFactory.ExecuteAllFakeTasks();
		}
		
		List<string> CallShowErrorMessageAndRecordPropertiesChanged(string message)
		{
			var propertyNamesChanged = RecordViewModelPropertiesChanged();
			viewModel.ShowErrorMessage(message);
			return propertyNamesChanged;
		}
		
		List<string> RecordViewModelPropertiesChanged()
		{
			var propertyNamesChanged = new List<string>();
			viewModel.PropertyChanged += (sender, e) => propertyNamesChanged.Add(e.PropertyName);
			return propertyNamesChanged;
		}

		List<string> CallClearMessageAndRecordPropertiesChanged()
		{
			var propertyNamesChanged = RecordViewModelPropertiesChanged();
			viewModel.ClearMessage();
			return propertyNamesChanged;
		}		
		
		[Test]
		public void InstalledPackagesViewModel_ProjectHasOneInstalledPackage_HasOnePackageViewModel()
		{
			CreatePackageManagementService();
			var projectManager = new FakeProjectManager();
			fakePackageManagementService.FakeActiveProjectManager = projectManager;
			FakePackage package = new FakePackage();
			projectManager.FakeLocalRepository.FakePackages.Add(package);
			CreateViewModel(fakePackageManagementService);
			
			IEnumerable<IPackage> expectedPackages = projectManager.FakeLocalRepository.FakePackages;
			IEnumerable<PackageViewModel> actualPackageViewModels = viewModel.InstalledPackagesViewModel.PackageViewModels;
			
			PackageCollectionAssert.AreEqual(expectedPackages, actualPackageViewModels);
		}
		
		[Test]
		public void AvailablePackagesViewModel_ActiveRepositoryHasOnePackage_HasOnePackageViewModel()
		{
			CreatePackageManagementService();
			var package = new FakePackage();
			package.Id = "Test";
			fakePackageManagementService.FakeActivePackageRepository.FakePackages.Add(package);
			CreateViewModel(fakePackageManagementService);

			List<FakePackage> expectedPackages = fakePackageManagementService.FakeActivePackageRepository.FakePackages;
			
			PackageCollectionAssert.AreEqual(expectedPackages, viewModel.AvailablePackagesViewModel.PackageViewModels);
		}
		
		[Test]
		public void PackageUpdatesViewModel_OneUpdatedPackageVersion_HasOnePackageViewModel()
		{
			CreatePackageManagementService();
			
			var oldPackage = new FakePackage() {
				Id = "Test",
				Version = new Version("1.0.0.0")
			};
			fakePackageManagementService.AddPackageToProjectLocalRepository(oldPackage);
			
			var newPackage = new FakePackage() {
				Id = "Test",
				Version = new Version("2.0.0.0")
			};
			fakePackageManagementService.FakeAggregateRepository.FakePackages.Add(newPackage);
			
			CreateViewModel(fakePackageManagementService);
			
			List<FakePackage> expectedPackages = fakePackageManagementService.FakeAggregateRepository.FakePackages;
			
			PackageCollectionAssert.AreEqual(expectedPackages, viewModel.PackageUpdatesViewModel.PackageViewModels);
		}
		
		[Test]
		public void Constructor_InstanceCreated_OutputMessagesCleared()
		{
			CreateViewModel();
			
			Assert.IsTrue(fakePackageManagementService.FakeOutputMessagesView.IsClearCalled);
		}
		
		[Test]
		public void RecentPackagesViewModel_RecentRepositoryHasOnePackage_HasOnePackageViewModel()
		{
			CreatePackageManagementService();
			var package = new FakePackage();
			package.Id = "Test";
			fakePackageManagementService.FakeRecentPackageRepository.FakePackages.Add(package);
			CreateViewModel(fakePackageManagementService);

			List<FakePackage> expectedPackages = fakePackageManagementService.FakeRecentPackageRepository.FakePackages;
			
			PackageCollectionAssert.AreEqual(expectedPackages, viewModel.RecentPackagesViewModel.PackageViewModels);
		}
		
		[Test]
		public void ShowErrorMessage_ErrorMessageToBeDisplayedToUser_MessageIsSet()
		{
			CreateViewModel();
			viewModel.ShowErrorMessage("Test");
			
			Assert.AreEqual("Test", viewModel.Message);
		}
		
		[Test]
		public void ShowErrorMessage_ErrorMessageToBeDisplayedToUser_MessagePropertyIsChanged()
		{
			CreateViewModel();
			List<string> propertyNamesChanged = CallShowErrorMessageAndRecordPropertiesChanged("Test");
			
			bool result = propertyNamesChanged.Contains("Message");
			
			Assert.IsTrue(result);
		}
		
		[Test]
		public void ShowErrorMessage_ErrorMessageToBeDisplayedToUser_HasErrorIsTrue()
		{
			CreateViewModel();
			viewModel.ShowErrorMessage("Test");
			
			Assert.IsTrue(viewModel.HasError);
		}
		
		[Test]
		public void ShowErrorMessage_ErrorMessageToBeDisplayedToUser_HasErrorPropertyIsChanged()
		{
			CreateViewModel();
			List<string> propertyNamesChanged = CallShowErrorMessageAndRecordPropertiesChanged("Test");
			
			bool result = propertyNamesChanged.Contains("HasError");
			
			Assert.IsTrue(result);
		}
		
		[Test]
		public void ClearMessage_ErrorMessageCurrentlyDisplayed_MessageIsCleared()
		{
			CreateViewModel();
			viewModel.Message = "test";
			viewModel.ClearMessage();
			
			Assert.IsNull(viewModel.Message);
		}
		
		[Test]
		public void ClearMessage_ErrorMessageCurrentlyDisplayed_MessagePropertyIsChanged()
		{
			CreateViewModel();
			List<string> propertyNamesChanged = CallClearMessageAndRecordPropertiesChanged();
			
			bool result = propertyNamesChanged.Contains("Message");
			
			Assert.IsTrue(result);
		}
		
		[Test]
		public void ClearMessages_ErrorMessageCurrentlyDisplayed_HasErrorPropertyIsChanged()
		{
			CreateViewModel();
			List<string> propertyNamesChanged = CallClearMessageAndRecordPropertiesChanged();
			
			bool result = propertyNamesChanged.Contains("HasError");
			
			Assert.IsTrue(result);
		}
		
		[Test]
		public void ClearMessage_ErrorMessageCurrentlyDisplayed_HasErrorIsFalse()
		{
			CreateViewModel();
			viewModel.HasError = true;
			viewModel.ClearMessage();
			
			Assert.IsFalse(viewModel.HasError);
		}
	}
}
