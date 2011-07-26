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
		FakePackageManagementSolution solution;
		PackageManagementEvents packageManagementEvents;
		FakeRegisteredPackageRepositories registeredPackageRepositories;
		ExceptionThrowingPackageManagementSolution exceptionThrowingSolution;
		FakeTaskFactory taskFactory;
		
		void CreateViewModel()
		{
			CreateSolution();
			CreateViewModel(solution);
		}
		
		void CreateSolution()
		{
			solution = new FakePackageManagementSolution();
			registeredPackageRepositories = new FakeRegisteredPackageRepositories();
		}
		
		void CreateExceptionThrowingSolution()
		{
			exceptionThrowingSolution = new ExceptionThrowingPackageManagementSolution();
		}
		
		void CreateViewModel(FakePackageManagementSolution solution)
		{
			registeredPackageRepositories = new FakeRegisteredPackageRepositories();
			var packageViewModelFactory = new FakePackageViewModelFactory();
			var installedPackageViewModelFactory = new InstalledPackageViewModelFactory(packageViewModelFactory);
			taskFactory = new FakeTaskFactory();
			packageManagementEvents = new PackageManagementEvents();
			
			viewModel = new InstalledPackagesViewModel(
				solution,
				packageManagementEvents,
				registeredPackageRepositories,
				installedPackageViewModelFactory,
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
		
		FakePackage AddPackageToProjectLocalRepository()
		{
			var package = new FakePackage("Test");
			solution.FakeActiveProject.FakePackages.Add(package);
			return package;
		}
		
		FakePackage AddPackageToSolution()
		{
			var package = new FakePackage("Test");
			solution.FakeInstalledPackages.Add(package);
			return package;
		}
		
		void NoProjectSelected()
		{
			solution.NoProjectsSelected();
		}
		
		[Test]
		public void PackageViewModels_PackageReferenceIsAdded_PackageViewModelsIsUpdated()
		{
			CreateViewModel();
			viewModel.ReadPackages();
			CompleteReadPackagesTask();
			FakePackage package = AddPackageToProjectLocalRepository();
			
			ClearReadPackagesTasks();
			packageManagementEvents.OnParentPackageInstalled(new FakePackage());
			CompleteReadPackagesTask();
		
			IPackage firstPackage = viewModel.PackageViewModels[0].GetPackage();
			Assert.AreEqual(package, firstPackage);
		}
		
		[Test]
		public void PackageViewModels_PackageReferenceIsRemoved_PackageViewModelsIsUpdated()
		{
			CreateViewModel();
			FakePackage package = AddPackageToProjectLocalRepository();
			viewModel.ReadPackages();
			CompleteReadPackagesTask();
			
			solution.FakeActiveProject.FakePackages.Clear();

			ClearReadPackagesTasks();
			packageManagementEvents.OnParentPackageUninstalled(new FakePackage());
			CompleteReadPackagesTask();
		
			Assert.AreEqual(0, viewModel.PackageViewModels.Count);
		}
		
		[Test]
		public void ReadPackages_GetActiveProjectThrowsException_ErrorMessageFromExceptionNotOverriddenByReadPackagesCall()
		{
			CreateExceptionThrowingSolution();
			exceptionThrowingSolution.ExceptionToThrowWhenGetActiveProjectCalled = new Exception("Test");
			CreateViewModel(exceptionThrowingSolution);
			viewModel.ReadPackages();
			
			ApplicationException ex = Assert.Throws<ApplicationException>(() => CompleteReadPackagesTask());
			Assert.AreEqual("Test", ex.Message);
		}
		
		[Test]
		public void ReadPackages_OnePackageInLocalRepository_ProjectIsNotCreatedByBackgroundThread()
		{
			CreateSolution();
			solution.AddPackageToActiveProjectLocalRepository(new FakePackage());
			CreateViewModel(solution);
			viewModel.ReadPackages();
			
			solution.FakeActiveProject = null;
			CompleteReadPackagesTask();
			
			Assert.AreEqual(1, viewModel.PackageViewModels.Count);
		}
		
		[Test]
		public void PackageViewModels_PackageReferenceIsAddedAfterViewModelIsDisposed_PackageViewModelsIsNotUpdated()
		{
			CreateViewModel();
			viewModel.ReadPackages();
			CompleteReadPackagesTask();
			FakePackage package = AddPackageToProjectLocalRepository();
			
			ClearReadPackagesTasks();
			viewModel.Dispose();
			packageManagementEvents.OnParentPackageInstalled(new FakePackage());
			CompleteReadPackagesTask();
		
			Assert.AreEqual(0, viewModel.PackageViewModels.Count);
		}
		
		[Test]
		public void PackageViewModels_PackageReferenceIsRemovedAfterViewModelIsDisposed_PackageViewModelsIsNotUpdated()
		{
			CreateViewModel();
			FakePackage package = AddPackageToProjectLocalRepository();
			viewModel.ReadPackages();
			CompleteReadPackagesTask();
			
			solution.FakeActiveProject.FakePackages.Clear();
			
			ClearReadPackagesTasks();
			viewModel.Dispose();
			packageManagementEvents.OnParentPackageUninstalled(new FakePackage());
			CompleteReadPackagesTask();
		
			Assert.AreEqual(1, viewModel.PackageViewModels.Count);
		}
		
		[Test]
		public void PackageViewModels_OnlySolutionSelectedThatContainsOneInstalledPackage_ReturnsOnePackageViewModel()
		{
			CreateSolution();
			NoProjectSelected();
			CreateViewModel(solution);
			FakePackage package = AddPackageToSolution();
			viewModel.ReadPackages();
			CompleteReadPackagesTask();
			
			var expectedPackages = new FakePackage[] {
				package
			};
		
			PackageCollectionAssert.AreEqual(expectedPackages, viewModel.PackageViewModels);
		}
	}
}
