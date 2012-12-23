// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using ICSharpCode.PackageManagement;
using ICSharpCode.PackageManagement.Design;
using NuGet;
using NUnit.Framework;
using PackageManagement.Tests.Helpers;

namespace PackageManagement.Tests
{
	[TestFixture]
	public class UpdatePackageActionTests
	{
		UpdatePackageAction action;
		FakePackageManagementEvents fakePackageManagementEvents;
		FakePackageManagementProject fakeProject;
		UpdatePackageHelper updatePackageHelper;
		
		void CreateSolution()
		{
			fakePackageManagementEvents = new FakePackageManagementEvents();
			fakeProject = new FakePackageManagementProject();
			action = new UpdatePackageAction(fakeProject, fakePackageManagementEvents);
			updatePackageHelper = new UpdatePackageHelper(action);
		}
		
		void AddInstallOperationWithFile(string fileName)
		{
			var package = new FakePackage();
			package.AddFile(fileName);
			
			var operation = new PackageOperation(package, PackageAction.Install);
			var operations = new List<PackageOperation>();
			operations.Add(operation);
			
			action.Operations = operations;
		}
		
		[Test]
		public void Execute_PackageAndRepositoryPassed_PackageIsUpdated()
		{
			CreateSolution();
			updatePackageHelper.UpdateTestPackage();
			
			FakePackage expectedPackage = updatePackageHelper.TestPackage;
			IPackage actualPackage = fakeProject.PackagePassedToUpdatePackage;
			
			Assert.AreEqual(expectedPackage, actualPackage);
		}
		
		[Test]
		public void UpdateDependencies_DefaultValue_ReturnsTrue()
		{
			CreateSolution();
			Assert.IsTrue(action.UpdateDependencies);
		}
		
		[Test]
		public void AllowPrereleaseVersions_DefaultValue_ReturnsFalse()
		{
			CreateSolution();
			Assert.IsFalse(action.AllowPrereleaseVersions);
		}
		
		[Test]
		public void Execute_PackageAndRepositoryPassed_PackageOperationsUsedToUpdatePackage()
		{
			CreateSolution();
			updatePackageHelper.UpdateTestPackage();
			
			IEnumerable<PackageOperation> expectedOperations = updatePackageHelper.PackageOperations;
			IEnumerable<PackageOperation> actualOperations = fakeProject.PackageOperationsPassedToUpdatePackage;
			
			Assert.AreEqual(expectedOperations, actualOperations);
		}
		
		[Test]
		public void Execute_PackageAndRepositoryPassed_DependenciesUpdated()
		{
			CreateSolution();
			updatePackageHelper.UpdateDependencies = true;
			updatePackageHelper.UpdateTestPackage();
			
			bool result = fakeProject.UpdateDependenciesPassedToUpdatePackage;
			
			Assert.IsTrue(result);
		}
		
		[Test]
		public void Execute_PackageAndRepositoryPassed_PrereleaseVersionsNotAllowed()
		{
			CreateSolution();
			updatePackageHelper.AllowPrereleaseVersions = false;
			updatePackageHelper.UpdateTestPackage();
			
			bool result = fakeProject.AllowPrereleaseVersionsPassedToUpdatePackage;
			
			Assert.IsFalse(result);
		}
		
		[Test]
		public void Execute_PackageAndRepositoryPassedAndAllowPrereleaseVersions_PrereleaseVersionsAllowed()
		{
			CreateSolution();
			updatePackageHelper.AllowPrereleaseVersions = true;
			updatePackageHelper.UpdateTestPackage();
			
			bool result = fakeProject.AllowPrereleaseVersionsPassedToUpdatePackage;
			
			Assert.IsTrue(result);
		}
		
		[Test]
		public void Execute_PackageAndRepositoryPassedAndUpdateDependenciesIsFalse_DependenciesNotUpdated()
		{
			CreateSolution();
			updatePackageHelper.UpdateDependencies = false;
			updatePackageHelper.UpdateTestPackage();
			
			bool result = fakeProject.UpdateDependenciesPassedToUpdatePackage;
			
			Assert.IsFalse(result);
		}
		
