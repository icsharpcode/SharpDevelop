// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

using ICSharpCode.RubyBinding;
using NUnit.Framework;
using RubyBinding.Tests.Utils;

namespace RubyBinding.Tests.Designer
{
	[TestFixture]
	public class LoadMenuStripFormTestFixture : LoadFormTestFixtureBase
	{		
		public override string RubyCode {
			get {
				return
					"class MainForm < System::Windows::Forms::Form\r\n" +
						"    def InitializeComponent()\r\n" +
						"        @menuStrip1 = System::Windows::Forms::MenuStrip.new()\r\n" +
						"        @fileToolStripMenuItem = System::Windows::Forms::ToolStripMenuItem.new()\r\n" +
						"        @openToolStripMenuItem = System::Windows::Forms::ToolStripMenuItem.new()\r\n" +
						"        @exitToolStripMenuItem = System::Windows::Forms::ToolStripMenuItem.new()\r\n" +
						"        @editToolStripMenuItem = System::Windows::Forms::ToolStripMenuItem.new()\r\n" +
						"        @menuStrip1.SuspendLayout()\r\n" +
						"        self.SuspendLayout()\r\n" +
						"        # \r\n" +
						"        # menuStrip1\r\n" +
						"        # \r\n" +
						"        @menuStrip1.Location = System::Drawing::Point.new(0, 0)\r\n" +
						"        @menuStrip1.Name = \"menuStrip1\"\r\n" +
						"        @menuStrip1.Size = System::Drawing::Size.new(200, 24)\r\n" +
						"        @menuStrip1.TabIndex = 0\r\n" +
						"        @menuStrip1.Text = \"menuStrip1\"\r\n" +
						"        @menuStrip1.Items.AddRange(System::Array[System::Windows::Forms::ToolStripItem].new(\r\n" +
						"            [@fileToolStripMenuItem,\r\n" +
						"            @editToolStripMenuItem]))\r\n" +
						"        # \r\n" +
						"        # fileToolStripMenuItem\r\n" +
						"        # \r\n" +
						"        @fileToolStripMenuItem.Name = \"fileToolStripMenuItem\"\r\n" +
						"        @fileToolStripMenuItem.Size = System::Drawing::Size.new(37, 20)\r\n" +
						"        @fileToolStripMenuItem.Text = \"&File\"\r\n" +
						"        @fileToolStripMenuItem.DropDownItems.AddRange(System.Array[System::Windows::Forms::ToolStripItem].new(\r\n" +
						"            [@openToolStripMenuItem,\r\n" +
						"            @exitToolStripMenuItem]))\r\n" +
						"        # \r\n" +
						"        # openToolStripMenuItem\r\n" +
						"        # \r\n" +
						"        @openToolStripMenuItem.Name = \"openToolStripMenuItem\"\r\n" +
						"        @openToolStripMenuItem.Size = System::Drawing::Size.new(37, 20)\r\n" +
						"        @openToolStripMenuItem.Text = \"&Open\"\r\n" +
						"        # \r\n" +
						"        # exitToolStripMenuItem\r\n" +
						"        # \r\n" +
						"        @exitToolStripMenuItem.Name = \"exitToolStripMenuItem\"\r\n" +
						"        @exitToolStripMenuItem.Size = System::Drawing::Size.new(37, 20)\r\n" +
						"        @exitToolStripMenuItem.Text = \"E&xit\"\r\n" +
						"        # \r\n" +
						"        # editToolStripMenuItem\r\n" +
						"        # \r\n" +
						"        @editToolStripMenuItem.Name = \"editToolStripMenuItem\"\r\n" +
						"        @editToolStripMenuItem.Size = System::Drawing::Size.new(39, 20)\r\n" +
						"        @editToolStripMenuItem.Text = \"&Edit\"\r\n" +				
						"        # \r\n" +
						"        # MainForm\r\n" +
						"        # \r\n" +
						"        self.ClientSize = System::Drawing::Size.new(200, 300)\r\n" +
						"        self.Name = \"MainForm\"\r\n" +
						"        self.Controls.Add(@menuStrip1)\r\n" +
						"        @menuStrip1.ResumeLayout(false)\r\n" +
						"        @menuStrip1.PerformLayout()\r\n" +
						"        self.ResumeLayout(False)\r\n" +
						"        self.PerformLayout()\r\n" +
						"    end\r\n" +
						"end";
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
