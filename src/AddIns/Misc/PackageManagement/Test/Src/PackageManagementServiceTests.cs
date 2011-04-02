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
		InstallPackageHelper installPackageHelper;
		UninstallPackageHelper uninstallPackageHelper;
		UpdatePackageHelper updatePackageHelper;
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

			installPackageHelper = new InstallPackageHelper(packageManagementService);
			uninstallPackageHelper = new UninstallPackageHelper(packageManagementService);
			updatePackageHelper = new UpdatePackageHelper(packageManagementService);
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
		
		TestableProject AddProject(string name)
		{
			var project = ProjectHelper.CreateTestProject(name);
			fakeProjectService.AddFakeProject(project);
			return project;
		}
		
		[Test]
		public void InstallPackage_PackageObjectPassed_CallsPackageManagerInstallPackage()
		{
			CreatePackageManagementService();
			installPackageHelper.InstallTestPackage();
			
			bool expectedIgnoreDependencies = false;
			var expectedInstallPackageParameters = new FakePackageManager.InstallPackageParameters() {
				IgnoreDependenciesPassedToInstallPackage = expectedIgnoreDependencies,
				PackagePassedToInstallPackage = installPackageHelper.TestPackage,
				PackageOperationsPassedToInstallPackage = installPackageHelper.PackageOperations
			};
			
			var actualInstallPackageParameters = fakePackageManagerFactory.FakePackageManager.ParametersPassedToInstallPackage;
			
			Assert.AreEqual(expectedInstallPackageParameters, actualInstallPackageParameters);
		}
		
		[Test]
		public void InstallPackage_PackageAndPackageRepositoryPassed_CreatesPackageManagerWithPackageRepository()
		{
			CreatePackageManagementService();
			installPackageHelper.InstallTestPackage();
			
			IPackageRepository expectedRepository = installPackageHelper.PackageRepository;
			IPackageRepository actualRepository = fakePackageManagerFactory.PackageRepositoryPassedToCreatePackageManager;
			
			Assert.AreEqual(expectedRepository, actualRepository);
		}
		
		[Test]
		public void InstallPackage_PackageAndPackageRepositoryPassed_CreatesPackageManagerWithCurrentlyActiveProject()
		{
			CreatePackageManagementService();
			IProject expectedProject = ProjectHelper.CreateTestProject();
			fakeProjectService.CurrentProject = expectedProject;
			
			installPackageHelper.InstallTestPackage();
			
			IProject actualProject = fakePackageManagerFactory.ProjectPassedToCreateRepository;
			
			Assert.AreEqual(expectedProject, actualProject);
		}
		
		[Test]
		public void InstallPackage_PackageAndPackageRepositoryPassed_RefreshesProjectBrowserWindow()
		{
			CreatePackageManagementService();
			installPackageHelper.InstallTestPackage();
			
			Assert.IsTrue(fakeProjectService.IsRefreshProjectBrowserCalled);
		}
		
		[Test]
		public void InstallPackage_PackageAndPackageRepositoryPassed_RefreshesProjectBrowserWindowAfterInstallingPackage()
		{
			CreatePackageManagementService();
			fakePackageManagerFactory.FakePackageManager.FakeProjectService = fakeProjectService;
			
			installPackageHelper.InstallTestPackage();
			
			bool result = fakePackageManagerFactory.FakePackageManager.IsRefreshProjectBrowserCalledWhenInstallPackageCalled;
			
			Assert.IsFalse(result);
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
		public void UninstallPackage_PackageObjectPassed_CallsPackageManagerUninstallPackageWithPackage()
		{
			CreatePackageManagementService();
			
			uninstallPackageHelper.UninstallTestPackage();
			
			var actualPackage = fakePackageManagerFactory.FakePackageManager.PackagePassedToUninstallPackage;
			var expectedPackage = uninstallPackageHelper.TestPackage;
			
			Assert.AreEqual(expectedPackage, actualPackage);
		}
		
		[Test]
		public void UninstallPackage_PackageObjectPassed_PackageRepositoryUsedToCreatePackageManager()
		{
			CreatePackageManagementService();
			
			uninstallPackageHelper.UninstallTestPackage();
	
			Assert.AreEqual(uninstallPackageHelper.FakePackageRepository, fakePackageManagerFactory.PackageRepositoryPassedToCreatePackageManager);
		}
		
		[Test]
		public void UninstallPackage_PackageObjectPassed_ProjectUsedToCreateRepository()
		{
			CreatePackageManagementService();
			
			uninstallPackageHelper.UninstallTestPackage();
			
			Assert.AreEqual(testProject, fakePackageManagerFactory.ProjectPassedToCreateRepository);
		}
		
		[Test]
		public void UninstallPackage_PackageAndPackageRepositoryPassed_RefreshesProjectBrowserWindow()
		{
			CreatePackageManagementService();

			uninstallPackageHelper.UninstallTestPackage();
			
			Assert.IsTrue(fakeProjectService.IsRefreshProjectBrowserCalled);
		}
		
		[Test]
		public void UninstallPackage_PackageAndPackageRepositoryPassed_RefreshesProjectBrowserWindowAfterUninstallingPackage()
		{
			CreatePackageManagementService();

			fakePackageManagerFactory.FakePackageManager.FakeProjectService = fakeProjectService;
			
			uninstallPackageHelper.UninstallTestPackage();
			
			bool result = fakePackageManagerFactory.FakePackageManager.IsRefreshProjectBrowserCalledWhenUninstallPackageCalled;
			
			Assert.IsFalse(result);
		}
		
		[Test]
		public void PackageInstalled_PackageIsInstalled_EventFiresAfterPackageInstalled()
		{
			CreatePackageManagementService();
			
			IPackage package = null;
			packageManagementService.PackageInstalled += (sender, e) => {
				package = fakePackageManagerFactory.FakePackageManager.PackagePassedToInstallPackage;
			};
			installPackageHelper.InstallTestPackage();
			
			Assert.AreEqual(installPackageHelper.TestPackage, package);
		}
		
		[Test]
		public void PackageUninstalled_PackageIsUninstalled_EventFiresAfterPackageUninstalled()
		{
			CreatePackageManagementService();
			
			IPackage package = null;
			packageManagementService.PackageUninstalled += (sender, e) => {
				package = fakePackageManagerFactory.FakePackageManager.PackagePassedToUninstallPackage;
			};
			
			uninstallPackageHelper.UninstallTestPackage();
			
			var expectedPackage = uninstallPackageHelper.TestPackage;
			
			Assert.AreEqual(expectedPackage, package);
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
		public void InstallPackage_OnePackageOperation_PackageOperationPassedToPackageManagerWhenInstallingPackage()
		{
			CreatePackageManagementService();
			
			installPackageHelper.AddPackageInstallOperation();
			installPackageHelper.InstallTestPackage();
			
			var actualOperations = 
				fakePackageManagerFactory
					.FakePackageManager
					.ParametersPassedToInstallPackage
					.PackageOperationsPassedToInstallPackage;
			
			CollectionAssert.AreEqual(installPackageHelper.PackageOperations, actualOperations);
		}
		
		[Test]
		public void InstallPackage_OnePackageOperation_LoggerUsedByPackageManagerIsOutputMessagesViewLogger()
		{
			CreatePackageManagementService();
			installPackageHelper.AddPackageInstallOperation();
			installPackageHelper.InstallTestPackage();
			
			ILogger expectedLogger = fakeOutputMessagesView;
			ILogger actualLogger = fakePackageManagerFactory.FakePackageManager.Logger;
			
			Assert.AreEqual(expectedLogger, actualLogger);
		}
		
		[Test]
		public void InstallPackage_OnePackageOperation_LoggerUsedByPackageManagerIsConfiguredBeforeInstallPackageCalled()
		{
			CreatePackageManagementService();
			installPackageHelper.AddPackageInstallOperation();
			installPackageHelper.InstallTestPackage();
			
			ILogger expectedLogger = fakeOutputMessagesView;
			ILogger actualLogger = fakePackageManagerFactory.FakePackageManager.LoggerSetBeforeInstallPackageCalled;
			
			Assert.AreEqual(expectedLogger, actualLogger);
		}
		
		[Test]
		public void InstallPackage_OnePackageOperation_ProjectManagerLoggerIsOutputMessagesViewLogger()
		{
			CreatePackageManagementService();
			installPackageHelper.AddPackageInstallOperation();
			installPackageHelper.InstallTestPackage();
			
			ILogger expectedLogger = fakeOutputMessagesView;
			ILogger actualLogger = fakePackageManagerFactory.FakePackageManager.FakeProjectManager.Logger;
			
			Assert.AreEqual(expectedLogger, actualLogger);
		}
		
		[Test]
		public void InstallPackage_OnePackageOperation_ProjectManagerProjectSystemLoggerIsOutputMessagesViewLogger()
		{
			CreatePackageManagementService();
			var projectSystem = new FakeProjectSystem();
			FakeProjectManager projectManager = fakePackageManagerFactory.FakePackageManager.FakeProjectManager;
			projectManager.Project = projectSystem;
			installPackageHelper.AddPackageInstallOperation();
			installPackageHelper.InstallTestPackage();
			
			ILogger expectedLogger = fakeOutputMessagesView;
			ILogger actualLogger = projectSystem.Logger;
			
			Assert.AreEqual(expectedLogger, actualLogger);
		}
		
		[Test]
		public void InstallPackage_OnePackageOperation_PackageManagerFileSystemLoggerIsOutputMessagesViewLogger()
		{
			CreatePackageManagementService();
			installPackageHelper.AddPackageInstallOperation();
			installPackageHelper.InstallTestPackage();
			
			ILogger expectedLogger = fakeOutputMessagesView;
			ILogger actualLogger = fakePackageManagerFactory.FakePackageManager.FileSystem.Logger;
			
			Assert.AreEqual(expectedLogger, actualLogger);
		}
		
		[Test]
		public void InstallPackage_OnePackageOperation_PackageInstalledAddedToRecentPackagesRepository()
		{
			CreatePackageManagementService();
			installPackageHelper.AddPackageInstallOperation();
			installPackageHelper.InstallTestPackage();
			
			var recentPackages = packageManagementService.RecentPackageRepository.GetPackages();
			
			var expectedPackages = new FakePackage[] {
				installPackageHelper.TestPackage
			};
			
			PackageCollectionAssert.AreEqual(expectedPackages, recentPackages);
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
		public void InstallPackage_PackageIdAndSourceAndProjectPassed_RepositoryCreatedForPackageSource()
		{
			CreatePackageManagementService();
			AddOneFakePackageToPackageRepositoryFactoryRepository("PackageId");
			MakePackageManagementSourceRepositoryAndPackageRepositoryFactoryRepositoryTheSame();
			
			installPackageHelper.InstallPackageById("PackageId");
			
			var expectedPackageSource = installPackageHelper.PackageSource;
			var actualPackageSource = fakePackageRepositoryFactory.FirstPackageSourcePassedToCreateRepository;
			
			Assert.AreEqual(expectedPackageSource, actualPackageSource);
		}
		
		[Test]
		public void InstallPackage_PackageIdAndSourceAndProjectPassed_InstallsPackageFromPackageSource()
		{
			CreatePackageManagementService();
			var package = AddOneFakePackageToPackageRepositoryFactoryRepository("PackageId");
			MakePackageManagementSourceRepositoryAndPackageRepositoryFactoryRepositoryTheSame();
			
			installPackageHelper.InstallPackageById("PackageId");
			
			var actualPackage = fakePackageManagerFactory.FakePackageManager.PackagePassedToInstallPackage;
			
			Assert.AreEqual(package, actualPackage);
		}
		
		[Test]
		public void InstallPackage_PackageIdAndSourceAndProjectPassed_RepositoryCreatedFromPackageSourceIsUsedToCreatePackageManager()
		{
			CreatePackageManagementService();
			var package = AddOneFakePackageToPackageRepositoryFactoryRepository("PackageId");
			MakePackageManagementSourceRepositoryAndPackageRepositoryFactoryRepositoryTheSame();
			var repository = fakePackageRepositoryFactory.FakePackageRepository;
			
			installPackageHelper.InstallPackageById("PackageId");
			
			var actualRepository = fakePackageManagerFactory.PackageRepositoryPassedToCreatePackageManager;
			
			Assert.AreEqual(repository, actualRepository);
		}
		
		[Test]
		public void InstallPackage_PackageIdAndSourceAndProjectPassed_ProjectIsUsedToCreatePackageManager()
		{
			CreatePackageManagementService();
			fakeProjectService.CurrentProject = null;
			AddOneFakePackageToPackageRepositoryFactoryRepository("PackageId");
			MakePackageManagementSourceRepositoryAndPackageRepositoryFactoryRepositoryTheSame();
			
			installPackageHelper.InstallPackageById("PackageId");
			
			var actualProject = fakePackageManagerFactory.ProjectPassedToCreateRepository;
			var expectedProject = installPackageHelper.TestableProject;
			
			Assert.AreEqual(expectedProject, actualProject);
		}
		
		[Test]
		public void InstallPackage_PackageIdAndSourceAndProjectPassed_PackageOperationsToInstallPackage()
		{
			CreatePackageManagementService();
			AddOneFakePackageToPackageRepositoryFactoryRepository("PackageId");
			MakePackageManagementSourceRepositoryAndPackageRepositoryFactoryRepositoryTheSame();
			var packageOperations = AddOnePackageOperationToPackageManager();
			
			installPackageHelper.InstallPackageById("PackageId");
			
			var packageManager = fakePackageManagerFactory.FakePackageManager;
			var actualPackageOperations = packageManager.ParametersPassedToInstallPackage.PackageOperationsPassedToInstallPackage;
			
			Assert.AreEqual(packageOperations, actualPackageOperations);
		}
		
		[Test]
		public void InstallPackage_PackageIdAndSourceAndProjectPassed_PackageOperationsCreatedForInstallPackage()
		{
			CreatePackageManagementService();
			var package = AddOneFakePackageToPackageRepositoryFactoryRepository("PackageId");
			MakePackageManagementSourceRepositoryAndPackageRepositoryFactoryRepositoryTheSame();
			var packageOperations = AddOnePackageOperationToPackageManager();
			
			installPackageHelper.InstallPackageById("PackageId");
			
			var actualPackage = fakePackageManagerFactory.FakePackageManager.PackagePassedToGetInstallPackageOperations;
			
			Assert.AreEqual(package, actualPackage);
		}
		
		[Test]
		public void InstallPackage_PackageIdAndSourceAndProjectPassedAndIgnoreDependenciesIsTrue_DependenciesIgnoredWhenInstallingPackage()
		{
			CreatePackageManagementService();
			var package = AddOneFakePackageToPackageRepositoryFactoryRepository("PackageId");
			MakePackageManagementSourceRepositoryAndPackageRepositoryFactoryRepositoryTheSame();
			
			installPackageHelper.IgnoreDependencies = true;
			installPackageHelper.InstallPackageById("PackageId");
			
			bool result = fakePackageManagerFactory.FakePackageManager.IgnoreDependenciesPassedToInstallPackage;
			
			Assert.IsTrue(result);
		}
		
		[Test]
		public void InstallPackage_PackageIdAndSourceAndProjectPassedAndIgnoreDependenciesIsTrue_DependenciesIgnoredWhenGettingPackageOperations()
		{
			CreatePackageManagementService();
			var package = AddOneFakePackageToPackageRepositoryFactoryRepository("PackageId");
			MakePackageManagementSourceRepositoryAndPackageRepositoryFactoryRepositoryTheSame();
			
			installPackageHelper.IgnoreDependencies = true;
			installPackageHelper.InstallPackageById("PackageId");
			
			bool result = fakePackageManagerFactory.FakePackageManager.IgnoreDependenciesPassedToGetInstallPackageOperations;
			
			Assert.IsTrue(result);
		}
		
		[Test]
		public void InstallPackage_PackageIdAndSourceAndProjectPassedAndIgnoreDependenciesIsFalse_DependenciesNotIgnoredWhenGettingPackageOperations()
		{
			CreatePackageManagementService();
			var package = AddOneFakePackageToPackageRepositoryFactoryRepository("PackageId");
			MakePackageManagementSourceRepositoryAndPackageRepositoryFactoryRepositoryTheSame();
			
			installPackageHelper.IgnoreDependencies = false;
			installPackageHelper.InstallPackageById("PackageId");
			
			bool result = fakePackageManagerFactory.FakePackageManager.IgnoreDependenciesPassedToGetInstallPackageOperations;
			
			Assert.IsFalse(result);
		}
		
		[Test]
		public void InstallPackage_PackageIdAndSourceAndProjectPassedAndIgnoreDependenciesIsFalse_DependenciesNotIgnoredWhenInstallingPackage()
		{
			CreatePackageManagementService();
			var package = AddOneFakePackageToPackageRepositoryFactoryRepository("PackageId");
			MakePackageManagementSourceRepositoryAndPackageRepositoryFactoryRepositoryTheSame();
			
			installPackageHelper.IgnoreDependencies = false;
			installPackageHelper.InstallPackageById("PackageId");
			
			bool result = fakePackageManagerFactory.FakePackageManager.IgnoreDependenciesPassedToInstallPackage;
			
			Assert.IsFalse(result);
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
		public void CreatePackageManagerForActiveProject_ProjectIsSelected_UsesActiveRepository()
		{
			CreatePackageManagementService();
			
			packageManagementService.CreatePackageManagerForActiveProject();
			
			var expectedRepository = packageManagementService.ActivePackageRepository;
			var actualRepository = fakePackageManagerFactory.PackageRepositoryPassedToCreatePackageManager;
			
			Assert.AreEqual(expectedRepository, actualRepository);
		}
		
		[Test]
		public void CreatePackageManagerForActiveProject_ReturnsPackageManager()
		{
			CreatePackageManagementService();
			
			ISharpDevelopPackageManager packageManager = 
				packageManagementService.CreatePackageManagerForActiveProject();
			
			var expectedPackageManager = fakePackageManagerFactory.FakePackageManager;
			
			Assert.AreEqual(expectedPackageManager, packageManager);
		}
		
		[Test]
		public void InstallPackage_VersionSpecified_VersionUsedWhenSearchingForPackage()
		{
			CreatePackageManagementService();
			MakePackageManagementSourceRepositoryAndPackageRepositoryFactoryRepositoryTheSame();
			
			var recentPackage = AddOneFakePackageToPackageRepositoryFactoryRepository("PackageId");
			recentPackage.Version = new Version("1.2.0");
			
			var oldPackage = AddOneFakePackageToPackageRepositoryFactoryRepository("PackageId");
			oldPackage.Version = new Version("1.0.0");
			
			var package = AddOneFakePackageToPackageRepositoryFactoryRepository("PackageId");
			var version = new Version("1.1.0");
			package.Version = version;
			
			installPackageHelper.Version = version;
			installPackageHelper.InstallPackageById("PackageId");
			
			var actualPackage = fakePackageManagerFactory.FakePackageManager.PackagePassedToInstallPackage;
			
			Assert.AreEqual(package, actualPackage);
		}
		
		[Test]
		public void InstallPackage_PackageIdSpecified_RefreshesProjectBrowserWindow()
		{
			CreatePackageManagementService();
			AddOneFakePackageToPackageRepositoryFactoryRepository("PackageId");
			MakePackageManagementSourceRepositoryAndPackageRepositoryFactoryRepositoryTheSame();
			
			installPackageHelper.InstallPackageById("PackageId");
			
			Assert.IsTrue(fakeProjectService.IsRefreshProjectBrowserCalled);
		}
		
		[Test]
		public void PackageInstalled_PackageIdSpecified_EventFiresAfterPackageInstalled()
		{
			CreatePackageManagementService();
			
			var expectedPackage = AddOneFakePackageToPackageRepositoryFactoryRepository("PackageId");
			MakePackageManagementSourceRepositoryAndPackageRepositoryFactoryRepositoryTheSame();
			
			IPackage package = null;
			packageManagementService.PackageInstalled += (sender, e) => {
				package = fakePackageManagerFactory.FakePackageManager.PackagePassedToInstallPackage;
			};
			
			installPackageHelper.InstallPackageById("PackageId");
			
			Assert.AreEqual(expectedPackage, package);
		}

		[Test]
		public void GetProject_ThreeProjectsOpenAndProjectWithNameExists_ReturnsMatchingProject()
		{
			CreatePackageManagementService();
			
			AddProject("One");
			var expectedProject = AddProject("Two");
			AddProject("Three");
			
			var actualProject = packageManagementService.GetProject("Two");
			
			Assert.AreEqual(expectedProject, actualProject);
		}
		
		[Test]
		public void GetProject_ProjectNameHasDifferentCase_ReturnsMatchingProjectIgnoringCase()
		{
			CreatePackageManagementService();
			
			AddProject("One");
			var expectedProject = AddProject("TWO");
			AddProject("Three");
			
			var actualProject = packageManagementService.GetProject("two");
			
			Assert.AreEqual(expectedProject, actualProject);
		}
		
		[Test]
		public void UninstallPackage_PackageIdSpecified_CallsPackageManagerUninstallPackage()
		{
			CreatePackageManagementService();
			var package = AddOneFakePackageToPackageRepositoryFactoryRepository("PackageId");
			MakePackageManagementSourceRepositoryAndPackageRepositoryFactoryRepositoryTheSame();
			
			uninstallPackageHelper.UninstallPackageById("PackageId");
			
			var actualPackage = fakePackageManagerFactory.FakePackageManager.PackagePassedToUninstallPackage;
			
			Assert.AreEqual(package, actualPackage);
		}
		
		[Test]
		public void UninstallPackage_PackageIdSpecified_RefreshesProjectBrowserWindow()
		{
			CreatePackageManagementService();
			AddOneFakePackageToPackageRepositoryFactoryRepository("PackageId");
			MakePackageManagementSourceRepositoryAndPackageRepositoryFactoryRepositoryTheSame();
			
			uninstallPackageHelper.UninstallPackageById("PackageId");
			
			Assert.IsTrue(fakeProjectService.IsRefreshProjectBrowserCalled);
		}
		
		[Test]
		public void PackageUninstalled_PackageIdSpecified_EventFiresAfterPackageUninstalled()
		{
			CreatePackageManagementService();
			var expectedPackage = AddOneFakePackageToPackageRepositoryFactoryRepository("PackageId");
			MakePackageManagementSourceRepositoryAndPackageRepositoryFactoryRepositoryTheSame();
			
			IPackage package = null;
			packageManagementService.PackageUninstalled += (sender, e) => {
				package = fakePackageManagerFactory.FakePackageManager.PackagePassedToUninstallPackage;
			};
			
			uninstallPackageHelper.UninstallPackageById("PackageId");
						
			Assert.AreEqual(expectedPackage, package);
		}
		
		[Test]
		public void UninstallPackage_PackageIdSpecified_PackageSourcePassedIsUsedToCreateRepository()
		{
			CreatePackageManagementService();
			AddOneFakePackageToPackageRepositoryFactoryRepository("PackageId");
			MakePackageManagementSourceRepositoryAndPackageRepositoryFactoryRepositoryTheSame();
			
			var expectedPackageSource = new PackageSource("http://test");
			uninstallPackageHelper.PackageSource = expectedPackageSource;
			uninstallPackageHelper.UninstallPackageById("PackageId");
			
			var expectedPackageSources = new PackageSource[] {
				expectedPackageSource
			};
			
			var actualPackageSources = fakePackageRepositoryFactory.PackageSourcesPassedToCreateRepository;
			
			CollectionAssert.AreEqual(expectedPackageSources, actualPackageSources);
		}
		
		[Test]
		public void UninstallPackage_PackageIdSpecified_ProjectPassedIsUsedToCreateRepository()
		{
			CreatePackageManagementService();
			AddOneFakePackageToPackageRepositoryFactoryRepository("PackageId");
			MakePackageManagementSourceRepositoryAndPackageRepositoryFactoryRepositoryTheSame();
			
			var expectedProject = ProjectHelper.CreateTestProject();
			uninstallPackageHelper.Project = expectedProject;
			
			uninstallPackageHelper.UninstallPackageById("PackageId");
			
			var actualProject = fakePackageManagerFactory.ProjectPassedToCreateRepository;
			
			Assert.AreEqual(expectedProject, actualProject);
		}
		
		[Test]
		public void UpdatePackage_PackageAndRepositoryPassed_PackageInstalled()
		{
			CreatePackageManagementService();
			updatePackageHelper.UpdateTestPackage();
			
			var expectedPackage = updatePackageHelper.TestPackage;
			var actualPackage = fakePackageManagerFactory.FakePackageManager.PackagePassedToUpdatePackage;
			
			Assert.AreEqual(expectedPackage, actualPackage);
		}
		
		[Test]
		public void UpdatePackage_PackageAndRepositoryPassed_RepositoryUsedToCreatePackageManager()
		{
			CreatePackageManagementService();
			updatePackageHelper.UpdateTestPackage();
			
			var expectedRepository = updatePackageHelper.PackageRepository;
			var actualRepository = fakePackageManagerFactory.PackageRepositoryPassedToCreatePackageManager;
			
			Assert.AreEqual(expectedRepository, actualRepository);
		}
		
		[Test]
		public void UpdatePackage_PackageAndRepositoryPassed_PackageOperationsUsedToUpdatePackage()
		{
			CreatePackageManagementService();
			updatePackageHelper.UpdateTestPackage();
			
			var expectedOperations = updatePackageHelper.PackageOperations;
			var actualOperations = fakePackageManagerFactory.FakePackageManager.PackageOperationsPassedToUpdatePackage;
			
			Assert.AreEqual(expectedOperations, actualOperations);
		}
		
		[Test]
		public void UpdatePackage_PackageAndRepositoryPassed_ProjectBrowserIsRefreshed()
		{
			CreatePackageManagementService();
			updatePackageHelper.UpdateTestPackage();
			
			bool refreshed = fakeProjectService.IsRefreshProjectBrowserCalled;
			
			Assert.IsTrue(refreshed);
		}
		
		[Test]
		public void UpdatePackage_PackageAndRepositoryPassed_PackageUpdateIsAddedToRecentPackagesRepository()
		{
			CreatePackageManagementService();
			updatePackageHelper.UpdateTestPackage();
			
			var recentPackages = packageManagementService.RecentPackageRepository.GetPackages();
			
			var expectedPackages = new FakePackage[] {
				updatePackageHelper.TestPackage
			};
			
			PackageCollectionAssert.AreEqual(expectedPackages, recentPackages);
		}
		
		[Test]
		public void UpdatePackage_PackageAndRepositoryPassed_PackageInstalledEventIsFired()
		{
			CreatePackageManagementService();
			
			IPackage package = null;
			packageManagementService.PackageInstalled += (sender, e) => {
				package = fakePackageManagerFactory.FakePackageManager.PackagePassedToUpdatePackage;
			};
			updatePackageHelper.UpdateTestPackage();
			
			Assert.AreEqual(installPackageHelper.TestPackage, package);

		}
	}
}
