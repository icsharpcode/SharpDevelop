// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Windows.Forms;

using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.UnitTesting;
using NUnit.Framework;
using UnitTesting.Tests.Utils;

namespace UnitTesting.Tests.Tree
{
	/// <summary>
	/// Tests that when a namespace tree node has a class and another
	/// namespace as a child the image index is updated correctly.
	/// </summary>
	[TestFixture]
	public class TwoTestClassesInDifferentNamesTestFixture
	{
		Solution solution;
		ExtTreeNode rootNode;
		TreeNodeCollection nodes;
		DummyParserServiceTestTreeView dummyTreeView;
		TestTreeView treeView;
		MSBuildBasedProject project;
		MockClass testClass1;
		MockClass testClass2;
		ExtTreeNode projectNamespaceNode;
		TestProject testProject;
		MockProjectContent projectContent;
		MockTestFrameworksWithNUnitFrameworkSupport testFrameworks;
		
		[SetUp]
		public void SetUp()
		{
			solution = new Solution(new MockProjectChangeWatcher());
			
			// Create a project to display in the test tree view.
			project = new MockCSharpProject(solution, "TestProject");
			ReferenceProjectItem nunitFrameworkReferenceItem = new ReferenceProjectItem(project);
			nunitFrameworkReferenceItem.Include = "NUnit.Framework";
			ProjectService.AddProjectItem(project, nunitFrameworkReferenceItem);
			
			// Add a test class with a TestFixture attributes.
			projectContent = new MockProjectContent();
			projectContent.Language = LanguageProperties.None;
			testClass1 = new MockClass(projectContent, "Project.Tests.MyTestFixture");
			testClass1.Attributes.Add(new MockAttribute("TestFixture"));
			projectContent.Classes.Add(testClass1);
			
			testClass2 = new MockClass(projectContent, "Project.MyTestFixture");
			testClass2.Attributes.Add(new MockAttribute("TestFixture"));
			projectContent.Classes.Add(testClass2);
						
			testFrameworks = new MockTestFrameworksWithNUnitFrameworkSupport();
			dummyTreeView = new DummyParserServiceTestTreeView(testFrameworks);
			dummyTreeView.ProjectContentForProject = projectContent;
			
			// Load the projects into the test tree view.
			treeView = dummyTreeView as TestTreeView;
			solution.Folders.Add(project);
			treeView.AddSolution(solution);
			nodes = treeView.Nodes;
			rootNode = (ExtTreeNode)treeView.Nodes[0];
			
			treeView.SelectedNode = rootNode;
			testProject = treeView.SelectedTestProject;
		}
		
		[TearDown]
		public void TearDown()
		{
			if (treeView != null) {
				treeView.Dispose();
			}
		}
		
		public void ExpandRootNode()
		{
			// Expand the root node so any child nodes are lazily created.
			rootNode.Expanding();
			projectNamespaceNode = (ExtTreeNode)rootNode.Nodes[0];
		}
		
		[Test]
		public void TestClass2PassedAfterTestClass1Failed()
		{
			ExpandRootNode();
			
			TestClass testClass1 = testProject.TestClasses["Project.Tests.MyTestFixture"];
			testClass1.Result = TestResultType.Failure;
			
			TestClass testClass2 = testProject.TestClasses["Project.MyTestFixture"];
			testClass2.Result = TestResultType.Success;

			Assert.AreEqual(TestTreeViewImageListIndex.TestFailed, (TestTreeViewImageListIndex)projectNamespaceNode.ImageIndex);
		}
		
		[Test]
		public void TestClass2PassedAfterTestClass1Ignored()
		{
			ExpandRootNode();
			
			TestClass testClass1 = testProject.TestClasses["Project.Tests.MyTestFixture"];
			testClass1.Result = TestResultType.Ignored;
			
			TestClass testClass2 = testProject.TestClasses["Project.MyTestFixture"];
			testClass2.Result = TestResultType.Success;

			Assert.AreEqual(TestTreeViewImageListIndex.TestIgnored, (TestTreeViewImageListIndex)projectNamespaceNode.ImageIndex);
		}
		
		[Test]
		public void ExpandProjectNodeAfterOnePassOneFail()
		{
			TestClass testClass1 = testProject.TestClasses["Project.Tests.MyTestFixture"];
			testClass1.Result = TestResultType.Failure;
			
			TestClass testClass2 = testProject.TestClasses["Project.MyTestFixture"];
			testClass2.Result = TestResultType.Success;
			
			ExpandRootNode();
			Assert.AreEqual(TestTreeViewImageListIndex.TestFailed, (TestTreeViewImageListIndex)projectNamespaceNode.ImageIndex);
		}
		
