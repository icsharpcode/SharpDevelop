// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision: 2111 $</version>
// </file>

using System;
using System.Collections.Generic;
using System.Windows.Forms;

using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.XmlEditor;
using NUnit.Framework;
using XmlEditor.Tests.Utils;

namespace XmlEditor.Tests.Tree
{
	/// <summary>
	/// Tests that the XmlTreeView fires an AfterSelect event
	/// when the user clicks into the XmlTreeView control but not
	/// onto a node and deselects all nodes. The standard behaviour
	/// of a tree control is to not fire an AfterSelect event in
	/// this case.
	/// </summary>
	[TestFixture]
	public class MouseDownTestFixture
	{
		DerivedXmlTreeViewControl treeView;
		List<TreeViewEventArgs> treeViewEventArgs;
		
		[SetUp]
		public void SetUpFixture()
		{
			treeViewEventArgs = new List<TreeViewEventArgs>();
			treeView = new DerivedXmlTreeViewControl();
			treeView.Height = 100;
			treeView.Nodes.Add(new ExtTreeNode());
			treeView.AfterSelect += XmlTreeViewAfterSelect;
		}
		
		[TearDown]
		public void TearDownFixture()
		{
			if (treeView != null) {
				treeView.AfterSelect -= XmlTreeViewAfterSelect;
				treeView.Dispose();
			}
		}
		
		/// <summary>
		/// Make sure the AfterSelect event is not fired twice and the
		/// standard behaviour of the OnMouseDown method is preserved
		/// in the normal case.
		/// </summary>
		[Test]
		public void MouseDownWithNodeSelected()
		{
			treeView.SelectedNode = treeView.Nodes[0];
			
			// Make sure the button click will select the first node
			// so choose x=0, y=0.
			MouseEventArgs e = new MouseEventArgs(MouseButtons.Left, 1, 0, 0, 0);
			treeView.CallMouseDown(e);
			
			Assert.IsNotNull(treeView.SelectedNode, "Sanity check: The mouse down call should not deselect the tree node.");
			Assert.AreEqual(1, treeViewEventArgs.Count, "AfterSelect event should be fired once.");
		}
		
		[Test]
		public void MouseDownWithNoNodeSelected()
		{			
			treeView.SelectedNode = null;
			
			// Make sure the mouse click will not select any
			// tree node (x=0, y=99 - Height of control=100)
			treeViewEventArgs.Clear();
			MouseEventArgs e = new MouseEventArgs(MouseButtons.Left, 1, 0, 99, 0);
			treeView.CallMouseDown(e);
			
			Assert.IsNull(treeView.SelectedNode, "Sanity check: The mouse down call should not select a tree node");
			Assert.AreEqual(1, treeViewEventArgs.Count, "AfterSelect event should be fired once.");
			TreeViewEventArgs treeViewEventArg = treeViewEventArgs[0];
			Assert.IsNull(treeViewEventArg.Node);
			Assert.AreEqual(TreeViewAction.ByMouse, treeViewEventArg.Action);
		}
		
		void XmlTreeViewAfterSelect(object sender, TreeViewEventArgs e)
		{
			treeViewEventArgs.Add(e);
		}
	}
}
