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
	/// Test that when a new class is added to the set of test classes
	/// currently being displayed in the tree the nodes are sorted
	/// correctly afterwards. We also test that the order of the
	/// tree nodes is correct after adding new methods.
	/// </summary>
	[TestFixture]
	public class TreeNodeSortingTestFixture
	{
		DummyParserServiceTestTreeView treeView;
		TestProjectTreeNode projectNode;
		TestNamespaceTreeNode myTestsNamespaceNode;
		TestClassTreeNode testFixtureNode;
		MockProjectContent projectContent;
		TestProject testProject;
		MockTestFrameworksWithNUnitFrameworkSupport testFrameworks;
		
		[SetUp]
		public void Init()
		{
			// Create a project to display in the test tree view.
			MockCSharpProject project = new MockCSharpProject();
			project.Name = "TestProject";
			ReferenceProjectItem nunitFrameworkReferenceItem = new ReferenceProjectItem(project);
			nunitFrameworkReferenceItem.Include = "NUnit.Framework";
			ProjectService.AddProjectItem(project, nunitFrameworkReferenceItem);
			List<IProject> projects = new List<IProject>();
			projects.Add(project);
					
			// Add a test class with a TestFixture attributes.
			projectContent = new MockProjectContent();
			projectContent.Language = LanguageProperties.None;
			TestClass testClass = CreateTestClass("MyTests.MyTestFixture");
			projectContent.Classes.Add(testClass.Class);
			
			// Add two methods to the test class only
			// one of which has test attributes.
			MockMethod testMethod = new MockMethod(testClass.Class, "NameExists");
			testMethod.Attributes.Add(new MockAttribute("Test"));
			testClass.Class.Methods.Add(testMethod);
			
			testFrameworks = new MockTestFrameworksWithNUnitFrameworkSupport();
			treeView = new DummyParserServiceTestTreeView(testFrameworks);
			treeView.ProjectContentForProject = projectContent;
			
			// Load the projects into the test tree view.
			treeView.AddProjects(projects);
			projectNode = (TestProjectTreeNode)treeView.Nodes[0];
			testProject = projectNode.TestProject;
			
			// Initialise the root node so the child nodes are created.
			projectNode.PerformInitialization();
			myTestsNamespaceNode = (TestNamespaceTreeNode)projectNode.FirstNode;
			
			// Initialise the first namespace node.
			myTestsNamespaceNode.PerformInitialization();
			testFixtureNode = (TestClassTreeNode)myTestsNamespaceNode.FirstNode;
		
			// Initialise the test method tree nodes.
			testFixtureNode.PerformInitialization();
		}
		
		[TearDown]
		public void TearDown()
		{
			if (treeView != null) {
				treeView.Dispose();
			}
		}
		
		/// <summary>
		/// Add a class that exists in a namespace that 
		/// should be inserted into the tree before the current 
		/// namespace node.
		/// </summary>
		[Test]
		public void AddClassInNewNamespace()
		{
			// Create the new test class.
			TestClass testClass = CreateTestClass("Apple.MyTestFixture");

			// Add the class to the existing classes.
			testProject.TestClasses.Add(testClass);
			
			// Check the namespace node is inserted before the
			// existing namespace node.
			TestNamespaceTreeNode namespaceTreeNode = (TestNamespaceTreeNode)projectNode.FirstNode;
			
			Assert.AreEqual("Apple", namespaceTreeNode.Text);
		}
		
		/// <summary>
		/// Here we add a class with no root namespace but with a name
		/// that alphabetically would occur before the existing 
		/// namespace tree node. NUnit does not treat namespace
		/// nodes any different to test class nodes so have the
		/// same behaviour. This means a class node could appear
		/// before the namespace node in the tree.
		/// </summary>
		[Test]
		public void AddClassWithNoRootNamespace()
		{
			TestClass testClass = CreateTestClass("AppleTest");
			testProject.TestClasses.Add(testClass);

			TestClassTreeNode classNode = projectNode.FirstNode as TestClassTreeNode;
			
			Assert.AreEqual(classNode.Text, "AppleTest");
		}
		
		/// <summary>
		/// Make sure that if we have a class with no root namespace
		/// and it is alphabetically before an existing namespace
		/// then it is added in the tree before that namespace. 
		/// We also test that the same thing occurs with a namespace
		/// tree node.
		/// </summary>
		[Test]
		public void ClassOccursBeforeNamespaceOnInitialLoad()
		{
			MockCSharpProject project = new MockCSharpProject();
			project.Name = "TestProject";
			ReferenceProjectItem nunitFrameworkReferenceItem = new ReferenceProjectItem(project);
			nunitFrameworkReferenceItem.Include = "NUnit.Framework";
			ProjectService.AddProjectItem(project, nunitFrameworkReferenceItem);
			List<IProject> projects = new List<IProject>();
			projects.Add(project);
					
			// Add a test class with a TestFixture attributes.
			TestClass testClass = CreateTestClass("MyTests.MyTestFixture");
			projectContent.Classes.Add(testClass.Class);
			
			// Add a second class with no namespace.
			testClass = CreateTestClass("AppleTestFixture");
			projectContent.Classes.Add(testClass.Class);
			
			// Add another class that exists in a namespace inside
			// MyTests.
			testClass = CreateTestClass("MyTests.ZebraTests.AddZebra");
			projectContent.Classes.Add(testClass.Class);
					
			// Load the project into the tree.
			treeView.Clear();
			treeView.AddProject(project);
			projectNode = (TestProjectTreeNode)treeView.Nodes[0];
			projectNode.PerformInitialization();
			
			ExtTreeNode treeNode = (ExtTreeNode)projectNode.LastNode;
			treeNode.PerformInitialization();
		
			// Get the class node without a root namespace and
			// the my tests namespace node.
			TestClassTreeNode appleTestFixtureNode = projectNode.FirstNode as TestClassTreeNode;
			TestNamespaceTreeNode myTestsNamespaceNode = projectNode.LastNode as TestNamespaceTreeNode;

			// Get the zebra namespace tree node. 
			TestNamespaceTreeNode zebraTestsNamespaceNode = treeNode.LastNode as TestNamespaceTreeNode;

			Assert.IsNotNull(appleTestFixtureNode);
			Assert.AreEqual(appleTestFixtureNode.Text, "AppleTestFixture");
			Assert.IsNotNull(myTestsNamespaceNode);
			Assert.AreEqual(myTestsNamespaceNode.Text, "MyTests");
			Assert.IsNotNull(zebraTestsNamespaceNode);
			Assert.AreEqual(zebraTestsNamespaceNode.Text, "ZebraTests");
		}
		
		/// <summary>
		/// Here we add a new class that exists in a namespace that
		/// did not previously exist and we make sure the tree node is
		/// inserted before any other tree node.
		/// </summary>
		[Test]
		public void NewNamespaceNode()
		{
			// Add a class in a new namespace.
			TestClass testClass = CreateTestClass("MyTests.AppleTests.IsEqual");
			testProject.TestClasses.Add(testClass);
			
			TestTreeNode insertedNode = myTestsNamespaceNode.FirstNode as TestTreeNode;
			Assert.AreEqual("AppleTests", insertedNode.Text);
		}
		
		/// <summary>
		/// Here we add a new class that exists in a namespace that
		/// already exists and we make sure the new tree node is
		/// inserted before any other tree node.
		/// </summary>
		[Test]
		public void AddNewClassToExistingNamespace()
		{
			// Add a class in the tests namespace.
			TestClass testClass = CreateTestClass("MyTests.AppleTestFixture");
			testProject.TestClasses.Add(testClass);
			
			TestTreeNode insertedNode = myTestsNamespaceNode.FirstNode as TestTreeNode;
			Assert.AreEqual("AppleTestFixture", insertedNode.Text);
		}
		
		/// <summary>
		/// Tests that new methods are inserted into the tree in the
		/// correct alphabetical order.
		/// </summary>
		[Test]
		public void NewClassMethodAdded()
		{
			MockClass c = (MockClass)testFixtureNode.Class;
			MockMethod method = new MockMethod(c, "Apple");
			method.Attributes.Add(new MockAttribute("Test"));
			c.SetCompoundClass(c);
			c.Methods.Add(method);
			
			// Add the test method to the class.
			DefaultCompilationUnit newUnit = new DefaultCompilationUnit(projectContent);
			newUnit.Classes.Add(c);
			testProject.UpdateParseInfo(null, newUnit);
			
			// Check that the new tree node is inserted before the
			// existing method node.
			TestTreeNode insertedNode = testFixtureNode.FirstNode as TestTreeNode;
			
			Assert.AreEqual("Apple", insertedNode.Text);
		}
		
		/// <summary>
		/// Creates a new class with the fully qualified name.
		/// </summary>
		TestClass CreateTestClass(string name)
		{
			MockClass c = new MockClass(projectContent, name);
			c.Attributes.Add(new MockAttribute("TestFixture"));
			return new TestClass(c, testFrameworks);
		}
	}
}
