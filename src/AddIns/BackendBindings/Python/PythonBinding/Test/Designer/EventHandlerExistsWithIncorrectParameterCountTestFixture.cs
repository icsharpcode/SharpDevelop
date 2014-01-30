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
using ICSharpCode.FormsDesigner;
using ICSharpCode.PythonBinding;
using ICSharpCode.Scripting.Tests.Utils;
using ICSharpCode.SharpDevelop.Dom;
using NUnit.Framework;
using PythonBinding.Tests.Utils;

namespace PythonBinding.Tests.Designer
{
	/// <summary>
	/// An event handler should be inserted if a method exists in the form's class but has the incorrect
	/// number of parameters.
	/// </summary>
	[TestFixture]
	public class EventHandlerExistsWithIncorrectParameterCountTestFixture : InsertEventHandlerTestFixtureBase
	{
		public override void AfterSetUpFixture()
		{						
			MockEventDescriptor mockEventDescriptor = new MockEventDescriptor("Click");
			insertedEventHandler = generator.InsertComponentEvent(null, mockEventDescriptor, "mybuttonclick", String.Empty, out file, out position);
		}

		[Test]
		public void ExpectedCodeAfterEventHandlerInserted()
		{
			string expectedCode = GetTextEditorCode();			
			string eventHandler =
				"\tdef mybuttonclick(self, sender, e):\r\n" +
				"\t\tpass";
			expectedCode = expectedCode + "\r\n" + eventHandler;
			
			Assert.AreEqual(expectedCode, viewContent.DesignerCodeFileContent);
		}
		
		protected override string GetTextEditorCode()
		{
			return
				"from System.Windows.Forms import Form\r\n" +
				"\r\n" +
				"class MainForm(Form):\r\n" +
				"\tdef __init__(self):\r\n" +
				"\t\tself.InitializeComponents()\r\n" +
				"\t\r\n" +
				"\tdef InitializeComponents(self):\r\n" +
				"\t\tself._button1 = System.Windows.Forms.Button()\r\n" +
				"\t\tself._button1.Click += mybuttonclick\r\n" +
				"\t\r\n" +
				"\tdef mybuttonclick(self):\r\n" +
				"\t\tpass\r\n";
		}		
	}
}
