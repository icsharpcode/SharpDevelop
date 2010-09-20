// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.CodeCoverage;
using NUnit.Framework;

namespace ICSharpCode.CodeCoverage.Tests.Gui
{
	/// <summary>
	/// Checks that the tree nodes have the correct text, images and 
	/// text colours as we expand the tree node by node.  The tree populates
	/// itself lazily as it is expanded.
	/// </summary>
	[TestFixture]
	public class CodeCoverageTreeViewTestFixture
	{
		TreeNodeCollection nodes;
		CodeCoverageModuleTreeNode fooModuleNode;
		CodeCoverageModuleTreeNode barModuleNode;
		CodeCoverageClassTreeNode fooTestFixtureTreeNode;
		CodeCoverageMethodTreeNode fooTestMethod1TreeNode;
		CodeCoverageMethodTreeNode fooTestMethod2TreeNode;
		CodeCoverageNamespaceTreeNode fooNamespaceTreeNode;
		CodeCoverageNamespaceTreeNode fooTestsNamespaceTreeNode;
		CodeCoverageNamespaceTreeNode fooTestsUtilNamespaceTreeNode;
		
		[TestFixtureSetUp]
		public void SetUpFixture()
		{
			List<CodeCoverageModule> modules = new List<CodeCoverageModule>();
			CodeCoverageModule fooModule = new CodeCoverageModule("Foo.Tests");
			CodeCoverageMethod fooTestMethod1 = new CodeCoverageMethod("FooTest1", "Foo.Tests.FooTestFixture");
			fooTestMethod1.SequencePoints.Add(new CodeCoverageSequencePoint("c:\\Projects\\Foo\\FooTestFixture.cs", 1, 1, 0, 2, 1, 1));
			fooTestMethod1.SequencePoints.Add(new CodeCoverageSequencePoint("c:\\Projects\\Foo\\FooTestFixture.cs", 0, 2, 2, 3, 4, 1));
			CodeCoverageMethod fooTestMethod2 = new CodeCoverageMethod("FooTest2", "Foo.Tests.FooTestFixture");
			CodeCoverageMethod helperMethod = new CodeCoverageMethod("GetCoverageFile", "Foo.Tests.Util.Helper");
				
			fooModule.Methods.Add(fooTestMethod1);
			fooModule.Methods.Add(fooTestMethod2);
			fooModule.Methods.Add(helperMethod);
			
			CodeCoverageModule barModule = new CodeCoverageModule("Bar.Tests");
			
			modules.Add(barModule);
			modules.Add(fooModule);
			
			using (CodeCoverageTreeView treeView = new CodeCoverageTreeView()) {
				treeView.AddModules(modules);
				nodes = treeView.Nodes;
			}
			
			barModuleNode = (CodeCoverageModuleTreeNode)nodes[0];
			fooModuleNode = (CodeCoverageModuleTreeNode)nodes[1];
				
			fooModuleNode.Expanding();
			fooNamespaceTreeNode = (CodeCoverageNamespaceTreeNode)fooModuleNode.Nodes[0];
			
			fooNamespaceTreeNode.Expanding();
			fooTestsNamespaceTreeNode = (CodeCoverageNamespaceTreeNode)fooNamespaceTreeNode.Nodes[0];

			fooTestsNamespaceTreeNode.Expanding();
			fooTestsUtilNamespaceTreeNode = (CodeCoverageNamespaceTreeNode)fooTestsNamespaceTreeNode.Nodes[0];
			fooTestFixtureTreeNode = (CodeCoverageClassTreeNode)fooTestsNamespaceTreeNode.Nodes[1];

			fooTestFixtureTreeNode.Expanding();		
			fooTestMethod1TreeNode = (CodeCoverageMethodTreeNode)fooTestFixtureTreeNode.Nodes[0];
			fooTestMethod2TreeNode = (CodeCoverageMethodTreeNode)fooTestFixtureTreeNode.Nodes[1];
		}
		
		[Test]
		public void RootNodesCount_TwoModules_ReturnsTwo()
		{
			int count = nodes.Count;
			Assert.AreEqual(2, count);
		}
		
		[Test]
		public void ModuleTreeNodeText_FooModuleTreeNode_ReturnsModuleNameWithPercentage()
		{
			string text = fooModuleNode.Text;
			string expectedText = "Foo.Tests (50%)";
			Assert.AreEqual(expectedText, text);
		}
				
		[Test]
		public void ModuleTreeNodeForeColor_FooModuleTreeNode_ForeColorIsPartialCoverageTextColor()
		{
			Color color = fooModuleNode.ForeColor;
			Color expectedColor = CodeCoverageTreeNode.PartialCoverageTextColor;
			Assert.AreEqual(expectedColor, color);
		}
		
