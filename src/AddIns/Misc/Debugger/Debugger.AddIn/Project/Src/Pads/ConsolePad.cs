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
	public class ConsolePad : AbstractConsolePad
	{
		protected override void AcceptCommand(string command)
		{
			if (!string.IsNullOrEmpty(command)) {
				string result = Evaluate(command);
				if (!string.IsNullOrEmpty(result)) {
					Append(result + Environment.NewLine);
				}
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
				Value val = ExpressionEvaluator.Evaluate(code, SupportedLanguage.CSharp, process.SelectedStackFrame);
				return ExpressionEvaluator.FormatValue(val);
			} catch (GetValueException e) {
				return e.Message;
			}
		}
		
		Process process;
		
		public Process Process {
			get { return process; }
			set { process = value; }
		}
		
		protected override string Prompt {
			get {
				return "> ";
			}
		}
		
		protected override void InitializeConsole()
		{
			base.InitializeConsole();
			
			SetHighlighting("C#");
		}
		
		public ConsolePad()
		{
			WindowsDebugger debugger = (WindowsDebugger)DebuggerService.CurrentDebugger;
			
			debugger.ProcessSelected += delegate(object sender, ProcessEventArgs e) {
				this.Process = e.Process;
			};
			this.Process = debugger.DebuggedProcess;
		}
	}
}
