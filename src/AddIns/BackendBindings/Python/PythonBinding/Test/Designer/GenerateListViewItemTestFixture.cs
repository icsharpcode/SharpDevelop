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
	public class GenerateListViewItemsFormTestFixture
	{
		string generatedPythonCode;
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

					PythonCodeDomSerializer serializer = new PythonCodeDomSerializer("    ");
					generatedPythonCode = serializer.GenerateInitializeComponentMethodBody(host, serializationManager, String.Empty, 1);
				}	
			}
		}
		
		[Test]
		public void GeneratedCode()
		{
			string expectedCode = "    listViewItem1 = System.Windows.Forms.ListViewItem(\"aaa\")\r\n" +
								"    listViewItem2 = System.Windows.Forms.ListViewItem(\"bbb\")\r\n" +
								"    listViewItem3 = System.Windows.Forms.ListViewItem(\"\")\r\n" +
								"    self._listView1 = System.Windows.Forms.ListView()\r\n" +
								"    self._columnHeader1 = System.Windows.Forms.ColumnHeader()\r\n" +
								"    self._columnHeader2 = System.Windows.Forms.ColumnHeader()\r\n" +
								"    self.SuspendLayout()\r\n" +
								"    # \r\n" +
								"    # listView1\r\n" +
								"    # \r\n" +
								"    self._listView1.Columns.AddRange(System.Array[System.Windows.Forms.ColumnHeader](\r\n" +
								"        [self._columnHeader1,\r\n" +
								"        self._columnHeader2]))\r\n" +
								"    listViewItem1.ToolTipText = \"tooltip\"\r\n" +
								"    self._listView1.Items.AddRange(System.Array[System.Windows.Forms.ListViewItem](\r\n" +
								"        [listViewItem1,\r\n" +
								"        listViewItem2,\r\n" +
								"        listViewItem3]))\r\n" +
								"    self._listView1.Location = System.Drawing.Point(0, 0)\r\n" +
								"    self._listView1.Name = \"listView1\"\r\n" +
								"    self._listView1.Size = System.Drawing.Size(204, 104)\r\n" +
								"    self._listView1.TabIndex = 0\r\n" +
								"    self._listView1.View = System.Windows.Forms.View.Details\r\n" +
								"    # \r\n" +
								"    # columnHeader1\r\n" +
								"    # \r\n" +
								"    self._columnHeader1.Text = \"columnHeader1\"\r\n" +
								"    # \r\n" +
								"    # columnHeader2\r\n" +
								"    # \r\n" +
								"    self._columnHeader2.Text = \"columnHeader2\"\r\n" +
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
