// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.SharpDevelop.Dom.Refactoring;
using ICSharpCode.SharpDevelop.Editor;

namespace ICSharpCode.PackageManagement
{
	public interface IDocumentLoader
	{
		IRefactoringDocument LoadRefactoringDocument(string fileName);
		IRefactoringDocumentView LoadRefactoringDocumentView(string fileName);
	}
}
