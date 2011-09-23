// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Editing;
using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.Core;
using ICSharpCode.Core.Presentation;
using ICSharpCode.SharpDevelop.Editor;
using ICSharpCode.SharpDevelop.Editor.AvalonEdit;

namespace ICSharpCode.SharpDevelop.Gui
{
	public abstract class AbstractConsolePad : AbstractPadContent, IEditable, IPositionable, ITextEditorProvider
	{
		const string toolBarTreePath = "/SharpDevelop/Pads/CommonConsole/ToolBar";
		
		DockPanel panel;
		protected ConsoleControl console;
		ToolBar toolbar;
		
		bool cleared;
		IList<string> history;
		int historyPointer;
		
		protected AbstractConsolePad()
		{
			this.panel = new DockPanel();
			
			this.console = new ConsoleControl();
			
			// creating the toolbar accesses the WordWrap property, so we must do this after creating the console
			this.toolbar = BuildToolBar();
			this.toolbar.SetValue(DockPanel.DockProperty, Dock.Top);
			
			this.panel.Children.Add(toolbar);
			this.panel.Children.Add(console);
			
			this.history = new List<string>();
			
			this.console.editor.TextArea.PreviewKeyDown += (sender, e) => {
				e.Handled = HandleInput(e.Key);
			};
			
			this.console.editor.TextArea.TextEntered += new TextCompositionEventHandler(AbstractConsolePadTextEntered);
			
			this.InitializeConsole();
		}

		protected virtual void AbstractConsolePadTextEntered(object sender, TextCompositionEventArgs e)
		{
		}
		
		protected virtual ToolBar BuildToolBar()
		{
			return ToolBarService.CreateToolBar(panel, this, toolBarTreePath);
		}
		
		public virtual ITextEditor TextEditor {
			get {
				return console.TextEditor;
			}
		}
		
		public override object Control {
			get { return panel; }
		}
		
		public override object InitiallyFocusedControl {
			get { return console.editor; }
		}
		
		string GetText()
		{
			return this.TextEditor.Document.Text;
		}
		
		/// <summary>
		/// Creates a snapshot of the editor content.
		/// This method is thread-safe.
		/// </summary>
		public ITextBuffer CreateSnapshot()
		{
			return new StringTextBuffer(GetText());
		}
		
		string IEditable.Text {
			get {
				return GetText();
			}
		}
		
		public virtual ICSharpCode.SharpDevelop.Editor.IDocument GetDocumentForFile(OpenedFile file)
		{
			return null;
		}
		
		#region IPositionable implementation
		void IPositionable.JumpTo(int line, int column)
		{
			this.TextEditor.JumpTo(line, column);
		}
		
		int IPositionable.Line {
			get {
				return this.TextEditor.Caret.Line;
			}
		}
		
		int IPositionable.Column {
			get {
				return this.TextEditor.Caret.Column;
			}
		}
		#endregion
		
		protected virtual bool HandleInput(Key key) {
			switch (key) {
				case Key.Back:
				case Key.Delete:
					if (console.editor.SelectionStart == 0 &&
					    console.editor.SelectionLength == console.editor.Document.TextLength) {
						ClearConsole();
						return true;
					}
					return false;
				case Key.Down:
					if (console.CommandText.Contains("\n"))
						return false;
					this.historyPointer = Math.Min(this.historyPointer + 1, this.history.Count);
					if (this.historyPointer == this.history.Count)
						console.CommandText = "";
					else
						console.CommandText = this.history[this.historyPointer];
					console.editor.ScrollToEnd();
					return true;
				case Key.Up:
					if (console.CommandText.Contains("\n"))
						return false;
					this.historyPointer = Math.Max(this.historyPointer - 1, 0);
					if (this.historyPointer == this.history.Count)
						console.CommandText = "";
					else
						console.CommandText = this.history[this.historyPointer];
						console.editor.ScrollToEnd();
						return true;
					case Key.Return:
						if (Keyboard.Modifiers == ModifierKeys.Shift)
							return false;
						int caretOffset = this.console.TextEditor.Caret.Offset;
						string commandText = console.CommandText;
						cleared = false;
						if (AcceptCommand(commandText)) {
							IDocument document = console.TextEditor.Document;
							if (!cleared) {
								if (document.GetCharAt(document.TextLength - 1) != '\n')
									document.Insert(document.TextLength, Environment.NewLine);
								AppendPrompt();
								console.TextEditor.Select(document.TextLength, 0);
							} else {
								console.CommandText = "";
							}
							cleared = false;
							this.history.Add(commandText);
							this.historyPointer = this.history.Count;
							console.editor.ScrollToEnd();
							return true;
						}
						return false;
				default:
					return false;
			}
		}
		
