// <file>
//     <owner name="David Srbecký" email="dsrbecky@post.cz"/>
// </file>

using System;
using System.Diagnostics.SymbolStore;

namespace DebuggerLibrary
{
	public class SourcecodeSegment
	{
		string moduleFilename;
		string sourceFullFilename;
		int startLine;
		int startColumn;
		int endLine;
		int endColumn;
		ISymbolDocument symUnmanagedDocument;
		
		public string ModuleFilename {
			get {
				return moduleFilename;
			}
			set {
				moduleFilename = value;
			}
		}
		
		public string SourceFullFilename {
			get {
				return sourceFullFilename;
			}
			set {
				sourceFullFilename = value;
			}
		}

		public string SourceFilename {
			get {
				return System.IO.Path.GetFileName(sourceFullFilename);
			}
		}
		
		public int StartLine {
			get {
				return startLine;
			}
			set {
				startLine = value;
			}
		}
		
		public int StartColumn {
			get {
				return startColumn;
			}
			set {
				startColumn = value;
			}
		}
		
		public int EndLine {
			get {
				return endLine;
			}
			set {
				endLine = value;
			}
		}
		
		public int EndColumn {
			get {
				return endColumn;
			}
			set {
				endColumn = value;
			}
		}
		
		public ISymbolDocument SymbolDocument {
			get {
				return symUnmanagedDocument;
			}
			set {
				symUnmanagedDocument = value;
			}
		}
	}
}
