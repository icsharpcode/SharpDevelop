// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.ComponentModel.Design;
using System.Linq;
using System.Windows.Controls.Primitives;

using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.Core.Presentation;
using ICSharpCode.SharpDevelop.Editor;
using ICSharpCode.SharpDevelop.Gui;

namespace ICSharpCode.AvalonEdit.AddIn.HiddenDefinition
{
	public class HiddenDefinitionRenderer : IDisposable
	{
		private CodeEditorView editor;
		private ExtendedPopup popup = new ExtendedPopup();
		private HiddenDefinitionControl control;
		
		public HiddenDefinitionRenderer(CodeEditorView editorView)
		{
			this.editor = editorView;
			control = new HiddenDefinitionControl();
			WorkbenchSingleton.Workbench.ActiveContentChanged += WorkbenchSingleton_Workbench_ActiveContentChanged;
		}

		public BracketSearchResult BracketSearchResult { get; set; }
		
		public void Dispose()
		{
			WorkbenchSingleton.Workbench.ActiveContentChanged -= WorkbenchSingleton_Workbench_ActiveContentChanged;
			ClosePopup();
			popup = null;
		}
		
		public void ClosePopup()
		{
			if (popup.IsOpen)
				popup.IsOpen = false;
		}
		
		public void Show()
		{
			ClosePopup();
			
			if (BracketSearchResult == null || BracketSearchResult.OpeningBracket != "{") return;
			
			var line = GetLineText(BracketSearchResult.OpeningBracketOffset);
			if(line == null) return;			
			
			control.DefinitionText = line;
			popup.Child = control;
			popup.HorizontalOffset = 70;
			popup.Placement = PlacementMode.Relative;
			popup.PlacementTarget = editor.TextArea;
			popup.IsOpen = true;
		}
		
		private string GetLineText(int offset)
		{
			// get folding manager
			var container = this.editor.Adapter.GetService(typeof(IServiceContainer)) as IServiceContainer;
			if (container == null) return null;
			var folding = container.GetService(typeof(ParserFoldingStrategy)) as ParserFoldingStrategy;
			if (folding == null) return null;
			
			// get folding
			var f = folding.FoldingManager.GetFoldingsContaining(offset).LastOrDefault();
			if (f == null) return null;
			
			// get line
			var line = editor.Document.GetLineByOffset(f.StartOffset);
			if (line == null || line.IsDeleted) return null;
			
			// check whether the line is visible
			int off = line.Offset;
			if (editor.TextArea.TextView.VisualLines[0].StartOffset > off) {
				return this.editor.TextArea.TextView.Document.GetText(off, line.Length);
			}
			
			return null;
		}
		
		private void WorkbenchSingleton_Workbench_ActiveContentChanged(object sender, EventArgs e)
		{
			ClosePopup();
		}
	}
}