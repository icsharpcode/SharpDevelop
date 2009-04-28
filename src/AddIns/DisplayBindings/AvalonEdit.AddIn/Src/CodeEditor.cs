// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <author name="Daniel Grunwald"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

using ICSharpCode.AvalonEdit.CodeCompletion;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Gui;
using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.AvalonEdit.Indentation;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Bookmarks;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Editor;

namespace ICSharpCode.AvalonEdit.AddIn
{
	/// <summary>
	/// Integrates AvalonEdit with SharpDevelop.
	/// </summary>
	public class CodeEditor : Grid
	{
		QuickClassBrowser quickClassBrowser;
		readonly TextEditor primaryTextEditor;
		readonly CodeEditorAdapter primaryTextEditorAdapter;
		TextEditor secondaryTextEditor;
		CodeEditorAdapter secondaryTextEditorAdapter;
		readonly IconBarManager iconBarManager;
		readonly TextMarkerService textMarkerService;
		
		public TextEditor PrimaryTextEditor {
			get { return primaryTextEditor; }
		}
		
		public TextEditor ActiveTextEditor {
			get { return primaryTextEditor; }
		}
		
		TextDocument document;
		
		public TextDocument Document {
			get {
				return document;
			}
			set {
				if (document != value) {
					document = value;
					
					if (DocumentChanged != null) {
						DocumentChanged(this, EventArgs.Empty);
					}
				}
			}
		}
		
		public event EventHandler DocumentChanged;
		
		public IDocument DocumentAdapter {
			get { return primaryTextEditorAdapter.Document; }
		}
		
		
		public CodeEditorAdapter ActiveTextEditorAdapter {
			get { return GetAdapter(this.ActiveTextEditor); }
		}
		
		CodeEditorAdapter GetAdapter(TextEditor editor)
		{
			if (editor == secondaryTextEditor)
				return secondaryTextEditorAdapter;
			else
				return primaryTextEditorAdapter;
		}
		
