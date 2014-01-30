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
using System.Windows.Media;

using ICSharpCode.NRefactory.Editor;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Editor;
using ICSharpCode.SharpDevelop.WinForms;

namespace ICSharpCode.CodeCoverage
{
	/// <summary>
	/// Highlights the covered and not covered code in the text editor.
	/// </summary>
	public class CodeCoverageHighlighter
	{
		/// <summary>
		/// Adds text markers for the code coverage sequence points.
		/// </summary>
		/// <remarks>The sequence points are added to the marker strategy even
		/// if they are not all for the same document.</remarks>
		public void AddMarkers(IDocument document, List<CodeCoverageSequencePoint> sequencePoints)
		{
			foreach (CodeCoverageSequencePoint sequencePoint in sequencePoints) {
				AddMarker(document, sequencePoint);
			}
		}
		
		public void AddMarker(IDocument document, CodeCoverageSequencePoint sequencePoint)
		{
			if (!IsValidSequencePoint(document, sequencePoint)) {
				return;
			}
			
			ITextMarkerService markerService = document.GetService(typeof(ITextMarkerService)) as ITextMarkerService;
			if (markerService != null) {
				int startOffset = document.PositionToOffset(sequencePoint.Line, sequencePoint.Column);
				int endOffset = document.PositionToOffset(sequencePoint.EndLine, sequencePoint.EndColumn);
				ITextMarker marker = markerService.Create(startOffset, endOffset - startOffset);
				marker.Tag = typeof(CodeCoverageHighlighter);
				marker.BackgroundColor = GetSequencePointBackColor(sequencePoint).ToWpf();
				marker.ForegroundColor = GetSequencePointForeColor(sequencePoint).ToWpf();
			}
		}
		
		/// <summary>
		/// Removes all CodeCoverageMarkers from the marker strategy.
		/// </summary>
		public void RemoveMarkers(IDocument document)
		{
			ITextMarkerService markerService = document.GetService(typeof(ITextMarkerService)) as ITextMarkerService;
			if (markerService != null) {
				markerService.RemoveAll(IsCodeCoverageTextMarker);
			}
		}
		
		bool IsCodeCoverageTextMarker(ITextMarker marker)
		{
			Type type = marker.Tag as Type;
			return type == typeof(CodeCoverageHighlighter);
		}
		
		/// <summary>
		/// Checks whether the sequence point can be added to the document.
		/// </summary>
		/// <remarks>
		/// Checks for invalid start lines, start columns, end columns and end
		/// lines that cannot fit in the document.</remarks>
		bool IsValidSequencePoint(IDocument document, CodeCoverageSequencePoint sequencePoint)
		{
			if (sequencePoint.Line <= 0 || sequencePoint.EndLine <= 0 || sequencePoint.Column <= 0 || sequencePoint.EndColumn <= 0) {
				return false;
			} else if (sequencePoint.Line > document.LineCount) {
				return false;
			} else if (sequencePoint.EndLine > document.LineCount) {
				return false;
			} else if (sequencePoint.Line == sequencePoint.EndLine && sequencePoint.Column > sequencePoint.EndColumn) {
				return false;
			} else {
				// Check the columns exist on the line.
				IDocumentLine lineSegment = document.GetLine(sequencePoint.Line);
				if (sequencePoint.Column > lineSegment.Length) {
					return false;
				}
				IDocumentLine endLineSegment = document.GetLine(sequencePoint.EndLine);
				if (sequencePoint.EndColumn > endLineSegment.Length + 1) {
					return false;
				}
			}
			return true;
		}
	
		public static System.Drawing.Color GetSequencePointBackColor(CodeCoverageSequencePoint sequencePoint) {
			if (sequencePoint.VisitCount > 0) {
				if ( sequencePoint.BranchCoverage == true ) {
					return CodeCoverageOptions.VisitedColor;
				}
				return CodeCoverageOptions.PartVisitedColor;
			}
			return CodeCoverageOptions.NotVisitedColor;
		}
		
		public static System.Drawing.Color GetSequencePointForeColor(CodeCoverageSequencePoint sequencePoint) {
			if (sequencePoint.VisitCount > 0) {
				if ( sequencePoint.BranchCoverage == true ) {
					return CodeCoverageOptions.VisitedForeColor;
				}
				return CodeCoverageOptions.PartVisitedForeColor;
			}
			return CodeCoverageOptions.NotVisitedForeColor;
		}

	}
}
