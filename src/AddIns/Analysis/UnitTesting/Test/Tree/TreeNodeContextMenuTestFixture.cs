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
