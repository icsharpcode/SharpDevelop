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
using ICSharpCode.Core;
using ICSharpCode.PackageManagement.Design;
using ICSharpCode.PackageManagement.EnvDTE;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Project;
using Microsoft.Build.Evaluation;
using NUnit.Framework;
using Rhino.Mocks;
using SD = ICSharpCode.SharpDevelop.Project;

namespace PackageManagement.Tests.Helpers
{
	public class SolutionHelper
	{
		public SolutionHelper(string fileName = @"d:\projects\MyProject\MyProject.sln")
		{
			ICSharpCode.SharpDevelop.SD.InitializeForUnitTests();
			OpenSolution(fileName);
		}
		
		public Solution Solution;
		public FakePackageManagementProjectService FakeProjectService;
		public SD.ISolution MSBuildSolution;
		
		void OpenSolution(string fileName)
		{
			FakeProjectService = new FakePackageManagementProjectService();
			MSBuildSolution = CreateSharpDevelopSolution(fileName);
			FakeProjectService.OpenSolution = MSBuildSolution;
			Solution = new Solution(FakeProjectService);
		}
		
		SD.ISolution CreateSharpDevelopSolution(string fileName = @"d:\projects\MyProject\MyProject.sln")
		{
			var solution = MockRepository.GenerateStub<ISolution>();
			var solutionFileName = new FileName(fileName);
			solution.Stub(s => s.FileName).Return(solutionFileName);
			solution.Stub(s => s.Directory).Return(solutionFileName.GetParentDirectory());
			
			var sections = new SimpleModelCollection<SolutionSection>(new SolutionSection[0]);
			solution.Stub(s => s.GlobalSections).Return(sections);
			
			solution.Stub(s => s.MSBuildProjectCollection).Return(new ProjectCollection());
			
			var projects = new SimpleModelCollection<IProject>();
			solution.Stub(s =>s.Projects).Return(projects);
			
			return solution;
		}
		
		public SD.ISolution OpenDifferentSolution()
		{
			SD.ISolution solution = CreateSharpDevelopSolution();
			FakeProjectService.OpenSolution = solution;
			return solution;
		}
		
		public void CloseSolution()
		{
			FakeProjectService.OpenSolution = null;
		}
		
		public TestableProject AddProjectToSolution(string projectName)
		{
			return AddProjectToSolutionWithFileName(projectName, @"c:\projects\MyProject\MyProject.csproj");
		}
		
		public TestableProject AddProjectToSolutionWithFileName(string projectName, string fileName)
		{
			TestableProject project = ProjectHelper.CreateTestProject(MSBuildSolution, projectName, fileName);
			FakeProjectService.AddProject(project);
			return project;
		}
		
		public void AddExtensibilityGlobalsSection()
		{
			var section = new SD.SolutionSection("ExtensibilityGlobals", "postSolution");
			MSBuildSolution.GlobalSections.Add(section);
		}
		
		public void AddVariableToExtensibilityGlobals(string name, string value)
		{
			SolutionSection section = GetExtensibilityGlobalsSection();
			section.Add(name, value);
		}
		
		public string GetExtensibilityGlobalsSolutionItem(string name)
		{
			SolutionSection section = GetExtensibilityGlobalsSection();
			return section[name];
		}
		
		public SD.SolutionSection GetExtensibilityGlobalsSection()
		{
			return MSBuildSolution.GlobalSections.SingleOrDefault(section => section.SectionName == "ExtensibilityGlobals");
		}
		
		public void AssertSolutionIsSaved()
		{
			Assert.AreEqual(MSBuildSolution, FakeProjectService.SavedSolution);
		}
		
		public void AssertSolutionIsNotSaved()
		{
			Assert.IsNull(FakeProjectService.SavedSolution);
		}
		
		public SD.FileProjectItem AddFileToFirstProjectInSolution(string include)
		{
			TestableProject project = FakeProjectService
				.AllProjects
				.Select(p => p as TestableProject)
				.First();
			return project.AddFile(include);
		}
		
		public SD.FileProjectItem AddFileToSecondProjectInSolution(string include)
		{
			TestableProject project = FakeProjectService
				.AllProjects
				.Select(p => p as TestableProject)
				.Skip(1)
				.First();
			return project.AddFile(include);
		}
		
		public void SetStartupProject(SD.IProject project)
		{
			MSBuildSolution.StartupProject = project;
		}
		
		public void SetActiveConfiguration(string name)
		{
			MSBuildSolution.ActiveConfiguration = new ConfigurationAndPlatform(name, "x86");
		}
	}
}
