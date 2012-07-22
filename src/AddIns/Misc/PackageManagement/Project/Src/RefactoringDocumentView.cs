// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Dom.Refactoring;
using ICSharpCode.SharpDevelop.Editor;
using ICSharpCode.SharpDevelop.Gui;

namespace ICSharpCode.PackageManagement
{
	public class RefactoringDocumentView : IRefactoringDocumentView
	{		
		public RefactoringDocumentView(string fileName)
		{
			View = FileService.OpenFile(fileName);
			TextEditor = GetTextEditor();
			FormattingStrategy = TextEditor.Language.FormattingStrategy;
			RefactoringDocument = LoadDocument();
		}
		
		IViewContent View { get; set; }
		ITextEditor TextEditor { get; set; }
		IFormattingStrategy FormattingStrategy { get; set; }
		
		ITextEditor GetTextEditor()
		{
			var textEditorProvider = View as ITextEditorProvider;
			return textEditorProvider.TextEditor;
		}
		
		public IRefactoringDocument RefactoringDocument { get; private set; }
		
		IRefactoringDocument LoadDocument()
		{
			return new RefactoringDocumentAdapter(new ThreadSafeDocument(TextEditor.Document));
		}
		
		public ICompilationUnit Parse()
		{
			if (WorkbenchSingleton.InvokeRequired) {
				return WorkbenchSingleton.SafeThreadFunction(() => Parse());
			}
			return ParserService.ParseViewContent(View).CompilationUnit;
		}
		
		public void IndentLines(int beginLine, int endLine)
		{
			if (WorkbenchSingleton.InvokeRequired) {
				WorkbenchSingleton.SafeThreadCall(() => IndentLines(beginLine, endLine));
			} else {
				using (IDisposable undoGroup = TextEditor.Document.OpenUndoGroup()) {
					FormattingStrategy.IndentLines(TextEditor, beginLine, endLine);
				}
			}
		}
	}
}
