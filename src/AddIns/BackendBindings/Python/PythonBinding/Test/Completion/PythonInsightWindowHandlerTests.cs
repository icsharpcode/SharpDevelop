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
using ICSharpCode.PythonBinding;
using ICSharpCode.Scripting.Tests.Utils;
using ICSharpCode.SharpDevelop.Editor;
using NUnit.Framework;
using PythonBinding.Tests.Utils;

namespace PythonBinding.Tests.Completion
{
	[TestFixture]
	public class PythonInsightWindowHandlerTests
	{
		PythonInsightWindowHandler handler;
		MockTextEditor fakeTextEditor;
		FakeInsightWindow fakeInsightWindow;
		
		void CreatePythonInsightWindowHandler()
		{
			fakeTextEditor = new MockTextEditor();
			fakeInsightWindow = new FakeInsightWindow();
			handler = new PythonInsightWindowHandler();
		}
		
		void InitializePythonInsightWindowHandler()
		{
			handler.InitializeOpenedInsightWindow(fakeTextEditor, fakeInsightWindow);
		}
		
		TextChangeEventArgs CreateInsertedTextChangeEventArgs(int offset, string insertedText)
		{
			return new TextChangeEventArgs(offset, String.Empty, insertedText);
		}
		
		[Test]
		public void InitializeOpenedInsightWindow_CloseParenthesisCharacterAddedToDocument_InsightWindowClosed()
		{
			CreatePythonInsightWindowHandler();
			fakeTextEditor.FakeDocument.Text = "method(";
			fakeTextEditor.FakeCaret.Offset = 7;
			fakeInsightWindow.StartOffset = 7;
			InitializePythonInsightWindowHandler();
			
			int newCaretOffset = 8;
			fakeTextEditor.FakeCaret.Offset = newCaretOffset;
			fakeTextEditor.FakeDocument.Text = "method()";
			TextChangeEventArgs e = CreateInsertedTextChangeEventArgs(newCaretOffset, ")");
			fakeInsightWindow.FireDocumentChangedEvent(e);
			
			bool closed = fakeInsightWindow.IsClosed;
			Assert.IsTrue(closed);
		}
		
		[Test]
		public void InitializeOpenedInsightWindow_MethodCallWithCursorAtOpenBracket_InsightWindowIsClosedBeforeDocumentIsChanged()
		{
			CreatePythonInsightWindowHandler();
			fakeTextEditor.FakeDocument.Text = "method(";
			fakeTextEditor.FakeCaret.Offset = 7;
			fakeInsightWindow.StartOffset = 7;
			InitializePythonInsightWindowHandler();
			
			bool closed = fakeInsightWindow.IsClosed;
			Assert.IsFalse(closed);
		}
		
		[Test]
		public void InitializeOpenedInsightWindow_SingleCharacterAddedToDocumentAfterOpenParenthesis_InsightWindowIsNotClosed()
		{
			CreatePythonInsightWindowHandler();
			fakeTextEditor.FakeDocument.Text = "method(";
			fakeTextEditor.FakeCaret.Offset = 7;
			fakeInsightWindow.StartOffset = 7;
			InitializePythonInsightWindowHandler();
			
			int newCaretOffset = 8;
			fakeTextEditor.FakeCaret.Offset = newCaretOffset;
			fakeTextEditor.FakeDocument.Text = "method(a";
			TextChangeEventArgs e = CreateInsertedTextChangeEventArgs(newCaretOffset, "a");
			fakeInsightWindow.FireDocumentChangedEvent(e);
			
			bool closed = fakeInsightWindow.IsClosed;
			Assert.IsFalse(closed);
		}
		
		[Test]
		public void InitializeOpenedInsightWindow_MethodCallInsideMethodCallAndCloseParenthesisCharacterAddedToDocument_InsightWindowIsNotClosed()
		{
			CreatePythonInsightWindowHandler();
			fakeTextEditor.FakeDocument.Text = "method(a(";
			fakeTextEditor.FakeCaret.Offset = 9;
			fakeInsightWindow.StartOffset = 7;
			InitializePythonInsightWindowHandler();
			
			int newCaretOffset = 10;
			fakeTextEditor.FakeCaret.Offset = newCaretOffset;
			fakeTextEditor.FakeDocument.Text = "method(a()";
			TextChangeEventArgs e = CreateInsertedTextChangeEventArgs(newCaretOffset, ")");
			fakeInsightWindow.FireDocumentChangedEvent(e);
			
			bool closed = fakeInsightWindow.IsClosed;
			Assert.IsFalse(closed);
		}
		
		[Test]
		public void InitializeOpenedInsightWindow_CharacterAddedToDocumentBeforeStartOfInsightWindow_InsightWindowClosed()
		{
			CreatePythonInsightWindowHandler();
			fakeTextEditor.FakeDocument.Text = "method(";
			fakeTextEditor.FakeCaret.Offset = 7;
			fakeInsightWindow.StartOffset = 7;
			InitializePythonInsightWindowHandler();
			
			int newCaretOffset = 1;
			fakeTextEditor.FakeCaret.Offset = newCaretOffset;
			fakeTextEditor.FakeDocument.Text = "aethod(";
			TextChangeEventArgs e = CreateInsertedTextChangeEventArgs(newCaretOffset, "a");
			fakeInsightWindow.FireDocumentChangedEvent(e);
			
			bool closed = fakeInsightWindow.IsClosed;
			Assert.IsTrue(closed);
		}
	}
}
