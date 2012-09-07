// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.SharpDevelop.Dom.Refactoring;
using ICSharpCode.SharpDevelop.Gui;

namespace ICSharpCode.PackageManagement
{
	public class DocumentLoader : IDocumentLoader
	{
		public IRefactoringDocument LoadRefactoringDocument(string fileName)
		{
			return LoadRefactoringDocumentView(fileName).RefactoringDocument;
		}
		
		public IRefactoringDocumentView LoadRefactoringDocumentView(string fileName)
		{
			if (WorkbenchSingleton.InvokeRequired) {
				return WorkbenchSingleton.SafeThreadFunction(() => LoadRefactoringDocumentView(fileName));
			} else {
				return new RefactoringDocumentView(fileName);
			}
		}
	}
}
