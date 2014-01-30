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

using ICSharpCode.RubyBinding;
using ICSharpCode.Scripting.Tests.Utils;
using NUnit.Framework;
using RubyBinding.Tests.Utils;

namespace RubyBinding.Tests.Designer
{
	[TestFixture]
	public class GenerateListViewItemsFormTestFixture
	{
		string generatedRubyCode;
		ColumnHeader columnHeader1;
		ColumnHeader columnHeader2;
		
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
				
				// Add column headers.
				columnHeader1 = (ColumnHeader)host.CreateComponent(typeof(ColumnHeader), "columnHeader1");
				descriptors = TypeDescriptor.GetProperties(columnHeader1);
				descriptor = descriptors.Find("Text", false);
				descriptor.SetValue(columnHeader1, "columnHeader1");
				listView.Columns.Add(columnHeader1);
				
				columnHeader2 = (ColumnHeader)host.CreateComponent(typeof(ColumnHeader), "columnHeader2");
				descriptors = TypeDescriptor.GetProperties(columnHeader2);
				descriptor = descriptors.Find("Text", false);
				descriptor.SetValue(columnHeader2, "columnHeader2");
				listView.Columns.Add(columnHeader2);
				
				// Add list view items.
				DesignerSerializationManager designerSerializationManager = new DesignerSerializationManager(host);
				IDesignerSerializationManager serializationManager = (IDesignerSerializationManager)designerSerializationManager;
				using (designerSerializationManager.CreateSession()) {
					ListViewItem item = (ListViewItem)serializationManager.CreateInstance(typeof(ListViewItem), new object[] {"aaa"}, "listViewItem1", false);
					item.ToolTipText = "tooltip";
					listView.Items.Add(item);
					
					item = (ListViewItem)serializationManager.CreateInstance(typeof(ListViewItem), new object[] {"bbb"}, "listViewItem2", false);
					listView.Items.Add(item);
					
					item = (ListViewItem)serializationManager.CreateInstance(typeof(ListViewItem), new object[0], "listViewItem3", false);
					listView.Items.Add(item);

					RubyCodeDomSerializer serializer = new RubyCodeDomSerializer("    ");
					generatedRubyCode = serializer.GenerateInitializeComponentMethodBody(host, serializationManager, 1);
				}	
			}
		}
		
		[Test]
		public void GeneratedCode()
		{
			string expectedCode = "    listViewItem1 = System::Windows::Forms::ListViewItem.new(\"aaa\")\r\n" +
								"    listViewItem2 = System::Windows::Forms::ListViewItem.new(\"bbb\")\r\n" +
								"    listViewItem3 = System::Windows::Forms::ListViewItem.new(\"\")\r\n" +
								"    @listView1 = System::Windows::Forms::ListView.new()\r\n" +
								"    @columnHeader1 = System::Windows::Forms::ColumnHeader.new()\r\n" +
								"    @columnHeader2 = System::Windows::Forms::ColumnHeader.new()\r\n" +
								"    self.SuspendLayout()\r\n" +
								"    # \r\n" +
								"    # listView1\r\n" +
								"    # \r\n" +
								"    @listView1.Columns.AddRange(System::Array[System::Windows::Forms::ColumnHeader].new(\r\n" +
								"        [@columnHeader1,\r\n" +
								"        @columnHeader2]))\r\n" +
								"    listViewItem1.ToolTipText = \"tooltip\"\r\n" +
								"    @listView1.Items.AddRange(System::Array[System::Windows::Forms::ListViewItem].new(\r\n" +
								"        [listViewItem1,\r\n" +
								"        listViewItem2,\r\n" +
								"        listViewItem3]))\r\n" +
								"    @listView1.Location = System::Drawing::Point.new(0, 0)\r\n" +
								"    @listView1.Name = \"listView1\"\r\n" +
								"    @listView1.Size = System::Drawing::Size.new(204, 104)\r\n" +
								"    @listView1.TabIndex = 0\r\n" +
								"    @listView1.View = System::Windows::Forms::View.Details\r\n" +
								"    # \r\n" +
								"    # columnHeader1\r\n" +
								"    # \r\n" +
								"    @columnHeader1.Text = \"columnHeader1\"\r\n" +
								"    # \r\n" +
								"    # columnHeader2\r\n" +
								"    # \r\n" +
								"    @columnHeader2.Text = \"columnHeader2\"\r\n" +
								"    # \r\n" +
								"    # MainForm\r\n" +
								"    # \r\n" +
								"    self.ClientSize = System::Drawing::Size.new(200, 300)\r\n" +
								"    self.Controls.Add(@listView1)\r\n" +
								"    self.Name = \"MainForm\"\r\n" +
								"    self.ResumeLayout(false)\r\n";
			
			Assert.AreEqual(expectedCode, generatedRubyCode, generatedRubyCode);
		}		
	}
}
