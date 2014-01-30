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

using ICSharpCode.PythonBinding;
using NUnit.Framework;
using PythonBinding.Tests.Utils;

namespace PythonBinding.Tests.Designer
{
	[TestFixture]
	public class LoadTextBoxOnPanelTestFixture : LoadFormTestFixtureBase
	{		
		public override string PythonCode {
			get {
				return "class MainForm(System.Windows.Forms.Form):\r\n" +
							"    def InitializeComponent(self):\r\n" +
							"        self._panel1 = System.Windows.Forms.Panel()\r\n" +
							"        self._textBox1 = System.Windows.Forms.TextBox()\r\n" +
							"        self._panel1.SuspendLayout()\r\n" +
							"        self.SuspendLayout()\r\n" +
							"        # \r\n" +
							"        # panel1\r\n" +
							"        # \r\n" +
							"        self._panel1.Location = System.Drawing.Point(10, 15)\r\n" +
							"        self._panel1.Name = \"panel1\"\r\n" +
							"        self._panel1.Size = System.Drawing.Size(100, 120)\r\n" +
							"        self._panel1.TabIndex = 0\r\n" +
							"        self._panel1.Controls.Add(self._textBox1)\r\n" +
							"        # \r\n" +
							"        # textBox1\r\n" +
							"        # \r\n" +
							"        self._textBox1.Location = System.Drawing.Point(5, 5)\r\n" +
							"        self._textBox1.Name = \"textBox1\"\r\n" +
							"        self._textBox1.Size = System.Drawing.Size(110, 20)\r\n" +
							"        self._textBox1.TabIndex = 0\r\n" +
							"        # \r\n" +
							"        # MainForm\r\n" +
							"        # \r\n" +
							"        self.ClientSize = System.Drawing.Size(284, 264)\r\n" +
							"        self.Name = \"MainForm\"\r\n" +
							"        self.Controls.Add(self._panel1)\r\n" +
							"        self._panel1.ResumeLayout(false)\r\n" +
							"        self._panel1.PerformLayout()\r\n" +
							"        self.ResumeLayout(False)\r\n" +
							"        self.PerformLayout()\r\n";
			}
		}
		
		public Panel Panel {
			get { return Form.Controls[0] as Panel; }
		}
		
		public TextBox TextBox { 
			get { return Panel.Controls[0] as TextBox; }
		}
		
		[Test]
		public void TextBoxAddedToPanel()
		{
			Assert.IsNotNull(TextBox);
		}		
	}
}
