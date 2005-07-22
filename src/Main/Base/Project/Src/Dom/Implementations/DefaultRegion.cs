// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Drawing;
using System.Diagnostics;

namespace ICSharpCode.SharpDevelop.Dom {
	
	[Serializable]
	public class DefaultRegion : System.MarshalByRefObject, IRegion
	{
		protected int beginLine = -1;
		protected int endLine = -1;
		protected int beginColumn = -1;
		protected int endColumn = -1;

		public virtual int BeginLine {
			get {
				return beginLine;
			}
		}

		public virtual int BeginColumn {
			get {
				return beginColumn;
			}
		}

		/// <value>
		/// if the end column is == -1 the end line is -1 too
		/// this stands for an unknwon end
		/// </value>
		public virtual int EndColumn {
			get {
				return endColumn;
			}
			set {
				endColumn = value;
			}
		}

		/// <value>
		/// if the end line is == -1 the end column is -1 too
		/// this stands for an unknwon end
		/// </value>
		public virtual int EndLine {
			get {
				return endLine;
			}
			set {
				endLine = value;
			}
		}

		public DefaultRegion(Point start, Point end)
			: this(start.Y, start.X, end.Y, end.X)
		{
		}
		
		public DefaultRegion(Point start)
			: this(start.Y, start.X)
		{
		}
		
		public DefaultRegion(int beginLine, int beginColumn, int endLine, int endColumn)
		{
			this.beginLine   = beginLine;
			this.beginColumn = beginColumn;
			this.endLine     = endLine;
			this.endColumn   = endColumn;
		}
		
		public DefaultRegion(int beginLine, int beginColumn)
		{
			this.beginLine   = beginLine;
			this.beginColumn = beginColumn;
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
		
		public virtual int CompareTo(IRegion value)
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
			return CompareTo((IRegion)value);
		}
	}
}
