// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;

using ICSharpCode.TextEditor;
using ICSharpCode.TextEditor.Document;
using ICSharpCode.PythonBinding;
using NUnit.Framework;

namespace PythonBinding.Tests.Console
{
	/// <summary>
	/// Tests the TextEditor class.
	/// </summary>
	[TestFixture]
	public class TextEditorTestFixture
	{
		TextEditorControl textEditorControl;
		TextEditor textEditor;
		IndentStyle defaultTextEditorControlIndentStyle;
		IndentStyle defaultTextEditorIndentStyle;
		char keyPressed = ' ';
		Keys dialogKeyPressed;
		Exception indentException;
		Exception getLineException;
		string lineReadOnDifferentThread = null;
		
		[TestFixtureSetUp]
		public void SetUpFixture()
		{
			textEditorControl = new TextEditorControl();
			
			// Force creation of handle otherwise InvokeRequired always returns false.
			textEditorControl.CreateControl();
			
			textEditorControl.IndentStyle = IndentStyle.Smart;
			defaultTextEditorControlIndentStyle = textEditorControl.IndentStyle;

			textEditor = new TextEditor(textEditorControl);
			defaultTextEditorIndentStyle = textEditor.IndentStyle;
		}
		
		[TestFixtureTearDown]
		public void TearDownFixture()
		{
			textEditorControl.Dispose();
		}
		
		[Test]
		public void DefaultTextEditorIndentStyleSameAsTextEditorControl()
		{
			Assert.AreEqual(defaultTextEditorControlIndentStyle, defaultTextEditorIndentStyle);
		}
		
		[Test]
		public void TextEditorImplementsITextEditorInterface()
		{
			Assert.IsNotNull(textEditor as ITextEditor);
		}
		
		[Test]
		public void IndentStyleUpdatesTextEditor()
		{			
			textEditor.IndentStyle = IndentStyle.None;
			Assert.AreEqual(IndentStyle.None, textEditorControl.IndentStyle);
		}
		
		[Test]
		public void KeyPressEventHandlerImplemented()
		{
			textEditor.KeyPress += ProcessKeyPress;
			try {
				textEditorControl.ActiveTextAreaControl.TextArea.SimulateKeyPress('a');
				Assert.AreEqual('a', keyPressed);
			} finally {
				textEditor.KeyPress -= ProcessKeyPress;
			}
		}
		
		[Test]
		public void KeyPressEventHandlerRemoved()
		{
			keyPressed = ' ';
			textEditor.KeyPress += ProcessKeyPress;
			textEditor.KeyPress -= ProcessKeyPress;
			textEditorControl.ActiveTextAreaControl.TextArea.SimulateKeyPress('b');
			
			Assert.AreEqual(' ', keyPressed);
		}
				
		[Test]
		public void DialogKeyPressEventHandlerImplemented()
		{
			textEditor.DialogKeyPress += ProcessDialogKeyPress;
			try {
				textEditorControl.ActiveTextAreaControl.TextArea.ExecuteDialogKey(Keys.B);
				Assert.AreEqual(dialogKeyPressed, Keys.B);
			} finally {
				textEditor.DialogKeyPress -= ProcessDialogKeyPress;
			}
		}
		
		[Test]
		public void DialogKeyPressEventHandlerRemoved()
		{
			dialogKeyPressed = Keys.Enter;
			textEditor.DialogKeyPress += ProcessDialogKeyPress;
			textEditor.DialogKeyPress -= ProcessDialogKeyPress;
			textEditorControl.ActiveTextAreaControl.TextArea.ExecuteDialogKey(Keys.Alt);
			
			Assert.AreEqual(Keys.Enter, dialogKeyPressed);
		}
		
		[Test]
		public void MakeCurrentTextEditorContent()
		{
			textEditorControl.Text = String.Empty;
			textEditor.Write("abc" + Environment.NewLine);
			textEditor.MakeCurrentContentReadOnly();
			
			TextMarker readOnlyTextMarker = GetReadOnlyTextMarker(textEditorControl);
			Assert.IsNotNull(readOnlyTextMarker);
			Assert.AreEqual(0, readOnlyTextMarker.Offset);
			Assert.AreEqual(textEditorControl.Text.Length, readOnlyTextMarker.Length);
			Assert.IsFalse(textEditorControl.Document.UndoStack.CanUndo);
		}
		
