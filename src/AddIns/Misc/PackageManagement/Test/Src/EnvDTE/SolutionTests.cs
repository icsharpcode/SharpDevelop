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
using ICSharpCode.PackageManagement.Design;
using ICSharpCode.PackageManagement.EnvDTE;
using NUnit.Framework;
using PackageManagement.Tests.Helpers;
using SD = ICSharpCode.SharpDevelop.Project;

namespace PackageManagement.Tests.EnvDTE
{
	[TestFixture]
	public class SolutionTests
	{
		SolutionHelper solutionHelper;
		Solution solution;
		
		void CreateSolution()
		{
			solutionHelper = new SolutionHelper();
			solution = solutionHelper.Solution;
		}
		
		SD.ISolution OpenDifferentSolution()
		{
			return solutionHelper.OpenDifferentSolution();
		}
		
		void NoOpenSolution()
		{
			solutionHelper.CloseSolution();
		}
		
		void AddProjectToSolution(string projectName)
		{
			solutionHelper.AddProjectToSolution(projectName);
		}
		
		void AddProjectToSolutionWithFileName(string projectName, string fileName)
		{
			solutionHelper.AddProjectToSolutionWithFileName(projectName, fileName);
		}
		
		void AddFileToFirstProjectInSolution(string fileName)
		{
			solutionHelper.AddFileToFirstProjectInSolution(fileName);
		}
		
		void AddFileToSecondProjectInSolution(string fileName)
		{
			solutionHelper.AddFileToSecondProjectInSolution(fileName);
		}
		
		[Test]
		public void IsOpen_NoOpenSolution_ReturnsFalse()
		{
			CreateSolution();
			NoOpenSolution();
			
			bool open = solution.IsOpen;
			
			Assert.IsFalse(open);
		}
		
		[Test]
		public void IsOpen_SolutionOpenInSharpDevelop_ReturnsTrue()
		{
			CreateSolution();
			
			bool open = solution.IsOpen;
			
			Assert.IsTrue(open);
		}

		[Test]
		public void IsOpen_DifferentSolutionOpenInSharpDevelop_ReturnsFalse()
		{
			CreateSolution();
			OpenDifferentSolution();
			
			bool open = solution.IsOpen;
			
			Assert.IsFalse(open);
		}
		
		[Test]
		public void Projects_SolutionHasNoProjects_NoProjectsInCollection()
		{
			CreateSolution();
			
			Projects projects = (Projects)solution.Projects;
			int count = projects.ToList().Count;
			
			Assert.AreEqual(0, count);
		}
		
		[Test]
		public void Projects_SolutionHasOnProject_OneProjectInCollection()
		{
			CreateSolution();
			AddProjectToSolution("MyProject");
			
			Projects projects = (Projects)solution.Projects;
			Project project = projects.First();
			
			Assert.AreEqual("MyProject", project.Name);
		}
		
		[Test]
		public void FindProjectItem_SolutionHasOneProjectWithNoItems_ReturnsNull()
		{
			CreateSolution();
			AddProjectToSolution("MyProject");
			
			global::EnvDTE.ProjectItem item = solution.FindProjectItem(@"c:\projects\MyProject\test.cs");
			
			Assert.IsNull(item);
		}
		
		[Test]
		public void FindProjectItem_SolutionHasOneProjectWithOneItemMatchingFileNamePassedToFindProjectItem_ReturnsProjectItem()
		{
			CreateSolution();
			AddProjectToSolutionWithFileName("MyProject", @"c:\projects\MyProject\MyProject.csproj");
			AddFileToFirstProjectInSolution(@"src\test.cs");
			string fileName = @"c:\projects\MyProject\src\test.cs";
			
			global::EnvDTE.ProjectItem item = solution.FindProjectItem(fileName);
			
			Assert.AreEqual("test.cs", item.Name);
		}
		
		[Test]
		public void FindProjectItem_SolutionHasOneProjectWithOneItemMatchingFileNamePassedToFindProjectItem_ProjectItemHasNonNullContainingProject()
		{
			CreateSolution();
			AddProjectToSolutionWithFileName("MyProject", @"c:\projects\MyProject\MyProject.csproj");
			AddFileToFirstProjectInSolution(@"src\test.cs");
			string fileName = @"c:\projects\MyProject\src\test.cs";
			
			global::EnvDTE.ProjectItem item = solution.FindProjectItem(fileName);
			
			Assert.AreEqual(@"c:\projects\MyProject\MyProject.csproj", item.ContainingProject.FileName);
		}
		
		[Test]
		public void FindProjectItem_SolutionHasTwoProjectsWithOneItemMatchingFileNameInSecondProject_ReturnsProjectItem()
		{
			CreateSolution();
			AddProjectToSolutionWithFileName("MyProject1", @"c:\projects\MyProject1\MyProject1.csproj");
			AddProjectToSolutionWithFileName("MyProject2", @"c:\projects\MyProject2\MyProject2.csproj");
			AddFileToSecondProjectInSolution(@"src\test.cs");
			string fileName = @"c:\projects\MyProject2\src\test.cs";
			
			global::EnvDTE.ProjectItem item = solution.FindProjectItem(fileName);
			
			Assert.AreEqual("test.cs", item.Name);
		}
		
		[Test]
		public void SolutionBuild_SolutionNotBuilt_ReturnsSolutionBuildWithNoProjectsFailingToBuild()
		{
			CreateSolution();
			
			global::EnvDTE.SolutionBuild solutionBuild = solution.SolutionBuild;
			int lastBuildInfo = solutionBuild.LastBuildInfo;
			
			Assert.AreEqual(0, lastBuildInfo);
		}
	}
}
