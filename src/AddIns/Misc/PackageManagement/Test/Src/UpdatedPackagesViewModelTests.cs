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
	public class UpdatedPackagesViewModelTests
	{
		UpdatedPackagesViewModel viewModel;
		FakePackageManagementService packageManagementService;
		FakeTaskFactory taskFactory;
		FakeRegisteredPackageRepositories registeredPackageRepositories;
		ExceptionThrowingPackageManagementService exceptionThrowingPackageManagementService;
		
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
			registeredPackageRepositories = new FakeRegisteredPackageRepositories();
			var packageViewModelFactory = new FakePackageViewModelFactory();
			viewModel = new UpdatedPackagesViewModel(
				packageManagementService,
				registeredPackageRepositories,
				packageViewModelFactory,
				taskFactory);
		}
		
		void CreateExceptionThrowingPackageManagementService()
		{
			exceptionThrowingPackageManagementService = new ExceptionThrowingPackageManagementService();
		}
		
		void CompleteReadPackagesTask()
		{
			taskFactory.ExecuteAllFakeTasks();
		}

		FakePackage AddPackageToLocalRepository(string version)
		{
			var package = FakePackage.CreatePackageWithVersion(version);
			packageManagementService.AddPackageToProjectLocalRepository(package);
			return package;
		}
		
		FakePackage AddPackageToAggregateRepository(string version)
		{
			return registeredPackageRepositories.AddFakePackageWithVersionToAggregrateRepository(version);
		}
		
		[Test]
		public void ReadPackages_OneNewerPackageVersionAvailable_NewerPackageVersionDisplayed()
		{
			CreateViewModel();
			AddPackageToLocalRepository("1.0.0.0");
			var newerPackage = AddPackageToAggregateRepository("1.1.0.0");
			
			viewModel.ReadPackages();
			CompleteReadPackagesTask();
			
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
			AddPackageToAggregateRepository("1.0.0.0");
			var newerPackage = AddPackageToAggregateRepository("1.1.0.0");
			
			viewModel.ReadPackages();
			CompleteReadPackagesTask();
			
			var expectedPackages = new FakePackage[] {
				newerPackage
			};
			
			PackageCollectionAssert.AreEqual(expectedPackages, viewModel.PackageViewModels);
		}
		
		[Test]
		public void ReadPackages_OneNewerPackageVersionAvailable_RepositoriesNotCreatedByBackgroundThread()
		{
			CreateViewModel();
			AddPackageToLocalRepository("1.0.0.0");
			var newerPackage = AddPackageToAggregateRepository("1.1.0.0");
			
			viewModel.ReadPackages();
			
			registeredPackageRepositories.FakeAggregateRepository = null;
			packageManagementService.FakeActiveProjectManager.LocalRepository = null;
			CompleteReadPackagesTask();
			
			Assert.AreEqual(1, viewModel.PackageViewModels.Count);
		}
		
		[Test]
		public void ReadPackages_ActiveProjectManagerThrowsException_ErrorMessageFromExceptionNotOverriddenByReadPackagesCall()
		{
			CreateExceptionThrowingPackageManagementService();
			exceptionThrowingPackageManagementService.ExeptionToThrowWhenActiveProjectManagerAccessed = 
				new Exception("Test");
			CreateViewModel(exceptionThrowingPackageManagementService);
			viewModel.ReadPackages();
			
			ApplicationException ex = Assert.Throws<ApplicationException>(() => CompleteReadPackagesTask());
			Assert.AreEqual("Test", ex.Message);
		}
	}
}
