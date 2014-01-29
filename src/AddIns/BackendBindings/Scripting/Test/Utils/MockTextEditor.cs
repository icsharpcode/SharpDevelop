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
using System.Windows.Input;
using ICSharpCode.NRefactory.Editor;
using ICSharpCode.SharpDevelop.Editor;

namespace ICSharpCode.Scripting.Tests.Utils
{
	public class MockTextEditor : ITextEditor
	{
		public MockTextEditorOptions MockTextEditorOptions = new MockTextEditorOptions();
		
		public FakeDocument FakeDocument = new FakeDocument();
		public FakeCaret FakeCaret = new FakeCaret();
		
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
			get { return FakeDocument; }
		}
		
		public ITextEditorCaret Caret {
			get { return FakeCaret; }
		}
		
		public ITextEditorOptions Options {
			get { return MockTextEditorOptions; }
		}
		
		public ICSharpCode.SharpDevelop.ILanguageBinding Language {
			get {
				throw new NotImplementedException();
			}
		}
		
		public int SelectionStart {
			get {
				throw new NotImplementedException();
			}
		}
		
		public int SelectionLength { get; set; }
		
		public string SelectedText { get; set; }
		
		public ICSharpCode.Core.FileName FileName {
			get {
				throw new NotImplementedException();
			}
		}
		
		public ICSharpCode.SharpDevelop.Editor.CodeCompletion.ICompletionListWindow ActiveCompletionWindow {
			get {
				throw new NotImplementedException();
			}
		}
		
		public ICSharpCode.SharpDevelop.Editor.CodeCompletion.IInsightWindow ActiveInsightWindow {
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
		
		public ICSharpCode.SharpDevelop.Editor.CodeCompletion.ICompletionListWindow ShowCompletionWindow(ICSharpCode.SharpDevelop.Editor.CodeCompletion.ICompletionItemList data)
		{
			throw new NotImplementedException();
		}
		
		public ICSharpCode.SharpDevelop.Editor.CodeCompletion.IInsightWindow ShowInsightWindow(System.Collections.Generic.IEnumerable<ICSharpCode.SharpDevelop.Editor.CodeCompletion.IInsightItem> items)
		{
			throw new NotImplementedException();
		}
		
		public System.Collections.Generic.IEnumerable<ICSharpCode.SharpDevelop.Editor.CodeCompletion.ISnippetCompletionItem> GetSnippets()
		{
			throw new NotImplementedException();
		}
		
		public object GetService(Type serviceType)
		{
			return null;
		}
		
		public System.Collections.Generic.IList<ICSharpCode.SharpDevelop.Refactoring.IContextActionProvider> ContextActionProviders {
			get {
				throw new NotImplementedException();
			}
		}
	}
}
