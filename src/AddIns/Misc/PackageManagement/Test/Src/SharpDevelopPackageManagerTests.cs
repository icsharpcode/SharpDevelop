// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using ICSharpCode.Core;
using ICSharpCode.PackageManagement;
using ICSharpCode.PackageManagement.Design;
using ICSharpCode.SharpDevelop.Project;
using NuGet;
using NUnit.Framework;
using PackageManagement.Tests.Helpers;

namespace PackageManagement.Tests
{
	[TestFixture]
	public class SharpDevelopPackageManagerTests
	{
		SharpDevelopPackageManager packageManager;
		FakePackageRepository fakeFeedSourceRepository;
		FakeSharedPackageRepository fakeSolutionSharedRepository;
		IProject testProject;
		PackageManagementOptions options;
		PackageRepositoryPaths repositoryPaths;
		PackageReferenceRepositoryHelper packageRefRepositoryHelper;
		TestableProjectManager testableProjectManager;
		FakeFileSystem fakeFileSystem;
		
		void CreatePackageManager(IProject project, PackageReferenceRepositoryHelper packageRefRepositoryHelper)
		{
			options = new PackageManagementOptions(new Properties());
			options.PackagesDirectory = "packages";
			
			repositoryPaths = new PackageRepositoryPaths(project, options);
			var pathResolver = new DefaultPackagePathResolver(repositoryPaths.SolutionPackagesPath);
			
			fakeFileSystem = new FakeFileSystem();
			
			fakeFeedSourceRepository = new FakePackageRepository();
			fakeSolutionSharedRepository = packageRefRepositoryHelper.FakeSharedSourceRepository;
			
			packageManager = new SharpDevelopPackageManager(fakeFeedSourceRepository,
				packageRefRepositoryHelper.FakeProjectSystem,
				fakeFileSystem,
				fakeSolutionSharedRepository,
				pathResolver);
		}
		
		void CreatePackageManager()
		{
			CreateTestProject();
			CreatePackageReferenceRepositoryHelper();
			CreatePackageManager(testProject, packageRefRepositoryHelper);
		}
		
		void CreatePackageReferenceRepositoryHelper()
		{
			packageRefRepositoryHelper = new PackageReferenceRepositoryHelper();
		}
		
		void CreateTestProject()
		{
			testProject = ProjectHelper.CreateTestProject();
		}
		
		void CreateTestableProjectManager()
		{
			testableProjectManager = new TestableProjectManager();
			packageManager.ProjectManager = testableProjectManager;
		}
		
		FakePackage CreateFakePackage()
		{
			var package = new FakePackage();
			package.Id = "Test";
			package.Version = new Version(1, 0, 0, 0);
			return package;
		}
		
		FakePackage InstallPackage()
		{
			FakePackage package = CreateFakePackage();
			packageManager.InstallPackage(package);
			return package;
		}
		
		FakePackage InstallPackageWithNoPackageOperations()
		{
			FakePackage package = CreateFakePackage();
			var operations = new List<PackageOperation>();
			packageManager.InstallPackage(package, operations);
			return package;
		}
		
		FakePackage InstallPackageWithPackageOperations(PackageOperation operation)
		{
			var operations = new PackageOperation[] {
				operation
			};
			FakePackage package = CreateFakePackage();
			packageManager.InstallPackage(package, operations);
			return package;
		}
		
		FakePackage InstallPackageAndIgnoreDependencies()
		{
			FakePackage package = CreateFakePackage();
			packageManager.InstallPackage(package, true);
			return package;
		}
		
		FakePackage InstallPackageAndDoNotIgnoreDependencies()
		{
			FakePackage package = CreateFakePackage();
			packageManager.InstallPackage(package, false);
			return package;
		}
		
		FakePackage UninstallPackage()
		{	
			FakePackage package = CreateFakePackage();
			testableProjectManager.FakeLocalRepository.FakePackages.Add(package);
			
			packageManager.UninstallPackage(package);
			return package;
		}

		FakePackage UninstallPackageAndForceRemove()
		{
			FakePackage package = CreateFakePackage();			
			testableProjectManager.FakeLocalRepository.FakePackages.Add(package);
			
			bool removeDependencies = false;
			bool forceRemove = true;
			packageManager.UninstallPackage(package, forceRemove, removeDependencies);
			
			return package;
		}
		
		FakePackage UninstallPackageAndRemoveDependencies()
		{
			FakePackage package = CreateFakePackage();
			testableProjectManager.FakeLocalRepository.FakePackages.Add(package);
			
			bool removeDependencies = true;
			bool forceRemove = false;
			packageManager.UninstallPackage(package, forceRemove, removeDependencies);
			
			return package;
		}
		
