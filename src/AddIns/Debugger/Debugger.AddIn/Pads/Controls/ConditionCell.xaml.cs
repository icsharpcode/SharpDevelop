// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

using ICSharpCode.AvalonEdit;
using ICSharpCode.Core;
using ICSharpCode.Core.Presentation;
using ICSharpCode.NRefactory;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Bookmarks.Pad.Controls;
using ICSharpCode.SharpDevelop.Debugging;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Dom.NRefactoryResolver;
using ICSharpCode.SharpDevelop.Editor;
using ICSharpCode.SharpDevelop.Editor.CodeCompletion;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Project;

namespace Debugger.AddIn.Pads.Controls
{
	public partial class ConditionCell : UserControl
	{
		private string language;
		
		protected ConsoleControl console;
		
		public static readonly DependencyProperty CommandTextProperty =
			DependencyProperty.Register("CommandText", typeof(string), typeof(ConditionCell),
			                            new UIPropertyMetadata(null, new PropertyChangedCallback(OnCommandTextChanged)));
		
		private NRefactoryResolver resolver;
		
		public ConditionCell()
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
		}
		
		/// <summary>
		/// Gets/sets the command text displayed at the command prompt.
		/// </summary>
		public string CommandText { 
			get { return console.CommandText.Trim(); }
			set { console.CommandText = value; }
		}
		
		private BreakpointBookmark Breakpoint {
			get {
				var model = Model;
				return model.Mark as BreakpointBookmark;
			}
		}
		
		private ListViewPadItemModel Model {
			get { return Tag as ListViewPadItemModel; }
		}
		
		private ITextEditor TextEditor {
			get {
				return console.TextEditor;
			}
		}
		
		private void console_TextAreaPreviewKeyDown(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Return || e.Key == Key.Escape) {
				
				if (e.Key == Key.Escape) 
					CommandText = string.Empty;
				else {
					if(!CheckSyntax())
					return;
				}
				
				UpdateBreakpoint();
				
				e.Handled = true;
			}
		}
		
		private void console_LostFocus(object sender, RoutedEventArgs e)
		{
			if (string.IsNullOrEmpty(CommandText) || !this.CheckSyntax())
				return;
			
			UpdateBreakpoint();
		}
		
		private void UpdateBreakpoint()
		{
			Breakpoint.Condition = CommandText;
			Model.Condition = CommandText;				
			Breakpoint.ScriptLanguage = language;
			Model.Language = language;
					
			if (!string.IsNullOrEmpty(console.CommandText)) {
				Breakpoint.Action = BreakpointAction.Condition;
				if (Breakpoint.IsEnabled)
					Model.Image = BreakpointBookmark.BreakpointConditionalImage.ImageSource;
			}
			else {
				Breakpoint.Action = BreakpointAction.Break;
				if (Breakpoint.IsEnabled)
					Model.Image = BreakpointBookmark.BreakpointImage.ImageSource;
			}
		}
		
		private bool CheckSyntax()
		{
			string command = CommandText;
			if (string.IsNullOrEmpty(command))
				return true;
			
			// FIXME workaround the NRefactory issue that needs a ; at the end
			if (language == "C#") {
				if(!command.EndsWith(";"))
					command += ";";
				// FIXME only one string should be available; highlighting expects C#, supproted language, CSharp
				language = "CSharp";
			}
			
			SupportedLanguage supportedLanguage = (SupportedLanguage)Enum.Parse(typeof(SupportedLanguage), language.ToString(), true);
			using (var parser = ParserFactory.CreateParser(supportedLanguage, new StringReader(TextEditor.Document.Text))) {
				parser.ParseExpression();
				if (parser.Errors.Count > 0) {
					MessageService.ShowError(parser.Errors.ErrorOutput);
					return false;
				}
			}
			
			return true;
		}
	
		private void consoleControl_TextAreaTextEntered(object sender, TextCompositionEventArgs e)
		{	
			foreach (char ch in e.Text) {
				if (ch == '.') {
					ShowDotCompletion(console.CommandText);
				}
			}
		}
		
		private void ShowDotCompletion(string currentText)
		{
			var seg = Breakpoint;
			
			var expressionFinder = ParserService.GetExpressionFinder(seg.FileName.ToString());
			var info = ParserService.GetParseInformation(seg.FileName.ToString());
			
			string text = ParserService.GetParseableFileContent(seg.FileName.ToString()).Text;
			
			int currentOffset = TextEditor.Caret.Offset - console.CommandOffset - 1;
			
			var expr = expressionFinder.FindExpression(currentText, currentOffset);
			
			expr.Region = new DomRegion(seg.LineNumber, seg.ColumnNumber, seg.LineNumber, seg.ColumnNumber);
			
			var rr = resolver.Resolve(expr, info, text);
			
			if (rr != null) {
				TextEditor.ShowCompletionWindow(new DotCodeCompletionItemProvider().GenerateCompletionListForResolveResult(rr, expr.Context));
			}
		}
		
		private static void OnCommandTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			var cell = d as ConditionCell;
			cell.CommandText = e.NewValue.ToString();
		}
	}
}