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
using System.Collections.Generic;
using System.Windows.Forms;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.WinForms;
using ICSharpCode.SharpDevelop.Workbench;
using ICSharpCode.XmlEditor;
using NUnit.Framework;
using Rhino.Mocks;
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
		public void SetUp()
		{
			SD.InitializeForUnitTests();
			SD.Services.AddStrictMockService<IWinFormsService>();
			SD.WinForms.Stub(w => w.MenuService).Return(MockRepository.GenerateStub<IWinFormsMenuService>());
			
			treeViewEventArgs = new List<TreeViewEventArgs>();
			treeView = new DerivedXmlTreeViewControl();
			treeView.Height = 100;
			treeView.Nodes.Add(new ExtTreeNode());
			treeView.AfterSelect += XmlTreeViewAfterSelect;
		}
		
		[TearDown]
		public void TearDown()
		{
			if (treeView != null) {
				treeView.AfterSelect -= XmlTreeViewAfterSelect;
				treeView.Dispose();
			}
			SD.TearDownForUnitTests();
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
