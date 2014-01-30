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
using System.Drawing;
using System.IO;
using System.Windows.Forms;

using ICSharpCode.RubyBinding;
using ICSharpCode.Scripting.Tests.Utils;
using NUnit.Framework;
using RubyBinding.Tests.Utils;

namespace RubyBinding.Tests.Designer
{
	[TestFixture]
	public class LoadEventHandlerTestFixture : LoadFormTestFixtureBase
	{		
		public override string RubyCode {
			get {
				return
					"class TestForm < System::Windows::Forms::Form\r\n" +
					"    def InitializeComponent()\r\n" +
					"        self.SuspendLayout()\r\n" +
					"        # \r\n" +
					"        # TestForm\r\n" +
					"        # \r\n" +
					"        self.Name = \"TestForm\"\r\n" +
					"        self.Load { |sender, e| self.TestFormLoad(sender, e) }\r\n" +
					"        self.ResumeLayout(false)\r\n" +
					"    end\r\n" +
					"end";
			}
		}
		
		public override void BeforeSetUpFixture()
		{
			base.ComponentCreator.SetEventPropertyDescriptor(new MockPropertyDescriptor("abc", "TestFormLoad", true));
		}		
		
		public EventDescriptor GetLoadEventDescriptor()
		{
			return TypeDescriptor.GetEvents(Form).Find("Load", true);
		}
		
		public MockPropertyDescriptor GetLoadEventPropertyDescriptor()
		{
			EventDescriptor loadEventDescriptor = GetLoadEventDescriptor();
			return base.ComponentCreator.GetEventProperty(loadEventDescriptor) as MockPropertyDescriptor;			
		}
		
		[Test]
		public void EventPropertyDescriptorValueSetToEventHandlerMethodName()
		{
			Assert.AreEqual("TestFormLoad", GetLoadEventPropertyDescriptor().GetValue(Form) as String);
		}
		
		[Test]
		public void PropertyDescriptorSetValueComponentIsForm()
		{
			Assert.AreEqual(Form, GetLoadEventPropertyDescriptor().GetSetValueComponent());
		}		
	}
}
