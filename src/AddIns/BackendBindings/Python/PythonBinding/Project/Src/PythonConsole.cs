// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;
using System.Threading;
using System.Windows.Forms;

using ICSharpCode.TextEditor;
using ICSharpCode.TextEditor.Document;
using Microsoft.Scripting.Hosting.Shell;

namespace ICSharpCode.PythonBinding
{
	public class PythonConsole : IConsole, IDisposable, IMemberProvider
	{
		ITextEditor textEditor;
		int lineReceivedEventIndex = 0; // The index into the waitHandles array where the lineReceivedEvent is stored.
		ManualResetEvent lineReceivedEvent = new ManualResetEvent(false);
		ManualResetEvent disposedEvent = new ManualResetEvent(false);
		WaitHandle[] waitHandles;
		int promptLength;
		List<string> previousLines = new List<string>();
		CommandLine commandLine;
		CommandLineHistory commandLineHistory = new CommandLineHistory();
		
		public PythonConsole(ITextEditor textEditor, CommandLine commandLine)
		{
			waitHandles = new WaitHandle[] {lineReceivedEvent, disposedEvent};
			
			this.commandLine = commandLine;
			
			this.textEditor = textEditor;
			textEditor.KeyPress += ProcessKeyPress;
			textEditor.DialogKeyPress += ProcessDialogKeyPress;
			textEditor.IndentStyle = IndentStyle.None;
		}
		
		public void Dispose()
		{
			disposedEvent.Set();
			//TextArea textArea = textEditor.ActiveTextAreaControl.TextArea;
			//textArea.KeyEventHandler -= ProcessKeyPress;
			//textArea.DoProcessDialogKey -= ProcessDialogKey;
		}
				
		public TextWriter Output {
			get {
				Console.WriteLine("PythonConsole.Output get");
				return null;
			}
			set {
				Console.WriteLine("PythonConsole.Output set");				
			}
		}
		
		public TextWriter ErrorOutput {
			get {
				Console.WriteLine("PythonConsole.ErrorOutput get");
				return null;
			}
			set {
				Console.WriteLine("PythonConsole.ErrorOutput get");
			}
		}
		
		/// <summary>
		/// Gets the member names of the specified item.
		/// </summary>
		public IList<string> GetMemberNames(string name)
		{
			return commandLine.GetMemberNames(name);
		}
		
		public IList<string> GetGlobals(string name)
		{
			return commandLine.GetGlobals(name);
		}
		
		/// <summary>
		/// Returns the next line typed in by the console user. If no line is available this method
		/// will block.
		/// </summary>
		public string ReadLine(int autoIndentSize)
		{
			Console.WriteLine("PythonConsole.ReadLine(): autoIndentSize: " + autoIndentSize);
			
			string indent = String.Empty;
			if (autoIndentSize > 0) {
				indent = String.Empty.PadLeft(autoIndentSize);
				Write(indent, Style.Prompt);
			}
			
			string line = ReadLineFromTextEditor();
			if (line != null) {
				Console.WriteLine("ReadLine: " + indent + line);
				return indent + line;
			}
			return null;
		}
		
		/// <summary>
		/// Writes text to the console.
		/// </summary>
		public void Write(string text, Style style)
		{
			Console.WriteLine("PythonConsole.Write(text, style): " + text);

			textEditor.Write(text);

			if (style == Style.Prompt) {
				promptLength = text.Length;
				textEditor.MakeCurrentContentReadOnly();
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
				lock (previousLines) {
					return previousLines.Count > 0;
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
		
		/// <summary>
		/// Gets the lines that have not been returned by the ReadLine method. This does not
		/// include the current line.
		/// </summary>
		public string[] GetUnreadLines()
		{
			return previousLines.ToArray();
		}
		
		string GetLastTextEditorLine()
		{
			return textEditor.GetLine(textEditor.TotalLines - 1);
		}
		
		string ReadLineFromTextEditor()
		{			
			int result = WaitHandle.WaitAny(waitHandles);
			if (result == lineReceivedEventIndex) {
				lock (previousLines) {
					string line = previousLines[0];
					previousLines.RemoveAt(0);
					if (previousLines.Count == 0) {
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
		bool ProcessKeyPress(char ch)
		{
			if (IsInReadOnlyRegion) {
				return true;
			}
			
			if (ch == '\n') {
				OnEnterKeyPressed();
			}
			
			if (ch == '.') {
				ShowCompletionWindow();
			}
			return false;
		}
		
		/// <summary>
		/// Process dialog keys such as the enter key when typed into the editor by the user.
		/// </summary>
		bool ProcessDialogKeyPress(Keys keyData)
		{
			if (textEditor.IsCompletionWindowDisplayed) {
				return false;
			}
			
			if (IsInReadOnlyRegion) {
				switch (keyData) {
					case Keys.Left:
					case Keys.Right:
					case Keys.Up:
					case Keys.Down:
						return false;
					default:
						return true;
				}
			}
			
			switch (keyData) {
				case Keys.Back:
					return !CanBackspace;
				case Keys.Home:
					MoveToHomePosition();
					return true;
				case Keys.Down:
					MoveToNextCommandLine();
					return true;
				case Keys.Up:
					MoveToPreviousCommandLine();
					return true;
			}
			return false;
		}
		
		/// <summary>
		/// Move cursor to the end of the line before retrieving the line.
		/// </summary>
		void OnEnterKeyPressed()
		{
			lock (previousLines) {
				// Move cursor to the end of the line.
				textEditor.Column = GetLastTextEditorLine().Length;

				// Append line.
				string currentLine = GetCurrentLine();
				previousLines.Add(currentLine);
				commandLineHistory.Add(currentLine);
				
				lineReceivedEvent.Set();
			}
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
			get { return textEditor.Line < textEditor.TotalLines - 1; }
		}
		
		/// <summary>
		/// Determines whether the current cursor position is in a prompt.
		/// </summary>
		bool IsInPrompt {
			get { return textEditor.Column - promptLength < 0; }
		}
		
		/// <summary>
		/// Returns true if the user can backspace at the current cursor position.
		/// </summary>
		bool CanBackspace {
			get {
				int cursorIndex = textEditor.Column - promptLength;
				int selectionStartIndex = textEditor.SelectionStart - promptLength;
				return cursorIndex > 0 && selectionStartIndex > 0;
			}
		}
		
		void ShowCompletionWindow()
		{
			PythonConsoleCompletionDataProvider completionProvider = new PythonConsoleCompletionDataProvider(this);
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
	}
}
