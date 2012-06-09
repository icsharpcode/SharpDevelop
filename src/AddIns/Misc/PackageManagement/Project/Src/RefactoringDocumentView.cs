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
		IViewContent view;
		
		public RefactoringDocumentView(string fileName)
		{
			view = FileService.OpenFile(fileName);
			RefactoringDocument = LoadDocument();
		}
		
		IRefactoringDocument LoadDocument()
		{
			var textEditorProvider = view as ITextEditorProvider;
			return new RefactoringDocumentAdapter(new ThreadSafeDocument(textEditorProvider.TextEditor.Document));
		}
		
		public IRefactoringDocument RefactoringDocument { get; private set; }
		
		public ICompilationUnit Parse()
		{
			if (WorkbenchSingleton.InvokeRequired) {
				return WorkbenchSingleton.SafeThreadFunction(() => Parse());
			}
			return ParserService.ParseViewContent(view).CompilationUnit;
		}
	}
}