		[Test]
		public void WriteMethodUpdatesTextEditor()
		{
			textEditorControl.Document.TextContent = String.Empty;
			textEditor.Write("abc");
			Assert.AreEqual("abc", textEditorControl.Document.TextContent);
		}

		[Test]
		public void WriteOnDifferentThread()
		{
			textEditorControl.Document.TextContent = String.Empty;
			Thread t = new Thread(WriteText);
			t.Start();
			
			// Make sure the GUI events are processed otherwise the
			// unit test will never complete.
			int maxWait = 2000;
			int currentWait = 0;
			int sleepInterval = 50;
			Application.DoEvents();
			while (textEditorControl.Text.Length == 0 && (currentWait < maxWait)) {
				Application.DoEvents();
				Thread.Sleep(sleepInterval);
				currentWait += sleepInterval;
			}
			
			// Wait for thread to finish.
			t.Join();
	
			Assert.AreEqual("test", textEditorControl.Text);
		}
		
		[Test]
		public void SetIndentStyleOnDifferentThread()
		{
			textEditorControl.IndentStyle = IndentStyle.Auto;
			Thread t = new Thread(SetIndentStyle);
			t.Start();
			
			// Make sure the GUI events are processed otherwise the
			// unit test will never complete.
			int maxWait = 2000;
			int currentWait = 0;
			int sleepInterval = 50;
			Application.DoEvents();
			while (textEditorControl.IndentStyle == IndentStyle.Auto && (currentWait < maxWait)) {
				Application.DoEvents();
				Thread.Sleep(sleepInterval);
				currentWait += sleepInterval;
			}
			
			// Wait for thread to finish.
			t.Join();
	
			string message = String.Empty;
			if (indentException != null) {
				message = indentException.ToString();
			}
			Assert.IsNull(indentException, message);
		}
		
		[Test]
		public void GetColumnPosition()
		{
			textEditorControl.Document.TextContent = "test";
			textEditorControl.ActiveTextAreaControl.TextArea.Caret.Column = 2;
			
			Assert.AreEqual(2, textEditor.Column);
		}
		
		[Test]
		public void SetColumnPosition()
		{
			textEditorControl.Document.TextContent = "test";
			textEditorControl.ActiveTextAreaControl.TextArea.Caret.Column = 2;

			textEditor.Column = 1;
			Assert.AreEqual(1, textEditorControl.ActiveTextAreaControl.TextArea.Caret.Column);
		}
		
		[Test]
		public void GetSelectionStartAndLength()
		{
			textEditorControl.Document.TextContent = "te000xt";
			TextLocation start = new TextLocation(2, 0);
			TextLocation end = new TextLocation(5, 0);
			textEditorControl.ActiveTextAreaControl.SelectionManager.SetSelection(start, end);
			
			// Sanity check.
			Assert.AreEqual("000", textEditorControl.ActiveTextAreaControl.SelectionManager.SelectedText);
			
			Assert.AreEqual(2, textEditor.SelectionStart);
			Assert.AreEqual(3, textEditor.SelectionLength);
		}
		
		[Test]
		public void GetSelectionStartAndLengthWhenNothingSelected()
		{
			textEditorControl.Document.TextContent = "text";
			textEditorControl.ActiveTextAreaControl.SelectionManager.ClearSelection();
			textEditorControl.ActiveTextAreaControl.Caret.Column = 1;

			Assert.AreEqual(1, textEditor.SelectionStart);
			Assert.AreEqual(0, textEditor.SelectionLength);
		}

		[Test]
		public void TextEditorLineEqualsCaretLine()
		{
			textEditorControl.Document.TextContent = "abc\r\ndef";
			textEditorControl.ActiveTextAreaControl.Caret.Line = 0;
			
			Assert.AreEqual(0, textEditor.Line);

			textEditorControl.ActiveTextAreaControl.Caret.Line = 1;
			Assert.AreEqual(1, textEditor.Line);			
		}
		
