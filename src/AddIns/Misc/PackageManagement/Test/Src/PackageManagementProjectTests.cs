// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Linq;

using ICSharpCode.PackageManagement;
using ICSharpCode.PackageManagement.Design;
using ICSharpCode.PackageManagement.EnvDTE;
using ICSharpCode.SharpDevelop.Project;
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
		FakePackageManagementEvents fakePackageManagementEvents;
		
		void CreateProject()
		{
			fakePackageManagerFactory = new FakePackageManagerFactory();
			fakePackageManager = fakePackageManagerFactory.FakePackageManager;
			fakeProjectManager = fakePackageManager.FakeProjectManager;
			fakeSourceRepository = new FakePackageRepository();
			fakeMSBuildProject = ProjectHelper.CreateTestProject();
			fakePackageManagementEvents = new FakePackageManagementEvents();
			
			project = new PackageManagementProject(
				fakeSourceRepository,
				fakeMSBuildProject,
				fakePackageManagementEvents,
				fakePackageManagerFactory);
		}
		
		[Test]
		public void IsInstalled_PackageIsInstalled_ReturnsTrue()
		{
			CreateProject();
			fakeProjectManager.IsInstalledReturnValue = true;
			var package = new FakePackage("Test");
			
			bool installed = project.IsPackageInstalled(package);
			
			Assert.IsTrue(installed);
		}
		
		[Test]
		public void IsInstalled_PackageIsNotInstalled_ReturnsFalse()
		{
			CreateProject();
			fakeProjectManager.IsInstalledReturnValue = false;
			var package = new FakePackage("Test");
			
			bool installed = project.IsPackageInstalled(package);
			
			Assert.IsFalse(installed);
		}
		
		[Test]
		public void IsInstalled_PackageIsInstalled_PackagePassedToProjectManager()
		{
			CreateProject();
			fakeProjectManager.IsInstalledReturnValue = false;
			var expectedPackage = new FakePackage("Test");
			
			project.IsPackageInstalled(expectedPackage);
			IPackage actualPackage = fakeProjectManager.PackagePassedToIsInstalled;
			
			Assert.AreEqual(expectedPackage, actualPackage);
		}
		
		[Test]
		public void Constructor_RepositoryAndProjectPassed_RepositoryUsedToCreatePackageManager()
		{
			CreateProject();
			IPackageRepository actualrepository = fakePackageManagerFactory.PackageRepositoryPassedToCreatePackageManager;
			
			Assert.AreEqual(fakeSourceRepository, actualrepository);
		}
		
		[Test]
		public void Constructor_RepositoryAndProjectPassed_ProjecUsedToCreatePackageManager()
		{
			CreateProject();
			MSBuildBasedProject actualProject = fakePackageManagerFactory.ProjectPassedToCreateRepository;
			
			Assert.AreEqual(fakeMSBuildProject, actualProject);
		}
		
		[Test]
		public void GetPackages_ProjectManagerLocalRepositoryHasTwoPackages_ReturnsTwoPackages()
		{
			CreateProject();
			FakePackageRepository repository = fakeProjectManager.FakeLocalRepository;
			FakePackage packageA = repository.AddFakePackage("A");
			FakePackage packageB = repository.AddFakePackage("B");
			
			IQueryable<IPackage> actualPackages = project.GetPackages();
			
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
			
			project.GetInstallPackageOperations(package, true, false);
			
			Assert.IsTrue(fakePackageManager.IgnoreDependenciesPassedToGetInstallPackageOperations);
		}
		
		[Test]
		public void GetInstallPackageOperations_IgnoreDependenciesIsFalse_DependenciesNotIgnoredWhenRetrievingPackageOperations()
		{
			CreateProject();
			var package = new FakePackage();
			
			project.GetInstallPackageOperations(package, false, false);
			
			Assert.IsFalse(fakePackageManager.IgnoreDependenciesPassedToGetInstallPackageOperations);
		}
		
		[Test]
		public void GetInstallPackageOperations_PackagePassed_PackageUsedToRetrievePackageOperations()
		{
			CreateProject();
			var expectedPackage = new FakePackage();
			
			project.GetInstallPackageOperations(expectedPackage, true, false);
			
			IPackage actualPackage = fakePackageManager.PackagePassedToGetInstallPackageOperations;
			
			Assert.AreEqual(expectedPackage, actualPackage);
		}
		
		[Test]
		public void GetInstallPackageOperations_PackagePassed_ReturnsPackageOperations()
		{
			CreateProject();
			var package = new FakePackage();
			
			IEnumerable<PackageOperation> operations = project.GetInstallPackageOperations(package, true, false);
			
			IEnumerable<PackageOperation> expectedOperations = fakePackageManager.PackageOperationsToReturnFromGetInstallPackageOperations;
			
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
			
			ILogger logger = project.Logger;
			ILogger expectedLogger = fakePackageManager.Logger;
			
			Assert.AreEqual(expectedLogger, logger);
		}
		
		[Test]
		public void InstallPackage_PackagePassed_PackageInstalled()
		{
			CreateProject();
			var package = new FakePackage();
			
			project.InstallPackage(package, null, true, false);
			
			IPackage expectedPackage = fakePackageManager.PackagePassedToInstallPackage;
			
			Assert.AreEqual(expectedPackage, package);
		}
		
		[Test]
		public void InstallPackage_IgnoreDependenciesIsTrue_DependenciesAreIgnoredWhenPackageIsInstalled()
		{
			CreateProject();
			project.InstallPackage(null, null, true, false);
			
			Assert.IsTrue(fakePackageManager.IgnoreDependenciesPassedToInstallPackage);
		}
		
		[Test]
		public void InstallPackage_IgnoreDependenciesIsFalse_DependenciesAreNotIgnoredWhenPackageIsInstalled()
		{
			CreateProject();
			project.InstallPackage(null, null, false, false);
			
			Assert.IsFalse(fakePackageManager.IgnoreDependenciesPassedToInstallPackage);
		}
		
		[Test]
		public void InstallPackage_PackageOperationsPassed_PackageOperationsUsedToInstallPackage()
		{
			CreateProject();
			var expectedOperations = new List<PackageOperation>();
			project.InstallPackage(null, expectedOperations, false, false);
			
			IEnumerable<PackageOperation> actualOperations = fakePackageManager.PackageOperationsPassedToInstallPackage;
			
			Assert.AreEqual(expectedOperations, actualOperations);
		}
		
		[Test]
		public void SourceRepository_NewInstance_ReturnsRepositoryUsedToCreateInstance()
		{
			CreateProject();
			IPackageRepository repository = project.SourceRepository;
			
			Assert.AreEqual(fakeSourceRepository, repository);
		}
		
		[Test]
		public void UninstallPackage_PackagePassed_PackageUninstalled()
		{
			CreateProject();
			var package = new FakePackage();
			
			project.UninstallPackage(package, true, true);
			
			IPackage expectedPackage = fakePackageManager.PackagePassedToUninstallPackage;
			
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
			
			project.UpdatePackage(package, null, true, false);
			
			IPackage expectedPackage = fakePackageManager.PackagePassedToUpdatePackage;
			
			Assert.AreEqual(expectedPackage, package);
		}
		
		[Test]
		public void UpdatePackage_UpdateDependenciesIsTrue_DependenciesUpdatedWhenPackageIsUpdated()
		{
			CreateProject();
			project.UpdatePackage(null, null, true, false);
			
			Assert.IsTrue(fakePackageManager.UpdateDependenciesPassedToUpdatePackage);
		}
		
		[Test]
		public void UpdatePackage_UpdateDependenciesIsFalse_DependenciesAreNotUpdatedWhenPackageIsUpdated()
		{
			CreateProject();
			project.UpdatePackage(null, null, false, false);
			
			Assert.IsFalse(fakePackageManager.UpdateDependenciesPassedToUpdatePackage);
		}
		
		[Test]
		public void UpdatePackage_PackageOperationsPassed_PackageOperationsUsedToUpdatePackage()
		{
			CreateProject();
			var expectedOperations = new List<PackageOperation>();
			project.UpdatePackage(null, expectedOperations, false, false);
			
			IEnumerable<PackageOperation> actualOperations = fakePackageManager.PackageOperationsPassedToUpdatePackage;
			
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
		
		[Test]
		public void PackageInstalled_PackagerManagerPackageInstalledEventFired_EventFiresWithPackage()
		{
			CreateProject();
			PackageOperationEventArgs eventArgs = null;
			project.PackageInstalled += (sender, e) => eventArgs = e;
			
			var expectedEventArgs = new PackageOperationEventArgs(new FakePackage(), null, String.Empty);
			fakePackageManager.FirePackageInstalled(expectedEventArgs);
			
			Assert.AreEqual(expectedEventArgs, eventArgs);
		}
		
		[Test]
		public void PackageUninstalled_PackagerManagerPackageUninstalledEventFired_EventFiresWithPackage()
		{
			CreateProject();
			PackageOperationEventArgs eventArgs = null;
			project.PackageUninstalled += (sender, e) => eventArgs = e;
			
			var expectedEventArgs = new PackageOperationEventArgs(new FakePackage(), null, String.Empty);
			fakePackageManager.FirePackageUninstalled(expectedEventArgs);
			
			Assert.AreEqual(expectedEventArgs, eventArgs);
		}
		
		[Test]
		public void PackageReferenceAdded_ProjectManagerPackageReferenceAddedEventFired_EventFiresWithPackage()
		{
			CreateProject();
			PackageOperationEventArgs eventArgs = null;
			project.PackageReferenceAdded += (sender, e) => eventArgs = e;
			
			var expectedPackage = new FakePackage();
			fakeProjectManager.FirePackageReferenceAdded(expectedPackage);
			
			Assert.AreEqual(expectedPackage, eventArgs.Package);
		}
		
		[Test]
		public void PackageReferenceRemoved_ProjectManagerPackageReferenceRemovedEventFired_EventFiresWithPackage()
		{
			CreateProject();
			PackageOperationEventArgs eventArgs = null;
			project.PackageReferenceRemoved += (sender, e) => eventArgs = e;
			
			var expectedPackage = new FakePackage();
			fakeProjectManager.FirePackageReferenceRemoved(expectedPackage);
			
			Assert.AreEqual(expectedPackage, eventArgs.Package);
		}
		
		[Test]
		public void ConvertToDTEProject_MethodCalled_ReturnsProjectWithExpectedName()
		{
			CreateProject();
			fakeMSBuildProject.Name = "Test";
			Project dteProject = project.ConvertToDTEProject();
			
			string name = dteProject.Name;
			
			Assert.AreEqual("Test", name);
		}
		
		[Test]
		public void Name_MSBuildProjectNameIsSet_ReturnsMSBuildProjectName()
		{
			CreateProject();
			fakeMSBuildProject.Name = "MyProject";
			
			string name = project.Name;
			
			Assert.AreEqual("MyProject", name);
		}
		
		[Test]
		public void IsInstalled_PackageIdPassedAndPackageIsInstalled_ReturnsTrue()
		{
			CreateProject();
			fakeProjectManager.IsInstalledReturnValue = true;
			
			bool installed = project.IsPackageInstalled("Test");
			
			Assert.IsTrue(installed);
		}
		
		[Test]
		public void IsInstalled_PackageIdPassedAndPackageIsNotInstalled_ReturnsFalse()
		{
			CreateProject();
			fakeProjectManager.IsInstalledReturnValue = false;
			
			bool installed = project.IsPackageInstalled("Test");
			
			Assert.IsFalse(installed);
		}
		
		[Test]
		public void IsInstalled_PackageIdPassedPackageIsInstalled_PackageIdPassedToProjectManager()
		{
			CreateProject();
			fakeProjectManager.IsInstalledReturnValue = false;
			
			project.IsPackageInstalled("Test");
			string id = fakeProjectManager.PackageIdPassedToIsInstalled;
			
			Assert.AreEqual("Test", id);
		}
		
		[Test]
		public void GetPackagesInReverseDependencyOrder_TwoPackages_ReturnsPackagesFromProjectLocalRepositoryInCorrectOrder()
		{
			CreateProject();
			FakePackage packageA = fakeProjectManager.FakeLocalRepository.AddFakePackageWithVersion("A", "1.0");
			FakePackage packageB = fakeProjectManager.FakeLocalRepository.AddFakePackageWithVersion("B", "1.0");
			
			packageB.DependenciesList.Add(new PackageDependency("A"));
			
			var expectedPackages = new FakePackage[] {
				packageB,
				packageA
			};
			
			IEnumerable<IPackage> packages = project.GetPackagesInReverseDependencyOrder();
			
			PackageCollectionAssert.AreEqual(expectedPackages, packages);
		}
	}
}
