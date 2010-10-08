using System;
using System.Windows;
using System.Windows.Input;

using ICSharpCode.AvalonEdit;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Debugging;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Dom.NRefactoryResolver;
using ICSharpCode.SharpDevelop.Editor.CodeCompletion;
using ICSharpCode.SharpDevelop.Gui.Pads;
using ICSharpCode.SharpDevelop.Services;

namespace Debugger.AddIn.Pads
{
	/// <summary>
	/// Interaction logic for WatchBox.xaml
	/// </summary>
	public partial class WatchInputBox : BaseWatchBox
	{
		private NRefactoryResolver resolver;	
		
		public WatchInputBox(string text, string caption) : base()
		{
			InitializeComponent();
			
			// UI
			text = StringParser.Parse(text);
			this.Title = StringParser.Parse(caption);
			AcceptButton.Content = StringParser.Parse("${res:Global.OKButtonText}");
			CancelButton.Content = StringParser.Parse("${res:Global.CancelButtonText}");
			this.ConsolePanel.Children.Add(console);
			
			// FIXME: for testing only
			var language = LanguageProperties.CSharp;
			resolver = new NRefactoryResolver(language);
			console.SetHighlighting("C#");
			
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
		
		private void AcceptButton_Click(object sender, RoutedEventArgs e)
		{
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
