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
using ICSharpCode.SharpDevelop.Workbench;
using NUnit.Framework;
using Rhino.Mocks;

namespace ICSharpCode.SharpDevelop.Project
{
	public class AddExistingProjectTests : SDTestFixtureBase
	{
		public override void FixtureSetUp()
		{
			base.FixtureSetUp();
			SD.Services.AddStrictMockService<IProjectService>();
			SD.ProjectService.Stub(p => p.CurrentSolution).Return(null);
			SD.ProjectService.Stub(p => p.LoadProject(Arg<ProjectLoadInformation>.Is.Anything)).Do(new Func<ProjectLoadInformation, IProject>(LoadProject));
		}
		
		[SetUp]
		public void SetUp()
		{
			projectGuid = Guid.Parse("5AA08931-8802-42FE-A489-1A1CCF3B59E6");
		}
		
		Guid projectGuid; // GUID of next project to be loaded
		
		IProject LoadProject(ProjectLoadInformation info)
		{
			var project = MockRepository.GenerateStrictMock<IProject>();
			project.Stub(p => p.IdGuid).PropertyBehavior();
			project.IdGuid = projectGuid;
			project.Stub(p => p.FileName).Return(info.FileName);
			project.Stub(p => p.ParentSolution).Return(info.Solution);
			project.Stub(p => p.ParentFolder).PropertyBehavior();
			project.Stub(p => p.ProjectSections).Return(new SimpleModelCollection<SolutionSection>());
			project.Stub(p => p.ConfigurationMapping).Return(new ConfigurationMapping());
			project.Stub(p => p.IsStartable).Return(false);
			project.Stub(p => p.ProjectLoaded()).Do(new Action(delegate { }));
			return project;
		}
		
		readonly FileName project1FileName = new FileName(@"c:\AddExistingProjectTests\Project1.csproj");
		readonly FileName project2FileName = new FileName(@"c:\AddExistingProjectTests\Project2.csproj");
		
		ISolution CreateSolution()
		{
			var fileName = new FileName(@"c:\AddExistingProjectTests\AddExistingProjectTests.sln");
			return new Solution(fileName, MockRepository.GenerateStub<IProjectChangeWatcher>(), MockRepository.GenerateStub<IFileService>());
		}
		
		[Test]
		public void AddingProjectUsesExistingGUID()
		{
			var solution = CreateSolution();
			var project = solution.AddExistingProject(project1FileName);
			Assert.AreEqual(projectGuid, project.IdGuid);
		}
		
		[Test, ExpectedException(typeof(ProjectLoadException))]
		public void AddDuplicateProject()
		{
			var solution = CreateSolution();
			// add project1 twice:
			solution.AddExistingProject(project1FileName);
			solution.AddExistingProject(project1FileName);
		}
		
		[Test]
		public void AddTwoProjectsWithSameGUID()
		{
			var solution = CreateSolution();
			var project1 = solution.AddExistingProject(project1FileName);
			var project2 = solution.AddExistingProject(project2FileName);
			Assert.AreEqual(projectGuid, project1.IdGuid);
			Assert.AreNotEqual(projectGuid, project2.IdGuid);
		}
	}
}
