// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
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
			using (Form form = new Form()) {
				form.Name = "MainForm";
				form.ClientSize = new Size(284, 264);
				
				Panel panel1 = new Panel();
				panel1.Location = new Point(0, 0);
				panel1.Name = "panel1";
				panel1.TabIndex = 0;
				panel1.Size = new Size(200, 220);
				
				Panel panel2 = new Panel();
				panel2.Location = new Point(10, 15);
				panel2.Name = "panel2";
				panel2.TabIndex = 0;
				panel2.Size = new Size(100, 120);
				TextBox textBox = new TextBox();
				textBox.Location = new Point(5, 5);
				textBox.Name = "textBox1";
				textBox.TabIndex = 0;
				textBox.Size = new Size(110, 20);
				panel2.Controls.Add(textBox);
				
				panel1.Controls.Add(panel2);
				form.Controls.Add(panel1);
				
				string indentString = "    ";
				PythonForm pythonForm = new PythonForm(indentString);
				generatedPythonCode = pythonForm.GenerateInitializeComponentMethod(form);
			}
		}
		
		[Test]
		public void GeneratedCode()
		{
			string expectedCode = "def InitializeComponent(self):\r\n" +
								"    self._panel1 = System.Windows.Forms.Panel()\r\n" +
								"    self._panel2 = System.Windows.Forms.Panel()\r\n" +
								"    self._textBox1 = System.Windows.Forms.TextBox()\r\n" +
								"    self._panel1.SuspendLayout()\r\n" +
								"    self._panel2.SuspendLayout()\r\n" +
								"    self.SuspendLayout()\r\n" +
								"    # \r\n" +
								"    # panel1\r\n" +
								"    # \r\n" +
								"    self._panel1.Location = System.Drawing.Point(0, 0)\r\n" +
								"    self._panel1.Name = \"panel1\"\r\n" +
								"    self._panel1.Size = System.Drawing.Size(200, 220)\r\n" +
								"    self._panel1.TabIndex = 0\r\n" +
								"    self._panel1.Controls.Add(self._panel2)\r\n" +
								"    # \r\n" +
								"    # panel2\r\n" +
								"    # \r\n" +
								"    self._panel2.Location = System.Drawing.Point(10, 15)\r\n" +
								"    self._panel2.Name = \"panel2\"\r\n" +
								"    self._panel2.Size = System.Drawing.Size(100, 120)\r\n" +
								"    self._panel2.TabIndex = 0\r\n" +
								"    self._panel2.Controls.Add(self._textBox1)\r\n" +
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
								"    self.Name = \"MainForm\"\r\n" +
								"    self.Visible = False\r\n" +
								"    self.Controls.Add(self._panel1)\r\n" +
								"    self._panel1.ResumeLayout(false)\r\n" +
								"    self._panel1.PerformLayout()\r\n" +
								"    self._panel2.ResumeLayout(false)\r\n" +
								"    self._panel2.PerformLayout()\r\n" +
								"    self.ResumeLayout(False)\r\n" +
								"    self.PerformLayout()\r\n";
			
			Assert.AreEqual(expectedCode, generatedPythonCode);
		}
	}
}
