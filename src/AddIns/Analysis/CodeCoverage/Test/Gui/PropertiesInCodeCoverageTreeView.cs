// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.CodeCoverage;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Windows.Forms;

namespace ICSharpCode.CodeCoverage.Tests.Gui
{
	/// <summary>
	/// Tests that getter and setter methods are shown in the tree view as child nodes of a parent
	/// property node.
	/// </summary>
	[TestFixture]
	public class PropertiesInCodeCoverageTreeView
	{
		ExtTreeNode fooModuleNode;
		CodeCoverageClassTreeNode fooTestTreeNode;
		CodeCoverageMethodTreeNode fooGetterTreeNode;
		CodeCoverageMethodTreeNode fooSetterTreeNode;
		CodeCoverageNamespaceTreeNode fooTestsNamespaceTreeNode;
		CodeCoveragePropertyTreeNode countPropertyTreeNode;
		CodeCoverageMethod fooGetterMethod;
		CodeCoverageMethod fooSetterMethod;
		
		[TestFixtureSetUp]
		public void SetUpFixture()
		{
			List<CodeCoverageModule> modules = new List<CodeCoverageModule>();
			CodeCoverageModule fooModule = new CodeCoverageModule("Tests");
			fooSetterMethod = new CodeCoverageMethod("set_Count", "Tests.FooTest", MethodAttributes.SpecialName);
			
			fooSetterMethod.SequencePoints.Add(new CodeCoverageSequencePoint("c:\\Projects\\Foo\\FooTest.cs", 1, 2, 2, 3, 4, 2));
			fooSetterMethod.SequencePoints.Add(new CodeCoverageSequencePoint("c:\\Projects\\Foo\\FooTest.cs", 0, 3, 4, 4, 4, 1));
			fooGetterMethod = new CodeCoverageMethod("get_Count", "Tests.FooTest", MethodAttributes.SpecialName);
			fooGetterMethod.SequencePoints.Add(new CodeCoverageSequencePoint("c:\\Projects\\Foo\\FooTest.cs", 1, 1, 0, 2, 1, 1));
				
			fooModule.Methods.Add(fooGetterMethod);
			fooModule.Methods.Add(fooSetterMethod);
			
			modules.Add(fooModule);
			
			using (CodeCoverageTreeView treeView = new CodeCoverageTreeView()) {
				treeView.AddModules(modules);
				fooModuleNode = (CodeCoverageModuleTreeNode)treeView.Nodes[0];
			}
			
				
			fooModuleNode.Expanding();
			
			fooTestsNamespaceTreeNode = (CodeCoverageNamespaceTreeNode)fooModuleNode.Nodes[0];

			fooTestsNamespaceTreeNode.Expanding();
			fooTestTreeNode = (CodeCoverageClassTreeNode)fooTestsNamespaceTreeNode.Nodes[0];

			fooTestTreeNode.Expanding();
			if (fooTestTreeNode.Nodes.Count > 0) {
				countPropertyTreeNode = fooTestTreeNode.Nodes[0] as CodeCoveragePropertyTreeNode;
			}
			
			countPropertyTreeNode.Expanding();
			if (countPropertyTreeNode != null && countPropertyTreeNode.Nodes.Count > 1) {
				fooGetterTreeNode = (CodeCoverageMethodTreeNode)countPropertyTreeNode.Nodes[0];
				fooSetterTreeNode = (CodeCoverageMethodTreeNode)countPropertyTreeNode.Nodes[1];
			}
		}
		
		[Test]
		public void ClassTreeNodeChildNodesCount_FooClassTreeNode_OnlyHasOneChildNode()
		{
			Assert.AreEqual(1, fooTestTreeNode.Nodes.Count);
		}
		
		[Test]
		public void PropertyTreeNode_CountPropertyTreeNode_Exists()
		{
			Assert.IsNotNull(countPropertyTreeNode);
		}
		
		[Test]
		public void PropertyTreeNodeImageIndex_CountPropertyTreeNode_ImageIndexIsProperty()
		{
			Assert.AreEqual(CodeCoverageImageListIndex.Property, (CodeCoverageImageListIndex)countPropertyTreeNode.ImageIndex);
		}

		[Test]
		public void PropertyTreeNodeChildNodesCount_CountPropertyTreeNode_HasTwoChildNodes()
		{
			Assert.AreEqual(2, countPropertyTreeNode.Nodes.Count);
		}
		
		[Test]
		public void PropertyTreeNodeVisitedCodeLength_CountPropertyTreeNode_ReturnsThree()
		{
			Assert.AreEqual(3, countPropertyTreeNode.VisitedCodeLength);
		}
		
		[Test]
		public void VisitedCodeLength_PropertyTreeNode_ReturnsThree()
		{
			int count = countPropertyTreeNode.VisitedCodeLength;
			Assert.AreEqual(3, count);
		}
		
		[Test]
		public void UnvisitedCodeLength_PropertyTreeNode_ReturnsThree()
		{
			int count = countPropertyTreeNode.UnvisitedCodeLength;
			Assert.AreEqual(1, count);
		}
		
		[Test]
		public void MethodTreeNodeMethod_GetterTreeNode_ReturnsGetterMethod()
		{
			Assert.AreEqual(fooGetterMethod, fooGetterTreeNode.Method);
		}
		
		[Test]
		public void MethodTreeNodeMethod_SetterTreeNode_ReturnsSetterMethod()
		{
			Assert.AreEqual(fooSetterMethod, fooSetterTreeNode.Method);
		}		
	}
}
