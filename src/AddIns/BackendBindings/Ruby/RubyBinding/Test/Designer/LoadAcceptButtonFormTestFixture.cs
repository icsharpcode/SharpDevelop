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
using System.ComponentModel.Design;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

using ICSharpCode.RubyBinding;
using NUnit.Framework;
using RubyBinding.Tests.Utils;

namespace RubyBinding.Tests.Designer
{
	[TestFixture]
	public class LoadAcceptButtonFormTestFixture : LoadFormTestFixtureBase
	{
		public override string RubyCode {
			get {
				return "class TestForm < System::Windows::Forms::Form\r\n" +
							"    def InitializeComponent()\r\n" +
							"        @button1 = System::Windows::Forms::Button.new()\r\n" +
							"        self.SuspendLayout()\r\n" +
							"        # \r\n" +
							"        # button1\r\n" +
							"        # \r\n" +
							"        @button1.Location = System::Drawing::Point.new(0, 0)\r\n" +
							"        @button1.Name = \"button1\"\r\n" +
							"        @button1.Size = System::Drawing::Size.new(10, 10)\r\n" +
							"        @button1.TabIndex = 0\r\n" +
							"        @button1.Text = \"button1\"\r\n" +
							"        # \r\n" +
							"        # MainForm\r\n" +
							"        # \r\n" +
							"        self.AcceptButton = @button1\r\n" +
							"        self.ClientSize = System::Drawing::Size.new(200, 300)\r\n" +
							"        self.Name = \"MainForm\"\r\n" +
							"        self.Controls.Add(@button1)\r\n" +
							"        self.ResumeLayout(false)\r\n" +
							"    end\r\n" +
							"end";
			}
		}
		
		[Test]
		public void FormHasAcceptButton()
		{
			Assert.IsNotNull(Form.AcceptButton);
		}
		
		[Test]
		public void AcceptButtonPropertyDescriptorObjectMatchesButton()
		{
			Button button = Form.Controls[0] as Button;
			PropertyDescriptor p = TypeDescriptor.GetProperties(Form).Find("AcceptButton", true);
			Assert.AreEqual(button, p.GetValue(Form));
		}
	}
}
