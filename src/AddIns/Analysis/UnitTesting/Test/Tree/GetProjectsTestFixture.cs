// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Windows.Forms;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.UnitTesting;
using NUnit.Framework;
using UnitTesting.Tests.Utils;

namespace UnitTesting.Tests.Tree
{
	/// <summary>
	/// Tests TestTreeView.GetMethods.
	/// </summary>
	[TestFixture]
	public class GetProjectsTestFixture
	{
		Solution solution;
		MSBuildBasedProject project1;
		MSBuildBasedProject project2;
		IProject[] projects;
		DummyParserServiceTestTreeView treeView;
		MockTestFrameworksWithNUnitFrameworkSupport testFrameworks;
		
		[SetUp]
		public void Init()
		{
			solution = new Solution(new MockProjectChangeWatcher());
			project1 = new MockCSharpProject(solution, "A");
			ReferenceProjectItem refProjectItem = new ReferenceProjectItem(project1);
			refProjectItem.Include = "NUnit.Framework";
			ProjectService.AddProjectItem(project1, refProjectItem);
			solution.Folders.Add(project1);
			
			project2 = new MockCSharpProject(solution, "Z");
			refProjectItem = new ReferenceProjectItem(project2);
			refProjectItem.Include = "NUnit.Framework";
			ProjectService.AddProjectItem(project2, refProjectItem);
			solution.Folders.Add(project2);
			
			MockProjectContent projectContent = new MockProjectContent();
			projectContent.Project = project1;
			
			testFrameworks = new MockTestFrameworksWithNUnitFrameworkSupport();
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
		public void TwoProjects()
		{
			Assert.AreEqual(2, projects.Length);
		}
		
		[Test]
		public void Project1Matches()
		{
			bool found = false;
			foreach (IProject project in projects) {
				found = Object.ReferenceEquals(project, project1);
				if (found) {
					break;
				}
			}
			Assert.IsTrue(found);
		}
		
		[Test]
		public void Project2Matches()
		{
			bool found = false;
			foreach (IProject project in projects) {
				found = Object.ReferenceEquals(project, project2);
				if (found) {
					break;
				}
			}
			Assert.IsTrue(found);
		}
		
		/// <summary>
		/// Tests that the TestTreeView.ResetTestResults method 
		/// resets all the test results.
		/// </summary>
		[Test]
		public void ResetTestResults()
		{
			foreach (IProject project in projects) {
				TestProject testProject = treeView.GetTestProject(project);
				MockClass mockClass = new MockClass("MyTestFixture");
				TestClass testClass = new TestClass(mockClass, testFrameworks);
				testClass.Result = TestResultType.Failure;
				testProject.TestClasses.Add(testClass);
				Assert.AreEqual(testProject.TestClasses.Result, TestResultType.Failure);
			}
			
			treeView.ResetTestResults();
			foreach (IProject project in projects) {
				TestProject testProject = treeView.GetTestProject(project);
				Assert.AreEqual(testProject.TestClasses.Result, TestResultType.None);
			}
		}
	}
}
