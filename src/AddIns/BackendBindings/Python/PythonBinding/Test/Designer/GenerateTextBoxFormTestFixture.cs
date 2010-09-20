// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
	[TestFixture]
	public class GenerateTextBoxFormTestFixture
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
				
				TextBox textBox = (TextBox)host.CreateComponent(typeof(TextBox), "textBox1");
				textBox.Size = new Size(110, 20);
				textBox.TabIndex = 1;
				textBox.Location = new Point(10, 10);
				
				form.Controls.Add(textBox);
				
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
			string expectedCode = "    self._textBox1 = System.Windows.Forms.TextBox()\r\n" +
								"    self.SuspendLayout()\r\n" +
								"    # \r\n" +
								"    # textBox1\r\n" +
								"    # \r\n" +
								"    self._textBox1.Location = System.Drawing.Point(10, 10)\r\n" +
								"    self._textBox1.Name = \"textBox1\"\r\n" +
								"    self._textBox1.Size = System.Drawing.Size(110, 20)\r\n" +
								"    self._textBox1.TabIndex = 1\r\n" +
								"    # \r\n" +
								"    # MainForm\r\n" +
								"    # \r\n" +
								"    self.ClientSize = System.Drawing.Size(284, 264)\r\n" +
								"    self.Controls.Add(self._textBox1)\r\n" +
								"    self.Name = \"MainForm\"\r\n" +
								"    self.ResumeLayout(False)\r\n" +
								"    self.PerformLayout()\r\n";
			
			Assert.AreEqual(expectedCode, generatedPythonCode);
		}		
	}
}
