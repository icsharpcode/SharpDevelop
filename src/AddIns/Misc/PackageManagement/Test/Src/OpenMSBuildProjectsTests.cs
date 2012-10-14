// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using ICSharpCode.PackageManagement;
using ICSharpCode.SharpDevelop.Project;
using NUnit.Framework;
using PackageManagement.Tests.Helpers;
using Rhino.Mocks;

namespace PackageManagement.Tests
{
	[TestFixture]
	public class OpenMSBuildProjectsTests
	{
		OpenMSBuildProjects projects;
		IPackageManagementProjectService fakeProjectService;
		List<IProject> openProjects;
		
		void CreateOpenMSBuildProjects()
		{
			fakeProjectService = MockRepository.GenerateStub<IPackageManagementProjectService>();
			openProjects = new List<IProject>();
			fakeProjectService.Stub(service => service.GetOpenProjects()).Return(openProjects);
			
			projects = new OpenMSBuildProjects(fakeProjectService);
		}
		
		TestableProject AddProjectWithShortName(string projectName)
		{
			TestableProject project = ProjectHelper.CreateTestProject(projectName);
			openProjects.Add(project);
			return project;
		}
		
		TestableProject AddProjectWithFileName(string fileName)
		{
			TestableProject project = AddProjectWithShortName("Test");
			project.FileName = fileName;
			return project;
		}
		
		[Test]
		public void FindProject_ProjectShortNameUsedAndProjectIsOpen_ReturnsProject()
		{
			CreateOpenMSBuildProjects();
			MSBuildBasedProject expectedProject = AddProjectWithShortName("MyProject");
			
			MSBuildBasedProject project = projects.FindProject("MyProject");
			
			Assert.AreEqual(expectedProject, project);
		}
		
		[Test]
		public void FindProject_ProjectFileNameUsedAndProjectIsOpen_ReturnsProject()
		{
			CreateOpenMSBuildProjects();
			string fileName = @"d:\projects\MyProject\MyProject.csproj";
			MSBuildBasedProject expectedProject = AddProjectWithFileName(fileName);
			
			MSBuildBasedProject project = projects.FindProject(fileName);
			
			Assert.AreEqual(expectedProject, project);
		}
	}
}
