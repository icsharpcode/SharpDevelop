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
using System.Collections.Generic;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Editor;
using ICSharpCode.SharpDevelop.Editor.CodeCompletion;
using ICSharpCode.XmlEditor;
using NUnit.Framework;
using XmlEditor.Tests.Utils;

namespace XmlEditor.Tests.Utils.Tests
{
	[TestFixture]
	public class MockTextEditorTests
	{
		MockTextEditor editor;

		[SetUp]
		public void Init()
		{
			editor = new MockTextEditor();
		}
			
		[Test]
		public void CanCheckCompletionWindowFromShowCompletionHasWidthPropertyModified()
		{
			XmlCompletionItemCollection list = new XmlCompletionItemCollection();
			list.Add(new XmlCompletionItem("a"));
			ICompletionListWindow window = editor.ShowCompletionWindow(list);
			window.Width = double.NaN;
			
			Assert.AreEqual(double.NaN, editor.CompletionWindowDisplayed.Width);
		}
		
		[Test]
		public void CanGetCompletionItemsUsedAsShowCompletionMethodParameters()
		{
			XmlCompletionItemCollection list = new XmlCompletionItemCollection();
			list.Add(new XmlCompletionItem("a"));
			editor.ShowCompletionWindow(list);

			Assert.AreSame(list, editor.CompletionItemsDisplayed);
		}
		
		[Test]
		public void CanConvertCompletionItemsUsedAsShowCompletionMethodParametersToArray()
		{
			XmlCompletionItem item = new XmlCompletionItem("a");
			List<XmlCompletionItem> expectedArray = new List<XmlCompletionItem>();
			expectedArray.Add(item);
			
			XmlCompletionItemCollection list = new XmlCompletionItemCollection();
			list.Add(item);
			editor.ShowCompletionWindow(list);

			Assert.AreEqual(expectedArray.ToArray(), editor.CompletionItemsDisplayedToArray());
		}
		
		[Test]
		public void TextEditorDocumentCanGetAndSetText()
		{
			editor.Document.Text = "test";
			Assert.AreEqual("test", editor.Document.Text);
		}
		
		[Test]
		public void TextEditorCaretCanGetAndSetCaretOffset()
		{
			editor.Caret.Offset = 5;
			Assert.AreEqual(5, editor.Caret.Offset);
		}
		
		[Test]
		public void TextEditorMockDocumentPropertyIsSameAsTextEditorDocumentProperty()
		{
			Assert.AreSame(editor.MockDocument, editor.Document);
		}
		
		[Test]
		public void TextEditorCanGetAndSetFileName()
		{
			editor.FileName = new FileName(@"c:\projects\a.xml");
			Assert.AreEqual(@"c:\projects\a.xml", editor.FileName.ToString());
		}
		
		[Test]
		public void TextEditorShowCompletionListReturnsNullWhenConfigured()
		{
			XmlCompletionItemCollection list = new XmlCompletionItemCollection();
			list.Add(new XmlCompletionItem("a"));
			
			editor.ShowCompletionWindowReturnsNull = true;
			
			Assert.IsNull(editor.ShowCompletionWindow(list));
		}
		
		[Test]
		public void TextEditorShowCompletionListIsCalledReturnsTrueIfShowCompletionListMethodReturnsNullWindow()
		{
			TextEditorShowCompletionListReturnsNullWhenConfigured();
			Assert.IsTrue(editor.IsShowCompletionWindowMethodCalled);
		}
		
		[Test]
		public void IsShowCompletionWindowMethodCalledReturnsFalseByDefault()
		{
			Assert.IsFalse(editor.IsShowCompletionWindowMethodCalled);
		}
		
		[Test]
		public void CanSetAndGetTextEditorOptions()
		{
			MockTextEditorOptions options = new MockTextEditorOptions();
			editor.Options = options;
			Assert.AreSame(options, editor.Options);
		}
		
		[Test]
		public void TextEditorHasNonNullTextEditorOptionsWhenCreated()
		{
			Assert.IsNotNull(editor.Options);
		}
		
		[Test]
		public void CanReplaceDocumentObjectUsed()
		{
			MockDocument doc = new MockDocument();
			editor.SetDocument(doc);
			Assert.AreSame(doc, editor.Document);
		}
		
		[Test]
		public void CanGetAndSetSelectionStart()
		{
			editor.Document.Text = "abc";
			int selectionStart = 1;
			int selectionLength = 2;
			editor.Select(selectionStart, selectionLength);
			
			Assert.AreEqual(selectionStart, editor.SelectionStart);
		}
		
		[Test]
		public void CanGetAndSetSelectionLength()
		{
			editor.Document.Text = "abc";
			int selectionStart = 1;
			int selectionLength = 2;
			editor.Select(selectionStart, selectionLength);
			
			Assert.AreEqual(selectionLength, editor.SelectionLength);
		}
		
		[Test]
		public void SelectedTextPropertyReturnsSelectedText()
		{
			editor.Document.Text = "abc";
			int selectionStart = 1;
			int selectionLength = 2;
			editor.Select(selectionStart, selectionLength);
			
			Assert.AreEqual("bc", editor.SelectedText);
		}
	}
}
