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
	public class PackageManagementSolutionTests
	{
		PackageManagementSolution solution;
		FakeRegisteredPackageRepositories fakeRegisteredPackageRepositories;
		OneRegisteredPackageSourceHelper packageSourcesHelper;
		FakePackageManagerFactory fakePackageManagerFactory;
		FakePackageManagementProjectService fakeProjectService;
		TestableProject testProject;
		
		void CreatePackageSources()
		{
			packageSourcesHelper = new OneRegisteredPackageSourceHelper();
		}
		
		void CreateSolution()
		{
			CreatePackageSources();
			CreateSolution(packageSourcesHelper.Options);
		}
		
		void CreateSolution(PackageManagementOptions options)
		{
			testProject = ProjectHelper.CreateTestProject();
			fakeRegisteredPackageRepositories = new FakeRegisteredPackageRepositories();
			fakePackageManagerFactory = new FakePackageManagerFactory();
			fakeProjectService = new FakePackageManagementProjectService();
			var packageManagementEvents = new FakePackageManagementEvents();
			
			fakeProjectService.CurrentProject = testProject;
			solution = 
				new PackageManagementSolution(
					fakeRegisteredPackageRepositories,
					fakePackageManagerFactory,
					packageManagementEvents,
					fakeProjectService);
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
			CreateSolution();
			
			IProjectManager activeProjectManager = solution.ActiveProjectManager;
			IProject actualProject = fakePackageManagerFactory.ProjectPassedToCreateRepository;
			
			Assert.AreEqual(testProject, actualProject);
		}
		
		[Test]
		public void CreateProjectManager_RepositoryAndProjectSpecified_CreatesPackageManagerFromPackageManagerFactory()
		{
			CreateSolution();
			var repository = new FakePackageRepository();
			var project = ProjectHelper.CreateTestProject();
			
			ISharpDevelopProjectManager projectManager = solution.CreateProjectManager(repository, project);
			
			var expectedProjectManager = fakePackageManagerFactory.FakePackageManager.FakeProjectManager;
			Assert.AreEqual(expectedProjectManager, projectManager);
		}
		
		[Test]
		public void CreateProjectManager_RepositorySpecified_RepositoryUsedToCreateProjectManager()
		{
			CreateSolution();
			var repository = new FakePackageRepository();
			
			solution.CreateProjectManager(repository, null);
			
			var expectedRepository = fakePackageManagerFactory.PackageRepositoryPassedToCreatePackageManager;
			
			Assert.AreEqual(expectedRepository, repository);
		}
		
		[Test]
		public void CreateProjectManager_ProjectSpecified_ProjectUsedToCreateProjectManager()
		{
			CreateSolution();
			var project = ProjectHelper.CreateTestProject();
			
			solution.CreateProjectManager(null, project);
			
			var expectedProject = fakePackageManagerFactory.ProjectPassedToCreateRepository;
			
			Assert.AreEqual(expectedProject, project);
		}		
		
		[Test]
		public void CreatePackageManagerForActiveProject_ProjectIsSelected_ReferencesSelectedProject()
		{
			CreateSolution();
			
			solution.CreatePackageManagerForActiveProject();
			
			IProject expectedProject = fakeProjectService.CurrentProject;
			IProject actualProject = fakePackageManagerFactory.ProjectPassedToCreateRepository;
			
			Assert.AreEqual(expectedProject, actualProject);
		}
		
		[Test]
		public void CreatePackageManagerForActiveProject_PackageRepositoryPassed_CreatesPackageManagerWithCurrentlyActiveProject()
		{
			CreateSolution();
			
			var repository = new FakePackageRepository();
			solution.CreatePackageManagerForActiveProject(repository);
			
			IProject expectedProject = fakeProjectService.CurrentProject;
			IProject actualProject = fakePackageManagerFactory.ProjectPassedToCreateRepository;
			
			Assert.AreEqual(expectedProject, actualProject);
		}
		
		[Test]
		public void CreatePackageManagerForActiveProject_PackageRepositoryPassed_PackageManagerCreatedWithRepository()
		{
			CreateSolution();
			
			var repository = new FakePackageRepository();
			solution.CreatePackageManagerForActiveProject(repository);
			
			var actualRepository = fakePackageManagerFactory.PackageRepositoryPassedToCreatePackageManager;
			
			Assert.AreEqual(repository, actualRepository);
		}
		
		[Test]
		public void CreatePackageManagerForActiveProject_ProjectIsSelected_UsesActiveRepository()
		{
			CreateSolution();
			
			solution.CreatePackageManagerForActiveProject();
			
			var expectedRepository = fakeRegisteredPackageRepositories.ActiveRepository;
			var actualRepository = fakePackageManagerFactory.PackageRepositoryPassedToCreatePackageManager;
			
			Assert.AreEqual(expectedRepository, actualRepository);
		}
		
		[Test]
		public void CreatePackageManagerForActiveProject_ProjectIsSelected_ReturnsPackageManager()
		{
			CreateSolution();
			
			ISharpDevelopPackageManager packageManager = 
				solution.CreatePackageManagerForActiveProject();
			
			var expectedPackageManager = fakePackageManagerFactory.FakePackageManager;
			
			Assert.AreEqual(expectedPackageManager, packageManager);
		}
		
		[Test]
		public void CreatePackageManager_PackageSourceAndProjectPassed_ReturnsNewPackageManager()
		{
			CreateSolution();
			
			var packageSource = new PackageSource("test");
			ISharpDevelopPackageManager packageManager = 
				solution.CreatePackageManager(packageSource, testProject);
			
			var expectedPackageManager = fakePackageManagerFactory.FakePackageManager;
			
			Assert.AreEqual(expectedPackageManager, packageManager);
		}
		
		[Test]
		public void CreatePackageManager_PackageSourceAndProjectPassed_PackageSourceUsedToCreateRepository()
		{
			CreateSolution();
			
			var packageSource = new PackageSource("test");
			ISharpDevelopPackageManager packageManager = 
				solution.CreatePackageManager(packageSource, testProject);
			
			var actualPackageSource = fakeRegisteredPackageRepositories.PackageSourcePassedToCreateRepository;
			
			Assert.AreEqual(packageSource, actualPackageSource);
		}
		
		[Test]
		public void CreatePackageManager_PackageSourceAndProjectPassed_CreatedRepositoryUsedToCreatePackageManager()
		{
			CreateSolution();
			
			var packageSource = new PackageSource("test");
			ISharpDevelopPackageManager packageManager = 
				solution.CreatePackageManager(packageSource, testProject);
			
			var actualRepository = fakePackageManagerFactory.PackageRepositoryPassedToCreatePackageManager;
			var expectedRepository = fakeRegisteredPackageRepositories.FakePackageRepository;
			
			Assert.AreEqual(expectedRepository, actualRepository);
		}
		
		[Test]
		public void CreatePackageManager_PackageSourceAndProjectPassed_ProjectUsedToCreatePackageManager()
		{
			CreateSolution();
			
			var packageSource = new PackageSource("test");
			ISharpDevelopPackageManager packageManager = 
				solution.CreatePackageManager(packageSource, testProject);
			
			var actualProject = fakePackageManagerFactory.ProjectPassedToCreateRepository;
			
			Assert.AreEqual(testProject, actualProject);
		}
	}
}
