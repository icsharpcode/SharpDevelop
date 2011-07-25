// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Linq;

using ICSharpCode.PackageManagement;
using ICSharpCode.PackageManagement.Design;
using ICSharpCode.SharpDevelop.Project;
using NUnit.Framework;
using PackageManagement.Tests.Helpers;

namespace PackageManagement.Tests
{
	[TestFixture]
	public class SelectedProjectsForInstalledPackagesTests
	{
		SelectedProjectsForInstalledPackages selectedProjects;
		FakePackageManagementSolution fakeSolution;

		void CreateFakeSolution()
		{
			fakeSolution = new FakePackageManagementSolution();
		}
		
		void CreateSelectedProjects()
		{
			selectedProjects = new SelectedProjectsForInstalledPackages(fakeSolution);
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
		
		[Test]
		public void GetProjects_SolutionHasTwoProjectsAndOneProjectSelectedInProjectsBrowserAndPackageIsInstalledInProject_ReturnsProjectAndIsSelectedIsTrue()
		{
			CreateFakeSolution();
			List<IProject> projectsAddedToSolution = AddSolutionWithTwoProjectsToProjectService();
			projectsAddedToSolution[0].Name = "Aaa";
			IProject msbuildProject = projectsAddedToSolution[1];
			msbuildProject.Name = "MyProject";
			fakeSolution.FakeActiveMSBuildProject = msbuildProject;
			
			var fakePackage = new FakePackage();
			var fakeProject = fakeSolution.AddFakeProjectToReturnFromGetProject("MyProject");
			fakeProject.FakePackages.Add(fakePackage);
			fakeSolution.AddFakeProjectToReturnFromGetProject("Aaa");

			CreateSelectedProjects();
			
			List<IPackageManagementSelectedProject> projects =
				selectedProjects.GetProjects(fakePackage).ToList();
			
			var expectedProject = new FakeSelectedProject("MyProject", selected: true);
			var expectedProjects = new List<IPackageManagementSelectedProject>();
			expectedProjects.Add(expectedProject);
			
			SelectedProjectCollectionAssert.AreEqual(expectedProjects, projects);
			Assert.AreEqual(fakePackage.FakePackageRepository, fakeSolution.RepositoryPassedToGetProject);
			Assert.AreEqual(msbuildProject, fakeSolution.ProjectPassedToGetProject);
		}
		
		[Test]
		public void GetProjects_SolutionHasTwoProjectsAndNoProjectSelectedInProjectsBrowserAndPackageIsInstalledInFirstProject_ReturnsAllProjectsInSolutionWithIsSelectedIsTrue()
		{
			CreateFakeSolution();
			List<IProject> projectsAddedToSolution = AddSolutionWithTwoProjectsToProjectService();
			projectsAddedToSolution[0].Name = "Project A";
			projectsAddedToSolution[1].Name = "Project B";
			fakeSolution.FakeActiveProject = null;
			
			var fakePackage = new FakePackage("Test");
			var fakeProject = fakeSolution.AddFakeProjectToReturnFromGetProject("Project A");
			fakeProject.FakePackages.Add(fakePackage);
			fakeSolution.AddFakeProjectToReturnFromGetProject("Project B");
			
			CreateSelectedProjects();
			
			List<IPackageManagementSelectedProject> projects =
				selectedProjects.GetProjects(fakePackage).ToList();
						
			var expectedProjects = new List<IPackageManagementSelectedProject>();
			expectedProjects.Add(new FakeSelectedProject("Project A", selected: true));
			expectedProjects.Add(new FakeSelectedProject("Project B", selected: false));
			
			SelectedProjectCollectionAssert.AreEqual(expectedProjects, projects);
			Assert.AreEqual(fakePackage.FakePackageRepository, fakeSolution.RepositoryPassedToGetProject);
			Assert.AreEqual(projectsAddedToSolution, fakeSolution.ProjectsPassedToGetProject);
		}
	}
}
