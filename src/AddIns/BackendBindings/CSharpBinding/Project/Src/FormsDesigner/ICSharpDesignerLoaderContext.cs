// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.NRefactory.Editor;
using ICSharpCode.NRefactory.TypeSystem;
using CSharpBinding.Parser;

namespace CSharpBinding.FormsDesigner
{
	/// <summary>
	/// Description of ICSharpDesignerLoaderContext.
	/// </summary>
	public interface ICSharpDesignerLoaderContext
	{
		//IDocument PrimaryFileDocument { get; }
		IDocument DesignerCodeFileDocument { get; }
		CSharpFullParseInformation GetPrimaryFileParseInformation();
		ICompilation GetCompilation();
	}
}
