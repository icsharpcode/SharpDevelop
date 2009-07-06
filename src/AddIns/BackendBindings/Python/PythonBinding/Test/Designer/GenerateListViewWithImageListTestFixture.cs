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
	public class GenerateListViewWithImageListFormTestFixture
	{
		string createListViewCode;

		
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
				
				// Add ImageList.
				Icon icon = new Icon(typeof(GenerateFormResourceTestFixture), "App.ico");
				ImageList imageList = (ImageList)host.CreateComponent(typeof(ImageList), "imageList1");
				imageList.Images.Add("App.ico", icon);
				imageList.Images.Add("b.ico", icon);
				imageList.Images.Add("c.ico", icon); 
				
				// Add list view items.
				ListViewItem item = new ListViewItem("aaa");
				item.ImageIndex = 1;
				listView.Items.Add(item);
				
				ListViewItem item2 = new ListViewItem("bbb");
				item2.ImageKey = "App.ico";
				listView.Items.Add(item2);
				
				ListViewItem item3 = new ListViewItem();
				item3.ImageIndex = 2;
				listView.Items.Add(item3);

				ListViewItem item4 = new ListViewItem();
				item4.ImageKey = "b.ico";
				listView.Items.Add(item4);
				
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
			string expectedCode = "listViewItem1 = System.Windows.Forms.ListViewItem(\"aaa\", 1)\r\n" +
								"listViewItem2 = System.Windows.Forms.ListViewItem(\"bbb\", \"App.ico\")\r\n" +
								"listViewItem3 = System.Windows.Forms.ListViewItem(\"\", 2)\r\n" +
								"listViewItem4 = System.Windows.Forms.ListViewItem(\"\", \"b.ico\")\r\n" +
								"self._listView1 = System.Windows.Forms.ListView()\r\n";
			Assert.AreEqual(expectedCode, createListViewCode, createListViewCode);
		}
	}
}
