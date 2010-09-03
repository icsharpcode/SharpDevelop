// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

using ICSharpCode.RubyBinding;
using NUnit.Framework;
using RubyBinding.Tests.Utils;

namespace RubyBinding.Tests.Designer
{
	[TestFixture]
	public class LoadSplitContainerTestFixture : LoadFormTestFixtureBase
	{		
		public override string RubyCode {
			get {
				return
					"class MainForm < System::Windows::Forms::Form\r\n" +
					"    def InitializeComponent()\r\n" +
					"        @splitContainer1 = System::Windows::Forms::SplitContainer.new()\r\n" +
					"        @treeView1 = System::Windows::Forms::TreeView.new()\r\n" +
					"        @propertyGrid1 = System::Windows::Forms::PropertyGrid.new()\r\n" +
					"        self.SuspendLayout()\r\n" +
					"        # \r\n" +
					"        # splitContainer1\r\n" +
					"        # \r\n" +
					"        @splitContainer1.Dock = System::Windows::Forms::DockStyle.Fill\r\n" +
					"        @splitContainer1.Location = System::Drawing::Point.new(0, 0)\r\n" +
					"        @splitContainer1.Name = \"splitContainer1\"\r\n" +
					"        @splitContainer1.Panel1.Controls.Add(@treeView1)\r\n" +
					"        @splitContainer1.Panel2.Controls.Add(@propertyGrid1)\r\n" +
					"        @splitContainer1.Size = System::Drawing::Size.new(284, 264)\r\n" +
					"        @splitContainer1.SplitterDistance = 94\r\n" +
					"        @splitContainer1.TabIndex = 0\r\n" +
					"        # \r\n" +
					"        # treeView1\r\n" +
					"        # \r\n" +
					"        @treeView1.Dock = System::Windows::Forms::DockStyle.Fill\r\n" +
					"        @treeView1.Location = System::Drawing::Point.new(0, 0)\r\n" +
					"        @treeView1.Name = \"treeView1\"\r\n" +
					"        @treeView1.Size = System::Drawing::Size.new(94, 264)\r\n" +
					"        @treeView1.TabIndex = 0\r\n" +
					"        # \r\n" +
					"        # propertyGrid1\r\n" +
					"        # \r\n" +
					"        @propertyGrid1.Dock = System::Windows::Forms::DockStyle.Fill\r\n" +
					"        @propertyGrid1.Location = System::Drawing::Point.new(0, 0)\r\n" +
					"        @propertyGrid1.Name = \"propertyGrid1\"\r\n" +
					"        @propertyGrid1.Size = System::Drawing::Size.new(186, 264)\r\n" +
					"        @propertyGrid1.TabIndex = 0\r\n" +
					"        # \r\n" +
					"        # MainForm\r\n" +
					"        # \r\n" +
					"        self.ClientSize = System::Drawing::Size.new(284, 264)\r\n" +
					"        self.Controls.Add(@splitContainer1)\r\n" +
					"        self.Name = \"MainForm\"\r\n" +
					"        self.ResumeLayout(false)\r\n" +
					"        self.PerformLayout()\r\n" +
					"    end\r\n" +
					"end";
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
