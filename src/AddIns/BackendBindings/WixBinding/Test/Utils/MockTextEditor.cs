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
using System.Windows.Input;

using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Editing;
using ICSharpCode.Core;
using ICSharpCode.NRefactory;
using ICSharpCode.NRefactory.Editor;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Editor;
using ICSharpCode.SharpDevelop.Editor.CodeCompletion;
using ICSharpCode.SharpDevelop.Refactoring;

namespace WixBinding.Tests.Utils
{
	public class MockTextEditor : ITextEditor
	{
		TextDocument textDocument = new TextDocument();
		MockCaret caret = new MockCaret();
		TextLocation locationJumpedTo = TextLocation.Empty;
		Selection selection;
		MockTextEditorOptions options = new MockTextEditorOptions();
		TextArea textArea;
		
		public MockTextEditor()
		{
			textArea = new TextArea();
			textArea.Document = textDocument;
			selection = Selection.Create(textArea, -1, -1);
		}
		
		public event EventHandler SelectionChanged;
		
		protected virtual void OnSelectionChanged(EventArgs e)
		{
			if (SelectionChanged != null) {
				SelectionChanged(this, e);
			}
		}
		
		public event KeyEventHandler KeyPress;
		
		protected virtual void OnKeyPress()
		{
			if (KeyPress != null) {
				// do nothing.
			}
		}
		
		public ITextEditor PrimaryView {
			get {
				throw new NotImplementedException();
			}
		}
		
		public IDocument Document {
			get { return textDocument; }
		}
		
		public ITextEditorCaret Caret {
			get { return caret; }
		}
		
		public ITextEditorOptions Options {
			get { return options; }
		}
		
		public bool OptionsConvertTabsToSpaces {
			get { return options.ConvertTabsToSpaces; }
			set { options.ConvertTabsToSpaces = value; }
		}

		public string OptionsIndentationString {
			get { return options.IndentationString; }
			set { options.IndentationString = value; }
		}
		
		public int OptionsIndentationSize {
			get { return options.IndentationSize; }
			set { options.IndentationSize = value; }
		}
		
		public ILanguageBinding Language {
			get {
				throw new NotImplementedException();
			}
		}
		
		public int SelectionStart {
			get {
				throw new NotImplementedException();
			}
		}
		
		public int SelectionLength {
			get {
				throw new NotImplementedException();
			}
		}
		
		public string SelectedText {
			get { 
				if (selection.IsEmpty) {
					return String.Empty;
				}
				return textDocument.GetText(selection.SurroundingSegment.Offset, selection.Length);
			}
			set {
				throw new NotImplementedException();
			}
		}
		
		public FileName FileName {
			get {
				throw new NotImplementedException();
			}
		}
		
		public ICompletionListWindow ActiveCompletionWindow {
			get {
				throw new NotImplementedException();
			}
		}
		
		public IInsightWindow ActiveInsightWindow {
			get {
				throw new NotImplementedException();
			}
		}
		
		public void Select(int selectionStart, int selectionLength)
		{
			selection = Selection.Create(textArea, selectionStart, selectionLength + selectionStart);
		}
		
		public void JumpTo(int line, int column)
		{
			locationJumpedTo = new TextLocation(column, line);
			selection = Selection.Create(textArea, -1, -1);
		}
		
		public TextLocation LocationJumpedTo {
			get { return locationJumpedTo; }
		}
		
		public ICompletionListWindow ShowCompletionWindow(ICompletionItemList data)
		{
			throw new NotImplementedException();
		}
		
		public IInsightWindow ShowInsightWindow(IEnumerable<IInsightItem> items)
		{
			throw new NotImplementedException();
		}
		
		public IEnumerable<ISnippetCompletionItem> GetSnippets()
		{
			throw new NotImplementedException();
		}
		
		public object GetService(Type serviceType)
		{
			throw new NotImplementedException();
		}
		
		public void Undo()
		{
			textDocument.UndoStack.Undo();
		}
		
		public IList<IContextActionProvider> ContextActionProviders {
			get {
				throw new NotImplementedException();
			}
		}
	}
}
