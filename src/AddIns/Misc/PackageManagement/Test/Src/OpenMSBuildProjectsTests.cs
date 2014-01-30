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
using ICSharpCode.Core;
using ICSharpCode.PackageManagement;
using ICSharpCode.SharpDevelop.Dom;
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
		SimpleModelCollection<IProject> openProjects;
		
		void CreateOpenMSBuildProjects()
		{
			fakeProjectService = MockRepository.GenerateStub<IPackageManagementProjectService>();
			openProjects = new SimpleModelCollection<IProject>();
			fakeProjectService.Stub(service => service.AllProjects).Return(openProjects);
			
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
			project.FileName = new FileName(fileName);
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
