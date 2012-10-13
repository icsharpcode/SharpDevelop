// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.PackageManagement.EnvDTE;
using NUnit.Framework;
using PackageManagement.Tests.Helpers;

namespace PackageManagement.Tests.EnvDTE
{
	[TestFixture]
	public class SolutionBuildTests
	{
		SolutionHelper solutionHelper;
		Solution solution;
		SolutionBuild solutionBuild;
		
		void CreateSolutionBuild()
		{
			solutionHelper = new SolutionHelper();
			solution = solutionHelper.Solution;
			solutionBuild = (SolutionBuild)solution.SolutionBuild;
		}
		
		void CreateSolutionBuild(string solutionFileName)
		{
			CreateSolutionBuild();
			solutionHelper.MSBuildSolution.FileName = solutionFileName;
		}
		
		void AddStartupProject(string projectFileName)
		{
			TestableProject project = solutionHelper.AddProjectToSolutionWithFileName("TestProject", projectFileName);
			solutionHelper.SetStartupProject(project);
		}
		
		[Test]
		public void StartupProjects_SolutionHasNoProjects_ReturnsEmptyArray()
		{
			CreateSolutionBuild();
			
			object startupProjects = solutionBuild.StartupProjects;
			object[] array = startupProjects as object[];
			
			Assert.AreEqual(0, array.Length);
		}
		
		[Test]
		public void StartupProjects_SolutionHasStartupProjectDefined_ReturnsArrayContainingStartupProjectFileName()
		{
			CreateSolutionBuild(@"d:\projects\MyProject\MySolution.sln");
			AddStartupProject(@"d:\projects\MyProject\MyProject.csproj");
			object[] expectedProjects = new string[] { "MyProject.csproj" };
			
			object[] projects = solutionBuild.StartupProjects as object[];
			
			CollectionAssert.AreEqual(expectedProjects, projects);
		}
	}
}
