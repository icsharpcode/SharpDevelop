// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.NRefactory;
using ICSharpCode.NRefactory.Editor;
using ICSharpCode.SharpDevelop.Editor.AvalonEdit;
using ICSharpCode.XmlEditor;
using NUnit.Framework;
using Rhino.Mocks;
using XmlEditor.Tests.Utils;

namespace XmlEditor.Tests.Editor
{
	[TestFixture]
	public class IndentChildElementEndTagAfterNewLineTypedTestFixture
	{
		XmlFormattingStrategy formattingStrategy;
		MockTextEditor textEditor;
		MockTextEditorOptions options;
		IDocumentLine docLine;
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
				"</child>\r\n" +
				"</root>\r\n";
			
			docLine = MockRepository.GenerateStub<IDocumentLine>();
			docLine.Stub(l => l.LineNumber).Return(3);
			formattingStrategy.IndentLine(textEditor, docLine);
		}
		
		[Test]
		public void ChildEndElementIndentedBySameLevelAsStartElementAfterNewLineTyped()
		{
			string expectedText = 
				"<root>\r\n" +
				"\t<child>\r\n" +
				"\t</child>\r\n" +
				"</root>\r\n";
			
			Assert.AreEqual(expectedText, document.Text);
		}
		
		[Test]
		public void IndentCanBeUndoneInOneStep()
		{
			string expectedText =
				"<root>\r\n" +
				"\t<child>\r\n" +
				"</child>\r\n" +
				"</root>\r\n";
			
			textDocument.UndoStack.Undo();
			
			Assert.AreEqual(expectedText, textDocument.Text);
		}
	}
}