		[Test]
		public void Execute_PackageAndRepositoryPassed_PackageInstalledEventIsFired()
		{
			CreateSolution();
			updatePackageHelper.UpdateTestPackage();
			
			FakePackage expectedPackage = updatePackageHelper.TestPackage;
			IPackage actualPackage = fakePackageManagementEvents.PackagePassedToOnParentPackageInstalled;
			Assert.AreEqual(expectedPackage, actualPackage);
		}
		
		[Test]
		public void Execute_PackagePassedButNoPackageOperations_PackageOperationsRetrievedFromProject()
		{
			CreateSolution();
			updatePackageHelper.PackageOperations = null;
			updatePackageHelper.UpdateTestPackage();
			
			IEnumerable<PackageOperation> actualOperations = action.Operations;
			List<FakePackageOperation> expectedOperations = fakeProject.FakeInstallOperations;
			
			Assert.AreEqual(expectedOperations, actualOperations);
		}
		
		[Test]
		public void Execute_PackagePassedButNoPackageOperations_PackageOperationsCreatedForPackage()
		{
			CreateSolution();
			updatePackageHelper.PackageOperations = null;
			updatePackageHelper.UpdateTestPackage();
			
			var expectedPackage = updatePackageHelper.TestPackage;
			var actualPackage = fakeProject.PackagePassedToGetInstallPackageOperations;
			
			Assert.AreEqual(expectedPackage, actualPackage);
		}
		
		[Test]
		public void Execute_PackageIdAndSourceAndProjectPassedAndUpdateDependenciesIsTrue_DependenciesUpdatedWhenUpdatingPackage()
		{
			CreateSolution();
			fakeProject.AddFakePackageToSourceRepository("PackageId");
			updatePackageHelper.UpdateDependencies = true;
			updatePackageHelper.UpdatePackageById("PackageId");
			
			bool result = fakeProject.UpdateDependenciesPassedToUpdatePackage;
			
			Assert.IsTrue(result);
		}
		
		[Test]
		public void Execute_PackageIdAndSourceAndProjectPassedAndUpdateDependenciesIsFalse_DependenciesNotUpdatedWhenGettingPackageOperations()
		{
			CreateSolution();
			fakeProject.AddFakePackageToSourceRepository("PackageId");
			updatePackageHelper.UpdateDependencies = false;
			updatePackageHelper.UpdatePackageById("PackageId");
			
			bool result = fakeProject.UpdateDependenciesPassedToUpdatePackage;
			
			Assert.IsFalse(result);
		}
		
		[Test]
		public void Execute_UpdatedDepdenciesIsFalseAndNoPackageOperations_DependenciesIgnoredWhenGettingPackageOperations()
		{
			CreateSolution();
			fakeProject.AddFakePackageToSourceRepository("PackageId");
			updatePackageHelper.UpdateDependencies = false;
			updatePackageHelper.UpdatePackageById("PackageId");
			
			bool result = fakeProject.IgnoreDependenciesPassedToGetInstallPackageOperations;
			
			Assert.IsTrue(result);
		}
		
		[Test]
		public void Execute_UpdateDependenciesIsTrueAndNoPackageOperations_DependenciesNotIgnoredWhenGettingPackageOperations()
		{
			CreateSolution();
			fakeProject.AddFakePackageToSourceRepository("PackageId");
			updatePackageHelper.UpdateDependencies = true;
			updatePackageHelper.UpdatePackageById("PackageId");
			
			bool result = fakeProject.IgnoreDependenciesPassedToGetInstallPackageOperations;
			
			Assert.IsFalse(result);
		}
		
		[Test]
		public void Execute_AllowPrereleaseVersionsIsFalseAndNoPackageOperations_PrereleaseVersionsNotAllowedWhenGettingPackageOperations()
		{
			CreateSolution();
			fakeProject.AddFakePackageToSourceRepository("PackageId");
			updatePackageHelper.AllowPrereleaseVersions = false;
			updatePackageHelper.UpdatePackageById("PackageId");
			
			bool result = fakeProject.AllowPrereleaseVersionsPassedToGetInstallPackageOperations;
			
			Assert.IsFalse(result);
		}
		
