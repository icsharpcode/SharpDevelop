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
	public class PackageUpdatesViewModelTests
	{
		PackageUpdatesViewModel viewModel;
		FakePackageManagementService packageManagementService;
		
		void CreateViewModel()
		{
			packageManagementService = new FakePackageManagementService();
			viewModel = new PackageUpdatesViewModel(packageManagementService);
		}
		
		FakePackage AddPackageToLocalRepository(string version)
		{
			var package = CreatePackage(version);
			packageManagementService.AddPackageToProjectLocalRepository(package);
			return package;
		}
		
		FakePackage AddPackageToSourceRepository(string version)
		{
			var package = CreatePackage(version);
			packageManagementService.FakeActivePackageRepository.FakePackages.Add(package);
			return package;
		}
		
		FakePackage CreatePackage(string version)
		{
			var package = new FakePackage() {
				Id = "Test",
				Description = String.Empty,
				Version = new Version(version)
			};
			return package;
		}
		
		[Test]
		public void ReadPackages_OneNewerPackageVersionAvailable_NewerPackageVersionDisplayed()
		{
			CreateViewModel();
			AddPackageToLocalRepository("1.0.0.0");
			var newerPackage = AddPackageToSourceRepository("1.1.0.0");
			
			viewModel.ReadPackages();
			
			var expectedPackages = new FakePackage[] {
				newerPackage
			};
			
			PackageCollectionAssert.AreEqual(expectedPackages, viewModel.PackageViewModels);
		}
		
		[Test]
		public void ReadPackages_TwoPackagesInSourceRepositoryAndOneNewerPackageVersionAvailable_NewerPackageVersionDisplayed()
		{
			CreateViewModel();
			AddPackageToLocalRepository("1.0.0.0");
			AddPackageToSourceRepository("1.0.0.0");
			var newerPackage = AddPackageToSourceRepository("1.1.0.0");
			
			viewModel.ReadPackages();
			
			var expectedPackages = new FakePackage[] {
				newerPackage
			};
			
			PackageCollectionAssert.AreEqual(expectedPackages, viewModel.PackageViewModels);
		}
	}
}
