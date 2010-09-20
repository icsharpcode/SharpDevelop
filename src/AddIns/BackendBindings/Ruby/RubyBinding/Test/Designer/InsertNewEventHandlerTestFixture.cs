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
	/// Tests the RubyDesignerGenerator inserts an event handler
	/// into the document correctly.
	/// </summary>
	[TestFixture]
	public class InsertNewEventHandlerTestFixture : InsertEventHandlerTestFixtureBase
	{
		public override void AfterSetUpFixture()
		{
			MockEventDescriptor mockEventDescriptor = new MockEventDescriptor("Click");
			insertedEventHandler = generator.InsertComponentEvent(null, mockEventDescriptor, "button1_click", String.Empty, out file, out position);
		}
		
		[Test]
		public void ExpectedCodeAfterEventHandlerInserted()
		{
			string expectedCode =
				"class MainForm < System::Windows::Forms::Form\r\n" +
				"\tdef initialize()\r\n" +
				"\t\tself.InitializeComponents()\r\n" +
				"\tend\r\n" +
				"\tdef InitializeComponents()\r\n" +
				"\t\t@button1 = System::Windows::Forms::Button.new()\r\n" +
				"\t\tself.Controls.Add(@button1)\r\n" +
				"\tend\r\n" +
				"\r\n" +
				"\tdef button1_click(sender, e)\r\n" +
				"\t\t\r\n" +
				"\tend\r\n" +
				"end\r\n";
			
			Assert.AreEqual(expectedCode, viewContent.DesignerCodeFileContent, viewContent.DesignerCodeFileContent);
		}
		
		protected override string GetTextEditorCode()
		{
			return
				"class MainForm < System::Windows::Forms::Form\r\n" +
				"\tdef initialize()\r\n" +
				"\t\tself.InitializeComponents()\r\n" +
				"\tend\r\n" +
				"\tdef InitializeComponents()\r\n" +
				"\t\t@button1 = System::Windows::Forms::Button.new()\r\n" +
				"\t\tself.Controls.Add(@button1)\r\n" +
				"\tend\r\n" +
				"end\r\n";
		}
		
		/// <summary>
		/// The generator should call the FormDesignerViewContent's
		/// MergeFormChanges method to make sure the latest version of the
		/// code is in the text editor.
		/// </summary>
		[Test]
		public void MergeFormChangesCalled()
		{
			Assert.IsTrue(viewContent.MergeFormChangesCalled);
		}
		
		[Test]
		public void InsertedEventHandlerReturnedTrue()
		{
			Assert.IsTrue(insertedEventHandler);
		}
		
		[Test]
		public void PositionEventHandlerInserted()
		{
			// The position is actually a line number. So 
			// it should be set to line 9.
			Assert.AreEqual(9, position);
		}
		
		[Test]
		public void FileNameSameAsDocumentFileName()
		{
			Assert.AreEqual(viewContent.DesignerCodeFile.FileName.ToString(), file);
		}
	}
}
