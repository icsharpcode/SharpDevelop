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
