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
using System.Drawing;
using System.Windows.Forms;

using ICSharpCode.PythonBinding;
using ICSharpCode.Scripting.Tests.Utils;
using NUnit.Framework;
using PythonBinding.Tests.Utils;

namespace PythonBinding.Tests.Designer
{
	[TestFixture]
	public class GenerateListViewGroupsTestFixture
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
				
				// Add list view.
				ListView listView = (ListView)host.CreateComponent(typeof(ListView), "listView1");
				listView.TabIndex = 0;
				listView.Location = new Point(0, 0);
				listView.ClientSize = new Size(200, 100);
				descriptors = TypeDescriptor.GetProperties(listView);
				PropertyDescriptor descriptor = descriptors.Find("UseCompatibleStateImageBehavior", false);
				descriptor.SetValue(listView, true);
				descriptor = descriptors.Find("View", false);
				descriptor.SetValue(listView, View.Details);
				form.Controls.Add(listView);

				DesignerSerializationManager designerSerializationManager = new DesignerSerializationManager(host);
				IDesignerSerializationManager serializationManager = (IDesignerSerializationManager)designerSerializationManager;
				using (designerSerializationManager.CreateSession()) {					
				
					// Add groups.
					ListViewGroup group1 = (ListViewGroup)serializationManager.CreateInstance(typeof(ListViewGroup), new object[0], "listViewGroup1", false);
					group1.Header = "ListViewGroup";
					group1.HeaderAlignment = HorizontalAlignment.Right;
					group1.Name = "defaultGroup";
					group1.Tag = "tag1";		
					listView.Groups.Add(group1);
					
					ListViewGroup group2 = (ListViewGroup)serializationManager.CreateInstance(typeof(ListViewGroup), new object[0], "listViewGroup2", false);
					group2.Header = "ListViewGroup";
					group2.HeaderAlignment = HorizontalAlignment.Center;
					group2.Name = "listViewGroup2";
					listView.Groups.Add(group2);
				
					PythonCodeDomSerializer serializer = new PythonCodeDomSerializer("    ");
					generatedPythonCode = serializer.GenerateInitializeComponentMethodBody(host, designerSerializationManager, String.Empty, 1);
				}
			}
		}
		
		[Test]
		public void GeneratedCode()
		{
			string expectedCode = "    listViewGroup1 = System.Windows.Forms.ListViewGroup(\"ListViewGroup\", System.Windows.Forms.HorizontalAlignment.Right)\r\n" +
								"    listViewGroup2 = System.Windows.Forms.ListViewGroup(\"ListViewGroup\", System.Windows.Forms.HorizontalAlignment.Center)\r\n" +
								"    self._listView1 = System.Windows.Forms.ListView()\r\n" +
								"    self.SuspendLayout()\r\n" +
								"    # \r\n" +
								"    # listView1\r\n" +
								"    # \r\n" +
								"    listViewGroup1.Header = \"ListViewGroup\"\r\n" +
								"    listViewGroup1.HeaderAlignment = System.Windows.Forms.HorizontalAlignment.Right\r\n" +
								"    listViewGroup1.Name = \"defaultGroup\"\r\n" +
								"    listViewGroup1.Tag = \"tag1\"\r\n" +
								"    listViewGroup2.Header = \"ListViewGroup\"\r\n" +
								"    listViewGroup2.HeaderAlignment = System.Windows.Forms.HorizontalAlignment.Center\r\n" +
								"    listViewGroup2.Name = \"listViewGroup2\"\r\n" +
								"    self._listView1.Groups.AddRange(System.Array[System.Windows.Forms.ListViewGroup](\r\n" +
								"        [listViewGroup1,\r\n" +
								"        listViewGroup2]))\r\n" +
								"    self._listView1.Location = System.Drawing.Point(0, 0)\r\n" +
								"    self._listView1.Name = \"listView1\"\r\n" +
								"    self._listView1.Size = System.Drawing.Size(204, 104)\r\n" +
								"    self._listView1.TabIndex = 0\r\n" +
								"    self._listView1.View = System.Windows.Forms.View.Details\r\n" +
								"    # \r\n" +
								"    # MainForm\r\n" +
								"    # \r\n" +
								"    self.ClientSize = System.Drawing.Size(200, 300)\r\n" +
								"    self.Controls.Add(self._listView1)\r\n" +
								"    self.Name = \"MainForm\"\r\n" +
								"    self.ResumeLayout(False)\r\n";
			
			Assert.AreEqual(expectedCode, generatedPythonCode, generatedPythonCode);
		}		
	}
}
