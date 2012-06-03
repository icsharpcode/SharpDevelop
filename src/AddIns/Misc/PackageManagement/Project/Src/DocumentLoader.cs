// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Editor;
using ICSharpCode.SharpDevelop.Gui;

namespace ICSharpCode.PackageManagement
{
	public class DocumentLoader : IDocumentLoader
	{
		public IDocument LoadDocument(string fileName)
		{
			if (WorkbenchSingleton.InvokeRequired) {
				return WorkbenchSingleton.SafeThreadFunction(() => LoadDocument(fileName));
			} else {
				var textEditorProvider = FileService.OpenFile(fileName) as ITextEditorProvider;
				return new ThreadSafeDocument(textEditorProvider.TextEditor.Document);
			}
		}
	}
}