		[Test]
		public void Execute_AllowPrereleaseVersionsIsTrueAndNoPackageOperations_PrereleaseVersionsAllowedWhenGettingPackageOperations()
		{
			CreateSolution();
			fakeProject.AddFakePackageToSourceRepository("PackageId");
			updatePackageHelper.AllowPrereleaseVersions = true;
			updatePackageHelper.UpdatePackageById("PackageId");
			
			bool result = fakeProject.AllowPrereleaseVersionsPassedToGetInstallPackageOperations;
			
			Assert.IsTrue(result);
		}
		
		[Test]
		public void Execute_PackageAndPackageOperationsSet_OperationsNotRetrievedFromPackageManager()
		{
			CreateSolution();
			updatePackageHelper.UpdateTestPackage();
			
			IPackage actualPackage = fakeProject.PackagePassedToGetInstallPackageOperations;
			
			Assert.IsNull(actualPackage);
		}
	
		[Test]
		public void HasPackageScriptsToRun_OnePackageInOperationsHasInitPowerShellScript_ReturnsTrue()
		{
			CreateSolution();
			fakeProject.AddFakePackageToSourceRepository("Test");
			action.PackageId = "Test";
			AddInstallOperationWithFile(@"tools\init.ps1");
			
			bool hasPackageScripts = action.HasPackageScriptsToRun();
			
			Assert.IsTrue(hasPackageScripts);
		}
		
		[Test]
		public void HasPackageScriptsToRun_OnePackageInOperationsHasNoFiles_ReturnsFalse()
		{
			CreateSolution();
			fakeProject.AddFakePackageToSourceRepository("Test");
			action.PackageId = "Test";
			action.Operations = new List<PackageOperation>();
			
			bool hasPackageScripts = action.HasPackageScriptsToRun();
			
			Assert.IsFalse(hasPackageScripts);
		}
		
		[Test]
		public void UpdateIfPackageDoesNotExistInProject_NewUpdateActionInstanceCreate_ReturnsTrue()
		{
			CreateSolution();
			bool update = action.UpdateIfPackageDoesNotExistInProject;
			
			Assert.IsTrue(update);
		}
		
		[Test]
		public void Execute_UpdateIfPackageDoesNotExistInProjectSetToFalseAndPackageDoesNotExistInProject_PackageIsNotUpdated()
		{
			CreateSolution();
			action.Package = new FakePackage("Test");
			action.UpdateIfPackageDoesNotExistInProject = false;
			action.Execute();
			
			bool updated = fakeProject.IsUpdatePackageCalled;
			
			Assert.IsFalse(updated);
		}

		[Test]
		public void Execute_UpdateIfPackageDoesNotExistInProjectSetToFalseAndPackageDoesNotExistInProject_PackageInstalledEventIsNotFired()
		{
			CreateSolution();
			action.UpdateIfPackageDoesNotExistInProject = false;
			updatePackageHelper.UpdateTestPackage();
			
			bool updated = fakePackageManagementEvents.IsOnParentPackageInstalledCalled;
			
			Assert.IsFalse(updated);
		}
		
		[Test]
		public void Execute_UpdateIfPackageDoesNotExistInProjectSetToFalseAndPackageExistsInProject_PackageIsUpdated()
		{
			CreateSolution();
			action.UpdateIfPackageDoesNotExistInProject = false;
			action.PackageId = "Test";
			FakePackage expectedPackage = fakeProject.FakeSourceRepository.AddFakePackageWithVersion("Test", "1.1");
			fakeProject.FakePackages.Add(new FakePackage("Test", "1.0"));
			action.Execute();
			
			IPackage actualPackage = fakeProject.PackagePassedToUpdatePackage;
			
			Assert.AreEqual(expectedPackage, actualPackage);
		}
		
		[Test]
		public void Execute_PackagePassedAndUpdateIfPackageDoesNotExistInProjectSetToFalseAndPackageExistsInProject_PackageIsUpdated()
		{
			CreateSolution();
			action.UpdateIfPackageDoesNotExistInProject = false;
			var expectedPackage = new FakePackage("Test");
			action.Package = expectedPackage;
			fakeProject.FakePackages.Add(new FakePackage("Test", "1.0"));
			action.Execute();
			
			IPackage actualPackage = fakeProject.PackagePassedToUpdatePackage;
			
			Assert.AreEqual(expectedPackage, actualPackage);
		}
	}
}
