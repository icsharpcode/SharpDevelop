// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

using ICSharpCode.PythonBinding;
using NUnit.Framework;
using PythonBinding.Tests.Utils;

namespace PythonBinding.Tests.Designer
{
	[TestFixture]
	public class LoadEventHandlerTestFixture : LoadFormTestFixtureBase
	{		
		public override string PythonCode {
			get {
				return "class TestForm(System.Windows.Forms.Form):\r\n" +
							"    def InitializeComponent(self):\r\n" +
							"        self.SuspendLayout()\r\n" +
							"        # \r\n" +
							"        # TestForm\r\n" +
							"        # \r\n" +
							"        self.Name = \"TestForm\"\r\n" +
							"        self.Load += self.TestFormLoad\r\n" +
							"        self.ResumeLayout(False)\r\n";
			}
		}
		
		public EventDescriptor GetLoadEventDescriptor()
		{
			return TypeDescriptor.GetEvents(Form).Find("Load", true);
		}
		
		[Test]
		public void EventPropertyDescriptorValueSetToEventHandlerMethodName()
		{
			EventDescriptor loadEventDescriptor = GetLoadEventDescriptor();
			PropertyDescriptor property = base.GetEventProperty(loadEventDescriptor);
			Assert.AreEqual("TestFormLoad", property.GetValue(Form) as String);
		}
	}
}
