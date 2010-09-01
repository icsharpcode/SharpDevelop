// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Windows.Input;
using ICSharpCode.SharpDevelop.Editor;

namespace PythonBinding.Tests.Utils
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
		
		public System.Collections.Generic.IEnumerable<ICSharpCode.SharpDevelop.Editor.CodeCompletion.ICompletionItem> GetSnippets()
		{
			throw new NotImplementedException();
		}
		
		public object GetService(Type serviceType)
		{
			return null;
		}
	}
}
