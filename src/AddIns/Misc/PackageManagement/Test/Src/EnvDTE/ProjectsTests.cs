// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
			solutionHelper = new SolutionHelper();
			solutionHelper.MSBuildSolution.FileName = fileName;
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
