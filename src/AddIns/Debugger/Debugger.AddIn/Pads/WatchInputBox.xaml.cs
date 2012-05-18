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
using ICSharpCode.SharpDevelop.Editor;
using ICSharpCode.SharpDevelop.Editor.CodeCompletion;
using ICSharpCode.SharpDevelop.Gui;
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
		NRefactoryResolver resolver;
		SupportedLanguage language;
		
		public SupportedLanguage ScriptLanguage {
			get { return language; }
		}
		
		public WatchInputBox(string text, string caption) : base()
		{
			InitializeComponent();
			
			// UI
			text = StringParser.Parse(text);
			this.Title = StringParser.Parse(caption);
			this.ConsolePanel.Content = console;

			if (ProjectService.CurrentProject == null)
				language = GetLanguageFromActiveViewContent();
			else
				language = GetLanguage(ProjectService.CurrentProject.Language);
			
			resolver = new NRefactoryResolver(LanguageProperties.GetLanguage(language.ToString()));
			
			switch (language) {
				case SupportedLanguage.CSharp:
					console.SetHighlighting("C#");
					break;
				case SupportedLanguage.VBNet:
					console.SetHighlighting("VBNET");
					break;
			}
			
			// get process
			this.Process = ((WindowsDebugger)DebuggerService.CurrentDebugger).DebuggedProcess;
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
		
		bool CheckSyntax()
		{
			string command = console.CommandText.Trim();
			
			// FIXME workaround the NRefactory issue that needs a ; at the end
			if (language == SupportedLanguage.CSharp && !command.EndsWith(";"))
				command += ";";
			
			using (var parser = ParserFactory.CreateParser(language, new StringReader(command))) {
				parser.ParseExpression();
				if (parser.Errors.Count > 0) {
					MessageService.ShowError(parser.Errors.ErrorOutput);
					return false;
				}
			}
			
			return true;
		}
		
		SupportedLanguage GetLanguage(string language)
		{
			if ("VBNet".Equals(language, StringComparison.OrdinalIgnoreCase)
			    || "VB".Equals(language, StringComparison.OrdinalIgnoreCase)
			    || "VB.NET".Equals(language, StringComparison.OrdinalIgnoreCase))
				return SupportedLanguage.VBNet;
			
			return SupportedLanguage.CSharp;
		}
		
		/// <summary>
		/// Gets the language used in the currently active view content. This is useful, when there is no project
		/// opened and we still want to add watches (i.e. the debugger is attached to an existing process without a solution).
		/// </summary>
		SupportedLanguage GetLanguageFromActiveViewContent()
		{
			ITextEditorProvider provider = WorkbenchSingleton.Workbench.ActiveViewContent as ITextEditorProvider;
			
			if (provider != null && provider.TextEditor != null) {
				string extension = Path.GetExtension(provider.TextEditor.FileName).ToLowerInvariant();
				switch (extension) {
					case ".cs":
						return SupportedLanguage.CSharp;
					case ".vb":
						return SupportedLanguage.VBNet;
				}
			}
			
			return SupportedLanguage.CSharp;
		}
		
		void AcceptButton_Click(object sender, RoutedEventArgs e)
		{
			if (!this.CheckSyntax())
				return;
			
			this.DialogResult = true;
			this.Close();
		}
		
		void CancelButton_Click(object sender, RoutedEventArgs e)
		{
			DialogResult = false;
			this.Close();
		}
	}
}