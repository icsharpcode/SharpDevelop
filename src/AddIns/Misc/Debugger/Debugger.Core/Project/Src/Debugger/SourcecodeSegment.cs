// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;

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
		internal bool GetFunctionAndOffset(Module module, bool normailize, out ICorDebugFunction function, out int ilOffset)
		{
			function = null;
			ilOffset = 0;
			
			ISymUnmanagedReader symReader = module.SymReader;
			if (symReader == null) {
				return false; // No symbols
			}
			
			// Do not use ISymUnmanagedReader.GetDocument!  It is broken if two files have the same name
			
			ISymUnmanagedDocument[] symDocs = module.SymDocuments;
			ISymUnmanagedDocument symDoc = null;
			foreach(ISymUnmanagedDocument d in symDocs) {
				if (d.URL.ToLower() == this.SourceFullFilename.ToLower()) {
					symDoc = d;
				}
			}
			if (symDoc == null) {
				return false; // Does not use source file
			}
			
			uint validLine;
			try {
				validLine = symDoc.FindClosestLine((uint)StartLine);
			} catch {
				return false; // Not on a vaild point
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
			
			// Do not use ISymUnmanagedMethod.GetOffset!  It sometimes returns negative offset
			
			SequencePoint[] seqPoints = symMethod.SequencePoints;
			Array.Sort(seqPoints);
			if (seqPoints.Length == 0) return false;
			if (this.StartLine < seqPoints[0].Line) return false;
			foreach(SequencePoint sqPoint in seqPoints) {
				if (sqPoint.Line == 0xFEEFEE) continue;
				// If the desired breakpoint position is before the end of the sequence point
				if (this.StartLine < sqPoint.EndLine || (this.StartLine == sqPoint.EndLine && this.StartColumn < sqPoint.EndColumn)) {
					function = module.CorModule.GetFunctionFromToken(symMethod.Token);
					ilOffset = (int)sqPoint.Offset;
					startLine = (int)sqPoint.Line;
					startColumn = (int)sqPoint.Column;
					endLine = (int)sqPoint.EndLine;
					endColumn = (int)sqPoint.EndColumn;
					return true;
				}
			}
			return false;
		}
		
		public override string ToString()
		{
			return string.Format("Start={0},{1} End={2},{3}", this.startLine, this.startColumn, this.endLine, this.endColumn);
		}
	}
}
