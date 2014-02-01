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
using ICSharpCode.Core;
using ICSharpCode.NRefactory.Editor;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Editor;
using ICSharpCode.SharpDevelop.Editor.CodeCompletion;
using Rhino.Mocks;

namespace XmlEditor.Tests.Utils
{
	public class MockTextEditor : ITextEditor
	{
		MockCompletionListWindow completionWindowDisplayed;
		ICompletionItemList completionItemsDisplayed;
		ITextEditorCaret caret = MockRepository.GenerateStub<ITextEditorCaret>();
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
		
		public IEnumerable<ISnippetCompletionItem> GetSnippets()
		{
			throw new NotImplementedException();
		}
		
		public object GetService(Type serviceType)
		{
			if (serviceType == typeof(ITextEditor)) {
				return this;
			}
			return document.GetService(serviceType);
		}
		
		public IList<ICSharpCode.SharpDevelop.Refactoring.IContextActionProvider> ContextActionProviders {
			get {
				throw new NotImplementedException();
			}
		}
	}
}
