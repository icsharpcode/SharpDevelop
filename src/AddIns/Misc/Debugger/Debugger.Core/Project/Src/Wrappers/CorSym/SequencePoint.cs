// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

namespace Debugger.Wrappers.CorSym
{
	using System;
	
	
	public class SequencePoint
	{
		ISymUnmanagedDocument document;
		uint offset;
		uint line;
		uint column;
		uint endLine;
		uint endColumn;
		
		public ISymUnmanagedDocument Document {
			get {
				return document;
			}
		}
		
		public uint Offset {
			get {
				return offset;
			}
		}
		
		public uint Line {
			get {
				return line;
			}
		}
		
		public uint Column {
			get {
				return column;
			}
		}
		
		public uint EndLine {
			get {
				return endLine;
			}
		}
		
		public uint EndColumn {
			get {
				return endColumn;
			}
		}
		
		public SequencePoint(ISymUnmanagedDocument document, uint offset, uint line, uint column, uint endLine, uint endColumn)
		{
			this.document = document;
			this.offset = offset;
			this.line = line;
			this.column = column;
			this.endLine = endLine;
			this.endColumn = endColumn;
		}
	}
}
