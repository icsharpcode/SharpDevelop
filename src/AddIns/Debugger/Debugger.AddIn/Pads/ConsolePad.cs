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
using System.Windows.Controls;
using System.Windows.Input;

using Debugger;
using Debugger.AddIn;
using ICSharpCode.Core.Presentation;
using ICSharpCode.NRefactory;
using ICSharpCode.SharpDevelop.Debugging;
using ICSharpCode.SharpDevelop.Editor.CodeCompletion;
using Debugger.AddIn.Pads.Controls;
using ICSharpCode.SharpDevelop.Services;

namespace ICSharpCode.SharpDevelop.Gui.Pads
{
	public class ConsolePad : AbstractConsolePad
	{
		const string debuggerConsoleToolBarTreePath = "/SharpDevelop/Pads/ConsolePad/ToolBar";
		
		protected override bool AcceptCommand(string command)
		{
			if (!string.IsNullOrEmpty(command)) {
				string result = Evaluate(command);
				if (!string.IsNullOrEmpty(result)) {
					Append(Environment.NewLine + result);
				}
			}
			return true;
		}
		
		string Evaluate(string code)
		{
			Process process = WindowsDebugger.CurrentProcess;
			StackFrame frame = WindowsDebugger.CurrentStackFrame;
			
			if (process == null)
				return "No process is being debugged";
			if (process.IsRunning)
				return "The process is running";
			if (frame == null)
				return "No current execution frame";
			
			try {
				var val = WindowsDebugger.Evaluate(code, allowSetValue: true);
				return ExpressionEvaluationVisitor.FormatValue(WindowsDebugger.EvalThread, val);
			} catch (GetValueException e) {
				return e.Message;
			}
		}
		
		protected override string Prompt {
			get {
				return "> ";
			}
		}
		
		public ConsolePad()
		{
			WindowsDebugger debugger = (WindowsDebugger)SD.Debugger;
		}
		
		protected override void AbstractConsolePadTextEntered(object sender, TextCompositionEventArgs e)
		{
			StackFrame frame = WindowsDebugger.CurrentStackFrame;
			if (e.Text == "." && frame != null)
				ShowDotCompletion(frame, console.CommandText);
		}
		
		void ShowDotCompletion(StackFrame frame, string currentText)
		{
			var fileName = new ICSharpCode.Core.FileName(frame.NextStatement.Filename);
			var textLocation = new TextLocation(frame.NextStatement.StartLine, frame.NextStatement.StartColumn);
			var binding = DebuggerDotCompletion.PrepareDotCompletion(currentText, SD.ParserService.ResolveContext(fileName, textLocation));
			if (binding == null) return;
			binding.HandleKeyPressed(console.TextEditor, '.');
		}
		
		protected override ToolBar BuildToolBar()
		{
			return ToolBarService.CreateToolBar(console, this, debuggerConsoleToolBarTreePath);
		}
	}
}
