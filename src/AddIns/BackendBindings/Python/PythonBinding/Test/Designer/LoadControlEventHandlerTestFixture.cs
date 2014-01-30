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
