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

		[Test]
		public void PackageViewModels_PackageIsInstalledAfterRecentPackagesDisplayed_PackagesOnDisplayAreUpdated()
		{
			CreateViewModel();
			viewModel.ReadPackages();
			CompleteReadPackagesTask();
			var package = new FakePackage("Test");
			FakePackageRepository repository = packageManagementService.FakeRecentPackageRepository;
			repository.FakePackages.Add(package);
			
			ClearReadPackagesTasks();
			packageManagementService.FirePackageInstalled();
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
			var package = new FakePackage("Test");
			FakePackageRepository repository = packageManagementService.FakeRecentPackageRepository;
			repository.FakePackages.Add(package);
			
			ClearReadPackagesTasks();
			packageManagementService.FirePackageUninstalled();
			CompleteReadPackagesTask();
			
			var expectedPackages = new FakePackage[] {
				package
			};
		
			PackageCollectionAssert.AreEqual(expectedPackages, viewModel.PackageViewModels);
		}
	}
}
