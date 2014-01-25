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
using System.Windows.Forms;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.UnitTesting;
using NUnit.Framework;
using UnitTesting.Tests.Utils;

namespace UnitTesting.Tests.Tree
{
	public class NonTestProjectNotAddedToTestTreeTestFixture
	{
		Solution solution;
		MSBuildBasedProject testProject;
		MSBuildBasedProject nonTestProject;
		DummyParserServiceTestTreeView treeView;
		IProject[] projects;
		MockRegisteredTestFrameworks testFrameworks;
		
		[SetUp]
		public void Init()
		{
			solution = new Solution(new MockProjectChangeWatcher());
			testProject = new MockCSharpProject(solution, "A");
			solution.Folders.Add(testProject);
			
			nonTestProject = new MockCSharpProject(solution, "Z");
			solution.Folders.Add(nonTestProject);
			
			MockProjectContent projectContent = new MockProjectContent();
			
			testFrameworks = new MockRegisteredTestFrameworks();
			testFrameworks.AddTestProject(testProject);
			
			treeView = new DummyParserServiceTestTreeView(testFrameworks);
			treeView.ProjectContentForProject = projectContent;
			treeView.AddSolution(solution);
			projects = treeView.GetProjects();
		}
		
		[TearDown]
		public void TearDown()
		{
			if (treeView != null) {
				treeView.Dispose();
			}
		}
		
		[Test]
		public void OneProjectInTestTree()
		{
			Assert.AreEqual(1, projects.Length);
		}
		
		[Test]
		public void TestProjectIsInTree()
		{
			bool found = false;
			foreach (IProject project in projects) {
				found = Object.ReferenceEquals(project, testProject);
				if (found) {
					break;
				}
			}
			Assert.IsTrue(found);
		}
		
		[Test]
		public void asdfasdf()
		{
			Assert.IsNotNull(testFrameworks.IsTestProjectParameterUsed);
		}
	}
}
