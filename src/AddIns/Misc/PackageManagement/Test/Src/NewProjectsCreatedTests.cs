// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using ICSharpCode.PackageManagement;
using ICSharpCode.PackageManagement.Design;
using ICSharpCode.SharpDevelop.Internal.Templates;
using ICSharpCode.SharpDevelop.Project;
using NUnit.Framework;
using PackageManagement.Tests.Helpers;

namespace PackageManagement.Tests
{
	[TestFixture]
	public class NewProjectsCreatedTests
	{
		FakePackageManagementProjectService fakeProjectService;
		NewProjectsCreated newProjectsCreated;
		
		TestableProject CreateProject(string name)
		{
			return ProjectHelper.CreateTestProject(name);
		}
		
		ProjectCreateInformation CreateProjectCreateInfo(MSBuildBasedProject project)
		{
			var projects = new List<MSBuildBasedProject>();
			projects.Add(project);
			return CreateProjectCreateInfo(projects);
		}
		
		ProjectCreateInformation CreateProjectCreateInfo(IEnumerable<MSBuildBasedProject> projects)
		{
			return new ProjectCreateInformation(projects);
		}
		
		void CreateNewProjectsCreated(ProjectCreateInformation createInfo)
		{
			fakeProjectService = new FakePackageManagementProjectService();
			newProjectsCreated = new NewProjectsCreated(createInfo, fakeProjectService);
		}
		
		TestableProject AddProjectToProjectServiceOpenProjects(string projectName)
		{
			TestableProject project = CreateProject(projectName);
			fakeProjectService.FakeOpenProjects.Add(project);
			return project;
		}
		
		[Test]
		public void GetProjects_OneProjectCreatedAndOneOldProjectInSolution_ReturnsProjectFromProjectService()
		{
			TestableProject project = CreateProject("TestProject");
			ProjectCreateInformation createInfo = CreateProjectCreateInfo(project);
			CreateNewProjectsCreated(createInfo);
			AddProjectToProjectServiceOpenProjects("OriginalProject");
			TestableProject expectedProject = AddProjectToProjectServiceOpenProjects("TestProject");
			
			var projects = new List<MSBuildBasedProject>();
			projects.AddRange(newProjectsCreated.GetProjects());
			
			var expectedProjects = new MSBuildBasedProject[] {
				expectedProject
			};
			
			Assert.AreEqual(expectedProjects, projects);
		}
		
		[Test]
		public void GetProjects_OneProjectCreatedWithDifferentNameCase_ReturnsProjectFromProjectService()
		{
			TestableProject project = CreateProject("TESTPROJECT");
			ProjectCreateInformation createInfo = CreateProjectCreateInfo(project);
			CreateNewProjectsCreated(createInfo);
			TestableProject expectedProject = AddProjectToProjectServiceOpenProjects("TestProject");
			
			var projects = new List<MSBuildBasedProject>();
			projects.AddRange(newProjectsCreated.GetProjects());
			
			var expectedProjects = new MSBuildBasedProject[] {
				expectedProject
			};
			
			Assert.AreEqual(expectedProjects, projects);
		}
		
		[Test]
		public void GetProjects_OneProjectCreatedButProjectIsNotOpen_ReturnsNoProjects()
		{
			TestableProject project = CreateProject("TestProject");
			ProjectCreateInformation createInfo = CreateProjectCreateInfo(project);
			CreateNewProjectsCreated(createInfo);
			
			var projects = new List<MSBuildBasedProject>();
			projects.AddRange(newProjectsCreated.GetProjects());
			
			Assert.AreEqual(0, projects.Count);
		}
	}
}
