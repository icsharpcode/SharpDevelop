// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.FormsDesigner;
using ICSharpCode.RubyBinding;
using ICSharpCode.Scripting.Tests.Utils;
using ICSharpCode.SharpDevelop.Dom;
using NUnit.Framework;
using RubyBinding.Tests.Utils;

namespace RubyBinding.Tests.Designer
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
			string expectedCode =
				"require \"System.Windows.Forms\"\r\n" +
				"\r\n" +
				"class MainForm < Form\r\n" +
				"\tdef initialize()\r\n" +
				"\t\tself.InitializeComponents()\r\n" +
				"\tend\r\n" +
				"\t\r\n" +
				"\tdef InitializeComponents()\r\n" +
				"\t\t@button1 = System::Windows::Forms::Button.new()\r\n" +
				"\t\t@button1.Click { self.mybuttonclick() }\r\n" +
				"\tend\r\n" +
				"\t\r\n" +
				"\tdef mybuttonclick()\r\n" +
				"\tend\r\n" +
				"\r\n" +
				"\tdef mybuttonclick(sender, e)\r\n" +
				"\t\t\r\n" +
				"\tend\r\n" +
				"end";
			
			Assert.AreEqual(expectedCode, viewContent.DesignerCodeFileContent, viewContent.DesignerCodeFileContent);
		}
		
		protected override string GetTextEditorCode()
		{
			return
				"require \"System.Windows.Forms\"\r\n" +
				"\r\n" +
				"class MainForm < Form\r\n" +
				"\tdef initialize()\r\n" +
				"\t\tself.InitializeComponents()\r\n" +
				"\tend\r\n" +
				"\t\r\n" +
				"\tdef InitializeComponents()\r\n" +
				"\t\t@button1 = System::Windows::Forms::Button.new()\r\n" +
				"\t\t@button1.Click { self.mybuttonclick() }\r\n" +
				"\tend\r\n" +
				"\t\r\n" +
				"\tdef mybuttonclick()\r\n" +
				"\tend\r\n" +
				"end";
		}		
	}
}
