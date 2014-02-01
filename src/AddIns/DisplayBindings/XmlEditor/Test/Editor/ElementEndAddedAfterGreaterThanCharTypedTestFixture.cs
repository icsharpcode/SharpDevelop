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
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.NRefactory;
using ICSharpCode.NRefactory.Editor;
using ICSharpCode.SharpDevelop.Editor.AvalonEdit;
using ICSharpCode.XmlEditor;
using NUnit.Framework;
using XmlEditor.Tests.Utils;

namespace XmlEditor.Tests.Editor
{
	[TestFixture]
	public class ElementEndAddedAfterGreaterThanCharTypedTestFixture
	{
		XmlFormattingStrategy formattingStrategy;
		MockTextEditor textEditor;
		MockTextEditorOptions options;
		IDocument document;
		TextDocument textDocument;
		
		[SetUp]
		public void Init()
		{
			formattingStrategy = new XmlFormattingStrategy();
			
			options = new MockTextEditorOptions();
			textEditor = new MockTextEditor();
			textEditor.Options = options;
			
			textDocument = new TextDocument();
			document = textDocument;
			textEditor.SetDocument(document);
			
			document.Text = 
				"<root>\r\n" +
				"\t<child>\r\n" +
				"</root>\r\n";
			
			// Just typed the '>' character of the <child> element
			textEditor.Caret.Offset = 16; 
			formattingStrategy.FormatLine(textEditor, '>');
		}
		
		[Test]
		public void ChildEndElementAddedAfterGreaterThanCharTyped()
		{
			string expectedText = 
				"<root>\r\n" +
				"\t<child></child>\r\n" +
				"</root>\r\n";
			
			Assert.AreEqual(expectedText, document.Text);
		}
		
		[Test]
		public void IndentCanBeUndoneInOneStep()
		{
			string expectedText =
				"<root>\r\n" +
				"\t<child>\r\n" +
				"</root>\r\n";
			
			textDocument.UndoStack.Undo();
			
			Assert.AreEqual(expectedText, textDocument.Text);
		}
		
		[Test]
		public void CursorIsJustBeforeEndElementTagAfterLinesFormatted()
		{
			int newCaretOffset = 16;
			Assert.AreEqual(newCaretOffset, textEditor.Caret.Offset);
		}
	}
}
