// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
	/// Tests the PythonDesignerGenerator adds an extra new line between the previous event handler
	/// and the new one inserted.
	/// </summary>
	[TestFixture]
	public class InsertSecondEventHandlerTestFixture : InsertEventHandlerTestFixtureBase
	{
		public override void AfterSetUpFixture()
		{						
			MockEventDescriptor mockEventDescriptor = new MockEventDescriptor("Click");
			generator.InsertComponentEvent(null, mockEventDescriptor, "button1_click", String.Empty, out file, out position);
			insertedEventHandler = generator.InsertComponentEvent(null, mockEventDescriptor, "button2_click", String.Empty, out file, out position);
		}
		
		[Test]
		public void ExpectedCodeAfterEventHandlerInserted()
		{
			string expectedCode = GetTextEditorCode();			
			string eventHandler = "\tdef button1_click(self, sender, e):\r\n" +
								"\t\tpass\r\n" +
								"\r\n" +
								"\tdef button2_click(self, sender, e):\r\n" +
								"\t\tpass";
			expectedCode = expectedCode + "\r\n" + eventHandler;
			
			Assert.AreEqual(expectedCode, viewContent.DesignerCodeFileContent);
		}
		
		protected override string GetTextEditorCode()
		{
			return "from System.Windows.Forms import Form\r\n" +
					"\r\n" +
					"class MainForm(Form):\r\n" +
					"\tdef __init__(self):\r\n" +
					"\t\tself.InitializeComponents()\r\n" +
					"\t\r\n" +
					"\tdef InitializeComponents(self):\r\n" +
					"\t\tself._button1 = System.Windows.Forms.Button()\r\n" +
					"\t\tself.Controls.Add(self._button1)\r\n";
		}
	}
}
