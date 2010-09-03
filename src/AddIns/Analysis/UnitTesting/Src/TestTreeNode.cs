// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.UnitTesting
{
	/// <summary>
	/// Base class for all test tree nodes.
	/// </summary>
	public abstract class TestTreeNode : ExtTreeNode
	{
		TestProject testProject;
		
		public event EventHandler ImageIndexChanged;
		
		public TestTreeNode(TestProject testProject, string text)
		{
			this.testProject = testProject;
			this.Text = text;
			ImageIndex = (int)TestTreeViewImageListIndex.TestNotRun;
		}
		
		public IProject Project {
			get {
				return testProject.Project;
			}
		}
		
		public TestProject TestProject {
			get {
				return testProject;
			}
		}
		
		/// <summary>
		/// Changes the node's icon based on the specified test result.
		/// </summary>
		protected void UpdateImageListIndex(TestResultType result)
		{
			int previousImageIndex = ImageIndex;
			switch (result) {
				case TestResultType.Failure:
					ImageIndex = (int)TestTreeViewImageListIndex.TestFailed;
					break;
				case TestResultType.Success:
					ImageIndex = (int)TestTreeViewImageListIndex.TestPassed;
					break;
				case TestResultType.Ignored:
					ImageIndex = (int)TestTreeViewImageListIndex.TestIgnored;
					break;
				case TestResultType.None:
					ImageIndex = (int)TestTreeViewImageListIndex.TestNotRun;
					break;			
			}
			SelectedImageIndex = ImageIndex;
			if (previousImageIndex != ImageIndex) {
				OnImageIndexChanged();
			}
		}
		
		/// <summary>
		/// Raises the ImageIndexChanged event.
		/// </summary>
		protected void OnImageIndexChanged()
		{
			if (ImageIndexChanged != null) {
				ImageIndexChanged(this, new EventArgs());
			}
		}
		
		/// <summary>
		/// Checks all the child nodes of this node to see
		/// if the root namespace (i.e. the first part of the 
		/// namespace) exists in the tree.
		/// </summary>
		/// <param name="rootNamespace">The first dotted part
		/// of the namespace.</param>
		protected bool NamespaceNodeExists(string rootNamespace)
		{
			foreach (ExtTreeNode node in Nodes) {
				TestNamespaceTreeNode namespaceNode = node as TestNamespaceTreeNode;
				if (namespaceNode != null && namespaceNode.Text == rootNamespace) {
					return true;
				}
			}
			return false;
		}
		
		/// <summary>
		/// Sorts the immediate child nodes of this node. The sort
		/// is not done recursively.
		/// </summary>
		protected void SortChildNodes()
		{
			ExtTreeView treeView = (ExtTreeView)TreeView;
			treeView.SortNodes(Nodes, false);
		}
	}
}
