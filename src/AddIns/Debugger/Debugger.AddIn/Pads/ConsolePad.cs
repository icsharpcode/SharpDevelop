// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the BSD license (for details please see \src\AddIns\Debugger\Debugger.AddIn\license.txt)

using System;
using System.Windows.Controls;
using System.Windows.Input;

using Debugger;
using ICSharpCode.Core.Presentation;
using ICSharpCode.NRefactory;
using ICSharpCode.NRefactory.Visitors;
using ICSharpCode.SharpDevelop.Debugging;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Dom.NRefactoryResolver;
using ICSharpCode.SharpDevelop.Editor.CodeCompletion;
using ICSharpCode.SharpDevelop.Services;

namespace ICSharpCode.SharpDevelop.Gui.Pads
{
	public class ConsolePad : AbstractConsolePad
	{
		SupportedLanguage language;
		NRefactoryResolver resolver;
		
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
					Append(Environment.NewLine + result);
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
				var debugger = (WindowsDebugger)DebuggerService.CurrentDebugger;
				StackFrame frame = debugger.DebuggedProcess.GetCurrentExecutingFrame();
				if (frame == null) return "No current execution frame";
				
				object data = debugger.debuggerDecompilerService.GetLocalVariableIndex(frame.MethodInfo.DeclaringType.MetadataToken,
				                                                                       frame.MethodInfo.MetadataToken,
				                                                                       code);
				Value val = ExpressionEvaluator.Evaluate(code, SelectedLanguage, frame, data);
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
			OnLanguageChanged();
		}
		
		void OnLanguageChanged()
		{
			switch (SelectedLanguage) {
				case SupportedLanguage.CSharp:
					resolver = new NRefactoryResolver(LanguageProperties.CSharp);
					SetHighlighting("C#");
					break;
				case SupportedLanguage.VBNet:
					resolver = new NRefactoryResolver(LanguageProperties.VBNet);
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
		
		protected override void AbstractConsolePadTextEntered(object sender, TextCompositionEventArgs e)
		{
			if (this.process == null || this.process.IsRunning)
				return;
			
			StackFrame frame = this.process.GetCurrentExecutingFrame();
			if (frame == null)
				return;
			
			foreach (char ch in e.Text) {
				if (ch == '.') {
					ShowDotCompletion(console.CommandText);
				}
			}
		}
		
		void ShowDotCompletion(string currentText)
		{
			StackFrame frame = this.process.GetCurrentExecutingFrame();
			if (frame == null)
				return;
			
			var seg = frame.NextStatement;
			if (seg == null)
				return;
			
			var expressionFinder = ParserService.GetExpressionFinder(seg.Filename);
			var info = ParserService.GetParseInformation(seg.Filename);
			
			string text = ParserService.GetParseableFileContent(seg.Filename).Text;
			
			int currentOffset = TextEditor.Caret.Offset - console.CommandOffset - 1;
			
			var expr = expressionFinder.FindExpression(currentText, currentOffset);
			
			expr.Region = new DomRegion(seg.StartLine, seg.StartColumn, seg.EndLine, seg.EndColumn);
			
			var rr = resolver.Resolve(expr, info, text);
			
			if (rr != null) {
				TextEditor.ShowCompletionWindow(new DotCodeCompletionItemProvider().GenerateCompletionListForResolveResult(rr, expr.Context));
			}
		}
		
		protected override ToolBar BuildToolBar()
		{
			return ToolBarService.CreateToolBar(console, this, debuggerConsoleToolBarTreePath);
		}
	}
}
