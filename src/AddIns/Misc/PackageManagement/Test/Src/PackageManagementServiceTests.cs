// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Linq;

using ICSharpCode.PackageManagement;
using ICSharpCode.PackageManagement.Design;
using ICSharpCode.SharpDevelop.Project;
using NuGet;
using NUnit.Framework;
using PackageManagement.Tests.Helpers;

namespace PackageManagement.Tests
{
	[TestFixture]
	public class PackageManagementServiceTests
	{
		PackageManagementService packageManagementService;
		FakeRegisteredPackageRepositories fakeRegisteredPackageRepositories;
		OneRegisteredPackageSourceHelper packageSourcesHelper;
		FakePackageManagerFactory fakePackageManagerFactory;
		FakePackageManagementProjectService fakeProjectService;
		TestableProject testProject;
		FakePackageManagementOutputMessagesView fakeOutputMessagesView;
		
		void CreatePackageSources()
		{
			packageSourcesHelper = new OneRegisteredPackageSourceHelper();
		}
		
		void CreatePackageManagementService()
		{
			CreatePackageSources();
			CreatePackageManagementService(packageSourcesHelper.Options);
		}
		
		void CreatePackageManagementService(PackageManagementOptions options)
		{
			testProject = ProjectHelper.CreateTestProject();
			fakeRegisteredPackageRepositories = new FakeRegisteredPackageRepositories();
			fakePackageManagerFactory = new FakePackageManagerFactory();
			fakeProjectService = new FakePackageManagementProjectService();
			fakeOutputMessagesView = new FakePackageManagementOutputMessagesView();
			
			fakeProjectService.CurrentProject = testProject;
			packageManagementService = 
				new PackageManagementService(options,
					fakeRegisteredPackageRepositories,
					fakePackageManagerFactory,
					fakeProjectService,
					fakeOutputMessagesView);
		}
		
		FakePackage AddOneFakePackageToPackageRepositoryFactoryRepository(string id)
		{
			return fakeRegisteredPackageRepositories.FakePackageRepository.AddFakePackage(id);
		}
		
		void MakePackageManagementSourceRepositoryAndPackageRepositoryFactoryRepositoryTheSame()
		{
			fakePackageManagerFactory.FakePackageManager.SourceRepository = 
				fakeRegisteredPackageRepositories.FakePackageRepository;
		}
		
		IEnumerable<PackageOperation> AddOnePackageOperationToPackageManager()
		{		
			var operations = fakePackageManagerFactory
				.FakePackageManager
				.PackageOperationsToReturnFromGetInstallPackageOperations;
			
			var operation = new PackageOperation(new FakePackage("A"), PackageAction.Install);
			operations.Add(operation);
			
			return operations;
		}
		
		[Test]
		public void ActiveProjectManager_ProjectIsSelected_ReferencesSelectedProject()
		{
			CreatePackageManagementService();
			
			IProjectManager activeProjectManager = packageManagementService.ActiveProjectManager;
			IProject actualProject = fakePackageManagerFactory.ProjectPassedToCreateRepository;
			
			Assert.AreEqual(testProject, actualProject);
		}
		
		[Test]
		public void OnParentPackageInstalled_PackagePassed_ParentPackageInstalledEventFires()
		{
			CreatePackageManagementService();
			
			bool fired = false;
			packageManagementService.ParentPackageInstalled += (sender, e) => {
				fired = true;
			};
			var package = new FakePackage();
			packageManagementService.OnParentPackageInstalled(package);
			
			Assert.IsTrue(fired);
		}
		
		[Test]
		public void OnParentPackageUninstalled_PackagePassed_OnParentPackageUninstalledEventFires()
		{
			CreatePackageManagementService();
			
			bool fired = false;
			packageManagementService.ParentPackageUninstalled += (sender, e) => {
				fired = true;
			};
			var package = new FakePackage();
			packageManagementService.OnParentPackageUninstalled(package);
			
			Assert.IsTrue(fired);
		}
		
		[Test]
		public void CreateProjectManager_RepositoryAndProjectSpecified_CreatesPackageManagerFromPackageManagerFactory()
		{
			CreatePackageManagementService();
			var repository = new FakePackageRepository();
			var project = ProjectHelper.CreateTestProject();
			
			ISharpDevelopProjectManager projectManager = packageManagementService.CreateProjectManager(repository, project);
			
			var expectedProjectManager = fakePackageManagerFactory.FakePackageManager.FakeProjectManager;
			Assert.AreEqual(expectedProjectManager, projectManager);
		}
		
		[Test]
		public void CreateProjectManager_RepositorySpecified_RepositoryUsedToCreateProjectManager()
		{
			CreatePackageManagementService();
			var repository = new FakePackageRepository();
			
			packageManagementService.CreateProjectManager(repository, null);
			
			var expectedRepository = fakePackageManagerFactory.PackageRepositoryPassedToCreatePackageManager;
			
			Assert.AreEqual(expectedRepository, repository);
		}
		
		[Test]
		public void CreateProjectManager_ProjectSpecified_ProjectUsedToCreateProjectManager()
		{
			CreatePackageManagementService();
			var project = ProjectHelper.CreateTestProject();
			
			packageManagementService.CreateProjectManager(null, project);
			
			var expectedProject = fakePackageManagerFactory.ProjectPassedToCreateRepository;
			
			Assert.AreEqual(expectedProject, project);
		}		
		
		[Test]
		public void CreatePackageManagerForActiveProject_ProjectIsSelected_ReferencesSelectedProject()
		{
			CreatePackageManagementService();
			
			packageManagementService.CreatePackageManagerForActiveProject();
			
			IProject expectedProject = fakeProjectService.CurrentProject;
			IProject actualProject = fakePackageManagerFactory.ProjectPassedToCreateRepository;
			
			Assert.AreEqual(expectedProject, actualProject);
		}
		
