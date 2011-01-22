// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
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
		
		void CreatePackageManager(IProject project, PackageReferenceRepositoryHelper packageRefRepositoryHelper)
		{
			options = new PackageManagementOptions(new Properties());
			options.PackagesDirectory = "packages";
			
			repositoryPaths = new PackageRepositoryPaths(project, options);
			
			fakeFeedSourceRepository = new FakePackageRepository();
			fakeSolutionSharedRepository = packageRefRepositoryHelper.FakeSharedSourceRepository;
			
			packageManager = new SharpDevelopPackageManager(fakeFeedSourceRepository,
				packageRefRepositoryHelper.FakeProjectSystem,
				fakeSolutionSharedRepository,
				repositoryPaths);
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
		}
		
		FakePackage CreateFakePackage()
		{
			var package = new FakePackage();
			package.Id = "Test";
			package.Version = new Version(1, 0, 0, 0);
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
			CreateTestableProjectManager();
			
			packageManager.ProjectManager = testableProjectManager;
			FakePackage package = CreateFakePackage();
			packageManager.InstallPackage(package);
			
			Assert.AreEqual(package, testableProjectManager.PackagePassedToAddPackageReference);
			Assert.IsFalse(testableProjectManager.IgnoreDependenciesPassedToAddPackageReference);
		}
		
		[Test]
		public void InstallPackage_PackageDependenciesNotIgnored_IgnoreDependenciesPassedToProjectManager()
		{
			CreatePackageManager();
			CreateTestableProjectManager();
			
			packageManager.ProjectManager = testableProjectManager;
			FakePackage package = CreateFakePackage();
			packageManager.InstallPackage(package, true);
			
			Assert.IsTrue(testableProjectManager.IgnoreDependenciesPassedToAddPackageReference);
			Assert.AreEqual(package, testableProjectManager.PackagePassedToAddPackageReference);
		}
		
		[Test]
		public void InstallPackage_PackageDependenciesIgnored_IgnoreDependenciesPassedToProjectManager()
		{
			CreatePackageManager();
			CreateTestableProjectManager();
			
			packageManager.ProjectManager = testableProjectManager;
			FakePackage package = CreateFakePackage();
			packageManager.InstallPackage(package, false);
			
			Assert.IsFalse(testableProjectManager.IgnoreDependenciesPassedToAddPackageReference);
			Assert.AreEqual(package, testableProjectManager.PackagePassedToAddPackageReference);
		}
		
		[Test]
		public void UninstallPackage_PackageInProjectLocalRepository_RemovesReferenceFromProject()
		{
			CreatePackageManager();
			CreateTestableProjectManager();
			
			packageManager.ProjectManager = testableProjectManager;
			FakePackage package = CreateFakePackage();
			package.Id = "Test";
			
			testableProjectManager.FakeLocalRepository.FakePackages.Add(package);
			
			packageManager.UninstallPackage(package);
			
			Assert.AreEqual(package.Id, testableProjectManager.PackagePassedToRemovePackageReference.Id);
			Assert.IsFalse(testableProjectManager.ForcePassedToRemovePackageReference);
			Assert.IsFalse(testableProjectManager.RemoveDependenciesPassedToRemovePackageReference);
		}
		
		[Test]
		public void UninstallPackage_PassingForceRemoveAndRemoveDependencyFlags_FlagsPassedToProjectManager()
		{
			CreatePackageManager();
			CreateTestableProjectManager();
			
			packageManager.ProjectManager = testableProjectManager;
			FakePackage package = CreateFakePackage();
			package.Id = "Test";
			
			testableProjectManager.FakeLocalRepository.FakePackages.Add(package);
			bool removeDependencies = true;
			bool forceRemove = true;
			packageManager.UninstallPackage(package, forceRemove, removeDependencies);
			
			Assert.IsTrue(testableProjectManager.ForcePassedToRemovePackageReference);
			Assert.IsTrue(testableProjectManager.RemoveDependenciesPassedToRemovePackageReference);
		}
		
		[Test]
		public void UninstallPackage_ProjectLocalRepositoryHasPackage_PackageRemovedFromProjectRepositoryBeforeSolutionRepository()
		{
			CreatePackageManager();
			CreateTestableProjectManager();
			
			packageManager.ProjectManager = testableProjectManager;
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
	}
}
