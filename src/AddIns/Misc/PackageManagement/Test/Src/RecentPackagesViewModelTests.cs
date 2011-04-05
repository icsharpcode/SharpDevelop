// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.PackageManagement;
using ICSharpCode.PackageManagement.Design;
using NUnit.Framework;
using PackageManagement.Tests.Helpers;

namespace PackageManagement.Tests
{
	[TestFixture]
	public class RecentPackagesViewModelTests
	{
		RecentPackagesViewModel viewModel;
		FakePackageManagementService packageManagementService;
		FakeTaskFactory taskFactory;
		
		void CreateViewModel()
		{
			CreatePackageManagementService();
			CreateViewModel(packageManagementService);
		}
		
		void CreatePackageManagementService()
		{
			packageManagementService = new FakePackageManagementService();
		}
		
		void CreateViewModel(FakePackageManagementService packageManagementService)
		{
			taskFactory = new FakeTaskFactory();
			var messageReporter = new FakeMessageReporter();
			viewModel = new RecentPackagesViewModel(packageManagementService, messageReporter, taskFactory);
		}
		
		void CompleteReadPackagesTask()
		{
			taskFactory.ExecuteAllFakeTasks();
		}
		
		void ClearReadPackagesTasks()
		{
			taskFactory.ClearAllFakeTasks();
		}
		
		FakePackage AddPackageToRecentPackageRepository()
		{
			var package = new FakePackage("Test");
			FakePackageRepository repository = packageManagementService.FakeRecentPackageRepository;
			repository.FakePackages.Add(package);
			return package;
		}

		[Test]
		public void PackageViewModels_PackageIsInstalledAfterRecentPackagesDisplayed_PackagesOnDisplayAreUpdated()
		{
			CreateViewModel();
			viewModel.ReadPackages();
			CompleteReadPackagesTask();
			var package = AddPackageToRecentPackageRepository();
			
			ClearReadPackagesTasks();
			packageManagementService.FireParentPackageInstalled();
			CompleteReadPackagesTask();
			
			var expectedPackages = new FakePackage[] {
				package
			};
			
			PackageCollectionAssert.AreEqual(expectedPackages, viewModel.PackageViewModels);
		}
		
		[Test]
		public void PackageViewModels_PackageIsUninstalledAfterRecentPackagesDisplayed_PackagesOnDisplayAreUpdated()
		{
			CreateViewModel();
			viewModel.ReadPackages();
			CompleteReadPackagesTask();
			var package = AddPackageToRecentPackageRepository();
			
			ClearReadPackagesTasks();
			packageManagementService.FireParentPackageUninstalled();
			CompleteReadPackagesTask();
			
			var expectedPackages = new FakePackage[] {
				package
			};
			
			PackageCollectionAssert.AreEqual(expectedPackages, viewModel.PackageViewModels);
		}
		
		[Test]
		public void PackageViewModels_PackageIsUninstalledAfterViewModelIsDisposed_PackagesOnDisplayAreNotUpdated()
		{
			CreateViewModel();
			viewModel.ReadPackages();
			CompleteReadPackagesTask();
			AddPackageToRecentPackageRepository();
			
			ClearReadPackagesTasks();
			viewModel.Dispose();
			
			packageManagementService.FireParentPackageUninstalled();
			CompleteReadPackagesTask();
			
			Assert.AreEqual(0, viewModel.PackageViewModels.Count);
		}
		
		[Test]
		public void PackageViewModels_PackageIsInstalledAfterViewModelIsDisposed_PackagesOnDisplayAreNotUpdated()
		{
			CreateViewModel();
			viewModel.ReadPackages();
			CompleteReadPackagesTask();
			AddPackageToRecentPackageRepository();
			
			ClearReadPackagesTasks();
			
			viewModel.Dispose();
			packageManagementService.FireParentPackageInstalled();
			CompleteReadPackagesTask();
			
			Assert.AreEqual(0, viewModel.PackageViewModels.Count);
		}
	}
}
