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
	/// Tests the RubyDesignerGenerator does not insert an event handler if a method already exists with the same
	/// name.
	/// </summary>
	[TestFixture]
	public class EventHandlerAlreadyExistsTestFixture : InsertEventHandlerTestFixtureBase
	{		
		public override void AfterSetUpFixture()
		{						
			MockEventDescriptor mockEventDescriptor = new MockEventDescriptor("Click");
			generator.InsertComponentEvent(null, mockEventDescriptor, "mybuttonclick", String.Empty, out file, out position);
			insertedEventHandler = generator.InsertComponentEvent(null, mockEventDescriptor, "mybuttonclick", String.Empty, out file, out position);
		}
		
		[Test]
		public void CodeAfterInsertComponentEventMethodCalledIsNotChanged()
		{
			string expectedCode = GetTextEditorCode();			
			Assert.AreEqual(expectedCode, viewContent.DesignerCodeFileContent);
		}
		
		[Test]
		public void InsertComponentEventMethodReturnsTrue()
		{
			Assert.IsTrue(insertedEventHandler);
		}
		
		[Test]
		public void FileIsForm()
		{
			Assert.AreEqual(fileName, file);
		}
		
		[Test]
		public void PositionOfEventHandlerIsLine12()
		{
			Assert.AreEqual(12, position);
		}
			
		protected override string GetTextEditorCode()
		{
			return "class MainForm < System::Windows::Forms::Form\r\n" +
					"\tdef initialize()\r\n" +
					"\t\tself.InitializeComponents()\r\n" +
					"\tend\r\n" +
					"\t\r\n" +
					"\tdef InitializeComponents()\r\n" +
					"\t\t@button1 = System::Windows::Forms::Button.new()\r\n" +
					"\t\t@button1.Click { |sender, e| self.mybuttonclick() }\r\n" +
					"\t\tself.Controls.Add(@button1)\r\n" +
					"\tend\r\n" +
					"\t\r\n" +
					"\tdef mybuttonclick(sender, e)\r\n" +
					"\t\t\r\n" +
					"\tend\r\n" +
					"end";
		}
	}
}
