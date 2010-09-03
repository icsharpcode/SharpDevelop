// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

using ICSharpCode.PythonBinding;
using NUnit.Framework;
using PythonBinding.Tests.Utils;

namespace PythonBinding.Tests.Designer
{
	[TestFixture]
	public class LoadListViewFormTestFixture : LoadFormTestFixtureBase
	{		
		public override string PythonCode {
			get {
				return "class MainForm(System.Windows.Forms.Form):\r\n" +
						"    def InitializeComponent(self):\r\n" +
						"        self._listView1 = System.Windows.Forms.ListView()\r\n" +
						"        listViewItem1 = System.Windows.Forms.ListViewItem()\r\n" +
						"        listViewItem2 = System.Windows.Forms.ListViewItem()\r\n" +
						"        self.SuspendLayout()\r\n" +
						"        # \r\n" +
						"        # listView1\r\n" +
						"        # \r\n" +
						"        self._listView1.Items.AddRange(System.Array[System.Windows.Forms.ListViewItem](\r\n" +
						"            [listViewItem1,\r\n" +
						"            listViewItem2]))\r\n" +
						"        self._listView1.Location = System.Drawing.Point(0, 0)\r\n" +
						"        self._listView1.Name = \"listView1\"\r\n" +
						"        self._listView1.Size = System.Drawing.Size(204, 104)\r\n" +
						"        self._listView1.TabIndex = 0\r\n" +
						"        listViewItem1.ToolTipText = \"tooltip1\"\r\n" +
						"        listViewItem2.ToolTipText = \"tooltip2\"\r\n" +
						"        # \r\n" +
						"        # MainForm\r\n" +
						"        # \r\n" +
						"        self.ClientSize = System.Drawing.Size(200, 300)\r\n" +
						"        self.Controls.Add(self._listView1)\r\n" +
						"        self.Name = \"MainForm\"\r\n" +
						"        self.ResumeLayout(False)\r\n" +
						"        self.PerformLayout()\r\n";
			}
		}
		
		public ListView ListView {
			get { return Form.Controls[0] as ListView; }
		}
		
		public ListViewItem ListViewItem1 {
			get { return ListView.Items[0]; }
		}
		
		public ListViewItem ListViewItem2 {
			get { return ListView.Items[1]; }
		}
		
		[Test]
		public void ListViewAddedToForm()
		{
			Assert.IsNotNull(ListView);
		}
		
		[Test]
		public void ListViewHasTwoItems()
		{
			Assert.AreEqual(2, ListView.Items.Count);
		}
		
		[Test]
		public void ListViewItem1TooltipText()
		{
			Assert.AreEqual("tooltip1", ListViewItem1.ToolTipText);
		}
	}
}
