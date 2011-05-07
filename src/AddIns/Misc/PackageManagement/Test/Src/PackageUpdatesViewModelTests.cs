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
		FakeTaskFactory taskFactory;
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
			var messageReporter = new FakeMessageReporter();
			viewModel = new PackageUpdatesViewModel(packageManagementService, messageReporter, taskFactory);
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
			var package = CreatePackage(version);
			packageManagementService.AddPackageToProjectLocalRepository(package);
			return package;
		}
		
		FakePackage AddPackageToSourceRepository(string version)
		{
			var package = CreatePackage(version);
			packageManagementService.FakeAggregateRepository.FakePackages.Add(package);
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
			AddPackageToSourceRepository("1.0.0.0");
			var newerPackage = AddPackageToSourceRepository("1.1.0.0");
			
			viewModel.ReadPackages();
			CompleteReadPackagesTask();
			
			var expectedPackages = new FakePackage[] {
				newerPackage
			};
			
			PackageCollectionAssert.AreEqual(expectedPackages, viewModel.PackageViewModels);
		}
		
		[Test]
		public void ReadPackages_OneNewerPackage_RepositoriesNotCreatedByBackgroundThread()
		{
			CreateViewModel();
			AddPackageToLocalRepository("1.0.0.0");
			var newerPackage = AddPackageToSourceRepository("1.1.0.0");
			
			viewModel.ReadPackages();
			
			packageManagementService.FakeAggregateRepository = null;
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
		
//		[Test]
//		public void ReadPackages_ActiveProjectManagerThrowsException_ErrorMessageFromExceptionReturned()
//		{
//			CreateExceptionThrowingPackageManagementService();
//			exceptionThrowingPackageManagementService.ExeptionToThrowWhenActiveProjectManagerAccessed = 
//				new Exception("Test");
//			CreateViewModel(exceptionThrowingPackageManagementService);
//			viewModel.ReadPackages();
//			
//			ApplicationException ex = Assert.Throws<ApplicationException>(() => CompleteReadPackagesTask());
//			Assert.AreEqual("Test", ex.Message);
//		}
	}
}
