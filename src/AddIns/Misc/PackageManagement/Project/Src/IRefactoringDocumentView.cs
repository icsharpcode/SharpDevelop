// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Dom.Refactoring;

namespace ICSharpCode.PackageManagement
{
	public interface IRefactoringDocumentView
	{
		IRefactoringDocument RefactoringDocument { get; }
		ICompilationUnit Parse();
		void IndentLines(int beginLine, int endLine);
	}
}
