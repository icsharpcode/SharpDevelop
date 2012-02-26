// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
		
		public bool IsParseInformationFoldingEnabled {
			get {
				if (CodeEditorView != null) {
					return !CodeEditorView.DisableParseInformationFolding;
				}
				return false;
			}
			set {
				if (CodeEditorView != null) {
					CodeEditorView.DisableParseInformationFolding = !value;
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
