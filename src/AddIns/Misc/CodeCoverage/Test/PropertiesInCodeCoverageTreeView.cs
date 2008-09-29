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
using System.Reflection;
using System.Windows.Forms;

namespace ICSharpCode.CodeCoverage.Tests
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
			fooSetterMethod.SequencePoints.Add(new CodeCoverageSequencePoint("c:\\Projects\\Foo\\FooTest.cs", 1, 2, 2, 3, 4));
			fooSetterMethod.SequencePoints.Add(new CodeCoverageSequencePoint("c:\\Projects\\Foo\\FooTest.cs", 0, 3, 4, 4, 4));
			fooGetterMethod = new CodeCoverageMethod("get_Count", "Tests.FooTest", MethodAttributes.SpecialName);
			fooGetterMethod.SequencePoints.Add(new CodeCoverageSequencePoint("c:\\Projects\\Foo\\FooTest.cs", 1, 1, 0, 2, 1));
				
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
		public void FooClassOnlyHasOneChildNode()
		{
			Assert.AreEqual(1, fooTestTreeNode.Nodes.Count);
		}
		
		[Test]
		public void PropertyTreeNodeExists()
		{
			Assert.IsNotNull(countPropertyTreeNode);
		}
		
		[Test]
		public void PropertyTreeNodeImageIndexIsProperty()
		{
			Assert.AreEqual(CodeCoverageImageListIndex.Property, (CodeCoverageImageListIndex)countPropertyTreeNode.ImageIndex);
		}

		[Test]
		public void PropertyTreeNodeHasTwoChildNodes()
		{
			Assert.AreEqual(2, countPropertyTreeNode.Nodes.Count);
		}
		
		[Test]
		public void PropertyTreeNodeVisitedCount()
		{
			Assert.AreEqual(2, countPropertyTreeNode.VisitedCount);
		}
		
		[Test]
		public void GetterTreeNodeMethod()
		{
			Assert.AreEqual(fooGetterMethod, fooGetterTreeNode.Method);
		}
		
		[Test]
		public void SetterTreeNodeMethod()
		{
			Assert.AreEqual(fooSetterMethod, fooSetterTreeNode.Method);
		}		
	}
}
