// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
