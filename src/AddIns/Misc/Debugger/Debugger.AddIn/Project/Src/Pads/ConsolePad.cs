// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections;
using System.Windows.Controls;

using Debugger;
using Debugger.AddIn;
using ICSharpCode.Core;
using ICSharpCode.Core.Presentation;
using ICSharpCode.NRefactory;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Debugging;
using ICSharpCode.SharpDevelop.DefaultEditor.Gui.Editor;
using ICSharpCode.SharpDevelop.Services;

namespace ICSharpCode.SharpDevelop.Gui.Pads
{
	public class ConsolePad : AbstractConsolePad
	{
		SupportedLanguage language;
		
		const string debuggerConsoleToolBarTreePath = "/SharpDevelop/Pads/ConsolePad/ToolBar";
		
		public SupportedLanguage SelectedLanguage {
			get { return language; }
			set {
				this.language = value;
				OnLanguageChanged();
			}
		}
		
		protected override bool AcceptCommand(string command)
		{
			if (!string.IsNullOrEmpty(command)) {
				string result = Evaluate(command);
				if (!string.IsNullOrEmpty(result)) {
					Append(result + Environment.NewLine);
				}
			}
			return true;
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
				Value val = ExpressionEvaluator.Evaluate(code, SelectedLanguage, process.SelectedStackFrame);
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
			switch (SelectedLanguage) {
				case SupportedLanguage.CSharp:
					SetHighlighting("C#");
					break;
				case SupportedLanguage.VBNet:
					SetHighlighting("VBNET");
					break;
			}
		}
		
		void OnLanguageChanged()
		{
			switch (SelectedLanguage) {
				case SupportedLanguage.CSharp:
					SetHighlighting("C#");
					break;
				case SupportedLanguage.VBNet:
					SetHighlighting("VBNET");
					break;
			}
		}
		
		public ConsolePad()
		{			
			WindowsDebugger debugger = (WindowsDebugger)DebuggerService.CurrentDebugger;
			
			debugger.ProcessSelected += delegate(object sender, ProcessEventArgs e) {
				this.Process = e.Process;
			};
			this.Process = debugger.DebuggedProcess;
		}
		
		protected override ToolBar BuildToolBar()
		{
			return ToolBarService.CreateToolBar(this, debuggerConsoleToolBarTreePath);
		}
	}
}
