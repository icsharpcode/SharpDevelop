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
	public class LoadMenuStripFormTestFixture : LoadFormTestFixtureBase
	{		
		public override string PythonCode {
			get {
				return "class MainForm(System.Windows.Forms.Form):\r\n" +
						"    def InitializeComponent(self):\r\n" +
						"        self._menuStrip1 = System.Windows.Forms.MenuStrip()\r\n" +
						"        self._fileToolStripMenuItem = System.Windows.Forms.ToolStripMenuItem()\r\n" +
						"        self._openToolStripMenuItem = System.Windows.Forms.ToolStripMenuItem()\r\n" +
						"        self._exitToolStripMenuItem = System.Windows.Forms.ToolStripMenuItem()\r\n" +
						"        self._editToolStripMenuItem = System.Windows.Forms.ToolStripMenuItem()\r\n" +
						"        self._menuStrip1.SuspendLayout()\r\n" +
						"        self.SuspendLayout()\r\n" +
						"        # \r\n" +
						"        # menuStrip1\r\n" +
						"        # \r\n" +
						"        self._menuStrip1.Location = System.Drawing.Point(0, 0)\r\n" +
						"        self._menuStrip1.Name = \"menuStrip1\"\r\n" +
						"        self._menuStrip1.Size = System.Drawing.Size(200, 24)\r\n" +
						"        self._menuStrip1.TabIndex = 0\r\n" +
						"        self._menuStrip1.Text = \"menuStrip1\"\r\n" +
						"        self._menuStrip1.Items.AddRange(System.Array[System.Windows.Forms.ToolStripItem](\r\n" +
						"            [self._fileToolStripMenuItem,\r\n" +
						"            self._editToolStripMenuItem]))\r\n" +
						"        # \r\n" +
						"        # fileToolStripMenuItem\r\n" +
						"        # \r\n" +
						"        self._fileToolStripMenuItem.Name = \"fileToolStripMenuItem\"\r\n" +
						"        self._fileToolStripMenuItem.Size = System.Drawing.Size(37, 20)\r\n" +
						"        self._fileToolStripMenuItem.Text = \"&File\"\r\n" +
						"        self._fileToolStripMenuItem.DropDownItems.AddRange(System.Array[System.Windows.Forms.ToolStripItem](\r\n" +
						"            [self._openToolStripMenuItem,\r\n" +
						"            self._exitToolStripMenuItem]))\r\n" +
						"        # \r\n" +
						"        # openToolStripMenuItem\r\n" +
						"        # \r\n" +
						"        self._openToolStripMenuItem.Name = \"openToolStripMenuItem\"\r\n" +
						"        self._openToolStripMenuItem.Size = System.Drawing.Size(37, 20)\r\n" +
						"        self._openToolStripMenuItem.Text = \"&Open\"\r\n" +
						"        # \r\n" +
						"        # exitToolStripMenuItem\r\n" +
						"        # \r\n" +
						"        self._exitToolStripMenuItem.Name = \"exitToolStripMenuItem\"\r\n" +
						"        self._exitToolStripMenuItem.Size = System.Drawing.Size(37, 20)\r\n" +
						"        self._exitToolStripMenuItem.Text = \"E&xit\"\r\n" +
						"        # \r\n" +
						"        # editToolStripMenuItem\r\n" +
						"        # \r\n" +
						"        self._editToolStripMenuItem.Name = \"editToolStripMenuItem\"\r\n" +
						"        self._editToolStripMenuItem.Size = System.Drawing.Size(39, 20)\r\n" +
						"        self._editToolStripMenuItem.Text = \"&Edit\"\r\n" +				
						"        # \r\n" +
						"        # MainForm\r\n" +
						"        # \r\n" +
						"        self.ClientSize = System.Drawing.Size(200, 300)\r\n" +
						"        self.Name = \"MainForm\"\r\n" +
						"        self.Controls.Add(self._menuStrip1)\r\n" +
						"        self._menuStrip1.ResumeLayout(False)\r\n" +
						"        self._menuStrip1.PerformLayout()\r\n" +
						"        self.ResumeLayout(False)\r\n" +
						"        self.PerformLayout()\r\n";
			}
		}
		
		public MenuStrip MenuStrip {
			get { return Form.Controls[0] as MenuStrip; }
		}
		
		public ToolStripMenuItem FileMenuItem {
			get { return MenuStrip.Items[0] as ToolStripMenuItem; }
		}
		
		[Test]
		public void MenuStripAddedToForm()
		{
			Assert.IsNotNull(MenuStrip);
		}
		
		[Test]
		public void MenuStripHasTwoItems()
		{
			Assert.AreEqual(2, MenuStrip.Items.Count);
		}
		
		[Test]
		public void MenuStripFirstItemIsFileMenuItem()
		{
			Assert.AreEqual("fileToolStripMenuItem", FileMenuItem.Name);
		}
		
		[Test]
		public void FileMenuItemText()
		{
			Assert.AreEqual("&File", FileMenuItem.Text);
		}
		
		[Test]
		public void MenuStripSecondItemIsEditMenuItem()
		{
			Assert.AreEqual("editToolStripMenuItem", MenuStrip.Items[1].Name);
		}
		
		[Test]
		public void FileMenuItemHasDropDownItems()
		{
			Assert.AreEqual(2, FileMenuItem.DropDownItems.Count);
		}
	}
}
