// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;
using System.Threading;
using System.Windows.Input;
using ICSharpCode.AvalonEdit.CodeCompletion;
using ICSharpCode.Scripting;
using IronPython.Hosting;
using Microsoft.Scripting.Hosting.Shell;

namespace ICSharpCode.PythonBinding
{
	public class PythonConsole : IConsole, IDisposable, IMemberProvider, IScriptingConsole
	{
		IScriptingConsoleTextEditor textEditor;
		int lineReceivedEventIndex = 0; // The index into the waitHandles array where the lineReceivedEvent is stored.
		ManualResetEvent lineReceivedEvent = new ManualResetEvent(false);
		ManualResetEvent disposedEvent = new ManualResetEvent(false);
		WaitHandle[] waitHandles;
		int promptLength;
		bool firstPromptDisplayed;
		string savedSendLineText;
		CommandLineHistory commandLineHistory = new CommandLineHistory();
		
		protected List<string> unreadLines = new List<string>();
		
		public PythonConsole(IScriptingConsoleTextEditor textEditor)
		{
			waitHandles = new WaitHandle[] {lineReceivedEvent, disposedEvent};
			
			this.textEditor = textEditor;
			textEditor.PreviewKeyDown += ProcessPreviewKeyDown;
		}
		
		public CommandLine CommandLine { get; set; }
		
		public void Dispose()
		{
			disposedEvent.Set();
		}
		
		public TextWriter Output {
			get { return null; }
			set { }
		}
		
		public TextWriter ErrorOutput {
			get { return null; }
			set { }
		}
		
		/// <summary>
		/// Gets the member names of the specified item.
		/// </summary>
		public IList<string> GetMemberNames(string name)
		{
			return CommandLine.GetMemberNames(name);
		}
		
		public IList<string> GetGlobals(string name)
		{
			return CommandLine.GetGlobals(name);
		}
		
		/// <summary>
		/// Returns the next line typed in by the console user. If no line is available this method
		/// will block.
		/// </summary>
		public string ReadLine(int autoIndentSize)
		{
			string indent = GetIndent(autoIndentSize);
			if (autoIndentSize > 0) {
				Write(indent, Style.Prompt);
			}
			
			string line = ReadLineFromTextEditor();
			if (line != null) {
				return indent + line;
			}
			return null;
		}
		
		string GetIndent(int autoIndentSize)
		{
			return String.Empty.PadLeft(autoIndentSize);
		}
		
		/// <summary>
		/// Writes text to the console.
		/// </summary>
		public void Write(string text, Style style)
		{
			textEditor.Write(text);
			if (style == Style.Prompt) {
				WriteSavedLineTextAfterFirstPrompt(text);
				promptLength = text.Length;
				textEditor.MakeCurrentContentReadOnly();
			}
		}
		
		void WriteSavedLineTextAfterFirstPrompt(string promptText)
		{
			firstPromptDisplayed = true;
			if (savedSendLineText != null) {
				textEditor.Write(savedSendLineText + "\r\n");
				savedSendLineText = null;
			}
		}
		
		/// <summary>
		/// Writes text followed by a newline to the console.
		/// </summary>
		public void WriteLine(string text, Style style)
		{
			Write(text + Environment.NewLine, style);
		}
		
		/// <summary>
		/// Writes an empty line to the console.
		/// </summary>
		public void WriteLine()
		{
			Write(Environment.NewLine, Style.Out);
		}

		/// <summary>
		/// Indicates whether there is a line already read by the console and waiting to be processed.
		/// </summary>
		public bool IsLineAvailable {
			get { 
				lock (unreadLines) {
					return (unreadLines.Count > 0);
				}
			}
		}

		/// <summary>
		/// Gets the text that is yet to be processed from the console. This is the text that is being
		/// typed in by the user who has not yet pressed the enter key.
		/// </summary>
		public string GetCurrentLine()
		{
			string fullLine = GetLastTextEditorLine();
			return fullLine.Substring(promptLength);
		}
		
		string GetLastTextEditorLine()
		{
			return textEditor.GetLine(textEditor.TotalLines - 1);
		}
		
		string ReadLineFromTextEditor()
		{
			int result = WaitHandle.WaitAny(waitHandles);
			if (result == lineReceivedEventIndex) {
				lock (unreadLines) {
					string line = unreadLines[0];
					unreadLines.RemoveAt(0);
					if (unreadLines.Count == 0) {
						lineReceivedEvent.Reset();
					}
					return line;
				}
			}
			return null;
		}
		
		/// <summary>
		/// Processes characters entered into the text editor by the user.
		/// </summary>
		void ProcessPreviewKeyDown(object source, ConsoleTextEditorKeyEventArgs e)
		{
			Key keyPressed = e.Key;
			e.Handled = HandleKeyDown(keyPressed); 
		}
		
