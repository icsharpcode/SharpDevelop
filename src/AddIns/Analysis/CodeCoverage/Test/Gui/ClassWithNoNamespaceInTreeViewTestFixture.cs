// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.CodeCoverage;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace ICSharpCode.CodeCoverage.Tests.Gui
{
	[TestFixture]
	public class ClassWithNoNamespaceInTreeViewTestFixture
	{
		CodeCoverageMethodTreeNode methodNode;
		CodeCoverageModule module;
		
		[SetUp]
		public void Init()
		{
			CodeCoverageMethod method = new CodeCoverageMethod("FileElementHasAttributeNamedType", "AbstractElementSchemaTestFixture");
			module = new CodeCoverageModule("XmlEditor.Tests");
			module.Methods.Add(method);
			List<CodeCoverageModule> modules = new List<CodeCoverageModule>();
			modules.Add(module);
			
			TreeNodeCollection nodes;
			using (CodeCoverageTreeView treeView = new CodeCoverageTreeView()) {
				treeView.AddModules(modules);
				nodes = treeView.Nodes;
			}
			
			CodeCoverageModuleTreeNode moduleNode = (CodeCoverageModuleTreeNode)nodes[0];
			moduleNode.Expanding();
			CodeCoverageClassTreeNode classNode = (CodeCoverageClassTreeNode)moduleNode.Nodes[0];
			classNode.Expanding();
			methodNode = (CodeCoverageMethodTreeNode)classNode.Nodes[0];
		}
		
		[Test]
		public void MethodNodeName()
		{
			Assert.AreEqual("FileElementHasAttributeNamedType", methodNode.Name);
		}
		
		[Test]
		public void NoRootNamespaces()
		{
			Assert.AreEqual(0, module.RootNamespaces.Count, 
			"Should not be any root namespaces since the class does not have one.");
		}
		
		[Test]
		public void OneClassWithNoNamespace()
		{
			Assert.AreEqual(1, CodeCoverageMethod.GetClassNames(module.Methods, String.Empty).Count);
		}
		
		[Test]
		public void ClassWithNoNamespace()
		{
			Assert.AreEqual("AbstractElementSchemaTestFixture", (CodeCoverageMethod.GetClassNames(module.Methods, String.Empty))[0]);
		}
	}
}
