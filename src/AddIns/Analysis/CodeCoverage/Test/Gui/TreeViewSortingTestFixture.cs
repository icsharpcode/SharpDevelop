// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Windows.Forms;
using ICSharpCode.CodeCoverage;
using ICSharpCode.SharpDevelop.Gui;
using NUnit.Framework;

namespace ICSharpCode.CodeCoverage.Tests.Gui
{
	/// <summary>
	/// Tests that the code coverage tree view is sorted correctly for
	/// all node types.
	/// </summary>
	[TestFixture]
	public class TreeViewSortingTestFixture
	{
		IComparer<TreeNode> nodeSorter;
		CodeCoverageTreeView treeView;
		CodeCoverageModuleTreeNode codeCoverageModuleTreeNode;
		CodeCoverageModuleTreeNode zModuleTreeNode;
		CodeCoverageTreeNode betaNamespaceTreeNode;
		CodeCoverageTreeNode testFixtureClassTreeNode;
		CodeCoverageTreeNode aardvarkNamespaceTreeNode;
		CodeCoverageTreeNode anotherTestFixtureTreeNode;
		CodeCoverageTreeNode testFixtureTreeNode;
		CodeCoverageTreeNode addNodeTestTreeNode;
		CodeCoverageTreeNode removeMarkersTestTreeNode;
		
		[TestFixtureSetUp]
		public void SetUpFixture()
		{
			treeView = new CodeCoverageTreeView();
			nodeSorter = treeView.NodeSorter;
			
			List<CodeCoverageModule> modules = new List<CodeCoverageModule>();
			
			// Create a module called Z.
			CodeCoverageModule zModule = new CodeCoverageModule("Z");
			modules.Add(zModule);
		
			// Create a module called CodeCoverage.
			CodeCoverageModule codeCoverageModule = new CodeCoverageModule("CodeCoverage");
			modules.Add(codeCoverageModule);
			
			// Add a method that lives in a class without any namespace.
			CodeCoverageMethod testMethod = new CodeCoverageMethod("Test", "TestFixture");
			codeCoverageModule.Methods.Add(testMethod);
			
			// Add a method which produces a namespace that alphabetically 
			// occurs after the class already added.
			CodeCoverageMethod removeCodeMarkersMethod = new CodeCoverageMethod("RemoveCodeMarkersMethod", "Beta.TestFixture");
			codeCoverageModule.Methods.Add(removeCodeMarkersMethod);
			
			// Add a method that lives in a namespace that 
			// occurs before the removeCodeMarkersMethod. We want to 
			// make sure that this namespace node gets added before the Beta one.
			CodeCoverageMethod zebraMethod = new CodeCoverageMethod("Zebra", "Aardvark.TestFixture");
			codeCoverageModule.Methods.Add(zebraMethod);
			
			// Add a second class in the beta namespace so we check the 
			// sorting of classes.
			CodeCoverageMethod addCodeMarkersMethod = new CodeCoverageMethod("AddCodeMarkersMethod", "Beta.AnotherTestFixture");
			codeCoverageModule.Methods.Add(addCodeMarkersMethod);
			
			// Add a method which produces occurs before the remove code markers method.
			CodeCoverageMethod addNodeMethod = new CodeCoverageMethod("AddNode", "Beta.TestFixture");
			codeCoverageModule.Methods.Add(addNodeMethod);
			
			// Add two get and set properties.
			CodeCoverageMethod method = new CodeCoverageMethod("get_Zebra", "Beta.AnotherTestFixture");
			codeCoverageModule.Methods.Add(method);
			method = new CodeCoverageMethod("set_Zebra", "Beta.AnotherTestFixture");
			codeCoverageModule.Methods.Add(method);
			
			method = new CodeCoverageMethod("set_Aardvark", "Beta.AnotherTestFixture");
			codeCoverageModule.Methods.Add(method);
			method = new CodeCoverageMethod("get_Aardvark", "Beta.AnotherTestFixture");
			codeCoverageModule.Methods.Add(method);
			
			// Add a method which should appear between the two properties.
			method = new CodeCoverageMethod("Chimp", "Beta.AnotherTestFixture");
			codeCoverageModule.Methods.Add(method);

			
			// Add the modules to the tree.
			treeView.AddModules(modules);
			
			codeCoverageModuleTreeNode = (CodeCoverageModuleTreeNode)treeView.Nodes[0];
			zModuleTreeNode = (CodeCoverageModuleTreeNode)treeView.Nodes[1];
			
			// Initialise the code coverage module tree node.
			codeCoverageModuleTreeNode.PerformInitialization();
			aardvarkNamespaceTreeNode = (CodeCoverageTreeNode)codeCoverageModuleTreeNode.Nodes[0];
			betaNamespaceTreeNode = (CodeCoverageTreeNode)codeCoverageModuleTreeNode.Nodes[1];
			testFixtureClassTreeNode = (CodeCoverageTreeNode)codeCoverageModuleTreeNode.Nodes[2];
		
			// Initialise the beta namespace tree node.
			betaNamespaceTreeNode.PerformInitialization();
			anotherTestFixtureTreeNode = (CodeCoverageTreeNode)betaNamespaceTreeNode.Nodes[0];
			testFixtureTreeNode = (CodeCoverageTreeNode)betaNamespaceTreeNode.Nodes[1];
		
			// Initialise the test fixture class tree node
			testFixtureTreeNode.PerformInitialization();
			addNodeTestTreeNode = (CodeCoverageTreeNode)testFixtureTreeNode.Nodes[0];
			removeMarkersTestTreeNode = (CodeCoverageTreeNode)testFixtureTreeNode.Nodes[1];
		
			// Initialise the anotherTestFixtureTreeNode
			anotherTestFixtureTreeNode.PerformInitialization();
		}
		
