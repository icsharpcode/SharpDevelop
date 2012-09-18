// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows.Input;

using ICSharpCode.Scripting;

namespace ICSharpCode.Scripting
{
	public class ScriptingConsole : IDisposable, IScriptingConsole
	{
		IScriptingConsoleTextEditor textEditor;
		CommandLineHistory commandLineHistory = new CommandLineHistory();
		ScriptingConsoleUnreadLines unreadLines;
		
		int promptLength;
		bool firstPromptDisplayed;
		
		TextSentToScriptingConsole textSent = new TextSentToScriptingConsole();
		
		public event EventHandler LineReceived;
		
		public IMemberProvider MemberProvider { get; set; }
		
		public ScriptingConsole(IScriptingConsoleTextEditor textEditor)
			: this(textEditor, new ScriptingConsoleUnreadLines())
		{
		}
		
		public ScriptingConsole(IScriptingConsoleTextEditor textEditor, ScriptingConsoleUnreadLines unreadLines)
		{
			this.textEditor = textEditor;
			this.unreadLines = unreadLines;
			this.ScrollToEndWhenTextWritten = true;
			textEditor.PreviewKeyDown += ProcessPreviewKeyDown;
		}
		
		public void Dispose()
		{
		}
		
		/// <summary>
		/// Returns the next line typed in by the console user. If no line is available this method
		/// will return null.
		/// </summary>
		public string ReadLine(int autoIndentSize)
		{
			string indent = GetIndent(autoIndentSize);
			if (autoIndentSize > 0) {
				Write(indent, ScriptingStyle.Prompt);
			}
			
			string line = ReadFirstUnreadLine();
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
		public void Write(string text, ScriptingStyle style)
		{
			textEditor.Append(text);
			if (style == ScriptingStyle.Prompt) {
				firstPromptDisplayed = true;
				promptLength = text.Length;
				
				WriteFirstLineOfSentText();
				
				textEditor.MakeCurrentContentReadOnly();
			}
			
			if (ScrollToEndWhenTextWritten) {
				ScrollToEnd();
			}
		}
		
		void WriteFirstLineOfSentText()
		{
			if (textSent.HasLine) {
				string line = textSent.RemoveFirstLine();
				textEditor.Append(line);
			}
		}
		
		/// <summary>
		/// Writes text followed by a newline to the console.
		/// </summary>
		public void WriteLine(string text, ScriptingStyle style)
		{
			Write(text + Environment.NewLine, style);
		}
		
		/// <summary>
		/// Writes an empty line to the console.
		/// </summary>
		public void WriteLine()
		{
			Write(Environment.NewLine, ScriptingStyle.Out);
		}

		/// <summary>
		/// Indicates whether there is a line already read by the console and waiting to be processed.
		/// </summary>
		public bool IsLineAvailable {
			get {  return (unreadLines.Count > 0); }
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
		
		/// <summary>
		/// Processes characters entered into the text editor by the user.
		/// </summary>
		void ProcessPreviewKeyDown(object source, ConsoleTextEditorKeyEventArgs e)
		{
			e.Handled = HandleKeyDown(e.Key, e.Modifiers); 
		}
		
		bool HandleKeyDown(Key keyPressed, ModifierKeys keyModifiers)
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
					case Key.C:
					case Key.A:
						return keyModifiers != ModifierKeys.Control;
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
			MoveCursorToEndOfLastTextEditorLine();
			SaveLastTextEditorLine();
			
			FireLineReceivedEvent();
		}
		
		protected virtual void OnLineReceived()
		{
			if (LineReceived != null) {
				LineReceived(this, new EventArgs());
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
			unreadLines.AddLine(currentLine);
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
			ScriptingConsoleCompletionDataProvider completionProvider = new ScriptingConsoleCompletionDataProvider(MemberProvider);
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
		
		public void SendLine(string line)
		{
			unreadLines.AddLine(line);					
			FireLineReceivedEvent();
			MoveCursorToEndOfLastTextEditorLine();
			
			if (firstPromptDisplayed) {
				WriteLine(line, ScriptingStyle.Out);
			} else {
				SaveLineToDisplayAfterFirstPromptDisplayed(line);
			}
		}
		
		protected virtual void FireLineReceivedEvent()
		{
			OnLineReceived();
		}
		
		void SaveLineToDisplayAfterFirstPromptDisplayed(string line)
		{
			string text = line + "\r\n";
			textSent.AddText(text);
		}
		
		public virtual IList<string> GetMemberNames(string name)
		{
			return new string[0];
		}
		
		public virtual IList<string> GetGlobals(string name)
		{
			return new string[0];
		}
		
		public void SendText(string text)
		{
			textSent.AddText(text);
			MoveCursorToEndOfLastTextEditorLine();
			SaveUnreadLinesOfSentText();
			
			if (firstPromptDisplayed) {
				WriteFirstLineOfSentText();
			}
		}
		
		void SaveUnreadLinesOfSentText()
		{
			if (textSent.HasAtLeastOneLine) {
				unreadLines.AddAllLinesExceptLast(textSent.lines);
				FireLineReceivedEvent();
			}
		}
		
		public string ReadFirstUnreadLine()
		{
			return unreadLines.RemoveFirstLine();
		}
		
		public bool ScrollToEndWhenTextWritten { get; set; }
		
		void ScrollToEnd()
		{
			textEditor.ScrollToEnd();
		}
		
		public int GetMaximumVisibleColumns()
		{
			return textEditor.GetMaximumVisibleColumns();
		}
		
		public void Clear()
		{
			textEditor.Clear();
		}
	}
}
