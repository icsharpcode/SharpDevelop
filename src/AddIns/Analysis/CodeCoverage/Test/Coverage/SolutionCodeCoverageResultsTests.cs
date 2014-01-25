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
using System.Collections.Generic;
using System.IO;
using System.Linq;

using ICSharpCode.CodeCoverage.Tests.Utils;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Project;
using NUnit.Framework;
using Rhino.Mocks;
using UnitTesting.Tests.Utils;

namespace ICSharpCode.CodeCoverage.Tests.Coverage
{
	[TestFixture]
	public class SolutionCodeCoverageResultsTests : SDTestFixtureBase
	{
		SolutionCodeCoverageResults solutionCodeCoverageResults;
		ISolution solution;
		IFileSystem fakeFileSystem;
		
		void CreateSolutionCodeCoverageResults()
		{
			solution = MockSolution.Create();
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
			project.FileName = new FileName(fileName);
			((ICollection<IProject>)solution.Projects).Add(project);
		}
		
		void AddCodeCoverageFile(string fileName)
		{
			string coverageXml = "<CodeCoverage></CodeCoverage>";
			AddCodeCoverageFile(FileName.Create(fileName), coverageXml);
		}
		
		void AddCodeCoverageFile(FileName fileName, string coverageXml)
		{
			var stringReader = new StringReader(coverageXml);
			fakeFileSystem.Stub(fs => fs.FileExists(fileName)).Return(true);
			fakeFileSystem.Stub(fs => fs.OpenText(fileName)).Return(stringReader);
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
