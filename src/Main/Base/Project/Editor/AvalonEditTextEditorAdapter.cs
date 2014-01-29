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
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Editing;
using ICSharpCode.NRefactory;
using ICSharpCode.NRefactory.Editor;
using ICSharpCode.SharpDevelop.Editor.CodeCompletion;

namespace ICSharpCode.SharpDevelop.Editor
{
	using ICSharpCode.AvalonEdit;
	
	/// <summary>
	/// Wraps AvalonEdit to provide the ITextEditor interface.
	/// </summary>
	public class AvalonEditTextEditorAdapter : ITextEditor
	{
		readonly TextEditor textEditor;
		
		public TextEditor TextEditor {
			get { return textEditor; }
		}
		
		public AvalonEditTextEditorAdapter(TextEditor textEditor)
		{
			if (textEditor == null)
				throw new ArgumentNullException("textEditor");
			this.textEditor = textEditor;
			this.Options = new OptionsAdapter(textEditor.Options);
		}
		
		public static TextEditor CreateAvalonEditInstance()
		{
			object editor;
			SD.EditorControlService.CreateEditor(out editor);
			if (!(editor is TextEditor))
				throw new NotSupportedException("Expected text editor to be AvalonEdit");
			return (TextEditor)editor;
		}
		
		public IDocument Document {
			get { return textEditor.Document; }
		}
		
		public ITextEditorCaret Caret { 
			get { return new CaretAdapter(textEditor.TextArea.Caret); }
		}
		
		public virtual ITextEditorOptions Options { get; private set; }
		
		public virtual ILanguageBinding Language {
			get { return DefaultLanguageBinding.DefaultInstance; }
		}
		
		sealed class CaretAdapter : ITextEditorCaret
		{
			Caret caret;
			
			public CaretAdapter(Caret caret)
			{
				this.caret = caret;
			}
			
			event EventHandler ITextEditorCaret.LocationChanged {
				add { caret.PositionChanged += value; }
				remove {  caret.PositionChanged -= value; }
			}
			
			int ITextEditorCaret.Offset {
				get { return caret.Offset; }
				set { caret.Offset = value; }
			}
			
			int ITextEditorCaret.Line {
				get { return caret.Line; }
				set { caret.Line = value; }
			}
			
			int ITextEditorCaret.Column {
				get { return caret.Column; }
				set { caret.Column = value; }
			}
			
			TextLocation ITextEditorCaret.Location {
				get { return caret.Location; }
				set { caret.Location = value; }
			}
		}
		
		sealed class OptionsAdapter : ITextEditorOptions
		{
			TextEditorOptions avalonEditOptions;
			
			public OptionsAdapter(TextEditorOptions avalonEditOptions)
			{
				this.avalonEditOptions = avalonEditOptions;
			}
			
			public string IndentationString {
				get {
					return avalonEditOptions.IndentationString;
				}
			}
			
			public bool AutoInsertBlockEnd {
				get {
					return true;
				}
			}
			
			public bool ConvertTabsToSpaces {
				get {
					return avalonEditOptions.ConvertTabsToSpaces;
				}
			}
			
			public int IndentationSize {
				get {
					return avalonEditOptions.IndentationSize;
				}
			}
			
			public int VerticalRulerColumn {
				get {
					return 120;
				}
			}
			
			public bool UnderlineErrors {
				get {
					return true;
				}
			}
			
			public event PropertyChangedEventHandler PropertyChanged {
				add    { avalonEditOptions.PropertyChanged += value; }
				remove { avalonEditOptions.PropertyChanged -= value; }
			}
			
			public string FontFamily {
				get {
					return "Consolas";
				}
			}
			
			public double FontSize {
				get {
					return 10.0;
				}
			}
		}
		
		public virtual ICSharpCode.Core.FileName FileName {
			get { return null; }
		}
		
		public object GetService(Type serviceType)
		{
			return textEditor.TextArea.GetService(serviceType);
		}
		
		public int SelectionStart {
			get {
				return textEditor.SelectionStart;
			}
		}
		
		public int SelectionLength {
			get {
				return textEditor.SelectionLength;
			}
		}
		
		public string SelectedText {
			get {
				return textEditor.SelectedText;
			}
			set {
				textEditor.SelectedText = value;
			}
		}
		
		public event KeyEventHandler KeyPress {
			add    { textEditor.TextArea.PreviewKeyDown += value; }
			remove { textEditor.TextArea.PreviewKeyDown -= value; }
		}
		
		public event EventHandler SelectionChanged {
			add    { textEditor.TextArea.SelectionChanged += value; }
			remove { textEditor.TextArea.SelectionChanged -= value; }
		}
		
		public void Select(int selectionStart, int selectionLength)
		{
			textEditor.Select(selectionStart, selectionLength);
			textEditor.TextArea.Caret.BringCaretToView();
		}
		
		public void JumpTo(int line, int column)
		{
			textEditor.TextArea.ClearSelection();
			textEditor.TextArea.Caret.Position = new TextViewPosition(line, column);
			// might have jumped to a different location if column was outside the valid range
			TextLocation actualLocation = textEditor.TextArea.Caret.Location;
			if (textEditor.ActualHeight > 0) {
				textEditor.ScrollTo(actualLocation.Line, actualLocation.Column);
			} else {
				// we have to delay the scrolling if the text editor is not yet loaded
				textEditor.Dispatcher.BeginInvoke(DispatcherPriority.Loaded, new Action(
					delegate {
						textEditor.ScrollTo(actualLocation.Line, actualLocation.Column);
					}));
			}
		}
		
		public virtual IInsightWindow ActiveInsightWindow {
			get { return null; }
		}
		
		public virtual IInsightWindow ShowInsightWindow(IEnumerable<IInsightItem> items)
		{
			return null;
		}
		
		public virtual ICompletionListWindow ActiveCompletionWindow {
			get { return null; }
		}
		
		public virtual ICompletionListWindow ShowCompletionWindow(ICompletionItemList data)
		{
			return null;
		}
		
		public virtual IEnumerable<ISnippetCompletionItem> GetSnippets()
		{
			return Enumerable.Empty<ISnippetCompletionItem>();
		}
		
		public virtual IList<ICSharpCode.SharpDevelop.Refactoring.IContextActionProvider> ContextActionProviders {
			get { return EmptyList<ICSharpCode.SharpDevelop.Refactoring.IContextActionProvider>.Instance; }
		}
		
		public virtual ITextEditor PrimaryView {
			get { return this; }
		}
	}
}
