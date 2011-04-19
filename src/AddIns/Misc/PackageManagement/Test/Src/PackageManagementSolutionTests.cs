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
		FakePackageManagementProjectFactory fakeProjectFactory;
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
			fakeProjectFactory = new FakePackageManagementProjectFactory();
			fakeProjectService = new FakePackageManagementProjectService();
			var packageManagementEvents = new FakePackageManagementEvents();
			
			fakeProjectService.CurrentProject = testProject;
			solution = 
				new PackageManagementSolution(
					fakeRegisteredPackageRepositories,
					fakePackageManagerFactory,
					packageManagementEvents,
					fakeProjectService,
					fakeProjectFactory);
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
		public void GetActiveProject_ProjectIsSelected_CreatesProjectUsingCurrentProjectSelectedInSharpDevelop()
		{
			CreateSolution();
			
			IPackageManagementProject activeProject = solution.GetActiveProject();
			IProject actualProject = fakeProjectFactory.ProjectPassedToCreateProject;
			
			Assert.AreEqual(testProject, actualProject);
		}
		
		[Test]
		public void GetActiveProject_ProjectIsSelected_CreatesProjectUsingCurrentActiveRepository()
		{
			CreateSolution();
			
			IPackageManagementProject activeProject = solution.GetActiveProject();
			
			IPackageRepository repository = fakeProjectFactory.RepositoryPassedToCreateProject;
			IPackageRepository expectedRepository = fakeRegisteredPackageRepositories.ActiveRepository;
			
			Assert.AreEqual(expectedRepository, repository);
		}
		
		[Test]
		public void GetActiveProject_ProjectIsSelected_ReturnsProjectCreatedByFactory()
		{
			CreateSolution();
			
			IPackageManagementProject activeProject = solution.GetActiveProject();
			IPackageManagementProject expectedProject = fakeProjectFactory.FakeProject;
			
			Assert.AreEqual(expectedProject, activeProject);
		}
		
		[Test]
		public void CreateProject_ProjectAndRepositoryPassed_ProjectUsedToCreateProject()
		{
			CreateSolution();
			var expectedProject = ProjectHelper.CreateTestProject();
			var repository = new FakePackageRepository();
			solution.CreateProject(repository, expectedProject);
			
			var actualProject = fakeProjectFactory.ProjectPassedToCreateProject;
			
			Assert.AreEqual(expectedProject, actualProject);
		}
		
		[Test]
		public void CreateProject_ProjectAndRepositoryPassed_CreatesProjectUsingRepository()
		{
			CreateSolution();
			var expectedRepository = new FakePackageRepository();
			
			solution.CreateProject(expectedRepository, null);
			
			IPackageRepository actualRepository = fakeProjectFactory.RepositoryPassedToCreateProject;
			
			Assert.AreEqual(expectedRepository, actualRepository);
		}
		
		[Test]
		public void CreateProject_ProjectAndRepositoryPassed_ReturnsProjectCreatedByFactory()
		{
			CreateSolution();
			var testProject = ProjectHelper.CreateTestProject();
			var repository = new FakePackageRepository();
			
			var project = solution.CreateProject(repository, testProject);
			
			IPackageManagementProject expectedProject = fakeProjectFactory.FakeProject;
			
			Assert.AreEqual(expectedProject, project);
		}
		
		[Test]
		public void GetActiveProject_RepositoryPassed_CreatesProjectUsingRepository()
		{
			CreateSolution();
			var expectedRepository = new FakePackageRepository();
			solution.GetActiveProject(expectedRepository);
			
			IPackageRepository repository = fakeProjectFactory.RepositoryPassedToCreateProject;
			
			Assert.AreEqual(expectedRepository, repository);
		}
		
		[Test]
		public void GetActiveProject_RepositoryPassed_CreatesProjectUsingCurrentActiveProject()
		{
			CreateSolution();
			var expectedRepository = new FakePackageRepository();
			var expectedProject = ProjectHelper.CreateTestProject();
			fakeProjectService.CurrentProject = expectedProject;
			
			solution.GetActiveProject(expectedRepository);
			
			var project = fakeProjectFactory.ProjectPassedToCreateProject;
			
			Assert.AreEqual(expectedProject, project);
		}
		
		[Test]
		public void GetActiveProject_RepositoryPassed_ReturnsProjectFromProjectFactory()
		{
			CreateSolution();
			var expectedRepository = new FakePackageRepository();
			var project = solution.GetActiveProject(expectedRepository);
			
			var expectedProject = fakeProjectFactory.FakeProject;
			
			Assert.AreEqual(expectedProject, project);
		}
		
		[Test]
		public void CreateProject_PackagesSourceAndProjectPassed_CreatesProjectUsingCreatedRepository()
		{
			CreateSolution();
			var source = new PackageSource("http://sharpdevelop.net");
			var testProject = ProjectHelper.CreateTestProject();
			solution.CreateProject(source, testProject);
			
			var repository = fakeProjectFactory.RepositoryPassedToCreateProject;
			var expectedRepository = fakeRegisteredPackageRepositories.FakePackageRepository;
			
			Assert.AreEqual(expectedRepository, repository);
		}
		
		[Test]
		public void CreateProject_PackagesSourceAndProjectPassed_PackageSourceUsedToCreateRepository()
		{
			CreateSolution();
			var expectedSource = new PackageSource("http://sharpdevelop.net");
			var testProject = ProjectHelper.CreateTestProject();
			
			solution.CreateProject(expectedSource, testProject);
			
			var source = fakeRegisteredPackageRepositories.PackageSourcePassedToCreateRepository;
			
			Assert.AreEqual(expectedSource, source);
		}
		
		[Test]
		public void CreateProject_PackagesSourceAndProjectPassed_CreatesProjectUsingProjectPassed()
		{
			CreateSolution();
			var source = new PackageSource("http://sharpdevelop.net");
			var expectedProject = ProjectHelper.CreateTestProject();
			
			solution.CreateProject(source, expectedProject);
			
			var project = fakeProjectFactory.ProjectPassedToCreateProject;
			
			Assert.AreEqual(expectedProject, project);
		}
		
		[Test]
		public void CreateProject_PackagesSourceAndProjectPassed_ReturnsProjectFromProjectFactory()
		{
			CreateSolution();
			var source = new PackageSource("http://sharpdevelop.net");
			var testProject = ProjectHelper.CreateTestProject();
			var project = solution.CreateProject(source, testProject);
			
			var expectedProject = fakeProjectFactory.FakeProject;
			
			Assert.AreEqual(expectedProject, project);
		}
	}
}
