// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

using System;

using Debugger.Wrappers.CorDebug;
using Debugger.Wrappers.CorSym;

namespace Debugger
{
	[Serializable]
	public class SourcecodeSegment
	{
		string moduleFilename;
		string sourceFullFilename;
		int startLine;
		int startColumn;
		int endLine;
		int endColumn;
		int ilOffset;
		int ilStart;
		int ilEnd;
		int[] stepRanges;

		internal SourcecodeSegment()
		{

		}

		public SourcecodeSegment(string sourceFilename, int line)
		{
			this.sourceFullFilename = sourceFilename;
			this.startLine = line;
			this.endLine = line;
		}

		public SourcecodeSegment(string sourceFilename, int line, int startColumn, int endColumn)
		{
			this.sourceFullFilename = sourceFilename;
			this.startLine = line;
			this.endLine = line;
			this.startColumn = startColumn;
			this.endColumn = endColumn;
		}

		public SourcecodeSegment(string sourceFilename, int startLine, int endLine, int startColumn, int endColumn)
		{
			this.sourceFullFilename = sourceFilename;
			this.startLine = startLine;
			this.endLine = endLine;
			this.startColumn = startColumn;
			this.endColumn = endColumn;
		}
		
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

		public int[] StepRanges {
			get {
				return stepRanges;
			}
			set {
				stepRanges = value;
			}
		}

		public int ILOffset {
			get { 
				return ilOffset;
			}
			set {
				ilOffset = value;
			}
		}

		public int ILStart {
			get { 
				return ilStart;
			}
			set {
				ilStart = value;
			}
		}

		public int ILEnd {
			get { 
				return ilEnd;
			}
			set {
				ilEnd = value;
			}
		}
		
		// Returns true if found
		internal bool GetFunctionAndOffset(NDebugger debugger, bool normailize, out ICorDebugFunction function, out int ilOffset)
		{
			function = null;
			ilOffset = 0;
			
			Module                 module     = null;
			ISymUnmanagedReader    symReader  = null;
			ISymUnmanagedDocument  symDoc     = null;
			
			// Try to get doc from moduleFilename
			if (moduleFilename != null) {
				try {
					module = debugger.GetModule(ModuleFilename);
					symReader = module.SymReader;
					symDoc = symReader.GetDocument(SourceFullFilename,Guid.Empty,Guid.Empty,Guid.Empty);
				} catch {}
			}

			// search all modules
			if (symDoc == null) {
				foreach (Module m in debugger.Modules) {
					module    = m;
					symReader = m.SymReader;
					if (symReader == null) {
						continue;
					}

					symDoc = symReader.GetDocument(SourceFullFilename,Guid.Empty,Guid.Empty,Guid.Empty);

					if (symDoc != null) {
						break;
					}
				}
			}

			if (symDoc == null) {
				return false; //Not found
			}
			
			uint validLine;
			try {
				validLine = symDoc.FindClosestLine((uint)StartLine);
			} catch {
				return false; //Not found
			}
			if (validLine != StartLine && normailize) {
				StartLine = (int)validLine;
				EndLine = (int)validLine;
				StartColumn = 0;
				EndColumn = 0;
			}
			
			ISymUnmanagedMethod symMethod;
			try {
				symMethod = symReader.GetMethodFromDocumentPosition(symDoc, validLine, 0);
			} catch {
				return false; //Not found
			}
			
			// Check that StartLine is within the method
			uint start = uint.MaxValue;
			uint end = uint.MinValue;
			foreach(SequencePoint sqPoint in symMethod.SequencePoints) {
				if (sqPoint.Line == 0xFEEFEE) continue;
				start = Math.Min(start, sqPoint.Line);
				end = Math.Max(end, sqPoint.EndLine);
			}
			if (StartLine < start || StartLine > end) return false;
			
			function = module.CorModule.GetFunctionFromToken(symMethod.Token);
			ilOffset = (int)symMethod.GetOffset(symDoc, validLine, 0);
			return true;
		}
	}
}
