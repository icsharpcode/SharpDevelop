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
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.SharpDevelop.Workbench;
using Microsoft.Build.Execution;
using NUnit.Framework;
using Rhino.Mocks;

namespace ICSharpCode.SharpDevelop
{
	[TestFixture]
	public class SolutionTests
	{
		ISolution CreateSolution()
		{
			IFileService fileService = MockRepository.GenerateStub<IFileService>();
			IProjectChangeWatcher changeWatcher = MockRepository.GenerateStub<IProjectChangeWatcher>();
			return new Solution(FileName.Create(@"d:\projects\MyProject\MySolution.sln"), changeWatcher, fileService);
		}
		
		/// <summary>
		/// Create a dummy project that can be added to a solution.
		/// </summary>
		IProject CreateProject(ISolution parentSolution)
		{
			var project = MockRepository.GenerateStrictMock<IProject>();
			project.Stub(p => p.ParentSolution).Return(parentSolution);
			project.Stub(p => p.ParentFolder).PropertyBehavior();
			project.Stub(p => p.ProjectSections).Return(new SimpleModelCollection<SolutionSection>());
			project.Stub(p => p.ConfigurationMapping).Return(new ConfigurationMapping());
			project.Stub(p => p.IsStartable).Return(false);
			return project;
		}
		
		[Test]
		public void UpdateMSBuildProperties_SolutionHasFileName_SolutionDefinesSolutionDirMSBuildPropertyWithDirectoryEndingInSlash()
		{
			var solution = CreateSolution();
			
			ProjectPropertyInstance property = solution.MSBuildProjectCollection.GetGlobalProperty("SolutionDir");
			string solutionDir = property.EvaluatedValue;
			
			string expectedSolutionDir = @"d:\projects\MyProject\";
			Assert.AreEqual(expectedSolutionDir, solutionDir);
		}
		
		[Test]
		public void AddProjectToDisconnectedFolder()
		{
			var solution = CreateSolution();
			var folder = solution.CreateFolder("folder");
			solution.Items.Remove(folder);
			
			var project = CreateProject(solution);
			folder.Items.Add(project);
			Assert.IsEmpty(solution.Projects);
		}
	}
}
