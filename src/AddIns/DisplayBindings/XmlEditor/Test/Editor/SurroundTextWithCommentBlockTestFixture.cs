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
	public class SurroundTextWithCommentBlockTestFixture
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
