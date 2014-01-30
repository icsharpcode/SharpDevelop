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
using NUnit.Framework;
using RubyBinding.Tests.Utils;

namespace RubyBinding.Tests.Designer
{
	[TestFixture]
	public class LoadRichTextBoxWithEmptyTextTestFixture : LoadFormTestFixtureBase
	{
		public override string RubyCode {
			get {
				return
					"class MainForm < System::Windows::Forms::Form\r\n" +
					"    def initialize()\r\n" +
					"        self.InitializeComponent()\r\n" +
					"    end\r\n" +
					"\r\n" +
					"    def InitializeComponent()\r\n" +
					"        @richTextBox1 = System::Windows::Forms::RichTextBox.new()\r\n" +
					"        self.SuspendLayout()\r\n" +
					"        # \r\n" +
					"        # richTextBox1\r\n" +
					"        # \r\n" +
					"        @richTextBox1.Anchor = System::Windows::Forms::AnchorStyles.Top | System::Windows::Forms::AnchorStyles.Bottom | System::Windows::Forms::AnchorStyles.Left | System::Windows::Forms::AnchorStyles.Right\r\n" +
					"        @richTextBox1.Location = System::Drawing::Point.new(39, 31)\r\n" +
					"        @richTextBox1.Name = \"richTextBox1\"\r\n" +
					"        @richTextBox1.Size = System::Drawing::Size.new(297, 208)\r\n" +
					"        @richTextBox1.TabIndex = 0\r\n" +
					"        @richTextBox1.Text = \"\"\r\n" +
					"        # \r\n" +
					"        # MainForm\r\n" +
					"        # \r\n" +
					"        self.ClientSize = System::Drawing::Size.new(380, 301)\r\n" +
					"        self.Controls.Add(@richTextBox1)\r\n" +
					"        self.Name = \"MainForm\"\r\n" +
					"        self.ResumeLayout(false)\r\n" +
					"    end\r\n" +
					"end";
			}
		}
		
		public RichTextBox RichTextBox { 
			get { return Form.Controls[0] as RichTextBox; }
		}
		
		[Test]
		public void RichTextBoxName()
		{
			Assert.AreEqual("richTextBox1", RichTextBox.Name);
		}
		
		[Test]
		public void RichTextBoxTextIsEmpty()
		{
			Assert.AreEqual(String.Empty, RichTextBox.Text);
		}
	}
}
