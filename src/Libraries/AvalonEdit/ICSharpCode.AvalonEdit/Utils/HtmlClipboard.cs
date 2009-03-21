// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <author name="Daniel Grunwald"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Diagnostics;
using System.Globalization;
using System.Text;
using System.Windows;

using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Highlighting;

namespace ICSharpCode.AvalonEdit.Utils
{
	/// <summary>
	/// Allows copying HTML text to the clipboard.
	/// </summary>
	public static class HtmlClipboard
	{
		/// <summary>
		/// Builds a header for the CF_HTML clipboard format.
		/// </summary>
		static string BuildHeader(int startHTML, int endHTML, int startFragment, int endFragment)
		{
			StringBuilder b = new StringBuilder();
			b.AppendLine("Version:1.0");
			b.AppendLine("StartHTML:" + startHTML.ToString("d8", CultureInfo.InvariantCulture));
			b.AppendLine("EndHTML:" + endHTML.ToString("d8", CultureInfo.InvariantCulture));
			b.AppendLine("StartFragment:" + startFragment.ToString("d8", CultureInfo.InvariantCulture));
			b.AppendLine("EndFragment:" + endFragment.ToString("d8", CultureInfo.InvariantCulture));
			return b.ToString();
		}
		
		/// <summary>
		/// Sets the TextDataFormat.Html on the data object to the specified html fragment.
		/// This helper methods takes care of creating the necessary CF_HTML header.
		/// </summary>
		public static void SetHtml(DataObject dataObject, string htmlFragment)
		{
			if (dataObject == null)
				throw new ArgumentNullException("dataObject");
			if (htmlFragment == null)
				throw new ArgumentNullException("htmlFragment");
			
			string htmlStart = @"<!DOCTYPE HTML PUBLIC ""-//W3C//DTD HTML 4.0 Transitional//EN"">" + Environment.NewLine
				+ "<HTML>" + Environment.NewLine
				+ "<HEAD><TITLE>Copied from AvalonEdit</TITLE></HEAD>" + Environment.NewLine
				+ "<BODY>" + Environment.NewLine
				+ "<!--StartFragment-->" + Environment.NewLine;
			string htmlEnd = "<!--EndFragment-->" + Environment.NewLine + "</BODY>" + Environment.NewLine + "</HTML>" + Environment.NewLine;
			string dummyHeader = BuildHeader(0, 0, 0, 0);
			// the offsets are stored as UTF-8 bytes (see CF_HTML documentation)
			int startHTML = dummyHeader.Length;
			int startFragment = startHTML + htmlStart.Length;
			int endFragment = startFragment + Encoding.UTF8.GetByteCount(htmlFragment);
			int endHTML = endFragment + htmlEnd.Length;
			string cf_html = BuildHeader(startHTML, endHTML, startFragment, endFragment) + htmlStart + htmlFragment + htmlEnd;
			Debug.WriteLine(cf_html);
			dataObject.SetText(cf_html, TextDataFormat.Html);
		}
		
		/// <summary>
		/// Creates a HTML fragment from a part of a document.
		/// </summary>
		/// <param name="document">The document to create HTML from.</param>
		/// <param name="highlighter">The highlighter used to highlight the document.</param>
		/// <param name="segment">The part of the document to create HTML for. You can pass null to create HTML for the whole document.</param>
		/// <returns>HTML code for the document part.</returns>
		public static string CreateHtmlFragment(TextDocument document, DocumentHighlighter highlighter, ISegment segment)
		{
			if (document == null)
				throw new ArgumentNullException("document");
			if (segment == null)
				segment = new SimpleSegment(0, document.TextLength);
			
			StringBuilder html = new StringBuilder();
			int segmentEndOffset = segment.EndOffset;
			DocumentLine line = document.GetLineByOffset(segment.Offset);
			while (line != null && line.Offset < segmentEndOffset) {
				HighlightedLine highlightedLine;
				if (highlighter != null)
					highlightedLine = highlighter.HighlightLine(line);
				else
					highlightedLine = new HighlightedLine(line);
				SimpleSegment s = segment.GetOverlap(line);
				if (html.Length > 0)
					html.AppendLine("<br>");
				html.Append(highlightedLine.ToHtml(s.Offset, s.EndOffset));
				line = line.NextLine;
			}
			return html.ToString();
		}
		
		/// <summary>
		/// Creates a HTML fragment for the selected part of the document.
		/// </summary>
		public static string CreateHtmlFragmentForSelection(TextArea textArea)
		{
			if (textArea == null)
				throw new ArgumentNullException("textArea");
			DocumentHighlighter highlighter = textArea.GetService(typeof(DocumentHighlighter)) as DocumentHighlighter;
			StringBuilder html = new StringBuilder();
			foreach (ISegment selectedSegment in textArea.Selection.Segments) {
				html.AppendLine(CreateHtmlFragment(textArea.Document, highlighter, selectedSegment));
			}
			return html.ToString();
		}
		
		/// <summary>
		/// Escapes text and writes the result to the StringBuilder.
		/// </summary>
		internal static void EscapeHtml(StringBuilder b, string text)
		{
			int spaceCount = -1;
			foreach (char c in text) {
				if (c == ' ') {
					if (spaceCount < 0)
						b.Append("&nbsp;");
					else
						spaceCount++;
				} else if (c == '\t') {
					if (spaceCount < 0)
						spaceCount = 0;
					// TODO: use tab width setting
					spaceCount += 4;
				} else {
					if (spaceCount == 1) {
						b.Append(' ');
					} else if (spaceCount >= 1) {
						for (int i = 0; i < spaceCount; i++) {
							b.Append("&nbsp;");
						}
					}
					spaceCount = 0;
					switch (c) {
						case '<':
							b.Append("&lt;");
							break;
						case '>':
							b.Append("&gt;");
							break;
						case '&':
							b.Append("&amp;");
							break;
						case '"':
							b.Append("&quot;");
							break;
						default:
							b.Append(c);
							break;
					}
				}
			}
			for (int i = 0; i < spaceCount; i++) {
				b.Append("&nbsp;");
			}
		}
	}
}
