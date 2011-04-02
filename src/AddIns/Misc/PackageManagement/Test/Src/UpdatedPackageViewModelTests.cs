// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.PackageManagement.Design;
using NUnit.Framework;
using PackageManagement.Tests.Helpers;

namespace PackageManagement.Tests
{
	[TestFixture]
	public class UpdatedPackageViewModelTests
	{
		TestableUpdatedPackageViewModel viewModel;
		FakePackageManagementService fakePackageManagementService;
		FakePackageRepository sourcePackageRepository;
		
		void CreateViewModel()
		{
			viewModel = new TestableUpdatedPackageViewModel();
			fakePackageManagementService = viewModel.FakePackageManagementService;
			sourcePackageRepository = viewModel.FakeSourcePackageRepository;
		}
		
		[Test]
		public void AddPackage_PackageAddedSuccessfully_PackageUpdatedUsingSourcePackageRepository()
		{
			CreateViewModel();
			viewModel.AddPackage();
						
			Assert.AreEqual(sourcePackageRepository, fakePackageManagementService.RepositoryPassedToUpdatePackage);
		}
	
		[Test]
		public void AddPackage_PackageAddedSuccessfully_PackageUpdated()
		{
			CreateViewModel();
			viewModel.AddPackage();
			
			var expectedPackage = viewModel.FakePackage;
			var actualPackage = fakePackageManagementService.PackagePassedToUpdatePackage;
						
			Assert.AreEqual(expectedPackage, actualPackage);
		}
		
		[Test]
		public void AddPackage_PackageAddedSuccessfully_PackageUpdatedUsingPackageOperations()
		{
			CreateViewModel();
			viewModel.AddPackage();
			
			var expectedOperations = viewModel.FakePackageOperationResolver.PackageOperations;
			var actualOperations = fakePackageManagementService.PackageOperationsPassedToUpdatePackage;
						
			Assert.AreEqual(expectedOperations, actualOperations);
		}
	}
}
