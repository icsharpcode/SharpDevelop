// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
