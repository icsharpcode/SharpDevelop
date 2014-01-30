// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

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
