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
	public class PackageManagementProjectTests
	{
		FakePackageManagerFactory fakePackageManagerFactory;
		FakePackageRepository fakeSourceRepository;
		TestableProject fakeMSBuildProject;
		PackageManagementProject project;
		FakeProjectManager fakeProjectManager;
		FakePackageManager fakePackageManager;
		
		void CreateProject()
		{
			fakePackageManagerFactory = new FakePackageManagerFactory();
			fakePackageManager = fakePackageManagerFactory.FakePackageManager;
			fakeProjectManager = fakePackageManager.FakeProjectManager;
			fakeSourceRepository = new FakePackageRepository();
			fakeMSBuildProject = ProjectHelper.CreateTestProject();
			
			project = new PackageManagementProject(fakeSourceRepository, fakeMSBuildProject, fakePackageManagerFactory);
		}
		
		[Test]
		public void IsInstalled_PackageIsInstalled_ReturnsTrue()
		{
			CreateProject();
			fakeProjectManager.IsInstalledReturnValue = true;
			var package = new FakePackage("Test");
			
			bool installed = project.IsInstalled(package);
			
			Assert.IsTrue(installed);
		}
		
		[Test]
		public void IsInstalled_PackageIsNotInstalled_ReturnsFalse()
		{
			CreateProject();
			fakeProjectManager.IsInstalledReturnValue = false;
			var package = new FakePackage("Test");
			
			bool installed = project.IsInstalled(package);
			
			Assert.IsFalse(installed);
		}
		
		[Test]
		public void IsInstalled_PackageIsInstalled_PackagePassedToProjectManager()
		{
			CreateProject();
			fakeProjectManager.IsInstalledReturnValue = false;
			var expectedPackage = new FakePackage("Test");
			
			project.IsInstalled(expectedPackage);
			var actualPackage = fakeProjectManager.PackagePassedToIsInstalled;
			
			Assert.AreEqual(expectedPackage, actualPackage);
		}
		
		[Test]
		public void Constructor_RepositoryAndProjectPassed_RepositoryUsedToCreatePackageManager()
		{
			CreateProject();
			var actualrepository = fakePackageManagerFactory.PackageRepositoryPassedToCreatePackageManager;
			
			Assert.AreEqual(fakeSourceRepository, actualrepository);
		}
		
		[Test]
		public void Constructor_RepositoryAndProjectPassed_ProjecUsedToCreatePackageManager()
		{
			CreateProject();
			var actualProject = fakePackageManagerFactory.ProjectPassedToCreateRepository;
			
			Assert.AreEqual(fakeMSBuildProject, actualProject);
		}
		
		[Test]
		public void GetPackages_ProjectManagerLocalRepositoryHasTwoPackages_ReturnsTwoPackages()
		{
			CreateProject();
			var repository = fakeProjectManager.FakeLocalRepository;
			var packageA = repository.AddFakePackage("A");
			var packageB = repository.AddFakePackage("B");
			
			var actualPackages = project.GetPackages();
			
			var expectedPackages = new FakePackage[] {
				packageA,
				packageB
			};
			
			PackageCollectionAssert.AreEqual(expectedPackages, actualPackages);
		}
		
		[Test]
		public void GetInstallPackageOperations_IgnoreDependenciesIsTrue_DependenciesIgnoredWhenRetrievingPackageOperations()
		{
			CreateProject();
			var package = new FakePackage();
			
			project.GetInstallPackageOperations(package, true);
			
			Assert.IsTrue(fakePackageManager.IgnoreDependenciesPassedToGetInstallPackageOperations);
		}
		
		[Test]
		public void GetInstallPackageOperations_IgnoreDependenciesIsFalse_DependenciesNotIgnoredWhenRetrievingPackageOperations()
		{
			CreateProject();
			var package = new FakePackage();
			
			project.GetInstallPackageOperations(package, false);
			
			Assert.IsFalse(fakePackageManager.IgnoreDependenciesPassedToGetInstallPackageOperations);
		}
		
		[Test]
		public void GetInstallPackageOperations_PackagePassed_PackageUsedToRetrievePackageOperations()
		{
			CreateProject();
			var expectedPackage = new FakePackage();
			
			project.GetInstallPackageOperations(expectedPackage, true);
			
			var actualPackage = fakePackageManager.PackagePassedToGetInstallPackageOperations;
			
			Assert.AreEqual(expectedPackage, actualPackage);
		}
		
		[Test]
		public void GetInstallPackageOperations_PackagePassed_ReturnsPackageOperations()
		{
			CreateProject();
			var package = new FakePackage();
			
			var operations = project.GetInstallPackageOperations(package, true);
			
			var expectedOperations = fakePackageManager.PackageOperationsToReturnFromGetInstallPackageOperations;
			
			Assert.AreEqual(expectedOperations, operations);
		}
		
		[Test]
		public void Logger_SetLogger_LoggerOnPackageManagerIsSet()
		{
			CreateProject();
			var expectedLogger = new FakeLogger();
			
			project.Logger = expectedLogger;
			
			Assert.AreEqual(expectedLogger, fakePackageManager.Logger);
		}
		
		[Test]
		public void Logger_GetLogger_LoggerOnPackageManagerIsReturned()
		{
			CreateProject();
			
			var logger = project.Logger;
			var expectedLogger = fakePackageManager.Logger;
			
			Assert.AreEqual(expectedLogger, logger);
		}
		
		[Test]
		public void InstallPackage_PackagePassed_PackageInstalled()
		{
			CreateProject();
			var package = new FakePackage();
			
			project.InstallPackage(package, null, true);
			
			var expectedPackage = fakePackageManager.PackagePassedToInstallPackage;
			
			Assert.AreEqual(expectedPackage, package);
		}
		
		[Test]
		public void InstallPackage_IgnoreDependenciesIsTrue_DependenciesAreIgnoredWhenPackageIsInstalled()
		{
			CreateProject();
			project.InstallPackage(null, null, true);
			
			Assert.IsTrue(fakePackageManager.IgnoreDependenciesPassedToInstallPackage);
		}
		
		[Test]
		public void InstallPackage_IgnoreDependenciesIsFalse_DependenciesAreNotIgnoredWhenPackageIsInstalled()
		{
			CreateProject();
			project.InstallPackage(null, null, false);
			
			Assert.IsFalse(fakePackageManager.IgnoreDependenciesPassedToInstallPackage);
		}
		
		[Test]
		public void InstallPackage_PackageOperationsPassed_PackageOperationsUsedToInstallPackage()
		{
			CreateProject();
			var expectedOperations = new List<PackageOperation>();
			project.InstallPackage(null, expectedOperations, false);
			
			var actualOperations = fakePackageManager.PackageOperationsPassedToInstallPackage;
			
			Assert.AreEqual(expectedOperations, actualOperations);
		}
		
		[Test]
		public void SourceRepository_NewInstance_ReturnsRepositoryUsedToCreateInstance()
		{
			CreateProject();
			var repository = project.SourceRepository;
			
			Assert.AreEqual(fakeSourceRepository, repository);
		}
		
		[Test]
		public void UninstallPackage_PackagePassed_PackageUninstalled()
		{
			CreateProject();
			var package = new FakePackage();
			
			project.UninstallPackage(package, true, true);
			
			var expectedPackage = fakePackageManager.PackagePassedToUninstallPackage;
			
			Assert.AreEqual(expectedPackage, package);
		}
		
		[Test]
		public void UninstallPackage_ForceRemoveIsTrue_PackageUninstallIsForced()
		{
			CreateProject();
			project.UninstallPackage(null, forceRemove: true, removeDependencies: false);
			
			Assert.IsTrue(fakePackageManager.ForceRemovePassedToUninstallPackage);
		}
		
		[Test]
		public void UninstallPackage_ForceRemoveIsFalse_PackageUninstallIsNotForced()
		{
			CreateProject();
			project.UninstallPackage(null, forceRemove: false, removeDependencies: true);
			
			Assert.IsFalse(fakePackageManager.ForceRemovePassedToUninstallPackage);
		}
		
		[Test]
		public void UninstallPackage_RemoveDependenciesIsTrue_PackageDependenciesIsRemoved()
		{
			CreateProject();
			project.UninstallPackage(null, forceRemove: false, removeDependencies: true);
			
			Assert.IsTrue(fakePackageManager.RemoveDependenciesPassedToUninstallPackage);
		}
		
		[Test]
		public void UninstallPackage_RemoveDependenciesIsFalse_PackageDependenciesNotRemoved()
		{
			CreateProject();
			project.UninstallPackage(null, forceRemove: true, removeDependencies: false);
			
			Assert.IsFalse(fakePackageManager.RemoveDependenciesPassedToUninstallPackage);
		}
		
		[Test]
		public void UpdatePackage_PackagePassed_PackageUpdated()
		{
			CreateProject();
			var package = new FakePackage();
			
			project.UpdatePackage(package, null, true);
			
			var expectedPackage = fakePackageManager.PackagePassedToUpdatePackage;
			
			Assert.AreEqual(expectedPackage, package);
		}
		
		[Test]
		public void UpdatePackage_UpdateDependenciesIsTrue_DependenciesUpdatedWhenPackageIsUpdated()
		{
			CreateProject();
			project.UpdatePackage(null, null, true);
			
			Assert.IsTrue(fakePackageManager.UpdateDependenciesPassedToUpdatePackage);
		}
		
		[Test]
		public void UpdatePackage_UpdateDependenciesIsFalse_DependenciesAreNotUpdatedWhenPackageIsUpdated()
		{
			CreateProject();
			project.UpdatePackage(null, null, false);
			
			Assert.IsFalse(fakePackageManager.UpdateDependenciesPassedToUpdatePackage);
		}
		
		[Test]
		public void UpdatePackage_PackageOperationsPassed_PackageOperationsUsedToUpdatePackage()
		{
			CreateProject();
			var expectedOperations = new List<PackageOperation>();
			project.UpdatePackage(null, expectedOperations, false);
			
			var actualOperations = fakePackageManager.PackageOperationsPassedToUpdatePackage;
			
			Assert.AreEqual(expectedOperations, actualOperations);
		}
		
		[Test]
		public void Logger_SetLogger_ProjectManagerUsesLogger()
		{
			CreateProject();
			ILogger expectedLogger = new NullLogger();
			project.Logger = expectedLogger;
			ILogger actualLogger = fakePackageManager.ProjectManager.Logger;
			
			Assert.AreEqual(expectedLogger, actualLogger);
		}
		
		[Test]
		public void Logger_SetLogger_ProjectManagerProjectSystemUsesLogger()
		{
			CreateProject();
			ILogger expectedLogger = new NullLogger();
			project.Logger = expectedLogger;
			ILogger actualLogger = fakePackageManager.ProjectManager.Project.Logger;
			
			Assert.AreEqual(expectedLogger, actualLogger);
		}
		
		[Test]
		public void Logger_SetLogger_PackageManagerFileSystemUsesLogger()
		{
			CreateProject();
			ILogger expectedLogger = new NullLogger();
			project.Logger = expectedLogger;
			ILogger actualLogger = fakePackageManager.FileSystem.Logger;
			
			Assert.AreEqual(expectedLogger, actualLogger);
		}
		
		[Test]
		public void Logger_GetLogger_ReturnsLogger()
		{
			CreateProject();
			ILogger expectedLogger = new NullLogger();
			project.Logger = expectedLogger;
			ILogger actualLogger = project.Logger;
			
			Assert.AreEqual(expectedLogger, actualLogger);
		}
	}
}
