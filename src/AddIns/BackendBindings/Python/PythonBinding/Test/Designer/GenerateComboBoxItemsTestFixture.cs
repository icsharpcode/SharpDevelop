// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.ComponentModel.Design.Serialization;
using System.Drawing;
using System.Windows.Forms;

using ICSharpCode.PythonBinding;
using ICSharpCode.Scripting.Tests.Utils;
using NUnit.Framework;
using PythonBinding.Tests.Utils;

namespace PythonBinding.Tests.Designer
{
	[TestFixture]
	public class GenerateComboBoxItemsFormTestFixture
	{
		string generatedPythonCode;
		
		[TestFixtureSetUp]
		public void SetUpFixture()
		{
			using (DesignSurface designSurface = new DesignSurface(typeof(Form))) {
				IDesignerHost host = (IDesignerHost)designSurface.GetService(typeof(IDesignerHost));
				IEventBindingService eventBindingService = new MockEventBindingService(host);
				Form form = (Form)host.RootComponent;
				form.ClientSize = new Size(200, 300);

				PropertyDescriptorCollection descriptors = TypeDescriptor.GetProperties(form);
				PropertyDescriptor namePropertyDescriptor = descriptors.Find("Name", false);
				namePropertyDescriptor.SetValue(form, "MainForm");
				
				// Add combo box.
				ComboBox comboBox = (ComboBox)host.CreateComponent(typeof(ComboBox), "comboBox1");
				comboBox.TabIndex = 0;
				comboBox.Location = new Point(0, 0);
				comboBox.Size = new System.Drawing.Size(121, 21);
				comboBox.Items.Add("aaa");
				comboBox.Items.Add("bbb");
				comboBox.Items.Add("ccc");
				
				form.Controls.Add(comboBox);
				
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
			string expectedCode = "    self._comboBox1 = System.Windows.Forms.ComboBox()\r\n" +
								"    self.SuspendLayout()\r\n" +
								"    # \r\n" +
								"    # comboBox1\r\n" +
								"    # \r\n" +
								"    self._comboBox1.Items.AddRange(System.Array[System.Object](\r\n" +
								"        [\"aaa\",\r\n" +
								"        \"bbb\",\r\n" +
								"        \"ccc\"]))\r\n" +
								"    self._comboBox1.Location = System.Drawing.Point(0, 0)\r\n" +
								"    self._comboBox1.Name = \"comboBox1\"\r\n" +
								"    self._comboBox1.Size = System.Drawing.Size(121, 21)\r\n" +
								"    self._comboBox1.TabIndex = 0\r\n" +
								"    # \r\n" +
								"    # MainForm\r\n" +
								"    # \r\n" +
								"    self.ClientSize = System.Drawing.Size(200, 300)\r\n" +
								"    self.Controls.Add(self._comboBox1)\r\n" +
 								"    self.Name = \"MainForm\"\r\n" +
								"    self.ResumeLayout(False)\r\n";
			
			Assert.AreEqual(expectedCode, generatedPythonCode, generatedPythonCode);
		}		
	}
}
