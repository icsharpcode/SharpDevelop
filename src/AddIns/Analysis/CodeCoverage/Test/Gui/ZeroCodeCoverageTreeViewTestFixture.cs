// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.CodeCoverage;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace ICSharpCode.CodeCoverage.Tests.Gui
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
			fooTestMethod.SequencePoints.Add(new CodeCoverageSequencePoint("c:\\Projects\\Foo\\FooTestFixture.cs", 0, 1, 0, 2, 1, 1));
			fooTestMethod.SequencePoints.Add(new CodeCoverageSequencePoint("c:\\Projects\\Foo\\FooTestFixture.cs", 0, 2, 2, 3, 4, 1));

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
		public void ModuleTreeNodeText_FooModuleTreeNode_ShowsModuleNameAndPercentageCoverage()
		{
			string text = fooModuleNode.Text;
			string expectedText = "Foo.Tests (0%)";
			Assert.AreEqual(expectedText, text);
		}
		
		[Test]
		public void ModuleTreeNodeForeColor_FooModuleTreeNode_IsZeroCodeCoverageTextColor()
		{
			Color color = fooModuleNode.ForeColor;
			Color expectedColor = CodeCoverageTreeNode.ZeroCoverageTextColor;
			Assert.AreEqual(expectedColor, color);
		}
		
		[Test]
		public void MethodTreeNodeText_FooMethodTreeNode_ShowsMethodNameAndCodeCoveragePercentage()
		{
			string text = fooTestMethodTreeNode.Text;
			string expectedText = "FooTest (0%)";
			Assert.AreEqual(expectedText, text);
		}
		
		[Test]
		public void MethodTreeNodeForeColor_FooMethodTreeNode_IsZeroCodeCoverageTextColor()
		{
			Color color = fooTestMethodTreeNode.ForeColor;
			Color expectedColor = CodeCoverageTreeNode.ZeroCoverageTextColor;
			Assert.AreEqual(expectedColor, color);
		}
		
		[Test]
		public void ClassTreeNodeForeColor_FooTestFixtureTreeNode_IsZeroCodeCoverageTextColor()
		{
			Color color = fooTestFixtureTreeNode.ForeColor;
			Color expectedColor = CodeCoverageTreeNode.ZeroCoverageTextColor;
			Assert.AreEqual(expectedColor, color);
		}
		
		[Test]
		public void MethodTreeNodeImageIndex_FooTestMethodTreeNode_ImageIndexIsForMethodWithZeroCodeCoverage()
		{
			CodeCoverageImageListIndex actual = (CodeCoverageImageListIndex)(fooTestMethodTreeNode.ImageIndex);
			CodeCoverageImageListIndex expected = CodeCoverageImageListIndex.MethodWithZeroCoverage;
			Assert.AreEqual(expected, actual);
		}
		
		[Test]
		public void MethodTreeNodeImageIndex_FooTestMethodVisitedCodeLengthChangedToNonZero_ImageIndexChangedToImageIndexForMethodWithNonZeroCodeCoverage()
		{
			fooTestMethodTreeNode.VisitedCodeLength = 1;
			CodeCoverageImageListIndex actual = (CodeCoverageImageListIndex)(fooTestMethodTreeNode.ImageIndex);
			CodeCoverageImageListIndex expected = CodeCoverageImageListIndex.Method;
			Assert.AreEqual(expected, actual);
		}
		
		[Test]
		public void MethodTreeNodeVisiteCount_FooTestMethodVisitedCodeLengthSetToNonZero_VisitedCodeLengthSetToNonZeroValue()
		{
			fooTestMethodTreeNode.VisitedCodeLength = 1;
			int length = fooTestMethodTreeNode.VisitedCodeLength;
			Assert.AreEqual(1, length);
		}
		
		[Test]
		public void MethodTreeNodeForeColor_FooTestMethodVisitCodeLengthChangedSoCodeCoverageIsOneHundredPercent_ImageIndexChangedTo()
		{
			fooTestMethodTreeNode.UnvisitedCodeLength = 0;
			fooTestMethodTreeNode.VisitedCodeLength = 2;
			
			Color color = fooTestMethodTreeNode.ForeColor;
			Color expectedColor = Color.Empty;
			Assert.AreEqual(expectedColor, color);
		}
		
		[Test]
		public void MethodTreeNodeNotVisitedCount_FooTestMethodUnvisitedCodeLengthChangedToZero_UnvisitedCodeLengthSetToZero()
		{
			fooTestMethodTreeNode.UnvisitedCodeLength = 0;
			int length = fooTestMethodTreeNode.UnvisitedCodeLength;
			Assert.AreEqual(0, length);
		}
	}
}
