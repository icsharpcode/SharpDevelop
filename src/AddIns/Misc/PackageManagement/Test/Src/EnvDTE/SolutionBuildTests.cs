// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.PackageManagement;
using ICSharpCode.PackageManagement.Design;
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
		FakeProjectBuilder projectBuilder;
		
		void CreateSolutionBuild()
		{
			solutionHelper = new SolutionHelper();
			solution = solutionHelper.Solution;
			projectBuilder = solutionHelper.FakeProjectService.FakeProjectBuilder;
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
		
		void SetActiveConfiguration(string name)
		{
			solutionHelper.SetActiveConfiguration(name);
		}
		
		void BuildProjectResultHasBuildError()
		{
			var error = new ICSharpCode.SharpDevelop.Project.BuildError();
			projectBuilder.BuildResults.Add(error);
		}
		
		void BuildProjectFails()
		{
			CreateSolutionBuild(@"d:\projects\MyProject\MySolution.sln");
			string projectFileName = @"d:\projects\MyProject\MyProject.csproj";
			solutionHelper.AddProjectToSolutionWithFileName("MyProject", projectFileName);
			BuildProjectResultHasBuildError();
			solutionBuild.BuildProject("Debug", "MyProject.csproj", true);
		}
		
		void BuildProjectSucceeds()
		{
			CreateSolutionBuild(@"d:\projects\MyProject\MySolution.sln");
			string projectFileName = @"d:\projects\MyProject\MyProject.csproj";
			solutionHelper.AddProjectToSolutionWithFileName("MyProject", projectFileName);
			solutionBuild.BuildProject("Debug", "MyProject.csproj", true);
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
		
		[Test]
		public void ActiveConfiguration_SolutionHasDebugAsActiveConfig_ReturnsDebugForActiveConfigName()
		{
			CreateSolutionBuild();
			SetActiveConfiguration("Debug");
			
			global::EnvDTE.SolutionConfiguration config = solutionBuild.ActiveConfiguration;
			string name = config.Name;
			
			Assert.AreEqual("Debug", name);
		}
		
		[Test]
		public void BuildProject_OneProjectInSolutionAndUniqueNameMatchesProject_ProjectPassedToProjectBuilder()
		{
			CreateSolutionBuild(@"d:\projects\MyProject\MySolution.sln");
			string projectFileName = @"d:\projects\MyProject\MyProject.csproj";
			TestableProject expectedProject = solutionHelper.AddProjectToSolutionWithFileName("MyProject", projectFileName);
			
			solutionBuild.BuildProject("Debug", "MyProject.csproj", true);
			
			Assert.AreEqual(expectedProject, projectBuilder.ProjectPassedToBuild);
		}
		
		[Test]
		public void BuildProject_ProjectIsSecondProjectInSolution_CorrectProjectPassedToProjectBuilder()
		{
			CreateSolutionBuild(@"d:\projects\MyProject\MySolution.sln");
			solutionHelper.AddProjectToSolution("FirstProject");
			string projectFileName = @"d:\projects\MyProject\MyProject.csproj";
			TestableProject expectedProject = solutionHelper.AddProjectToSolutionWithFileName("MyProject", projectFileName);
			
			solutionBuild.BuildProject("Debug", "MyProject.csproj", true);
			
			Assert.AreEqual(expectedProject, projectBuilder.ProjectPassedToBuild);
		}
		
		[Test]
		public void BuildProject_OneProjectInSubFolderInsideSolutionFolderAndUniqueNameMatchesProject_ProjectPassedToProjectBuilder()
		{
			CreateSolutionBuild(@"d:\projects\MyProject\MySolution.sln");
			string projectFileName = @"d:\projects\MyProject\SubFolder\MyProject.csproj";
			TestableProject expectedProject = solutionHelper.AddProjectToSolutionWithFileName("MyProject", projectFileName);
			
			solutionBuild.BuildProject("Debug", @"SubFolder\MyProject.csproj", true);
			
			Assert.AreEqual(expectedProject, projectBuilder.ProjectPassedToBuild);
		}
		
		[Test]
		public void LastBuildInfo_BuildProjectFails_LastBuildInfoIsOne()
		{
			BuildProjectFails();
			
			int result = solutionBuild.LastBuildInfo;
			
			Assert.AreEqual(1, result);
		}
		
		[Test]
		public void LastBuildInfo_BuildProjectFails_SolutionBuildLastBuildInfoUnchangedWhenAccessedViaSolution()
		{
			BuildProjectFails();
			
			int result = solution.SolutionBuild.LastBuildInfo;
			
			Assert.AreEqual(1, result);
		}
		
		[Test]
		public void LastBuildInfo_BuildProjectSucceeds_LastBuildInfoIsZero()
		{
			BuildProjectSucceeds();
			
			int result = solution.SolutionBuild.LastBuildInfo;
			
			Assert.AreEqual(0, result);
		}
	}
}
