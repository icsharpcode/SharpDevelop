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
using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.UnitTesting;
using NUnit.Framework;
using UnitTesting.Tests.Utils;

namespace UnitTesting.Tests.Tree
{
	[TestFixture]
	public class EmptyUnitTestsPadTests
	{
		EmptyUnitTestsPad pad;
		Solution solution;
		
		void CreatePadWithNullSolution()
		{
			CreatePad(null);
		}
		
		void CreatePad(Solution solution)
		{
			pad = new EmptyUnitTestsPad(solution);
		}
		
		void CreatePadWithSolution()
		{
			solution = new Solution(new MockProjectChangeWatcher());
			CreatePad(solution);
		}
		
		IProject AddProjectToSolution()
		{
			var project = new MockCSharpProject(solution, "MyProject");
			solution.AddFolder(project);
			return project;
		}
		
		[Test]
		public void GetProjects_NullSolution_ReturnsNoProjects()
		{
			CreatePadWithNullSolution();
			
			int count = pad.GetProjects().Length;
			
			Assert.AreEqual(0, count);
		}
		
		[Test]
		public void GetProjects_SolutionHasOneProject_ReturnsOneProject()
		{
			CreatePadWithSolution();
			IProject project = AddProjectToSolution();
			var expectedProjects = new IProject[] { project };
			
			IProject[] projects = pad.GetProjects();
			
			CollectionAssert.AreEqual(expectedProjects, projects);
		}
	}
}
