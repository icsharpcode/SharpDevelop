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
using System.Text;
using ICSharpCode.NRefactory;
using ICSharpCode.NRefactory.Editor;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.SharpDevelop.Editor;

namespace ICSharpCode.WixBinding
{
	/// <summary>
	/// A utility class that can replace xml in a Wix document currently open
	/// in the text editor.
	/// </summary>	
	public class WixDocumentEditor
	{
		ITextEditor textEditor;
		IDocument document;
		
		public WixDocumentEditor(ITextEditor textEditor)
		{
			this.textEditor = textEditor;
			this.document = textEditor.Document;
		}
		
		/// <summary>
		/// Tries to replace the element defined by element name and its Id attribute in the
		/// text editor with the specified xml.
		/// </summary>
		/// <param name="id">The Id attribute of the element.</param>
		/// <param name="elementName">The name of the element.</param>
		/// <param name="xml">The replacement xml.</param>
		public DomRegion ReplaceElement(string elementAttributeId, string elementName, string replacementXml)
		{
			WixDocumentReader wixReader = new WixDocumentReader(document.Text);
			DomRegion region = wixReader.GetElementRegion(elementName, elementAttributeId);
			if (!region.IsEmpty) {
				Replace(region, replacementXml);
			}
			return region;
		}
		
		/// <summary>
		/// Replaces the text at the given region with the specified xml. After replacing
		/// the inserted text is indented and then selected.
		/// </summary>
		public void Replace(DomRegion region, string xml)
		{
			WixDocumentLineSegment segment = WixDocumentLineSegment.ConvertRegionToSegment(document, region);
			
			using (textEditor.Document.OpenUndoGroup()) {
				
				// Replace the original xml with the new xml and indent it.
				int originalLineCount = document.LineCount;
				int initialIndent = GetIndent(region.BeginLine);
				document.Replace(segment.Offset, segment.Length, xml);
				int addedLineCount = document.LineCount - originalLineCount;
				
				// Indent the xml.
				int insertedCharacterCount = IndentAllLinesTheSame(region.BeginLine + 1, region.EndLine + addedLineCount, initialIndent);
				
				// Make sure the text inserted is visible.
				textEditor.JumpTo(region.BeginLine, 1);
				
				// Select the text just inserted.
				int textInsertedLength = insertedCharacterCount + xml.Length;
				textEditor.Select(segment.Offset, textInsertedLength);
			}
		}
		
		public void InsertIndented(TextLocation location, string xml)
		{
			InsertIndented(location.Line, location.Column, xml);
		}
		
		/// <summary>
		/// Inserts and indents the xml at the specified location.
		/// </summary>
		/// <remarks>
		/// Lines and columns are one based.
		/// </remarks>
		public void InsertIndented(int line, int column, string xml)
		{
			using (textEditor.Document.OpenUndoGroup()) {

				// Insert the xml and indent it.
				IDocumentLine documentLine = document.GetLineByNumber(line);
				int initialIndent = GetIndent(line);
				int offset = documentLine.Offset + column - 1;
				int originalLineCount = document.LineCount;
				document.Insert(offset, xml);
				int addedLineCount = document.LineCount - originalLineCount;
	
				// Indent the xml.
				int insertedCharacterCount = IndentLines(line, line + addedLineCount, initialIndent);
				
				// Make sure the text inserted is visible.
				textEditor.JumpTo(line, 1);
				
				// Select the text just inserted.
				int textInsertedLength = xml.Length + insertedCharacterCount;
				textEditor.Select(offset, textInsertedLength);
			}
		}
		
		/// <summary>
		/// Indents the lines and returns the total number of extra characters added.
		/// </summary>
		int IndentLines(int begin, int end, int initialIndent)
		{
			int totalInsertedCharacters = 0;
			
			for (int i = begin; i <= end; ++i) {
				if ((i == end) || (i == begin)) {
					totalInsertedCharacters += IndentLine(i, initialIndent);
				} else {
					totalInsertedCharacters += IndentLine(i, initialIndent + 1);
				}
			}
			return totalInsertedCharacters;
		}
		
		int IndentAllLinesTheSame(int begin, int end, int indent)
		{
			int totalInsertedCharacters = 0;
			for (int i = begin; i <= end; ++i) {
				totalInsertedCharacters += IndentLine(i, indent);
			}
			return totalInsertedCharacters;
		}
		
		/// <summary>
		/// Gets the current indentation for the specified line.
		/// </summary>
		int GetIndent(int line)
		{			
			int whitespaceCharacterCount = 0;
			string lineText = GetLineAsString(line);			
			foreach (char ch in lineText) {
				if (Char.IsWhiteSpace(ch)) {
					whitespaceCharacterCount++;
				} else {
					break;
				}
			}
			if (textEditor.Options.ConvertTabsToSpaces) {
				return (whitespaceCharacterCount / textEditor.Options.IndentationSize);
			}
			return whitespaceCharacterCount;
		}
		
		string GetLineAsString(int line)
		{
			IDocumentLine documentLine = document.GetLineByNumber(line);
			return document.GetText(documentLine);
		}
		
		int IndentLine(int line, int howManyIndents)
		{
			IDocumentLine documentLine = document.GetLineByNumber(line);
			int offset = documentLine.Offset;
			
			string indentationString = GetIndentationString(howManyIndents);
			document.Insert(offset, indentationString);
			return indentationString.Length;
		}
		
		string GetIndentationString(int howManyIndents)
		{
			string singleIndent = GetSingleIndentString();
			
			StringBuilder indent = new StringBuilder();
			for (int i = 0; i < howManyIndents; ++i) {
				indent.Append(singleIndent);
			}
			return indent.ToString();
		}
		
		string GetSingleIndentString()
		{
			if (textEditor.Options.ConvertTabsToSpaces) {
				return new String(' ', textEditor.Options.IndentationSize);
			}
			return "\t";
		}
	}
}
