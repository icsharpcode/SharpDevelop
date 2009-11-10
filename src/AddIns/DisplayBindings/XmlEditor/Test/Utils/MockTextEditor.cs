// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using System.Windows.Input;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Editor;
using ICSharpCode.SharpDevelop.Editor.CodeCompletion;
using ICSharpCode.TextEditor.Gui.CompletionWindow;

namespace XmlEditor.Tests.Utils
{
	public class MockTextEditor : ITextEditor
	{
		MockCompletionListWindow completionWindowDisplayed;
		ICompletionItemList completionItemsDisplayed;
		MockCaret caret = new MockCaret();
		MockDocument document = new MockDocument();
		FileName fileName;
		bool showCompletionWindowReturnsNull;
		bool showCompletionWindowMethodCalled;
						
		public MockTextEditor()
		{
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
		
		public MockDocument MockDocument {
			get { return document; }
		}
		
		public ITextEditorCaret Caret {
			get { return caret; }
		}
		
		public ITextEditorOptions Options {
			get {
				throw new NotImplementedException();
			}
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
				throw new NotImplementedException();
			}
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
			throw new NotImplementedException();
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
		
		public void ShowCompletionWindow(ICompletionDataProvider provider, char ch)
		{
			throw new NotImplementedException();
		}
		
		public object GetService(Type serviceType)
		{
			throw new NotImplementedException();
		}
	}
}
