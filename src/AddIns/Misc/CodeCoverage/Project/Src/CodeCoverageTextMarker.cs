// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Drawing;
using ICSharpCode.TextEditor.Document;

namespace ICSharpCode.CodeCoverage
{
	/// <summary>
	/// Custom text marker used when highlighting code coverage lines.
	/// </summary>
	public class CodeCoverageTextMarker : TextMarker
	{		
		public CodeCoverageTextMarker(int offset, CodeCoverageSequencePoint sequencePoint) : this(offset, GetSequencePointLength(sequencePoint), sequencePoint)
		{
		}
		
		public CodeCoverageTextMarker(int offset, int length, CodeCoverageSequencePoint sequencePoint) : base(offset, length, TextMarkerType.SolidBlock, GetSequencePointColor(sequencePoint), GetSequencePointForeColor(sequencePoint))
		{
		}

		public static int GetSequencePointLength(CodeCoverageSequencePoint sequencePoint)
		{
			return sequencePoint.EndColumn - sequencePoint.Column;
		}
		
		public static Color GetSequencePointColor(CodeCoverageSequencePoint sequencePoint)
		{
			if (sequencePoint.VisitCount > 0) {
				return CodeCoverageOptions.VisitedColor;
			}
			return CodeCoverageOptions.NotVisitedColor;
		}
		
		public static Color GetSequencePointForeColor(CodeCoverageSequencePoint sequencePoint)
		{
			if (sequencePoint.VisitCount > 0) {
				return CodeCoverageOptions.VisitedForeColor;
			}
			return CodeCoverageOptions.NotVisitedForeColor;
		}

	}
}