		PackageOperation CreateOneInstallPackageOperation()
		{
			var package = CreateFakePackage();
			package.Id = "PackageToInstall";
			
			return new PackageOperation(package, PackageAction.Install);
		}
		
		FakePackage UpdatePackageWithNoPackageOperations()
		{
			FakePackage package = CreateFakePackage();
			var operations = new List<PackageOperation>();
			packageManager.UpdatePackage(package, operations);
			return package;
		}
		
		FakePackage UpdatePackageWithPackageOperations(PackageOperation operation)
		{
			var operations = new PackageOperation[] {
				operation
			};
			FakePackage package = CreateFakePackage();
			packageManager.UpdatePackage(package, operations);
			return package;
		}

		[Test]
		public void ProjectManager_InstanceCreated_SourceRepositoryIsSharedRepositoryPassedToPackageManager()
		{
			CreatePackageManager();
			Assert.AreEqual(fakeSolutionSharedRepository, packageManager.ProjectManager.SourceRepository);
		}
		
		[Test]
		public void ProjectManager_InstanceCreated_LocalRepositoryIsPackageReferenceRepository()
		{
			CreatePackageManager();
			PackageReferenceRepository packageRefRepository = packageManager.ProjectManager.LocalRepository as PackageReferenceRepository;
			Assert.IsNotNull(packageRefRepository);
		}
		
		[Test]
		public void ProjectManager_InstanceCreated_LocalRepositoryIsRegisteredWithSharedRepository()
		{
			CreateTestProject();
			CreatePackageReferenceRepositoryHelper();
			
			string expectedPath = @"c:\projects\Test\MyProject";
			packageRefRepositoryHelper.FakeProjectSystem.PathToReturnFromGetFullPath = expectedPath;
			
			CreatePackageManager(testProject, packageRefRepositoryHelper);
			
			string actualPath = fakeSolutionSharedRepository.PathPassedToRegisterRepository;
			
			Assert.AreEqual(expectedPath, actualPath);
		}
		
		[Test]
		public void ProjectManager_InstanceCreated_PathResolverIsPackageManagerPathResolver()
		{
			CreatePackageManager();
			
			Assert.AreEqual(packageManager.PathResolver, packageManager.ProjectManager.PathResolver);
		}
		
		[Test]
		public void InstallPackage_PackageInstancePassed_AddsReferenceToProject()
		{
			CreatePackageManager();
			var package = InstallPackage();
			
			Assert.AreEqual(package, testableProjectManager.PackagePassedToAddPackageReference);
		}
		
		[Test]
		public void InstallPackage_PackageInstancePassed_DependenciesNotIgnoredWhenAddingReferenceToProject()
		{
			CreatePackageManager();
			CreateTestableProjectManager();
			InstallPackage();
			
			Assert.IsFalse(testableProjectManager.IgnoreDependenciesPassedToAddPackageReference);
		}
		
		[Test]
		public void InstallPackage_PackageInstanceAndPackageOperationsPassed_AddsReferenceToProject()
		{
			CreatePackageManager();
			CreateTestableProjectManager();
			FakePackage package = InstallPackageWithNoPackageOperations();
				
			Assert.AreEqual(package, testableProjectManager.PackagePassedToAddPackageReference);
		}
		
		[Test]
		public void InstallPackage_PackageInstanceAndPackageOperationsPassed_DoNotIgnoreDependenciesWhenAddingReferenceToProject()
		{
			CreatePackageManager();
			CreateTestableProjectManager();
			InstallPackageWithNoPackageOperations();
				
			Assert.IsFalse(testableProjectManager.IgnoreDependenciesPassedToAddPackageReference);
		}
		
		[Test]
		public void InstallPackage_PackageInstanceAndOneInstallPackageOperationPassed_PackageDefinedInOperationIsInstalledInLocalRepository()
		{
			CreatePackageManager();
			CreateTestableProjectManager();
			
			PackageOperation operation = CreateOneInstallPackageOperation();
			InstallPackageWithPackageOperations(operation);
			
			Assert.AreEqual(operation.Package, fakeSolutionSharedRepository.FirstPackageAdded);
		}
		
		[Test]
		public void InstallPackage_PackageDependenciesIgnored_IgnoreDependenciesPassedToProjectManager()
		{
			CreatePackageManager();
			CreateTestableProjectManager();
			InstallPackageAndIgnoreDependencies();
			
			Assert.IsTrue(testableProjectManager.IgnoreDependenciesPassedToAddPackageReference);
		}
		
		[Test]
		public void InstallPackage_PackageDependenciesIgnored_AddsReferenceToPackage()
		{
			CreatePackageManager();
			CreateTestableProjectManager();
			var package = InstallPackageAndIgnoreDependencies();
			
			Assert.AreEqual(package, testableProjectManager.PackagePassedToAddPackageReference);
		}
		
