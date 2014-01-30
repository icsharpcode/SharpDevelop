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
