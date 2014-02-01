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
using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.AddIn;
using ICSharpCode.AvalonEdit.Folding;
using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.SharpDevelop.Editor;
using ICSharpCode.SharpDevelop.Editor.AvalonEdit;

namespace ICSharpCode.AspNet.Mvc.Folding
{
	public class TextEditorWithParseInformationFolding : ITextEditorWithParseInformationFolding
	{
		ITextEditor textEditor;
		CodeEditorView codeEditorView;
		FoldingManager foldingManager;
		
		public TextEditorWithParseInformationFolding(ITextEditor textEditor)
		{
			this.textEditor = textEditor;
		}
		
		public void InstallFoldingManager()
		{
			var textEditorAdapter = textEditor as AvalonEditTextEditorAdapter;
			if (textEditorAdapter != null) {
				foldingManager = FoldingManager.Install(textEditorAdapter.TextEditor.TextArea);
			}
		}
		
		// TODO disable parse information folding?
		public bool IsParseInformationFoldingEnabled {
			get {
				if (CodeEditorView != null) {
					//return !CodeEditorView.DisableParseInformationFolding;
				}
				return false;
			}
			set {
				if (CodeEditorView != null) {
					//CodeEditorView.DisableParseInformationFolding = !value;
				}
			}
		}
		
		CodeEditorView CodeEditorView {
			get {
				if (codeEditorView == null) {
					codeEditorView = GetCodeEditorView();
				}
				return codeEditorView;
			}
		}	
		
		CodeEditorView GetCodeEditorView()
		{
			return textEditor.GetService(typeof(TextEditor)) as CodeEditorView;
		}
		
		public void UpdateFolds(IEnumerable<NewFolding> folds)
		{
			foldingManager.UpdateFoldings(folds, -1);
		}
		
		public string GetTextSnapshot()
		{
			return textEditor.Document.CreateSnapshot().Text;
		}
		
		public void Dispose()
		{
			FoldingManager.Uninstall(foldingManager);
		}
	}
}
