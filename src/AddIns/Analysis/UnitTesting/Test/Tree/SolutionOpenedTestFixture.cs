// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
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
	/// Tests that the UnitTestPad loads the test tree view when
	/// a new solution is opened.
	/// </summary>
	[TestFixture]
	public class SolutionOpenedTestFixture
	{
		DerivedUnitTestsPad pad;
		Solution solution;
		MSBuildBasedProject project;
		MockProjectContent projectContent;
		MockTestFrameworksWithNUnitFrameworkSupport testFrameworks;
		
		[TestFixtureSetUp]
		public void SetUpFixture()
		{
			pad = new DerivedUnitTestsPad();
		}
		
		[SetUp]
		public void Init()
		{
			testFrameworks = new MockTestFrameworksWithNUnitFrameworkSupport();
			projectContent = new MockProjectContent();
			pad.ProjectContent = projectContent;
			solution = new Solution(new MockProjectChangeWatcher());
			project = new MockCSharpProject();
			projectContent.Project = project;
			projectContent.Language = LanguageProperties.None;
			ReferenceProjectItem refProjectItem = new ReferenceProjectItem(project);
			refProjectItem.Include = "NUnit.Framework";
			ProjectService.AddProjectItem(project, refProjectItem);
			solution.Folders.Add(project);
			
			pad.CallSolutionLoaded(solution);
		}
		
		[TestFixtureTearDown]
		public void TearDownFixture()
		{
			if (pad != null) {
				pad.Dispose();
			}
		}
		
		[Test]
		public void TestTreeHasNodes()
		{
			Assert.AreEqual(1, pad.TestTreeView.Nodes.Count);
		}
		
		[Test]
		public void CloseSolution()
		{
			pad.CallSolutionClosed();
			
			Assert.AreEqual(0, pad.TestTreeView.Nodes.Count);
		}
		
		[Test]
		public void SolutionFolderRemoved()
		{
			pad.CallSolutionFolderRemoved(project);
			
			Assert.AreEqual(0, pad.TestTreeView.Nodes.Count);
		}
		
		/// <summary>
		/// Add a new project and check that we also add an All Tests
		/// root node and a new project node.
		/// </summary>
		[Test]
		public void ProjectAdded()
		{
			IProject project = new MockCSharpProject();
			project.Name = "NewProject";
			ReferenceProjectItem refProjectItem = new ReferenceProjectItem(project);
			refProjectItem.Include = "NUnit.Framework";
			ProjectService.AddProjectItem(project, refProjectItem);
			
			pad.CallProjectAdded(project);
			
			Assert.AreEqual(2, pad.TestTreeView.GetProjects().Length);
			Assert.AreEqual(1, pad.TestTreeView.Nodes.Count);
			Assert.IsInstanceOf(typeof(AllTestsTreeNode), pad.TestTreeView.Nodes[0]);
			Assert.AreEqual(2, pad.TestTreeView.Nodes[0].Nodes.Count);
		}
		
		[Test]
		public void ReferenceProjectItemRemoved()
		{
			ProjectItem refProjectItem = project.Items[0];
			ProjectService.RemoveProjectItem(project, refProjectItem);
			
			pad.CallProjectItemRemoved(refProjectItem);
			
			Assert.AreEqual(0, pad.TestTreeView.Nodes.Count);
		}
		
		[Test]
		public void ReferenceProjectItemAdded()
		{
			IProject project = new MockCSharpProject();
			project.Name = "NewProject";
			
			pad.CallProjectAdded(project);
			
			// Project should not be added at first.
			Assert.AreEqual(1, pad.TestTreeView.Nodes.Count);
			
			ReferenceProjectItem refProjectItem = new ReferenceProjectItem(project);
			refProjectItem.Include = "NUnit.Framework";
			ProjectService.AddProjectItem(project, refProjectItem);
			
			pad.CallProjectItemAdded(refProjectItem);
			
			// Project should be added since it has a reference to
			// NUnit.
			Assert.AreEqual(2, pad.TestTreeView.GetProjects().Length);
			Assert.AreEqual(1, pad.TestTreeView.Nodes.Count);
			Assert.IsInstanceOf(typeof(AllTestsTreeNode), pad.TestTreeView.Nodes[0]);
			Assert.AreEqual(2, pad.TestTreeView.Nodes[0].Nodes.Count);
		}
		
		[Test]
		public void ReferenceProjectItemAddedTwice()
		{
			IProject project = new MockCSharpProject();
			project.Name = "NewProject";
			ReferenceProjectItem refProjectItem = new ReferenceProjectItem(project);
			refProjectItem.Include = "NUnit.Framework";
			ProjectService.AddProjectItem(project, refProjectItem);
			
			pad.CallProjectAdded(project);
			
			// Add a second NUnit.Framework reference.
			refProjectItem = new ReferenceProjectItem(project);
			refProjectItem.Include = "NUnit.Framework";
			ProjectService.AddProjectItem(project, refProjectItem);
			
			pad.CallProjectItemAdded(refProjectItem);

			Assert.AreEqual(2, pad.TestTreeView.GetProjects().Length);
			Assert.AreEqual(1, pad.TestTreeView.Nodes.Count);
			Assert.IsInstanceOf(typeof(AllTestsTreeNode), pad.TestTreeView.Nodes[0]);
			Assert.AreEqual(2, pad.TestTreeView.Nodes[0].Nodes.Count);
		}
		
		[Test]
		public void ParserInfoUpdated()
		{
			DefaultCompilationUnit newUnit = new DefaultCompilationUnit(pad.ProjectContent);
			MockClass mockClass = new MockClass(pad.ProjectContent, "MyTestFixture");
			mockClass.Attributes.Add(new MockAttribute("TestFixture"));
			newUnit.Classes.Add(mockClass);

			ExtTreeNode rootNode = (ExtTreeNode)pad.TestTreeView.Nodes[0];
			rootNode.Expanding();

			pad.CallUpdateParseInfo(null, newUnit);
			
			Assert.AreEqual(1, rootNode.Nodes.Count);
			Assert.AreEqual("MyTestFixture", rootNode.Nodes[0].Text);
		}
		
		[Test]
		public void GetTestProjectFromProject()
		{
			TestProjectTreeNode projectNode = (TestProjectTreeNode)pad.TestTreeView.Nodes[0];
			TestProject expectedTestProject = projectNode.TestProject;
			
			Assert.AreSame(expectedTestProject, pad.TestTreeView.GetTestProject(project));
		}
		
		[Test]
		public void GetTestProjectFromUnknownProject()
		{
			IProject project = new MockCSharpProject();
			Assert.IsNull(pad.TestTreeView.GetTestProject(project));
		}
		
		/// <summary>
		/// Tests that an empty project node after being expanded
		/// will update itself if a new class is added to the project.
		/// </summary>
		[Test]
		public void ClassNodeAddedAfterProjectNodeExpanded()
		{
			// Expand the project node.
			TestProjectTreeNode projectNode = (TestProjectTreeNode)pad.TestTreeView.Nodes[0];
			projectNode.Expanding();
			
			// Add a new class to a non-empty namespace so it gets
			// added to a new namespace node.
			MockClass mockClass = new MockClass("RootNamespace.MyTestFixture");
			TestClass testClass = new TestClass(mockClass, testFrameworks);
			projectNode.TestProject.TestClasses.Add(testClass);
			
			Assert.AreEqual(1, projectNode.Nodes.Count,
			                "Project node should have one child node.");
		}
		
		/// <summary>
		/// Tests the following:
		/// 
		/// A parent namespace node has been expanded and initially
		/// there are no namespace nodes below it, just one test
		/// class. Then a new test class is added which should
		/// cause a new namespace node to be added to the existing
		/// node.
		/// </summary>
		[Test]
		public void ClassNodeAddedAfterNamespaceNodeExpanded()
		{
			ClassNodeAddedAfterProjectNodeExpanded();
			
			// Expand the namespace node.
			TestProjectTreeNode projectNode = (TestProjectTreeNode)pad.TestTreeView.Nodes[0];
			TestNamespaceTreeNode parentNamespaceNode = (TestNamespaceTreeNode)projectNode.Nodes[0];
			parentNamespaceNode.Expanding();
			
			// Add a new class to a namespace so it gets
			// added to a new namespace node.
			MockClass mockClass = new MockClass("RootNamespace.Tests.MyTestFixture");
			TestClass testClass = new TestClass(mockClass, testFrameworks);
			projectNode.TestProject.TestClasses.Add(testClass);
			
			// Get the newly added namespace node.
			TestNamespaceTreeNode namespaceNode = null;
			foreach (ExtTreeNode node in parentNamespaceNode.Nodes) {
				namespaceNode = node as TestNamespaceTreeNode;
				if (namespaceNode != null) {
					break;
				}
			}
			
			Assert.AreEqual(2, parentNamespaceNode.Nodes.Count,
			                "Namespace node should have two child nodes.");
			Assert.IsNotNull(namespaceNode, "Namespace node has not been added");
			Assert.AreEqual("Tests", namespaceNode.Text);
		}
		
		/// <summary>
		/// Adds a new test class in the non-existant namespace
		/// RootNamepace.Tests, expands both these namespace tree nodes
		/// and then removes the test class from the TestProject.TestClasses
		/// collection. The test then checks that the two namespace
		/// nodes are removed from the tree.
		/// </summary>
		[Test]
		public void EmptyNamespaceNodesRemoved()
		{
			// Expand the project node.
			TestProjectTreeNode projectNode = (TestProjectTreeNode)pad.TestTreeView.Nodes[0];
			projectNode.Expanding();
			
			// Add a new class to a non-empty namespace so it gets
			// added to a new namespace node.
			MockClass mockClass = new MockClass("RootNamespace.Tests.MyTestFixture");
			TestClass testClass = new TestClass(mockClass, testFrameworks);
			projectNode.TestProject.TestClasses.Add(testClass);
			
			// Expand RootNamespace tree node.
			TestNamespaceTreeNode rootNamespaceNode = (TestNamespaceTreeNode)projectNode.Nodes[0];
			rootNamespaceNode.Expanding();
			
			// Expand the Tests namespace tree node.
			TestNamespaceTreeNode testsNamespaceNode = (TestNamespaceTreeNode)rootNamespaceNode.Nodes[0];
			testsNamespaceNode.Expanding();
			
			// Get the test class node.
			TestClassTreeNode classNode = (TestClassTreeNode)testsNamespaceNode.Nodes[0];
			
			// Remove the test class from the test project.
			projectNode.TestProject.TestClasses.Remove(testClass);
			
			Assert.AreEqual(0, projectNode.Nodes.Count,
			                "Namespace nodes should have been removed from project node.");
			
			// Make sure the two namespace nodes are properly disposed.
			Assert.IsTrue(testsNamespaceNode.IsDisposed);
			Assert.IsTrue(rootNamespaceNode.IsDisposed);
			
			// Make sure the test class node has been disposed.
			Assert.IsTrue(classNode.IsDisposed);
			
			// Make sure the namespace node Dispose method removes
			// the TestProject.TestClasses.TestClassAdded event handler.
			Assert.AreEqual(0, testsNamespaceNode.Nodes.Count);
			projectNode.TestProject.TestClasses.Add(testClass);
			Assert.AreEqual(0, testsNamespaceNode.Nodes.Count);
		}
		
		/// <summary>
		/// SD2-1203. The namespace tree nodes were not removing
		/// themselves when they were empty if they were not expanded
		/// first. This test makes sure this problem  is fixed.
		/// </summary>
		[Test]
		public void EmptyNamespaceNodesRemovedWhenChildNamespaceNodeNotExpanded()
		{
			// Expand the project node.
			TestProjectTreeNode projectNode = (TestProjectTreeNode)pad.TestTreeView.Nodes[0];
			projectNode.Expanding();
			
			// Add a new class to a non-empty namespace so it gets
			// added to a new namespace node.
			MockClass mockClass = new MockClass("RootNamespace.Tests.MyTestFixture");
			TestClass testClass = new TestClass(mockClass, testFrameworks);
			projectNode.TestProject.TestClasses.Add(testClass);
			
			// Get the root namespace node.
			TestNamespaceTreeNode rootNamespaceNode = (TestNamespaceTreeNode)projectNode.Nodes[0];
			
			// Check that the rootNamespaceNode does not consider itself
			// empty.
			Assert.IsFalse(rootNamespaceNode.IsEmpty);
			
			// Expand RootNamespace tree node.
			rootNamespaceNode.Expanding();
			
			// Remove the test class from the test project.
			projectNode.TestProject.TestClasses.Remove(testClass);
			
			Assert.AreEqual(0, projectNode.Nodes.Count,
			                "Namespace nodes should have been removed from project node.");
		}
		
		[Test]
		public void IsParserLoadingSolutionCalled()
		{
			Assert.IsTrue(pad.IsParserLoadingSolutionCalled);
		}
		
		[Test]
		public void GetOpenSolutionCalled()
		{
			Assert.IsTrue(pad.GetOpenSolutionCalled);
		}
		
		/// <summary>
		/// Tests that a null solution clears the test tree.
		/// </summary>
		[Test]
		public void NullSolution()
		{
			pad.CallSolutionLoaded(null);
			Assert.AreEqual(0, pad.TestTreeView.Nodes.Count);
		}
		
		/// <summary>
		/// If the user opens another solution before the parser thread
		/// has finished we can sometimes get into a state where the 
		/// first solution is being loaded into the test tree view but
		/// its project content has been removed since the parser is working
		/// on the next solution that was opened. In this case the test
		/// tree view should ignore any project's that have null project
		/// contents.
		/// </summary>
		[Test]
		public void NullProjectContent()
		{
			DummyParserServiceTestTreeView tree = (DummyParserServiceTestTreeView)pad.TestTreeView;
			tree.ProjectContentForProject = null;
			
			pad.CallSolutionLoaded(solution);
		}
	}
}
