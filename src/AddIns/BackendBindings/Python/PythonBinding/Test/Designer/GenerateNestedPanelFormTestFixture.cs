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
using System.ComponentModel;
using System.ComponentModel.Design;
using System.ComponentModel.Design.Serialization;
using System.Drawing;
using System.Windows.Forms;
using ICSharpCode.PythonBinding;
using NUnit.Framework;
using PythonBinding.Tests.Utils;

namespace PythonBinding.Tests.Designer
{
	/// <summary>
	/// Ensures that SuspendLayout and ResumeLayout methods are generated for a panel containing controls and sitting on top of 
	/// another panel.
	/// </summary>
	[TestFixture]
	public class GenerateNestedPanelFormTestFixture
	{
		string generatedPythonCode;
		
		[TestFixtureSetUp]
		public void SetUpFixture()
		{
			using (DesignSurface designSurface = new DesignSurface(typeof(Form))) {
				IDesignerHost host = (IDesignerHost)designSurface.GetService(typeof(IDesignerHost));
				Form form = (Form)host.RootComponent;			
				form.ClientSize = new Size(284, 264);
				
				PropertyDescriptorCollection descriptors = TypeDescriptor.GetProperties(form);
				PropertyDescriptor namePropertyDescriptor = descriptors.Find("Name", false);
				namePropertyDescriptor.SetValue(form, "MainForm");
				
				Panel panel1 = (Panel)host.CreateComponent(typeof(Panel), "panel1");
				panel1.Location = new Point(0, 0);
				panel1.TabIndex = 0;
				panel1.Size = new Size(200, 220);
				
				Panel panel2 = (Panel)host.CreateComponent(typeof(Panel), "panel2");
				panel2.Location = new Point(10, 15);
				panel2.TabIndex = 0;
				panel2.Size = new Size(100, 120);
				TextBox textBox = (TextBox)host.CreateComponent(typeof(TextBox), "textBox1");
				textBox.Location = new Point(5, 5);
				textBox.TabIndex = 0;
				textBox.Size = new Size(110, 20);
				panel2.Controls.Add(textBox);
				
				panel1.Controls.Add(panel2);
				form.Controls.Add(panel1);
				
				DesignerSerializationManager serializationManager = new DesignerSerializationManager(host);
				using (serializationManager.CreateSession()) {					
					PythonCodeDomSerializer serializer = new PythonCodeDomSerializer("    ");
					generatedPythonCode = serializer.GenerateInitializeComponentMethodBody(host, serializationManager, String.Empty, 1);
				}
			}
		}
		
		[Test]
		public void GeneratedCode()
		{
			string expectedCode = "    self._panel1 = System.Windows.Forms.Panel()\r\n" +
								"    self._panel2 = System.Windows.Forms.Panel()\r\n" +
								"    self._textBox1 = System.Windows.Forms.TextBox()\r\n" +
								"    self._panel1.SuspendLayout()\r\n" +
								"    self._panel2.SuspendLayout()\r\n" +
								"    self.SuspendLayout()\r\n" +
								"    # \r\n" +
								"    # panel1\r\n" +
								"    # \r\n" +
								"    self._panel1.Controls.Add(self._panel2)\r\n" +
								"    self._panel1.Location = System.Drawing.Point(0, 0)\r\n" +
								"    self._panel1.Name = \"panel1\"\r\n" +
								"    self._panel1.Size = System.Drawing.Size(200, 220)\r\n" +
								"    self._panel1.TabIndex = 0\r\n" +
								"    # \r\n" +
								"    # panel2\r\n" +
								"    # \r\n" +
								"    self._panel2.Controls.Add(self._textBox1)\r\n" +
								"    self._panel2.Location = System.Drawing.Point(10, 15)\r\n" +
								"    self._panel2.Name = \"panel2\"\r\n" +
								"    self._panel2.Size = System.Drawing.Size(100, 120)\r\n" +
								"    self._panel2.TabIndex = 0\r\n" +
								"    # \r\n" +
								"    # textBox1\r\n" +
								"    # \r\n" +
								"    self._textBox1.Location = System.Drawing.Point(5, 5)\r\n" +
								"    self._textBox1.Name = \"textBox1\"\r\n" +
								"    self._textBox1.Size = System.Drawing.Size(110, 20)\r\n" +
								"    self._textBox1.TabIndex = 0\r\n" +
								"    # \r\n" +
								"    # MainForm\r\n" +
								"    # \r\n" +
								"    self.ClientSize = System.Drawing.Size(284, 264)\r\n" +
								"    self.Controls.Add(self._panel1)\r\n" +
								"    self.Name = \"MainForm\"\r\n" +
								"    self._panel1.ResumeLayout(False)\r\n" +
								"    self._panel2.ResumeLayout(False)\r\n" +
								"    self._panel2.PerformLayout()\r\n" +
								"    self.ResumeLayout(False)\r\n";
			
			Assert.AreEqual(expectedCode, generatedPythonCode, generatedPythonCode);
		}
	}
}
