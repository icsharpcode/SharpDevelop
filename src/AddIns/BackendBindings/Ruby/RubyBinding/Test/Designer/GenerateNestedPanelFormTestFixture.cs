// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.ComponentModel.Design.Serialization;
using System.Drawing;
using System.Windows.Forms;
using ICSharpCode.RubyBinding;
using NUnit.Framework;
using RubyBinding.Tests.Utils;

namespace RubyBinding.Tests.Designer
{
	/// <summary>
	/// Ensures that SuspendLayout and ResumeLayout methods are generated for a panel containing controls and sitting on top of 
	/// another panel.
	/// </summary>
	[TestFixture]
	public class GenerateNestedPanelFormTestFixture
	{
		string generatedRubyCode;
		
		[TestFixtureSetUp]
		public void SetUpFixture()
		{
			using (DesignSurface designSurface = new DesignSurface(typeof(Form))) {
				IDesignerHost host = (IDesignerHost)designSurface.GetService(typeof(IDesignerHost));
				Form form = (Form)host.RootComponent;			
				form.ClientSize = new Size(284, 264);
				
				PropertyDescriptorCollection descriptors = TypeDescriptor.GetProperties(form);
				PropertyDescriptor namePropertyDescriptor = descriptors.Find("Name", false);
				namePropertyDescriptor.SetValue(form, "MainForm");
				
				Panel panel1 = (Panel)host.CreateComponent(typeof(Panel), "panel1");
				panel1.Location = new Point(0, 0);
				panel1.TabIndex = 0;
				panel1.Size = new Size(200, 220);
				
				Panel panel2 = (Panel)host.CreateComponent(typeof(Panel), "panel2");
				panel2.Location = new Point(10, 15);
				panel2.TabIndex = 0;
				panel2.Size = new Size(100, 120);
				TextBox textBox = (TextBox)host.CreateComponent(typeof(TextBox), "textBox1");
				textBox.Location = new Point(5, 5);
				textBox.TabIndex = 0;
				textBox.Size = new Size(110, 20);
				panel2.Controls.Add(textBox);
				
				panel1.Controls.Add(panel2);
				form.Controls.Add(panel1);
				
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
			string expectedCode = "    @panel1 = System::Windows::Forms::Panel.new()\r\n" +
								"    @panel2 = System::Windows::Forms::Panel.new()\r\n" +
								"    @textBox1 = System::Windows::Forms::TextBox.new()\r\n" +
								"    @panel1.SuspendLayout()\r\n" +
								"    @panel2.SuspendLayout()\r\n" +
								"    self.SuspendLayout()\r\n" +
								"    # \r\n" +
								"    # panel1\r\n" +
								"    # \r\n" +
								"    @panel1.Controls.Add(@panel2)\r\n" +
								"    @panel1.Location = System::Drawing::Point.new(0, 0)\r\n" +
								"    @panel1.Name = \"panel1\"\r\n" +
								"    @panel1.Size = System::Drawing::Size.new(200, 220)\r\n" +
								"    @panel1.TabIndex = 0\r\n" +
								"    # \r\n" +
								"    # panel2\r\n" +
								"    # \r\n" +
								"    @panel2.Controls.Add(@textBox1)\r\n" +
								"    @panel2.Location = System::Drawing::Point.new(10, 15)\r\n" +
								"    @panel2.Name = \"panel2\"\r\n" +
								"    @panel2.Size = System::Drawing::Size.new(100, 120)\r\n" +
								"    @panel2.TabIndex = 0\r\n" +
								"    # \r\n" +
								"    # textBox1\r\n" +
								"    # \r\n" +
								"    @textBox1.Location = System::Drawing::Point.new(5, 5)\r\n" +
								"    @textBox1.Name = \"textBox1\"\r\n" +
								"    @textBox1.Size = System::Drawing::Size.new(110, 20)\r\n" +
								"    @textBox1.TabIndex = 0\r\n" +
								"    # \r\n" +
								"    # MainForm\r\n" +
								"    # \r\n" +
								"    self.ClientSize = System::Drawing::Size.new(284, 264)\r\n" +
								"    self.Controls.Add(@panel1)\r\n" +
								"    self.Name = \"MainForm\"\r\n" +
								"    @panel1.ResumeLayout(false)\r\n" +
								"    @panel2.ResumeLayout(false)\r\n" +
								"    @panel2.PerformLayout()\r\n" +
								"    self.ResumeLayout(false)\r\n";
			
			Assert.AreEqual(expectedCode, generatedRubyCode, generatedRubyCode);
		}
	}
}
