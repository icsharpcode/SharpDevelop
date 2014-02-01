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
using System.Drawing;
using System.IO;
using System.Windows.Forms;

using ICSharpCode.PythonBinding;
using NUnit.Framework;
using PythonBinding.Tests.Utils;

namespace PythonBinding.Tests.Designer
{
	[TestFixture]
	public class LoadSplitContainerTestFixture : LoadFormTestFixtureBase
	{		
		public override string PythonCode {
			get {
				return "class MainForm(System.Windows.Forms.Form):\r\n" +
							"    def InitializeComponent(self):\r\n" +
							"        self._splitContainer1 = System.Windows.Forms.SplitContainer()\r\n" +
							"        self._treeView1 = System.Windows.Forms.TreeView()\r\n" +
							"        self._propertyGrid1 = System.Windows.Forms.PropertyGrid()\r\n" +
							"        self.SuspendLayout()\r\n" +
							"        # \r\n" +
							"        # splitContainer1\r\n" +
							"        # \r\n" +
							"        self._splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill\r\n" +
							"        self._splitContainer1.Location = System.Drawing.Point(0, 0)\r\n" +
							"        self._splitContainer1.Name = \"splitContainer1\"\r\n" +
							"        self._splitContainer1.Panel1.Controls.Add(self._treeView1)\r\n" +
							"        self._splitContainer1.Panel2.Controls.Add(self._propertyGrid1)\r\n" +
							"        self._splitContainer1.Size = System.Drawing.Size(284, 264)\r\n" +
							"        self._splitContainer1.SplitterDistance = 94\r\n" +
							"        self._splitContainer1.TabIndex = 0\r\n" +
							"        # \r\n" +
							"        # treeView1\r\n" +
							"        # \r\n" +
							"        self._treeView1.Dock = System.Windows.Forms.DockStyle.Fill\r\n" +
							"        self._treeView1.Location = System.Drawing.Point(0, 0)\r\n" +
							"        self._treeView1.Name = \"treeView1\"\r\n" +
							"        self._treeView1.Size = System.Drawing.Size(94, 264)\r\n" +
							"        self._treeView1.TabIndex = 0\r\n" +
							"        # \r\n" +
							"        # propertyGrid1\r\n" +
							"        # \r\n" +
							"        self._propertyGrid1.Dock = System.Windows.Forms.DockStyle.Fill\r\n" +
							"        self._propertyGrid1.Location = System.Drawing.Point(0, 0)\r\n" +
							"        self._propertyGrid1.Name = \"propertyGrid1\"\r\n" +
							"        self._propertyGrid1.Size = System.Drawing.Size(186, 264)\r\n" +
							"        self._propertyGrid1.TabIndex = 0\r\n" +
							"        # \r\n" +
							"        # MainForm\r\n" +
							"        # \r\n" +
							"        self.ClientSize = System.Drawing.Size(284, 264)\r\n" +
							"        self.Controls.Add(self._splitContainer1)\r\n" +
							"        self.Name = \"MainForm\"\r\n" +
							"        self.ResumeLayout(False)\r\n" +
							"        self.PerformLayout()\r\n";
			}
		}
		
		SplitContainer SplitContainer {
			get { return Form.Controls[0] as SplitContainer; }
		}
		
		[Test]
		public void TreeViewAddedToSplitContainer()
		{
			Assert.IsInstanceOf(typeof(TreeView), SplitContainer.Panel1.Controls[0]);
		}
		
		[Test]
		public void PropertyGridAddedToSplitContainer()
		{
			Assert.IsInstanceOf(typeof(PropertyGrid), SplitContainer.Panel2.Controls[0]);
		}
		
	}
}
