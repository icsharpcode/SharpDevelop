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
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Folding;
using ICSharpCode.AvalonEdit.Rendering;
using ICSharpCode.NRefactory.Editor;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Editor;

namespace ICSharpCode.AvalonEdit.AddIn
{
	/// <summary>
	/// Description of TextMarkerToolTipProvider.
	/// </summary>
	public class TextMarkerToolTipProvider : ITextAreaToolTipProvider
	{
		public void HandleToolTipRequest(ToolTipRequestEventArgs args)
		{
			if (args.InDocument) {
				int offset = args.Editor.Document.GetOffset(args.LogicalPosition);
				
				FoldingManager foldings = args.Editor.GetService(typeof(FoldingManager)) as FoldingManager;
				if (foldings != null) {
					var foldingsAtOffset = foldings.GetFoldingsAt(offset);
					FoldingSection collapsedSection = foldingsAtOffset.FirstOrDefault(section => section.IsFolded);
					
					if (collapsedSection != null) {
						args.SetToolTip(GetTooltipTextForCollapsedSection(args, collapsedSection));
					}
				}
				
				TextMarkerService textMarkerService = args.Editor.GetService(typeof(ITextMarkerService)) as TextMarkerService;
				if (textMarkerService != null) {
					var markersAtOffset = textMarkerService.GetMarkersAtOffset(offset);
					ITextMarker markerWithToolTip = markersAtOffset.FirstOrDefault(marker => marker.ToolTip != null);
					
					if (markerWithToolTip != null) {
						args.SetToolTip(markerWithToolTip.ToolTip);
					}
				}
			}
		}
		
		string GetTooltipTextForCollapsedSection(ToolTipRequestEventArgs args, FoldingSection foldingSection)
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
			
			const int maxLineCount = 15;
			
			TextDocument document = (TextDocument)args.Editor.Document;
			int startOffset = foldingSection.StartOffset;
			int endOffset = foldingSection.EndOffset;
			
			DocumentLine startLine = document.GetLineByOffset(startOffset);
			DocumentLine endLine = document.GetLineByOffset(endOffset);
			StringBuilder builder = new StringBuilder();
			
			DocumentLine current = startLine;
			ISegment startIndent = TextUtilities.GetLeadingWhitespace(document, startLine);
			int lineCount = 0;
			while (current != endLine.NextLine && lineCount < maxLineCount) {
				ISegment currentIndent = TextUtilities.GetLeadingWhitespace(document, current);
				
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
