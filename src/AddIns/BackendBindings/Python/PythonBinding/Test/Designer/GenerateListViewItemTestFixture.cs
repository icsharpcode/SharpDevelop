// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
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
	public class GenerateListViewItemsFormTestFixture
	{
		string generatedPythonCode;
		object[] listViewChildren;
		ListViewItem listViewItem1;
		ListViewItem listViewItem2;
		
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
				form.Controls.Add(listView);
				
				ListViewItem item = new ListViewItem("aaa");
				item.ToolTipText = "tooltip";
				listView.Items.Add(item);
				
				ListViewItem item2 = new ListViewItem("bbb");
				listView.Items.Add(item2);

				PythonControl pythonForm = new PythonControl("    ");
				generatedPythonCode = pythonForm.GenerateInitializeComponentMethod(form);
				
				listViewChildren = PythonControl.GetChildComponents(listView);
				if (listViewChildren != null && listViewChildren.Length > 1) {
					listViewItem1 = listViewChildren[0] as ListViewItem;
					listViewItem2 = listViewChildren[1] as ListViewItem;
				}
			}
		}
		
		[Test]
		public void GeneratedCode()
		{
			string expectedCode = "def InitializeComponent(self):\r\n" +
								"    self._listView1 = System.Windows.Forms.ListView()\r\n" +
								"    listViewItem1 = System.Windows.Forms.ListViewItem()\r\n" +
								"    listViewItem2 = System.Windows.Forms.ListViewItem()\r\n" +
								"    self.SuspendLayout()\r\n" +
								"    # \r\n" +
								"    # listView1\r\n" +
								"    # \r\n" +
								"    self._listView1.Items.AddRange(System.Array[System.Windows.Forms.ListViewItem](\r\n" +
								"        [listViewItem1,\r\n" +
								"        listViewItem2]))\r\n" +
								"    self._listView1.Location = System.Drawing.Point(0, 0)\r\n" +
								"    self._listView1.Name = \"listView1\"\r\n" +
								"    self._listView1.Size = System.Drawing.Size(204, 104)\r\n" +
								"    self._listView1.TabIndex = 0\r\n" +
								"    listViewItem1.ToolTipText = \"tooltip\"\r\n" +
								"    # \r\n" +
								"    # MainForm\r\n" +
								"    # \r\n" +
								"    self.ClientSize = System.Drawing.Size(200, 300)\r\n" +
								"    self.Controls.Add(self._listView1)\r\n" +
								"    self.Name = \"MainForm\"\r\n" +
								"    self.ResumeLayout(False)\r\n" +
								"    self.PerformLayout()\r\n";
			
			Assert.AreEqual(expectedCode, generatedPythonCode);
		}
		
		[Test]
		public void TwoListViewChildren()
		{
			Assert.AreEqual(2, listViewChildren.Length);
		}
		
		[Test]
		public void ListViewItem1Text()
		{
			Assert.AreEqual("aaa", listViewItem1.Text);
		}
		
		[Test]
		public void ListViewItem1TooltipText()
		{
			Assert.AreEqual("tooltip", listViewItem1.ToolTipText);
		}	
		
		[Test]
		public void ListViewItem2Text()
		{
			Assert.AreEqual("bbb", listViewItem2.Text);
		}
	}
}
