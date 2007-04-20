// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;

namespace ICSharpCode.TextEditor.Document
{
	public delegate void LineManagerEventHandler(object sender, LineManagerEventArgs e);
	public delegate void LineLengthEventHandler(object sender, LineLengthEventArgs e);
	
	public class LineManagerEventArgs : EventArgs
	{
		IDocument document;
		int       start;
		int       moved;
		
		/// <returns>
		/// always a valid Document which is related to the Event.
		/// </returns>
		public IDocument Document {
			get {
				return document;
			}
		}
		
		/// <returns>
		/// -1 if no offset was specified for this event
		/// </returns>
		public int LineStart {
			get {
				return start;
			}
		}
		
		/// <returns>
		/// -1 if no length was specified for this event
		/// </returns>
		public int LinesMoved {
			get {
				return moved;
			}
		}
		
		public LineManagerEventArgs(IDocument document, int lineStart, int linesMoved)
		{
			this.document = document;
			this.start    = lineStart;
			this.moved    = linesMoved;
		}
	}
	
	public class LineLengthEventArgs : EventArgs
	{
		IDocument document;
		LineSegment lineSegment;
		int lengthDelta;
		
		public IDocument Document {
			get { return document; }
		}
		
		public LineSegment LineSegment {
			get { return lineSegment; }
		}
		
		public int LengthDelta {
			get { return lengthDelta; }
		}
		
		public LineLengthEventArgs(IDocument document, LineSegment lineSegment, int moved)
		{
			this.document = document;
			this.lineSegment = lineSegment;
			this.lengthDelta = moved;
		}
		
		public override string ToString()
		{
			return string.Format("[LineLengthEventArgs Document={0} LineSegment={1} LengthDelta={2}]", this.document, this.lineSegment, this.lengthDelta);
		}
	}
}
