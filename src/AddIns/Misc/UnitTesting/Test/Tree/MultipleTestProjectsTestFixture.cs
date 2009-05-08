// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.UnitTesting;
using NUnit.Framework;
using UnitTesting.Tests.Utils;

namespace UnitTesting.Tests.Tree
{
	/// <summary>
	/// When there are multiple test projects in a solution then the 
	/// test tree should have an All Tests root node.
	/// </summary>
	[TestFixture]
	public class MultipleTestProjectsTestFixture
	{
		DummyParserServiceTestTreeView treeView;
		MockCSharpProject firstProject;
		MockCSharpProject secondProject;
		AllTestsTreeNode allTestsTreeNode;
		TestProject firstTestProject;
		TestProject secondTestProject;
		Solution solution;
		
		[SetUp]
		public void SetUp()
		{
			treeView = new DummyParserServiceTestTreeView();
			
			// Create a solution with two test projects.
			solution = new Solution();
			
			// Create the first test project.
			firstProject = new MockCSharpProject(solution);
			firstProject.Name = "FirstTestProject";
			ReferenceProjectItem nunitFrameworkReferenceItem = new ReferenceProjectItem(firstProject);
			nunitFrameworkReferenceItem.Include = "NUnit.Framework";
			ProjectService.AddProjectItem(firstProject, nunitFrameworkReferenceItem);

			// Create the second test project.
			secondProject = new MockCSharpProject(solution);
			secondProject.Name = "SecondTestProject";
			nunitFrameworkReferenceItem = new ReferenceProjectItem(secondProject);
			nunitFrameworkReferenceItem.Include = "NUnit.Framework";
			ProjectService.AddProjectItem(secondProject, nunitFrameworkReferenceItem);
					
			// Add the projects to the solution.
			solution.Folders.Add(firstProject);
			solution.Folders.Add(secondProject);
			
			// Create a dummy project content so the projects will be added
			// to the tree.
			treeView.ProjectContentForProject = new MockProjectContent();
			
			// Add the solution to the tree.
			treeView.AddSolution(solution);
			
			allTestsTreeNode = treeView.Nodes[0] as AllTestsTreeNode;
			firstTestProject = treeView.GetTestProject(firstProject);
			secondTestProject = treeView.GetTestProject(secondProject);
		}
		
		[TearDown]
		public void TearDown()
		{
			treeView.Dispose();
		}
		
		[Test]
		public void OneAllTestsRootNode()
		{
			Assert.AreEqual(1, treeView.Nodes.Count);
		}
		
		[Test]
		public void RootNodeIsAllTestsNode()
		{
			Assert.IsNotNull(allTestsTreeNode);
		}
		
		[Test]
		public void AllTestsNodeText()
		{
			Assert.AreEqual(StringParser.Parse("${res:ICSharpCode.UnitTesting.AllTestsTreeNode.Text}"), treeView.Nodes[0].Text);
		}
		
		[Test]
		public void GetProjectsAfterClear()
		{
			treeView.Clear();
			Assert.AreEqual(0, treeView.GetProjects().Length);
		}
		
		/// <summary>
		/// Tests that the All Tests node is removed when only one
		/// project remains.
		/// </summary>
		[Test]
		public void ProjectRemoved()
		{
			treeView.RemoveProject(firstProject);
			
			Assert.AreEqual(1, treeView.GetProjects().Length);
			Assert.IsInstanceOf(typeof(TestProjectTreeNode), treeView.Nodes[0]);
		}
		
		/// <summary>
		/// Tests that the all test node has the correct image index.
		/// </summary>
		[Test]
		public void ProjectIgnoredTestResult()
		{
			TestClass c = new TestClass(new MockClass("Tests.TestFixture"));
			firstTestProject.TestClasses.Add(c);
			c.Result = TestResultType.Ignored;
			
			Assert.AreEqual((int)TestTreeViewImageListIndex.TestIgnored, allTestsTreeNode.ImageIndex);
		}
		
		/// <summary>
		/// Tests that a removed project does not affect the image index of the
		/// All Tests root node.
		/// </summary>
		[Test]
		public void IgnoredTestResultAfterProjectRemoved()
		{
			// Add an extra project so when we remove one the All Tests node
			// is not removed.
			MockCSharpProject project = new MockCSharpProject(solution);
			project.Name = "ThirdTestProject";
			ReferenceProjectItem nunitFrameworkReferenceItem = new ReferenceProjectItem(project);
			nunitFrameworkReferenceItem.Include = "NUnit.Framework";
			ProjectService.AddProjectItem(project, nunitFrameworkReferenceItem);
			
			// Add the project into a dummy project node.
			TestProject testProject = new TestProject(project, new MockProjectContent());
			DerivedTestProjectTreeNode projectNode = new DerivedTestProjectTreeNode(testProject);
			allTestsTreeNode.AddProjectNode(projectNode);
			
			// Remove an existing project.
			treeView.RemoveProject(project);
			
			// Modify the image index for the removed tree node.
			projectNode.CallUpdateImageListIndex(TestResultType.Ignored);
			
			// Image index of the all tests node should be unchanged.
			Assert.AreEqual((int)TestTreeViewImageListIndex.TestNotRun, allTestsTreeNode.ImageIndex);			
		}
		
		[Test]
		public void ProjectFailedTestResult()
		{
			TestClass c = new TestClass(new MockClass("Tests.TestFixture"));
			firstTestProject.TestClasses.Add(c);
			c.Result = TestResultType.Failure;
			
			Assert.AreEqual((int)TestTreeViewImageListIndex.TestFailed, allTestsTreeNode.ImageIndex);
		}
		
		[Test]
		public void ProjectPassedTestResult()
		{
			TestClass c = new TestClass(new MockClass("Tests.TestFixture"));
			firstTestProject.TestClasses.Add(c);
			c.Result = TestResultType.Success;
			
			Assert.AreEqual((int)TestTreeViewImageListIndex.TestNotRun, allTestsTreeNode.ImageIndex);
		}
		
		[Test]
		public void AllTestsNodeImageIndexReset()
		{
			// Make one of the test projects pass so the all tests
			// node's image index is changed.
			ProjectPassedTestResult();
			
			// Set the test class result back to not run.
			firstTestProject.TestClasses[0].Result = TestResultType.None;
			
			Assert.AreEqual((int)TestTreeViewImageListIndex.TestNotRun, allTestsTreeNode.ImageIndex);
		}
		
		[Test]
		public void AllTestProjectsPassed()
		{
			TestClass c = new TestClass(new MockClass("Tests.TestFixture"));
			firstTestProject.TestClasses.Add(c);
			c.Result = TestResultType.Success;
			
			c = new TestClass(new MockClass("Tests.TestFixture"));
			secondTestProject.TestClasses.Add(c);
			c.Result = TestResultType.Success;
			
			Assert.AreEqual((int)TestTreeViewImageListIndex.TestPassed, allTestsTreeNode.ImageIndex);
		}
		
		[Test]
		public void CanClearSelection()
		{
			Assert.IsFalse(treeView.CanClearSelection);
		}
	}
}
