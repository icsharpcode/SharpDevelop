// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;

namespace ICSharpCode.CodeCoverage
{
	public class CodeCoverageSequencePoint
	{
		string document = String.Empty;
		int visitCount = 0;
		int line = 0;
		int column = 0;		
		int endLine = 0;		
		int endColumn = 0;	
		bool excluded = false;
		
		public CodeCoverageSequencePoint(string document, int visitCount, int line, int column, int endLine, int endColumn) : this(document, visitCount, line, column, endLine, endColumn, false)
		{
		}

		public CodeCoverageSequencePoint(string document, int visitCount, int line, int column, int endLine, int endColumn, bool excluded)
		{
			this.document = document;
			this.visitCount = visitCount;
			this.line = line;
			this.column = column;
			this.endLine = endLine;
			this.endColumn = endColumn;
			this.excluded = excluded;
		}
		
		public bool IsExcluded {
			get {
				return excluded;
			}
		}
		
		public string Document {
			get {
				return document;
			}
		}
		
		public int VisitCount {
			get {
				return visitCount;
			}
		}
		
		public int Line {
			get {
				return line;
			}
		}
		
		public int Column {
			get {
				return column;
			}
		}
		
		public int EndLine {
			get {
				return endLine;
			}
		}
		
		public int EndColumn {
			get {
				return endColumn;
			}
		}
	}
}
