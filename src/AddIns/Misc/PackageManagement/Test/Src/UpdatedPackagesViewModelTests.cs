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
			solution.FakeActiveMSBuildProject = ProjectHelper.CreateTestProject();
		}
		
		void NoProjectsSelected()
		{
			solution.NoProjectsSelected();
		}
		
		void CreateViewModel(FakePackageManagementSolution solution)
		{
			CreateRegisteredPackageRepositories();
			CreateViewModel(solution, registeredPackageRepositories);
		}
		
		void CreateViewModel(FakePackageManagementSolution solution, FakeRegisteredPackageRepositories registeredPackageRepositories)
		{
			taskFactory = new FakeTaskFactory();
			var packageViewModelFactory = new FakePackageViewModelFactory();
			var updatedPackageViewModelFactory = new UpdatedPackageViewModelFactory(packageViewModelFactory);
			viewModel = new UpdatedPackagesViewModel(
				solution,
				registeredPackageRepositories,
				updatedPackageViewModelFactory,
				taskFactory);
		}
		
		void CreateRegisteredPackageRepositories()
		{
			registeredPackageRepositories = new FakeRegisteredPackageRepositories();
		}
		
		void CreateViewModel(FakeRegisteredPackageRepositories registeredPackageRepositories)
		{
			CreateSolution();
			CreateViewModel(solution, registeredPackageRepositories);
		}
		
		void CreateExceptionThrowingSolution()
		{
			exceptionThrowingSolution = new ExceptionThrowingPackageManagementSolution();
			exceptionThrowingSolution.FakeActiveMSBuildProject = ProjectHelper.CreateTestProject();
		}
		
		void CompleteReadPackagesTask()
		{
			taskFactory.ExecuteAllFakeTasks();
		}

		FakePackage AddPackageToLocalRepository(string version)
		{
			FakePackage package = FakePackage.CreatePackageWithVersion(version);
			solution.FakeProjectToReturnFromGetProject.FakePackages.Add(package);
			return package;
		}
		
		FakePackage AddPackageToActiveRepository(string version)
		{
			return registeredPackageRepositories.AddFakePackageWithVersionToActiveRepository(version);
		}
		
		FakePackage AddPackageToSolution(string version)
		{
			FakePackage package = FakePackage.CreatePackageWithVersion(version);
			solution.FakeInstalledPackages.Add(package);
			return package;
		}
		
		void AddOnePackageSourceToRegisteredSources()
		{
			registeredPackageRepositories.ClearPackageSources();
			registeredPackageRepositories.AddOnePackageSource();
			registeredPackageRepositories.HasMultiplePackageSources = false;
		}
		
		void AddTwoPackageSourcesToRegisteredSources()
		{
			var expectedPackageSources = new PackageSource[] {
				new PackageSource("http://first.com", "First"),
				new PackageSource("http://second.com", "Second")
			};
			AddPackageSourcesToRegisteredSources(expectedPackageSources);
			registeredPackageRepositories.HasMultiplePackageSources = true;
		}
		
		void AddPackageSourcesToRegisteredSources(PackageSource[] sources)
		{
			registeredPackageRepositories.ClearPackageSources();
			registeredPackageRepositories.AddPackageSources(sources);
		}
		
		[Test]
		public void ReadPackages_OneNewerPackageVersionAvailable_NewerPackageVersionDisplayed()
		{
			CreateViewModel();
			AddPackageToLocalRepository("1.0.0.0");
			FakePackage newerPackage = AddPackageToActiveRepository("1.1.0.0");
			
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
			AddPackageToActiveRepository("1.0.0.0");
			FakePackage newerPackage = AddPackageToActiveRepository("1.1.0.0");
			
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
			FakePackage newerPackage = AddPackageToActiveRepository("1.1.0.0");
			
			viewModel.ReadPackages();
			
			registeredPackageRepositories.FakeAggregateRepository = null;
			CompleteReadPackagesTask();
			
			Assert.AreEqual(1, viewModel.PackageViewModels.Count);
		}
		
		[Test]
		public void ReadPackages_GetProjectThrowsException_ErrorMessageFromExceptionNotOverriddenByReadPackagesCall()
		{
			CreateExceptionThrowingSolution();
			exceptionThrowingSolution.ExceptionToThrowWhenGetProjectCalled =
				new Exception("Test");
			CreateViewModel(exceptionThrowingSolution);
			viewModel.ReadPackages();
			
			ApplicationException ex = Assert.Throws<ApplicationException>(() => CompleteReadPackagesTask());
			Assert.AreEqual("Test", ex.Message);
		}
		
		[Test]
		public void ReadPackages_OnlySolutionSelectedAndOneNewerPackageVersionAvailable_NewerPackageVersionDisplayed()
		{
			CreateSolution();
			NoProjectsSelected();
			CreateViewModel(solution);
			AddPackageToSolution("1.0.0.0");
			FakePackage newerPackage = AddPackageToActiveRepository("1.1.0.0");
			
			viewModel.ReadPackages();
			CompleteReadPackagesTask();
			
			var expectedPackages = new FakePackage[] {
				newerPackage
			};
			
			PackageCollectionAssert.AreEqual(expectedPackages, viewModel.PackageViewModels);
		}
		
		[Test]
		public void ReadPackages_SolutionInitiallySelectedWithOneNewerPackageAndProjectSelectedBeforeSecondReadOfPackages_NewerPackageVersionDisplayed()
		{
			CreateSolution();
			NoProjectsSelected();
			CreateViewModel(solution);
			AddPackageToSolution("1.0.0.0");
			FakePackage newerPackage = AddPackageToActiveRepository("1.1.0.0");
			
			viewModel.ReadPackages();
			CompleteReadPackagesTask();
			
			TestableProject msbuildProject = ProjectHelper.CreateTestProject("Test");
			solution.FakeActiveMSBuildProject = msbuildProject;
			solution.FakeActiveProject = new FakePackageManagementProject("Test");
			
			viewModel.ReadPackages();
			CompleteReadPackagesTask();
			
			var expectedPackages = new FakePackage[] {
				newerPackage
			};
			
			PackageCollectionAssert.AreEqual(expectedPackages, viewModel.PackageViewModels);
		}
		
		[Test]
		public void ReadPackages_PrereleasePackageVersionAvailable_NoUpdatesFound()
		{
			CreateViewModel();
			
			AddPackageToLocalRepository("1.0.0");
			FakePackage newerPackage = AddPackageToActiveRepository("1.1.0-alpha");
			
			viewModel.ReadPackages();
			CompleteReadPackagesTask();
			
			Assert.AreEqual(0, viewModel.PackageViewModels.Count);
		}
		
		[Test]
		public void ShowSources_TwoPackageSources_ReturnsTrue()
		{
			CreateRegisteredPackageRepositories();
			AddTwoPackageSourcesToRegisteredSources();
			CreateViewModel(registeredPackageRepositories);
			
			Assert.IsTrue(viewModel.ShowPackageSources);
		}
		
		[Test]
		public void ShowPackageSources_OnePackageSources_ReturnsTrue()
		{
			CreateRegisteredPackageRepositories();
			AddOnePackageSourceToRegisteredSources();
			CreateViewModel(registeredPackageRepositories);
			
			Assert.IsTrue(viewModel.ShowPackageSources);
		}
	}
}
