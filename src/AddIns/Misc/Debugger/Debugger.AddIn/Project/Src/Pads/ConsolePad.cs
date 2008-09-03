// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

using System;
using Debugger;
using Debugger.AddIn;
using ICSharpCode.NRefactory;
using ICSharpCode.SharpDevelop.Debugging;
using ICSharpCode.SharpDevelop.DefaultEditor.Gui.Editor;
using ICSharpCode.SharpDevelop.Services;
using ICSharpCode.TextEditor;

namespace ICSharpCode.SharpDevelop.Gui.Pads
{
	public class ConsolePad : TextEditorBasedPad
	{
		ConsoleControl editor = new ConsoleControl();
		
		public override TextEditorControl TextEditorControl {
			get {
				return editor;
			}
		}
		
		public ConsolePad()
		{
			WindowsDebugger debugger = (WindowsDebugger)DebuggerService.CurrentDebugger;
			
			debugger.ProcessSelected += delegate(object sender, ProcessEventArgs e) {
				editor.Process = e.Process;
			};
			editor.Process = debugger.DebuggedProcess;
		}
	}
	
	class ConsoleControl : CommandPromptControl
	{
		Process process;
		
		public Process Process {
			get { return process; }
			set { process = value; }
		}
		
		public ConsoleControl()
		{
			SetHighlighting("C#");
			PrintPrompt();
		}
		
		protected override void PrintPromptInternal()
		{
			Append("> ");
		}
		
		protected override void AcceptCommand(string command)
		{
			if (!string.IsNullOrEmpty(command)) {
				string result = Evaluate(command);
				Append(Environment.NewLine);
				if (!string.IsNullOrEmpty(result)) {
					Append(result + Environment.NewLine);
				}
				PrintPrompt();
			}
		}
		
		string Evaluate(string code)
		{
			if (process == null) {
				return "No process is being debugged";
			}
			if (process.IsRunning) {
				return "The process is running";
			}
			try {
				Value val = AstEvaluator.Evaluate(code, SupportedLanguage.CSharp, process.SelectedStackFrame);
				if (val == null) {
					return string.Empty;
				} if (val.IsNull) {
					return "null";
				} else {
					return val.InvokeToString();
				}
			} catch (GetValueException e) {
				return e.Message;
			}
		}
	}
}
