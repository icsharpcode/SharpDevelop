// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using ICSharpCode.PythonBinding;
using ICSharpCode.FormsDesigner;
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
			string eventHandler = "\tdef mybuttonclick(self, sender, e):\r\n" +
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
					"\t\tself._button1.Click += mybuttonclick\r\n" +
					"\t\r\n" +
					"\tdef mybuttonclick(self)\r\n" +
					"\t\tpass\r\n";
		}		
	}
}
