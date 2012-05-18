// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

using ICSharpCode.AvalonEdit;
using ICSharpCode.Core;
using ICSharpCode.NRefactory;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Debugging;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Dom.NRefactoryResolver;
using ICSharpCode.SharpDevelop.Editor;
using ICSharpCode.SharpDevelop.Editor.CodeCompletion;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Gui.Pads;
using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.SharpDevelop.Services;

namespace Debugger.AddIn.Pads.Controls
{
	public partial class WatchListAutoCompleteCell : UserControl
	{
		string language;
		
		protected ConsoleControl console;
		
		public static readonly DependencyProperty CommandTextProperty =
			DependencyProperty.Register("CommandText", typeof(string), typeof(WatchListAutoCompleteCell),
			                            new UIPropertyMetadata(null, new PropertyChangedCallback(OnCommandTextChanged)));
		
		NRefactoryResolver resolver;
		
		public event EventHandler CommandEntered;
		
		public WatchListAutoCompleteCell()
		{
			InitializeComponent();
			
			console = new ConsoleControl();
			console.TextAreaTextEntered += new TextCompositionEventHandler(consoleControl_TextAreaTextEntered);
			console.TextAreaPreviewKeyDown += new KeyEventHandler(console_TextAreaPreviewKeyDown);
			console.LostFocus += new RoutedEventHandler(console_LostFocus);
			console.HideScrollBar();
			ConsolePanel.Content = console;
			
			// get language
			if (ProjectService.CurrentProject == null)
				language = "C#";
			else
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
		
		Process Process { get; set; }
		
		/// <summary>
		/// Gets/sets the command text displayed at the command prompt.
		/// </summary>
		public string CommandText {
			get { return console.CommandText.Trim(); }
			set { console.CommandText = value; }
		}
		
		ITextEditor TextEditor {
			get {
				return console.TextEditor;
			}
		}
		
		void console_TextAreaPreviewKeyDown(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Return || e.Key == Key.Escape) {
				
				if (e.Key == Key.Escape)
					CommandText = string.Empty;
				else {
					if(!CheckSyntax())
						return;
				}
				
				if (CommandEntered != null)
					CommandEntered(this, EventArgs.Empty);
				
				e.Handled = true;
			}
		}
		
		void console_LostFocus(object sender, RoutedEventArgs e)
		{
			if (string.IsNullOrEmpty(CommandText) || !this.CheckSyntax())
				return;
			
			if (CommandEntered != null)
				CommandEntered(this, EventArgs.Empty);
		}
		
		bool CheckSyntax()
		{
			string command = CommandText;
			
			// FIXME workaround the NRefactory issue that needs a ; at the end
			if (language == "C#" || language == "CSharp") {
				if (!command.EndsWith(";"))
					command += ";";
				// FIXME only one string should be available; highlighting expects C#, supported language, CSharp
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
		
		void consoleControl_TextAreaTextEntered(object sender, TextCompositionEventArgs e)
		{
			foreach (char ch in e.Text) {
				if (ch == '.') {
					ShowDotCompletion(console.CommandText);
				}
			}
		}
		
		void ShowDotCompletion(string currentText)
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
		
		static void OnCommandTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			var cell = d as WatchListAutoCompleteCell;
			if (cell != null && e.NewValue != null) {
				cell.CommandText = e.NewValue.ToString();
			}
		}
	}
}