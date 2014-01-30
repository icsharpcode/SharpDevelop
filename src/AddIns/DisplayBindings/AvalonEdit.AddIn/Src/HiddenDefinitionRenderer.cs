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
