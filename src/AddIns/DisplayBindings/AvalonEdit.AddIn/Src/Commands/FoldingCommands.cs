// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Linq;
using ICSharpCode.AvalonEdit.Folding;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Editor;
using ICSharpCode.SharpDevelop.Gui;

namespace ICSharpCode.AvalonEdit.AddIn.Commands
{
	public class ToggleFolding : AbstractMenuCommand
	{
		public override void Run()
		{
			ITextEditorProvider provider = WorkbenchSingleton.Workbench.ActiveViewContent as ITextEditorProvider;
			ITextEditor editor = provider.TextEditor;
			FoldingManager foldingManager = editor.GetService(typeof(FoldingManager)) as FoldingManager;
			
			if (foldingManager != null) {
				// look for folding on this line:
				FoldingSection folding = foldingManager.GetNextFolding(editor.Document.PositionToOffset(editor.Caret.Line, 1));
				if (folding == null || editor.Document.GetLineForOffset(folding.StartOffset).LineNumber != editor.Caret.Line) {
					// no folding found on current line: find innermost folding containing the caret
					folding = foldingManager.GetFoldingsContaining(editor.Caret.Offset).LastOrDefault();
				}
				if (folding != null) {
					folding.IsFolded = !folding.IsFolded;
				}
			}
		}
	}
	
	public class ToggleAllFoldings : AbstractMenuCommand
	{
		public override void Run()
		{
			ITextEditorProvider provider = WorkbenchSingleton.Workbench.ActiveViewContent as ITextEditorProvider;
			ITextEditor editor = provider.TextEditor;
			FoldingManager foldingManager = editor.GetService(typeof(FoldingManager)) as FoldingManager;
			
			if (foldingManager != null) {
				bool doFold = true;
				foreach (FoldingSection fm in foldingManager.AllFoldings) {
					if (fm.IsFolded) {
						doFold = false;
						break;
					}
				}
				foreach (FoldingSection fm in foldingManager.AllFoldings) {
					fm.IsFolded = doFold;
				}
			}
		}
	}
	
	public class ShowDefinitionsOnly : AbstractMenuCommand
	{
		public override void Run()
		{
			ITextEditorProvider provider = WorkbenchSingleton.Workbench.ActiveViewContent as ITextEditorProvider;
			ITextEditor editor = provider.TextEditor;
			FoldingManager foldingManager = editor.GetService(typeof(FoldingManager)) as FoldingManager;
			
			if (foldingManager != null) {
				foreach (FoldingSection fm in foldingManager.AllFoldings) {
					fm.IsFolded = ParserFoldingStrategy.IsDefinition(fm);
				}
			}
		}
	}
}
