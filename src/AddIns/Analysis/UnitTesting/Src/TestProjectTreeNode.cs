// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.UnitTesting
{
	/// <summary>
	/// The root tree node for a project that has tests.
	/// </summary>
	public class TestProjectTreeNode : TestTreeNode
	{
		public TestProjectTreeNode(TestProject project)
			: base(project, project.Name)
		{
			Nodes.Add(new ExtTreeNode());
			TestProject.TestClasses.ResultChanged += TestClassesResultChanged;
			TestProject.TestClasses.TestClassAdded += TestClassAdded;
			TestProject.TestClasses.TestClassRemoved += TestClassRemoved;
		}
		
		public override void Dispose()
		{
			if (!IsDisposed) {
				TestProject.TestClasses.ResultChanged -= TestClassesResultChanged;	
				TestProject.TestClasses.TestClassAdded -= TestClassAdded;
				TestProject.TestClasses.TestClassRemoved -= TestClassRemoved;
			}
			base.Dispose();
		}
		
		/// <summary>
		/// Adds the child nodes after this node has been expanded.
		/// </summary>
		protected override void Initialize()
		{
			Nodes.Clear();
			
			// Add namespace nodes.
			foreach (string rootNamespace in TestProject.RootNamespaces) {
				TestNamespaceTreeNode node = new TestNamespaceTreeNode(TestProject, rootNamespace);
				node.AddTo(this);
			}
			
			// Add class nodes.
			foreach (TestClass c in TestProject.GetTestClasses(String.Empty)) {
				AddClassNode(c);
			}
			
			// Sort the nodes.
			SortChildNodes();
		}
		
		/// <summary>
		/// Updates this node's icon based on the overall result of the
		/// test classes.
		/// </summary>
		void TestClassesResultChanged(object source, EventArgs e)
		{
			UpdateImageListIndex(TestProject.TestClasses.Result);
		}
		
		/// <summary>
		/// Adds a new class node to this project node if the
		/// class added has no root namespace.
		/// </summary>
		void TestClassAdded(object source, TestClassEventArgs e)
		{
			if (e.TestClass.Namespace == String.Empty) {
				AddClassNode(e.TestClass);
				SortChildNodes();
			} else if (isInitialized) {
				// Check that we have a namespace node for this class.
				if (!NamespaceNodeExists(e.TestClass.RootNamespace)) {
					// Add a new namespace node.
					TestNamespaceTreeNode node = new TestNamespaceTreeNode(TestProject, e.TestClass.RootNamespace);
					node.AddTo(this);
					SortChildNodes();
				}
			}
		}
		
		/// <summary>
		/// Removes the corresponding tree node that is a child of
		/// this project tree node if the class has no root namespace.
		/// </summary>
		void TestClassRemoved(object source, TestClassEventArgs e)
		{
			if (e.TestClass.Namespace == String.Empty) {
				foreach (ExtTreeNode node in Nodes) {
					TestClassTreeNode classNode = node as TestClassTreeNode;
					if (classNode != null && classNode.Text == e.TestClass.Name) {
						classNode.Remove();
						classNode.Dispose();
						break;
					}
				}
			}
		}
		
		/// <summary>
		/// Adds a new TestClassTreeNode to this node.
		/// </summary>
		void AddClassNode(TestClass c)
		{
			TestClassTreeNode node = new TestClassTreeNode(TestProject, c);
			node.AddTo(this);
		}
	}
}
