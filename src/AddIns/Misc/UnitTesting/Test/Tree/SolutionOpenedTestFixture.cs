// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

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
	public class SolutionOpenedTestFixture : UnitTestsPad
	{
		Solution solution;
		MSBuildProject project;
		MockProjectContent projectContent = new MockProjectContent();
		
		[SetUp]
		public void Init()
		{
			solution = new Solution();
			project = new MSBuildProject();
			projectContent.Project = project;
			projectContent.Language = LanguageProperties.None;
			ReferenceProjectItem refProjectItem = new ReferenceProjectItem(project);
			refProjectItem.Include = "NUnit.Framework";
			project.Items.Add(refProjectItem);
			solution.Folders.Add(project);
			
			base.SolutionLoaded(solution);
		}
		
		[Test]
		public void TestTreeHasNodes()
		{
			Assert.AreEqual(1, base.TestTreeView.Nodes.Count);
		}
		
		[Test]
		public void CloseSolution()
		{
			base.SolutionClosed();
			
			Assert.AreEqual(0, base.TestTreeView.Nodes.Count);
		}
		
		[Test]
		public void SolutionFolderRemoved()
		{
			base.SolutionFolderRemoved(project);
			
			Assert.AreEqual(0, base.TestTreeView.Nodes.Count);
		}
		
		[Test]
		public void ProjectAdded()
		{
			MSBuildProject project = new MSBuildProject();
			project.Name = "NewProject";
			ReferenceProjectItem refProjectItem = new ReferenceProjectItem(project);
			refProjectItem.Include = "NUnit.Framework";
			project.Items.Add(refProjectItem);
			
			base.ProjectAdded(project);
			
			Assert.AreEqual(2, base.TestTreeView.Nodes.Count);
		}
		
		[Test]
		public void ReferenceProjectItemRemoved()
		{
			ProjectItem refProjectItem = project.Items[0];
			project.Items.Remove(refProjectItem);
			
			base.ProjectItemRemoved(refProjectItem);
			
			Assert.AreEqual(0, base.TestTreeView.Nodes.Count);
		}
		
		[Test]
		public void ReferenceProjectItemAdded()
		{
			MSBuildProject project = new MSBuildProject();
			project.Name = "NewProject";
			
			base.ProjectAdded(project);
			
			// Project should not be added at first.
			Assert.AreEqual(1, base.TestTreeView.Nodes.Count);
			
			ReferenceProjectItem refProjectItem = new ReferenceProjectItem(project);
			refProjectItem.Include = "NUnit.Framework";
			project.Items.Add(refProjectItem);
			
			base.ProjectItemAdded(refProjectItem);
			
			// Project should be added since it has a reference to
			// NUnit.
			Assert.AreEqual(2, base.TestTreeView.Nodes.Count);
		}
		
		[Test]
		public void ReferenceProjectItemAddedTwice()
		{
			MSBuildProject project = new MSBuildProject();
			project.Name = "NewProject";
			ReferenceProjectItem refProjectItem = new ReferenceProjectItem(project);
			refProjectItem.Include = "NUnit.Framework";
			project.Items.Add(refProjectItem);
			
			base.ProjectAdded(project);
			
			project.Items.Add(refProjectItem);
			
			base.ProjectItemAdded(refProjectItem);

			Assert.AreEqual(2, base.TestTreeView.Nodes.Count);
		}
		
		[Test]
		public void ParserInfoUpdated()
		{
			DefaultCompilationUnit newUnit = new DefaultCompilationUnit(projectContent);
			MockClass mockClass = new MockClass("MyTestFixture");
			mockClass.Attributes.Add(new MockAttribute("TestFixture"));
			mockClass.ProjectContent = projectContent;
			mockClass.SetCompoundClass(mockClass);
			newUnit.Classes.Add(mockClass);

			ExtTreeNode rootNode = (ExtTreeNode)base.TestTreeView.Nodes[0];
			rootNode.Expanding();

			base.UpdateParseInfo(null, newUnit);
			
			Assert.AreEqual(1, rootNode.Nodes.Count);
			Assert.AreEqual("MyTestFixture", rootNode.Nodes[0].Text);
		}
		
		[Test]
		public void GetTestProjectFromProject()
		{
			TestProjectTreeNode projectNode = (TestProjectTreeNode)base.TestTreeView.Nodes[0];
			TestProject expectedTestProject = projectNode.TestProject;
			
			Assert.AreSame(expectedTestProject, base.TestTreeView.GetTestProject(project));
		}
		
		[Test]
		public void GetTestProjectFromUnknownProject()
		{
			MSBuildProject project = new MSBuildProject();
			Assert.IsNull(base.TestTreeView.GetTestProject(project));
		}
		
		/// <summary>
		/// Tests that an empty project node after being expanded
		/// will update itself if a new class is added to the project.
		/// </summary>
		[Test]
		public void ClassNodeAddedAfterProjectNodeExpanded()
		{
			// Expand the project node.
			TestProjectTreeNode projectNode = (TestProjectTreeNode)base.TestTreeView.Nodes[0];
			projectNode.Expanding();
			
			// Add a new class to a non-empty namespace so it gets
			// added to a new namespace node.
			MockClass mockClass = new MockClass("RootNamespace.MyTestFixture");
			TestClass testClass = new TestClass(mockClass);
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
			TestProjectTreeNode projectNode = (TestProjectTreeNode)base.TestTreeView.Nodes[0];
			TestNamespaceTreeNode parentNamespaceNode = (TestNamespaceTreeNode)projectNode.Nodes[0];
			parentNamespaceNode.Expanding();
			
			// Add a new class to a namespace so it gets
			// added to a new namespace node.
			MockClass mockClass = new MockClass("RootNamespace.Tests.MyTestFixture");
			TestClass testClass = new TestClass(mockClass);
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
			TestProjectTreeNode projectNode = (TestProjectTreeNode)base.TestTreeView.Nodes[0];
			projectNode.Expanding();
			
			// Add a new class to a non-empty namespace so it gets
			// added to a new namespace node.
			MockClass mockClass = new MockClass("RootNamespace.Tests.MyTestFixture");
			TestClass testClass = new TestClass(mockClass);
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
			TestProjectTreeNode projectNode = (TestProjectTreeNode)base.TestTreeView.Nodes[0];
			projectNode.Expanding();
			
			// Add a new class to a non-empty namespace so it gets
			// added to a new namespace node.
			MockClass mockClass = new MockClass("RootNamespace.Tests.MyTestFixture");
			TestClass testClass = new TestClass(mockClass);
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
		
		/// <summary>
		/// Returns a dummy toolstrip so the UnitTestsPad can be
		/// tested. If the default method is called the AddInTree
		/// is referenced which is not available during testing.
		/// </summary>
		protected override ToolStrip CreateToolStrip(string name)
		{
			return new ToolStrip();
		}
		
		/// <summary>
		/// Returns a dummy ContextMenuStrip so the UnitTestsPad can be
		/// tested. If the default method is called the AddInTree
		/// is referenced which is not available during testing.
		/// </summary>
		protected override ContextMenuStrip CreateContextMenu(string name)
		{
			return new ContextMenuStrip();
		}
		
		/// <summary>
		/// Returns a dummy tree view where we can mock the
		/// IProjectContent that will be used by the TestTreeView.
		/// </summary>
		protected override TestTreeView CreateTestTreeView()
		{
			DummyParserServiceTestTreeView treeView = new DummyParserServiceTestTreeView();
			treeView.AddProjectContentForProject(projectContent);
			return treeView;
		}
	}
}
