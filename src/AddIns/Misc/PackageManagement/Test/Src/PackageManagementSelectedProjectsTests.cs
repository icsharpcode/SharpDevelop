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
	public class PackageManagementSelectedProjectsTests
	{
		PackageManagementSelectedProjects selectedProjects;
		FakePackageManagementSolution fakeSolution;
		
		void CreateSelectedProjects()
		{
			fakeSolution = new FakePackageManagementSolution();
			selectedProjects = new PackageManagementSelectedProjects(fakeSolution);
		}
		
		List<IProject> AddSolutionWithOneProjectToProjectService()
		{
			TestableProject project = ProjectHelper.CreateTestProject("Test1");
			fakeSolution.FakeMSBuildProjects.Add(project);
			
			return fakeSolution.FakeMSBuildProjects;
		}
		
		List<IProject> AddSolutionWithTwoProjectsToProjectService()
		{
			TestableProject project1 = ProjectHelper.CreateTestProject("Test1");
			TestableProject project2 = ProjectHelper.CreateTestProject("Test2");
			
			Solution solution = project1.ParentSolution;
			project2.Parent = solution;
			
			fakeSolution.FakeMSBuildProjects.Add(project1);
			fakeSolution.FakeMSBuildProjects.Add(project2);
			
			return fakeSolution.FakeMSBuildProjects;
		}
		
		void NoProjectsSelected()
		{
			fakeSolution.NoProjectsSelected();
		}
		
		[Test]
		public void GetProjects_SolutionHasTwoProjectsAndOneProjectSelectedInProjectsBrowser_ReturnsProjectSelectedInProjects()
		{
			CreateSelectedProjects();
			List<IProject> projectsAddedToSolution = AddSolutionWithTwoProjectsToProjectService();
			IProject project = projectsAddedToSolution[1];
			project.Name = "MyProject";
			fakeSolution.FakeActiveMSBuildProject = project;
			
			var fakeProject = fakeSolution.AddFakeProjectToReturnFromGetProject("MyProject");
			
			var fakePackage = new FakePackage();
			var projects = new List<IPackageManagementSelectedProject>();
			projects.AddRange(selectedProjects.GetProjects(fakePackage));
			
			var expectedProject = new FakeSelectedProject("MyProject");
			var expectedProjects = new List<IPackageManagementSelectedProject>();
			expectedProjects.Add(expectedProject);
			
			SelectedProjectCollectionAssert.AreEqual(expectedProjects, projects);
		}
		
		[Test]
		public void GetProjects_SolutionHasTwoProjectsAndOneProjectSelectedInitiallyAndGetProjectsCalledAgainAfterNoProjectsAreSelected_ReturnsProjectSelectedInProjects()
		{
			CreateSelectedProjects();
			List<IProject> projectsAddedToSolution = AddSolutionWithTwoProjectsToProjectService();
			IProject project = projectsAddedToSolution[1];
			project.Name = "MyProject";
			fakeSolution.FakeActiveMSBuildProject = project;
			
			var fakeProject = fakeSolution.AddFakeProjectToReturnFromGetProject("MyProject");
			
			var fakePackage = new FakePackage();
			var projects = new List<IPackageManagementSelectedProject>();
			projects.AddRange(selectedProjects.GetProjects(fakePackage));
			
			projects.Clear();
			
			NoProjectsSelected();
			projects.AddRange(selectedProjects.GetProjects(fakePackage));
			
			var expectedProject = new FakeSelectedProject("MyProject");
			var expectedProjects = new List<IPackageManagementSelectedProject>();
			expectedProjects.Add(expectedProject);
			
			SelectedProjectCollectionAssert.AreEqual(expectedProjects, projects);
		}
		
		[Test]
		public void GetProjects_SolutionHasTwoProjectsAndOneProjectSelectedInProjectsBrowserAndPackageIsInstalledInProject_ReturnsProjectAndIsSelectedIsTrue()
		{
			CreateSelectedProjects();
			List<IProject> projectsAddedToSolution = AddSolutionWithTwoProjectsToProjectService();
			projectsAddedToSolution[0].Name = "Aaa";
			IProject msbuildProject = projectsAddedToSolution[1];
			msbuildProject.Name = "MyProject";
			fakeSolution.FakeActiveMSBuildProject = msbuildProject;
			
			var fakePackage = new FakePackage();
			var fakeProject = fakeSolution.AddFakeProjectToReturnFromGetProject("MyProject");
			fakeProject.FakePackages.Add(fakePackage);
			fakeSolution.AddFakeProjectToReturnFromGetProject("Aaa");

			var projects = new List<IPackageManagementSelectedProject>();
			projects.AddRange(selectedProjects.GetProjects(fakePackage));
			
			var expectedProject = new FakeSelectedProject("MyProject", selected: true);
			var expectedProjects = new List<IPackageManagementSelectedProject>();
			expectedProjects.Add(expectedProject);
			
			SelectedProjectCollectionAssert.AreEqual(expectedProjects, projects);
			Assert.AreEqual(fakePackage.FakePackageRepository, fakeSolution.RepositoryPassedToGetProject);
			Assert.AreEqual(msbuildProject, fakeSolution.ProjectPassedToGetProject);
		}
		
		[Test]
		public void HasMultipleProjects_SolutionHasTwoProjectsAndOneProjectSelectedInProjectsBrowser_ReturnsFalse()
		{
			CreateSelectedProjects();
			List<IProject> projectsAddedToSolution = AddSolutionWithTwoProjectsToProjectService();
			IProject expectedProject = projectsAddedToSolution[1];
			fakeSolution.FakeActiveMSBuildProject = expectedProject;
			
			bool hasMultipleProjects = selectedProjects.HasMultipleProjects();
			
			Assert.IsFalse(hasMultipleProjects);
		}
		
		[Test]
		public void GetProjects_SolutionHasTwoProjectsAndNoProjectSelectedInProjectsBrowser_ReturnsAllProjectsInSolutionForPackage()
		{
			CreateSelectedProjects();
			List<IProject> projectsAddedToSolution = AddSolutionWithTwoProjectsToProjectService();
			projectsAddedToSolution[0].Name = "Project A";
			projectsAddedToSolution[1].Name = "Project B";
			fakeSolution.FakeActiveProject = null;
			
			fakeSolution.AddFakeProjectToReturnFromGetProject("Project A");
			fakeSolution.AddFakeProjectToReturnFromGetProject("Project B");
			
			var fakePackage = new FakePackage();
			var projects = new List<IPackageManagementSelectedProject>();
			projects.AddRange(selectedProjects.GetProjects(fakePackage));
						
			var expectedProjects = new List<IPackageManagementSelectedProject>();
			expectedProjects.Add(new FakeSelectedProject("Project A"));
			expectedProjects.Add(new FakeSelectedProject("Project B"));
			
			SelectedProjectCollectionAssert.AreEqual(expectedProjects, projects);
		}
		
		[Test]
		public void GetProjects_SolutionHasTwoProjectsAndNoProjectSelectedInProjectsBrowserAndPackageIsInstalledInFirstProject_ReturnsAllProjectsInSolutionWithIsSelectedIsTrue()
		{
			CreateSelectedProjects();
			List<IProject> projectsAddedToSolution = AddSolutionWithTwoProjectsToProjectService();
			projectsAddedToSolution[0].Name = "Project A";
			projectsAddedToSolution[1].Name = "Project B";
			fakeSolution.FakeActiveProject = null;
			
			var fakePackage = new FakePackage("Test");
			var fakeProject = fakeSolution.AddFakeProjectToReturnFromGetProject("Project A");
			fakeProject.FakePackages.Add(fakePackage);
			fakeSolution.AddFakeProjectToReturnFromGetProject("Project B");
			
			var projects = new List<IPackageManagementSelectedProject>();
			projects.AddRange(selectedProjects.GetProjects(fakePackage));
						
			var expectedProjects = new List<IPackageManagementSelectedProject>();
			expectedProjects.Add(new FakeSelectedProject("Project A", selected: true));
			expectedProjects.Add(new FakeSelectedProject("Project B", selected: false));
			
			SelectedProjectCollectionAssert.AreEqual(expectedProjects, projects);
			Assert.AreEqual(fakePackage.FakePackageRepository, fakeSolution.RepositoryPassedToGetProject);
			Assert.AreEqual(projectsAddedToSolution, fakeSolution.ProjectsPassedToGetProject);
		}
		
		[Test]
		public void HasMultipleProjects_SolutionHasTwoProjectsAndNoProjectSelectedInProjectsBrowser_ReturnsTrue()
		{
			CreateSelectedProjects();
			List<IProject> projectsAddedToSolution = AddSolutionWithTwoProjectsToProjectService();
			fakeSolution.FakeActiveProject = null;
			
			bool hasMultipleProjects = selectedProjects.HasMultipleProjects();
			
			Assert.IsTrue(hasMultipleProjects);
		}
		
		[Test]
		public void HasMultipleProjects_SolutionHasOneProjectAndNoProjectSelectedInProjectsBrowser_ReturnsFalse()
		{
			CreateSelectedProjects();
			List<IProject> projectsAddedToSolution = AddSolutionWithOneProjectToProjectService();
			fakeSolution.FakeActiveProject = null;
			
			bool hasMultipleProjects = selectedProjects.HasMultipleProjects();
			
			Assert.IsFalse(hasMultipleProjects);
		}
		
		[Test]
		public void SelectionName_SolutionHasOneProject_ReturnsProjectNameWithoutFileExtension()
		{
			CreateSelectedProjects();
			List<IProject> projectsAddedToSolution = AddSolutionWithOneProjectToProjectService();
			projectsAddedToSolution[0].Name = "MyProject";
			fakeSolution.FakeActiveMSBuildProject = projectsAddedToSolution[0];
			
			string name = selectedProjects.SelectionName;
			
			Assert.AreEqual("MyProject", name);
		}
		
		[Test]
		public void SelectionName_SolutionHasTwoProjectsAndNoProjectSelected_ReturnsSolutionFileNameWithoutFullPath()
		{
			CreateSelectedProjects();
			List<IProject> projectsAddedToSolution = AddSolutionWithTwoProjectsToProjectService();
			NoProjectsSelected();
			fakeSolution.FileName = @"d:\projects\MyProject\MySolution.sln";
			
			string name = selectedProjects.SelectionName;
			
			Assert.AreEqual("MySolution.sln", name);
		}
		
		[Test]
		public void IsPackageInstalled_PackageInstalledInSolutionWithTwoProjectsAndNoProjectSelected_ReturnsTrue()
		{
			CreateSelectedProjects();
			List<IProject> projectsAddedToSolution = AddSolutionWithTwoProjectsToProjectService();
			NoProjectsSelected();
			
			var package = new FakePackage("Test");
			fakeSolution.FakeInstalledPackages.Add(package);
			
			bool installed = selectedProjects.IsPackageInstalled(package);
			
			Assert.IsTrue(installed);
		}
		
		[Test]
		public void IsPackageInstalled_PackageIsInstalledInSolutionWithTwoProjectsAndNoProjectSelected_ReturnsFalse()
		{
			CreateSelectedProjects();
			AddSolutionWithTwoProjectsToProjectService();
			NoProjectsSelected();
			
			var package = new FakePackage("Test");
			bool installed = selectedProjects.IsPackageInstalled(package);
			
			Assert.IsFalse(installed);
		}
		
		[Test]
		public void IsPackageInstalled_PackageIsInstalledInProjectAndProjectSelected_ReturnsTrue()
		{
			CreateSelectedProjects();
			List<IProject> projectsAddedToSolution = AddSolutionWithTwoProjectsToProjectService();
			fakeSolution.FakeActiveMSBuildProject = projectsAddedToSolution[0];
			
			var package = new FakePackage("Test");
			fakeSolution.FakeActiveProject.FakePackages.Add(package);
			
			bool installed = selectedProjects.IsPackageInstalled(package);
			
			Assert.IsTrue(installed);
		}	
		
		[Test]
		public void IsPackageInstalled_PackageIsNotInstalledInProjectAndProjectSelected_ReturnsFalse()
		{
			CreateSelectedProjects();
			List<IProject> projectsAddedToSolution = AddSolutionWithTwoProjectsToProjectService();
			fakeSolution.FakeActiveMSBuildProject = projectsAddedToSolution[0];
			
			var package = new FakePackage("Test");
			bool installed = selectedProjects.IsPackageInstalled(package);
			
			Assert.IsFalse(installed);
		}
		
		[Test]
		public void IsPackageInstalled_PackagePackageIsNotInstalledInProjectAndProjectSelected_ProjectCreatedUsingPackageRepository()
		{
			CreateSelectedProjects();
			List<IProject> projectsAddedToSolution = AddSolutionWithTwoProjectsToProjectService();
			fakeSolution.FakeActiveMSBuildProject = projectsAddedToSolution[0];
			
			var package = new FakePackage("Test");
			bool installed = selectedProjects.IsPackageInstalled(package);
			
			IPackageRepository repository = fakeSolution.RepositoryPassedToGetActiveProject;
			IPackageRepository expectedRepository = package.FakePackageRepository;
			
			Assert.AreEqual(expectedRepository, repository);
		}
		
		[Test]
		public void IsPackageInstalledInSolution_PackageInstalledInSolutionWithTwoProjectsAndOneProjectSelected_ReturnsTrue()
		{
			CreateSelectedProjects();
			List<IProject> projectsAddedToSolution = AddSolutionWithTwoProjectsToProjectService();
			fakeSolution.FakeActiveMSBuildProject = projectsAddedToSolution[0];
			
			var package = new FakePackage("Test");
			fakeSolution.FakeInstalledPackages.Add(package);
			
			bool installed = selectedProjects.IsPackageInstalledInSolution(package);
			
			Assert.IsTrue(installed);
		}
		
		[Test]
		public void IsPackageInstalledInSolution_PackageNotInstalledInSolutionWithTwoProjectsAndOneProjectSelected_ReturnsFalse()
		{
			CreateSelectedProjects();
			List<IProject> projectsAddedToSolution = AddSolutionWithTwoProjectsToProjectService();
			fakeSolution.FakeActiveMSBuildProject = projectsAddedToSolution[0];
			
			var package = new FakePackage("Test");
			
			bool installed = selectedProjects.IsPackageInstalledInSolution(package);
			
			Assert.IsFalse(installed);
		}
		
		[Test]
		public void GetPackagesInstalledInSolution_PackageInstalledInSolutionAndProjectNotSelected_ReturnsPackageInstalledInSolution()
		{
			CreateSelectedProjects();
			List<IProject> projectsAddedToSolution = AddSolutionWithTwoProjectsToProjectService();
			NoProjectsSelected();
			
			var package = new FakePackage("Test");
			fakeSolution.FakeInstalledPackages.Add(package);
			
			IQueryable<IPackage> packages = selectedProjects.GetPackagesInstalledInSolution();
			
			var expectedPackages = new FakePackage[] {
				package
			};
			
			PackageCollectionAssert.AreEqual(expectedPackages, packages);
		}
		
		[Test]
		public void GetActiveProject_ProjectSelected_ReturnsProject()
		{
			CreateSelectedProjects();
			List<IProject> projectsAddedToSolution = AddSolutionWithTwoProjectsToProjectService();
			fakeSolution.FakeActiveMSBuildProject = projectsAddedToSolution[0];
			
			var repository = new FakePackageRepository();
			IPackageManagementProject project = selectedProjects.GetActiveProject(repository);
			
			FakePackageManagementProject expectedProject = fakeSolution.FakeActiveProject;
			
			Assert.AreEqual(expectedProject, project);
		}
		
		[Test]
		public void GetActiveProject_ProjectSelectedAndRepositoryPassed_ReturnsProjectCreatedWithRepository()
		{
			CreateSelectedProjects();
			List<IProject> projectsAddedToSolution = AddSolutionWithTwoProjectsToProjectService();
			fakeSolution.FakeActiveMSBuildProject = projectsAddedToSolution[0];
			
			var repository = new FakePackageRepository();
			IPackageManagementProject project = selectedProjects.GetActiveProject(repository);
			
			Assert.AreEqual(repository, fakeSolution.RepositoryPassedToGetActiveProject);
		}
		
		[Test]
		public void HasSingleProjectSelected_SolutionHasTwoProjectsAndOneProjectSelectedInProjectsBrowser_ReturnsTrue()
		{
			CreateSelectedProjects();
			List<IProject> projectsAddedToSolution = AddSolutionWithTwoProjectsToProjectService();
			IProject expectedProject = projectsAddedToSolution[1];
			fakeSolution.FakeActiveMSBuildProject = expectedProject;
			
			bool singleProjectSelected = selectedProjects.HasSingleProjectSelected();
			
			Assert.IsTrue(singleProjectSelected);
		}
		
		[Test]
		public void HasSingleProjectSelected_SolutionHasTwoProjectsAndNoProjectsSelectedInProjectsBrowser_ReturnsFalse()
		{
			CreateSelectedProjects();
			List<IProject> projectsAddedToSolution = AddSolutionWithTwoProjectsToProjectService();
			NoProjectsSelected();
			
			bool singleProjectSelected = selectedProjects.HasSingleProjectSelected();
			
			Assert.IsFalse(singleProjectSelected);
		}
		
		[Test]
		public void HasSingleProjectSelected_NoProjectsInitiallySelectedAndProjectSelectedAfterInitialCall_IsUnchangedAndReturnsFalse()
		{
			CreateSelectedProjects();
			List<IProject> projectsAddedToSolution = AddSolutionWithTwoProjectsToProjectService();
			NoProjectsSelected();
			
			bool singleProjectSelected = selectedProjects.HasSingleProjectSelected();
			fakeSolution.FakeActiveMSBuildProject = fakeSolution.FakeMSBuildProjects[0];
			singleProjectSelected = selectedProjects.HasSingleProjectSelected();
			
			Assert.IsFalse(singleProjectSelected);
		}
		
		[Test]
		public void GetInstalledPackages_PackageInstalledInSolutionAndProjectNotSelected_ReturnsPackageInstalledInSolution()
		{
			CreateSelectedProjects();
			List<IProject> projectsAddedToSolution = AddSolutionWithTwoProjectsToProjectService();
			NoProjectsSelected();
			
			var package = new FakePackage("Test");
			fakeSolution.FakeInstalledPackages.Add(package);
			
			var repository = new FakePackageRepository();
			IQueryable<IPackage> packages = selectedProjects.GetInstalledPackages(repository);
			
			var expectedPackages = new FakePackage[] {
				package
			};
			
			PackageCollectionAssert.AreEqual(expectedPackages, packages);
		}
		
		[Test]
		public void GetInstalledPackages_PackageInstalledInProjectAndProjectIsSelected_ReturnsPackageInstalledInProject()
		{
			CreateSelectedProjects();
			List<IProject> projectsAddedToSolution = AddSolutionWithTwoProjectsToProjectService();
			fakeSolution.FakeActiveMSBuildProject = projectsAddedToSolution[0];
			
			var package = new FakePackage("Test");
			fakeSolution.FakeActiveProject.FakePackages.Add(package);
			
			var repository = new FakePackageRepository();
			IQueryable<IPackage> packages = selectedProjects.GetInstalledPackages(repository);
			
			var expectedPackages = new FakePackage[] {
				package
			};
			
			PackageCollectionAssert.AreEqual(expectedPackages, packages);
		}
		
		[Test]
		public void GetInstalledPackages_PackageInstalledInProjectAndProjectIsSelected_CreatesProjectUsingRepository()
		{
			CreateSelectedProjects();
			List<IProject> projectsAddedToSolution = AddSolutionWithTwoProjectsToProjectService();
			fakeSolution.FakeActiveMSBuildProject = projectsAddedToSolution[0];
			
			var expectedRepository = new FakePackageRepository();
			IQueryable<IPackage> packages = selectedProjects.GetInstalledPackages(expectedRepository);
			
			IPackageRepository repository = fakeSolution.RepositoryPassedToGetActiveProject;
			
			Assert.AreEqual(expectedRepository, repository);
		}
	}
}