		bool HandleKeyDown(Key keyPressed)
		{
			if (textEditor.IsCompletionWindowDisplayed) {
				return false;
			}
			
			if (IsInReadOnlyRegion) {
				switch (keyPressed) {
					case Key.Left:
					case Key.Right:
					case Key.Up:
					case Key.Down:
						return false;
					default:
						return true;
				}
			}
			
			switch (keyPressed) {
				case Key.Back:
					return !CanBackspace;
				case Key.Home:
					MoveToHomePosition();
					return true;
				case Key.Down:
					MoveToNextCommandLine();
					return true;
				case Key.Up:
					MoveToPreviousCommandLine();
					return true;
			}
						
			if (keyPressed == Key.Return) {
				OnEnterKeyPressed();
			}
			
			if (keyPressed == Key.OemPeriod) {
				ShowCompletionWindow();
			}
			return false;
		}
		
		void OnEnterKeyPressed()
		{
			lock (unreadLines) {
				MoveCursorToEndOfLastTextEditorLine();
				SaveLastTextEditorLine();
				
				lineReceivedEvent.Set();
			}
		}
		
		void MoveCursorToEndOfLastTextEditorLine()
		{
			textEditor.Line = textEditor.TotalLines - 1;
			textEditor.Column = GetLastTextEditorLine().Length;
		}
		
		void SaveLastTextEditorLine()
		{
			string currentLine = GetCurrentLine();
			unreadLines.Add(currentLine);
			commandLineHistory.Add(currentLine);
		}

		/// <summary>
		/// Returns true if the cursor is in a readonly text editor region.
		/// </summary>
		bool IsInReadOnlyRegion {
			get { return IsCurrentLineReadOnly || IsInPrompt; }
		}
		
		/// <summary>
		/// Only the last line in the text editor is not read only.
		/// </summary>
		bool IsCurrentLineReadOnly {
			get { return textEditor.Line < (textEditor.TotalLines - 1); }
		}
		
		/// <summary>
		/// Determines whether the current cursor position is in a prompt.
		/// </summary>
		bool IsInPrompt {
			get { return (textEditor.Column - promptLength) < 0; }
		}
		
		/// <summary>
		/// Returns true if the user can backspace at the current cursor position.
		/// </summary>
		bool CanBackspace {
			get {
				int cursorIndex = textEditor.Column - promptLength;
				int selectionStartIndex = textEditor.SelectionStart - promptLength;
				return (cursorIndex > 0) && (selectionStartIndex > 0);
			}
		}
		
		void ShowCompletionWindow()
		{
			ScriptingConsoleCompletionDataProvider completionProvider = new ScriptingConsoleCompletionDataProvider(this);
			textEditor.ShowCompletionWindow(completionProvider);
		}

		/// <summary>
		/// The home position is at the start of the line after the prompt.
		/// </summary>
		void MoveToHomePosition()
		{
			textEditor.Column = promptLength;
		}
		
		/// <summary>
		/// Shows the previous command line in the command line history.
		/// </summary>
		void MoveToPreviousCommandLine()
		{
			if (commandLineHistory.MovePrevious()) {
				ReplaceCurrentLineTextAfterPrompt(commandLineHistory.Current);
			}
		}
		
		/// <summary>
		/// Shows the next command line in the command line history.
		/// </summary>
		void MoveToNextCommandLine()
		{
			if (commandLineHistory.MoveNext()) {
				ReplaceCurrentLineTextAfterPrompt(commandLineHistory.Current);
			}
		}
		
		/// <summary>
		/// Replaces the current line text after the prompt with the specified text.
		/// </summary>
		void ReplaceCurrentLineTextAfterPrompt(string text)
		{
			string currentLine = GetCurrentLine();
			textEditor.Replace(promptLength, currentLine.Length, text);
			
			// Put cursor at end.
			textEditor.Column = promptLength + text.Length;
		}
		
		public void SendLine(string text)
		{
			using (ILock linesLock = CreateLock(unreadLines)) {
				unreadLines.Add(text);					
			}
			FireLineReceivedEvent();
			MoveCursorToEndOfLastTextEditorLine();
			WriteLineIfFirstPromptHasBeenDisplayed(text);
		}
		
		protected virtual ILock CreateLock(List<string> lines)
		{
			return new StringListLock(lines);
		}
		
		protected virtual void FireLineReceivedEvent()
		{
			lineReceivedEvent.Set();
		}
		
		void WriteLineIfFirstPromptHasBeenDisplayed(string text)
		{
			if (firstPromptDisplayed) {
				WriteLine(text, Style.Out);
			} else {
				savedSendLineText = text;
			}
		}
	}
}
