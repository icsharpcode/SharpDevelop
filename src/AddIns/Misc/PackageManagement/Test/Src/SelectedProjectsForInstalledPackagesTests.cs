// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

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
			ISolution solution = ProjectHelper.CreateSolution();
			TestableProject project1 = ProjectHelper.CreateTestProject(solution, "Test1");
			TestableProject project2 = ProjectHelper.CreateTestProject(solution, "Test2");
			
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
