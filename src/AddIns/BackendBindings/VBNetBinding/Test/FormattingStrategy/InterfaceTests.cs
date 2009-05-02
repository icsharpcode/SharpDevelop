// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Siegfried Pammer" email="sie_pam@gmx.at"/>
//     <version>$Revision: 3548 $</version>
// </file>
using System;
using NUnit.Framework;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop;
using ICSharpCode.TextEditor;
using VBNetBinding;
using VBNetBinding.FormattingStrategy;

namespace VBNetBinding.Tests
{
	/// <summary>
	/// Tests the special case of an interface. for ex. no insertion of End-Tags etc.
	/// </summary>
	[TestFixture]
	public class InterfaceTests
	{
		[TestFixtureSetUp]
		public void SetUpFixture()
		{
			if (!PropertyService.Initialized) {
				PropertyService.InitializeService(String.Empty, String.Empty, "VBNetBindingTests");
			}
		}

		[Test]
		public void InterfaceEndSub()
		{
			string code = "Public Interface Foo\r\n" +
				"\tPublic Sub Bar\r\n" +
				"\r\n" + // This extra new line is required. This is the new line just entered by the user.
				"End Interface";
			
			string bar = "Sub Bar";
			int cursorOffset = code.IndexOf(bar) + bar.Length;
			int line = 2;
			
			string expectedCode = "Public Interface Foo\r\n" +
				"\tPublic Sub Bar\r\n" +
				"\t\r\n" +
				"End Interface";
			
			using (TextEditorControl editor = new TextEditorControl()) {
				editor.Document.TextContent = code;
				editor.ActiveTextAreaControl.Caret.Position = editor.Document.OffsetToPosition(cursorOffset);
				VBFormattingStrategy formattingStrategy = new VBFormattingStrategy();
				formattingStrategy.FormatLine(editor.ActiveTextAreaControl.TextArea, line, cursorOffset, '\n');
				
				Assert.AreEqual(expectedCode, editor.Document.TextContent);
			}
		}
	}
}
