// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.NRefactory;
using ICSharpCode.SharpDevelop.Editor.AvalonEdit;
using ICSharpCode.XmlEditor;
using NUnit.Framework;
using XmlEditor.Tests.Utils;

namespace XmlEditor.Tests.Editor
{
	[TestFixture]
	public class SurroundTextWithCommentBlockTestFixture
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
			
			textDocument.Text = 
				"<root>\r\n" +
				"\t<child></child>\r\n" +
				"</root>";
			
			int selectionStart = 9;
			int selectionLength = 15;
			textEditor.Select(selectionStart, selectionLength);
			
			formattingStrategy.SurroundSelectionWithComment(textEditor);
		}
		
		[Test]
		public void ChildElementSurroundedByCommentTags()
		{
			string expectedText = 
				"<root>\r\n" +
				"\t<!--<child></child>-->\r\n" +
				"</root>";
			Assert.AreEqual(expectedText, document.Text);
		}
		
		[Test]
		public void CallingSurroundSelectionWithCommentRemovesCommentTagsWhenCommentTagsExist()
		{
			int selectionStart = 14;
			int selectionLength = 2;
			textEditor.Select(selectionStart, selectionLength);
			formattingStrategy.SurroundSelectionWithComment(textEditor);
			
			string expectedText = 
				"<root>\r\n" +
				"\t<child></child>\r\n" +
				"</root>";
			
			Assert.AreEqual(expectedText, document.Text);
		}
	}
}
