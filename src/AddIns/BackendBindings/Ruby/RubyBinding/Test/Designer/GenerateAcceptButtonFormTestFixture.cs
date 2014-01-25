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
	public class GenerateAcceptButtonFormTestFixture
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
				
				Button button = (Button)host.CreateComponent(typeof(Button), "button1");
				button.Location = new Point(0, 0);
				button.Size = new Size(10, 10);
				button.Text = "button1";
				button.UseCompatibleTextRendering = false;
				form.Controls.Add(button);

				PropertyDescriptorCollection descriptors = TypeDescriptor.GetProperties(form);
				PropertyDescriptor acceptButtonPropertyDescriptor = descriptors.Find("AcceptButton", false);
				acceptButtonPropertyDescriptor.SetValue(form, button);

				PropertyDescriptor namePropertyDescriptor = descriptors.Find("Name", false);
				namePropertyDescriptor.SetValue(form, "MainForm");
				
				DesignerSerializationManager serializationManager = new DesignerSerializationManager(host);
				using (serializationManager.CreateSession()) {
					RubyCodeDomSerializer serializer = new RubyCodeDomSerializer("    ");
					generatedRubyCode = serializer.GenerateInitializeComponentMethodBody(host, serializationManager);
				}
			}
		}
		
		[Test]
		public void GeneratedCode()
		{
			string expectedCode = "@button1 = System::Windows::Forms::Button.new()\r\n" +
								"self.SuspendLayout()\r\n" +
								"# \r\n" +
								"# button1\r\n" +
								"# \r\n" +
								"@button1.Location = System::Drawing::Point.new(0, 0)\r\n" +
								"@button1.Name = \"button1\"\r\n" +
								"@button1.Size = System::Drawing::Size.new(10, 10)\r\n" +
								"@button1.TabIndex = 0\r\n" +
								"@button1.Text = \"button1\"\r\n" +
								"# \r\n" +
								"# MainForm\r\n" +
								"# \r\n" +
								"self.AcceptButton = @button1\r\n" +
								"self.ClientSize = System::Drawing::Size.new(200, 300)\r\n" +
								"self.Controls.Add(@button1)\r\n" +
								"self.Name = \"MainForm\"\r\n" +
								"self.ResumeLayout(false)\r\n";
			
			Assert.AreEqual(expectedCode, generatedRubyCode, generatedRubyCode);
		}
	}
}
