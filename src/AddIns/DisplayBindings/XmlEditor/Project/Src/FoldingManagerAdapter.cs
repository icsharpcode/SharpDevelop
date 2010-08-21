// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

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
	}
}
