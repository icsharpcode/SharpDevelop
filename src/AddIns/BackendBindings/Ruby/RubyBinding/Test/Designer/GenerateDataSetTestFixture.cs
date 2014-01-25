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
using System.ComponentModel.Design;
using System.ComponentModel.Design.Serialization;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

using ICSharpCode.RubyBinding;
using ICSharpCode.Scripting.Tests.Utils;
using NUnit.Framework;
using RubyBinding.Tests.Utils;

namespace RubyBinding.Tests.Designer
{
	[TestFixture]
	public class GenerateDataSetTestFixture
	{
		string generatedRubyCode;
		
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
					RubyCodeDomSerializer serializer = new RubyCodeDomSerializer("    ");
					generatedRubyCode = serializer.GenerateInitializeComponentMethodBody(host, serializationManager, 1);
				}
			}
		}
		
		[Test]
		public void GeneratedCode()
		{
			string expectedCode = "    @dataGridView1 = System::Windows::Forms::DataGridView.new()\r\n" +
								"    @dataSet1 = System::Data::DataSet.new()\r\n" +
								"    self.SuspendLayout()\r\n" +
								"    # \r\n" +
								"    # dataGridView1\r\n" +
								"    # \r\n" +
								"    @dataGridView1.AutoGenerateColumns = false\r\n" +
								"    @dataGridView1.DataSource = @dataSet1\r\n" +
								"    @dataGridView1.Location = System::Drawing::Point.new(0, 0)\r\n" +
								"    @dataGridView1.Name = \"dataGridView1\"\r\n" +
								"    @dataGridView1.Size = System::Drawing::Size.new(100, 100)\r\n" +
								"    @dataGridView1.TabIndex = 0\r\n" +
								"    # \r\n" +
								"    # dataSet1\r\n" +
								"    # \r\n" +
								"    @dataSet1.DataSetName = \"NewDataSet\"\r\n" +
								"    # \r\n" +
								"    # MainForm\r\n" +
								"    # \r\n" +
								"    self.ClientSize = System::Drawing::Size.new(200, 300)\r\n" +
								"    self.Controls.Add(@dataGridView1)\r\n" +
								"    self.Name = \"MainForm\"\r\n" +
								"    self.ResumeLayout(false)\r\n";
			
			Assert.AreEqual(expectedCode, generatedRubyCode, generatedRubyCode);
		}
	}
}
