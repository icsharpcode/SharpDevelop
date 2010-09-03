// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Windows.Media;

using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Editor;

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
				marker.BackgroundColor = GetSequencePointColor(sequencePoint);
				marker.ForegroundColor = GetSequencePointForeColor(sequencePoint);
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
			} else if (sequencePoint.Line > document.TotalNumberOfLines) {
				return false;
			} else if (sequencePoint.EndLine > document.TotalNumberOfLines) {
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
		
		public static Color GetSequencePointColor(CodeCoverageSequencePoint sequencePoint)
		{
			if (sequencePoint.VisitCount > 0) {
				return CodeCoverageOptions.VisitedColor.ToWpf();
			}
			return CodeCoverageOptions.NotVisitedColor.ToWpf();
		}
		
		public static Color GetSequencePointForeColor(CodeCoverageSequencePoint sequencePoint)
		{
			if (sequencePoint.VisitCount > 0) {
				return CodeCoverageOptions.VisitedForeColor.ToWpf();
			}
			return CodeCoverageOptions.NotVisitedForeColor.ToWpf();
		}
	}
}
