// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <author name="Daniel Grunwald"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

using ICSharpCode.AvalonEdit.CodeCompletion;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Gui;
using ICSharpCode.AvalonEdit.Indentation;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Bookmarks;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Editor;
using System.Windows.Threading;

namespace ICSharpCode.AvalonEdit.AddIn
{
	/// <summary>
	/// Integrates AvalonEdit with SharpDevelop.
	/// </summary>
	public class CodeEditor : DockPanel
	{
		readonly QuickClassBrowser quickClassBrowser = new QuickClassBrowser();
		readonly TextEditor textEditor = new TextEditor();
		readonly CodeEditorAdapter textEditorAdapter;
		readonly IconBarMargin iconBarMargin;
		readonly TextMarkerService textMarkerService;
		
		public TextEditor TextEditor {
			get { return textEditor; }
		}
		
		public TextDocument Document {
			get { return textEditor.Document; }
			set { textEditor.Document = value; }
		}
		
		public CodeEditorAdapter TextEditorAdapter {
			get { return textEditorAdapter; }
		}
		
		public IconBarMargin IconBarMargin {
			get { return iconBarMargin; }
		}
		
		string fileName;
		
		public string FileName {
			get { return fileName; }
			set {
				if (fileName != value) {
					fileName = value;
					
					FetchParseInformation();
				}
			}
		}
		
		public CodeEditor()
		{
			textEditorAdapter = new CodeEditorAdapter(this);
			TextView textView = textEditor.TextArea.TextView;
			textView.Services.AddService(typeof(ITextEditor), textEditorAdapter);
			textView.Services.AddService(typeof(CodeEditor), this);
			
			textEditor.Background = Brushes.White;
			textEditor.FontFamily = new FontFamily("Consolas");
			textEditor.FontSize = 13;
			textEditor.TextArea.TextEntered += TextArea_TextInput;
			textEditor.MouseHover += textEditor_MouseHover;
			textEditor.MouseHoverStopped += textEditor_MouseHoverStopped;
			textEditor.TextArea.Caret.PositionChanged += caret_PositionChanged;
			textEditor.TextArea.DefaultInputHandler.CommandBindings.Add(
				new CommandBinding(CustomCommands.CtrlSpaceCompletion, OnCodeCompletion));
			textEditor.TextArea.DefaultInputHandler.CommandBindings.Add(
				new CommandBinding(CustomCommands.DeleteLine, OnDeleteLine));
			
			this.iconBarMargin = new IconBarMargin { TextView = textView };
			textEditor.TextArea.LeftMargins.Insert(0, iconBarMargin);
			textView.Services.AddService(typeof(IBookmarkMargin), iconBarMargin);
			
			textMarkerService = new TextMarkerService(this);
			textView.BackgroundRenderers.Add(textMarkerService);
			textView.LineTransformers.Add(textMarkerService);
			textView.Services.AddService(typeof(ITextMarkerService), textMarkerService);
			
			quickClassBrowser.JumpAction = quickClassBrowser_Jump;
			SetDock(quickClassBrowser, Dock.Top);
			this.Children.Add(quickClassBrowser);
			this.Children.Add(textEditor);
		}

		void caret_PositionChanged(object sender, EventArgs e)
		{
			InvalidateQuickClassBrowserCaretPosition();
		}
		
		bool quickClassBrowserCaretPositionIsValid;
		
		/// <summary>
		/// Only call 'SelectItemAtCaretPosition' once when the caret position
		/// changes multiple times (e.g. running refactoring which causes lots of caret changes).
		/// </summary>
		void InvalidateQuickClassBrowserCaretPosition()
		{
			if (quickClassBrowserCaretPositionIsValid) {
				quickClassBrowserCaretPositionIsValid = false;
				Dispatcher.BeginInvoke(
					DispatcherPriority.Normal,
					new Action(
						delegate {
							quickClassBrowser.SelectItemAtCaretPosition(textEditorAdapter.Caret.Position);
						}));
			}
		}
		
		void quickClassBrowser_Jump(DomRegion region)
		{
			textEditor.TextArea.Caret.Position = new TextViewPosition(region.BeginLine, region.BeginColumn);
			textEditor.Focus();
		}
		
		ToolTip toolTip;

