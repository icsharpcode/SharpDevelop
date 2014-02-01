// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

using System;
using System.Linq;
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
		public void PackageViewModels_PackageReferenceIsBeingRemoved_PackageViewModelsIsUpdated()
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
		public void PackageViewModels_PackageReferenceIsBeingRemovedAfterViewModelIsDisposed_PackageViewModelsIsNotUpdated()
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
		
		[Test]
		public void PackageViewModels_PackagesUpdated_PackageViewModelsIsUpdated()
		{
			CreateViewModel();
			viewModel.ReadPackages();
			CompleteReadPackagesTask();
			FakePackage package = AddPackageToProjectLocalRepository();
			ClearReadPackagesTasks();
			
			packageManagementEvents.OnParentPackagesUpdated(new FakePackage[] { package });
			CompleteReadPackagesTask();
		
			IPackage firstPackage = viewModel.PackageViewModels[0].GetPackage();
			Assert.AreEqual(package, firstPackage);
		}
		
		[Test]
		public void PackageViewModels_PackagesUpdatedAfterViewModelIsDisposed_PackageViewModelsIsNotUpdated()
		{
			CreateViewModel();
			viewModel.ReadPackages();
			CompleteReadPackagesTask();
			FakePackage package = AddPackageToProjectLocalRepository();
			ClearReadPackagesTasks();
			
			viewModel.Dispose();
			
			packageManagementEvents.OnParentPackagesUpdated(new FakePackage[] { package });
			CompleteReadPackagesTask();
		
			Assert.AreEqual(0, viewModel.PackageViewModels.Count);
		}
		
		[Test]
		public void PackageViewModels_ChildViewModelParent_IsInstalledPackagesViewModel()
		{
			CreateViewModel();
			FakePackage package = AddPackageToProjectLocalRepository();
			viewModel.ReadPackages();
			CompleteReadPackagesTask();
		
			PackageViewModel childViewModel = viewModel.PackageViewModels.First();
			IPackageViewModelParent parent = childViewModel.GetParent();
			Assert.AreEqual(viewModel, parent);
		}
	}
}