		public IconBarManager IconBarManager {
			get { return iconBarManager; }
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
		
		public void Redraw(ISegment segment, DispatcherPriority priority)
		{
			primaryTextEditor.TextArea.TextView.Redraw(segment, priority);
			if (secondaryTextEditor != null) {
				secondaryTextEditor.TextArea.TextView.Redraw(segment, priority);
			}
		}
		
		public CodeEditor()
		{
			this.CommandBindings.Add(new CommandBinding(SharpDevelopRoutedCommands.SplitView, OnSplitView));
			
			textMarkerService = new TextMarkerService(this);
			iconBarManager = new IconBarManager();
			
			primaryTextEditor = CreateTextEditor();
			primaryTextEditorAdapter = (CodeEditorAdapter)primaryTextEditor.TextArea.GetService(typeof(ITextEditor));
			Debug.Assert(primaryTextEditorAdapter != null);
			
			this.Document = primaryTextEditor.Document;
			primaryTextEditor.SetBinding(TextEditor.DocumentProperty, new Binding("Document") { Source = this });
			
			this.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
			this.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
			SetRow(primaryTextEditor, 1);
			
			this.Children.Add(primaryTextEditor);
		}
		
		protected virtual TextEditor CreateTextEditor()
		{
			TextEditor textEditor = new TextEditor();
			CodeEditorAdapter adapter = new CodeEditorAdapter(this, textEditor);
			TextView textView = textEditor.TextArea.TextView;
			textView.Services.AddService(typeof(ITextEditor), adapter);
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
			
			textView.BackgroundRenderers.Add(textMarkerService);
			textView.LineTransformers.Add(textMarkerService);
			textView.Services.AddService(typeof(ITextMarkerService), textMarkerService);
			
			textView.Services.AddService(typeof(IBookmarkMargin), iconBarManager);
			var iconBarMargin = new IconBarMargin(iconBarManager) { TextView = textView };
			textEditor.TextArea.LeftMargins.Insert(0, iconBarMargin);
			
			return textEditor;
		}
		
		// always use primary text editor for loading/saving
		// (the file encoding is stored only there)
		public void Load(Stream stream)
		{
			primaryTextEditor.Load(stream);
		}
		
		public void Save(Stream stream)
		{
			primaryTextEditor.Save(stream);
		}
		
		void OnSplitView(object sender, ExecutedRoutedEventArgs e)
		{
			if (secondaryTextEditor == null) {
				// create secondary editor
				this.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
				secondaryTextEditor = CreateTextEditor();
				secondaryTextEditorAdapter = (CodeEditorAdapter)secondaryTextEditor.TextArea.GetService(typeof(ITextEditor));
				Debug.Assert(primaryTextEditorAdapter != null);
				
				secondaryTextEditor.SetBinding(TextEditor.DocumentProperty,
				                               new Binding(TextEditor.DocumentProperty.Name) { Source = primaryTextEditor });
				secondaryTextEditor.TextArea.SetBinding(TextArea.IndentationStrategyProperty,
				                                        new Binding(TextArea.IndentationStrategyProperty.Name) { Source = primaryTextEditor.TextArea });
				secondaryTextEditor.SyntaxHighlighting = primaryTextEditor.SyntaxHighlighting;
				
				SetRow(secondaryTextEditor, 2);
				this.Children.Add(secondaryTextEditor);
			} else {
				// remove secondary editor
				this.Children.Remove(secondaryTextEditor);
				secondaryTextEditor = null;
				secondaryTextEditorAdapter = null;
				this.RowDefinitions.RemoveAt(this.RowDefinitions.Count - 1);
			}
		}
		
		void caret_PositionChanged(object sender, EventArgs e)
		{
			InvalidateQuickClassBrowserCaretPosition();
		}
		
		bool quickClassBrowserCaretPositionInvalid;
		
		/// <summary>
		/// Only call 'SelectItemAtCaretPosition' once when the caret position
		/// changes multiple times (e.g. running refactoring which causes lots of caret changes).
		/// </summary>
		void InvalidateQuickClassBrowserCaretPosition()
		{
			if (!quickClassBrowserCaretPositionInvalid) {
				quickClassBrowserCaretPositionInvalid = true;
				Dispatcher.BeginInvoke(
					DispatcherPriority.Normal,
					new Action(
						delegate {
							quickClassBrowserCaretPositionInvalid = false;
							if (quickClassBrowser != null) {
								quickClassBrowser.SelectItemAtCaretPosition(this.ActiveTextEditorAdapter.Caret.Position);
							}
						}));
			}
		}
		
		public void JumpTo(int line, int column)
		{
			// the adapter sets the caret position and takes care of scrolling
			this.ActiveTextEditorAdapter.JumpTo(line, column);
			this.ActiveTextEditor.Focus();
		}
		
		ToolTip toolTip;

		void textEditor_MouseHover(object sender, MouseEventArgs e)
		{
			TextEditor textEditor = (TextEditor)sender;
			ToolTipRequestEventArgs args = new ToolTipRequestEventArgs(GetAdapter(textEditor));
			var pos = textEditor.GetPositionFromPoint(e.GetPosition(textEditor));
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
			
			TextArea textArea = (TextArea)sender;
			ITextEditor adapter = (ITextEditor)textArea.GetService(typeof(ITextEditor));
			Debug.Assert(adapter != null);
			
			foreach (char c in e.Text) {
				foreach (ICodeCompletionBinding cc in CodeCompletionBindings) {
					CodeCompletionKeyPressResult result = cc.HandleKeyPress(adapter, c);
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
			TextEditor textEditor = (TextEditor)sender;
			if (completionWindow != null)
				completionWindow.Close();
			foreach (ICodeCompletionBinding cc in CodeCompletionBindings) {
				if (cc.CtrlSpace(GetAdapter(textEditor))) {
					e.Handled = true;
					break;
				}
			}
		}
		
		void OnDeleteLine(object sender, ExecutedRoutedEventArgs e)
		{
			TextEditor textEditor = (TextEditor)sender;
			e.Handled = true;
			using (textEditor.Document.RunUpdate()) {
				DocumentLine currentLine = textEditor.Document.GetLineByNumber(textEditor.TextArea.Caret.Line);
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
						primaryTextEditor.TextArea.IndentationStrategy = new IndentationStrategyAdapter(primaryTextEditorAdapter, value);
					else
						primaryTextEditor.TextArea.IndentationStrategy = new DefaultIndentationStrategy();
					// no need to update the secondary text editor - its IndentationStrategy property will
					// update using a binding.
				}
			}
		}
		
		public IHighlightingDefinition SyntaxHighlighting {
			get { return primaryTextEditor.SyntaxHighlighting; }
			set {
				primaryTextEditor.SyntaxHighlighting = value;
				if (secondaryTextEditor != null) {
					secondaryTextEditor.SyntaxHighlighting = value;
				}
			}
		}
		
		void FetchParseInformation()
		{
			ParseInformationUpdated(ParserService.GetParseInformation(this.FileName));
		}
		
		public void ParseInformationUpdated(ParseInformation parseInfo)
		{
			if (parseInfo != null) {
				if (quickClassBrowser == null) {
					quickClassBrowser = new QuickClassBrowser();
					quickClassBrowser.JumpAction = JumpTo;
					SetRow(quickClassBrowser, 0);
					this.Children.Add(quickClassBrowser);
				}
				quickClassBrowser.Update(parseInfo.MostRecentCompilationUnit);
				InvalidateQuickClassBrowserCaretPosition();
			} else {
				if (quickClassBrowser != null) {
					this.Children.Remove(quickClassBrowser);
					quickClassBrowser = null;
				}
			}
		}
	}
}
