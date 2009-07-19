/*
 * Created by SharpDevelop.
 * User: Daniel
 * Date: 19.07.2009
 * Time: 17:13
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using System.Linq;

using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.Highlighting;

namespace ICSharpCode.SharpDevelop.Editor.AvalonEdit
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
			DocumentHighlighter highlighter = textEditor.GetService(typeof(DocumentHighlighter)) as DocumentHighlighter;
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
	}
}
