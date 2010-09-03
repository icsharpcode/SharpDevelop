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
	public class NoElementEndAddedAfterGreaterThanCharTypedTests
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
		}
		
		[Test]
		public void TextNotChangedWhenGreaterThanCharTypedAndChildEndElementAlreadyExists()
		{
			string text = 
				"<root>\r\n" +
				"\t<child></child>\r\n" +
				"</root>\r\n";
			document.Text = text;
			
			// Just typed the '>' character of the <child> element
			textEditor.Caret.Offset = 16; 
			formattingStrategy.FormatLine(textEditor, '>');
			
			Assert.AreEqual(text, document.Text);
		}
		
		[Test]
		public void TextNotChangedWhenGreaterThanCharTypedAndChildElementIsEmptyElement()
		{
			string text = 
				"<root>\r\n" +
				"\t<child/>\r\n" +
				"</root>\r\n";
			document.Text = text;
			
			// Just typed the '>' character of the <child/> element
			textEditor.Caret.Offset = 17; 
			formattingStrategy.FormatLine(textEditor, '>');
			
			Assert.AreEqual(text, document.Text);
		}
		
		[Test]
		public void TextNotChangedWhenGreaterThanCharTypedForCommentTag()
		{
			string text = "<!-- a -->";
			document.Text = text;
			
			// Just typed the '>' character of the comment tag
			textEditor.Caret.Offset = 10; 
			formattingStrategy.FormatLine(textEditor, '>');
			
			Assert.AreEqual(text, document.Text);
		}
		
		[Test]
		public void TextNotChangedWhenGreaterThanCharTypedForPreProcessingInstruction()
		{
			string text = "<?xml version=\"1.0\"?>";
			document.Text = text;
			
			// Just typed the '>' character of the pre-processing instruction
			textEditor.Caret.Offset = 21; 
			formattingStrategy.FormatLine(textEditor, '>');
			
			Assert.AreEqual(text, document.Text);
		}
	}
}
