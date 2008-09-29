// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
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
	/// Tests that Operator overrides have "End Operator" added after the user presses the return key.
	/// </summary>
	[TestFixture]
	public class EndInsertionTests
	{
		[TestFixtureSetUp]
		public void SetUpFixture()
		{
			if (!PropertyService.Initialized) {
				PropertyService.InitializeService(String.Empty, String.Empty, "VBNetBindingTests");
			}
		}

		[Test]
		public void EndSub()
		{
			string code = "Public Class Foo\r\n" +
				"\tPublic Sub Bar\r\n" +
				"\r\n" + // This extra new line is required. This is the new line just entered by the user.
				"End Class";
			
			string bar = "Bar";
			int cursorOffset = code.IndexOf(bar) + bar.Length;
			int line = 2;
			
			string expectedCode = "Public Class Foo\r\n" +
				"\tPublic Sub Bar\r\n" +
				"\t\t\r\n" +
				"\tEnd Sub\r\n" +
				"End Class";
			
			using (TextEditorControl editor = new TextEditorControl()) {
				editor.Document.TextContent = code;
				editor.ActiveTextAreaControl.Caret.Position = editor.Document.OffsetToPosition(cursorOffset);
				VBFormattingStrategy formattingStrategy = new VBFormattingStrategy();
				formattingStrategy.FormatLine(editor.ActiveTextAreaControl.TextArea, line, cursorOffset, '\n');
				
				Assert.AreEqual(expectedCode, editor.Document.TextContent);
			}
		}
		
		[Test]
		public void EndIf()
		{
			string code = "Public Class Foo\r\n" +
				"\tPublic Sub Bar\r\n" +
				"\t\tIf True Then\r\n" +
				"\r\n" + // This extra new line is required. This is the new line just entered by the user.
				"\tEnd Sub\r\n" +
				"End Class";
			
			string bar = "Then";
			int cursorOffset = code.IndexOf(bar) + bar.Length;
			int line = 3;
			
			string expectedCode = "Public Class Foo\r\n" +
				"\tPublic Sub Bar\r\n" +
				"\t\tIf True Then\r\n" +
				"\t\t\t\r\n" +
				"\t\tEnd If\r\n" +
				"\tEnd Sub\r\n" +
				"End Class";
			
			using (TextEditorControl editor = new TextEditorControl()) {
				editor.Document.TextContent = code;
				editor.ActiveTextAreaControl.Caret.Position = editor.Document.OffsetToPosition(cursorOffset);
				VBFormattingStrategy formattingStrategy = new VBFormattingStrategy();
				formattingStrategy.FormatLine(editor.ActiveTextAreaControl.TextArea, line, cursorOffset, '\n');
				
				Assert.AreEqual(expectedCode, editor.Document.TextContent);
			}
		}
		
		[Test]
		public void SingleLineIf()
		{
			string code = "Public Class Foo\r\n" +
				"\tPublic Sub Bar\r\n" +
				"\t\tIf True Then _\r\n" +
				"\r\n" + // This extra new line is required. This is the new line just entered by the user.
				"\tEnd Sub\r\n" +
				"End Class";
			
			string bar = "Then _";
			int cursorOffset = code.IndexOf(bar) + bar.Length;
			int line = 3;
			
			string expectedCode = "Public Class Foo\r\n" +
				"\tPublic Sub Bar\r\n" +
				"\t\tIf True Then _\r\n" +
				"\t\t\t\r\n" +
				"\tEnd Sub\r\n" +
				"End Class";
			
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
