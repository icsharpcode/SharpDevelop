// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using ICSharpCode.AvalonEdit;
using ICSharpCode.Scripting;

namespace ICSharpCode.RubyBinding
{
	public class ThreadSafeRubyConsoleTextEditor : IConsoleTextEditor
	{
		delegate string GetLineInvoker(int index);
		
		IConsoleTextEditor consoleTextEditor;
		IControlDispatcher dispatcher;
		
		public ThreadSafeRubyConsoleTextEditor(TextEditor textEditor)
			: this(new RubyConsoleTextEditor(textEditor), new ControlDispatcher(textEditor))
		{
		}
		
		public ThreadSafeRubyConsoleTextEditor(IConsoleTextEditor consoleTextEditor, IControlDispatcher dispatcher)
		{
			this.consoleTextEditor = consoleTextEditor;
			this.dispatcher = dispatcher;
		}
		
		public event ConsoleTextEditorKeyEventHandler PreviewKeyDown {
			add { consoleTextEditor.PreviewKeyDown += value; }
			remove { consoleTextEditor.PreviewKeyDown -= value; }
		}
		
		public void Dispose()
		{
			consoleTextEditor.Dispose();
		}
		
		public int Column {
			get { return consoleTextEditor.Column; }
			set { consoleTextEditor.Column = value; }
		}
		
		public int SelectionLength {
			get { return consoleTextEditor.SelectionLength; }
		}
		
		public int SelectionStart {
			get { return consoleTextEditor.SelectionStart; }
		}
		
		public int Line {
			get { return consoleTextEditor.Line; }
			set { consoleTextEditor.Line = value; }
		}
		
		public int TotalLines {
			get { return consoleTextEditor.TotalLines; }
		}
		
		public bool IsCompletionWindowDisplayed {
			get { return consoleTextEditor.IsCompletionWindowDisplayed; }
		}
		
		public void Write(string text)
		{			
			if (dispatcher.CheckAccess()) {
				consoleTextEditor.Write(text);
			} else {
				Action<string> action = Write;
				dispatcher.Invoke(action, text);
			}
		}
		
		public void Replace(int index, int length, string text)
		{
			if (dispatcher.CheckAccess()) {
				consoleTextEditor.Replace(index, length, text);
			} else {
				Action<int, int, string> action = Replace;
				dispatcher.Invoke(action, index, length, text);
			}
		}
		
		public string GetLine(int index)
		{
			if (dispatcher.CheckAccess()) {
				return consoleTextEditor.GetLine(index);
			} else {
				GetLineInvoker invoker = new GetLineInvoker(GetLine);
				return (string)dispatcher.Invoke(invoker, index);
			}
		}
		
		public void ShowCompletionWindow(RubyConsoleCompletionDataProvider completionDataProvider)
		{
			if (dispatcher.CheckAccess()) {
				consoleTextEditor.ShowCompletionWindow(completionDataProvider);
			} else {
				Action<RubyConsoleCompletionDataProvider> action = ShowCompletionWindow;
				dispatcher.Invoke(action, completionDataProvider);
			}
		}
		
		public void MakeCurrentContentReadOnly()
		{
			if (dispatcher.CheckAccess()) {
				consoleTextEditor.MakeCurrentContentReadOnly();
			} else {
				Action action = MakeCurrentContentReadOnly;
				dispatcher.Invoke(action);
			}
		}
	}
}