		[Test]
		public void ModuleTreeNodeNodesCount_FooModule_OneChildNode()
		{
			int count = fooModuleNode.Nodes.Count;
			Assert.AreEqual(1, count);
		}
		
		[Test]
		public void ModuleTreeNodeName_FooModule_ReturnsAssemblyName()
		{
			string name = fooModuleNode.Name;
			string expectedName = "Foo.Tests";
			Assert.AreEqual(expectedName, name);
		}
		
		[Test]
		public void ModuleTreeNodeText_BarModule_ReturnsAssemblyName()
		{
			string text = barModuleNode.Text;
			string expectedText = "Bar.Tests";
			Assert.AreEqual(expectedText, text);
		}
		
		[Test]
		public void ModuleTreeNodeForeColor_BarModule_ForeColorIsForZeroCodeCoverage()
		{
			Color color = barModuleNode.ForeColor;
			Color expectedColor = CodeCoverageTreeNode.ZeroCoverageTextColor;
			Assert.AreEqual(expectedColor, color);
		}
		
		[Test]
		public void ClassTreeNodeName_FooTestFixtureTreeNode_ReturnsClassName()
		{
			string name = fooTestFixtureTreeNode.Name;
			string expectedName = "FooTestFixture";
			Assert.AreEqual(expectedName, name);
		}
		
		[Test]
		public void ClassTreeNodeText_FooTestFixtureTreeNode_ReturnsClassNameWithPercentageCoverage()
		{
			string name = fooTestFixtureTreeNode.Text;
			string expectedName = "FooTestFixture (50%)";
			Assert.AreEqual(expectedName, name);
		}
		
		[Test]
		public void NamespaceTreeNodeName_FooNamespaceTreeNode_ReturnsNamespaceName()
		{
			string name = fooNamespaceTreeNode.Name;
			string expectedName = "Foo";
			Assert.AreEqual(expectedName, name);
		}
		
		[Test]
		public void NamespaceTreeNodeName_FooTestsNamespaceTreeNode_ReturnsNamespaceName()
		{
			string name = fooTestsNamespaceTreeNode.Name;
			string expectedName = "Tests";
			Assert.AreEqual(expectedName, name);
		}
		
		[Test]
		public void NamespaceTreeNodeText_FooNamespaceTreeNode_ReturnsNamespaceNameWithPercentageCodeCoverages()
		{
			string text = fooNamespaceTreeNode.Text;
			string expectedText = "Foo (50%)";
			Assert.AreEqual(expectedText, text);
		}
		
		[Test]
		public void NamespaceTreeNodeText_FooTestsNamespaceTreeNode_ReturnsNamespaceWithPercentageCodeCoverage()
		{
			string text = fooTestsNamespaceTreeNode.Text;
			string expectedText = "Tests (50%)";
			Assert.AreEqual(expectedText, text);
		}
		
		[Test]
		public void MethodTreeNodeName_FooTestMethod1TreeNode_ReturnsMethodName()
		{
			string name = fooTestMethod1TreeNode.Name;
			string expectedText = "FooTest1";
			Assert.AreEqual(expectedText, name);
		}
		
		[Test]
		public void MethodTreeNodeText_FooTestMethod1TreeNode_ReturnsMethodNameWithPercentageCodeCoverage()
		{
			string text = fooTestMethod1TreeNode.Text;
			string expectedText = "FooTest1 (50%)";
			Assert.AreEqual(expectedText, text);
		}
		
		[Test]
		public void MethodTreeNodeName_FooMethod2TreeNode_ReturnsMethodName()
		{
			string name = fooTestMethod2TreeNode.Name;
			string expectedName = "FooTest2";
			Assert.AreEqual(expectedName, name);
		}
		
		[Test]
		public void ClassTreeNodeChildNodesCount_FooTestFixtureTreeNode_ReturnsTwoChildNodes()
		{
			int count = fooTestFixtureTreeNode.Nodes.Count;
			Assert.AreEqual(2, count);
		}
		
		[Test]
		public void NamespaceTreeNodeChildNodesCount_FooTestsNamespaceTreeNode_ReturnsTwoChildNodes()
		{
			int count = fooTestsNamespaceTreeNode.Nodes.Count;
			Assert.AreEqual(2, count);
		}
		
		[Test]
		public void ClassTreeNodeImageIndex_FooTestFixtureTreeNode_ReturnsClassImageIndex()
		{
			CodeCoverageImageListIndex actual = (CodeCoverageImageListIndex)(fooTestFixtureTreeNode.ImageIndex);
			CodeCoverageImageListIndex expected = CodeCoverageImageListIndex.Class;
			Assert.AreEqual(expected, actual);
		}
	}
}
