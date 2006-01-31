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
	[TestFixture]
	public class CodeCoverageTreeViewTestFixture
	{
		TreeNodeCollection nodes;
		CodeCoverageModuleTreeNode fooModuleNode;
		CodeCoverageModuleTreeNode barModuleNode;
		CodeCoverageClassTreeNode class1TreeNode;
		CodeCoverageMethodTreeNode method1TreeNode;
		CodeCoverageMethodTreeNode method2TreeNode;
		CodeCoverageNamespaceTreeNode namespace1TreeNode;
		CodeCoverageNamespaceTreeNode namespace2TreeNode;
		
		[TestFixtureSetUp]
		public void SetUpFixture()
		{
			List<CodeCoverageModule> modules = new List<CodeCoverageModule>();
			CodeCoverageModule m1 = new CodeCoverageModule("Foo.Tests");
			CodeCoverageMethod method1 = new CodeCoverageMethod("Test1", "Foo.Tests.TestFixture1");
			method1.SequencePoints.Add(new CodeCoverageSequencePoint("c:\\Projects\\Foo\\TestFixture1.cs", 1, 1, 0, 2, 1));
			method1.SequencePoints.Add(new CodeCoverageSequencePoint("c:\\Projects\\Foo\\TestFixture1.cs", 0, 2, 2, 3, 4));
			CodeCoverageMethod method2 = new CodeCoverageMethod("Test2", "Foo.Tests.TestFixture1");

			m1.Methods.Add(method1);
			m1.Methods.Add(method2);
			
			CodeCoverageModule m2 = new CodeCoverageModule("Bar.Tests");
			
			modules.Add(m1);
			modules.Add(m2);
			
			using (CodeCoverageTreeView treeView = new CodeCoverageTreeView()) {
				treeView.AddModules(modules);
				treeView.ExpandAll();
				nodes = treeView.Nodes;
			}
			
			fooModuleNode = (CodeCoverageModuleTreeNode)nodes[0];
			barModuleNode = (CodeCoverageModuleTreeNode)nodes[1];
			
			namespace1TreeNode = (CodeCoverageNamespaceTreeNode)fooModuleNode.Nodes[0];
			namespace2TreeNode = (CodeCoverageNamespaceTreeNode)namespace1TreeNode.Nodes[0];
			
			class1TreeNode = (CodeCoverageClassTreeNode)namespace2TreeNode.Nodes[0];
			method1TreeNode = (CodeCoverageMethodTreeNode)class1TreeNode.Nodes[0];
			method2TreeNode = (CodeCoverageMethodTreeNode)class1TreeNode.Nodes[1];
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
		public void Class1TreeNodeName()
		{
			Assert.AreEqual("TestFixture1", class1TreeNode.Name);
		}
		
		[Test]
		public void Class1TreeNodeText()
		{
			Assert.AreEqual("TestFixture1 (50%)", class1TreeNode.Text);
		}
		
		[Test]
		public void Namespace1TreeNodeName()
		{
			Assert.AreEqual("Foo", namespace1TreeNode.Name);
		}
		
		[Test]
		public void Namespace2TreeNodeName()
		{
			Assert.AreEqual("Tests", namespace2TreeNode.Name);
		}
		
		[Test]
		public void Namespace1TreeNodeText()
		{
			Assert.AreEqual("Foo (50%)", namespace1TreeNode.Text);
		}
		
		[Test]
		public void Namespace2TreeNodeText()
		{
			Assert.AreEqual("Tests (50%)", namespace2TreeNode.Text);
		}
		
		[Test]
		public void Method1TreeNodeName()
		{
			Assert.AreEqual("Test1", method1TreeNode.Name);
		}
		
		[Test]
		public void Method1TreeNodeText()
		{
			Assert.AreEqual("Test1 (50%)", method1TreeNode.Text);
		}
		
		[Test]
		public void Method2TreeNodeText()
		{
			Assert.AreEqual("Test2", method2TreeNode.Name);
		}
		
		[Test]
		public void Class1TreeNodeChildNodesCount()
		{
			Assert.AreEqual(2, class1TreeNode.Nodes.Count);
		}
		
		[Test]
		public void Namespace2TreeNodeChildNodesCount()
		{
			Assert.AreEqual(1, namespace2TreeNode.Nodes.Count);
		}
	}
}
