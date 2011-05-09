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
			fakeProjectFactory = new FakePackageManagementProjectFactory();
			fakeProjectService = new FakePackageManagementProjectService();
			var packageManagementEvents = new FakePackageManagementEvents();
			
			fakeProjectService.CurrentProject = testProject;
			solution =
				new PackageManagementSolution(
					fakeRegisteredPackageRepositories,
					packageManagementEvents,
					fakeProjectService,
					fakeProjectFactory);
		}
		
		TestableProject AddProjectToOpenProjects(string projectName)
		{
			var project = ProjectHelper.CreateTestProject(projectName);
			fakeProjectService.FakeOpenProjects.Add(project);
			return project;
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
		public void GetProject_PackagesSourceAndProjectNamePassed_CreatesProjectUsingFoundProjectMatchingName()
		{
			CreateSolution();
			var expectedProject = AddProjectToOpenProjects("Test");
			var source = new PackageSource("http://sharpdevelop.net");
			
			solution.GetProject(source, "Test");
			
			var project = fakeProjectFactory.ProjectPassedToCreateProject;
			
			Assert.AreEqual(expectedProject, project);
		}
		
		[Test]
		public void GetProject_PackagesSourceAndProjectNameWithDifferentCasePassed_CreatesProjectUsingFoundProjectMatchingName()
		{
			CreateSolution();
			var expectedProject = AddProjectToOpenProjects("Test");
			var source = new PackageSource("http://sharpdevelop.net");
			
			solution.GetProject(source, "TEST");
			
			var project = fakeProjectFactory.ProjectPassedToCreateProject;
			
			Assert.AreEqual(expectedProject, project);
		}
		
		[Test]
		public void GetProject_PackagesSourceAndProjectPassed_ReturnsProjectFromProjectFactory()
		{
			CreateSolution();
			AddProjectToOpenProjects("Test");
			var source = new PackageSource("http://sharpdevelop.net");
			var project = solution.GetProject(source, "Test");
			
			var expectedProject = fakeProjectFactory.FakeProject;
			
			Assert.AreEqual(expectedProject, project);
		}
		
		[Test]
		public void GetProject_PackagesSourceAndProjectPassed_PackageSourceUsedToCreateRepository()
		{
			CreateSolution();
			AddProjectToOpenProjects("Test");
			var expectedSource = new PackageSource("http://sharpdevelop.net");
			var project = solution.GetProject(expectedSource, "Test");
			
			var actualSource = fakeRegisteredPackageRepositories.PackageSourcePassedToCreateRepository;
			
			Assert.AreEqual(expectedSource, actualSource);
		}
		
		[Test]
		public void GetProject_PackagesRepositoryAndProjectNamePassed_CreatesProjectUsingFoundProjectMatchingName()
		{
			CreateSolution();
			var expectedProject = AddProjectToOpenProjects("Test");
			var repository = new FakePackageRepository();
			
			solution.GetProject(repository, "Test");
			
			var project = fakeProjectFactory.ProjectPassedToCreateProject;
			
			Assert.AreEqual(expectedProject, project);
		}
		
		[Test]
		public void GetProject_RepositoryAndProjectNameWithDifferentCasePassed_CreatesProjectUsingFoundProjectMatchingName()
		{
			CreateSolution();
			var expectedProject = AddProjectToOpenProjects("Test");
			var repository = new FakePackageRepository();
			
			solution.GetProject(repository, "TEST");
			
			var project = fakeProjectFactory.ProjectPassedToCreateProject;
			
			Assert.AreEqual(expectedProject, project);
		}
		
		[Test]
		public void GetProject_RepositoryAndProjectNamePassed_ReturnsProject()
		{
			CreateSolution();
			AddProjectToOpenProjects("Test");
			var repository = new FakePackageRepository();
			
			var project = solution.GetProject(repository, "Test");
			
			var expectedProject = fakeProjectFactory.FakeProject;
			
			Assert.AreEqual(expectedProject, project);
		}
		
		[Test]
		public void GetProject_RepositoryAndProjectNamePassed_RepositoryUsedToCreateProject()
		{
			CreateSolution();
			AddProjectToOpenProjects("Test");
			var expectedRepository = new FakePackageRepository();
			
			solution.GetProject(expectedRepository, "Test");
			
			var actualRepository = fakeProjectFactory.RepositoryPassedToCreateProject;
			
			Assert.AreEqual(expectedRepository, actualRepository);
		}
		
		[Test]
		public void GetMSBuildProjects_TwoProjectsInOpenSolution_ReturnsTwoProjects()
		{
			CreateSolution();
			AddProjectToOpenProjects("A");
			AddProjectToOpenProjects("B");
			
			var projects = solution.GetMSBuildProjects();
			var expectedProjects = fakeProjectService.FakeOpenProjects;
			
			CollectionAssert.AreEqual(expectedProjects, projects);
		}
	}
}