		/// <summary>
		/// Deletes the content of the console and prints a new prompt.
		/// </summary>
		public void ClearConsole()
		{
			this.console.editor.Document.Text = "";
			cleared = true;
			AppendPrompt();
		}
		
		/// <summary>
		/// Deletes the console history.
		/// </summary>
		public void DeleteHistory()
		{
			this.history.Clear();
			this.historyPointer = 0;
		}
		
		public void SetHighlighting(string language)
		{
			if (this.console != null)
				this.console.SetHighlighting(language);
		}
		
		public bool WordWrap {
			get { return this.console.editor.WordWrap; }
			set { this.console.editor.WordWrap = value; }
		}
		
		protected abstract string Prompt {
			get;
		}
		
		protected abstract bool AcceptCommand(string command);
		
		protected virtual void InitializeConsole()
		{
			AppendPrompt();
		}
		
		protected virtual void AppendPrompt()
		{
			console.Append(Prompt);
			console.SetReadonly();
			console.editor.Document.UndoStack.ClearAll();
		}
		
		protected void AppendLine(string text)
		{
			console.Append(text + Environment.NewLine);
		}
		
		protected void Append(string text)
		{
			console.Append(text);
		}
		
		protected void InsertBeforePrompt(string text)
		{
			int endOffset = this.console.readOnlyRegion.EndOffset;
			bool needScrollDown = this.console.editor.CaretOffset >= endOffset;
			this.console.editor.Document.Insert(endOffset - Prompt.Length, text);
			this.console.editor.ScrollToEnd();
			this.console.SetReadonly(endOffset + text.Length);
		}
		
		protected virtual void Clear()
		{
			this.ClearConsole();
		}
	}
	
	public class ConsoleControl : Grid
	{
		internal AvalonEdit.TextEditor editor;
		internal ITextEditor editorAdapter;
		internal BeginReadOnlySectionProvider readOnlyRegion;
		
		public event TextCompositionEventHandler TextAreaTextEntered;
		public event KeyEventHandler TextAreaPreviewKeyDown;
		
		static TextEditorOptions consoleOptions;
		
