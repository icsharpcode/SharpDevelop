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
	public class GenerateListViewSubItemsTestFixture
	{
		string createListViewCode;
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
				
				// Add list view item with 3 sub items.
				ListViewItem item = new ListViewItem("listItem1");
				item.SubItems.Add("subItem1");
				item.SubItems.Add("subItem2");
				item.SubItems.Add("subItem3");
				listView.Items.Add(item);
				
				
				// Get list view creation code.
				PythonCodeBuilder codeBuilder = new PythonCodeBuilder();
				codeBuilder.IndentString = "    ";
				PythonListViewComponent listViewComponent = new PythonListViewComponent(listView);
				listViewComponent.AppendCreateInstance(codeBuilder);
				createListViewCode = codeBuilder.ToString();			
			}
		}
		
		/// <summary>
		/// Should include the column header and list view item creation.
		/// </summary>
		[Test]
		public void ListViewCreationCode()
		{
			string expectedCode = "listViewItem1 = System.Windows.Forms.ListViewItem(System.Array[System.String](\r\n" +
								"    [\"listItem1\",\r\n" +
								"    \"subItem1\",\r\n" +
								"    \"subItem2\",\r\n" +
								"    \"subItem3\"]))\r\n" +
								"self._listView1 = System.Windows.Forms.ListView()\r\n";
			Assert.AreEqual(expectedCode, createListViewCode, createListViewCode);
		}		
	}
}
