// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.IO;
using System.Windows;
using System.Windows.Input;

using ICSharpCode.AvalonEdit;
using ICSharpCode.Core;
using ICSharpCode.NRefactory;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Debugging;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Dom.NRefactoryResolver;
using ICSharpCode.SharpDevelop.Editor.CodeCompletion;
using ICSharpCode.SharpDevelop.Gui.Pads;
using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.SharpDevelop.Services;

namespace Debugger.AddIn.Pads
{
	/// <summary>
	/// Interaction logic for WatchBox.xaml
	/// </summary>
	public partial class WatchInputBox : BaseWatchBox
	{
		private NRefactoryResolver resolver;	
		private string language;
		
		public WatchInputBox(string text, string caption) : base()
		{
			InitializeComponent();
			
			// UI
			text = StringParser.Parse(text);
			this.Title = StringParser.Parse(caption);
			this.ConsolePanel.Content = console;

			if (ProjectService.CurrentProject == null) return;
			
			// get language
			language = ProjectService.CurrentProject.Language;			
			resolver = new NRefactoryResolver(LanguageProperties.GetLanguage(language));
			
			// FIXME set language
			if (language == "VB" || language == "VBNet") {
				console.SetHighlighting("VBNET");
			}
			else {
				console.SetHighlighting("C#");
			}
			
			// get process
			WindowsDebugger debugger = (WindowsDebugger)DebuggerService.CurrentDebugger;
			
			debugger.ProcessSelected += delegate(object sender, ProcessEventArgs e) {
				this.Process = e.Process;
			};
			this.Process = debugger.DebuggedProcess;
		}
		
		private Process Process { get; set; }
		
		protected override void AbstractConsolePadTextEntered(object sender, TextCompositionEventArgs e)
		{
			if (this.Process == null || this.Process.IsRunning)
				return;
			
			if (this.Process.SelectedStackFrame == null || this.Process.SelectedStackFrame.NextStatement == null)
				return;
			
			foreach (char ch in e.Text) {
				if (ch == '.') {
					ShowDotCompletion(console.CommandText);
				}
			}
		}
		
		private void ShowDotCompletion(string currentText)
		{
			var seg = Process.SelectedStackFrame.NextStatement;
			
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
		
		private bool CheckSyntax()
		{
			string command = console.CommandText.Trim();
			
			// FIXME workaround the NRefactory issue that needs a ; at the end
			if (language == "C#") {
				if(!command.EndsWith(";"))
					command += ";";
				// FIXME only one string should be available; highlighting expects C#, supproted language, CSharp
				language = "CSharp";
			}
			
			SupportedLanguage supportedLanguage = (SupportedLanguage)Enum.Parse(typeof(SupportedLanguage), language.ToString(), true);
			using (var parser = ParserFactory.CreateParser(supportedLanguage, new StringReader(command))) {
				parser.ParseExpression();
				if (parser.Errors.Count > 0) {
					MessageService.ShowError(parser.Errors.ErrorOutput);
					return false;
				}
			}
			
			return true;
		}
		
		private void AcceptButton_Click(object sender, RoutedEventArgs e)
		{
			if (!this.CheckSyntax())
				return;
			
			this.DialogResult = true;
			this.Close();
		}
		
		private void CancelButton_Click(object sender, RoutedEventArgs e)
		{
			DialogResult = false;
			this.Close();
		}
	}
}