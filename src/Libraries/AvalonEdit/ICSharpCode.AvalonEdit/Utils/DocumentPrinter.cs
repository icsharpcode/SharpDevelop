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

using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.NRefactory.Editor;

namespace ICSharpCode.AvalonEdit.Utils
{
	/// <summary>
	/// Helps printing documents.
	/// </summary>
	public static class DocumentPrinter
	{
		public static Block ConvertTextDocumentToBlock(ReadOnlyDocument document, IHighlightingDefinition highlightingDefinition)
		{
			IHighlighter highlighter;
			if (highlightingDefinition != null)
				highlighter = new DocumentHighlighter(document, highlightingDefinition);
			else
				highlighter = null;
			return ConvertTextDocumentToBlock(document, highlighter);
		}
		
		public static Block ConvertTextDocumentToBlock(IDocument document, IHighlighter highlighter)
		{
			if (document == null)
				throw new ArgumentNullException("document");
//			Table table = new Table();
//			table.Columns.Add(new TableColumn { Width = GridLength.Auto });
//			table.Columns.Add(new TableColumn { Width = new GridLength(1, GridUnitType.Star) });
//			TableRowGroup trg = new TableRowGroup();
//			table.RowGroups.Add(trg);
			Paragraph p = new Paragraph();
			p.TextAlignment = TextAlignment.Left;
			for (int lineNumber = 1; lineNumber <= document.LineCount; lineNumber++) {
				if (lineNumber > 1)
					p.Inlines.Add(new LineBreak());
				var line = document.GetLineByNumber(lineNumber);
//				TableRow row = new TableRow();
//				trg.Rows.Add(row);
//				row.Cells.Add(new TableCell(new Paragraph(new Run(lineNumber.ToString()))) { TextAlignment = TextAlignment.Right });
//				Paragraph p = new Paragraph();
//				row.Cells.Add(new TableCell(p));
				if (highlighter != null) {
					HighlightedLine highlightedLine = highlighter.HighlightLine(lineNumber);
					p.Inlines.AddRange(highlightedLine.ToRichText().CreateRuns());
				} else {
					p.Inlines.Add(document.GetText(line));
				}
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
