// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;

namespace ICSharpCode.TextEditor.Document
{
	/// <summary>
	/// Description of TextAnchor.
	/// </summary>
	public sealed class TextAnchor
	{
		static Exception AnchorDeletedError()
		{
			return new InvalidOperationException("The text containing the anchor was deleted");
		}
		
		LineSegment lineSegment;
		int columnNumber;
		
		public LineSegment Line {
			get {
				if (lineSegment == null) throw AnchorDeletedError();
				return lineSegment;
			}
			internal set {
				lineSegment = value;
			}
		}
		
		public bool IsDeleted {
			get {
				return lineSegment == null;
			}
		}
		
		public int LineNumber {
			get {
				return this.Line.LineNumber;
			}
		}
		
		public int ColumnNumber {
			get {
				if (lineSegment == null) throw AnchorDeletedError();
				return columnNumber;
			}
			internal set {
				columnNumber = value;
			}
		}
		
		public TextLocation Location {
			get {
				return new TextLocation(this.ColumnNumber, this.LineNumber);
			}
		}
		
		public int Offset {
			get {
				return this.Line.Offset + columnNumber;
			}
		}
		
		internal void Deleted()
		{
			lineSegment = null;
		}
		
		internal TextAnchor(LineSegment lineSegment, int columnNumber)
		{
			this.lineSegment = lineSegment;
			this.columnNumber = columnNumber;
		}
		
		public override string ToString()
		{
			if (this.IsDeleted)
				return "[TextAnchor (deleted)]";
			else
				return "[TextAnchor " + this.Location.ToString() + "]";
		}
	}
}
