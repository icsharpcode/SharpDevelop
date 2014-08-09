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
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.NRefactory.Editor;

namespace ICSharpCode.SharpDevelop.Editor
{
	public static class ToolTipUtils
	{
		public static string GetAlignedText(IDocument document, int startOffset, int endOffset, int maxLineCount = 15)
		{
			// This fixes SD-1394:
			// Each line is checked for leading indentation whitespaces. If
			// a line has the same or more indentation than the first line,
			// it is reduced. If a line is less indented than the first line
			// the indentation is removed completely.
			//
			// See the following example:
			// 	line 1
			// 		line 2
			// 			line 3
			//  line 4
			//
			// is reduced to:
			// line 1
			// 	line 2
			// 		line 3
			// line 4
			
			if (maxLineCount < 1)
				maxLineCount = int.MaxValue;
			
			IDocumentLine startLine = document.GetLineByOffset(startOffset);
			IDocumentLine endLine = document.GetLineByOffset(endOffset);
			StringBuilder builder = new StringBuilder();
			
			IDocumentLine current = startLine;
			ISegment startIndent = TextUtilities.GetWhitespaceAfter(document, startLine.Offset);
			int lineCount = 0;
			while (current != endLine.NextLine && lineCount < maxLineCount) {
				ISegment currentIndent = TextUtilities.GetWhitespaceAfter(document, current.Offset);
				
				if (current == startLine && current == endLine)
					builder.Append(document.GetText(startOffset, endOffset - startOffset));
				else if (current == startLine) {
					if (current.EndOffset - startOffset > 0)
						builder.AppendLine(document.GetText(startOffset, current.EndOffset - startOffset).TrimStart());
				} else if (current == endLine) {
					if (startIndent.Length <= currentIndent.Length)
						builder.Append(document.GetText(current.Offset + startIndent.Length, endOffset - current.Offset - startIndent.Length));
					else
						builder.Append(document.GetText(current.Offset + currentIndent.Length, endOffset - current.Offset - currentIndent.Length));
				} else {
					if (startIndent.Length <= currentIndent.Length)
						builder.AppendLine(document.GetText(current.Offset + startIndent.Length, current.Length - startIndent.Length));
					else
						builder.AppendLine(document.GetText(current.Offset + currentIndent.Length, current.Length - currentIndent.Length));
				}
				
				current = current.NextLine;
				lineCount++;
			}
			if (current != endLine.NextLine)
				builder.Append("...");
			
			return builder.ToString();
		}
	}
}
