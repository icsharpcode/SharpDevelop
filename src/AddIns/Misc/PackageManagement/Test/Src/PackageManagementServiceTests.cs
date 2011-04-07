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
		OneRegisteredPackageSourceHelper packageSourcesHelper;
		FakePackageRepositoryFactory fakePackageRepositoryFactory;
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
			fakePackageRepositoryFactory = new FakePackageRepositoryFactory();
			fakePackageManagerFactory = new FakePackageManagerFactory();
			fakeProjectService = new FakePackageManagementProjectService();
			fakeOutputMessagesView = new FakePackageManagementOutputMessagesView();
			fakeProjectService.CurrentProject = testProject;
			packageManagementService = 
				new PackageManagementService(options,
					fakePackageRepositoryFactory,
					fakePackageManagerFactory,
					fakeProjectService,
					fakeOutputMessagesView);
		}
		
		FakePackage AddOneFakePackageToPackageRepositoryFactoryRepository(string id)
		{
			return fakePackageRepositoryFactory.FakePackageRepository.AddOneFakePackage(id);
		}
		
		void MakePackageManagementSourceRepositoryAndPackageRepositoryFactoryRepositoryTheSame()
		{
			fakePackageManagerFactory.FakePackageManager.SourceRepository = 
				fakePackageRepositoryFactory.FakePackageRepository;
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
		public void ActivePackageRepository_OneRegisteredSource_RepositoryCreatedFromRegisteredSource()
		{
			CreatePackageManagementService();
			IPackageRepository activeRepository = packageManagementService.ActivePackageRepository;
			
			PackageSource actualPackageSource = fakePackageRepositoryFactory.FirstPackageSourcePassedToCreateRepository;
			
			Assert.AreEqual(packageSourcesHelper.PackageSource, actualPackageSource);
		}
		
		[Test]
		public void ActivePackageRepository_CalledTwice_RepositoryCreatedOnce()
		{
			CreatePackageManagementService();
			IPackageRepository activeRepository = packageManagementService.ActivePackageRepository;
			activeRepository = packageManagementService.ActivePackageRepository;
			
			int count = fakePackageRepositoryFactory.PackageSourcesPassedToCreateRepository.Count;
			
			Assert.AreEqual(1, count);
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
		public void HasMultiplePackageSources_OnePackageSource_ReturnsFalse()
		{
			CreatePackageManagementService();
			packageSourcesHelper.AddOnePackageSource();
			
			bool result = packageManagementService.HasMultiplePackageSources;
			Assert.IsFalse(result);
		}
		
		[Test]
		public void HasMultiplePackageSources_TwoPackageSources_ReturnsTrue()
		{
			CreatePackageManagementService();
			packageSourcesHelper.AddTwoPackageSources();
			
			bool result = packageManagementService.HasMultiplePackageSources;
			Assert.IsTrue(result);
		}
		
		[Test]
		public void ActivePackageSource_TwoPackageSources_ByDefaultReturnsFirstPackageSource()
		{
			CreatePackageManagementService();
			packageSourcesHelper.AddTwoPackageSources();
			
			var expectedPackageSource = packageManagementService.Options.PackageSources[0];
			var packageSource = packageManagementService.ActivePackageSource;
			
			Assert.AreEqual(expectedPackageSource, packageSource);
		}
		
		[Test]
		public void ActivePackageSource_ChangedToSecondRegisteredPackageSources_ReturnsSecondPackageSource()
		{
			CreatePackageManagementService();
			packageSourcesHelper.AddTwoPackageSources();
			
			var expectedPackageSource = packageManagementService.Options.PackageSources[1];
			packageManagementService.ActivePackageSource = expectedPackageSource;
			var packageSource = packageManagementService.ActivePackageSource;
			
			Assert.AreEqual(expectedPackageSource, packageSource);
		}
		
		[Test]
		public void ActivePackageRepository_ActivePackageSourceChangedToSecondRegisteredPackageSource_CreatesRepositoryUsingSecondPackageSource()
		{
			CreatePackageManagementService();
			packageSourcesHelper.AddTwoPackageSources();
			
			var expectedPackageSource = packageManagementService.Options.PackageSources[1];
			packageManagementService.ActivePackageSource = expectedPackageSource;
			
			IPackageRepository repository = packageManagementService.ActivePackageRepository;
			var packageSource = fakePackageRepositoryFactory.FirstPackageSourcePassedToCreateRepository;
			
			Assert.AreEqual(expectedPackageSource, packageSource);
		}
		
		[Test]
		public void ActivePackageRepository_ActivePackageSourceChangedAfterActivePackageRepositoryCreated_CreatesNewRepositoryUsingActivePackageSource()
		{
			CreatePackageManagementService();
			packageSourcesHelper.AddTwoPackageSources();
			
			IPackageRepository initialRepository = packageManagementService.ActivePackageRepository;
			fakePackageRepositoryFactory.PackageSourcesPassedToCreateRepository.Clear();
			
			var expectedPackageSource = packageManagementService.Options.PackageSources[1];
			packageManagementService.ActivePackageSource = expectedPackageSource;
			
			IPackageRepository repository = packageManagementService.ActivePackageRepository;
			var packageSource = fakePackageRepositoryFactory.FirstPackageSourcePassedToCreateRepository;
			
			Assert.AreEqual(expectedPackageSource, packageSource);
		}
		
		[Test]
		public void ActivePackageRepository_ActivePackageSourceSetToSameValueAfterActivePackageRepositoryCreated_NewRepositoryNotCreated()
		{
			CreatePackageManagementService();
			packageSourcesHelper.AddOnePackageSource();
			
			IPackageRepository initialRepository = packageManagementService.ActivePackageRepository;
			fakePackageRepositoryFactory.PackageSourcesPassedToCreateRepository.Clear();
			
			var expectedPackageSource = packageManagementService.Options.PackageSources[0];
			packageManagementService.ActivePackageSource = expectedPackageSource;
			
			IPackageRepository repository = packageManagementService.ActivePackageRepository;
			
			Assert.AreEqual(0, fakePackageRepositoryFactory.PackageSourcesPassedToCreateRepository.Count);
		}
		
		[Test]
		public void ActivePackageSource_ChangedToNonNullPackageSource_SavedInOptions()
		{
			CreatePackageManagementService();
			
			packageManagementService.Options.ActivePackageSource = null;
			
			var packageSource = new PackageSource("http://source-url", "Test");
			packageManagementService.Options.PackageSources.Add(packageSource);
			packageManagementService.ActivePackageSource = packageSource;
			
			Assert.AreEqual(packageSource, packageManagementService.Options.ActivePackageSource);
		}
		
		[Test]
		public void ActivePackageSource_ActivePackageSourceNonNullInOptionsBeforeInstanceCreate_ActivePackageSourceReadFromOptions()
		{
			CreatePackageSources();
			var packageSource = new PackageSource("http://source-url", "Test");
			packageSourcesHelper.Options.PackageSources.Add(packageSource);
			packageSourcesHelper.Options.ActivePackageSource = packageSource;
			CreatePackageManagementService(packageSourcesHelper.Options);
			
			Assert.AreEqual(packageSource, packageManagementService.ActivePackageSource);
		}
		
		[Test]
		public void CreateAggregatePackageRepository_TwoRegisteredPackageRepositories_ReturnsPackagesFromBothRepositories()
		{
			CreatePackageSources();
			packageSourcesHelper.AddTwoPackageSources();
			CreatePackageManagementService(packageSourcesHelper.Options);
			
			var repository1Package = new FakePackage("One");
			var repository1 = new FakePackageRepository();
			repository1.FakePackages.Add(repository1Package);
			
			fakePackageRepositoryFactory.FakePackageRepositories.Add(packageSourcesHelper.RegisteredPackageSources[0], repository1);
			
			var repository2Package = new FakePackage("Two");
			var repository2 = new FakePackageRepository();
			repository2.FakePackages.Add(repository2Package);
			
			fakePackageRepositoryFactory.FakePackageRepositories.Add(packageSourcesHelper.RegisteredPackageSources[1], repository2);
			
			IPackageRepository repository = packageManagementService.CreateAggregatePackageRepository();
			IQueryable<IPackage> packages = repository.GetPackages().OrderBy(x => x.Id);
			
			var expectedPackages = new FakePackage[] {
				repository1Package,
				repository2Package
			};
			
			CollectionAssert.AreEqual(expectedPackages, packages);
		}
	
		[Test]
		public void CreatePackageRepository_PackageSourceSpecified_CreatesPackageRepositoryFromPackageRepositoryFactory()
		{
			CreatePackageManagementService();
			var repository = packageManagementService.CreatePackageRepository(new PackageSource("a"));
			var expectedRepository = fakePackageRepositoryFactory.FakePackageRepository;
			
			Assert.AreEqual(expectedRepository, repository);
		}
		
		[Test]
		public void CreatePackageRepository_PackageSourceSpecified_PackageSourcePassedToPackageRepositoryFactory()
		{
			CreatePackageManagementService();
			var source = new PackageSource("Test");
			var repository = packageManagementService.CreatePackageRepository(source);
			var actualSource = fakePackageRepositoryFactory.PackageSourcesPassedToCreateRepository.First();
			
			Assert.AreEqual(source, actualSource);
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
			
			var expectedRepository = packageManagementService.ActivePackageRepository;
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
			
			var actualPackageSource = fakePackageRepositoryFactory.FirstPackageSourcePassedToCreateRepository;
			
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
			var expectedRepository = fakePackageRepositoryFactory.FakePackageRepository;
			
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
			
			var recentPackages = packageManagementService.RecentPackageRepository.GetPackages();
			
			var expectedPackages = new FakePackage[] {
				package
			};
			
			PackageCollectionAssert.AreEqual(expectedPackages, recentPackages);
		}
	}
}
