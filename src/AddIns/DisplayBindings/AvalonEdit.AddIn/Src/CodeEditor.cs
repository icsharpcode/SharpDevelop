// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <author name="Daniel Grunwald"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.ObjectModel;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Editing;
using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.AvalonEdit.Indentation;
using ICSharpCode.AvalonEdit.Rendering;
using ICSharpCode.Core;
using ICSharpCode.Core.Presentation;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Bookmarks;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Editor;
using ICSharpCode.SharpDevelop.Editor.AvalonEdit;
using ICSharpCode.SharpDevelop.Editor.CodeCompletion;
using ICSharpCode.SharpDevelop.Editor.Commands;

namespace ICSharpCode.AvalonEdit.AddIn
{
	/// <summary>
	/// Integrates AvalonEdit with SharpDevelop.
	/// Also provides support for Split-View (showing two AvalonEdit instances using the same TextDocument)
	/// </summary>
	public class CodeEditor : Grid, IDisposable
	{
		const string contextMenuPath = "/SharpDevelop/ViewContent/AvalonEdit/ContextMenu";
		
		QuickClassBrowser quickClassBrowser;
		readonly CodeEditorView primaryTextEditor;
		readonly CodeEditorAdapter primaryTextEditorAdapter;
		CodeEditorView secondaryTextEditor;
		CodeEditorAdapter secondaryTextEditorAdapter;
		readonly IconBarManager iconBarManager;
		readonly TextMarkerService textMarkerService;
		ErrorPainter errorPainter;
		
		BracketHighlightRenderer primaryBracketRenderer;
		BracketHighlightRenderer secondaryBracketRenderer;
		
		public CodeEditorView PrimaryTextEditor {
			get { return primaryTextEditor; }
		}
		
		public CodeEditorView ActiveTextEditor {
			get { return primaryTextEditor; }
		}
		
		TextDocument document;
		
