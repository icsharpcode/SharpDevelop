// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Windows.Input;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Editor;
using ICSharpCode.SharpDevelop.Editor.CodeCompletion;

namespace XmlEditor.Tests.Utils
{
	public class MockTextEditor : ITextEditor
	{
		MockCompletionListWindow completionWindowDisplayed;
		ICompletionItemList completionItemsDisplayed;
		MockCaret caret = new MockCaret();
		IDocument document;
		ITextEditorOptions options = new MockTextEditorOptions();
		FileName fileName;
		bool showCompletionWindowReturnsNull;
		bool showCompletionWindowMethodCalled;
		int selectionStart;
		int selectionLength;
						
		public MockTextEditor()
		{
			document = new MockDocument(this);
		}
		
		public event EventHandler SelectionChanged;
		
		protected virtual void OnSelectionChanged(EventArgs e)
		{
			if (SelectionChanged != null) {
				SelectionChanged(this, e);
			}
		}
		
		public event KeyEventHandler KeyPress;
		
		protected virtual void OnKeyPress(KeyEventArgs e)
		{
			if (KeyPress != null) {
				KeyPress(this, e);
			}
		}
		
		public ITextEditor PrimaryView {
			get {
				throw new NotImplementedException();
			}
		}
		
		public IDocument Document {
			get { return document; }
		}
		
		public void SetDocument(IDocument document)
		{
			this.document = document;
		}
		
		public MockDocument MockDocument {
			get { return document as MockDocument; }
		}
		
		public ITextEditorCaret Caret {
			get { return caret; }
		}
		
		public ITextEditorOptions Options {
			get { return options; }
			set { options = value; }
		}
		
		public ILanguageBinding Language {
			get {
				throw new NotImplementedException();
			}
		}
		
		public int SelectionStart {
			get { return selectionStart; }
		}
		
		public int SelectionLength {
			get { return selectionLength; }
		}
		
		public string SelectedText {
			get { return document.GetText(selectionStart, selectionLength); }
			set {
				throw new NotImplementedException();
			}
		}
		
		public FileName FileName {
			get { return fileName; }
			set { fileName = value; }
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
			this.selectionStart = selectionStart;
			this.selectionLength = selectionLength;
		}
		
		public void JumpTo(int line, int column)
		{
			throw new NotImplementedException();
		}
		
		public ICompletionListWindow ShowCompletionWindow(ICompletionItemList data)
		{
			completionItemsDisplayed = data;
			showCompletionWindowMethodCalled = true;
			
			if (showCompletionWindowReturnsNull) {
				completionWindowDisplayed = null;
			} else {
				completionWindowDisplayed = new MockCompletionListWindow();
			}
			return completionWindowDisplayed;
		}
		
		public bool IsShowCompletionWindowMethodCalled {
			get { return showCompletionWindowMethodCalled; }
		}
		
		public bool ShowCompletionWindowReturnsNull {
			get { return showCompletionWindowReturnsNull; }
			set { showCompletionWindowReturnsNull = value; }
		}
		
		public MockCompletionListWindow CompletionWindowDisplayed {
			get { return completionWindowDisplayed; }
		}
		
		public ICompletionItemList CompletionItemsDisplayed {
			get { return completionItemsDisplayed; }
		}		
		
		public ICompletionItem[] CompletionItemsDisplayedToArray()
		{
			List<ICompletionItem> items = new List<ICompletionItem>();
			if (completionItemsDisplayed == null)
				return items.ToArray();
			foreach (ICompletionItem item in completionItemsDisplayed.Items) {
				items.Add(item);
			}
			return items.ToArray();
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
	}
}
