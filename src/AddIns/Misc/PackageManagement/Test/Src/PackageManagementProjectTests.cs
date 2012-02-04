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
		FakeInstallPackageAction fakeInstallAction;
		FakeUninstallPackageAction fakeUninstallAction;
		FakeUpdatePackageAction fakeUpdateAction;
		
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
		
		FakeInstallPackageAction CreateFakeInstallAction()
		{
			fakeInstallAction = new FakeInstallPackageAction();
			return fakeInstallAction;
		}
		
		FakeUninstallPackageAction CreateFakeUninstallAction()
		{
			fakeUninstallAction = new FakeUninstallPackageAction(project);
			return fakeUninstallAction;
		}
		
		FakeUpdatePackageAction CreateFakeUpdateAction()
		{
			fakeUpdateAction = new FakeUpdatePackageAction(project);
			return fakeUpdateAction;
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
			CreateFakeInstallAction()
				.IgnoreDependencies = true;
			
			project.GetInstallPackageOperations(package, fakeInstallAction);
			
			Assert.IsTrue(fakePackageManager.IgnoreDependenciesPassedToGetInstallPackageOperations);
		}
		
		[Test]
		public void GetInstallPackageOperations_IgnoreDependenciesIsFalse_DependenciesNotIgnoredWhenRetrievingPackageOperations()
		{
			CreateProject();
			var package = new FakePackage();
			CreateFakeInstallAction()
				.IgnoreDependencies = false;
			
			project.GetInstallPackageOperations(package, fakeInstallAction);
			
			Assert.IsFalse(fakePackageManager.IgnoreDependenciesPassedToGetInstallPackageOperations);
		}
		
		[Test]
		public void GetInstallPackageOperations_PackagePassed_PackageUsedToRetrievePackageOperations()
		{
			CreateProject();
			var expectedPackage = new FakePackage();
			CreateFakeInstallAction()
				.IgnoreDependencies = true;

			project.GetInstallPackageOperations(expectedPackage, fakeInstallAction);
			
			IPackage actualPackage = fakePackageManager.PackagePassedToGetInstallPackageOperations;
			
			Assert.AreEqual(expectedPackage, actualPackage);
		}
		
		[Test]
		public void GetInstallPackageOperations_PackagePassed_ReturnsPackageOperations()
		{
			CreateProject();
			var package = new FakePackage();
			CreateFakeInstallAction()
				.IgnoreDependencies = true;

			IEnumerable<PackageOperation> operations = project.GetInstallPackageOperations(package, fakeInstallAction);
			
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
			CreateFakeInstallAction()
				.Package = package;
			
			project.InstallPackage(package, fakeInstallAction);
			
			IPackage expectedPackage = fakePackageManager.PackagePassedToInstallPackage;
			
			Assert.AreEqual(expectedPackage, package);
		}
		
		[Test]
		public void InstallPackage_IgnoreDependenciesIsTrue_DependenciesAreIgnoredWhenPackageIsInstalled()
		{
			CreateProject();
			CreateFakeInstallAction()
				.IgnoreDependencies = true;
			project.InstallPackage(null, fakeInstallAction);
			
			Assert.IsTrue(fakePackageManager.IgnoreDependenciesPassedToInstallPackage);
		}
		
		[Test]
		public void InstallPackage_IgnoreDependenciesIsFalse_DependenciesAreNotIgnoredWhenPackageIsInstalled()
		{
			CreateProject();
			CreateFakeInstallAction()
				.IgnoreDependencies = false;
			project.InstallPackage(null, fakeInstallAction);
			
			Assert.IsFalse(fakePackageManager.IgnoreDependenciesPassedToInstallPackage);
		}
		
		[Test]
		public void InstallPackage_PackageOperationsPassed_PackageOperationsUsedToInstallPackage()
		{
			CreateProject();
			var expectedOperations = new List<PackageOperation>();
			CreateFakeInstallAction()
				.Operations = expectedOperations;
			project.InstallPackage(null, fakeInstallAction);
			
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
			CreateFakeUninstallAction();
			fakeUninstallAction.ForceRemove = true;
			fakeUninstallAction.RemoveDependencies = true;
			var package = new FakePackage();
			
			project.UninstallPackage(package, fakeUninstallAction);
			
			IPackage expectedPackage = fakePackageManager.PackagePassedToUninstallPackage;
			
			Assert.AreEqual(expectedPackage, package);
		}
		
		[Test]
		public void UninstallPackage_ForceRemoveIsTrue_PackageUninstallIsForced()
		{
			CreateProject();
			CreateFakeUninstallAction();
			fakeUninstallAction.ForceRemove = true;
			fakeUninstallAction.RemoveDependencies = false;
			
			project.UninstallPackage(null, fakeUninstallAction);
			
			Assert.IsTrue(fakePackageManager.ForceRemovePassedToUninstallPackage);
		}
		
		[Test]
		public void UninstallPackage_ForceRemoveIsFalse_PackageUninstallIsNotForced()
		{
			CreateProject();
			CreateFakeUninstallAction();
			fakeUninstallAction.ForceRemove = false;
			fakeUninstallAction.RemoveDependencies = true;
			
			project.UninstallPackage(null, fakeUninstallAction);
			
			Assert.IsFalse(fakePackageManager.ForceRemovePassedToUninstallPackage);
		}
		
		[Test]
		public void UninstallPackage_RemoveDependenciesIsTrue_PackageDependenciesIsRemoved()
		{
			CreateProject();
			CreateFakeUninstallAction();
			fakeUninstallAction.ForceRemove = false;
			fakeUninstallAction.RemoveDependencies = true;

			project.UninstallPackage(null, fakeUninstallAction);
			
			Assert.IsTrue(fakePackageManager.RemoveDependenciesPassedToUninstallPackage);
		}
		
		[Test]
		public void UninstallPackage_RemoveDependenciesIsFalse_PackageDependenciesNotRemoved()
		{
			CreateProject();
			CreateFakeUninstallAction();
			fakeUninstallAction.ForceRemove = true;
			fakeUninstallAction.RemoveDependencies = false;

			project.UninstallPackage(null, fakeUninstallAction);
			
			Assert.IsFalse(fakePackageManager.RemoveDependenciesPassedToUninstallPackage);
		}
		
		[Test]
		public void UpdatePackage_PackagePassed_PackageUpdated()
		{
			CreateProject();
			var package = new FakePackage();
			CreateFakeUpdateAction()
				.UpdateDependencies = true;
			
			project.UpdatePackage(package, fakeUpdateAction);
			
			IPackage expectedPackage = fakePackageManager.PackagePassedToUpdatePackage;
			
			Assert.AreEqual(expectedPackage, package);
		}
		
		[Test]
		public void UpdatePackage_UpdateDependenciesIsTrue_DependenciesUpdatedWhenPackageIsUpdated()
		{
			CreateProject();
			CreateFakeUpdateAction()
				.UpdateDependencies = true;
			
			project.UpdatePackage(null, fakeUpdateAction);
			
			Assert.IsTrue(fakePackageManager.UpdateDependenciesPassedToUpdatePackage);
		}
		
		[Test]
		public void UpdatePackage_UpdateDependenciesIsFalse_DependenciesAreNotUpdatedWhenPackageIsUpdated()
		{
			CreateProject();
			CreateFakeUpdateAction()
				.UpdateDependencies = false;

			project.UpdatePackage(null, fakeUpdateAction);
			
			Assert.IsFalse(fakePackageManager.UpdateDependenciesPassedToUpdatePackage);
		}
		
		[Test]
		public void UpdatePackage_PackageOperationsPassed_PackageOperationsUsedToUpdatePackage()
		{
			CreateProject();
			var expectedOperations = new List<PackageOperation>();
			CreateFakeUpdateAction()
				.Operations = expectedOperations;

			project.UpdatePackage(null, fakeUpdateAction);
			
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
		
		[Test]
		public void InstallPackage_AllowPrereleaseVersionsIsTrue_PrereleaseVersionsAreNotAllowedWhenPackageIsInstalled()
		{
			CreateProject();
			CreateFakeInstallAction()
				.AllowPrereleaseVersions = false;
			project.InstallPackage(null, fakeInstallAction);
			
			Assert.IsFalse(fakePackageManager.AllowPrereleaseVersionsPassedToInstallPackage);
		}
		
		[Test]
		public void InstallPackage_AllowPrereleaseVersionsIsFalse_PrereleaseVersionsAreAllowedWhenPackageIsInstalled()
		{
			CreateProject();
			CreateFakeInstallAction()
				.AllowPrereleaseVersions = true;
			project.InstallPackage(null, fakeInstallAction);
			
			Assert.IsTrue(fakePackageManager.AllowPrereleaseVersionsPassedToInstallPackage);
		}
		
		[Test]
		public void GetInstallPackageOperations_AllowPrereleaseVersionsIsTrue_PrereleaseVersionsAllowedWhenRetrievingPackageOperations()
		{
			CreateProject();
			var package = new FakePackage();
			CreateFakeInstallAction()
				.AllowPrereleaseVersions = true;
			
			project.GetInstallPackageOperations(package, fakeInstallAction);
			
			Assert.IsTrue(fakePackageManager.AllowPrereleaseVersionsPassedToGetInstallPackageOperations);
		}
		
		[Test]
		public void GetInstallPackageOperations_AllowPrereleaseVersionsIsFalse_PrereleaseVersionsNotAllowedWhenRetrievingPackageOperations()
		{
			CreateProject();
			var package = new FakePackage();
			CreateFakeInstallAction()
				.AllowPrereleaseVersions = false;

			project.GetInstallPackageOperations(package, fakeInstallAction);
			
			Assert.IsFalse(fakePackageManager.AllowPrereleaseVersionsPassedToGetInstallPackageOperations);
		}
		
		[Test]
		public void UpdatePackage_AllowPrereleaseVersionsIsTrue_PrereleaseVersionsNotAllowedWhenPackageIsUpdated()
		{
			CreateProject();
			CreateFakeUpdateAction()
				.AllowPrereleaseVersions = true;

			project.UpdatePackage(null, fakeUpdateAction);
			
			Assert.IsTrue(fakePackageManager.AllowPrereleaseVersionsPassedToInstallPackage);
		}
		
		[Test]
		public void UpdatePackage_AllowPrereleaseVersionsIsFalse_PrereleaseVersionsNotAllowedWhenPackageIsUpdated()
		{
			CreateProject();
			CreateFakeUpdateAction()
				.AllowPrereleaseVersions = false;

			project.UpdatePackage(null, fakeUpdateAction);
			
			Assert.IsFalse(fakePackageManager.AllowPrereleaseVersionsPassedToInstallPackage);
		}
	}
}
