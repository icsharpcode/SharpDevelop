// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
