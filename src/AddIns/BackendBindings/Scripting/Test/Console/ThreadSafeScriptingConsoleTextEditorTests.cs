// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows.Input;

using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.Editing;
using ICSharpCode.Scripting;
using ICSharpCode.Scripting.Tests.Utils;
using NUnit.Framework;

namespace ICSharpCode.Scripting.Tests.Console
{
	[TestFixture]
	public class ThreadSafeScriptingConsoleTextEditorTests
	{
		ThreadSafeScriptingConsoleTextEditor threadSafeConsoleTextEditor;
		TextEditor avalonEditTextEditor;
		MockControlDispatcher dispatcher;
		MockConsoleTextEditor fakeConsoleTextEditor;
		ConsoleTextEditorKeyEventArgs actualKeyEventArgs;
		
		[SetUp]
		public void Init()
		{
			avalonEditTextEditor = new TextEditor();
			dispatcher = new MockControlDispatcher();
			dispatcher.CheckAccessReturnValue = true;
			
			fakeConsoleTextEditor = new MockConsoleTextEditor();
			threadSafeConsoleTextEditor = new ThreadSafeScriptingConsoleTextEditor(fakeConsoleTextEditor, dispatcher);
		}
		
		[Test]
		public void PreviewKeyDown_PreviewKeyDownFiresInWrappedNonThreadSafeConsoleTextEditor_CausesPreviewKeyDownEventToFireInThreadSafeConsoleTextEditor()
		{
			threadSafeConsoleTextEditor.PreviewKeyDown += ThreadSafeConsoleTextEditorPreviewKeyDown;
			MockConsoleTextEditorKeyEventArgs expectedKeyEventArgs = new MockConsoleTextEditorKeyEventArgs(Key.OemPeriod);
			fakeConsoleTextEditor.RaisePreviewKeyDownEvent(expectedKeyEventArgs);
						
			Assert.AreEqual(expectedKeyEventArgs, actualKeyEventArgs);
		}
		
		void ThreadSafeConsoleTextEditorPreviewKeyDown(object source, ConsoleTextEditorKeyEventArgs e)
		{
			actualKeyEventArgs = e;
		}
		
		[Test]
		public void PreviewKeyDown_PreviewKeyDownFiresInWrappedNonThreadSafeConsoleTextEditorAfterEventHandlerRemoved_DoesNotFirePreviewKeyDownEventinThreadSafeConsoleTextEditor()
		{			
			actualKeyEventArgs = null;
			threadSafeConsoleTextEditor.PreviewKeyDown += ThreadSafeConsoleTextEditorPreviewKeyDown;
			threadSafeConsoleTextEditor.PreviewKeyDown -= ThreadSafeConsoleTextEditorPreviewKeyDown;
			MockConsoleTextEditorKeyEventArgs expectedKeyEventArgs = new MockConsoleTextEditorKeyEventArgs(Key.OemPeriod);
			fakeConsoleTextEditor.RaisePreviewKeyDownEvent(expectedKeyEventArgs);
			
			Assert.IsNull(actualKeyEventArgs);
		}
		
		[Test]
		public void Dispose_WrappedNonThreadSafeConsoleTextEditor_DisposesWrappedNonThreadSafeConsoleTextEditor()
		{
			threadSafeConsoleTextEditor.Dispose();
			bool disposed = fakeConsoleTextEditor.IsDisposed;
			Assert.IsTrue(disposed);
		}
		
		[Test]
		public void Column_WrappedConsoleTextEditorColumnIsThree_ReturnsThree()
		{
			fakeConsoleTextEditor.Column = 3;
			int column = threadSafeConsoleTextEditor.Column;
			int expectedColumn = 3;
			
			Assert.AreEqual(expectedColumn, column);
		}
		
		[Test]
		public void Column_SettingColumnToFour_SetsWrappedConsoleTextEditorColumnToFour()
		{
			threadSafeConsoleTextEditor.Column = 4;
			int column = fakeConsoleTextEditor.Column;
			int expectedColumn = 4;
			
			Assert.AreEqual(expectedColumn, column);
		}
		
		[Test]
		public void SelectionStart_WrappedConsoleTextEditorSelectionStartIsThree_ReturnsThree()
		{
			fakeConsoleTextEditor.SelectionStart = 3;
			int index = threadSafeConsoleTextEditor.SelectionStart;
			int expectedIndex = 3;
			
			Assert.AreEqual(expectedIndex, index);
		}
		
