// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Threading;
using ICSharpCode.AvalonEdit.AddIn.Options;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Editing;
using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.AvalonEdit.Rendering;
using ICSharpCode.AvalonEdit.Search;
using ICSharpCode.AvalonEdit.Utils;
using ICSharpCode.Core;
using ICSharpCode.Core.Presentation;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Bookmarks;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Editor;
using ICSharpCode.SharpDevelop.Editor.AvalonEdit;
using ICSharpCode.SharpDevelop.Editor.CodeCompletion;

namespace ICSharpCode.AvalonEdit.AddIn
{
	public interface ICodeEditor
	{
		TextDocument Document { get; }
		
		void Redraw(ISegment segment, DispatcherPriority priority);
		
		event EventHandler DocumentChanged;
	}
	
	/// <summary>
	/// Integrates AvalonEdit with SharpDevelop.
	/// Also provides support for Split-View (showing two AvalonEdit instances using the same TextDocument)
	/// </summary>
	public class CodeEditor : Grid, IDisposable, ICodeEditor
	{
		const string contextMenuPath = "/SharpDevelop/ViewContent/AvalonEdit/ContextMenu";
		
		QuickClassBrowser quickClassBrowser;
		readonly CodeEditorView primaryTextEditor;
		readonly CodeEditorAdapter primaryTextEditorAdapter;
		CodeEditorView secondaryTextEditor;
		CodeEditorView activeTextEditor;
		CodeEditorAdapter secondaryTextEditorAdapter;
		GridSplitter gridSplitter;
		readonly IconBarManager iconBarManager;
		readonly TextMarkerService textMarkerService;
		readonly IChangeWatcher changeWatcher;
		ErrorPainter errorPainter;
		
		public CodeEditorView PrimaryTextEditor {
			get { return primaryTextEditor; }
		}
		
		public CodeEditorView ActiveTextEditor {
			get { return activeTextEditor; }
			private set {
				if (activeTextEditor != value) {
					activeTextEditor = value;
					HandleCaretPositionChange();
				}
			}
		}
		
		TextDocument document;
		
