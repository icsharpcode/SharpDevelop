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
		string createListViewCode;
		string createListViewChildComponentsCode;
		string suspendLayoutCode;
		string resumeLayoutCode;
		string listViewPropertiesCode;
		PythonDesignerComponent[] listViewChildComponents;
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
				ListViewItem item = new ListViewItem("aaa");
				item.ToolTipText = "tooltip";
				listView.Items.Add(item);
				
				ListViewItem item2 = new ListViewItem("bbb");
				listView.Items.Add(item2);
				
				ListViewItem item3 = new ListViewItem();
				listView.Items.Add(item3);

				PythonControl pythonForm = new PythonControl("    ");
				generatedPythonCode = pythonForm.GenerateInitializeComponentMethod(form);
				
				// Get list view creation code.
				PythonCodeBuilder codeBuilder = new PythonCodeBuilder();
				codeBuilder.IndentString = "    ";
				PythonListViewComponent listViewComponent = new PythonListViewComponent(listView);
				listViewComponent.AppendCreateInstance(codeBuilder);
				createListViewCode = codeBuilder.ToString();
			
				// Get list view child component's creation code.
				codeBuilder = new PythonCodeBuilder();
				codeBuilder.IndentString = "    ";
				listViewComponent.AppendCreateChildComponents(codeBuilder);
				createListViewChildComponentsCode = codeBuilder.ToString();
				
				// Get list view suspend layout code.
				codeBuilder = new PythonCodeBuilder();
				codeBuilder.IndentString = "    ";
				listViewComponent.AppendSuspendLayout(codeBuilder);
				suspendLayoutCode = codeBuilder.ToString();
		
				// Get generated list view property code.
				codeBuilder = new PythonCodeBuilder();
				codeBuilder.IndentString = "    ";
				codeBuilder.IncreaseIndent();
				listViewComponent.AppendComponent(codeBuilder);
				listViewPropertiesCode = codeBuilder.ToString();
				
				// Get list view resume layout code.
				codeBuilder = new PythonCodeBuilder();
				codeBuilder.IndentString = "    ";
				listViewComponent.AppendResumeLayout(codeBuilder);
				resumeLayoutCode = codeBuilder.ToString();
				
				listViewChildComponents = listViewComponent.GetChildComponents();
			}
		}
		
		[Test]
		public void GeneratedCode()
		{
			string expectedCode = "def InitializeComponent(self):\r\n" +
								"    listViewItem1 = System.Windows.Forms.ListViewItem(\"aaa\")\r\n" +
								"    listViewItem2 = System.Windows.Forms.ListViewItem(\"bbb\")\r\n" +
								"    listViewItem3 = System.Windows.Forms.ListViewItem()\r\n" +
								"    self._listView1 = System.Windows.Forms.ListView()\r\n" +
								"    self._columnHeader1 = System.Windows.Forms.ColumnHeader()\r\n" +
								"    self._columnHeader2 = System.Windows.Forms.ColumnHeader()\r\n" +
								"    self._listView1.SuspendLayout()\r\n" +
								"    self.SuspendLayout()\r\n" +
								"    # \r\n" +
								"    # listView1\r\n" +
								"    # \r\n" +
								"    listViewItem1.ToolTipText = \"tooltip\"\r\n" +
								"    self._listView1.Columns.AddRange(System.Array[System.Windows.Forms.ColumnHeader](\r\n" +
								"        [self._columnHeader1,\r\n" +
								"        self._columnHeader2]))\r\n" +
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
								"    self._listView1.ResumeLayout(False)\r\n" +
								"    self._listView1.PerformLayout()\r\n" +	
								"    self.ResumeLayout(False)\r\n" +
								"    self.PerformLayout()\r\n";
			
			Assert.AreEqual(expectedCode, generatedPythonCode, generatedPythonCode);
		}
		
		/// <summary>
		/// Should include the column header and list view item creation.
		/// </summary>
		[Test]
		public void ListViewCreationCode()
		{
			string expectedCode = "listViewItem1 = System.Windows.Forms.ListViewItem(\"aaa\")\r\n" +
								"listViewItem2 = System.Windows.Forms.ListViewItem(\"bbb\")\r\n" +
								"listViewItem3 = System.Windows.Forms.ListViewItem()\r\n" +
								"self._listView1 = System.Windows.Forms.ListView()\r\n";
			Assert.AreEqual(expectedCode, createListViewCode);
		}
		
		[Test]
		public void TwoListViewChildComponents()
		{
			Assert.AreEqual(2, listViewChildComponents.Length);
		}
		
		[Test]
		public void ListViewChildComponentAreColumnHeaders()
		{
			Assert.AreEqual(typeof(ColumnHeader), listViewChildComponents[0].GetComponentType());
			Assert.AreEqual(typeof(ColumnHeader), listViewChildComponents[1].GetComponentType());
		}
		
		[Test]
		public void SuspendLayoutGeneratedCode()
		{
			string expectedCode = "self._listView1.SuspendLayout()\r\n";
			Assert.AreEqual(expectedCode, suspendLayoutCode);
		}
		
		[Test]
		public void ResumeLayoutGeneratedCode()
		{
			string expectedCode = "self._listView1.ResumeLayout(False)\r\n" +
								"self._listView1.PerformLayout()\r\n";
			Assert.AreEqual(expectedCode, resumeLayoutCode);
		}		
		
		[Test]
		public void GeneratedListViewPropertiesCode()
		{
			string expectedCode = "    # \r\n" +
								"    # listView1\r\n" +
								"    # \r\n" +
								"    listViewItem1.ToolTipText = \"tooltip\"\r\n" +
								"    self._listView1.Columns.AddRange(System.Array[System.Windows.Forms.ColumnHeader](\r\n" +
								"        [self._columnHeader1,\r\n" +
								"        self._columnHeader2]))\r\n" +
								"    self._listView1.Items.AddRange(System.Array[System.Windows.Forms.ListViewItem](\r\n" +
								"        [listViewItem1,\r\n" +
								"        listViewItem2,\r\n" +
								"        listViewItem3]))\r\n" +
								"    self._listView1.Location = System.Drawing.Point(0, 0)\r\n" +
								"    self._listView1.Name = \"listView1\"\r\n" +
								"    self._listView1.Size = System.Drawing.Size(204, 104)\r\n" +
								"    self._listView1.TabIndex = 0\r\n" +
								"    self._listView1.View = System.Windows.Forms.View.Details\r\n";

			Assert.AreEqual(expectedCode, listViewPropertiesCode);
		}
		
		[Test]
		public void ListViewChildComponentsCreated()
		{
			string expectedCode = "self._columnHeader1 = System.Windows.Forms.ColumnHeader()\r\n" +
								"self._columnHeader2 = System.Windows.Forms.ColumnHeader()\r\n";
			
			Assert.AreEqual(expectedCode, createListViewChildComponentsCode);
		}
		
		[Test]
		public void ColumnHeaderIsHiddenFromDesigner()
		{
			Assert.IsTrue(PythonDesignerComponent.IsHiddenFromDesigner(columnHeader1));
		}
	}
}
