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
	public class LoadControlEventHandlerTestFixture : LoadFormTestFixtureBase
	{		
		public override string RubyCode {
			get {
				return
					"class TestForm < System::Windows::Forms::Form\r\n" +
					"    def InitializeComponent()\r\n" +
					"        @button2 = System::Windows::Forms::Button.new()\r\n" +
					"        self.SuspendLayout()\r\n" +
					"        # \r\n" +
					"        # button2\r\n" +
					"        # \r\n" +
					"        @button2.Location = System::Drawing::Point.new(75, 39)\r\n" +
					"        @button2.Name = \"button2\"\r\n" +
					"        @button2.Size = System::Drawing::Size.new(75, 23)\r\n" +
					"        @button2.TabIndex = 1\r\n" +
					"        @button2.Text = \"button2\"\r\n" +
					"        @button2.KeyDown { |sender, e| self.Button2KeyDown(sender, e) }\r\n" +
					"        # \r\n" +
					"        # TestForm\r\n" +
					"        # \r\n" +					
					"        self.Name = \"TestForm\"\r\n" +
					"        self.Controls.Add(@button2)\r\n" +
					"        self.ResumeLayout(false)\r\n" +
					"    end\r\n" +
					"end";
			}
		}
		
		public override void BeforeSetUpFixture()
		{
			base.ComponentCreator.SetEventPropertyDescriptor(new MockPropertyDescriptor("abc", "Button2KeyDown", true));
		}
		
		public Button GetButton()
		{
			return Form.Controls[0] as Button;
		}
		
		public EventDescriptor GetButtonKeyDownEventDescriptor()
		{
			Button button = GetButton();
			return TypeDescriptor.GetEvents(button).Find("KeyDown", true);
		}
		
		public MockPropertyDescriptor GetButtonKeyDownEventPropertyDescriptor()
		{
			EventDescriptor eventDescriptor = GetButtonKeyDownEventDescriptor();
			return base.ComponentCreator.GetEventProperty(eventDescriptor) as MockPropertyDescriptor;			
		}		
		
		[Test]
		public void PropertyDescriptorSetValueComponentIsForm()
		{
			Assert.AreEqual(GetButton(), GetButtonKeyDownEventPropertyDescriptor().GetSetValueComponent());
		}
	}
}
