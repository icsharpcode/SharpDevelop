// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Windows.Input;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Editing;
using ICSharpCode.Core;
using ICSharpCode.NRefactory;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Editor;
using ICSharpCode.SharpDevelop.Editor.AvalonEdit;
using ICSharpCode.SharpDevelop.Editor.CodeCompletion;

namespace WixBinding.Tests.Utils
{
	public class MockTextEditor : ITextEditor
	{
		TextDocument textDocument = new TextDocument();
		AvalonEditDocumentAdapter documentAdapter;
		MockCaret caret = new MockCaret();
		Location locationJumpedTo = Location.Empty;
		Selection selection;
		MockTextEditorOptions options = new MockTextEditorOptions();
		TextArea textArea;
				
		public MockTextEditor()
		{
			documentAdapter = new AvalonEditDocumentAdapter(textDocument, null);
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
			get { return documentAdapter; }
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
				return documentAdapter.GetText(selection.SurroundingSegment.Offset, selection.Length);
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
			locationJumpedTo = new Location(column, line);
			selection = Selection.Create(textArea, -1, -1);
		}
		
		public Location LocationJumpedTo {
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
		
		public IEnumerable<ICompletionItem> GetSnippets()
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
	}
}
