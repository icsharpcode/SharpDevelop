// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;

using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.NRefactory;
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
		AvalonEditDocumentAdapter document;
		TextDocument textDocument;
		
		[SetUp]
		public void Init()
		{
			formattingStrategy = new XmlFormattingStrategy();
			
			options = new MockTextEditorOptions();
			textEditor = new MockTextEditor();
			textEditor.Options = options;
			
			textDocument = new TextDocument();
			document = new AvalonEditDocumentAdapter(textDocument, null);
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
