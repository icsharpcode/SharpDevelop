// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
	public class GenerateTableLayoutPanelTestFixture
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

				PropertyDescriptorCollection descriptors = TypeDescriptor.GetProperties(form);
				PropertyDescriptor namePropertyDescriptor = descriptors.Find("Name", false);
				namePropertyDescriptor.SetValue(form, "MainForm");
				
				// Add table layout panel.
				TableLayoutPanel tableLayoutPanel1 = (TableLayoutPanel)host.CreateComponent(typeof(TableLayoutPanel), "tableLayoutPanel1");
				tableLayoutPanel1.ColumnCount = 2;
				tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 40F));
				tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 60F));
				tableLayoutPanel1.Location = new Point(0, 0);
				tableLayoutPanel1.RowCount = 2;
				tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 20F));
				tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 25F));
				tableLayoutPanel1.Size = new Size(200, 100);
				tableLayoutPanel1.TabIndex = 0;
								
				form.Controls.Add(tableLayoutPanel1);
				
				DesignerSerializationManager serializationManager = new DesignerSerializationManager(host);
				using (serializationManager.CreateSession()) {					
					RubyCodeDomSerializer serializer = new RubyCodeDomSerializer("    ");
					generatedRubyCode = serializer.GenerateInitializeComponentMethodBody(host, serializationManager, String.Empty, 1);
				}
			}
		}
		
		[Test]
		public void GeneratedCode()
		{
			string expectedCode = "    @tableLayoutPanel1 = System::Windows::Forms::TableLayoutPanel.new()\r\n" +
								"    self.SuspendLayout()\r\n" +
								"    # \r\n" +
								"    # tableLayoutPanel1\r\n" +
								"    # \r\n" +
								"    @tableLayoutPanel1.ColumnCount = 2\r\n" +
								"    @tableLayoutPanel1.ColumnStyles.Add(System::Windows::Forms::ColumnStyle.new(System::Windows::Forms::SizeType.Percent, 40))\r\n" +
								"    @tableLayoutPanel1.ColumnStyles.Add(System::Windows::Forms::ColumnStyle.new(System::Windows::Forms::SizeType.Percent, 60))\r\n" +
								"    @tableLayoutPanel1.Location = System::Drawing::Point.new(0, 0)\r\n" +
								"    @tableLayoutPanel1.Name = \"tableLayoutPanel1\"\r\n" +
								"    @tableLayoutPanel1.RowCount = 2\r\n" +
								"    @tableLayoutPanel1.RowStyles.Add(System::Windows::Forms::RowStyle.new(System::Windows::Forms::SizeType.Absolute, 20))\r\n" +
								"    @tableLayoutPanel1.RowStyles.Add(System::Windows::Forms::RowStyle.new(System::Windows::Forms::SizeType.Absolute, 25))\r\n" +
								"    @tableLayoutPanel1.Size = System::Drawing::Size.new(200, 100)\r\n" +
								"    @tableLayoutPanel1.TabIndex = 0\r\n" +
								"    # \r\n" +
								"    # MainForm\r\n" +
								"    # \r\n" +
								"    self.ClientSize = System::Drawing::Size.new(200, 300)\r\n" +
								"    self.Controls.Add(@tableLayoutPanel1)\r\n" +
								"    self.Name = \"MainForm\"\r\n" +
								"    self.ResumeLayout(false)\r\n";
			
			Assert.AreEqual(expectedCode, generatedRubyCode, generatedRubyCode);
		}		
	}
}
