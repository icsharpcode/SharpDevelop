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
	/// Tests that a form's MinimumSize, AutoScrollMinSize and AutoScrollMargin properties are generated
	/// in the InitializeComponent method.
	/// </summary>
	[TestFixture]
	public class GenerateMinSizeFormTestFixture
	{
		string generatedPythonCode;
		
		[TestFixtureSetUp]
		public void SetUpFixture()
		{
			using (Form form = new Form()) {
				form.Name = "MainForm";
				form.ClientSize = new Size(284, 264);
				form.MinimumSize = new Size(100, 200);
				form.AutoScrollMinSize = new Size(10, 20);
				form.AutoScrollMargin = new Size(11, 22);
				form.AutoScroll = false;
				
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
								"    # MainForm\r\n" +
								"    # \r\n" +
								"    self.ClientSize = System.Drawing.Size(284, 264)\r\n" +
								"    self.MinimumSize = System.Drawing.Size(100, 200)\r\n" +
								"    self.AutoScrollMargin = System.Drawing.Size(11, 22)\r\n" +
								"    self.AutoScrollMinSize = System.Drawing.Size(10, 20)\r\n" +
								"    self.Name = \"MainForm\"\r\n" +
								"    self.Visible = False\r\n" +
								"    self.ResumeLayout(False)\r\n" +
								"    self.PerformLayout()\r\n";
			
			Assert.AreEqual(expectedCode, generatedPythonCode);
		}
	}
}