		[Test]
		public void CreatePackageManagerForActiveProject_PackageRepositoryPassed_CreatesPackageManagerWithCurrentlyActiveProject()
		{
			CreatePackageManagementService();
			
			var repository = new FakePackageRepository();
			packageManagementService.CreatePackageManagerForActiveProject(repository);
			
			IProject expectedProject = fakeProjectService.CurrentProject;
			IProject actualProject = fakePackageManagerFactory.ProjectPassedToCreateRepository;
			
			Assert.AreEqual(expectedProject, actualProject);
		}
		
		[Test]
		public void CreatePackageManagerForActiveProject_PackageRepositoryPassed_PackageManagerCreatedWithRepository()
		{
			CreatePackageManagementService();
			
			var repository = new FakePackageRepository();
			packageManagementService.CreatePackageManagerForActiveProject(repository);
			
			var actualRepository = fakePackageManagerFactory.PackageRepositoryPassedToCreatePackageManager;
			
			Assert.AreEqual(repository, actualRepository);
		}
		
		[Test]
		public void CreatePackageManagerForActiveProject_ProjectIsSelected_UsesActiveRepository()
		{
			CreatePackageManagementService();
			
			packageManagementService.CreatePackageManagerForActiveProject();
			
			var expectedRepository = fakeRegisteredPackageRepositories.ActiveRepository;
			var actualRepository = fakePackageManagerFactory.PackageRepositoryPassedToCreatePackageManager;
			
			Assert.AreEqual(expectedRepository, actualRepository);
		}
		
		[Test]
		public void CreatePackageManagerForActiveProject_ProjectIsSelected_ReturnsPackageManager()
		{
			CreatePackageManagementService();
			
			ISharpDevelopPackageManager packageManager = 
				packageManagementService.CreatePackageManagerForActiveProject();
			
			var expectedPackageManager = fakePackageManagerFactory.FakePackageManager;
			
			Assert.AreEqual(expectedPackageManager, packageManager);
		}
		
		[Test]
		public void CreatePackageManager_PackageSourceAndProjectPassed_ReturnsNewPackageManager()
		{
			CreatePackageManagementService();
			
			var packageSource = new PackageSource("test");
			ISharpDevelopPackageManager packageManager = 
				packageManagementService.CreatePackageManager(packageSource, testProject);
			
			var expectedPackageManager = fakePackageManagerFactory.FakePackageManager;
			
			Assert.AreEqual(expectedPackageManager, packageManager);
		}
		
		[Test]
		public void CreatePackageManager_PackageSourceAndProjectPassed_PackageSourceUsedToCreateRepository()
		{
			CreatePackageManagementService();
			
			var packageSource = new PackageSource("test");
			ISharpDevelopPackageManager packageManager = 
				packageManagementService.CreatePackageManager(packageSource, testProject);
			
			var actualPackageSource = fakeRegisteredPackageRepositories.PackageSourcePassedToCreateRepository;
			
			Assert.AreEqual(packageSource, actualPackageSource);
		}
		
		[Test]
		public void CreatePackageManager_PackageSourceAndProjectPassed_CreatedRepositoryUsedToCreatePackageManager()
		{
			CreatePackageManagementService();
			
			var packageSource = new PackageSource("test");
			ISharpDevelopPackageManager packageManager = 
				packageManagementService.CreatePackageManager(packageSource, testProject);
			
			var actualRepository = fakePackageManagerFactory.PackageRepositoryPassedToCreatePackageManager;
			var expectedRepository = fakeRegisteredPackageRepositories.FakePackageRepository;
			
			Assert.AreEqual(expectedRepository, actualRepository);
		}
		
		[Test]
		public void CreatePackageManager_PackageSourceAndProjectPassed_ProjectUsedToCreatePackageManager()
		{
			CreatePackageManagementService();
			
			var packageSource = new PackageSource("test");
			ISharpDevelopPackageManager packageManager = 
				packageManagementService.CreatePackageManager(packageSource, testProject);
			
			var actualProject = fakePackageManagerFactory.ProjectPassedToCreateRepository;
			
			Assert.AreEqual(testProject, actualProject);
		}
		
		[Test]
		public void OnParentPackageUninstalled_PackagePassed_RefreshesProjectBrowserWindow()
		{
			CreatePackageManagementService();
			var package = new FakePackage("Test");
			packageManagementService.OnParentPackageUninstalled(package);
			
			Assert.IsTrue(fakeProjectService.IsRefreshProjectBrowserCalled);
		}
		
		[Test]
		public void OnParentPackageInstalled_PackagePassed_RefreshesProjectBrowserWindow()
		{
			CreatePackageManagementService();
			var package = new FakePackage("Test");
			packageManagementService.OnParentPackageInstalled(package);
			
			Assert.IsTrue(fakeProjectService.IsRefreshProjectBrowserCalled);
		}
		
		[Test]
		public void OnParentPackageInstalled_PackagePassed_PackageInstalledIsAddedToRecentPackagesRepository()
		{
			CreatePackageManagementService();
			var package = new FakePackage("Test");
			packageManagementService.OnParentPackageInstalled(package);
			
			var recentPackages = fakeRegisteredPackageRepositories.FakeRecentPackageRepository.PackagesAdded;
			
			var expectedPackages = new FakePackage[] {
				package
			};
			
			PackageCollectionAssert.AreEqual(expectedPackages, recentPackages);
		}
	}
}
