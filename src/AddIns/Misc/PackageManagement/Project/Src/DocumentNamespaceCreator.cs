// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Editor;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Refactoring;

namespace ICSharpCode.PackageManagement
{
	public class DocumentNamespaceCreator : IDocumentNamespaceCreator
	{
		public void AddNamespace(ICompilationUnit compilationUnit, string newNamespace)
		{
			if (WorkbenchSingleton.InvokeRequired) {
				WorkbenchSingleton.SafeThreadCall(() => AddNamespace(compilationUnit, newNamespace));
			} else {
				IViewContent view = FileService.OpenFile(compilationUnit.FileName);
				var textEditor = view as ITextEditorProvider;
				IDocument document = textEditor.TextEditor.Document;
				NamespaceRefactoringService.AddUsingDeclaration(compilationUnit, document, newNamespace, false);
			}
		}
	}
}
