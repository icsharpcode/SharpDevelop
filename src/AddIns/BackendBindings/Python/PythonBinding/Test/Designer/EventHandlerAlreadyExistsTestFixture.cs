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
	/// Tests the PythonDesignerGenerator does not insert an event handler if a method already exists with the same
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
		
		[Test]
		public void TextPassedToParseFileMethod()
		{
			Assert.AreEqual(GetTextEditorCode(), generator.TextContentPassedToParseFileMethod);
		}
		
		[Test]
		public void FileNamePassedToParseFileMethod()
		{
			Assert.AreEqual(fileName, generator.FileNamePassedToParseFileMethod);
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
					"\t\tself.Controls.Add(self._button1)\r\n" +
					"\t\r\n" +
					"\tdef mybuttonclick(self, sender, e)\r\n" +
					"\t\tpass\r\n";
		}
	}
}
