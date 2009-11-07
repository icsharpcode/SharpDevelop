// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald"/>
//     <version>$Revision$</version>
// </file>

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
			ParserFoldingStrategy strategy = editor.GetService(typeof(ParserFoldingStrategy)) as ParserFoldingStrategy;
			
			if (strategy != null) {
				// look for folding on this line:
				FoldingSection folding = strategy.FoldingManager.GetNextFolding(editor.Document.PositionToOffset(editor.Caret.Line, 1));
				if (folding == null || editor.Document.GetLineForOffset(folding.StartOffset).LineNumber != editor.Caret.Line) {
					// no folding found on current line: find innermost folding containing the caret
					folding = strategy.FoldingManager.GetFoldingsContaining(editor.Caret.Offset).LastOrDefault();
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
			ParserFoldingStrategy strategy = editor.GetService(typeof(ParserFoldingStrategy)) as ParserFoldingStrategy;
			
			if (strategy != null) {
				bool doFold = true;
				foreach (FoldingSection fm in strategy.FoldingManager.AllFoldings) {
					if (fm.IsFolded) {
						doFold = false;
						break;
					}
				}
				foreach (FoldingSection fm in strategy.FoldingManager.AllFoldings) {
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
			ParserFoldingStrategy strategy = editor.GetService(typeof(ParserFoldingStrategy)) as ParserFoldingStrategy;
			
			if (strategy != null) {
				foreach (FoldingSection fm in strategy.FoldingManager.AllFoldings) {
					fm.IsFolded = ParserFoldingStrategy.IsDefinition(fm);
				}
			}
		}
	}
}