		void textEditor_MouseHover(object sender, MouseEventArgs e)
		{
			ToolTipRequestEventArgs args = new ToolTipRequestEventArgs(textEditorAdapter);
			var pos = textEditor.GetPositionFromPoint(e.GetPosition(this));
			args.InDocument = pos.HasValue;
			if (pos.HasValue) {
				args.LogicalPosition = AvalonEditDocumentAdapter.ToLocation(pos.Value);
			}
			ToolTipRequestService.RequestToolTip(args);
			if (args.ContentToShow != null) {
				if (toolTip == null) {
					toolTip = new ToolTip();
					toolTip.Closed += toolTip_Closed;
				}
				toolTip.Content = args.ContentToShow;
				toolTip.IsOpen = true;
				e.Handled = true;
			}
		}
		
		void textEditor_MouseHoverStopped(object sender, MouseEventArgs e)
		{
			if (toolTip != null) {
				toolTip.IsOpen = false;
				e.Handled = true;
			}
		}

		void toolTip_Closed(object sender, RoutedEventArgs e)
		{
			toolTip = null;
		}
		
		volatile static ReadOnlyCollection<ICodeCompletionBinding> codeCompletionBindings;
		
		public static ReadOnlyCollection<ICodeCompletionBinding> CodeCompletionBindings {
			get {
				if (codeCompletionBindings == null) {
					codeCompletionBindings = AddInTree.BuildItems<ICodeCompletionBinding>("/AddIns/DefaultTextEditor/CodeCompletion", null, false).AsReadOnly();
				}
				return codeCompletionBindings;
			}
		}
		
		void TextArea_TextInput(object sender, TextCompositionEventArgs e)
		{
			// don't start new code completion if there is still a completion window open
			if (completionWindow != null)
				return;
			
			foreach (char c in e.Text) {
				foreach (ICodeCompletionBinding cc in CodeCompletionBindings) {
					CodeCompletionKeyPressResult result = cc.HandleKeyPress(textEditorAdapter, c);
					if (result == CodeCompletionKeyPressResult.Completed) {
						if (completionWindow != null) {
							// a new CompletionWindow was shown, but does not eat the input
							// tell it to expect the text insertion
							completionWindow.ExpectInsertionBeforeStart = true;
						}
						return;
					} else if (result == CodeCompletionKeyPressResult.CompletedIncludeKeyInCompletion) {
						if (completionWindow != null) {
							if (completionWindow.StartOffset == completionWindow.EndOffset) {
								completionWindow.CloseWhenCaretAtBeginning = true;
							}
						}
						return;
					} else if (result == CodeCompletionKeyPressResult.EatKey) {
						e.Handled = true;
						return;
					}
				}
			}
		}
		
		void OnCodeCompletion(object sender, ExecutedRoutedEventArgs e)
		{
			if (completionWindow != null)
				completionWindow.Close();
			foreach (ICodeCompletionBinding cc in CodeCompletionBindings) {
				if (cc.CtrlSpace(textEditorAdapter)) {
					e.Handled = true;
					break;
				}
			}
		}
		
		void OnDeleteLine(object sender, ExecutedRoutedEventArgs e)
		{
			e.Handled = true;
			using (this.Document.RunUpdate()) {
				DocumentLine currentLine = this.Document.GetLineByNumber(textEditor.TextArea.Caret.Line);
				textEditor.Select(currentLine.Offset, currentLine.TotalLength);
				textEditor.SelectedText = string.Empty;
			}
		}
		
		CompletionWindow completionWindow;
		
		internal void NotifyCompletionWindowOpened(CompletionWindow window)
		{
			if (completionWindow != null) {
				// if there already is a completion window open, close it
				completionWindow.Close();
			}
			completionWindow = window;
			window.Closed += delegate {
				completionWindow = null;
			};
		}
		
		IFormattingStrategy formattingStrategy;
		
		public IFormattingStrategy FormattingStrategy {
			get { return formattingStrategy; }
			set {
				if (formattingStrategy != value) {
					formattingStrategy = value;
					if (value != null)
						textEditor.TextArea.IndentationStrategy = new IndentationStrategyAdapter(textEditorAdapter, value);
					else
						textEditor.TextArea.IndentationStrategy = new DefaultIndentationStrategy();
				}
			}
		}
		
		void FetchParseInformation()
		{
			ParseInformationUpdated(ParserService.GetParseInformation(this.FileName));
		}
		
		public void ParseInformationUpdated(ParseInformation parseInfo)
		{
			if (parseInfo != null)
				quickClassBrowser.Update(parseInfo.MostRecentCompilationUnit);
			else
				quickClassBrowser.Update(null);
			InvalidateQuickClassBrowserCaretPosition();
		}
	}
}
