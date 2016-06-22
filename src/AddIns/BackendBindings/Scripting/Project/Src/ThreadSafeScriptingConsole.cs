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

namespace ICSharpCode.Scripting
{
	public class ThreadSafeScriptingConsole : IScriptingConsole
	{
		IScriptingConsole nonThreadSafeScriptingConsole;
		IControlDispatcher dispatcher;
		ThreadSafeScriptingConsoleEvents consoleEvents;
		
		delegate string ThreadSafeReadLineInvoker(int autoIndentSize);
		delegate string ThreadSafeReadFirstUnreadLineInvoker();
		delegate int ThreadSafeGetMaximumVisibleColumnsInvoker();
		
		public ThreadSafeScriptingConsole(IScriptingConsole nonThreadSafeScriptingConsole, IControlDispatcher dispatcher)
			: this(nonThreadSafeScriptingConsole, new ThreadSafeScriptingConsoleEvents(), dispatcher)
		{
		}
		
		public ThreadSafeScriptingConsole(IScriptingConsole nonThreadSafeScriptingConsole,
			ThreadSafeScriptingConsoleEvents consoleEvents,
			IControlDispatcher dispatcher)
		{
			this.nonThreadSafeScriptingConsole = nonThreadSafeScriptingConsole;
			this.consoleEvents = consoleEvents;
			this.dispatcher = dispatcher;
			
			nonThreadSafeScriptingConsole.LineReceived += NonThreadSafeScriptingConsoleLineReceived;
		}
		
		void NonThreadSafeScriptingConsoleLineReceived(object source, EventArgs e)
		{
			consoleEvents.SetLineReceivedEvent();
		}
		
		public bool ScrollToEndWhenTextWritten {
			get { return nonThreadSafeScriptingConsole.ScrollToEndWhenTextWritten; }
			set { nonThreadSafeScriptingConsole.ScrollToEndWhenTextWritten = value; }
		}
		
		public void WriteLine()
		{
			if (dispatcher.CheckAccess()) {
				nonThreadSafeScriptingConsole.WriteLine();
			} else {
				Action action = WriteLine;
				dispatcher.Invoke(action);
			}
		}
		
		public void WriteLine(string text, ScriptingStyle style)
		{
			if (dispatcher.CheckAccess()) {
				nonThreadSafeScriptingConsole.WriteLine(text, style);
			} else {
				Action<string, ScriptingStyle> action = WriteLine;
				dispatcher.Invoke(action, text, style);
			}
		}
		
		public void Write(string text, ScriptingStyle style)
		{
			if (dispatcher.CheckAccess()) {
				nonThreadSafeScriptingConsole.Write(text, style);
			} else {
				Action<string, ScriptingStyle> action = Write;
				dispatcher.Invoke(action, text, style);
			}
		}
		
		public string ReadLine(int autoIndentSize)
		{
			consoleEvents.ResetLineReceivedEvent();
			string line = ThreadSafeReadLine(autoIndentSize);
			if (line != null) {
				return line;
			}
			
			if (consoleEvents.WaitForLine()) {
				return ThreadSafeReadFirstUnreadLine();
			}
			return null;
		}
		
		string ThreadSafeReadLine(int autoIndentSize)
		{
			if (dispatcher.CheckAccess()) {
				return nonThreadSafeScriptingConsole.ReadLine(autoIndentSize);
			} else {
				ThreadSafeReadLineInvoker action = ThreadSafeReadLine;
				return (string)dispatcher.Invoke(action, autoIndentSize);
			}
		}
		
		string ThreadSafeReadFirstUnreadLine()
		{
			if (dispatcher.CheckAccess()) {
				return nonThreadSafeScriptingConsole.ReadFirstUnreadLine();
			} else {
				ThreadSafeReadFirstUnreadLineInvoker action = ThreadSafeReadFirstUnreadLine;
				return (string)dispatcher.Invoke(action);
			}
		}
		
		public void Dispose()
		{
			nonThreadSafeScriptingConsole.LineReceived -= NonThreadSafeScriptingConsoleLineReceived;
			consoleEvents.SetDisposedEvent();
		}
		
		public event EventHandler LineReceived;
		
		protected virtual void OnLineReceived(EventArgs e)
		{
			if (LineReceived != null) {
				LineReceived(this, e);
			}
		}
		
		public void SendLine(string line)
		{
			nonThreadSafeScriptingConsole.SendLine(line);
		}
		
		public void SendText(string text)
		{
			nonThreadSafeScriptingConsole.SendText(text);
		}
		
		public string ReadFirstUnreadLine()
		{
			return nonThreadSafeScriptingConsole.ReadFirstUnreadLine();
		}
		
		public int GetMaximumVisibleColumns()
		{
			if (dispatcher.CheckAccess()) {
				return nonThreadSafeScriptingConsole.GetMaximumVisibleColumns();
			} else {
				ThreadSafeGetMaximumVisibleColumnsInvoker action = GetMaximumVisibleColumns;
				return (int)dispatcher.Invoke(action);
			}
		}
		
		public void Clear()
		{
			if (dispatcher.CheckAccess()) {
				nonThreadSafeScriptingConsole.Clear();
			} else {
				Action action = Clear;
				dispatcher.Invoke(action);
			}
		}
	}
}
