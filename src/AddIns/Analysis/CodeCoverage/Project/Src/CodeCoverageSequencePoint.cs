// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Xml.Linq;

namespace ICSharpCode.CodeCoverage
{
	public class CodeCoverageSequencePoint
	{
		public CodeCoverageSequencePoint()
		{
		}
		
		public CodeCoverageSequencePoint(string document, int visitCount, int line, int column, int endLine, int endColumn)
			: this(document, visitCount, line, column, endLine, endColumn, 0)
		{
		}
		
		public CodeCoverageSequencePoint(string document, int visitCount, int line, int column, int endLine, int endColumn, int length)
		{
			this.Document = document;
			this.VisitCount = visitCount;
			this.Line = line;
			this.Column = column;
			this.EndLine = endLine;
			this.EndColumn = endColumn;
			this.Length = 1;
		}
		
		public CodeCoverageSequencePoint(string document, XElement reader)
		{
			this.Document = document;
			Read(reader);
		}

		void Read(XElement reader)
		{
			VisitCount = GetInteger(reader, "vc");
			Line = GetInteger(reader, "sl");
			Column = GetInteger(reader, "sc");
			EndLine = GetInteger(reader, "el");
			EndColumn = GetInteger(reader, "ec");
			Length = 1; // TODO: need to find a way to get this. GetInteger(reader, "len");
		}
		
		int GetInteger(XElement reader, string attributeName)
		{
			string attributeValue = reader.Attribute(attributeName).Value;
			return GetInteger(attributeValue);
		}
		
		int GetInteger(string text)
		{
			int val;
			if (Int32.TryParse(text, out val)) {
				return val;
			}
			return 0;
		}
		
		public bool HasDocument()
		{
			return !String.IsNullOrEmpty(Document);
		}
		
		public string Document { get; set; }
		public int VisitCount { get; set; }
		public int Line { get; set; }
		public int Column { get; set; }
		public int EndLine { get; set; }
		public int EndColumn { get; set; }
		public int Length { get; set; }
		
		public override bool Equals(object obj)
		{
			CodeCoverageSequencePoint rhs = obj as CodeCoverageSequencePoint;
			if (rhs != null) {
				return (Document == rhs.Document) &&
					(Column == rhs.Column) &&
					(EndColumn == rhs.EndColumn) &&
					(EndLine == rhs.EndLine) &&
					(Line == rhs.Line) &&
					(VisitCount == rhs.VisitCount) &&
					(Length == rhs.Length);
			}
			return false;
		}
		
		public override int GetHashCode()
		{
			return base.GetHashCode();
		}
		
		public override string ToString()
		{
			return String.Format("Document: '{0}' VisitCount: {1} Line: {2} Col: {3} EndLine: {4} EndCol: {5} Length: {6}",
				Document, VisitCount, Line, Column, EndLine, EndColumn, Length);
		}
	}
}