		[Test]
		public void SelectionLength_WrappedConsoleTextEditorSelectionLengthIsThree_ReturnsThree()
		{
			fakeConsoleTextEditor.SelectionLength = 3;
			int length = threadSafeConsoleTextEditor.SelectionLength;
			int expectedLength = 3;
			
			Assert.AreEqual(expectedLength, length);
		}
		
		[Test]
		public void Line_WrappedConsoleTextEditorLineIsTwo_ReturnsTwo()
		{
			fakeConsoleTextEditor.Line = 2;
			int line = threadSafeConsoleTextEditor.Line;
			int expectedLine = 2;
			
			Assert.AreEqual(expectedLine, line);
		}
		
		[Test]
		public void Line_SettingLineToOne_SetsWrappedConsoleTextEditorLineToOne()
		{
			threadSafeConsoleTextEditor.Line = 1;
			int line = fakeConsoleTextEditor.Line;
			int expectedLine = 1;
			
			Assert.AreEqual(expectedLine, line);
		}

		[Test]
		public void TotalLines_WrappedConsoleTextEditorTotalLinesIsOne_ReturnsOne()
		{
			fakeConsoleTextEditor.TotalLines = 1;
			int total = threadSafeConsoleTextEditor.TotalLines;
			int expectedTotal = 1;
			
			Assert.AreEqual(expectedTotal, total);
		}
		
		[Test]
		public void IsCompletionWindowDisplayed_WrappedConsoleTextEditorIsCompletionWindowDisplayedIsTrue_ReturnsTrue()
		{
			fakeConsoleTextEditor.IsCompletionWindowDisplayed = true;
			bool result = threadSafeConsoleTextEditor.IsCompletionWindowDisplayed;
			
			Assert.IsTrue(result);
		}
		
		[Test]
		public void MakeCurrentContentReadOnly_WrappedConsoleTextEditor_CallsWrappedConsoleTextEditorMethod()
		{
			threadSafeConsoleTextEditor.MakeCurrentContentReadOnly();
			Assert.IsTrue(fakeConsoleTextEditor.IsMakeCurrentContentReadOnlyCalled);
		}
		
		[Test]
		public void ShowCompletionWindow_WrappedConsoleTextEditor_CallsWrappedConsoleTextEditorMethod()
		{
			ScriptingConsoleCompletionDataProvider expectedProvider = new ScriptingConsoleCompletionDataProvider(null);
			threadSafeConsoleTextEditor.ShowCompletionWindow(expectedProvider);
			
			ScriptingConsoleCompletionDataProvider provider = fakeConsoleTextEditor.CompletionProviderPassedToShowCompletionWindow;
			
			Assert.AreEqual(expectedProvider, provider);
		}
		
		[Test]
		public void Write_WrappedConsoleTextEditor_WritesTextToWrappedConsoleTextEditor()
		{
			threadSafeConsoleTextEditor.Write("abc");
			string text = fakeConsoleTextEditor.TextPassedToWrite;
			string expectedText = "abc";
			
			Assert.AreEqual(expectedText, text);
		}
		
		[Test]
		public void GetLine_GetFirstLine_GetsFirstLineFromWrappedConsoleTextEditor()
		{
			fakeConsoleTextEditor.LineBuilder.Append("abc\r\n");
			string line = threadSafeConsoleTextEditor.GetLine(0);
			string expectedText = "abc\r\n";
			
			Assert.AreEqual(expectedText, line);
		}
		
		[Test]
		public void Replace_WrappedConsoleTextEditor_ReplacementTextPassedToWrappedConsoleTextEditor()
		{
			fakeConsoleTextEditor.LineBuilder.Append("abc\r\n");
			threadSafeConsoleTextEditor.Replace(1, 2, "ddd");
			
			string replacementText = fakeConsoleTextEditor.TextPassedToReplace;
			string expectedText = "ddd";
			
			Assert.AreEqual(expectedText, replacementText);
		}
		
		[Test]
		public void Replace_WrappedConsoleTextEditor_ReplacementIndexPassedToWrappedConsoleTextEditor()
		{
			fakeConsoleTextEditor.LineBuilder.Append("abc\r\n");
			int expectedIndex = 1;
			threadSafeConsoleTextEditor.Replace(expectedIndex, 2, "ddd");
			
			int index = fakeConsoleTextEditor.IndexPassedToReplace;
			
			Assert.AreEqual(expectedIndex, index);
		}
		
