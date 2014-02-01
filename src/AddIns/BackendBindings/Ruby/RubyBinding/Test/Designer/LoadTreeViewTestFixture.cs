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
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

using ICSharpCode.RubyBinding;
using ICSharpCode.Scripting.Tests.Utils;
using NUnit.Framework;
using RubyBinding.Tests.Utils;

namespace RubyBinding.Tests.Designer
{
	[TestFixture]
	public class LoadTreeViewTestFixture : LoadFormTestFixtureBase
	{		
		public override string RubyCode {
			get {
				return
					"class MainForm < System::Windows::Forms::Form\r\n" +
					"    def InitializeComponent()\r\n" +
					"        treeNode1 = System::Windows::Forms::TreeNode.new()\r\n" +
					"        treeNode2 = System::Windows::Forms::TreeNode.new()\r\n" +
					"        treeNode3 = System::Windows::Forms::TreeNode.new()\r\n" +
					"        @treeView1 = System::Windows::Forms::TreeView.new()\r\n" +
					"        self.SuspendLayout()\r\n" +
					"        # \r\n" +
					"        # treeView1\r\n" +
					"        # \r\n" +
					"        treeNode1.BackColor = System::Drawing::Color.Yellow\r\n" +
					"        treeNode1.Checked = true\r\n" +
					"        treeNode1.ForeColor = System::Drawing::Color.FromArgb(0, 64, 64)\r\n" +
					"        treeNode1.Name = \"RootNode0\"\r\n" +
					"        treeNode1.NodeFont = System::Drawing::Font.new(\"Times New Roman\", 8.25, System::Drawing::FontStyle.Regular, System::Drawing::GraphicsUnit.Point, 1)\r\n" +
					"        treeNode1.Text = \"RootNode0.Text\"\r\n" +
					"        treeNode1.Nodes.AddRange(System::Array[System::Windows::Forms::TreeNode].new(\r\n" +
					"            [treeNode2]))\r\n" +
					"        treeNode2.Name = \"ChildNode0\"\r\n" +
					"        treeNode2.Text = \"ChildNode0.Text\"\r\n" +
					"        treeNode2.NodeFont = System::Drawing::Font.new(\"Garamond\", 8.25, System::Drawing::FontStyle.Regular, System::Drawing::GraphicsUnit.Point, 0)\r\n" +
					"        treeNode2.Nodes.AddRange(System::Array[System::Windows::Forms::TreeNode].new(\r\n" +
					"            [treeNode3]))\r\n" +
					"        treeNode3.Name = \"ChildNode1\"\r\n" +
					"        treeNode3.Text = \"ChildNode1.Text\"\r\n" +
					"        @treeView1.Location = System::Drawing::Point.new(0, 0)\r\n" +
					"        @treeView1.Name = \"treeView1\"\r\n" +
					"        @treeView1.Nodes.AddRange(System::Array[System::Windows::Forms::TreeNode].new(\r\n" +
					"            [treeNode1]))\r\n" +
					"        @treeView1.Size = System::Drawing::Size.new(100, 100)\r\n" +
					"        @treeView1.TabIndex = 0\r\n" +
					"        # \r\n" +
					"        # MainForm\r\n" +
					"        # \r\n" +
					"        self.ClientSize = System::Drawing::Size.new(200, 300)\r\n" +
					"        self.Controls.Add(@treeView1)\r\n" +
					"        self.Name = \"MainForm\"\r\n" +
					"        self.ResumeLayout(false)\r\n" +
					"        self.PerformLayout()\r\n" +
					"    end\r\n" +
					"end";
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