		public TextDocument Document {
			get {
				return document;
			}
			private set {
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
		
		public ITextEditor PrimaryTextEditorAdapter {
			get { return primaryTextEditorAdapter; }
		}
		
		public ITextEditor ActiveTextEditorAdapter {
			get { return this.ActiveTextEditor.Adapter; }
		}
		
		public IconBarManager IconBarManager {
			get { return iconBarManager; }
		}
		
		FileName fileName;
		
		public FileName FileName {
			get { return fileName; }
			set {
				if (fileName != value) {
					fileName = value;
					
					primaryTextEditorAdapter.FileNameChanged();
					if (secondaryTextEditorAdapter != null)
						secondaryTextEditorAdapter.FileNameChanged();
					
					if (this.errorPainter == null) {
						this.errorPainter = new ErrorPainter(primaryTextEditorAdapter);
					} else {
						this.errorPainter.UpdateErrors();
					}
					if (changeWatcher != null) {
						changeWatcher.Initialize(this.DocumentAdapter);
					}
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
		
		const double minRowHeight = 40;
		
		public CodeEditor()
		{
			CodeEditorOptions.Instance.PropertyChanged += CodeEditorOptions_Instance_PropertyChanged;
			CustomizedHighlightingColor.ActiveColorsChanged += CustomizedHighlightingColor_ActiveColorsChanged;
			ParserService.ParseInformationUpdated += ParserServiceParseInformationUpdated;
			
			this.FlowDirection = FlowDirection.LeftToRight; // code editing is always left-to-right
			this.CommandBindings.Add(new CommandBinding(SharpDevelopRoutedCommands.SplitView, OnSplitView));
			
			textMarkerService = new TextMarkerService(this);
			iconBarManager = new IconBarManager();
			if (CodeEditorOptions.Instance.EnableChangeMarkerMargin) {
				changeWatcher = new DefaultChangeWatcher();
			}
			primaryTextEditor = CreateTextEditor();
			primaryTextEditorAdapter = (CodeEditorAdapter)primaryTextEditor.TextArea.GetService(typeof(ITextEditor));
			Debug.Assert(primaryTextEditorAdapter != null);
			activeTextEditor = primaryTextEditor;
			
			this.Document = primaryTextEditor.Document;
			primaryTextEditor.SetBinding(TextEditor.DocumentProperty, new Binding("Document") { Source = this });
			
			this.ColumnDefinitions.Add(new ColumnDefinition());
			this.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
			this.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star), MinHeight = minRowHeight });
			SetRow(primaryTextEditor, 1);
			
			this.Children.Add(primaryTextEditor);
		}
		
		void CodeEditorOptions_Instance_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
			if (e.PropertyName == "EnableQuickClassBrowser")
				FetchParseInformation();
		}
		
		void CustomizedHighlightingColor_ActiveColorsChanged(object sender, EventArgs e)
		{
			// CustomizableHighlightingColorizer loads the new values automatically, we just need
			// to force a refresh in AvalonEdit.
			primaryTextEditor.UpdateCustomizedHighlighting();
			if (secondaryTextEditor != null)
				secondaryTextEditor.UpdateCustomizedHighlighting();
		}
		
		/// <summary>
		/// This method is called to create a new text editor view (=once for the primary editor; and whenever splitting the editor)
		/// </summary>
		protected virtual CodeEditorView CreateTextEditor()
		{
			CodeEditorView codeEditorView = new CodeEditorView();
			CodeEditorAdapter adapter = new CodeEditorAdapter(this, codeEditorView);
			codeEditorView.Adapter = adapter;
			TextView textView = codeEditorView.TextArea.TextView;
			textView.Services.AddService(typeof(ITextEditor), adapter);
			textView.Services.AddService(typeof(CodeEditor), this);
			
			codeEditorView.TextArea.TextEntering += TextAreaTextEntering;
			codeEditorView.TextArea.TextEntered += TextAreaTextEntered;
			codeEditorView.TextArea.Caret.PositionChanged += TextAreaCaretPositionChanged;
			codeEditorView.TextArea.DefaultInputHandler.CommandBindings.Add(
				new CommandBinding(CustomCommands.CtrlSpaceCompletion, OnCodeCompletion));
			codeEditorView.TextArea.DefaultInputHandler.NestedInputHandlers.Add(new SearchInputHandler(codeEditorView.TextArea));
			
			textView.BackgroundRenderers.Add(textMarkerService);
			textView.LineTransformers.Add(textMarkerService);
			textView.Services.AddService(typeof(ITextMarkerService), textMarkerService);
			
			textView.Services.AddService(typeof(IEditorUIService), new AvalonEditEditorUIService(textView));
			
			textView.Services.AddService(typeof(IBookmarkMargin), iconBarManager);
			codeEditorView.TextArea.LeftMargins.Insert(0, new IconBarMargin(iconBarManager));
			
			if (changeWatcher != null) {
				codeEditorView.TextArea.LeftMargins.Add(new ChangeMarkerMargin(changeWatcher));
			}
			
			textView.Services.AddService(typeof(ISyntaxHighlighter), new AvalonEditSyntaxHighlighterAdapter(textView));
			textView.Services.AddService(typeof(IBreakpointCreateService), new BreakpointCreateService());
			
			codeEditorView.TextArea.MouseRightButtonDown += TextAreaMouseRightButtonDown;
			codeEditorView.TextArea.ContextMenuOpening += TextAreaContextMenuOpening;
			codeEditorView.TextArea.TextCopied += textEditor_TextArea_TextCopied;
			codeEditorView.GotFocus += textEditor_GotFocus;
			
			return codeEditorView;
		}
		
		public event EventHandler<TextEventArgs> TextCopied;

		void textEditor_TextArea_TextCopied(object sender, TextEventArgs e)
		{
			if (TextCopied != null)
				TextCopied(this, e);
		}

		protected virtual void DisposeTextEditor(CodeEditorView textEditor)
		{
			foreach (var d in textEditor.TextArea.LeftMargins.OfType<IDisposable>())
				d.Dispose();
			textEditor.Dispose();
		}
		
		void textEditor_GotFocus(object sender, RoutedEventArgs e)
		{
			Debug.Assert(sender is CodeEditorView);
			this.ActiveTextEditor = (CodeEditorView)sender;
		}
		
		void TextAreaContextMenuOpening(object sender, ContextMenuEventArgs e)
		{
			ITextEditor adapter = GetAdapterFromSender(sender);
			MenuService.ShowContextMenu(sender as UIElement, adapter, contextMenuPath);
		}
		
		void TextAreaMouseRightButtonDown(object sender, MouseButtonEventArgs e)
		{
			TextEditor textEditor = GetTextEditorFromSender(sender);
			var position = textEditor.GetPositionFromPoint(e.GetPosition(textEditor));
			if (position.HasValue) {
				textEditor.TextArea.Caret.Position = position.Value;
			}
		}
		
		/// <summary>
		/// Use fixed encoding for loading.
		/// </summary>
		public bool UseFixedEncoding { get; set; }
		
		public Encoding Encoding {
			get { return primaryTextEditor.Encoding; }
			set { primaryTextEditor.Encoding = value; }
		}
		
		/// <summary>
		/// Gets if the document can be saved with the current encoding without losing data.
		/// </summary>
		public bool CanSaveWithCurrentEncoding()
		{
			Encoding encoding = this.Encoding;
			if (encoding == null || FileReader.IsUnicode(encoding))
				return true;
			// not a unicode codepage
			string text = document.Text;
			return encoding.GetString(encoding.GetBytes(text)) == text;
		}
		
		// always use primary text editor for loading/saving
		// (the file encoding is stored only there)
		public void Load(Stream stream)
		{
			if (UseFixedEncoding) {
				using (StreamReader reader = new StreamReader(stream, primaryTextEditor.Encoding, detectEncodingFromByteOrderMarks: false)) {
					primaryTextEditor.Text = reader.ReadToEnd();
				}
			} else {
				// do encoding auto-detection
				using (StreamReader reader = FileReader.OpenStream(stream, this.Encoding ?? FileService.DefaultFileEncoding.GetEncoding())) {
					primaryTextEditor.Text = reader.ReadToEnd();
					this.Encoding = reader.CurrentEncoding;
				}
			}
			// raise event which allows removing existing NewLineConsistencyCheck overlays
			if (LoadedFileContent != null)
				LoadedFileContent(this, EventArgs.Empty);
			NewLineConsistencyCheck.StartConsistencyCheck(this);
		}
		
		public event EventHandler LoadedFileContent;
		
		public void Save(Stream stream)
		{
			// don't use TextEditor.Save here because that would touch the Modified flag,
			// but OpenedFile is already managing IsDirty
			using (StreamWriter writer = new StreamWriter(stream, primaryTextEditor.Encoding ?? Encoding.UTF8)) {
				writer.Write(primaryTextEditor.Text);
			}
		}
		
		void OnSplitView(object sender, ExecutedRoutedEventArgs e)
		{
			if (secondaryTextEditor == null) {
				// create secondary editor
				this.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star), MinHeight = minRowHeight });
				secondaryTextEditor = CreateTextEditor();
				secondaryTextEditorAdapter = (CodeEditorAdapter)secondaryTextEditor.TextArea.GetService(typeof(ITextEditor));
				Debug.Assert(primaryTextEditorAdapter != null);
				
