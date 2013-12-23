// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.Core;
using ICSharpCode.NRefactory.Editor;
using ICSharpCode.NRefactory.TypeSystem;
using CSharpBinding.Parser;

namespace CSharpBinding.FormsDesigner
{
	public interface ICSharpDesignerLoaderContext
	{
		//IDocument PrimaryFileDocument { get; }
		IDocument DesignerCodeFileDocument { get; }
		CSharpFullParseInformation GetPrimaryFileParseInformation();
		ICompilation GetCompilation();
		IDocument GetDocument(FileName fileName);
		void ShowSourceCode(int lineNumber = 0);
	}
}