		[Test]
		public void TextEditorTotalLines()
		{
			textEditorControl.Document.TextContent = "abc\r\ndef\r\nghi";
			Assert.AreEqual(3, textEditor.TotalLines);
		}
		
		[Test]
		public void GetFirstLine()
		{
			textEditorControl.Document.TextContent = "abc\r\ndef\r\nghi";
			Assert.AreEqual("abc", textEditor.GetLine(0));
		}

		[Test]
		public void GetLineOnDifferentThread()
		{
			textEditorControl.IndentStyle = IndentStyle.Auto;
			Thread t = new Thread(GetLine);
			t.Start();
			
			// Make sure the GUI events are processed otherwise the
			// unit test will never complete.
			int maxWait = 2000;
			int currentWait = 0;
			int sleepInterval = 50;
			Application.DoEvents();
			while (lineReadOnDifferentThread == null && (currentWait < maxWait)) {
				Application.DoEvents();
				Thread.Sleep(sleepInterval);
				currentWait += sleepInterval;
			}
			
			// Wait for thread to finish.
			t.Join();
	
			string message = String.Empty;
			if (getLineException != null) {
				message = getLineException.ToString();
			}
			Assert.IsNull(getLineException, message);
		}
		
		[Test]
		public void WriteTextWithBackgroundColour()
		{
			textEditorControl.Document.TextContent = "abc\r\n>>>";
			textEditorControl.ActiveTextAreaControl.Caret.Line = 1;
			textEditor.Write(">>> ", Color.Blue);
			
			int offset = textEditorControl.Document.PositionToOffset(new TextLocation(0, 1));
			List<TextMarker> markers = textEditorControl.Document.MarkerStrategy.GetMarkers(offset);
			TextMarker marker = markers[0];
			Assert.AreEqual(Color.Blue, marker.Color);
			Assert.AreEqual(TextMarkerType.SolidBlock, marker.TextMarkerType);
			Assert.AreEqual(4, marker.Length);
		}
		
		[Test]
		public void SupportReadOnlySegmentsIsTrue()
		{
			Assert.IsTrue(textEditorControl.TextEditorProperties.SupportReadOnlySegments);
		}
		
		[Test]
		public void ReplaceText()
		{
			textEditorControl.Document.TextContent = "abc\r\ndef";
			textEditorControl.ActiveTextAreaControl.Caret.Line = 1;
			
			textEditor.Replace(1, 1, "test");
			Assert.AreEqual("abc\r\ndtestf", textEditorControl.Document.TextContent);
		}
		
		/// <summary>
		/// Run on different thread to set the text editor's indent style.
		/// </summary>
		void SetIndentStyle()
		{
			try {
				textEditor.IndentStyle = IndentStyle.None;
			} catch (Exception ex) {
				indentException = ex;
			}
		}
		
		/// <summary>
		/// Run on different thread to set the text editor's indent style.
		/// </summary>
		void GetLine()
		{
			try {
				lineReadOnDifferentThread = textEditor.GetLine(0);
			} catch (Exception ex) {
				getLineException = ex;
			}
		}		
		
		/// <summary>
		/// Run on different thread to write text to the text editor.
		/// </summary>
		void WriteText()
		{
			try {
				textEditor.Write("test");
			} catch (Exception ex) {
				System.Console.WriteLine("WriteText error: " + ex.ToString());
			}
		}
		
		/// <summary>
		/// Event handler for the text editor's DialogKeyPress event.
		/// </summary>
		bool ProcessDialogKeyPress(Keys keysData)
		{
			dialogKeyPressed = keysData;
			return true;
		}

		/// <summary>
		/// Event handler for the text editor's KeyPress event.
		/// </summary>
		bool ProcessKeyPress(char ch)
		{
			keyPressed = ch;
			return false;
		}
		
		/// <summary>
		/// Used to remove all text markers from the text editor.
		/// </summary>
		bool AllMarkersMatch(TextMarker marker)
		{
			return true;
		}
		
		TextMarker GetReadOnlyTextMarker(TextEditorControl textEditorControl)
		{
			foreach (TextMarker marker in textEditorControl.Document.MarkerStrategy.TextMarker) {
				if (marker.IsReadOnly) {
					return marker;
				}
			}
			return null;			
		}
	}
}
