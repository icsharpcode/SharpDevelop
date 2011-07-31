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
			
		protected override string GetTextEditorCode()
		{
			return
				"from System.Windows.Forms import Form\r\n" +
				"\r\n" +
				"class MainForm(Form):\r\n" +
				"    def __init__(self):\r\n" +
				"        self.InitializeComponents()\r\n" +
				"    \r\n" +
				"    def InitializeComponents(self):\r\n" +
				"        self._button1 = System.Windows.Forms.Button()\r\n" +
				"        self._button1.Click += mybuttonclick\r\n" +
				"        self.Controls.Add(self._button1)\r\n" +
				"    \r\n" +
				"    def mybuttonclick(self, sender, e):\r\n" +
				"        pass\r\n";
		}
	}
}
