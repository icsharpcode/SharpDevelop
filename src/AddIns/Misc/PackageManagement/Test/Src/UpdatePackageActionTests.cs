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
	public class UpdatePackageActionTests
	{
		UpdatePackageAction action;
		FakePackageManagementService fakePackageManagementService;
		FakePackageManager fakePackageManager;
		UpdatePackageHelper updatePackageHelper;
		
		void CreatePackageManagementService()
		{
			fakePackageManagementService = new FakePackageManagementService();
			fakePackageManager = fakePackageManagementService.FakePackageManagerToReturnFromCreatePackageManager;
			action = new UpdatePackageAction(fakePackageManagementService);
			updatePackageHelper = new UpdatePackageHelper(action);
		}
		
		[Test]
		public void Execute_PackageAndRepositoryPassed_PackageIsUpdated()
		{
			CreatePackageManagementService();
			updatePackageHelper.UpdateTestPackage();
			
			var expectedPackage = updatePackageHelper.TestPackage;
			var actualPackage = fakePackageManager.PackagePassedToUpdatePackage;
			
			Assert.AreEqual(expectedPackage, actualPackage);
		}
		
		[Test]
		public void UpdateDependencies_DefaultValue_ReturnsTrue()
		{
			CreatePackageManagementService();
			Assert.IsTrue(action.UpdateDependencies);
		}
		
		[Test]
		public void Execute_PackageAndRepositoryPassed_RepositoryUsedToCreatePackageManager()
		{
			CreatePackageManagementService();
			updatePackageHelper.UpdateTestPackage();
			
			var expectedRepository = updatePackageHelper.PackageRepository;
			var actualRepository = fakePackageManagementService.PackageRepositoryPassedToCreatePackageManager;
			
			Assert.AreEqual(expectedRepository, actualRepository);
		}
		
		[Test]
		public void Execute_PackageAndRepositoryPassed_PackageOperationsUsedToUpdatePackage()
		{
			CreatePackageManagementService();
			updatePackageHelper.UpdateTestPackage();
			
			var expectedOperations = updatePackageHelper.PackageOperations;
			var actualOperations = fakePackageManager.PackageOperationsPassedToUpdatePackage;
			
			Assert.AreEqual(expectedOperations, actualOperations);
		}
		
		[Test]
		public void Execute_PackageAndRepositoryPassed_DependenciesUpdated()
		{
			CreatePackageManagementService();
			updatePackageHelper.UpdateDependencies = true;
			updatePackageHelper.UpdateTestPackage();
			
			bool result = fakePackageManager.UpdateDependenciesPassedToUpdatePackage;
			
			Assert.IsTrue(result);
		}
		
		[Test]
		public void Execute_PackageAndRepositoryPassed_PackageInstalledEventIsFired()
		{
			CreatePackageManagementService();
			updatePackageHelper.UpdateTestPackage();
			
			var expectedPackage = updatePackageHelper.TestPackage;
			var actualPackage = fakePackageManagementService.PackagePassedToOnParentPackageInstalled;
			Assert.AreEqual(expectedPackage, actualPackage);
		}
		
		[Test]
		public void Execute_PackageIdAndSourceAndProjectPassed_PackageSourceUsedToCreatePackageManager()
		{
			CreatePackageManagementService();
			updatePackageHelper.UpdatePackageById("PackageId");
			
			var expectedPackageSource = updatePackageHelper.PackageSource;
			var actualPackageSource = fakePackageManagementService.PackageSourcePassedToCreatePackageManager;
			
			Assert.AreEqual(expectedPackageSource, actualPackageSource);
		}
		
		[Test]
		public void Execute_PackageIdAndSourceAndProjectPassed_ProjectIsUsedToCreatePackageManager()
		{
			CreatePackageManagementService();
			updatePackageHelper.UpdatePackageById("PackageId");
			
			var actualProject = fakePackageManagementService.ProjectPassedToCreatePackageManager;
			var expectedProject = updatePackageHelper.TestableProject;
			
			Assert.AreEqual(expectedProject, actualProject);
		}
		
		[Test]
		public void Execute_PackagePassedButNoPackageOperations_PackageOperationsRetrievedFromPackageManager()
		{
			CreatePackageManagementService();
			updatePackageHelper.PackageOperations = null;
			updatePackageHelper.UpdateTestPackage();
			
			var actualOperations = action.Operations;
			var expectedOperations = fakePackageManager.PackageOperationsToReturnFromGetInstallPackageOperations;
			
			Assert.AreEqual(expectedOperations, actualOperations);
		}
		
		[Test]
		public void Execute_PackagePassedButNoPackageOperations_PackageOperationsCreatedForPackage()
		{
			CreatePackageManagementService();
			updatePackageHelper.PackageOperations = null;
			updatePackageHelper.UpdateTestPackage();
			
			var expectedPackage = updatePackageHelper.TestPackage;
			var actualPackage = fakePackageManager.PackagePassedToGetInstallPackageOperations;
			
			Assert.AreEqual(expectedPackage, actualPackage);
		}
		
		[Test]
		public void Execute_PackageIdAndSourceAndProjectPassedAndUpdateDependenciesIsTrue_DependenciesUpdatedWhenUpdatingPackage()
		{
			CreatePackageManagementService();
			
			updatePackageHelper.UpdateDependencies = true;
			updatePackageHelper.UpdatePackageById("PackageId");
			
			bool result = fakePackageManager.UpdateDependenciesPassedToUpdatePackage;
			
			Assert.IsTrue(result);
		}
		
		[Test]
		public void Execute_PackageIdAndSourceAndProjectPassedAndUpdateDependenciesIsFalse_DependenciesNotUpdatedWhenGettingPackageOperations()
		{
			CreatePackageManagementService();
			
			updatePackageHelper.UpdateDependencies = false;
			updatePackageHelper.UpdatePackageById("PackageId");
			
			bool result = fakePackageManager.UpdateDependenciesPassedToUpdatePackage;
			
			Assert.IsFalse(result);
		}
		
		[Test]
		public void Execute_UpdatedDepdenciesIsFalseAndNoPackageOperations_DependenciesIgnoredWhenGettingPackageOperations()
		{
			CreatePackageManagementService();
			
			updatePackageHelper.UpdateDependencies = false;
			updatePackageHelper.UpdatePackageById("PackageId");
			
			bool result = fakePackageManager.IgnoreDependenciesPassedToGetInstallPackageOperations;
			
			Assert.IsTrue(result);
		}
		
		[Test]
		public void Execute_UpdateDependenciesIsTrueAndNoPackageOperations_DependenciesNotIgnoredWhenGettingPackageOperations()
		{
			CreatePackageManagementService();
			
			updatePackageHelper.UpdateDependencies = true;
			updatePackageHelper.UpdatePackageById("PackageId");
			
			bool result = fakePackageManager.IgnoreDependenciesPassedToGetInstallPackageOperations;
			
			Assert.IsFalse(result);
		}
		
		[Test]
		public void Execute_PackageAndPackageOperationsSet_OperationsNotRetrievedFromPackageManager()
		{
			CreatePackageManagementService();
			updatePackageHelper.UpdateTestPackage();
			
			var actualPackage = fakePackageManager.PackagePassedToGetInstallPackageOperations;
			
			Assert.IsNull(actualPackage);
		}
	}
}
