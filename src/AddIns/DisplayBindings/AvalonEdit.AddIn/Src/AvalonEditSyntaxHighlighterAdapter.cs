// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows;

using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.SharpDevelop.Editor;

namespace ICSharpCode.AvalonEdit.AddIn
{
	public class AvalonEditSyntaxHighlighterAdapter : ISyntaxHighlighter
	{
		ITextEditorComponent textEditor;
		
		public AvalonEditSyntaxHighlighterAdapter(ITextEditorComponent textEditor)
		{
			if (textEditor == null)
				throw new ArgumentNullException("textEditor");
			this.textEditor = textEditor;
		}
		
		public IEnumerable<string> GetSpanColorNamesFromLineStart(int lineNumber)
		{
			IHighlighter highlighter = textEditor.GetService(typeof(IHighlighter)) as IHighlighter;
			if (highlighter != null) {
				// delayed evaluation doesn't cause a problem here: GetSpanStack is called immediately,
				// only the where/select portian is evaluated later. But that won't be a problem because the
				// HighlightingSpan instance shouldn't change once it's in use.
				return from span in highlighter.GetSpanStack(lineNumber - 1)
					where span.SpanColor != null && span.SpanColor.Name != null
					select span.SpanColor.Name;
			} else {
				return Enumerable.Empty<string>();
			}
		}
		
		public HighlightingColor GetNamedColor(string name)
		{
			TextEditor editor = textEditor.GetService(typeof(TextEditor)) as TextEditor;
			if (editor == null)
				return null;
			var highlighting = editor.SyntaxHighlighting;
			if (highlighting == null)
				return null;
			return CustomizableHighlightingColorizer.CustomizeColor(name, CustomizedHighlightingColor.FetchCustomizations(highlighting.Name));
		}
		
		public HighlightedInlineBuilder BuildInlines(int lineNumber)
		{
			HighlightedInlineBuilder builder = new HighlightedInlineBuilder(textEditor.Document.GetText(textEditor.Document.GetLineByNumber(lineNumber)));
			IHighlighter highlighter = textEditor.GetService(typeof(IHighlighter)) as IHighlighter;
			if (highlighter != null) {
				HighlightedLine highlightedLine = highlighter.HighlightLine(lineNumber);
				int startOffset = highlightedLine.DocumentLine.Offset;
				// copy only the foreground and background colors
				foreach (HighlightedSection section in highlightedLine.Sections) {
					if (section.Color.Foreground != null) {
						builder.SetForeground(section.Offset - startOffset, section.Length, section.Color.Foreground.GetBrush(null));
					}
					if (section.Color.Background != null) {
						builder.SetBackground(section.Offset - startOffset, section.Length, section.Color.Background.GetBrush(null));
					}
				}
			}
			return builder;
		}
		
		public HighlightingColor DefaultTextColor {
			get {
				return GetNamedColor(CustomizableHighlightingColorizer.DefaultTextAndBackground);
			}
		}
	}
	
	public class DocumentSyntaxHighlighter : ISyntaxHighlighter
	{
		IDocument document;
		IHighlighter highlighter;
		string highlightingName;
		
		public DocumentSyntaxHighlighter(IDocument document, IHighlighter highlighter, string highlightingName)
		{
			if (document == null)
				throw new ArgumentNullException("document");
			this.document = document;
			this.highlighter = highlighter;
			this.highlightingName = highlightingName;
		}
		
		public IEnumerable<string> GetSpanColorNamesFromLineStart(int lineNumber)
		{
			if (highlighter != null) {
				// delayed evaluation doesn't cause a problem here: GetSpanStack is called immediately,
				// only the where/select portian is evaluated later. But that won't be a problem because the
				// HighlightingSpan instance shouldn't change once it's in use.
				return from span in highlighter.GetSpanStack(lineNumber - 1)
					where span.SpanColor != null && span.SpanColor.Name != null
					select span.SpanColor.Name;
			} else {
				return Enumerable.Empty<string>();
			}
		}
		
		public HighlightingColor GetNamedColor(string name)
		{
			return CustomizableHighlightingColorizer.CustomizeColor(name, CustomizedHighlightingColor.FetchCustomizations(highlightingName));
		}
		
		public HighlightedInlineBuilder BuildInlines(int lineNumber)
		{
			HighlightedInlineBuilder builder = new HighlightedInlineBuilder(document.GetLine(lineNumber).Text);
			if (highlighter != null) {
				HighlightedLine highlightedLine = highlighter.HighlightLine(lineNumber);
				int startOffset = highlightedLine.DocumentLine.Offset;
				// copy only the foreground and background colors
				foreach (HighlightedSection section in highlightedLine.Sections) {
					if (section.Color.Foreground != null) {
						builder.SetForeground(section.Offset - startOffset, section.Length, section.Color.Foreground.GetBrush(null));
					}
					if (section.Color.Background != null) {
						builder.SetBackground(section.Offset - startOffset, section.Length, section.Color.Background.GetBrush(null));
					}
				}
			}
			return builder;
		}
		
		public HighlightingColor DefaultTextColor {
			get {
				return GetNamedColor(CustomizableHighlightingColorizer.DefaultTextAndBackground);
			}
		}
	}
}
