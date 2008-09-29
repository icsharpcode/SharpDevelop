// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.CodeCoverage;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace ICSharpCode.CodeCoverage.Tests
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
			fooTestMethod1.SequencePoints.Add(new CodeCoverageSequencePoint("c:\\Projects\\Foo\\FooTestFixture.cs", 1, 1, 0, 2, 1));
			fooTestMethod1.SequencePoints.Add(new CodeCoverageSequencePoint("c:\\Projects\\Foo\\FooTestFixture.cs", 0, 2, 2, 3, 4));
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
		public void RootNodesCount()
		{
			Assert.AreEqual(2, nodes.Count);
		}
		
		[Test]
		public void FooModuleTreeNodeText()
		{
			Assert.AreEqual("Foo.Tests (50%)", fooModuleNode.Text);
		}
		
		[Test]
		public void FooModuleTreeNodeForeColor()
		{
			Assert.AreEqual(CodeCoverageTreeNode.PartialCoverageTextColor, fooModuleNode.ForeColor);
		}
		
		[Test]
		public void FooModuleChildNodesCount()
		{
			Assert.AreEqual(1, fooModuleNode.Nodes.Count);
		}
		
		[Test]
		public void FooModuleTreeNodeName()
		{
			Assert.AreEqual("Foo.Tests", fooModuleNode.Name);
		}
		
		[Test]
		public void BarModuleTreeNodeText()
		{
			Assert.AreEqual("Bar.Tests", barModuleNode.Text);
		}
		
		[Test]
		public void BarModuleTreeNodeForeColor()
		{
			Assert.AreEqual(CodeCoverageTreeNode.ZeroCoverageTextColor, barModuleNode.ForeColor);
		}
		
		[Test]
		public void FooTestFixtureTreeNodeName()
		{
			Assert.AreEqual("FooTestFixture", fooTestFixtureTreeNode.Name);
		}
		
		[Test]
		public void FooTestFixtureTreeNodeText()
		{
			Assert.AreEqual("FooTestFixture (50%)", fooTestFixtureTreeNode.Text);
		}
		
		[Test]
		public void FooNamespaceTreeNodeName()
		{
			Assert.AreEqual("Foo", fooNamespaceTreeNode.Name);
		}
		
		[Test]
		public void FooTestsNamespaceTreeNodeName()
		{
			Assert.AreEqual("Tests", fooTestsNamespaceTreeNode.Name);
		}
		
		[Test]
		public void FooNamespaceTreeNodeText()
		{
			Assert.AreEqual("Foo (50%)", fooNamespaceTreeNode.Text);
		}
		
		[Test]
		public void FooTestsNamespaceTreeNodeText()
		{
			Assert.AreEqual("Tests (50%)", fooTestsNamespaceTreeNode.Text);
		}
		
		[Test]
		public void FooTestMethod1TreeNodeName()
		{
			Assert.AreEqual("FooTest1", fooTestMethod1TreeNode.Name);
		}
		
		[Test]
		public void FooTestMethod1TreeNodeText()
		{
			Assert.AreEqual("FooTest1 (50%)", fooTestMethod1TreeNode.Text);
		}
		
		[Test]
		public void FooMethod2TreeNodeText()
		{
			Assert.AreEqual("FooTest2", fooTestMethod2TreeNode.Name);
		}
		
		[Test]
		public void FooTestFixtureTreeNodeChildNodesCount()
		{
			Assert.AreEqual(2, fooTestFixtureTreeNode.Nodes.Count);
		}
		
		[Test]
		public void FooTestsNamespaceTreeNodeChildNodesCount()
		{
			Assert.AreEqual(2, fooTestsNamespaceTreeNode.Nodes.Count);
		}
		
		[Test]
		public void FooTestFixtureTreeNodeImageIndex()
		{
			Assert.AreEqual(CodeCoverageImageListIndex.Class, (CodeCoverageImageListIndex)(fooTestFixtureTreeNode.ImageIndex));
		}
	}
}
