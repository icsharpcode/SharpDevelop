// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Drawing;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.TextEditor;
using ICSharpCode.TextEditor.Document;

namespace ICSharpCode.WixBinding
{
	/// <summary>
	/// A utility class that can replace xml in a Wix document currently open
	/// in the text editor.
	/// </summary>
	public class WixDocumentEditor
	{
		TextAreaControl textAreaControl;
		
		public WixDocumentEditor(TextAreaControl textAreaControl)
		{
			this.textAreaControl = textAreaControl;
		}
		
		/// <summary>
		/// Replaces the text at the given region with the specified xml. After replacing
		/// the inserted text is indented and then selected.
		/// </summary>
		public void Replace(DomRegion region, string xml)
		{
			IDocument document = textAreaControl.Document;
			ISegment segment = WixDocument.ConvertRegionToSegment(document, region);
						
			// Replace the original xml with the new xml and indent it.
			int originalLineCount = document.TotalNumberOfLines;
			document.Replace(segment.Offset, segment.Length, xml);
			int addedLineCount = document.TotalNumberOfLines - originalLineCount;
			
			// Make sure the text inserted is visible.
			textAreaControl.ScrollTo(region.BeginLine);

			// Indent the xml.
			int insertedCharacterCount = IndentLines(textAreaControl.TextArea, region.BeginLine + 1, region.EndLine + addedLineCount, document.FormattingStrategy);
			
			// Select the text just inserted.
			SelectText(textAreaControl.SelectionManager, document, segment.Offset, xml.Length + insertedCharacterCount);
		}
		
		/// <summary>
		/// Inserts and indents the xml at the specified location.
		/// </summary>
		public void Insert(int line, int column, string xml)
		{
			IDocument document = textAreaControl.Document;
			ISegment segment = document.GetLineSegment(line);
						
			// Insert the xml and indent it.
			int originalLineCount = document.TotalNumberOfLines;
			int offset = segment.Offset + column;
			document.Insert(offset, xml);
			int addedLineCount = document.TotalNumberOfLines - originalLineCount;
			
			// Make sure the text inserted is visible.
			textAreaControl.ScrollTo(line);

			// Indent the xml.
			int insertedCharacterCount = IndentLines(textAreaControl.TextArea, line, line + addedLineCount, document.FormattingStrategy);
			
			// Select the text just inserted.
			SelectText(textAreaControl.SelectionManager, document, offset, xml.Length + insertedCharacterCount);
		}
		
		/// <summary>
		/// Selects the specified text range.
		/// </summary>
		static void SelectText(SelectionManager selectionManager, IDocument document, int startOffset, int length)
		{
			selectionManager.ClearSelection();
			TextLocation selectionStart = document.OffsetToPosition(startOffset);
			TextLocation selectionEnd = document.OffsetToPosition(startOffset + length);
			selectionManager.SetSelection(selectionStart, selectionEnd);
		}
						
		/// <summary>
		/// Indents the lines and returns the total number of extra characters added.
		/// </summary>
		static int IndentLines(TextArea textArea, int begin, int end, IFormattingStrategy formattingStrategy)
		{
			int totalInsertedCharacters = 0;
			
			textArea.Document.UndoStack.StartUndoGroup();
			for (int i = begin; i <= end; ++i) {
				int existingCharacterCount = GetIndent(textArea, i);
				int insertedCharacterCount = formattingStrategy.IndentLine(textArea, i) - existingCharacterCount;
				totalInsertedCharacters += insertedCharacterCount;
			}
			textArea.Document.UndoStack.EndUndoGroup();
			
			return totalInsertedCharacters;
		}
		
		/// <summary>
		/// Gets the current indentation for the specified line.
		/// </summary>
		static int GetIndent(TextArea textArea, int line)
		{			
			int indentCount = 0;
			string lineText = TextUtilities.GetLineAsString(textArea.Document, line);			
			foreach (char ch in lineText) {
				if (Char.IsWhiteSpace(ch)) {
					indentCount++;
				} else {
					break;
				}
			}
			return indentCount;
		}
	}
}
