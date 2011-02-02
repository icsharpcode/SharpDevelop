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
	}
}
