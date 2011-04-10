// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.PackageManagement;
using ICSharpCode.PackageManagement.Design;
using NuGet;
using NUnit.Framework;
using PackageManagement.Tests.Helpers;

namespace PackageManagement.Tests
{
	[TestFixture]
	public class InstalledPackagesViewModelTests
	{
		InstalledPackagesViewModel viewModel;
		FakePackageManagementService packageManagementService;
		FakeRegisteredPackageRepositories registeredPackageRepositories;
		ExceptionThrowingPackageManagementService exceptionThrowingPackageManagementService;
		FakeTaskFactory taskFactory;
		
		void CreateViewModel()
		{
			CreatePackageManagementService();
			CreateViewModel(packageManagementService);
		}
		
		void CreatePackageManagementService()
		{
			packageManagementService = new FakePackageManagementService();
			registeredPackageRepositories = new FakeRegisteredPackageRepositories();
		}
		
		void CreateExceptionThrowingPackageManagementService()
		{
			exceptionThrowingPackageManagementService = new ExceptionThrowingPackageManagementService();
		}
		
		void CreateViewModel(FakePackageManagementService packageManagementService)
		{
			registeredPackageRepositories = new FakeRegisteredPackageRepositories();
			var packageViewModelFactory = new FakePackageViewModelFactory();
			taskFactory = new FakeTaskFactory();
			
			viewModel = new InstalledPackagesViewModel(
				packageManagementService,
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
		
		FakePackage AddPackageToProjectManagerLocalPackageRepository()
		{
			var package = new FakePackage("Test");
			FakePackageRepository repository = packageManagementService.FakeActiveProjectManager.FakeLocalRepository;
			repository.FakePackages.Add(package);
			return package;
		}
		
		[Test]
		public void PackageViewModels_PackageReferenceIsAdded_PackageViewModelsIsUpdated()
		{
			CreateViewModel();
			viewModel.ReadPackages();
			CompleteReadPackagesTask();
			FakePackage package = AddPackageToProjectManagerLocalPackageRepository();
			
			ClearReadPackagesTasks();
			packageManagementService.FireParentPackageInstalled();
			CompleteReadPackagesTask();
		
			IPackage firstPackage = viewModel.PackageViewModels[0].GetPackage();
			Assert.AreEqual(package, firstPackage);
		}
		
		[Test]
		public void PackageViewModels_PackageReferenceIsRemoved_PackageViewModelsIsUpdated()
		{
			CreateViewModel();
			FakePackage package = AddPackageToProjectManagerLocalPackageRepository();
			viewModel.ReadPackages();
			CompleteReadPackagesTask();
			
			packageManagementService.FakeActiveProjectManager.FakeLocalRepository.FakePackages.Clear();
			
			ClearReadPackagesTasks();
			packageManagementService.FireParentPackageUninstalled();
			CompleteReadPackagesTask();
		
			Assert.AreEqual(0, viewModel.PackageViewModels.Count);
		}
		
		[Test]
		public void ReadPackages_ActiveProjectManagerThrowsException_ErrorMessageFromExceptionNotOverriddenByReadPackagesCall()
		{
			CreateExceptionThrowingPackageManagementService();
			exceptionThrowingPackageManagementService.ExeptionToThrowWhenActiveProjectManagerAccessed = new Exception("Test");
			CreateViewModel(exceptionThrowingPackageManagementService);
			viewModel.ReadPackages();
			
			ApplicationException ex = Assert.Throws<ApplicationException>(() => CompleteReadPackagesTask());
			Assert.AreEqual("Test", ex.Message);
		}
		
		[Test]
		public void ReadPackages_OnePackageInLocalRepository_RepositoryIsNotCreatedByBackgroundThread()
		{
			CreatePackageManagementService();
			packageManagementService.AddPackageToProjectLocalRepository(new FakePackage());
			CreateViewModel(packageManagementService);
			viewModel.ReadPackages();
			
			packageManagementService.FakeActiveProjectManager.FakeLocalRepository = null;
			CompleteReadPackagesTask();
			
			Assert.AreEqual(1, viewModel.PackageViewModels.Count);
		}
		
		[Test]
		public void PackageViewModels_PackageReferenceIsAddedAfterViewModelIsDisposed_PackageViewModelsIsNotUpdated()
		{
			CreateViewModel();
			viewModel.ReadPackages();
			CompleteReadPackagesTask();
			FakePackage package = AddPackageToProjectManagerLocalPackageRepository();
			
			ClearReadPackagesTasks();
			viewModel.Dispose();
			packageManagementService.FireParentPackageInstalled();
			CompleteReadPackagesTask();
		
			Assert.AreEqual(0, viewModel.PackageViewModels.Count);
		}
		
		[Test]
		public void PackageViewModels_PackageReferenceIsRemovedAfterViewModelIsDisposed_PackageViewModelsIsNotUpdated()
		{
			CreateViewModel();
			FakePackage package = AddPackageToProjectManagerLocalPackageRepository();
			viewModel.ReadPackages();
			CompleteReadPackagesTask();
			
			packageManagementService.FakeActiveProjectManager.FakeLocalRepository.FakePackages.Clear();
			
			ClearReadPackagesTasks();
			viewModel.Dispose();
			packageManagementService.FireParentPackageUninstalled();
			CompleteReadPackagesTask();
		
			Assert.AreEqual(1, viewModel.PackageViewModels.Count);
		}
	}
}
