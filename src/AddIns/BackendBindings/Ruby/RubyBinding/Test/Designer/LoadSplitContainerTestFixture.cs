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
