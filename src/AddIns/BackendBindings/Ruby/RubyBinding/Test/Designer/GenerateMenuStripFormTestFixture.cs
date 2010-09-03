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
	/// <summary>
	/// Adding a MenuStrip control to a form in the designer generates code for a 
	/// miniToolStrip - System.Windows.Forms.Design.ToolStripTemplateNode+TransparentToolStrip()
	/// This is a design time control and should be ignored.
	/// </summary>
	[TestFixture]
	public class GenerateMenuStripFormTestFixture
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
				
				// Add menu strip.
				MenuStrip menuStrip = (MenuStrip)host.CreateComponent(typeof(MenuStrip), "menuStrip1");
				menuStrip.Text = "menuStrip1";
				menuStrip.TabIndex = 0;
				menuStrip.Location = new Point(0, 0);
				form.Controls.Add(menuStrip);
				
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
			string expectedCode = "    @menuStrip1 = System::Windows::Forms::MenuStrip.new()\r\n" +
								"    self.SuspendLayout()\r\n" +
								"    # \r\n" +
								"    # menuStrip1\r\n" +
								"    # \r\n" +
								"    @menuStrip1.Location = System::Drawing::Point.new(0, 0)\r\n" +
								"    @menuStrip1.Name = \"menuStrip1\"\r\n" +
								"    @menuStrip1.Size = System::Drawing::Size.new(200, 24)\r\n" +
								"    @menuStrip1.TabIndex = 0\r\n" +
								"    @menuStrip1.Text = \"menuStrip1\"\r\n" +
								"    # \r\n" +
								"    # MainForm\r\n" +
								"    # \r\n" +
								"    self.ClientSize = System::Drawing::Size.new(200, 300)\r\n" +
								"    self.Controls.Add(@menuStrip1)\r\n" +
								"    self.Name = \"MainForm\"\r\n" +
								"    self.ResumeLayout(false)\r\n" +
								"    self.PerformLayout()\r\n";
			
			Assert.AreEqual(expectedCode, generatedRubyCode);
		}
	}
}
