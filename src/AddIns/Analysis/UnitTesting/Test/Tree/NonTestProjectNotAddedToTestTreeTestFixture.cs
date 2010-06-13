// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

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
			solution = new Solution();
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
