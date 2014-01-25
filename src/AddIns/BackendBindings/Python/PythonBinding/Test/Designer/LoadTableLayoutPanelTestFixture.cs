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
	public class LoadTableLayoutPanelTestFixture : LoadFormTestFixtureBase
	{		
		public override string PythonCode {
			get {
				return "class MainForm(System.Windows.Forms.Form):\r\n" +
							"    def InitializeComponent(self):\r\n" +
							"        self._button1 = System.Windows.Forms.Button()\r\n" +
							"        self._checkBox1 = System.Windows.Forms.CheckBox()\r\n" +
							"        self._tableLayoutPanel1 = System.Windows.Forms.TableLayoutPanel()\r\n" +
							"        self._tableLayoutPanel1.SuspendLayout()\r\n" +
							"        self.SuspendLayout()\r\n" +
							"        # \r\n" +
							"        # button1\r\n" +
							"        # \r\n" +
							"        self._button1.Location = System.Drawing.Point(3, 3)\r\n" +
							"        self._button1.Name = \"button1\"\r\n" +
							"        self._button1.Size = System.Drawing.Size(75, 23)\r\n" +
							"        self._button1.TabIndex = 0\r\n" +
							"        self._button1.Text = \"button1\"\r\n" +
							"        self._button1.UseVisualStyleBackColor = True\r\n" +
							"        # \r\n" +
							"        # checkBox1\r\n" +
							"        # \r\n" +
							"        self._checkBox1.Location = System.Drawing.Point(103, 3)\r\n" +
							"        self._checkBox1.Name = \"checkBox1\"\r\n" +
							"        self._checkBox1.Size = System.Drawing.Size(94, 24)\r\n" +
							"        self._checkBox1.TabIndex = 1\r\n" +
							"        self._checkBox1.Text = \"checkBox1\"\r\n" +
							"        self._checkBox1.UseVisualStyleBackColor = True\r\n" +
							"        # \r\n" +
							"        # tableLayoutPanel1\r\n" +
							"        # \r\n" +
							"        self._tableLayoutPanel1.ColumnCount = 2\r\n" +
							"        self._tableLayoutPanel1.Controls.Add(self._button1)\r\n" +
							"        self._tableLayoutPanel1.Controls.Add(self._checkBox1)\r\n" +
							"        self._tableLayoutPanel1.Location = System.Drawing.Point(89, 36)\r\n" +
							"        self._tableLayoutPanel1.Name = \"tableLayoutPanel1\"\r\n" +
							"        self._tableLayoutPanel1.RowCount = 2\r\n" +
							"        self._tableLayoutPanel1.Size = System.Drawing.Size(200, 100)\r\n" +
							"        self._tableLayoutPanel1.TabIndex = 0\r\n" +
							"        # \r\n" +
							"        # MainForm\r\n" +
							"        # \r\n" +
							"        self.ClientSize = System.Drawing.Size(284, 264)\r\n" +
							"        self.Controls.Add(self._tableLayoutPanel1)\r\n" +
							"        self.Name = \"MainForm\"\r\n" +
							"        self.ResumeLayout(False)\r\n" +
							"        self.PerformLayout()\r\n";
			}
		}
		
		TableLayoutPanel TableLayoutPanel {
			get { return Form.Controls[0] as TableLayoutPanel; }
		}
		
		[Test]
		public void ButtonAddedToTableLayout()
		{
			Assert.IsInstanceOf(typeof(Button), TableLayoutPanel.Controls[0]);
		}
		
		[Test]
		public void CheckBoxAddedToTableLayout()
		{
			Assert.IsInstanceOf(typeof(CheckBox), TableLayoutPanel.Controls[1]);
		}
	}
}
