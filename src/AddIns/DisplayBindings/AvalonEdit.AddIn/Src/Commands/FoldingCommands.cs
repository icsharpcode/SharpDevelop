// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

using System;
using System.Linq;
using ICSharpCode.AvalonEdit.Folding;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Editor;
using ICSharpCode.SharpDevelop.Gui;

namespace ICSharpCode.AvalonEdit.AddIn.Commands
{
	public class ToggleFolding : AbstractMenuCommand
	{
		public override void Run()
		{
			ITextEditor editor = SD.GetActiveViewContentService<ITextEditor>();
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
			ITextEditor editor = SD.GetActiveViewContentService<ITextEditor>();
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
			ITextEditor editor = SD.GetActiveViewContentService<ITextEditor>();
			FoldingManager foldingManager = editor.GetService(typeof(FoldingManager)) as FoldingManager;
			
			if (foldingManager != null) {
				foreach (FoldingSection fm in foldingManager.AllFoldings) {
					var newFolding = fm.Tag as NewFolding;
					if (newFolding != null)
						fm.IsFolded = newFolding.IsDefinition;
				}
			}
		}
	}
}
