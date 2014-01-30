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
using System.Drawing;
using System.IO;
using System.Windows.Forms;

using ICSharpCode.PythonBinding;
using ICSharpCode.Scripting.Tests.Utils;
using NUnit.Framework;
using PythonBinding.Tests.Utils;

namespace PythonBinding.Tests.Designer
{
	/// <summary>
	/// Fix ArgumentNullException when form has a button followed by a timer and the
	/// first timer statement is the setting of the event handler.
	/// </summary>
	[TestFixture]
	public class LoadFormWithButtonFollowedByTimerTestFixture : LoadFormTestFixtureBase
	{
		MockPropertyDescriptor mockTickPropertyDescriptor;
		
		public override string PythonCode {
			get {
				return
					"import System.Drawing\r\n" +
					"import System.Windows.Forms\r\n" +
					"\r\n" +
					"from System.Drawing import *\r\n" +
					"from System.Windows.Forms import *\r\n" +
					"\r\n" +
					"class MainForm(Form):\r\n" +
					"    def __init__(self):\r\n" +
					"        self.InitializeComponent()\r\n" +
					"\r\n" +
					"    def InitializeComponent(self):\r\n" +
					"        self._components = System.ComponentModel.Container()\r\n" +
					"        self._button1 = System.Windows.Forms.Button()\r\n" +
					"        self._timer1 = System.Windows.Forms.Timer(self._components)\r\n" +
					"        self.SuspendLayout()\r\n" +
					"        # \r\n" +
					"        # button1\r\n" +
					"        # \r\n" +
					"        self._button1.Location = System.Drawing.Point(37, 29)\r\n" +
					"        self._button1.Name = \"button1\"\r\n" +
					"        self._button1.Size = System.Drawing.Size(75, 23)\r\n" +
					"        self._button1.TabIndex = 0\r\n" +
					"        self._button1.Text = \"button1\"\r\n" +
					"        self._button1.UseVisualStyleBackColor = True\r\n" +
					"        # \r\n" +
					"        # timer1\r\n" +
					"        # \r\n" +
					"        self._timer1.Tick += self.Timer1Tick\r\n" +
					"        # \r\n" +
					"        # MainForm\r\n" +
					"        # \r\n" +
					"        self.ClientSize = System.Drawing.Size(284, 264)\r\n" +
					"        self.Controls.Add(self._button1)\r\n" +
					"        self.Name = \"MainForm\"\r\n" +
					"        self.Text = \"PyWinTest\"\r\n" +
					"        self.ResumeLayout(False)\r\n" +
					"\r\n" +
					"\r\n" +
					"    def Timer1Tick(self, sender, e):\r\n" +
					"        pass";
			}
		}
		
		public override void BeforeSetUpFixture()
		{
			mockTickPropertyDescriptor = new MockPropertyDescriptor("Tick", null, false);
			ComponentCreator.SetEventPropertyDescriptor(mockTickPropertyDescriptor);
		}
		
		[Test]
		public void EventDescriptorUsedToGetEventProperty()
		{
			Assert.IsNotNull(ComponentCreator.EventDescriptorPassedToGetEventProperty);
		}
		
		[Test]
		public void EventDescriptorNameUsedToGetEventPropertyIsTick()
		{
			Assert.AreEqual("Tick", ComponentCreator.EventDescriptorPassedToGetEventProperty.Name);
		}
		
		[Test]
		public void GetPropertyValueSetForTimer()
		{
			string value = mockTickPropertyDescriptor.GetValue(null) as String;
			Assert.AreEqual("Timer1Tick", value);
		}
		
		[Test]
		public void GetComponentUsedWhenSettingTickHandler()
		{
			IComponent expectedComponent = ComponentCreator.GetComponent("timer1");
			
			object component = mockTickPropertyDescriptor.GetSetValueComponent();
			
			Assert.AreEqual(expectedComponent, component);
		}
	}
}
