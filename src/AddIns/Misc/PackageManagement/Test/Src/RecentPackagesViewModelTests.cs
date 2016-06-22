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
using ICSharpCode.PackageManagement;
using ICSharpCode.PackageManagement.Design;
using NUnit.Framework;
using PackageManagement.Tests.Helpers;

namespace PackageManagement.Tests
{
	[TestFixture]
	public class RecentPackagesViewModelTests
	{
		RecentPackagesViewModel viewModel;
		PackageManagementEvents packageManagementEvents;
		FakeRegisteredPackageRepositories registeredPackageRepositories;
		FakeTaskFactory taskFactory;
		
		void CreateViewModel()
		{
			CreateViewModel(new FakeRegisteredPackageRepositories());
		}
		
		void CreateViewModel(FakeRegisteredPackageRepositories registeredPackageRepositories)
		{
			this.registeredPackageRepositories = registeredPackageRepositories;
			taskFactory = new FakeTaskFactory();
			var packageViewModelFactory = new FakePackageViewModelFactory();
			packageManagementEvents = new PackageManagementEvents();
			viewModel = new RecentPackagesViewModel(
				new FakePackageManagementSolution(),
				packageManagementEvents,
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
		
		FakePackage AddPackageToRecentPackageRepository()
		{
			var package = new FakePackage("Test");
			FakePackageRepository repository = registeredPackageRepositories.FakeRecentPackageRepository;
			repository.FakePackages.Add(package);
			return package;
		}

		[Test]
		public void PackageViewModels_PackageIsInstalledAfterRecentPackagesDisplayed_PackagesOnDisplayAreUpdated()
		{
			CreateViewModel();
			viewModel.ReadPackages();
			CompleteReadPackagesTask();
			var package = AddPackageToRecentPackageRepository();
			
			ClearReadPackagesTasks();
			packageManagementEvents.OnParentPackageInstalled(new FakePackage());
			CompleteReadPackagesTask();
			
			var expectedPackages = new FakePackage[] {
				package
			};
			
			PackageCollectionAssert.AreEqual(expectedPackages, viewModel.PackageViewModels);
		}
		
		[Test]
		public void PackageViewModels_PackageIsUninstalledAfterRecentPackagesDisplayed_PackagesOnDisplayAreNotUpdated()
		{
			CreateViewModel();
			viewModel.ReadPackages();
			CompleteReadPackagesTask();
			var package = AddPackageToRecentPackageRepository();

			ClearReadPackagesTasks();
			packageManagementEvents.OnParentPackageUninstalled(new FakePackage());
			CompleteReadPackagesTask();

			var expectedPackages = new FakePackage[0];

			PackageCollectionAssert.AreEqual(expectedPackages, viewModel.PackageViewModels);
		}
		
		[Test]
		public void PackageViewModels_PackageIsUninstalledAfterViewModelIsDisposed_PackagesOnDisplayAreNotUpdated()
		{
			CreateViewModel();
			viewModel.ReadPackages();
			CompleteReadPackagesTask();
			AddPackageToRecentPackageRepository();
			
			ClearReadPackagesTasks();
			viewModel.Dispose();
			
			packageManagementEvents.OnParentPackageUninstalled(new FakePackage());
			CompleteReadPackagesTask();
			
			Assert.AreEqual(0, viewModel.PackageViewModels.Count);
		}
		
		[Test]
		public void PackageViewModels_PackageIsInstalledAfterViewModelIsDisposed_PackagesOnDisplayAreNotUpdated()
		{
			CreateViewModel();
			viewModel.ReadPackages();
			CompleteReadPackagesTask();
			AddPackageToRecentPackageRepository();
			
			ClearReadPackagesTasks();
			
			viewModel.Dispose();
			packageManagementEvents.OnParentPackageInstalled(new FakePackage());
			CompleteReadPackagesTask();
			
			Assert.AreEqual(0, viewModel.PackageViewModels.Count);
		}
		
		[Test]
		public void ReadPackages_ExceptionThrownWhenAccessingActiveRepository_ErrorMessageFromExceptionNotOverriddenByReadPackagesCall()
		{
			var registeredRepositories = new ExceptionThrowingRegisteredPackageRepositories();
			registeredRepositories.ExceptionToThrowWhenRecentPackageRepositoryAccessed = 
				new Exception("Test");
			CreateViewModel(registeredRepositories);
			
			viewModel.ReadPackages();
			
			ApplicationException ex = Assert.Throws<ApplicationException>(CompleteReadPackagesTask);
			Assert.AreEqual("Test", ex.Message);
		}
	}
}
