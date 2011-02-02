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
		
		void CreateViewModel(FakePackageManagementService packageManagementService)
		{
			taskFactory = new FakeTaskFactory();
			viewModel = new InstalledPackagesViewModel(packageManagementService, taskFactory);			
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
		public void HasError_ActiveProjectManagerThrowsException_ReturnsTrue()
		{
			CreatePackageManagementService();
			packageManagementService.ActiveProjectManagerExeptionToThrow = new Exception();
			CreateViewModel(packageManagementService);
			
			Assert.IsTrue(viewModel.HasError);
		}
		
		[Test]
		public void ErrorMessage_ActiveProjectManagerThrowsException_ReturnsErrorMessageFromException()
		{
			CreatePackageManagementService();
			packageManagementService.ActiveProjectManagerExeptionToThrow = new Exception("Test");
			CreateViewModel(packageManagementService);
			
			Assert.AreEqual("Test", viewModel.ErrorMessage);
		}
		
		[Test]
		public void ReadPackages_ActiveProjectManagerThrowsException_ErrorMessageFromExceptionNotOverriddenByReadPackagesCall()
		{
			CreatePackageManagementService();
			packageManagementService.ActiveProjectManagerExeptionToThrow = new Exception("Test");
			CreateViewModel(packageManagementService);
			viewModel.ReadPackages();
			
			ApplicationException ex = Assert.Throws<ApplicationException>(() => CompleteReadPackagesTask());
			Assert.AreEqual("Test", ex.Message);
		}
	}
}
