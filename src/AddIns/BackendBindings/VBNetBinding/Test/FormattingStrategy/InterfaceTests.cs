// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using ICSharpCode.AvalonEdit;
using System;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Editor.AvalonEdit;
using NUnit.Framework;

namespace ICSharpCode.VBNetBinding.Tests
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
			
			string bar = "Sub Bar\r\n";
			int cursorOffset = code.IndexOf(bar) + bar.Length;
			
			string expectedCode = "Public Interface Foo\r\n" +
				"\tPublic Sub Bar\r\n" +
				"\t\r\n" +
				"End Interface";
			
			int expectedOffset = ("Public Interface Foo\r\n" +
				"\tPublic Sub Bar\r\n" +
				"\t").Length;
			
			AvalonEditTextEditorAdapter editor = new AvalonEditTextEditorAdapter(new TextEditor());
			editor.Document.Text = code;
			editor.Caret.Offset = cursorOffset;
			VBNetFormattingStrategy formattingStrategy = new VBNetFormattingStrategy();
			formattingStrategy.FormatLine(editor, '\n');
			
			Assert.AreEqual(expectedCode, editor.Document.Text);
			Assert.AreEqual(expectedOffset, editor.Caret.Offset);
		}
	}
}