		[Test]
		public void AddNewClass()
		{
			ExtTreeNode projectNamespaceNode = ExpandProjectNamespaceNode();
			ExtTreeNode testsNamespaceNode = ExpandTestsNamespaceNode();
			
			MockClass mockClass = new MockClass(projectContent, "Project.Tests.MyNewTestFixture");
			mockClass.Attributes.Add(new MockAttribute("TestFixture"));
			TestClass newTestClass = new TestClass(mockClass, testFrameworks);
			testProject.TestClasses.Add(newTestClass);
			
			ExtTreeNode newTestClassNode = null;
			foreach (ExtTreeNode node in testsNamespaceNode.Nodes) {
				if (node.Text == "MyNewTestFixture") {
					newTestClassNode = node;
					break;
				}
			}
			newTestClass.Result = TestResultType.Failure;
			
			// New test class node should be added to the test namespace node.
			Assert.AreEqual(2, testsNamespaceNode.Nodes.Count);
			Assert.IsNotNull(newTestClassNode);
			
			// Make sure the namespace node image index is affected by the
			// new test class added.
			Assert.AreEqual(TestTreeViewImageListIndex.TestFailed, (TestTreeViewImageListIndex)testsNamespaceNode.SelectedImageIndex);
			
			// Project namespace node should have two child nodes, one for the
			// Tests namespace and one test class node.
			Assert.AreEqual(2, projectNamespaceNode.Nodes.Count);
		}
		
		/// <summary>
		/// Tests that the test class is removed from the tree.
		/// </summary>
		[Test]
		public void RemoveClass()
		{
			AddNewClass();
			
			ExtTreeNode projectNamespaceNode = ExpandProjectNamespaceNode();
			ExtTreeNode testsNamespaceNode = ExpandTestsNamespaceNode();
			
			// Reset the new TestClass result after it was modified
			// in the AddNewClass call.
			TestClass newTestClass = testProject.TestClasses["Project.Tests.MyNewTestFixture"];
			newTestClass.Result = TestResultType.None;
			
			// Locate the class we are going to remove.
			TestClass testClass = testProject.TestClasses["Project.Tests.MyTestFixture"];
			testProject.TestClasses.Remove(testClass);
			
			ExtTreeNode testClassNode = null;
			foreach (ExtTreeNode node in testsNamespaceNode.Nodes) {
				if (node.Text == "MyTestFixture") {
					testClassNode = node;
					break;
				}
			}
			testClass.Result = TestResultType.Failure;
			
			Assert.AreEqual(1, testsNamespaceNode.Nodes.Count);
			Assert.IsNull(testClassNode);
			
			// Make sure the namespace node image index is NOT affected by the
			// test class just removed.
			Assert.AreEqual(TestTreeViewImageListIndex.TestNotRun, (TestTreeViewImageListIndex)testsNamespaceNode.SelectedImageIndex);
			
			// Check that the test class does not affect the project namespace node
			// image index either.
			Assert.AreEqual(TestTreeViewImageListIndex.TestNotRun, (TestTreeViewImageListIndex)projectNamespaceNode.ImageIndex);
		}
		
		/// <summary>
		/// Tests that after all the child nodes from a namespace node are removed
		/// the namespace node removes itself.
		/// </summary>
		[Test]
		public void RemoveNamespaceNode()
		{
			ExtTreeNode projectNamespaceNode = ExpandProjectNamespaceNode();
			ExtTreeNode testsNamespaceNode = ExpandTestsNamespaceNode();
			
			// Locate the class we are going to remove.
			TestClass testClass = testProject.TestClasses["Project.Tests.MyTestFixture"];
			testProject.TestClasses.Remove(testClass);
			
			ExtTreeNode testClassNode = null;
			foreach (ExtTreeNode node in testsNamespaceNode.Nodes) {
				if (node.Text == "MyTestFixture") {
					testClassNode = node;
					break;
				}
			}
			Assert.IsNull(testClassNode);
			
			// Project namespace node should only have one child node.
			Assert.AreEqual(1, projectNamespaceNode.Nodes.Count);
			Assert.AreEqual("MyTestFixture", projectNamespaceNode.Nodes[0].Text);
		}
		
		[Test]
		public void RemoveClassFromNamespaceNodeWithNonClassNodeChildren()
		{
			ExtTreeNode projectNamespaceNode = ExpandProjectNamespaceNode();
			ExtTreeNode testsNamespaceNode = ExpandTestsNamespaceNode();
			
			// Add a dummy tree node to the tests namespace node to make 
			// sure the TestNamespaceTreeNode handles non-TestClassTreeNode
			// children when removing a class.
			testsNamespaceNode.Nodes.Insert(0, new ExtTreeNode());
			
			// Locate the class we are going to remove.
			TestClass testClass = testProject.TestClasses["Project.Tests.MyTestFixture"];
			testProject.TestClasses.Remove(testClass);
			
			ExtTreeNode testClassNode = null;
			foreach (ExtTreeNode node in testsNamespaceNode.Nodes) {
				if (node.Text == "MyTestFixture") {
					testClassNode = node;
					break;
				}
			}
			Assert.IsNull(testClassNode);
			Assert.AreEqual(1, testsNamespaceNode.Nodes.Count);
		}
		
		ExtTreeNode ExpandProjectNamespaceNode()
		{
			ExpandRootNode();
			foreach (ExtTreeNode node in rootNode.Nodes) {
				if (node.Text == "Project") {
					node.Expanding();
					return node;
				}
			}
			return null;
		}
		
		ExtTreeNode ExpandTestsNamespaceNode()
		{
			ExtTreeNode projectNamespaceNode = ExpandProjectNamespaceNode();
			foreach (ExtTreeNode childNode in projectNamespaceNode.Nodes) {
				if (childNode.Text == "Tests") {
					childNode.Expanding();
					return childNode;
				}
			}
			return null;
		}
	}
}
