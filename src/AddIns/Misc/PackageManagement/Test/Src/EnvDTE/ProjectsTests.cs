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
using System.Linq;
using ICSharpCode.PackageManagement.EnvDTE;
using NUnit.Framework;
using PackageManagement.Tests.Helpers;

namespace PackageManagement.Tests.EnvDTE
{
	[TestFixture]
	public class ProjectsTests
	{
		Projects projects;
		SolutionHelper solutionHelper;
		
		void CreateSolutionWithSingleProject(string projectName)
		{
			solutionHelper = new SolutionHelper();
			solutionHelper.AddProjectToSolution(projectName);
			projects = (Projects)solutionHelper.Solution.Projects;
		}
		
		void CreateSolutionWithTwoProjects(string projectName1, string projectName2)
		{
			solutionHelper = new SolutionHelper();
			TestableProject project = solutionHelper.AddProjectToSolutionWithFileName(projectName1, @"d:\projects\" + projectName1 + ".csproj");
			solutionHelper.AddProjectToSolutionWithFileName(projectName2, @"d:\projects\" + projectName2 + ".csproj");
			projects = (Projects)solutionHelper.Solution.Projects;
		}
		
		void CreateSolution(string fileName)
		{
			solutionHelper = new SolutionHelper(fileName);
			projects = (Projects)solutionHelper.Solution.Projects;
		}
		
		void AddProjectToSolution(string fileName)
		{
			solutionHelper.AddProjectToSolutionWithFileName("MyProject", fileName);
		}
		
		[Test]
		public void Item_OneProjectAndFirstItemRequested_ReturnsProject()
		{
			CreateSolutionWithSingleProject("MyProject");
			
			global::EnvDTE.Project project = projects.Item(1);
			
			Assert.AreEqual("MyProject", project.Name);
		}
		
		[Test]
		public void Item_TwoProjectsAndSecondItemRequested_ReturnsSecondProject()
		{
			CreateSolutionWithTwoProjects("MyProject1", "MyProject2");
			
			global::EnvDTE.Project project = projects.Item(2);
			
			Assert.AreEqual("MyProject2", project.Name);
		}
		
		[Test]
		public void Count_OneProject_ReturnsOne()
		{
			CreateSolutionWithSingleProject("MyProject");
			
			int count = projects.Count;
			
			Assert.AreEqual(1, count);
		}
		
		[Test]
		public void Count_TwoProjects_ReturnsTwo()
		{
			CreateSolutionWithTwoProjects("MyProject1", "MyProject2");
			
			int count = projects.Count;
			
			Assert.AreEqual(2, count);
		}
		
		[Test]
		public void Item_GetProjectByUniqueName_ReturnsProject()
		{
			CreateSolution(@"d:\projects\MyProject\MySolution.sln");
			AddProjectToSolution(@"d:\projects\MyProject\SubFolder\MyProject.csproj");
			
			Project project = (Project)projects.Item(@"SubFolder\MyProject.csproj");
			
			Assert.AreEqual(@"d:\projects\MyProject\SubFolder\MyProject.csproj", project.FileName);
		}
	}
}