		public TextDocument Document {
			get {
				return document;
			}
			private set {
				if (document != value) {
					if (document != null)
						document.UpdateFinished -= DocumentUpdateFinished;
					
					document = value;
					
					if (document != null)
						document.UpdateFinished += DocumentUpdateFinished;
					
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
		
		public ITextEditor PrimaryTextEditorAdapter {
			get { return primaryTextEditorAdapter; }
		}
		
		public ITextEditor ActiveTextEditorAdapter {
			get { return this.ActiveTextEditor.Adapter; }
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
					
					primaryTextEditorAdapter.FileNameChanged();
					if (secondaryTextEditorAdapter != null)
						secondaryTextEditorAdapter.FileNameChanged();
					
					if (this.errorPainter != null)
						this.errorPainter.Dispose();
					
					this.errorPainter = new ErrorPainter(primaryTextEditorAdapter);
					
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
			
			// always support scrolling below the end of the document - it's better when folding is enabled
			primaryTextEditor.Options.AllowScrollBelowDocument = true;
			
			this.primaryBracketRenderer = new BracketHighlightRenderer(primaryTextEditor.TextArea.TextView);
			
			this.Document = primaryTextEditor.Document;
			primaryTextEditor.SetBinding(TextEditor.DocumentProperty, new Binding("Document") { Source = this });
			
			this.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
			this.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
			SetRow(primaryTextEditor, 1);
			
			this.Children.Add(primaryTextEditor);
		}
		
		protected virtual CodeEditorView CreateTextEditor()
		{
			CodeEditorView textEditor = new CodeEditorView();
			CodeEditorAdapter adapter = new CodeEditorAdapter(this, textEditor);
			textEditor.Adapter = adapter;
			TextView textView = textEditor.TextArea.TextView;
			textView.Services.AddService(typeof(ITextEditor), adapter);
			textView.Services.AddService(typeof(CodeEditor), this);
			
			textEditor.Background = Brushes.White;
			textEditor.FontFamily = new FontFamily("Consolas");
			textEditor.FontSize = 13;
			textEditor.ShowLineNumbers = true;
			textEditor.TextArea.TextEntering += TextAreaTextEntering;
			textEditor.TextArea.TextEntered += TextAreaTextEntered;
			textEditor.TextArea.Caret.PositionChanged += TextAreaCaretPositionChanged;
			textEditor.TextArea.DefaultInputHandler.CommandBindings.Add(
				new CommandBinding(CustomCommands.CtrlSpaceCompletion, OnCodeCompletion));
			
			textView.BackgroundRenderers.Add(textMarkerService);
			textView.LineTransformers.Add(textMarkerService);
			textView.Services.AddService(typeof(ITextMarkerService), textMarkerService);
			
			textView.Services.AddService(typeof(IBookmarkMargin), iconBarManager);
			textEditor.TextArea.LeftMargins.Insert(0, new IconBarMargin(iconBarManager));
			
			textView.Services.AddService(typeof(ParserFoldingStrategy), new ParserFoldingStrategy(textEditor.TextArea));
			
			textView.Services.AddService(typeof(ISyntaxHighlighter), new AvalonEditSyntaxHighlighterAdapter(textView));
			
			textEditor.TextArea.TextView.MouseRightButtonDown += TextViewMouseRightButtonDown;
			textEditor.TextArea.TextView.ContextMenuOpening += TextViewContextMenuOpening;
			
			return textEditor;
		}

		protected virtual void DisposeTextEditor(TextEditor textEditor)
		{
			// detach IconBarMargin from IconBarManager
			textEditor.TextArea.LeftMargins.OfType<IconBarMargin>().Single().TextView = null;
		}
		
		void TextViewContextMenuOpening(object sender, ContextMenuEventArgs e)
		{
			ITextEditor adapter = GetAdapterFromSender(sender);
			MenuService.CreateContextMenu(adapter, contextMenuPath).IsOpen = true;
		}
		
		void TextViewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
		{
			TextEditor textEditor = GetTextEditorFromSender(sender);
			var position = textEditor.GetPositionFromPoint(e.GetPosition(textEditor));
			if (position.HasValue) {
				textEditor.TextArea.Caret.Position = position.Value;
			}
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
				secondaryTextEditor.SyntaxHighlighting = primaryTextEditor.SyntaxHighlighting;
				secondaryTextEditor.Options = primaryTextEditor.Options;
				
				SetRow(secondaryTextEditor, 2);
				this.Children.Add(secondaryTextEditor);
				
				this.secondaryBracketRenderer = new BracketHighlightRenderer(secondaryTextEditor.TextArea.TextView);
				
				secondaryTextEditorAdapter.FileNameChanged();
			} else {
				// remove secondary editor
				this.Children.Remove(secondaryTextEditor);
				DisposeTextEditor(secondaryTextEditor);
				secondaryTextEditor = null;
				secondaryTextEditorAdapter.Language.Detach();
				secondaryTextEditorAdapter = null;
				this.secondaryBracketRenderer = null;
				this.RowDefinitions.RemoveAt(this.RowDefinitions.Count - 1);
			}
		}
		
		public event EventHandler CaretPositionChanged;
		bool caretPositionWasChanged;
		
		void TextAreaCaretPositionChanged(object sender, EventArgs e)
		{
			Debug.Assert(sender is Caret);
			if (sender == this.ActiveTextEditor.TextArea.Caret) {
				if (document.IsInUpdate)
					caretPositionWasChanged = true;
				else
					HandleCaretPositionChange();
			}
		}
		
		void DocumentUpdateFinished(object sender, EventArgs e)
		{
			if (caretPositionWasChanged) {
				caretPositionWasChanged = false;
				HandleCaretPositionChange();
			}
		}
		
		void HandleCaretPositionChange()
		{
			if (quickClassBrowser != null) {
				quickClassBrowser.SelectItemAtCaretPosition(this.ActiveTextEditorAdapter.Caret.Position);
			}
			
			var caret = this.ActiveTextEditor.TextArea.Caret;
			var activeAdapter = this.ActiveTextEditorAdapter;
			
			/*
			 * Special case: ITextEditor.Language guarantees that it never returns null.
			 * In this case however it can be null, since this code is called while the document is loaded.
			 * ITextEditor.Language gets set in CodeEditorAdapter.FileNameChanged, which is called after
			 * loading of the document has finished.
			 * */
			if (activeAdapter.Language != null) {
				var bracketSearchResult = activeAdapter.Language.BracketSearcher.SearchBracket(activeAdapter.Document, activeAdapter.Caret.Offset);
				if (activeAdapter == primaryTextEditorAdapter)
					this.primaryBracketRenderer.SetHighlight(bracketSearchResult);
				else
					this.secondaryBracketRenderer.SetHighlight(bracketSearchResult);
			}
			
			StatusBarService.SetCaretPosition(caret.Column, caret.Line, caret.Column);
			CaretPositionChanged.RaiseEvent(this, EventArgs.Empty);
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
		
		void TextAreaTextEntering(object sender, TextCompositionEventArgs e)
		{
			// don't start new code completion if there is still a completion window open
			if (completionWindow != null)
				return;
			
			if (e.Handled)
				return;
			
			ITextEditor adapter = GetAdapterFromSender(sender);
			
			foreach (char c in e.Text) {
				foreach (ICodeCompletionBinding cc in CodeCompletionBindings) {
					CodeCompletionKeyPressResult result = cc.HandleKeyPress(adapter, c);
					if (result == CodeCompletionKeyPressResult.Completed) {
						if (completionWindow != null) {
							// a new CompletionWindow was shown, but does not eat the input
							// tell it to expect the text insertion
							completionWindow.ExpectInsertionBeforeStart = true;
						}
						if (insightWindow != null) {
							insightWindow.ExpectInsertionBeforeStart = true;
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
		
		void TextAreaTextEntered(object sender, TextCompositionEventArgs e)
		{
			if (e.Text.Length > 0 && !e.Handled) {
				ILanguageBinding languageBinding = GetAdapterFromSender(sender).Language;
				if (languageBinding != null && languageBinding.FormattingStrategy != null) {
					char c = e.Text[0];
					// When entering a newline, AvalonEdit might use either "\r\n" or "\n", depending on
					// what was passed to TextArea.PerformTextInput. We'll normalize this to '\n'
					// so that formatting strategies don't have to handle both cases.
					if (c == '\r')
						c = '\n';
					languageBinding.FormattingStrategy.FormatLine(GetAdapterFromSender(sender), c);
					
					if (c == '\n') {
						// Immediately parse on enter.
						// This ensures we have up-to-date CC info about the method boundary when a user
						// types near the end of a method.
						ParserService.BeginParse(this.FileName, this.DocumentAdapter.CreateSnapshot());
					}
				}
			}
		}
		
		ITextEditor GetAdapterFromSender(object sender)
		{
			ITextEditorComponent textArea = (ITextEditorComponent)sender;
			ITextEditor textEditor = (ITextEditor)textArea.GetService(typeof(ITextEditor));
			if (textEditor == null)
				throw new InvalidOperationException("could not find TextEditor service");
			return textEditor;
		}
		
		CodeEditorView GetTextEditorFromSender(object sender)
		{
			ITextEditorComponent textArea = (ITextEditorComponent)sender;
			CodeEditorView textEditor = (CodeEditorView)textArea.GetService(typeof(TextEditor));
			if (textEditor == null)
				throw new InvalidOperationException("could not find TextEditor service");
			return textEditor;
		}
		
		void OnCodeCompletion(object sender, ExecutedRoutedEventArgs e)
		{
			CloseExistingCompletionWindow();
			CodeEditorView textEditor = GetTextEditorFromSender(sender);
			foreach (ICodeCompletionBinding cc in CodeCompletionBindings) {
				if (cc.CtrlSpace(textEditor.Adapter)) {
					e.Handled = true;
					break;
				}
			}
		}
		
		SharpDevelopCompletionWindow completionWindow;
		SharpDevelopInsightWindow insightWindow;
		
		void CloseExistingCompletionWindow()
		{
			if (completionWindow != null) {
				completionWindow.Close();
			}
		}
		
		void CloseExistingInsightWindow()
		{
			if (insightWindow != null) {
				insightWindow.Close();
			}
		}
		
		public SharpDevelopCompletionWindow ActiveCompletionWindow {
			get { return completionWindow; }
		}
		
		public SharpDevelopInsightWindow ActiveInsightWindow {
			get { return insightWindow; }
		}
		
		internal void ShowCompletionWindow(SharpDevelopCompletionWindow window)
		{
			CloseExistingCompletionWindow();
			completionWindow = window;
			window.Closed += delegate {
				completionWindow = null;
			};
			Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(
				delegate {
					if (completionWindow == window) {
						window.Show();
					}
				}
			));
		}
		
		internal void ShowInsightWindow(SharpDevelopInsightWindow window)
		{
			CloseExistingInsightWindow();
			insightWindow = window;
			window.Closed += delegate {
				insightWindow = null;
			};
			Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(
				delegate {
					if (insightWindow == window) {
						window.Show();
					}
				}
			));
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
				// don't create quickClassBrowser for files that don't have any classes
				// (but do keep the quickClassBrowser when the last class is removed from a file)
				if (quickClassBrowser != null || parseInfo.CompilationUnit.Classes.Count > 0) {
					if (quickClassBrowser == null) {
						quickClassBrowser = new QuickClassBrowser();
						quickClassBrowser.JumpAction = (line, col) => ActiveTextEditor.JumpTo(line, col);
						SetRow(quickClassBrowser, 0);
						this.Children.Add(quickClassBrowser);
					}
					quickClassBrowser.Update(parseInfo.CompilationUnit);
					quickClassBrowser.SelectItemAtCaretPosition(this.ActiveTextEditorAdapter.Caret.Position);
				}
			} else {
				if (quickClassBrowser != null) {
					this.Children.Remove(quickClassBrowser);
					quickClassBrowser = null;
				}
			}
			iconBarManager.UpdateClassMemberBookmarks(parseInfo);
			UpdateFolding(primaryTextEditorAdapter, parseInfo);
			UpdateFolding(secondaryTextEditorAdapter, parseInfo);
		}
		
		void UpdateFolding(ITextEditor editor, ParseInformation parseInfo)
		{
			if (editor != null) {
				IServiceContainer container = editor.GetService(typeof(IServiceContainer)) as IServiceContainer;
				ParserFoldingStrategy folding = container.GetService(typeof(ParserFoldingStrategy)) as ParserFoldingStrategy;
				if (folding != null) {
					if (parseInfo == null) {
						folding.Dispose();
						container.RemoveService(typeof(ParserFoldingStrategy));
					} else {
						folding.UpdateFoldings(parseInfo);
					}
				} else {
					TextArea textArea = editor.GetService(typeof(TextArea)) as TextArea;
					if (parseInfo != null) {
						folding = new ParserFoldingStrategy(textArea);
						container.AddService(typeof(ParserFoldingStrategy), folding);
					}
				}
			}
		}
		
		public void Dispose()
		{
			primaryTextEditorAdapter.Language.Detach();
			if (secondaryTextEditorAdapter != null)
				secondaryTextEditorAdapter.Language.Detach();
			
			if (errorPainter != null)
				errorPainter.Dispose();
			this.Document = null;
		}
	}
}
