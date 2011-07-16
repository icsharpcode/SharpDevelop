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
		FakeSolutionPackageRepositoryFactory fakeSolutionPackageRepositoryFactory;
		FakeSolutionPackageRepository fakeSolutionPackageRepository;
		
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
			
			fakeProjectService.CurrentProject = testProject;
			fakeProjectService.OpenSolution = testProject.ParentSolution;
			
			fakeSolutionPackageRepositoryFactory = new FakeSolutionPackageRepositoryFactory();
			fakeSolutionPackageRepository = fakeSolutionPackageRepositoryFactory.FakeSolutionPackageRepository;
			
			solution =
				new PackageManagementSolution(
					fakeRegisteredPackageRepositories,
					fakeProjectService,
					fakeProjectFactory,
					fakeSolutionPackageRepositoryFactory);
		}
		
		TestableProject AddProjectToOpenProjects(string projectName)
		{
			TestableProject project = ProjectHelper.CreateTestProject(projectName);
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
			TestableProject expectedProject = ProjectHelper.CreateTestProject();
			fakeProjectService.CurrentProject = expectedProject;
			
			solution.GetActiveProject(expectedRepository);
			
			MSBuildBasedProject project = fakeProjectFactory.ProjectPassedToCreateProject;
			
			Assert.AreEqual(expectedProject, project);
		}
		
		[Test]
		public void GetActiveProject_RepositoryPassed_ReturnsProjectFromProjectFactory()
		{
			CreateSolution();
			var expectedRepository = new FakePackageRepository();
			IPackageManagementProject project = solution.GetActiveProject(expectedRepository);
			
			FakePackageManagementProject expectedProject = fakeProjectFactory.FakeProject;
			
			Assert.AreEqual(expectedProject, project);
		}
		
		[Test]
		public void GetProject_PackagesSourceAndProjectNamePassed_CreatesProjectUsingFoundProjectMatchingName()
		{
			CreateSolution();
			TestableProject expectedProject = AddProjectToOpenProjects("Test");
			var source = new PackageSource("http://sharpdevelop.net");
			
			solution.GetProject(source, "Test");
			
			MSBuildBasedProject project = fakeProjectFactory.ProjectPassedToCreateProject;
			
			Assert.AreEqual(expectedProject, project);
		}
		
		[Test]
		public void GetProject_PackagesSourceAndProjectNameWithDifferentCasePassed_CreatesProjectUsingFoundProjectMatchingName()
		{
			CreateSolution();
			TestableProject expectedProject = AddProjectToOpenProjects("Test");
			var source = new PackageSource("http://sharpdevelop.net");
			
			solution.GetProject(source, "TEST");
			
			MSBuildBasedProject project = fakeProjectFactory.ProjectPassedToCreateProject;
			
			Assert.AreEqual(expectedProject, project);
		}
		
		[Test]
		public void GetProject_PackagesSourceAndProjectPassed_ReturnsProjectFromProjectFactory()
		{
			CreateSolution();
			AddProjectToOpenProjects("Test");
			var source = new PackageSource("http://sharpdevelop.net");
			IPackageManagementProject project = solution.GetProject(source, "Test");
			
			FakePackageManagementProject expectedProject = fakeProjectFactory.FakeProject;
			
			Assert.AreEqual(expectedProject, project);
		}
		
		[Test]
		public void GetProject_PackagesSourceAndProjectPassed_PackageSourceUsedToCreateRepository()
		{
			CreateSolution();
			AddProjectToOpenProjects("Test");
			var expectedSource = new PackageSource("http://sharpdevelop.net");
			IPackageManagementProject project = solution.GetProject(expectedSource, "Test");
			
			PackageSource actualSource = fakeRegisteredPackageRepositories.PackageSourcePassedToCreateRepository;
			
			Assert.AreEqual(expectedSource, actualSource);
		}
		
		[Test]
		public void GetProject_PackagesRepositoryAndProjectNamePassed_CreatesProjectUsingFoundProjectMatchingName()
		{
			CreateSolution();
			TestableProject expectedProject = AddProjectToOpenProjects("Test");
			var repository = new FakePackageRepository();
			
			solution.GetProject(repository, "Test");
			
			MSBuildBasedProject project = fakeProjectFactory.ProjectPassedToCreateProject;
			
			Assert.AreEqual(expectedProject, project);
		}
		
		[Test]
		public void GetProject_PackagesRepositoryAndProjectPassed_CreatesProjectUsingProjectPassed()
		{
			CreateSolution();
			TestableProject expectedProject = AddProjectToOpenProjects("Test");
			var repository = new FakePackageRepository();
			
			solution.GetProject(repository, expectedProject);
			
			MSBuildBasedProject project = fakeProjectFactory.ProjectPassedToCreateProject;
			
			Assert.AreEqual(expectedProject, project);
		}
		
		[Test]
		public void GetProject_PackagesRepositoryAndProjectPassed_ReturnsProjectCreatedFromProjectFactory()
		{
			CreateSolution();
			TestableProject msbuildProject = AddProjectToOpenProjects("Test");
			var repository = new FakePackageRepository();
			
			IPackageManagementProject project = solution.GetProject(repository, msbuildProject);
			
			FakePackageManagementProject expectedProject = fakeProjectFactory.FakeProject;
			
			Assert.AreEqual(expectedProject, project);
		}
		
		[Test]
		public void GetProject_PackagesRepositoryAndProjectPassed_CreatesProjectUsingRepository()
		{
			CreateSolution();
			TestableProject expectedProject = AddProjectToOpenProjects("Test");
			var expectedRepository = new FakePackageRepository();
			
			solution.GetProject(expectedRepository, expectedProject);
			
			IPackageRepository repository = fakeProjectFactory.RepositoryPassedToCreateProject;
			
			Assert.AreEqual(expectedRepository, repository);
		}
		
		[Test]
		public void GetProject_RepositoryAndProjectNameWithDifferentCasePassed_CreatesProjectUsingFoundProjectMatchingName()
		{
			CreateSolution();
			TestableProject expectedProject = AddProjectToOpenProjects("Test");
			var repository = new FakePackageRepository();
			
			solution.GetProject(repository, "TEST");
			
			MSBuildBasedProject project = fakeProjectFactory.ProjectPassedToCreateProject;
			
			Assert.AreEqual(expectedProject, project);
		}
		
		[Test]
		public void GetProject_RepositoryAndProjectNamePassed_ReturnsProject()
		{
			CreateSolution();
			AddProjectToOpenProjects("Test");
			var repository = new FakePackageRepository();
			
			IPackageManagementProject project = solution.GetProject(repository, "Test");
			
			FakePackageManagementProject expectedProject = fakeProjectFactory.FakeProject;
			
			Assert.AreEqual(expectedProject, project);
		}
		
		[Test]
		public void GetProject_RepositoryAndProjectNamePassed_RepositoryUsedToCreateProject()
		{
			CreateSolution();
			AddProjectToOpenProjects("Test");
			var expectedRepository = new FakePackageRepository();
			
			solution.GetProject(expectedRepository, "Test");
			
			IPackageRepository actualRepository = fakeProjectFactory.RepositoryPassedToCreateProject;
			
			Assert.AreEqual(expectedRepository, actualRepository);
		}
		
		[Test]
		public void GetMSBuildProjects_TwoProjectsInOpenSolution_ReturnsTwoProjects()
		{
			CreateSolution();
			AddProjectToOpenProjects("A");
			AddProjectToOpenProjects("B");
			
			IEnumerable<IProject> projects = solution.GetMSBuildProjects();
			List<IProject> expectedProjects = fakeProjectService.FakeOpenProjects;
			
			CollectionAssert.AreEqual(expectedProjects, projects);
		}
		
		[Test]
		public void IsOpen_NoSolutionOpen_ReturnsFalse()
		{
			CreateSolution();
			fakeProjectService.OpenSolution = null;
			
			bool open = solution.IsOpen;
			
			Assert.IsFalse(open);
		}
		
		[Test]
		public void IsOpen_SolutionIsOpen_ReturnsTrue()
		{
			CreateSolution();
			fakeProjectService.OpenSolution = new Solution();
			
			bool open = solution.IsOpen;
			
			Assert.IsTrue(open);
		}
		
		[Test]
		public void GetActiveMSBuildProject_CurrentProjectIsSetInProjectService_ReturnsProjectCurrentlySelected()
		{
			CreateSolution();
			fakeProjectService.CurrentProject = testProject;
			
			IProject activeProject = solution.GetActiveMSBuildProject();
			
			Assert.AreEqual(testProject, activeProject);
		}
		
		[Test]
		public void HasMultipleProjects_OneProjectInSolution_ReturnsFalse()
		{
			CreateSolution();
			TestableProject project = ProjectHelper.CreateTestProject();
			fakeProjectService.AddFakeProject(project);
			
			bool hasMultipleProjects = solution.HasMultipleProjects();
			
			Assert.IsFalse(hasMultipleProjects);
		}
		
		[Test]
		public void HasMultipleProjects_TwoProjectsInSolution_ReturnsTrue()
		{
			CreateSolution();
			TestableProject project1 = ProjectHelper.CreateTestProject();
			fakeProjectService.AddFakeProject(project1);
			TestableProject project2 = ProjectHelper.CreateTestProject();
			fakeProjectService.AddFakeProject(project2);
			
			bool hasMultipleProjects = solution.HasMultipleProjects();
			
			Assert.IsTrue(hasMultipleProjects);
		}
		
		[Test]
		public void FileName_SolutionHasFileName_ReturnsSolutionFileName()
		{
			CreateSolution();
			var solution = new Solution();
			string expectedFileName = @"d:\projects\myproject\Project.sln";
			solution.FileName = expectedFileName;
			fakeProjectService.OpenSolution = solution;
			
			string fileName = solution.FileName;
			
			Assert.AreEqual(expectedFileName, fileName);
		}
		
		[Test]
		public void IsInstalled_PackageIsInstalledInSolutionLocalRepository_ReturnsTrue()
		{
			CreateSolution();
			FakePackage package = FakePackage.CreatePackageWithVersion("Test", "1.3.4.5");
			fakeSolutionPackageRepository.FakeSharedRepository.FakePackages.Add(package);
			
			bool installed = solution.IsPackageInstalled(package);
			
			Assert.IsTrue(installed);
		}
		
		[Test]
		public void IsInstalled_PackageIsNotInstalledInSolutionLocalRepository_ReturnsFalse()
		{
			CreateSolution();
			FakePackage package = FakePackage.CreatePackageWithVersion("Test", "1.3.4.5");
			
			bool installed = solution.IsPackageInstalled(package);
			
			Assert.IsFalse(installed);
		}
		
		[Test]
		public void IsInstalled_PackageIsNotInstalledInSolutionLocalRepository_ActivSolutionUsedToCreateSolutionPackageRepository()
		{
			CreateSolution();
			FakePackage package = FakePackage.CreatePackageWithVersion("Test", "1.3.4.5");
			
			solution.IsPackageInstalled(package);
			
			Solution expectedSolution = fakeProjectService.OpenSolution;
			Solution solutionUsedToCreateSolutionPackageRepository = 
				fakeSolutionPackageRepositoryFactory.SolutionPassedToCreateSolutionPackageRepository;
			
			Assert.AreEqual(expectedSolution, solutionUsedToCreateSolutionPackageRepository);
		}
		
		[Test]
		public void GetActiveProject_SolutionOpenButNoProjectSelected_ReturnsNull()
		{
			CreateSolution();
			fakeProjectService.CurrentProject = null;
			
			IPackageManagementProject activeProject = solution.GetActiveProject();
			
			Assert.IsNull(activeProject);
		}
		
		[Test]
		public void GetActiveProject_RepositoryPassedWhenSolutionOpenButNoProjectSelected_ReturnsNull()
		{
			CreateSolution();
			fakeProjectService.CurrentProject = null;
			
			var repository = new FakePackageRepository();
			IPackageManagementProject activeProject = solution.GetActiveProject(repository);
			
			Assert.IsNull(activeProject);
		}
		
		[Test]
		public void GetPackages_OnePackageInSolutionRepository_ReturnsOnePackage()
		{
			CreateSolution();
			fakeProjectService.CurrentProject = null;
			FakePackage package = FakePackage.CreatePackageWithVersion("Test", "1.3.4.5");
			fakeSolutionPackageRepository.FakeSharedRepository.FakePackages.Add(package);
			
			IQueryable<IPackage> packages = solution.GetPackages();
			
			var expectedPackages = new FakePackage[] {
				package
			};
			
			PackageCollectionAssert.AreEqual(expectedPackages, packages);
		}
	}
}
