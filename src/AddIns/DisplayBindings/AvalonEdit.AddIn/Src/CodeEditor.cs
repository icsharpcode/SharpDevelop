// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

using System;
using System.Collections.ObjectModel;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
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
using ICSharpCode.NRefactory.Editor;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Editor.Bookmarks;
using ICSharpCode.SharpDevelop.Editor;
using ICSharpCode.SharpDevelop.Editor.CodeCompletion;
using ICSharpCode.SharpDevelop.Parser;
using ICSharpCode.SharpDevelop.Widgets.MyersDiff;

namespace ICSharpCode.AvalonEdit.AddIn
{
	/// <summary>
	/// Integrates AvalonEdit with SharpDevelop.
	/// Also provides support for Split-View (showing two AvalonEdit instances using the same TextDocument)
	/// </summary>
	[TextEditorService, ViewContentService]
	public class CodeEditor
		: Grid, IDisposable, ICSharpCode.SharpDevelop.Gui.IEditable, IFileDocumentProvider, ICSharpCode.SharpDevelop.Gui.IPositionable, IServiceProvider
	{
		const string contextMenuPath = "/SharpDevelop/ViewContent/AvalonEdit/ContextMenu";
		
		QuickClassBrowser quickClassBrowser;
		readonly CodeEditorView primaryTextEditor;
		readonly CodeEditorAdapter primaryTextEditorAdapter;
		readonly IconBarManager iconBarManager;
		readonly TextMarkerService textMarkerService;
		readonly IChangeWatcher changeWatcher;
		ErrorPainter errorPainter;
		
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
					document = value;
					
					if (DocumentChanged != null) {
						DocumentChanged(this, EventArgs.Empty);
					}
				}
			}
		}
		
		public event EventHandler DocumentChanged;
		
		[Obsolete("Use CodeEditor.Document instead; TextDocument now directly implements IDocument")]
		public IDocument DocumentAdapter {
			get { return this.Document; }
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
					this.document.FileName = fileName;
					
					primaryTextEditorAdapter.FileNameChanged();
					
					if (this.errorPainter == null) {
						this.errorPainter = new ErrorPainter(primaryTextEditorAdapter);
					} else {
						this.errorPainter.UpdateErrors();
					}
					if (changeWatcher != null) {
						changeWatcher.Initialize(this.Document, value);
					}
					UpdateSyntaxHighlighting(value);
					FetchParseInformation();
				}
			}
		}
		
		void UpdateSyntaxHighlighting(FileName fileName)
		{
			var oldHighlighter = primaryTextEditor.GetService<IHighlighter>();
			
			var highlighting = HighlightingManager.Instance.GetDefinitionByExtension(Path.GetExtension(fileName));
			var highlighter = SD.EditorControlService.CreateHighlighter(document);

			primaryTextEditor.SyntaxHighlighting = highlighting;
			primaryTextEditor.TextArea.TextView.LineTransformers.RemoveAll(t => t is HighlightingColorizer);
			primaryTextEditor.TextArea.TextView.LineTransformers.Insert(0, new HighlightingColorizer(highlighter));
			primaryTextEditor.UpdateCustomizedHighlighting();

			// Dispose the old highlighter; necessary to avoid memory leaks as
			// semantic highlighters might attach to global parser events.
			if (oldHighlighter != null) {
				oldHighlighter.Dispose();
			}
		}
		
		public void Redraw(ISegment segment, DispatcherPriority priority)
		{
			primaryTextEditor.TextArea.TextView.Redraw(segment, priority);
		}
		
		const double minRowHeight = 40;
		
		public CodeEditor()
		{
			CodeEditorOptions.Instance.PropertyChanged += CodeEditorOptions_Instance_PropertyChanged;
			CustomizedHighlightingColor.ActiveColorsChanged += CustomizedHighlightingColor_ActiveColorsChanged;
			SD.ParserService.ParseInformationUpdated += ParserServiceParseInformationUpdated;
			
			this.FlowDirection = FlowDirection.LeftToRight; // code editing is always left-to-right
			this.document = new TextDocument();
			var documentServiceContainer = document.GetRequiredService<IServiceContainer>();
			
			textMarkerService = new TextMarkerService(document);
			documentServiceContainer.AddService(typeof(ITextMarkerService), textMarkerService);
			
			iconBarManager = new IconBarManager();
			documentServiceContainer.AddService(typeof(IBookmarkMargin), iconBarManager);
			
			if (CodeEditorOptions.Instance.EnableChangeMarkerMargin) {
				changeWatcher = new DefaultChangeWatcher();
			}
			primaryTextEditor = CreateTextEditor();
			primaryTextEditorAdapter = (CodeEditorAdapter)primaryTextEditor.TextArea.GetService(typeof(ITextEditor));
			Debug.Assert(primaryTextEditorAdapter != null);
			
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
			foreach (var bookmark in SD.BookmarkManager.GetBookmarks(fileName).OfType<SDMarkerBookmark>())
				bookmark.SetMarker();
		}
		
		/// <summary>
		/// This method is called to create a new text editor view (=once for the primary editor; and whenever splitting the editor)
		/// </summary>
		protected virtual CodeEditorView CreateTextEditor()
		{
			CodeEditorView codeEditorView = new CodeEditorView();
			CodeEditorAdapter adapter = new CodeEditorAdapter(this, codeEditorView);
			codeEditorView.Adapter = adapter;
			codeEditorView.Document = document;
			TextView textView = codeEditorView.TextArea.TextView;
			textView.Services.AddService(typeof(ITextEditor), adapter);
			textView.Services.AddService(typeof(CodeEditor), this);
			textView.Services.AddService(typeof(ICSharpCode.SharpDevelop.Gui.IEditable), this);
			textView.Services.AddService(typeof(ICSharpCode.SharpDevelop.Gui.IPositionable), this);
			textView.Services.AddService(typeof(IFileDocumentProvider), this);
			
			codeEditorView.TextArea.TextEntering += TextAreaTextEntering;
			codeEditorView.TextArea.TextEntered += TextAreaTextEntered;
			codeEditorView.TextArea.Caret.PositionChanged += TextAreaCaretPositionChanged;
			codeEditorView.TextArea.SelectionChanged += TextAreaSelectionChanged;
			codeEditorView.TextArea.DefaultInputHandler.CommandBindings.Add(
				new CommandBinding(CustomCommands.CtrlSpaceCompletion, OnCodeCompletion));
			codeEditorView.TextArea.DefaultInputHandler.CommandBindings.Add(
				new CommandBinding(CustomCommands.CtrlShiftSpaceInsight, OnCodeInsight));
			SearchPanel.Install(codeEditorView.TextArea);
			
			textView.BackgroundRenderers.Add(textMarkerService);
			textView.LineTransformers.Add(textMarkerService);
			
			textView.Services.AddService(typeof(IEditorUIService), new AvalonEditEditorUIService(textView));
			
			codeEditorView.TextArea.LeftMargins.Insert(0, new IconBarMargin(iconBarManager));
			
			if (changeWatcher != null) {
				codeEditorView.TextArea.LeftMargins.Add(new ChangeMarkerMargin(changeWatcher));
			}
			textView.Services.AddService(typeof(EnhancedScrollBar), new EnhancedScrollBar(codeEditorView, textMarkerService, changeWatcher));
			
			codeEditorView.TextArea.MouseRightButtonDown += TextAreaMouseRightButtonDown;
			codeEditorView.TextArea.ContextMenuOpening += TextAreaContextMenuOpening;
			codeEditorView.TextArea.TextCopied += textEditor_TextArea_TextCopied;
			
			return codeEditorView;
		}
		
		void textEditor_TextArea_TextCopied(object sender, TextEventArgs e)
		{
			ICSharpCode.SharpDevelop.Gui.TextEditorSideBar.Instance.PutInClipboardRing(e.Text);
		}

		protected virtual void DisposeTextEditor(CodeEditorView textEditor)
		{
			foreach (var d in textEditor.TextArea.LeftMargins.OfType<IDisposable>())
				d.Dispose();
			textEditor.TextArea.GetRequiredService<EnhancedScrollBar>().Dispose();
			var highlighter = textEditor.TextArea.GetService<IHighlighter>();
			if (highlighter != null)
				highlighter.Dispose();
			textEditor.Dispose();
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
					ReloadDocument(primaryTextEditor.Document, reader.ReadToEnd());
				}
			} else {
				// do encoding auto-detection
				using (StreamReader reader = FileReader.OpenStream(stream, this.Encoding ?? SD.FileService.DefaultFileEncoding)) {
					ReloadDocument(primaryTextEditor.Document, reader.ReadToEnd());
					this.Encoding = reader.CurrentEncoding;
				}
			}
			// raise event which allows removing existing NewLineConsistencyCheck overlays
			if (LoadedFileContent != null)
				LoadedFileContent(this, EventArgs.Empty);
			NewLineConsistencyCheck.StartConsistencyCheck(this);
		}
		
		bool documentFirstLoad = true;
		bool clearUndoStackOnSwitch = true;
		
		/// <summary>
		/// Gets/Sets whether to clear the undo stack when reloading the document.
		/// The default is true.
		/// http://community.sharpdevelop.net/forums/t/15816.aspx
		/// </summary>
		public bool ClearUndoStackOnSwitch {
			get { return clearUndoStackOnSwitch; }
			set { clearUndoStackOnSwitch = value; }
		}
		
		void ReloadDocument(TextDocument document, string newContent)
		{
			var diff = new MyersDiffAlgorithm(new StringSequence(document.Text), new StringSequence(newContent));
			document.Replace(0, document.TextLength, newContent, diff.GetEdits().ToOffsetChangeMap());
			
			if (this.ClearUndoStackOnSwitch || documentFirstLoad)
				document.UndoStack.ClearAll();
			
			if (documentFirstLoad)
				documentFirstLoad = false;
		}
		
		public event EventHandler LoadedFileContent;
		
		public void Save(Stream stream)
		{
			// don't use TextEditor.Save here because that would touch the Modified flag,
			// but OpenedFile is already managing IsDirty
			using (StreamWriter writer = new StreamWriter(stream, primaryTextEditor.Encoding ?? Encoding.UTF8)) {
				primaryTextEditor.Document.WriteTextTo(writer);
			}
		}
		
		public event EventHandler CaretPositionChanged;
		
		void TextAreaCaretPositionChanged(object sender, EventArgs e)
		{
			if (document == null)
				return; // can happen if the editor is closed with Ctrl+F4 while selecting text
			Debug.Assert(sender is Caret);
			Debug.Assert(!document.IsInUpdate);
			if (sender == this.ActiveTextEditor.TextArea.Caret) {
				HandleCaretPositionChange();
			}
		}
		
		void HandleCaretPositionChange()
		{
			if (CaretPositionChanged != null)
				CaretPositionChanged(this, EventArgs.Empty);
			
			if (quickClassBrowser != null) {
				quickClassBrowser.SelectItemAtCaretPosition(this.ActiveTextEditor.TextArea.Caret.Location);
			}
			
			NavigationService.Log(this.BuildNavPoint());
			var document = this.Document;
			int lineOffset = document.GetLineByNumber(this.Line).Offset;
			int chOffset = this.Column;
			int col = 1;
			for (int i = 1; i < chOffset; i++) {
				if (document.GetCharAt(lineOffset + i - 1) == '\t')
					col += CodeEditorOptions.Instance.IndentationSize;
				else
					col += 1;
			}
			SD.StatusBar.SetCaretPosition(col, this.Line, chOffset);
		}
		
		void TextAreaSelectionChanged(object sender, EventArgs e)
		{
			if (document == null)
				return;
			
			if (sender == this.ActiveTextEditor.TextArea) {
				HandleSelectionChanged();
			}
		}
		
		void HandleSelectionChanged()
		{
			TextArea textArea = this.ActiveTextEditor.TextArea;
			if (textArea == null)
				return;
			
			Selection selection = textArea.Selection;
			if (selection is RectangleSelection) {
				int rows = Math.Abs(selection.EndPosition.Line - selection.StartPosition.Line) + 1;
				int cols = Math.Abs(selection.EndPosition.VisualColumn - selection.StartPosition.VisualColumn);
				SD.StatusBar.SetSelectionMulti(rows, cols);
			} else {
				SD.StatusBar.SetSelectionSingle(selection.Length);
			}
		}
		
		public INavigationPoint BuildNavPoint()
		{
			int lineNumber = this.Line;
			string txt = this.Document.GetText(this.Document.GetLineByNumber(lineNumber));
			return new TextNavigationPoint(this.FileName, lineNumber, this.Column, txt);
		}
		
		volatile static ReadOnlyCollection<ICodeCompletionBinding> codeCompletionBindings;
		
		public static ReadOnlyCollection<ICodeCompletionBinding> CodeCompletionBindings {
			get {
				if (codeCompletionBindings == null) {
					codeCompletionBindings = AddInTree.BuildItems<ICodeCompletionBinding>("/SharpDevelop/ViewContent/TextEditor/CodeCompletion", null, false).AsReadOnly();
				}
				return codeCompletionBindings;
			}
		}
		
		SharpDevelopCompletionWindow CompletionWindow {
			get {
				return primaryTextEditor.ActiveCompletionWindow;
			}
		}
		
		SharpDevelopInsightWindow InsightWindow {
			get {
				return primaryTextEditor.ActiveInsightWindow;
			}
		}
		
		#region IEditable
		public ITextSource CreateSnapshot()
		{
			return this.Document.CreateSnapshot();
		}
		
		/// <summary>
		/// Gets the document text.
		/// </summary>
		public string Text {
			get {
				return this.Document.Text;
			}
		}
		#endregion
		
		#region IFileDocumentProvider
		public IDocument GetDocumentForFile(ICSharpCode.SharpDevelop.Workbench.OpenedFile file)
		{
			if (file.FileName == this.FileName)
				return this.Document;
			else
				return null;
		}
		#endregion
		
		#region IPositionable
		public int Line {
			get { return this.PrimaryTextEditor.Adapter.Caret.Line; }
		}
		
		public int Column {
			get { return this.PrimaryTextEditor.Adapter.Caret.Column; }
		}
		
		public void JumpTo(int line, int column)
		{
			this.PrimaryTextEditor.JumpTo(line, column);
		}
		#endregion
		
		#region IServiceProvider implementation
		public object GetService(Type serviceType)
		{
			return this.primaryTextEditor.Adapter.GetService(serviceType);
		}
		#endregion
		
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
				var adapter = GetAdapterFromSender(sender);
				ILanguageBinding languageBinding = adapter.Language;
				if (languageBinding != null && languageBinding.FormattingStrategy != null) {
					char c = e.Text[0];
					// When entering a newline, AvalonEdit might use either "\r\n" or "\n", depending on
					// what was passed to TextArea.PerformTextInput. We'll normalize this to '\n'
					// so that formatting strategies don't have to handle both cases.
					if (c == '\r')
						c = '\n';
					languageBinding.FormattingStrategy.FormatLine(adapter, c);
					
					if (c == '\n') {
						// Immediately parse on enter.
						// This ensures we have up-to-date CC info about the method boundary when a user
						// types near the end of a method.
						SD.ParserService.ParseAsync(this.FileName, this.Document.CreateSnapshot()).FireAndForget();
					} else {
						if (e.Text.Length == 1) {
							// disable all code completion bindings when CC is disabled
							if (!CodeCompletionOptions.EnableCodeCompletion)
								return;
							
							foreach (ICodeCompletionBinding cc in CodeCompletionBindings) {
								if (cc.HandleKeyPressed(adapter, c))
									break;
							}
						}
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

		void OnCodeInsight(object sender, ExecutedRoutedEventArgs e)
		{
			if (InsightWindow != null)
				InsightWindow.Close();

			// disable all code insight bindings when Insight is disabled
			if (!CodeCompletionOptions.InsightEnabled)
				return;

			CodeEditorView textEditor = GetTextEditorFromSender(sender);
			foreach (IInsightCodeCompletionBinding cc in CodeCompletionBindings.OfType<IInsightCodeCompletionBinding>()) {
				if (cc.CtrlShiftSpace(textEditor.Adapter)) {
					e.Handled = true;
					break;
				}
			}
		}
		
		void FetchParseInformation()
		{
			ParseInformation parseInfo = SD.ParserService.GetCachedParseInformation(this.FileName);
			if (parseInfo == null) {
				// if parse info is not yet available, start parsing on background
				SD.ParserService.ParseAsync(this.FileName, primaryTextEditorAdapter.Document).FireAndForget();
				// we'll receive the result using the ParseInformationUpdated event
			}
			ParseInformationUpdated(parseInfo);
		}
		
		ParseInformation updateParseInfoTo;
		
		void ParserServiceParseInformationUpdated(object sender, ParseInformationEventArgs e)
		{
			if (e.FileName != this.FileName)
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
				if (quickClassBrowser != null || parseInfo.UnresolvedFile.TopLevelTypeDefinitions.Count > 0) {
					if (quickClassBrowser == null) {
						quickClassBrowser = new QuickClassBrowser();
						quickClassBrowser.JumpAction = (line, col) => ActiveTextEditor.JumpTo(line, col);
						SetRow(quickClassBrowser, 0);
						this.Children.Add(quickClassBrowser);
					}
					quickClassBrowser.Update(parseInfo.UnresolvedFile);
					quickClassBrowser.SelectItemAtCaretPosition(this.ActiveTextEditor.TextArea.Caret.Location);
				}
			} else {
				if (quickClassBrowser != null) {
					this.Children.Remove(quickClassBrowser);
					quickClassBrowser = null;
				}
			}
			iconBarManager.UpdateClassMemberBookmarks(parseInfo != null ? parseInfo.UnresolvedFile : null, document);
			primaryTextEditor.UpdateParseInformationForFolding(parseInfo);
		}
		
		public void Dispose()
		{
			CodeEditorOptions.Instance.PropertyChanged -= CodeEditorOptions_Instance_PropertyChanged;
			CustomizedHighlightingColor.ActiveColorsChanged -= CustomizedHighlightingColor_ActiveColorsChanged;
			SD.ParserService.ParseInformationUpdated -= ParserServiceParseInformationUpdated;
			
			if (primaryTextEditorAdapter.Language != null)
				primaryTextEditorAdapter.DetachExtensions();
			
			if (errorPainter != null)
				errorPainter.Dispose();
			if (changeWatcher != null)
				changeWatcher.Dispose();
			this.Document = null;
			DisposeTextEditor(primaryTextEditor);
		}
	}
}
