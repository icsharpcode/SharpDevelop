// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.PackageManagement;
using ICSharpCode.PackageManagement.Design;
using NuGet;
using NUnit.Framework;

namespace PackageManagement.Tests
{
	[TestFixture]
	public class InstalledPackagesViewModelTests
	{
		InstalledPackagesViewModel viewModel;
		FakePackageManagementService packageManagementService;
		
		void CreateViewModel()
		{
			packageManagementService = new FakePackageManagementService();
			viewModel = new InstalledPackagesViewModel(packageManagementService);
		}
		
		[Test]
		public void PackageViewModels_PackageReferenceIsAdded_PackageViewModelsIsUpdated()
		{
			CreateViewModel();
			viewModel.ReadPackages();
			FakePackage package = new FakePackage();
			package.Id = "Test";
			FakePackageRepository repository = packageManagementService.FakeActiveProjectManager.FakeLocalRepository;
			repository.FakePackages.Add(package);
			
			packageManagementService.FirePackageInstalled();
		
			IPackage firstPackage = viewModel.PackageViewModels[0].GetPackage();
			Assert.AreEqual(package, firstPackage);
		}
		
		[Test]
		public void PackageViewModels_PackageReferenceIsRemoved_PackageViewModelsIsUpdated()
		{
			CreateViewModel();
			FakePackage package = new FakePackage();
			package.Id = "Test";
			FakePackageRepository repository = packageManagementService.FakeActiveProjectManager.FakeLocalRepository;
			repository.FakePackages.Add(package);
			viewModel.ReadPackages();
			
			repository.FakePackages.Clear();
			
			packageManagementService.FirePackageUninstalled();
		
			Assert.AreEqual(0, viewModel.PackageViewModels.Count);
		}
	}
}
