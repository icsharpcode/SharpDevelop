// <file>
//     <owner name="David Srbecký" email="dsrbecky@post.cz"/>
// </file>

using System;

using DebuggerInterop.Symbols;

namespace DebuggerLibrary
{
	public class SourcecodeSegment
	{
		string moduleFilename;
		string sourceFilename;
		uint startLine;
		uint startColumn;
		uint endLine;
		uint endColumn;
		ISymUnmanagedDocument symUnmanagedDocument;
		
		public string ModuleFilename {
			get {
				return moduleFilename;
			}
			set {
				moduleFilename = value;
			}
		}
		
		public string SourceFilename {
			get {
				return sourceFilename;
			}
			set {
				sourceFilename = value;
			}
		}
		
		public uint StartLine {
			get {
				return startLine;
			}
			set {
				startLine = value;
			}
		}
		
		public uint StartColumn {
			get {
				return startColumn;
			}
			set {
				startColumn = value;
			}
		}
		
		public uint EndLine {
			get {
				return endLine;
			}
			set {
				endLine = value;
			}
		}
		
		public uint EndColumn {
			get {
				return endColumn;
			}
			set {
				endColumn = value;
			}
		}
		
		public ISymUnmanagedDocument SymUnmanagedDocument {
			get {
				return symUnmanagedDocument;
			}
			set {
				symUnmanagedDocument = value;
			}
		}
	}
}