		[Test]
		public void InstallPackage_PackageDependenciesNotIgnored_IgnoreDependenciesPassedToProjectManager()
		{
			CreatePackageManager();
			CreateTestableProjectManager();
			InstallPackageAndDoNotIgnoreDependencies();
			
			Assert.IsFalse(testableProjectManager.IgnoreDependenciesPassedToAddPackageReference);
		}
		
		[Test]
		public void InstallPackage_PackageDependenciesNotIgnored_AddsReferenceToPackage()
		{
			CreatePackageManager();
			CreateTestableProjectManager();
			var package = InstallPackageAndDoNotIgnoreDependencies();
			
			Assert.AreEqual(package, testableProjectManager.PackagePassedToAddPackageReference);
		}
		
		[Test]
		public void UninstallPackage_PackageInProjectLocalRepository_RemovesReferenceFromProject()
		{
			CreatePackageManager();
			CreateTestableProjectManager();
			var package = UninstallPackage();
			
			Assert.AreEqual(package.Id, testableProjectManager.PackagePassedToRemovePackageReference.Id);
		}
		
		[Test]
		public void UninstallPackage_PackageInProjectLocalRepository_DoesNotRemoveReferenceForcefullyFromProject()
		{
			CreatePackageManager();
			CreateTestableProjectManager();
			UninstallPackage();
			
			Assert.IsFalse(testableProjectManager.ForcePassedToRemovePackageReference);
		}
		
		[Test]
		public void UninstallPackage_PackageInProjectLocalRepository_DependenciesNotRemovedWhenPackageReferenceRemovedFromProject()
		{
			CreatePackageManager();
			CreateTestableProjectManager();
			UninstallPackage();
			
			Assert.IsFalse(testableProjectManager.RemoveDependenciesPassedToRemovePackageReference);
		}
		
		[Test]
		public void UninstallPackage_PassingForceRemove_ReferenceForcefullyRemovedFromProject()
		{
			CreatePackageManager();
			CreateTestableProjectManager();
			
			UninstallPackageAndForceRemove();
			
			Assert.IsTrue(testableProjectManager.ForcePassedToRemovePackageReference);
		}
		
		[Test]
		public void UninstallPackage_PassingRemoveDependencies_DependenciesRemovedWhenPackageReferenceRemovedFromProject()
		{
			CreatePackageManager();
			CreateTestableProjectManager();
			
			UninstallPackageAndRemoveDependencies();
			
			Assert.IsTrue(testableProjectManager.RemoveDependenciesPassedToRemovePackageReference);
		}
		
		[Test]
		public void UninstallPackage_ProjectLocalRepositoryHasPackage_PackageRemovedFromProjectRepositoryBeforeSolutionRepository()
		{
			CreatePackageManager();
			CreateTestableProjectManager();
			
			FakePackage package = CreateFakePackage();
			package.Id = "Test";
			
			testableProjectManager.FakeLocalRepository.FakePackages.Add(package);
			
			IPackage packageRemovedFromProject = null;
			packageManager.PackageUninstalled += (sender, e) => {
				packageRemovedFromProject = testableProjectManager.PackagePassedToRemovePackageReference;
			};
			packageManager.UninstallPackage(package);
			
			Assert.AreEqual("Test", packageRemovedFromProject.Id);
		}
		
		[Test]
		public void UpdatePackage_PackageInstanceAndNoPackageOperationsPassed_UpdatesReferenceInProject()
		{
			CreatePackageManager();
			CreateTestableProjectManager();
			var package = UpdatePackageWithNoPackageOperations();
			
			Assert.AreEqual(package, testableProjectManager.PackagePassedToUpdatePackageReference);
		}
		
		[Test]
		public void UpdatePackage_PackageInstanceAndNoPackageOperationsPassed_UpdatesDependenciesInProject()
		{
			CreatePackageManager();
			CreateTestableProjectManager();
			var package = UpdatePackageWithNoPackageOperations();
			
			Assert.IsTrue(testableProjectManager.UpdateDependenciesPassedToUpdatePackageReference);
		}
		
		[Test]
		public void UpdatePackage_PackageInstanceAndOneInstallPackageOperationPassed_PackageDefinedInOperationIsInstalledInLocalRepository()
		{
			CreatePackageManager();
			CreateTestableProjectManager();
			
			PackageOperation operation = CreateOneInstallPackageOperation();
			UpdatePackageWithPackageOperations(operation);
			
			Assert.AreEqual(operation.Package, fakeSolutionSharedRepository.FirstPackageAdded);
		}
	}
}
