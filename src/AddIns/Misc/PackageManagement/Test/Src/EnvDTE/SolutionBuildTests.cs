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
		
		void CreateSolutionBuild(string solutionFileName = @"d:\projects\MyProject\MySolution.sln")
		{
			solutionHelper = new SolutionHelper(solutionFileName);
			solution = solutionHelper.Solution;
			projectBuilder = solutionHelper.FakeProjectService.FakeProjectBuilder;
			solutionBuild = (SolutionBuild)solution.SolutionBuild;
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
