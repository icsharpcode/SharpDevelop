// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using ICSharpCode.CodeCoverage.Tests.Utils;
using ICSharpCode.SharpDevelop.Project;
using NUnit.Framework;
using Rhino.Mocks;
using UnitTesting.Tests.Utils;

namespace ICSharpCode.CodeCoverage.Tests.Coverage
{
	[TestFixture]
	public class SolutionCodeCoverageResultsTests
	{
		SolutionCodeCoverageResults solutionCodeCoverageResults;
		Solution solution;
		IFileSystem fakeFileSystem;
		
		void CreateSolutionCodeCoverageResults()
		{
			solution = new Solution(new MockProjectChangeWatcher());
			fakeFileSystem = MockRepository.GenerateStub<IFileSystem>();
			solutionCodeCoverageResults = new SolutionCodeCoverageResults(solution, fakeFileSystem);
		}
		
		IEnumerable<CodeCoverageResults> GetCodeCoverageResultsForAllProjects()
		{
			return solutionCodeCoverageResults.GetCodeCoverageResultsForAllProjects();
		}
		
		void AddProject(string fileName)
		{
			var project = new MockCSharpProject(solution, Path.GetFileNameWithoutExtension(fileName));
			project.FileName = fileName;
			solution.Folders.Add(project);
		}
		
		void AddCodeCoverageFile(string fileName)
		{
			string coverageXml = "<CodeCoverage></CodeCoverage>";
			AddCodeCoverageFile(fileName, coverageXml);
		}
		
		void AddCodeCoverageFile(string fileName, string coverageXml)
		{
			var stringReader = new StringReader(coverageXml);
			fakeFileSystem.Stub(fs => fs.FileExists(fileName)).Return(true);
			fakeFileSystem.Stub(fs => fs.CreateTextReader(fileName)).Return(stringReader);
		}
		
		[Test]
		public void GetCodeCoverageResultsForAllProjects_NoProjects_ReturnsNoResults()
		{
			CreateSolutionCodeCoverageResults();
			
			List<CodeCoverageResults> results = GetCodeCoverageResultsForAllProjects().ToList();
			
			Assert.AreEqual(0, results.Count);
		}
		
		[Test]
		public void GetCodeCoverageResultsForAllProjects_OneProject_ReturnsOneCodeCoverageResult()
		{
			CreateSolutionCodeCoverageResults();
			AddProject(@"d:\Projects\MyProject\MyProject.csproj");
			AddCodeCoverageFile(@"d:\Projects\MyProject\OpenCover\coverage.xml");
			
			List<CodeCoverageResults> results = GetCodeCoverageResultsForAllProjects().ToList();
			
			Assert.AreEqual(1, results.Count);
		}
		
		[Test]
		public void GetCodeCoverageResultsForAllProjects_OneProjectInSolutionWithoutCodeCoverageResultsFile_ReturnsNoResults()
		{
			CreateSolutionCodeCoverageResults();
			AddProject(@"d:\Projects\MyProject\MyProject.csproj");
			
			List<CodeCoverageResults> results = GetCodeCoverageResultsForAllProjects().ToList();
			
			Assert.AreEqual(0, results.Count);
		}
	}
}
