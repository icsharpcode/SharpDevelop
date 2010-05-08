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
using System.Drawing;
using System.Windows.Forms;

namespace ICSharpCode.CodeCoverage.Tests
{
	[TestFixture]
	public class ZeroCodeCoverageTreeViewTestFixture
	{
		TreeNodeCollection nodes;
		CodeCoverageModuleTreeNode fooModuleNode;
		CodeCoverageClassTreeNode fooTestFixtureTreeNode;
		CodeCoverageMethodTreeNode fooTestMethodTreeNode;
		CodeCoverageNamespaceTreeNode fooNamespaceTreeNode;
		CodeCoverageNamespaceTreeNode fooTestsNamespaceTreeNode;
		
		[SetUp]
		public void Init()
		{
			List<CodeCoverageModule> modules = new List<CodeCoverageModule>();
			CodeCoverageModule fooModule = new CodeCoverageModule("Foo.Tests");
			CodeCoverageMethod fooTestMethod = new CodeCoverageMethod("FooTest", "Foo.Tests.FooTestFixture");
			fooTestMethod.SequencePoints.Add(new CodeCoverageSequencePoint("c:\\Projects\\Foo\\FooTestFixture.cs", 0, 1, 0, 2, 1));
			fooTestMethod.SequencePoints.Add(new CodeCoverageSequencePoint("c:\\Projects\\Foo\\FooTestFixture.cs", 0, 2, 2, 3, 4));

			fooModule.Methods.Add(fooTestMethod);
					
			modules.Add(fooModule);
			
			using (CodeCoverageTreeView treeView = new CodeCoverageTreeView()) {
				treeView.AddModules(modules);
				nodes = treeView.Nodes;
			}
			
			fooModuleNode = (CodeCoverageModuleTreeNode)nodes[0];
			
			fooModuleNode.Expanding();
			fooNamespaceTreeNode = (CodeCoverageNamespaceTreeNode)fooModuleNode.Nodes[0];
			
			fooNamespaceTreeNode.Expanding();
			fooTestsNamespaceTreeNode = (CodeCoverageNamespaceTreeNode)fooNamespaceTreeNode.Nodes[0];
			
			fooTestsNamespaceTreeNode.Expanding();
			fooTestFixtureTreeNode = (CodeCoverageClassTreeNode)fooTestsNamespaceTreeNode.Nodes[0];
			
			fooTestFixtureTreeNode.Expanding();
			fooTestMethodTreeNode = (CodeCoverageMethodTreeNode)fooTestFixtureTreeNode.Nodes[0];
		}

		[Test]
		public void FooModuleTreeNodeText()
		{
			Assert.AreEqual("Foo.Tests (0%)", fooModuleNode.Text);
		}
		
		[Test]
		public void FooModuleTreeNodeForeColor()
		{
			Assert.AreEqual(CodeCoverageTreeNode.ZeroCoverageTextColor, fooModuleNode.ForeColor);
		}
		
		[Test]
		public void FooMethodTreeNodeText()
		{
			Assert.AreEqual("FooTest (0%)", fooTestMethodTreeNode.Text);
		}
		
		[Test]
		public void FooMethodTreeNodeForeColor()
		{
			Assert.AreEqual(CodeCoverageTreeNode.ZeroCoverageTextColor, fooTestMethodTreeNode.ForeColor);
		}
		
		[Test]
		public void FooTestFixtureTreeNodeForeColor()
		{
			Assert.AreEqual(CodeCoverageTreeNode.ZeroCoverageTextColor, fooTestFixtureTreeNode.ForeColor);
		}
		
		[Test]
		public void FooMethodTreeNodeImageIndex()
		{
			Assert.AreEqual(CodeCoverageImageListIndex.MethodWithZeroCoverage, (CodeCoverageImageListIndex)(fooTestMethodTreeNode.ImageIndex));
		}
		
		[Test]
		public void ChangeFooMethodFixtureVisitCount()
		{
			fooTestMethodTreeNode.VisitedCount = 1;
			Assert.AreEqual(CodeCoverageImageListIndex.Method, (CodeCoverageImageListIndex)(fooTestMethodTreeNode.ImageIndex));
			Assert.AreEqual(1, fooTestMethodTreeNode.VisitedCount);
		}

		[Test]
		public void ChangeFooMethodFixtureTotalVisitsCount()
		{
			fooTestMethodTreeNode.NotVisitedCount = 0;
			fooTestMethodTreeNode.VisitedCount = 2;
			Assert.AreEqual(Color.Empty, fooTestMethodTreeNode.ForeColor);
			Assert.AreEqual(0, fooTestMethodTreeNode.NotVisitedCount);
		}
	}
}