				secondaryTextEditor.SetBinding(TextEditor.DocumentProperty,
				                               new Binding(TextEditor.DocumentProperty.Name) { Source = primaryTextEditor });
				secondaryTextEditor.SetBinding(TextEditor.IsReadOnlyProperty,
				                               new Binding(TextEditor.IsReadOnlyProperty.Name) { Source = primaryTextEditor });
				secondaryTextEditor.SyntaxHighlighting = primaryTextEditor.SyntaxHighlighting;
				secondaryTextEditor.UpdateCustomizedHighlighting();
				
				gridSplitter = new GridSplitter {
					Height = 4,
					HorizontalAlignment = HorizontalAlignment.Stretch,
					VerticalAlignment = VerticalAlignment.Top
				};
				SetRow(gridSplitter, 2);
				this.Children.Add(gridSplitter);
				
				secondaryTextEditor.Margin = new Thickness(0, 4, 0, 0);
				SetRow(secondaryTextEditor, 2);
				this.Children.Add(secondaryTextEditor);
				
				secondaryTextEditorAdapter.FileNameChanged();
				FetchParseInformation();
			} else {
				// remove secondary editor
				this.Children.Remove(secondaryTextEditor);
				this.Children.Remove(gridSplitter);
				secondaryTextEditorAdapter.Language.Detach();
				DisposeTextEditor(secondaryTextEditor);
				secondaryTextEditor = null;
				secondaryTextEditorAdapter = null;
				gridSplitter = null;
				this.RowDefinitions.RemoveAt(this.RowDefinitions.Count - 1);
				this.ActiveTextEditor = primaryTextEditor;
			}
		}
		
		public event EventHandler CaretPositionChanged;
		
		void TextAreaCaretPositionChanged(object sender, EventArgs e)
		{
			Debug.Assert(sender is Caret);
			Debug.Assert(!document.IsInUpdate);
			if (sender == this.ActiveTextEditor.TextArea.Caret) {
				HandleCaretPositionChange();
			}
		}
		
		void HandleCaretPositionChange()
		{
			if (quickClassBrowser != null) {
				quickClassBrowser.SelectItemAtCaretPosition(this.ActiveTextEditorAdapter.Caret.Position);
			}
			
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
		
		SharpDevelopCompletionWindow CompletionWindow {
			get {
				return primaryTextEditor.ActiveCompletionWindow
					?? (secondaryTextEditor == null ? null : secondaryTextEditor.ActiveCompletionWindow);
			}
		}
		
		SharpDevelopInsightWindow InsightWindow {
			get {
				return primaryTextEditor.ActiveInsightWindow
					?? (secondaryTextEditor == null ? null : secondaryTextEditor.ActiveInsightWindow);
			}
		}
		
		void TextAreaTextEntering(object sender, TextCompositionEventArgs e)
		{
			// don't start new code completion if there is still a completion window open
			if (CompletionWindow != null)
				return;
			
			if (e.Handled)
				return;
			
			// disable all code completion bindings when CC is disabled
			if (!CodeCompletionOptions.EnableCodeCompletion)
				return;
			
			TextArea textArea = GetTextEditorFromSender(sender).TextArea;
			if (textArea.ActiveInputHandler != textArea.DefaultInputHandler)
				return; // deactivate CC for non-default input handlers
			
			ITextEditor adapter = GetAdapterFromSender(sender);
			
			foreach (char c in e.Text) {
				foreach (ICodeCompletionBinding cc in CodeCompletionBindings) {
					CodeCompletionKeyPressResult result = cc.HandleKeyPress(adapter, c);
					if (result == CodeCompletionKeyPressResult.Completed) {
						if (CompletionWindow != null) {
							// a new CompletionWindow was shown, but does not eat the input
							// tell it to expect the text insertion
							CompletionWindow.ExpectInsertionBeforeStart = true;
						}
						if (InsightWindow != null) {
							InsightWindow.ExpectInsertionBeforeStart = true;
						}
						return;
					} else if (result == CodeCompletionKeyPressResult.CompletedIncludeKeyInCompletion) {
						if (CompletionWindow != null) {
							if (CompletionWindow.StartOffset == CompletionWindow.EndOffset) {
								CompletionWindow.CloseWhenCaretAtBeginning = true;
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
			if (CompletionWindow != null)
				CompletionWindow.Close();
			
			// disable all code completion bindings when CC is disabled
			if (!CodeCompletionOptions.EnableCodeCompletion)
				return;
			
			CodeEditorView textEditor = GetTextEditorFromSender(sender);
			foreach (ICodeCompletionBinding cc in CodeCompletionBindings) {
				if (cc.CtrlSpace(textEditor.Adapter)) {
					e.Handled = true;
					break;
				}
			}
		}
		
		public IHighlightingDefinition SyntaxHighlighting {
			get { return primaryTextEditor.SyntaxHighlighting; }
			set {
				primaryTextEditor.SyntaxHighlighting = value;
				primaryTextEditor.UpdateCustomizedHighlighting();
				if (secondaryTextEditor != null) {
					secondaryTextEditor.SyntaxHighlighting = value;
					secondaryTextEditor.UpdateCustomizedHighlighting();
				}
			}
		}
		
		void FetchParseInformation()
		{
			ParseInformation parseInfo = ParserService.GetExistingParseInformation(this.FileName);
			if (parseInfo == null) {
				// if parse info is not yet available, start parsing on background
				ParserService.BeginParse(this.FileName, primaryTextEditorAdapter.Document);
				// we'll receive the result using the ParseInformationUpdated event
			}
			ParseInformationUpdated(parseInfo);
		}
		
		ParseInformation updateParseInfoTo;
		
		void ParserServiceParseInformationUpdated(object sender, ParseInformationEventArgs e)
		{
			if (e.FileName != this.FileName || !e.IsPrimaryParseInfoForFile)
				return;
			this.VerifyAccess();
			// When parse information is updated quickly in succession, only do a single update
			// to the latest version.
			updateParseInfoTo = e.NewParseInformation;
			this.Dispatcher.BeginInvoke(
				DispatcherPriority.Background,
				new Action(
					delegate {
						if (updateParseInfoTo != null) {
							ParseInformationUpdated(updateParseInfoTo);
							updateParseInfoTo = null;
						}
					}));
		}
		
		public void ParseInformationUpdated(ParseInformation parseInfo)
		{
			if (parseInfo != null && CodeEditorOptions.Instance.EnableQuickClassBrowser) {
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
			iconBarManager.UpdateClassMemberBookmarks(parseInfo, document);
			primaryTextEditor.UpdateParseInformationForFolding(parseInfo);
			if (secondaryTextEditor != null)
				secondaryTextEditor.UpdateParseInformationForFolding(parseInfo);
		}
		
		public void Dispose()
		{
			CodeEditorOptions.Instance.PropertyChanged -= CodeEditorOptions_Instance_PropertyChanged;
			CustomizedHighlightingColor.ActiveColorsChanged -= CustomizedHighlightingColor_ActiveColorsChanged;
			ParserService.ParseInformationUpdated -= ParserServiceParseInformationUpdated;
			
			if (primaryTextEditorAdapter.Language != null)
				primaryTextEditorAdapter.Language.Detach();
			if (secondaryTextEditorAdapter != null && secondaryTextEditorAdapter.Language != null)
				secondaryTextEditorAdapter.Language.Detach();
			
			if (errorPainter != null)
				errorPainter.Dispose();
			this.Document = null;
			DisposeTextEditor(primaryTextEditor);
			if (secondaryTextEditor != null)
				DisposeTextEditor(secondaryTextEditor);
		}
	}
}
