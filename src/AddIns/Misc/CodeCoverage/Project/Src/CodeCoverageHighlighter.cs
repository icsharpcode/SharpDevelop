// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using ICSharpCode.TextEditor.Document;

namespace ICSharpCode.CodeCoverage
{
	/// <summary>
	/// Highlights the covered and not covered code in the text editor.
	/// </summary>
	public class CodeCoverageHighlighter
	{
		public CodeCoverageHighlighter()
		{
		}
		
		/// <summary>
		/// Adds text markers for the code coverage sequence points.
		/// </summary>
		/// <remarks>The sequence points are added to the marker strategy even 
		/// if they are not all for the same document.</remarks>
		public void AddMarkers(MarkerStrategy markerStrategy, List<CodeCoverageSequencePoint> sequencePoints)
		{
			foreach (CodeCoverageSequencePoint sequencePoint in sequencePoints) {
				AddMarker(markerStrategy, sequencePoint);
			}
		}
		
		public void AddMarker(MarkerStrategy markerStrategy, CodeCoverageSequencePoint sequencePoint)
		{
			if (!IsValidSequencePoint(markerStrategy.Document, sequencePoint)) {
				return;
			}
			
			if (sequencePoint.EndLine == sequencePoint.Line) {
				LineSegment lineSegment = markerStrategy.Document.GetLineSegment(sequencePoint.Line - 1);
				markerStrategy.AddMarker(new CodeCoverageTextMarker(lineSegment.Offset + sequencePoint.Column - 1, sequencePoint));
			} else {
				// Sequence point spread across lines.
				for (int line = sequencePoint.Line; line <= sequencePoint.EndLine; ++line) {
					LineSegment lineSegment = markerStrategy.Document.GetLineSegment(line - 1);
					if (line == sequencePoint.Line) {
						// First line.
						markerStrategy.AddMarker(new CodeCoverageTextMarker(lineSegment.Offset + sequencePoint.Column - 1, lineSegment.Length - (sequencePoint.Column - 1), sequencePoint));
					} else if (line == sequencePoint.EndLine) {
						// Last line.
						markerStrategy.AddMarker(new CodeCoverageTextMarker(lineSegment.Offset, sequencePoint.EndColumn - 1, sequencePoint));
					} else {
						markerStrategy.AddMarker(new CodeCoverageTextMarker(lineSegment.Offset, lineSegment.Length, sequencePoint));						
					}
				}
			}
		}
		
		/// <summary>
		/// Removes all CodeCoverageMarkers from the marker strategy.
		/// </summary>
		public void RemoveMarkers(MarkerStrategy markerStrategy)
		{
			markerStrategy.RemoveAll(IsCodeCoverageTextMarkerMatch);
		}
		
		bool IsCodeCoverageTextMarkerMatch(TextMarker marker)
		{
			if (marker is CodeCoverageTextMarker) {
				return true;
			}
			return false;
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
				LineSegment lineSegment = document.GetLineSegment(sequencePoint.Line - 1);
				if (sequencePoint.Column > lineSegment.Length) {
					return false;
				} 
				LineSegment endLineSegment = document.GetLineSegment(sequencePoint.EndLine - 1);
				if (sequencePoint.EndColumn > endLineSegment.Length + 1) {
					return false;
				} 
			}
			return true;
		}
	}
}
