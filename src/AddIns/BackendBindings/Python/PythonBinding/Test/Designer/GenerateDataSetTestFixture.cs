// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.ComponentModel.Design.Serialization;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

using ICSharpCode.PythonBinding;
using ICSharpCode.Scripting.Tests.Utils;
using NUnit.Framework;
using PythonBinding.Tests.Utils;

namespace PythonBinding.Tests.Designer
{
	[TestFixture]
	public class GenerateDataSetTestFixture
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

				DataGridView dataGridView = (DataGridView)host.CreateComponent(typeof(DataGridView), "dataGridView1");
				dataGridView.Location = new Point(0, 0);
				dataGridView.Size = new Size(100, 100);
				form.Controls.Add(dataGridView);

				DataSet dataSet = (DataSet)host.CreateComponent(typeof(DataSet), "dataSet1");
				dataGridView.DataSource = dataSet;
				
				PropertyDescriptorCollection descriptors = TypeDescriptor.GetProperties(form);
				PropertyDescriptor namePropertyDescriptor = descriptors.Find("Name", false);
				namePropertyDescriptor.SetValue(form, "MainForm");
				
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
			string expectedCode = "    self._dataGridView1 = System.Windows.Forms.DataGridView()\r\n" +
								"    self._dataSet1 = System.Data.DataSet()\r\n" +
								"    self.SuspendLayout()\r\n" +
								"    # \r\n" +
								"    # dataGridView1\r\n" +
								"    # \r\n" +
								"    self._dataGridView1.AutoGenerateColumns = False\r\n" +
								"    self._dataGridView1.DataSource = self._dataSet1\r\n" +
								"    self._dataGridView1.Location = System.Drawing.Point(0, 0)\r\n" +
								"    self._dataGridView1.Name = \"dataGridView1\"\r\n" +
								"    self._dataGridView1.Size = System.Drawing.Size(100, 100)\r\n" +
								"    self._dataGridView1.TabIndex = 0\r\n" +
								"    # \r\n" +
								"    # dataSet1\r\n" +
								"    # \r\n" +
								"    self._dataSet1.DataSetName = \"NewDataSet\"\r\n" +
								"    # \r\n" +
								"    # MainForm\r\n" +
								"    # \r\n" +
								"    self.ClientSize = System.Drawing.Size(200, 300)\r\n" +
								"    self.Controls.Add(self._dataGridView1)\r\n" +
								"    self.Name = \"MainForm\"\r\n" +
								"    self.ResumeLayout(False)\r\n";
			
			Assert.AreEqual(expectedCode, generatedPythonCode, generatedPythonCode);
		}
	}
}
