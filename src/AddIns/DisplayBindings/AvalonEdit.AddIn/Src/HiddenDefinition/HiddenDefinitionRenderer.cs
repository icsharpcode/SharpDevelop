// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Windows.Controls.Primitives;
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
			
			if (BracketSearchResult == null) return;
			
			// verify if we have a open bracket
			if (this.editor.Document.GetCharAt(BracketSearchResult.OpeningBracketOffset) != '{')
				return;
			
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
			// get line
			var line = editor.Document.GetLineByOffset(offset);
			string text = editor.Document.Text;
			
			while (true) {
				if (line == null || line.IsDeleted) return null;
				string lineString = text.Substring(line.Offset, line.Length).Trim();
				
				if (lineString != "{" && !string.IsNullOrEmpty(lineString) &&
				    !lineString.StartsWith("//") && !lineString.StartsWith("/*") &&
				    !lineString.StartsWith("*") && !lineString.StartsWith("'"))
					break;
				line = line.PreviousLine;
			}
			
			if (!editor.TextArea.TextView.VisualLinesValid)
				return null;
			
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