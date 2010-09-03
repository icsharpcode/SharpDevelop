// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.IO;
using System.IO.Packaging;
using System.Printing;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Xps;
using System.Windows.Xps.Packaging;

using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Highlighting;

namespace ICSharpCode.AvalonEdit.AddIn
{
	/// <summary>
	/// Helps printing documents.
	/// </summary>
	public static class DocumentPrinter
	{
		public static Block ConvertTextDocumentToBlock(TextDocument document, IHighlighter highlighter)
		{
			if (document == null)
				throw new ArgumentNullException("document");
//			Table table = new Table();
//			table.Columns.Add(new TableColumn { Width = GridLength.Auto });
//			table.Columns.Add(new TableColumn { Width = new GridLength(1, GridUnitType.Star) });
//			TableRowGroup trg = new TableRowGroup();
//			table.RowGroups.Add(trg);
			Paragraph p = new Paragraph();
			foreach (DocumentLine line in document.Lines) {
				int lineNumber = line.LineNumber;
//				TableRow row = new TableRow();
//				trg.Rows.Add(row);
//				row.Cells.Add(new TableCell(new Paragraph(new Run(lineNumber.ToString()))) { TextAlignment = TextAlignment.Right });
				HighlightedInlineBuilder inlineBuilder = new HighlightedInlineBuilder(document.GetText(line));
				if (highlighter != null) {
					HighlightedLine highlightedLine = highlighter.HighlightLine(lineNumber);
					int lineStartOffset = line.Offset;
					foreach (HighlightedSection section in highlightedLine.Sections)
						inlineBuilder.SetHighlighting(section.Offset - lineStartOffset, section.Length, section.Color);
				}
//				Paragraph p = new Paragraph();
//				row.Cells.Add(new TableCell(p));
				p.Inlines.AddRange(inlineBuilder.CreateRuns());
				p.Inlines.Add(new LineBreak());
			}
			return p;
		}
		
		public static FlowDocument CreateFlowDocumentForEditor(TextEditor editor)
		{
			IHighlighter highlighter = editor.TextArea.GetService(typeof(IHighlighter)) as IHighlighter;
			FlowDocument doc = new FlowDocument(ConvertTextDocumentToBlock(editor.Document, highlighter));
			doc.FontFamily = editor.FontFamily;
			doc.FontSize = editor.FontSize;
			return doc;
		}
	}
}
