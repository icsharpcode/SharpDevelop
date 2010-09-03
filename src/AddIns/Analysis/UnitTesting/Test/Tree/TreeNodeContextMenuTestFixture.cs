// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Windows.Forms;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.UnitTesting;
using NUnit.Framework;
using UnitTesting.Tests.Utils;

namespace UnitTesting.Tests.Tree
{
	/// <summary>
	/// Tests that the TestTreeNode's ContextMenuStrip is set so
	/// that if the user uses the shortcut Shift+F10 the menu appears
	/// at the node rather than in the middle of the Unit Tests window.
	/// </summary>
	[TestFixture]
	public class TreeNodeContextMenuTestFixture
	{
		/// <summary>
		/// Tests that the context menu strip for the tree node is
		/// the same as that used with the tree view.
		/// </summary>
		[Test]
		public void TreeNodeContextMenuMatches()
		{
			using (DerivedTestTreeView treeView = new DerivedTestTreeView()) {
				ContextMenuStrip menuStrip = new ContextMenuStrip();
				treeView.ContextMenuStrip = menuStrip;
				
				// Add a root node to the tree.
				TestProject project = new TestProject(new MockCSharpProject(), new MockProjectContent(), new MockTestFrameworksWithNUnitFrameworkSupport());
				TestProjectTreeNode node = new TestProjectTreeNode(project);
				node.AddTo(treeView);
				
				// Select the tree node.
				treeView.SelectedNode = node;
				
				// Call the treeView's OnBeforeSelect so the context menu 
				// is connected to the tree node.
				treeView.CallOnBeforeSelect(new TreeViewCancelEventArgs(node, false, TreeViewAction.ByMouse));
				
				// Context menu strip on tree node should match
				// the tree view's context menu strip.
				Assert.IsTrue(Object.ReferenceEquals(menuStrip, node.ContextMenuStrip));
			}
		}
	}
}
