// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using Location = ICSharpCode.NRefactory.Parser.Location;

namespace ICSharpCode.SharpDevelop.Dom
{
	[Serializable]
	public struct DomRegion : IComparable, IComparable<DomRegion>
	{
		readonly int beginLine;
		readonly int endLine;
		readonly int beginColumn;
		readonly int endColumn;
		
		public readonly static DomRegion Empty = new DomRegion(-1, -1);
		
		public bool IsEmpty {
			get {
				return BeginLine <= 0;
			}
		}
		
		public int BeginLine {
			get {
				return beginLine;
			}
		}
		
		/// <value>
		/// if the end line is == -1 the end column is -1 too
		/// this stands for an unknwon end
		/// </value>
		public int EndLine {
			get {
				return endLine;
			}
		}
		
		public int BeginColumn {
			get {
				return beginColumn;
			}
		}
		
		/// <value>
		/// if the end column is == -1 the end line is -1 too
		/// this stands for an unknown end
		/// </value>
		public int EndColumn {
			get {
				return endColumn;
			}
		}
		
		public DomRegion(Location start, Location end)
			: this(start.Y, start.X, end.Y, end.X)
		{
		}
		
		public DomRegion(int beginLine, int beginColumn, int endLine, int endColumn)
		{
			this.beginLine   = beginLine;
			this.beginColumn = beginColumn;
			this.endLine     = endLine;
			this.endColumn   = endColumn;
		}
		
		public DomRegion(int beginLine, int beginColumn)
		{
			this.beginLine   = beginLine;
			this.beginColumn = beginColumn;
			this.endLine = -1;
			this.endColumn = -1;
		}
		
		/// <remarks>
		/// Returns true, if the given coordinates (row, column) are in the region.
		/// This method assumes that for an unknown end the end line is == -1
		/// </remarks>
		public bool IsInside(int row, int column)
		{
			return row >= BeginLine &&
				(row <= EndLine   || EndLine == -1) &&
				(row != BeginLine || column >= BeginColumn) &&
				(row != EndLine   || column <= EndColumn);
		}

		public override string ToString()
		{
			return String.Format("[Region: BeginLine = {0}, EndLine = {1}, BeginColumn = {2}, EndColumn = {3}]",
			                     beginLine,
			                     endLine,
			                     beginColumn,
			                     endColumn);
		}
		
		public int CompareTo(DomRegion value)
		{
			int cmp;
			if (0 != (cmp = (BeginLine - value.BeginLine))) {
				return cmp;
			}
			
			if (0 != (cmp = (BeginColumn - value.BeginColumn))) {
				return cmp;
			}
			
			if (0 != (cmp = (EndLine - value.EndLine))) {
				return cmp;
			}
			
			return EndColumn - value.EndColumn;
		}
		
		int IComparable.CompareTo(object value) {
			return CompareTo((DomRegion)value);
		}
	}
}
