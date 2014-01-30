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
using IronPython.Compiler.Ast;
using NUnit.Framework;
using PythonBinding.Tests.Utils;

namespace PythonBinding.Tests.Designer
{
	class PublicPanelBaseForm : Form
	{
		public Panel panel1 = new Panel();
		Button button1 = new Button();
		
		public PublicPanelBaseForm()
		{
			button1.Name = "button1";

			panel1.Name = "panel1";
			panel1.Location = new Point(5, 10);
			panel1.Size = new Size(200, 100);
			panel1.Controls.Add(button1);

			Controls.Add(panel1);
		}
	}
	
	class PublicPanelDerivedForm : PublicPanelBaseForm 
	{
	}
	
	[TestFixture]
	public class LoadInheritedPublicPanelFormTestFixture : LoadFormTestFixtureBase
	{		
		public override string PythonCode {
			get {
				base.ComponentCreator.AddType("PythonBinding.Tests.Designer.PublicPanelDerivedForm", typeof(PublicPanelDerivedForm));
				
				return "class MainForm(PythonBinding.Tests.Designer.PublicPanelDerivedForm):\r\n" +
							"    def InitializeComponent(self):\r\n" +
							"        self.SuspendLayout()\r\n" +
							"        # \r\n" +
							"        # panel1\r\n" +
							"        # \r\n" +
							"        self.panel1.Location = System.Drawing.Point(10, 15)\r\n" +
							"        self.panel1.Size = System.Drawing.Size(108, 120)\r\n" +
							"        # \r\n" +
							"        # form1\r\n" +
							"        # \r\n" +
							"        self.Location = System.Drawing.Point(10, 20)\r\n" +
							"        self.Name = \"form1\"\r\n" +
							"        self.Controls.Add(self._textBox1)\r\n" +
							"        self.ResumeLayout(False)\r\n";
			}
		}
		
		PublicPanelDerivedForm DerivedForm { 
			get { return Form as PublicPanelDerivedForm; }
		}
				
		[Test]
		public void PanelLocation()
		{
			Assert.AreEqual(new Point(10, 15), DerivedForm.panel1.Location);
		}

		[Test]
		public void PanelSize()
		{
			Assert.AreEqual(new Size(108, 120), DerivedForm.panel1.Size);
		}
		
		[Test]
		public void GetPublicPanelObject()
		{
			Assert.AreEqual(DerivedForm.panel1, PythonControlFieldExpression.GetInheritedObject("panel1", DerivedForm));
		}		
	}
}
