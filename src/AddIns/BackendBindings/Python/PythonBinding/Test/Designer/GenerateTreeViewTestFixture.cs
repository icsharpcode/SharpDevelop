// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.ComponentModel.Design.Serialization;
using System.Drawing;
using System.Windows.Forms;

using ICSharpCode.PythonBinding;
using ICSharpCode.Scripting.Tests.Utils;
using NUnit.Framework;
using PythonBinding.Tests.Utils;

namespace PythonBinding.Tests.Designer
{
	[TestFixture]
	public class GenerateTreeViewTestFixture
	{
		string generatedPythonCode;
		
		[TestFixtureSetUp]
		public void SetUpFixture()
		{
			using (DesignSurface designSurface = new DesignSurface(typeof(Form))) {
				IDesignerHost host = (IDesignerHost)designSurface.GetService(typeof(IDesignerHost));
				IEventBindingService eventBindingService = new MockEventBindingService(host);
				Form form = (Form)host.RootComponent;
				form.ClientSize = new Size(200, 300);

				PropertyDescriptorCollection descriptors = TypeDescriptor.GetProperties(form);
				PropertyDescriptor namePropertyDescriptor = descriptors.Find("Name", false);
				namePropertyDescriptor.SetValue(form, "MainForm");
				
				// Add tree view.
				TreeView treeView = (TreeView)host.CreateComponent(typeof(TreeView), "treeView1");
				treeView.LineColor = Color.Black;
				treeView.Location = new Point(0, 0);
				treeView.Size = new Size(100, 100);
				
				DesignerSerializationManager designerSerializationManager = new DesignerSerializationManager(host);
				IDesignerSerializationManager serializationManager = (IDesignerSerializationManager)designerSerializationManager;
				using (designerSerializationManager.CreateSession()) {

					// Add first root node.
					TreeNode firstRootNode = (TreeNode)serializationManager.CreateInstance(typeof(TreeNode), new object[0], "treeNode3", false);
					firstRootNode.Name = "RootNode0";
					firstRootNode.Text = "RootNode0.Text";
					treeView.Nodes.Add(firstRootNode);
					
					// Add first child node.
					TreeNode firstChildNode = (TreeNode)serializationManager.CreateInstance(typeof(TreeNode), new object[0], "treeNode2", false);
					firstChildNode.Name = "ChildNode0";
					firstChildNode.Text = "ChildNode0.Text";
					firstRootNode.Nodes.Add(firstChildNode);
					
					// Add second child node.
					TreeNode secondChildNode = (TreeNode)serializationManager.CreateInstance(typeof(TreeNode), new object[0], "treeNode1", false);
					secondChildNode.Name = "ChildNode1";
					secondChildNode.Text = "ChildNode1.Text";
					firstChildNode.Nodes.Add(secondChildNode);
					
					form.Controls.Add(treeView);
					
					PythonCodeDomSerializer serializer = new PythonCodeDomSerializer("    ");
					generatedPythonCode = serializer.GenerateInitializeComponentMethodBody(host, designerSerializationManager, String.Empty, 1);
				}
			}
		}
		
		[Test]
		public void GeneratedCode()
		{
			string expectedCode = "    treeNode1 = System.Windows.Forms.TreeNode(\"ChildNode1.Text\")\r\n" +
								"    treeNode2 = System.Windows.Forms.TreeNode(\"ChildNode0.Text\", System.Array[System.Windows.Forms.TreeNode](\r\n" +
								"        [treeNode1]))\r\n" +
								"    treeNode3 = System.Windows.Forms.TreeNode(\"RootNode0.Text\", System.Array[System.Windows.Forms.TreeNode](\r\n" +
								"        [treeNode2]))\r\n" +
								"    self._treeView1 = System.Windows.Forms.TreeView()\r\n" +
								"    self.SuspendLayout()\r\n" +
								"    # \r\n" +
								"    # treeView1\r\n" +
								"    # \r\n" +
								"    self._treeView1.Location = System.Drawing.Point(0, 0)\r\n" +
								"    self._treeView1.Name = \"treeView1\"\r\n" +
								"    treeNode1.Name = \"ChildNode1\"\r\n" +
								"    treeNode1.Text = \"ChildNode1.Text\"\r\n" +
								"    treeNode2.Name = \"ChildNode0\"\r\n" +
								"    treeNode2.Text = \"ChildNode0.Text\"\r\n" +
								"    treeNode3.Name = \"RootNode0\"\r\n" +
								"    treeNode3.Text = \"RootNode0.Text\"\r\n" +
								"    self._treeView1.Nodes.AddRange(System.Array[System.Windows.Forms.TreeNode](\r\n" +
								"        [treeNode3]))\r\n" +
								"    self._treeView1.Size = System.Drawing.Size(100, 100)\r\n" +
								"    self._treeView1.TabIndex = 0\r\n" +
								"    # \r\n" +
								"    # MainForm\r\n" +
								"    # \r\n" +
								"    self.ClientSize = System.Drawing.Size(200, 300)\r\n" +
								"    self.Controls.Add(self._treeView1)\r\n" +
								"    self.Name = \"MainForm\"\r\n" +
								"    self.ResumeLayout(False)\r\n";
			
			Assert.AreEqual(expectedCode, generatedPythonCode, generatedPythonCode);
		}		
	}
}
