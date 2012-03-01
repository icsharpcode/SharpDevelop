// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using ICSharpCode.AvalonEdit.Folding;
using ICSharpCode.SharpDevelop.Editor;

namespace ICSharpCode.AspNet.Mvc.Folding
{
	public interface ITextEditorWithParseInformationFolding : IDisposable
	{
		bool IsParseInformationFoldingEnabled { get; set; }
		
		void UpdateFolds(IEnumerable<NewFolding> folds);
		void InstallFoldingManager();
		string GetTextSnapshot();
	}
}
