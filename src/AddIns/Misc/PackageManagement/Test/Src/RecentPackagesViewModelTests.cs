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
		PackageManagementEvents packageManagementEvents;
		FakeRegisteredPackageRepositories registeredPackageRepositories;
		FakeTaskFactory taskFactory;
		
		void CreateViewModel()
		{
			registeredPackageRepositories = new FakeRegisteredPackageRepositories();
			taskFactory = new FakeTaskFactory();
			var packageViewModelFactory = new FakePackageViewModelFactory();
			packageManagementEvents = new PackageManagementEvents();
			viewModel = new RecentPackagesViewModel(
				packageManagementEvents,
				registeredPackageRepositories,
				packageViewModelFactory,
				taskFactory);
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
			FakePackageRepository repository = registeredPackageRepositories.FakeRecentPackageRepository;
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
			packageManagementEvents.OnParentPackageInstalled(new FakePackage());
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
			packageManagementEvents.OnParentPackageUninstalled(new FakePackage());
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
			
			packageManagementEvents.OnParentPackageUninstalled(new FakePackage());
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
			packageManagementEvents.OnParentPackageInstalled(new FakePackage());
			CompleteReadPackagesTask();
			
			Assert.AreEqual(0, viewModel.PackageViewModels.Count);
		}
	}
}
