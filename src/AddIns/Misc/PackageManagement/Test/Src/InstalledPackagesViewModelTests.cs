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
		}
		
		void CreateExceptionThrowingPackageManagementService()
		{
			exceptionThrowingPackageManagementService = new ExceptionThrowingPackageManagementService();
		}
		
		void CreateViewModel(FakePackageManagementService packageManagementService)
		{
			taskFactory = new FakeTaskFactory();
			var messageReporter = new FakeMessageReporter();
			viewModel = new InstalledPackagesViewModel(packageManagementService, messageReporter, taskFactory);			
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
		public void PackageViewModels_PackageReferenceIsAdded_PackageViewModelsIsUpdated()
		{
			CreateViewModel();
			viewModel.ReadPackages();
			CompleteReadPackagesTask();
			FakePackage package = new FakePackage();
			package.Id = "Test";
			FakePackageRepository repository = packageManagementService.FakeActiveProjectManager.FakeLocalRepository;
			repository.FakePackages.Add(package);
			
			ClearReadPackagesTasks();
			packageManagementService.FirePackageInstalled();
			CompleteReadPackagesTask();
		
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
			CompleteReadPackagesTask();
			
			repository.FakePackages.Clear();
			
			ClearReadPackagesTasks();
			packageManagementService.FirePackageUninstalled();
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
	}
}
