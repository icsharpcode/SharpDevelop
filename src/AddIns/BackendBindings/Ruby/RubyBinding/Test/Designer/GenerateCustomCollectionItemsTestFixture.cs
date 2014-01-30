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
	/// <summary>
	/// Tests that a custom collection class generates the correct code.
	/// The collection class should be a property of a custom component or user control
	/// and it should be marked with DesignerSerializationVisibility.Content.
	/// </summary>
	[TestFixture]
	public class GenerateCustomCollectionItemsTestFixture
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
				
				// Add custom control
				CustomUserControl userControl = (CustomUserControl)host.CreateComponent(typeof(CustomUserControl), "userControl1");
				userControl.Location = new Point(0, 0);
				userControl.ClientSize = new Size(200, 100);
				
				DesignerSerializationManager designerSerializationManager = new DesignerSerializationManager(host);
				IDesignerSerializationManager serializationManager = (IDesignerSerializationManager)designerSerializationManager;
				using (designerSerializationManager.CreateSession()) {					
					FooItem fooItem = (FooItem)serializationManager.CreateInstance(typeof(FooItem), new object[] {"aa"}, "fooItem1", false);
					userControl.FooItems.Add(fooItem);
					fooItem = (FooItem)serializationManager.CreateInstance(typeof(FooItem), new object[] {"bb"}, "fooItem2", false);
					userControl.FooItems.Add(fooItem);
					
					BarItem barItem = (BarItem)serializationManager.CreateInstance(typeof(BarItem), new object[] {"cc"}, "barItem1", false);
					userControl.ParentComponent.ParentBarItems.Add(barItem);
					barItem = (BarItem)serializationManager.CreateInstance(typeof(BarItem), new object[] {"dd"}, "barItem2", false);
					userControl.ParentComponent.ParentBarItems.Add(barItem);
					form.Controls.Add(userControl);
				
					RubyCodeDomSerializer serializer = new RubyCodeDomSerializer("    ");
					generatedRubyCode = serializer.GenerateInitializeComponentMethodBody(host, serializationManager, 1);
				}	
			}
		}
		
		[Test]
		public void GeneratedCode()
		{
			string expectedCode = "    fooItem1 = ICSharpCode::Scripting::Tests::Utils::FooItem.new()\r\n" +
								"    fooItem2 = ICSharpCode::Scripting::Tests::Utils::FooItem.new()\r\n" +
								"    barItem1 = ICSharpCode::Scripting::Tests::Utils::BarItem.new()\r\n" +
								"    barItem2 = ICSharpCode::Scripting::Tests::Utils::BarItem.new()\r\n" +
								"    @userControl1 = ICSharpCode::Scripting::Tests::Utils::CustomUserControl.new()\r\n" +
								"    self.SuspendLayout()\r\n" +
								"    # \r\n" +
								"    # userControl1\r\n" +
								"    # \r\n" +
								"    fooItem1.Text = \"aa\"\r\n" +
								"    fooItem2.Text = \"bb\"\r\n" +
								"    @userControl1.FooItems.AddRange(System::Array[ICSharpCode::Scripting::Tests::Utils::FooItem].new(\r\n" +
								"        [fooItem1,\r\n" +
								"        fooItem2]))\r\n" +
								"    @userControl1.Location = System::Drawing::Point.new(0, 0)\r\n" +
								"    @userControl1.Name = \"userControl1\"\r\n" +
								"    # \r\n" +
								"    # \r\n" +
								"    # \r\n" +
								"    barItem1.Text = \"cc\"\r\n" +
								"    barItem2.Text = \"dd\"\r\n" +
								"    @userControl1.ParentComponent.ParentBarItems.AddRange(System::Array[ICSharpCode::Scripting::Tests::Utils::BarItem].new(\r\n" +
								"        [barItem1,\r\n" +
								"        barItem2]))\r\n" +
								"    @userControl1.Size = System::Drawing::Size.new(200, 100)\r\n" +
								"    @userControl1.TabIndex = 0\r\n" +
								"    # \r\n" +
								"    # MainForm\r\n" +
								"    # \r\n" +
								"    self.ClientSize = System::Drawing::Size.new(200, 300)\r\n" +
								"    self.Controls.Add(@userControl1)\r\n" +
								"    self.Name = \"MainForm\"\r\n" +
								"    self.ResumeLayout(false)\r\n";
			
			Assert.AreEqual(expectedCode, generatedRubyCode, generatedRubyCode);
		}
	}
}