		public ConsoleControl()
		{
			this.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
			this.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1, GridUnitType.Star) });
			
			object tmp;
			
			this.editorAdapter = EditorControlService.CreateEditor(out tmp);
			
			this.editor = (AvalonEdit.TextEditor)tmp;
			this.editor.SetValue(Grid.ColumnProperty, 0);
			this.editor.SetValue(Grid.RowProperty, 0);
			this.editor.ShowLineNumbers = false;
			
			if (consoleOptions == null) {
				consoleOptions = new TextEditorOptions(editor.Options);
				consoleOptions.AllowScrollBelowDocument = false;
			}
			
			this.editor.Options = consoleOptions;
			
			this.Children.Add(editor);
			
			editor.TextArea.ReadOnlySectionProvider = readOnlyRegion = new BeginReadOnlySectionProvider();
			editor.TextArea.TextEntered += new TextCompositionEventHandler(editor_TextArea_TextEntered);
			editor.TextArea.PreviewKeyDown += new KeyEventHandler(editor_TextArea_PreviewKeyDown);
		}
		
		public ITextEditor TextEditor {
			get {
				return editorAdapter;
			}
		}
		
		public Encoding Encoding {
			get {
				return this.editor.Encoding;
			}
			set {
				this.editor.Encoding = value;
			}
		}
		
		public void SelectText(int line, int column, int length)
		{
			int offset = this.editor.Document.GetOffset(new TextLocation(line, column));
			this.editor.Select(offset, length);
		}
		
		public void SetHighlighting(string language)
		{
			editor.SyntaxHighlighting = HighlightingManager.Instance.GetDefinition(language);
		}
		
		public void Append(string text)
		{
			editor.AppendText(text);
		}
		
		/// <summary>
		/// Sets the readonly region to a specified offset.
		/// </summary>
		public void SetReadonly(int offset)
		{
			readOnlyRegion.EndOffset = offset;
		}
		
		/// <summary>
		/// Sets the readonly region to the end of the document.
		/// </summary>
		public void SetReadonly()
		{
			readOnlyRegion.EndOffset = editor.Document.TextLength;
		}
		
		/// <summary>
		/// Hides the scroll bar.
		/// </summary>
		public void HideScrollBar()
		{
			this.editor.HorizontalScrollBarVisibility = ScrollBarVisibility.Hidden;
			this.editor.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
		}
		
		public void JumpToLine(int line)
		{
			this.editor.ScrollToLine(line);
		}
		
		public int CommandOffset {
			get { return readOnlyRegion.EndOffset; }
		}
		
		/// <summary>
		/// Gets/sets the command text displayed at the command prompt.
		/// </summary>
		public string CommandText {
			get {
				return editor.Document.GetText(new TextSegment() { StartOffset = readOnlyRegion.EndOffset, EndOffset = editor.Document.TextLength });
			}
			set {
				editor.Document.Replace(new TextSegment() { StartOffset = readOnlyRegion.EndOffset, EndOffset = editor.Document.TextLength }, value);
			}
		}
		
		void editor_TextArea_TextEntered(object sender, TextCompositionEventArgs e)
		{
			TextCompositionEventHandler handler = TextAreaTextEntered;
			
			if (handler != null)
				handler(this, e);
		}
		
		void editor_TextArea_PreviewKeyDown(object sender, KeyEventArgs e)
		{
			KeyEventHandler handler = TextAreaPreviewKeyDown;
			
			if (handler != null)
				handler(this, e);
		}
		
		public void Clear()
		{
			editor.Clear();
		}
	}
	
	public class BeginReadOnlySectionProvider : IReadOnlySectionProvider
	{
		public int EndOffset { get; set; }
		
		public bool CanInsert(int offset)
		{
			return offset >= EndOffset;
		}
		
		public IEnumerable<ISegment> GetDeletableSegments(ISegment segment)
		{
			if (segment.EndOffset < this.EndOffset)
				return Enumerable.Empty<ISegment>();
			
			return new[] {
				new TextSegment() {
					StartOffset = Math.Max(this.EndOffset, segment.Offset),
					EndOffset = segment.EndOffset
				}
			};
		}
	}
	
	class ClearConsoleCommand : AbstractMenuCommand
	{
		public override void Run()
		{
			var pad = this.Owner as AbstractConsolePad;
			if (pad != null)
				pad.ClearConsole();
		}
	}
	
	class DeleteHistoryCommand : AbstractMenuCommand
	{
		public override void Run()
		{
			var pad = this.Owner as AbstractConsolePad;
			if (pad != null)
				pad.DeleteHistory();
		}
	}
	
	class ToggleConsoleWordWrapCommand : AbstractCheckableMenuCommand
	{
		AbstractConsolePad pad;
		
		public override object Owner {
			get { return base.Owner; }
			set {
				if (!(value is AbstractConsolePad))
					throw new Exception("Owner has to be a AbstractConsolePad");
				pad = value as AbstractConsolePad;
				base.Owner = value;
			}
		}
		
		public override bool IsChecked {
			get { return pad.WordWrap; }
			set { pad.WordWrap = value; }
		}
		
		public override void Run()
		{
			IsChecked = !IsChecked;
		}
	}
}