		[TestFixtureTearDown]
		public void TearDownFixture()
		{
			treeView.Dispose();
		}
		
		[Test]
		public void FirstModuleTreeNode()
		{
			Assert.AreEqual("CodeCoverage", codeCoverageModuleTreeNode.Name);
		}
		
		[Test]
		public void SecondModuleTreeNode()
		{
			Assert.AreEqual("Z", zModuleTreeNode.Name);
		}
		
		/// <summary>
		/// Makes sure this is the second child node of the code coverage module
		/// parent tree node.
		/// </summary>
		[Test]
		public void BetaNamespaceTreeNodeName()
		{
			Assert.AreEqual("Beta", betaNamespaceTreeNode.Name);
		}
		
		[Test]
		public void AardvarkNamespaceTreeNodeName()
		{
			Assert.AreEqual("Aardvark", aardvarkNamespaceTreeNode.Name);
		}
		
		/// <summary>
		/// Test the code coverage tree view's node sorted correctly 
		/// orders the namespace tree nodes.
		/// </summary>
		[Test]
		public void SameNamespaceTreeNodeCompared()
		{
			Assert.AreEqual(0, nodeSorter.Compare(aardvarkNamespaceTreeNode, aardvarkNamespaceTreeNode));
		}
		
		[Test]
		public void FirstNamespaceTreeNodeLessThan()
		{
			int expectedResult = aardvarkNamespaceTreeNode.CompareString.CompareTo(betaNamespaceTreeNode.CompareString);
			Assert.AreEqual(aardvarkNamespaceTreeNode.Name, aardvarkNamespaceTreeNode.CompareString);
			Assert.AreEqual(betaNamespaceTreeNode.Name, betaNamespaceTreeNode.CompareString);
			Assert.AreEqual(expectedResult, nodeSorter.Compare(aardvarkNamespaceTreeNode, betaNamespaceTreeNode));
		}
		
		[Test]
		public void FirstNamespaceTreeNodeMoreThan()
		{		
			int expectedResult = betaNamespaceTreeNode.CompareString.CompareTo(aardvarkNamespaceTreeNode.CompareString);
			Assert.AreEqual(expectedResult, nodeSorter.Compare(betaNamespaceTreeNode, aardvarkNamespaceTreeNode));
		}
		
		/// <summary>
		/// The namespace node should always be ordered before any class node.
		/// </summary>
		[Test]
		public void ClassAndNamespaceTreeNodeCompared()
		{
			List<CodeCoverageMethod> methods = new List<CodeCoverageMethod>();
			CodeCoverageNamespaceTreeNode namespaceTreeNode = new CodeCoverageNamespaceTreeNode("Z", methods);
			CodeCoverageClassTreeNode classTreeNode = new CodeCoverageClassTreeNode("A", methods);
			Assert.AreEqual(-1, nodeSorter.Compare(namespaceTreeNode, classTreeNode));
			Assert.Greater(classTreeNode.SortOrder, namespaceTreeNode.SortOrder);
		}
		
		/// <summary>
		/// The compare string should be the name of the node not including
		/// any percentages.
		/// </summary>
		[Test]
		public void CodeCoverageTreeNodeCompareString()
		{
			CodeCoverageTreeNode node = new CodeCoverageTreeNode("A", CodeCoverageImageListIndex.Class, 1, 2);
			Assert.AreEqual("A", node.CompareString);
		}
		
		[Test]
		public void TestFixtureTreeNodeName()
		{
			Assert.AreEqual("TestFixture", testFixtureTreeNode.Name);
		}
		
		[Test]
		public void AnotherTestFixtureTreeNodeName()
		{
			Assert.AreEqual("AnotherTestFixture", anotherTestFixtureTreeNode.Name);
		}
		
		[Test]
		public void RemoveMarkersTestTreeNodeName()
		{
			Assert.AreEqual("RemoveCodeMarkersMethod", removeMarkersTestTreeNode.Name);
		}
		
		[Test]
		public void AddNodeTestTreeNodeName()
		{
			Assert.AreEqual("AddNode", addNodeTestTreeNode.Name);
		}
		
		[Test]
		public void PropertyMethodsSorted()
		{
			Assert.AreEqual("get_Aardvark", anotherTestFixtureTreeNode.Nodes[0].Name);
			Assert.AreEqual("set_Aardvark", anotherTestFixtureTreeNode.Nodes[1].Name);
			Assert.AreEqual("AddCodeMarkersMethod", anotherTestFixtureTreeNode.Nodes[2].Name);
			Assert.AreEqual("Chimp", anotherTestFixtureTreeNode.Nodes[3].Name);
			Assert.AreEqual("get_Zebra", anotherTestFixtureTreeNode.Nodes[4].Name);
			Assert.AreEqual("set_Zebra", anotherTestFixtureTreeNode.Nodes[5].Name);
		}
		
		[Test]
		public void GetterPropertyCompareString()
		{
			CodeCoverageMethod getterMethod = new CodeCoverageMethod("get_Aardvark", "Animal");
			CodeCoverageMethodTreeNode getterNode = new CodeCoverageMethodTreeNode(getterMethod);
			Assert.AreEqual("Aardvark get", getterNode.CompareString);
		}
		
		[Test]
		public void SetterPropertyCompareString()
		{
			CodeCoverageMethod setterMethod = new CodeCoverageMethod("set_Aardvark", "Animal");
			CodeCoverageMethodTreeNode setterNode = new CodeCoverageMethodTreeNode(setterMethod);
			Assert.AreEqual("Aardvark set", setterNode.CompareString);
		}
	}
}
