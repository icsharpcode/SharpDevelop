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
		FakePackageManagementSolution solution;
		FakeTaskFactory taskFactory;
		FakeRegisteredPackageRepositories registeredPackageRepositories;
		ExceptionThrowingPackageManagementSolution exceptionThrowingSolution;
		
		void CreateViewModel()
		{
			CreateSolution();
			CreateViewModel(solution);
		}
		
		void CreateSolution()
		{
			solution = new FakePackageManagementSolution();
		}
		
		void CreateViewModel(FakePackageManagementSolution solution)
		{
			taskFactory = new FakeTaskFactory();
			registeredPackageRepositories = new FakeRegisteredPackageRepositories();
			var packageViewModelFactory = new FakePackageViewModelFactory();
			viewModel = new UpdatedPackagesViewModel(
				solution,
				registeredPackageRepositories,
				packageViewModelFactory,
				taskFactory);
		}
		
		void CreateExceptionThrowingSolution()
		{
			exceptionThrowingSolution = new ExceptionThrowingPackageManagementSolution();
		}
		
		void CompleteReadPackagesTask()
		{
			taskFactory.ExecuteAllFakeTasks();
		}

		FakePackage AddPackageToLocalRepository(string version)
		{
			var package = FakePackage.CreatePackageWithVersion(version);
			solution.AddPackageToActiveProjectLocalRepository(package);
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
		public void ReadPackages_OneNewerPackageVersionAvailable_ProjectNotCreatedByBackgroundThread()
		{
			CreateViewModel();
			AddPackageToLocalRepository("1.0.0.0");
			var newerPackage = AddPackageToAggregateRepository("1.1.0.0");
			
			viewModel.ReadPackages();
			
			registeredPackageRepositories.FakeAggregateRepository = null;
			solution.FakeActiveProject = null;
			CompleteReadPackagesTask();
			
			Assert.AreEqual(1, viewModel.PackageViewModels.Count);
		}
		
		[Test]
		public void ReadPackages_GetActiveProjectThrowsException_ErrorMessageFromExceptionNotOverriddenByReadPackagesCall()
		{
			CreateExceptionThrowingSolution();
			exceptionThrowingSolution.ExceptionToThrowWhenGetActiveProjectCalled =
				new Exception("Test");
			CreateViewModel(exceptionThrowingSolution);
			viewModel.ReadPackages();
			
			ApplicationException ex = Assert.Throws<ApplicationException>(() => CompleteReadPackagesTask());
			Assert.AreEqual("Test", ex.Message);
		}
	}
}