		[Test]
		public void Replace_WrappedConsoleTextEditor_ReplacementLengthPassedToWrappedConsoleTextEditor()
		{
			fakeConsoleTextEditor.LineBuilder.Append("abc\r\n");
			int expectedLength = 2;
			threadSafeConsoleTextEditor.Replace(1, expectedLength, "ddd");
			
			int length = fakeConsoleTextEditor.LengthPassedToReplace;
			
			Assert.AreEqual(expectedLength, length);
		}
		
		[Test]
		public void Write_DispatcherCheckAccessReturnsFalse_MethodIsInvoked()
		{
			dispatcher.CheckAccessReturnValue = false;
			dispatcher.MethodInvoked = null;
			
			threadSafeConsoleTextEditor.Write("abc");
			Assert.IsNotNull(dispatcher.MethodInvoked);
		}
		
		[Test]
		public void Write_DispatcherCheckAccessReturnsFalse_MethodIsInvokedWithTextAsArg()
		{
			dispatcher.CheckAccessReturnValue = false;
			dispatcher.MethodInvokedArgs = null;
			
			threadSafeConsoleTextEditor.Write("abc");
			object[] expectedArgs = new object[] { "abc" };
			Assert.AreEqual(expectedArgs, dispatcher.MethodInvokedArgs);
		}
		
		[Test]
		public void GetLine_DispatcherCheckAccessReturnsFalse_MethodIsInvoked()
		{
			dispatcher.CheckAccessReturnValue = false;
			dispatcher.MethodInvoked = null;
			
			threadSafeConsoleTextEditor.GetLine(0);
			Assert.IsNotNull(dispatcher.MethodInvoked);
		}
		
		[Test]
		public void GetLine_DispatcherCheckAccessReturnsFalse_MethodIsInvokedWithLineNumberAsArg()
		{
			dispatcher.CheckAccessReturnValue = false;
			dispatcher.MethodInvokedArgs = null;
			
			threadSafeConsoleTextEditor.GetLine(0);
			object[] expectedArgs = new object[] { 0 };
			Assert.AreEqual(expectedArgs, dispatcher.MethodInvokedArgs);
		}
		
		[Test]
		public void Replace_DispatcherCheckAccessReturnsFalse_MethodIsInvoked()
		{
			dispatcher.CheckAccessReturnValue = false;
			dispatcher.MethodInvoked = null;
			fakeConsoleTextEditor.Text = "abcd";
			
			threadSafeConsoleTextEditor.Replace(0, 2, "12");
			Assert.IsNotNull(dispatcher.MethodInvoked);
		}
		
		[Test]
		public void Replace_DispatcherCheckAccessReturnsFalse_MethodIsInvokedWithThreeArgs()
		{
			dispatcher.CheckAccessReturnValue = false;
			dispatcher.MethodInvokedArgs = null;
			fakeConsoleTextEditor.Text = "abcd";
			
			threadSafeConsoleTextEditor.Replace(0, 2, "12");
			object[] expectedArgs = new object[] { 0, 2, "12" };
			Assert.AreEqual(expectedArgs, dispatcher.MethodInvokedArgs);
		}
		
		[Test]
		public void MakeCurrentContentReadOnly_DispatcherCheckAccessReturnsFalse_MethodIsInvoked()
		{
			dispatcher.CheckAccessReturnValue = false;
			dispatcher.MethodInvoked = null;
			
			threadSafeConsoleTextEditor.MakeCurrentContentReadOnly();
			Assert.IsNotNull(dispatcher.MethodInvoked);
		}
		
		[Test]
		public void ShowCompletionWindow_DispatcherCheckAccessReturnsFalse_MethodIsInvoked()
		{
			dispatcher.CheckAccessReturnValue = false;
			dispatcher.MethodInvoked = null;
			
			ScriptingConsoleCompletionDataProvider provider = new ScriptingConsoleCompletionDataProvider(null);
			threadSafeConsoleTextEditor.ShowCompletionWindow(provider);
			Assert.IsNotNull(dispatcher.MethodInvoked);
		}
		
		[Test]
		public void ShowCompletionWindow_DispatcherCheckAccessReturnsFalse_MethodIsInvokedWithDataProviderPassedToMethod()
		{
			dispatcher.CheckAccessReturnValue = false;
			dispatcher.MethodInvoked = null;
			
			ScriptingConsoleCompletionDataProvider provider = new ScriptingConsoleCompletionDataProvider(null);
			threadSafeConsoleTextEditor.ShowCompletionWindow(provider);
			object[] expectedArgs = new object[] { provider };
			
			Assert.AreEqual(expectedArgs, dispatcher.MethodInvokedArgs);
		}
	}
}
