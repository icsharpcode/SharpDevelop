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
			if (popup != null && popup.IsOpen)
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
		
		/// <summary>
		/// Gets the line text near the offset.
		/// </summary>
		/// <param name="offset">Offset.</param>
		/// <returns></returns>
		private string GetLineText(int offset)
		{
			if (!editor.TextArea.TextView.VisualLinesValid)
				return null;
			
			// get line
			var documentLine = editor.Document.GetLineByOffset(offset);
			string documentText = editor.Document.Text;
			string lineText = string.Empty;
			int off, length;
			
			do {
				if (documentLine == null || documentLine.IsDeleted) return null;
				off = documentLine.Offset;
				length = documentLine.Length;
				lineText = documentText.Substring(off, length).Trim();
				
				documentLine = documentLine.PreviousLine;
			}
			while (lineText == "{" || string.IsNullOrEmpty(lineText) ||
			       lineText.StartsWith("//") || lineText.StartsWith("/*") ||
			       lineText.StartsWith("*") || lineText.StartsWith("'"));
			
			// check whether the line is visible
			if (editor.TextArea.TextView.VisualLines[0].StartOffset > off) {
				return this.editor.TextArea.TextView.Document.GetText(off, length);
			}
			
			return null;
		}
		
		private void WorkbenchSingleton_Workbench_ActiveContentChanged(object sender, EventArgs e)
		{
			ClosePopup();
		}
	}
}