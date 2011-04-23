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
		FakePackageManagementSolution fakeSolution;
		FakePackageManagementProject fakeProject;
		FakeUpdatePackageAction updatePackageAction;
		
		void CreateViewModel()
		{
			viewModel = new TestableUpdatedPackageViewModel();
			fakeSolution = viewModel.FakeSolution;
			fakeProject = fakeSolution.FakeProject;
			updatePackageAction = fakeProject.FakeUpdatePackageAction;
		}
		
		[Test]
		public void AddPackage_PackageAddedSuccessfully_ProjectCreatedUsingSourcePackageRepository()
		{
			CreateViewModel();
			viewModel.AddPackage();
						
			Assert.AreEqual(viewModel.FakePackage.Repository, fakeSolution.RepositoryPassedToGetActiveProject);
		}
	
		[Test]
		public void AddPackage_PackageAddedSuccessfully_PackageUpdated()
		{
			CreateViewModel();
			viewModel.AddPackage();
			
			var expectedPackage = viewModel.FakePackage;
			var actualPackage = updatePackageAction.Package;
						
			Assert.AreEqual(expectedPackage, actualPackage);
		}
		
		[Test]
		public void AddPackage_PackageAddedSuccessfully_PackageUpdatedUsingPackageOperations()
		{
			CreateViewModel();
			viewModel.AddPackage();
			
			var expectedOperations = viewModel.FakePackageOperationResolver.PackageOperations;
			var actualOperations = updatePackageAction.Operations;
						
			Assert.AreEqual(expectedOperations, actualOperations);
		}
		
		[Test]
		public void AddPackage_PackageAddedSuccessfully_PackageIsUpdated()
		{
			CreateViewModel();
			viewModel.AddPackage();
			
			Assert.IsTrue(updatePackageAction.IsExecuted);
		}
	}
}
