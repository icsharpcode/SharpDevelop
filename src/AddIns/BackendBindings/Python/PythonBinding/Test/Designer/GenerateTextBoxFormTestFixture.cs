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
	[TestFixture]
	public class GenerateTextBoxFormTestFixture
	{
		string generatedPythonCode;
		
		[TestFixtureSetUp]
		public void SetUpFixture()
		{
			using (Form form = new Form()) {
				form.Name = "MainForm";
				form.ClientSize = new Size(284, 264);
				
				TextBox textBox = new TextBox();
				textBox.Name = "textBox1";
				textBox.Size = new Size(110, 20);
				textBox.TabIndex = 1;
				textBox.Location = new Point(10, 10);
				
				form.Controls.Add(textBox);
				
				string indentString = "    ";
				PythonForm pythonForm = new PythonForm(indentString);
				generatedPythonCode = pythonForm.GenerateInitializeComponentMethod(form);
			}
		}
		
		[Test]
		public void GeneratedCode()
		{
			string expectedCode = "def InitializeComponent(self):\r\n" +
								"    self.SuspendLayout()\r\n" +
								"    # \r\n" +
								"    # textBox1\r\n" +
								"    # \r\n" +
								"    self.textBox1.Location = System.Drawing.Point(10, 10)\r\n" +
								"    self.textBox1.Name = \"textBox1\"\r\n" +
								"    self.textBox1.Size = System.Drawing.Size(110, 20)\r\n" +
								"    self.textBox1.TabIndex = 1\r\n" +
								"    # \r\n" +
								"    # MainForm\r\n" +
								"    # \r\n" +
								"    self.ClientSize = System.Drawing.Size(284, 264)\r\n" +
								"    self.Name = \"MainForm\"\r\n" +
								"    self.Visible = False\r\n" +
								"    self.Controls.Add(self.textBox1)\r\n" +
								"    self.ResumeLayout(False)\r\n" +
								"    self.PerformLayout()\r\n";
			
			Assert.AreEqual(expectedCode, generatedPythonCode);
		}
	}
}
