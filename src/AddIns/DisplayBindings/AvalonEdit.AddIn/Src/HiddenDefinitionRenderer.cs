// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.AvalonEdit.Rendering;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Editor;
using ICSharpCode.SharpDevelop.Widgets;

namespace ICSharpCode.AvalonEdit.AddIn.HiddenDefinition
{
	public class HiddenDefinitionRenderer
	{
		readonly CodeEditorView editor;
		ExtendedPopup popup;
		int currentStartOffset;
		
		public HiddenDefinitionRenderer(CodeEditorView editorView)
		{
			this.editor = editorView;
		}

		void textView_VisualLinesChanged(object sender, EventArgs e)
		{
			// This event is registered only while the popup exists
			var textView = editor.TextArea.TextView;
			VisualLine visualLine = textView.VisualLines.FirstOrDefault();
			popup.IsOpenIfFocused = visualLine != null && visualLine.StartOffset > currentStartOffset;
		}

		public void ClosePopup()
		{
			if (popup != null) {
				editor.TextArea.TextView.VisualLinesChanged -= textView_VisualLinesChanged;
				popup.IsOpenIfFocused = false;
				popup = null;
			}
		}
		
		public void Show(BracketSearchResult bracketSearchResult)
		{
			ClosePopup();
			
			if (bracketSearchResult == null || bracketSearchResult.DefinitionHeaderLength == 0)
				return;
			
			int startOffset = bracketSearchResult.DefinitionHeaderOffset;
			int endOffset = startOffset + bracketSearchResult.DefinitionHeaderLength;
			// show whole line even if the definition is only a part:
			DocumentLine firstLine = editor.Document.GetLineByOffset(startOffset);
			DocumentLine lastLine = editor.Document.GetLineByOffset(endOffset);
			
			TextEditor popupEditor = new TextEditor();
			popupEditor.IsReadOnly = true;
			popupEditor.HorizontalScrollBarVisibility = ScrollBarVisibility.Hidden;
			popupEditor.VerticalScrollBarVisibility = ScrollBarVisibility.Hidden;
			popupEditor.CopySettingsFrom(editor);
			
			IHighlighter oldHighlighter = editor.GetRequiredService<IHighlighter>();
			FixedHighlighter newHighlighter = FixedHighlighter.CreateView(oldHighlighter, firstLine.Offset, lastLine.EndOffset);
			popupEditor.Document = (TextDocument)newHighlighter.Document;
			popupEditor.TextArea.TextView.LineTransformers.Add(new HighlightingColorizer(newHighlighter));
			
			popup = new ExtendedPopup(editor.TextArea);
			const double borderThickness = 1;
			popup.Child = new Border() {
				Child = popupEditor,
				BorderBrush = editor.TextArea.Foreground,
				BorderThickness = new Thickness(borderThickness)
			};
			popup.HorizontalOffset = -borderThickness - editor.TextArea.TextView.ScrollOffset.X;
			popup.Placement = PlacementMode.Top;
			popup.PlacementTarget = editor.TextArea.TextView;
			this.currentStartOffset = firstLine.Offset;
			editor.TextArea.TextView.VisualLinesChanged += textView_VisualLinesChanged;
			if (editor.TextArea.TextView.VisualLinesValid)
				textView_VisualLinesChanged(null, null); // open popup if necessary
		}
	}
}