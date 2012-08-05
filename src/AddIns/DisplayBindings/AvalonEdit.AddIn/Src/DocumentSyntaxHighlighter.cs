// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using ICSharpCode.AvalonEdit.AddIn.Options;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.NRefactory.Editor;
using ICSharpCode.SharpDevelop.Editor;
using ICSharpCode.SharpDevelop.Editor.AvalonEdit;

namespace ICSharpCode.AvalonEdit.AddIn
{
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
				return from color in highlighter.GetColorStack(lineNumber - 1)
					where color.Name != null
					select color.Name;
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
			HighlightedInlineBuilder builder = new HighlightedInlineBuilder(document.GetText(document.GetLineByNumber(lineNumber)));
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
		
		public event EventHandler VisibleDocumentLinesChanged;
		
		public IHighlightingDefinition HighlightingDefinition {
			get {
				throw new NotImplementedException();
			}
		}
		
		public void AddAdditionalHighlighter(IHighlighter highlighter)
		{
			throw new NotImplementedException();
		}
		
		public void RemoveAdditionalHighlighter(IHighlighter highlighter)
		{
			throw new NotImplementedException();
		}
		
		public void InvalidateLine(IDocumentLine line)
		{
			throw new NotImplementedException();
		}
		
		public void InvalidateAll()
		{
			throw new NotImplementedException();
		}
		
		public IEnumerable<IDocumentLine> GetVisibleDocumentLines()
		{
			return Enumerable.Empty<IDocumentLine>();
		}
	}
}
