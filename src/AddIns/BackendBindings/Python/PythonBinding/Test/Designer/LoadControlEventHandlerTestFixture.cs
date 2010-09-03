// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

using ICSharpCode.PythonBinding;
using ICSharpCode.Scripting.Tests.Utils;
using NUnit.Framework;
using PythonBinding.Tests.Utils;

namespace PythonBinding.Tests.Designer
{
	[TestFixture]
	public class LoadControlEventHandlerTestFixture : LoadFormTestFixtureBase
	{		
		public override string PythonCode {
			get {
				return "class TestForm(System.Windows.Forms.Form):\r\n" +
							"    def InitializeComponent(self):\r\n" +
							"        self._button2 = System.Windows.Forms.Button()\r\n" +
							"        self.SuspendLayout()\r\n" +
							"        # \r\n" +
							"        # button2\r\n" +
							"        # \r\n" +
							"        self._button2.Location = System.Drawing.Point(75, 39)\r\n" +
							"        self._button2.Name = \"button2\"\r\n" +
							"        self._button2.Size = System.Drawing.Size(75, 23)\r\n" +
							"        self._button2.TabIndex = 1\r\n" +
							"        self._button2.Text = \"button2\"\r\n" +
							"        self._button2.KeyDown += self.Button2KeyDown\r\n" +
							"        # \r\n" +
							"        # TestForm\r\n" +
							"        # \r\n" +					
							"        self.Name = \"TestForm\"\r\n" +
							"        self.Controls.Add(self._button2)\r\n" +
							"        self.ResumeLayout(False)\r\n";
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
