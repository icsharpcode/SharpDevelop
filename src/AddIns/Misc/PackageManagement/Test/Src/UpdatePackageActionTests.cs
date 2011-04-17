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
		FakePackageManagementSolution fakeSolution;
		FakePackageManagementEvents fakePackageManagementEvents;
		FakePackageManager fakePackageManager;
		UpdatePackageHelper updatePackageHelper;
		
		void CreateSolution()
		{
			fakeSolution = new FakePackageManagementSolution();
			fakePackageManagementEvents = new FakePackageManagementEvents();
			fakePackageManager = fakeSolution.FakePackageManagerToReturnFromCreatePackageManager;
			action = new UpdatePackageAction(fakeSolution, fakePackageManagementEvents);
			updatePackageHelper = new UpdatePackageHelper(action);
		}
		
		[Test]
		public void Execute_PackageAndRepositoryPassed_PackageIsUpdated()
		{
			CreateSolution();
			updatePackageHelper.UpdateTestPackage();
			
			var expectedPackage = updatePackageHelper.TestPackage;
			var actualPackage = fakePackageManager.PackagePassedToUpdatePackage;
			
			Assert.AreEqual(expectedPackage, actualPackage);
		}
		
		[Test]
		public void UpdateDependencies_DefaultValue_ReturnsTrue()
		{
			CreateSolution();
			Assert.IsTrue(action.UpdateDependencies);
		}
		
		[Test]
		public void Execute_PackageAndRepositoryPassed_RepositoryUsedToCreatePackageManager()
		{
			CreateSolution();
			updatePackageHelper.UpdateTestPackage();
			
			var expectedRepository = updatePackageHelper.PackageRepository;
			var actualRepository = fakeSolution.PackageRepositoryPassedToCreatePackageManager;
			
			Assert.AreEqual(expectedRepository, actualRepository);
		}
		
		[Test]
		public void Execute_PackageAndRepositoryPassed_PackageOperationsUsedToUpdatePackage()
		{
			CreateSolution();
			updatePackageHelper.UpdateTestPackage();
			
			var expectedOperations = updatePackageHelper.PackageOperations;
			var actualOperations = fakePackageManager.PackageOperationsPassedToUpdatePackage;
			
			Assert.AreEqual(expectedOperations, actualOperations);
		}
		
		[Test]
		public void Execute_PackageAndRepositoryPassed_DependenciesUpdated()
		{
			CreateSolution();
			updatePackageHelper.UpdateDependencies = true;
			updatePackageHelper.UpdateTestPackage();
			
			bool result = fakePackageManager.UpdateDependenciesPassedToUpdatePackage;
			
			Assert.IsTrue(result);
		}
		
		[Test]
		public void Execute_PackageAndRepositoryPassed_PackageInstalledEventIsFired()
		{
			CreateSolution();
			updatePackageHelper.UpdateTestPackage();
			
			var expectedPackage = updatePackageHelper.TestPackage;
			var actualPackage = fakePackageManagementEvents.PackagePassedToOnParentPackageInstalled;
			Assert.AreEqual(expectedPackage, actualPackage);
		}
		
		[Test]
		public void Execute_PackageIdAndSourceAndProjectPassed_PackageSourceUsedToCreatePackageManager()
		{
			CreateSolution();
			updatePackageHelper.UpdatePackageById("PackageId");
			
			var expectedPackageSource = updatePackageHelper.PackageSource;
			var actualPackageSource = fakeSolution.PackageSourcePassedToCreatePackageManager;
			
			Assert.AreEqual(expectedPackageSource, actualPackageSource);
		}
		
		[Test]
		public void Execute_PackageIdAndSourceAndProjectPassed_ProjectIsUsedToCreatePackageManager()
		{
			CreateSolution();
			updatePackageHelper.UpdatePackageById("PackageId");
			
			var actualProject = fakeSolution.ProjectPassedToCreatePackageManager;
			var expectedProject = updatePackageHelper.TestableProject;
			
			Assert.AreEqual(expectedProject, actualProject);
		}
		
		[Test]
		public void Execute_PackagePassedButNoPackageOperations_PackageOperationsRetrievedFromPackageManager()
		{
			CreateSolution();
			updatePackageHelper.PackageOperations = null;
			updatePackageHelper.UpdateTestPackage();
			
			var actualOperations = action.Operations;
			var expectedOperations = fakePackageManager.PackageOperationsToReturnFromGetInstallPackageOperations;
			
			Assert.AreEqual(expectedOperations, actualOperations);
		}
		
		[Test]
		public void Execute_PackagePassedButNoPackageOperations_PackageOperationsCreatedForPackage()
		{
			CreateSolution();
			updatePackageHelper.PackageOperations = null;
			updatePackageHelper.UpdateTestPackage();
			
			var expectedPackage = updatePackageHelper.TestPackage;
			var actualPackage = fakePackageManager.PackagePassedToGetInstallPackageOperations;
			
			Assert.AreEqual(expectedPackage, actualPackage);
		}
		
		[Test]
		public void Execute_PackageIdAndSourceAndProjectPassedAndUpdateDependenciesIsTrue_DependenciesUpdatedWhenUpdatingPackage()
		{
			CreateSolution();
			
			updatePackageHelper.UpdateDependencies = true;
			updatePackageHelper.UpdatePackageById("PackageId");
			
			bool result = fakePackageManager.UpdateDependenciesPassedToUpdatePackage;
			
			Assert.IsTrue(result);
		}
		
		[Test]
		public void Execute_PackageIdAndSourceAndProjectPassedAndUpdateDependenciesIsFalse_DependenciesNotUpdatedWhenGettingPackageOperations()
		{
			CreateSolution();
			
			updatePackageHelper.UpdateDependencies = false;
			updatePackageHelper.UpdatePackageById("PackageId");
			
			bool result = fakePackageManager.UpdateDependenciesPassedToUpdatePackage;
			
			Assert.IsFalse(result);
		}
		
		[Test]
		public void Execute_UpdatedDepdenciesIsFalseAndNoPackageOperations_DependenciesIgnoredWhenGettingPackageOperations()
		{
			CreateSolution();
			
			updatePackageHelper.UpdateDependencies = false;
			updatePackageHelper.UpdatePackageById("PackageId");
			
			bool result = fakePackageManager.IgnoreDependenciesPassedToGetInstallPackageOperations;
			
			Assert.IsTrue(result);
		}
		
		[Test]
		public void Execute_UpdateDependenciesIsTrueAndNoPackageOperations_DependenciesNotIgnoredWhenGettingPackageOperations()
		{
			CreateSolution();
			
			updatePackageHelper.UpdateDependencies = true;
			updatePackageHelper.UpdatePackageById("PackageId");
			
			bool result = fakePackageManager.IgnoreDependenciesPassedToGetInstallPackageOperations;
			
			Assert.IsFalse(result);
		}
		
		[Test]
		public void Execute_PackageAndPackageOperationsSet_OperationsNotRetrievedFromPackageManager()
		{
			CreateSolution();
			updatePackageHelper.UpdateTestPackage();
			
			var actualPackage = fakePackageManager.PackagePassedToGetInstallPackageOperations;
			
			Assert.IsNull(actualPackage);
		}
	}
}
