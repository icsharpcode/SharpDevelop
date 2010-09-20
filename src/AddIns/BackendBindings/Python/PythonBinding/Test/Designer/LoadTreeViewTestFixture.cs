// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

using ICSharpCode.PythonBinding;
using ICSharpCode.Scripting.Tests.Utils;
using NUnit.Framework;
using PythonBinding.Tests.Utils;

namespace PythonBinding.Tests.Designer
{
	[TestFixture]
	public class LoadTreeViewTestFixture : LoadFormTestFixtureBase
	{		
		public override string PythonCode {
			get {
				return "class MainForm(System.Windows.Forms.Form):\r\n" +
						"    def InitializeComponent(self):\r\n" +
						"        treeNode1 = System.Windows.Forms.TreeNode()\r\n" +
						"        treeNode2 = System.Windows.Forms.TreeNode()\r\n" +
						"        treeNode3 = System.Windows.Forms.TreeNode()\r\n" +
						"        self._treeView1 = System.Windows.Forms.TreeView()\r\n" +
						"        self.SuspendLayout()\r\n" +
						"        # \r\n" +
						"        # treeView1\r\n" +
						"        # \r\n" +
						"        treeNode1.BackColor = System.Drawing.Color.Yellow\r\n" +
						"        treeNode1.Checked = True\r\n" +
						"        treeNode1.ForeColor = System.Drawing.Color.FromArgb(0, 64, 64)\r\n" +
						"        treeNode1.Name = \"RootNode0\"\r\n" +
						"        treeNode1.NodeFont = System.Drawing.Font(\"Times New Roman\", 8.25, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 1)\r\n" +
						"        treeNode1.Text = \"RootNode0.Text\"\r\n" +
						"        treeNode1.Nodes.AddRange(System.Array[System.Windows.Forms.TreeNode](\r\n" +
						"            [treeNode2]))\r\n" +
						"        treeNode2.Name = \"ChildNode0\"\r\n" +
						"        treeNode2.Text = \"ChildNode0.Text\"\r\n" +
						"        treeNode2.NodeFont = System.Drawing.Font(\"Garamond\", 8.25, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0)\r\n" +
						"        treeNode2.Nodes.AddRange(System.Array[System.Windows.Forms.TreeNode](\r\n" +
						"            [treeNode3]))\r\n" +
						"        treeNode3.Name = \"ChildNode1\"\r\n" +
						"        treeNode3.Text = \"ChildNode1.Text\"\r\n" +
						"        self._treeView1.Location = System.Drawing.Point(0, 0)\r\n" +
						"        self._treeView1.Name = \"treeView1\"\r\n" +
						"        self._treeView1.Nodes.AddRange(System.Array[System.Windows.Forms.TreeNode](\r\n" +
						"            [treeNode1]))\r\n" +
						"        self._treeView1.Size = System.Drawing.Size(100, 100)\r\n" +
						"        self._treeView1.TabIndex = 0\r\n" +
						"        # \r\n" +
						"        # MainForm\r\n" +
						"        # \r\n" +
						"        self.ClientSize = System.Drawing.Size(200, 300)\r\n" +
						"        self.Controls.Add(self._treeView1)\r\n" +
						"        self.Name = \"MainForm\"\r\n" +
						"        self.ResumeLayout(False)\r\n" +
						"        self.PerformLayout()\r\n";
			}
		}
		
		public TreeView TreeView {
			get { return Form.Controls[0] as TreeView; }
		}
		
		public TreeNode RootTreeNode {
			get { return TreeView.Nodes[0]; }
		}
		
		public TreeNode FirstChildTreeNode {
			get { return RootTreeNode.Nodes[0]; }
		}
		
		[Test]
		public void OneRootNode()
		{
			Assert.AreEqual(1, TreeView.Nodes.Count);
		}
		
		[Test]
		public void RootNodeHasOneChildNode()
		{
			Assert.AreEqual(1, RootTreeNode.Nodes.Count);
		}
		
		[Test]
		public void ChildNodeHasOneChildNode()
		{
			Assert.AreEqual(1, RootTreeNode.Nodes[0].Nodes.Count);
		}
		
		[Test]
		public void RootTreeNodeBackColor()
		{
			Assert.AreEqual(Color.Yellow, RootTreeNode.BackColor);
		}
		
		[Test]
		public void RootTreeNodeForeColor()
		{
			Assert.AreEqual(Color.FromArgb(0, 64, 64), RootTreeNode.ForeColor);
		}
		
		[Test]
		public void RootTreeNodeFontProperty()
		{
			Font font = new Font("Times New Roman", 8.25F, FontStyle.Regular, GraphicsUnit.Point, 1);
			Assert.AreEqual(font, RootTreeNode.NodeFont);
		}
		
		[Test]
		public void RootTreeNodeCheckedProperty()
		{
			Assert.IsTrue(RootTreeNode.Checked);
		}
		
		[Test]
		public void ChildTreeNodeFontProperty()
		{
			Font font = new Font("Garamond", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);		
			Assert.AreEqual(font, FirstChildTreeNode.NodeFont);
		}
		
		/// <summary>
		/// Ensures that we are not creating an instance with a name of NodeFont.
		/// </summary>
		[Test]
		public void InstancesCreated()
		{
			foreach (CreatedInstance instance in ComponentCreator.CreatedInstances) {
				if (instance.InstanceType == typeof(Font)) {
					Assert.IsNull(instance.Name);
				}
			}
			Assert.IsTrue(ComponentCreator.CreatedInstances.Count > 0);
		}
	}
}
