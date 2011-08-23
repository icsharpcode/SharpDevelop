// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using ICSharpCode.AvalonEdit.Folding;
using ICSharpCode.SharpDevelop.Editor;
using ICSharpCode.SharpDevelop.Editor.AvalonEdit;

namespace ICSharpCode.XmlEditor
{
	/// <summary>
	/// Description of FoldingManagerAdapter.
	/// </summary>
	public class FoldingManagerAdapter : IFoldingManager
	{
		FoldingManager foldingManager;
		
		public FoldingManagerAdapter(ITextEditor textEditor)
		{
			AvalonEditTextEditorAdapter adaptor = textEditor as AvalonEditTextEditorAdapter;
			if (adaptor != null) {
				this.foldingManager = FoldingManager.Install(adaptor.TextEditor.TextArea);
			}
		}
		
		public void UpdateFoldings(IEnumerable<NewFolding> newFoldings, int firstErrorOffset)
		{
			if (foldingManager != null) {
				foldingManager.UpdateFoldings(newFoldings, firstErrorOffset);
			}
		}
		
		public void Dispose()
		{
			if (foldingManager != null) {
				FoldingManager.Uninstall(foldingManager);
				foldingManager = null;
			}
		}
	}
}
