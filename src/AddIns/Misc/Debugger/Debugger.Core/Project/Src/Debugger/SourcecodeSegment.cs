// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using System.IO;

using Debugger.Wrappers.CorDebug;
using Debugger.Wrappers.CorSym;
using System.Runtime.InteropServices;

namespace Debugger
{
	public class SourcecodeSegment: DebuggerObject
	{
		Module module;
		
		string filename;
		byte[] checkSum;
		int startLine;
		int startColumn;
		int endLine;
		int endColumn;
		
		ICorDebugFunction corFunction;
		int ilStart;
		int ilEnd;
		int[] stepRanges;
		
		public Module Module {
			get { return module; }
		}
		
		public string Filename {
			get { return filename; }
		}
		
		public byte[] CheckSum {
			get { return checkSum; }
		}
		
		public int StartLine {
			get { return startLine; }
		}
		
		public int StartColumn {
			get { return startColumn; }
		}
		
		public int EndLine {
			get { return endLine; }
		}
		
		public int EndColumn {
			get { return endColumn; }
		}
		
		internal ICorDebugFunction CorFunction {
			get { return corFunction; }
		}
		
		public int ILStart {
			get { return ilStart; }
		}
		
		public int ILEnd {
			get { return ilEnd; }
		}
		
		public int[] StepRanges {
			get { return stepRanges; }
		}
		
		private SourcecodeSegment()
		{
		}
		
		static ISymUnmanagedDocument GetSymDocumentFromFilename(Module module, string filename, byte[] checksum)
		{
			if (filename == null) throw new ArgumentNullException("filename");
			filename = filename.ToLower();
			
			ISymUnmanagedDocument[] symDocs = module.SymDocuments;
			
			// "c:\project\file.cs" N/A
			if (Path.IsPathRooted(filename) && checksum == null) {
				foreach(ISymUnmanagedDocument symDoc in symDocs) {
					if (symDoc.URL.ToLower() == filename) return symDoc;
				}
				if (module.IsDynamic) {
					foreach(ISymUnmanagedDocument symDoc in symDocs) {
						string url = symDoc.URL.ToLower();
						if (!String.IsNullOrEmpty(url) && !Path.IsPathRooted(url)) {
							string workingDir = Path.GetFullPath(module.Process.WorkingDirectory).ToLower();
							url = Path.GetFullPath(Path.Combine(workingDir, url));
						}
						if (url == filename) return symDoc;
					}
				}
				return null; // Not found
			}
			
			// "c:\project\file.cs" 0123456789
			if (Path.IsPathRooted(filename) && checksum != null) {
				foreach(ISymUnmanagedDocument symDoc in symDocs) {
					if (symDoc.URL.ToLower() == filename) return symDoc;
				}
				// Not found - try to find using checksum
				filename = Path.GetFileName(filename);
			}
			
			// "file.cs" N/A
			if (!Path.IsPathRooted(filename) && checksum == null) {
				if (!filename.StartsWith(@"\")) {
					filename = @"\" + filename;
				}
				foreach(ISymUnmanagedDocument symDoc in symDocs) {
					if (symDoc.URL.ToLower().EndsWith(filename)) return symDoc;
				}
				return null; // Not found
			}
			
			// "file.cs" 0123456789
			if (!Path.IsPathRooted(filename) && checksum != null) {
				if (!filename.StartsWith(@"\")) {
					filename = @"\" + filename;
				}
				foreach(ISymUnmanagedDocument symDoc in symDocs) {
					if (!symDoc.URL.ToLower().EndsWith(filename)) continue;
					byte[] symDocCheckSum = symDoc.CheckSum;
					if (symDocCheckSum.Length != checksum.Length) continue;
					for (int i = 0; i < checksum.Length; i++) {
						if (symDocCheckSum[i] != checksum[i]) continue;
					}
					return symDoc;
				}
				return null; // Not found
			}
			
			return null;
		}
		
		public static SourcecodeSegment Resolve(Module module, string fileName, byte[] checkSum, int line, int column)
		{
			// Do not use ISymUnmanagedReader.GetDocument!  It is broken if two files have the same name
			// Do not use ISymUnmanagedMethod.GetOffset!  It sometimes returns negative offset
			
			ISymUnmanagedReader symReader = module.SymReader;
			if (symReader == null) return null; // No symbols
			
			ISymUnmanagedDocument symDoc = GetSymDocumentFromFilename(module, fileName, checkSum);
			if (symDoc == null) return null; // Document not found
			
			ISymUnmanagedMethod symMethod;
			try {
				uint validLine = symDoc.FindClosestLine((uint)line);
				symMethod = symReader.GetMethodFromDocumentPosition(symDoc, (uint)validLine, (uint)column);
			} catch {
				return null; //Not found
			}
			
			SequencePoint[] seqPoints = symMethod.SequencePoints;
			Array.Sort(seqPoints);
			if (seqPoints.Length == 0) return null;
			if (line < seqPoints[0].Line) return null;
			foreach(SequencePoint sqPoint in seqPoints) {
				if (sqPoint.Line == 0xFEEFEE) continue;
				// If the desired breakpoint position is before the end of the sequence point
				if (line < sqPoint.EndLine || (line == sqPoint.EndLine && column < sqPoint.EndColumn)) {
					SourcecodeSegment segment = new SourcecodeSegment();
					segment.module        = module;
					segment.filename      = symDoc.URL;
					segment.checkSum      = symDoc.CheckSum;
					segment.startLine     = (int)sqPoint.Line;
					segment.startColumn   = (int)sqPoint.Column;
					segment.endLine       = (int)sqPoint.EndLine;
					segment.endColumn     = (int)sqPoint.EndColumn;
					segment.corFunction   = module.CorModule.GetFunctionFromToken(symMethod.Token);
					segment.ilStart = (int)sqPoint.Offset;
					segment.ilEnd   = (int)sqPoint.Offset;
					segment.stepRanges    = null;
					return segment;
				}
			}
			return null;
		}
		
		static string GetFilenameFromSymDocument(Module module, ISymUnmanagedDocument symDoc)
		{
			if (File.Exists(symDoc.URL)) return symDoc.URL;
			
			List<string> searchPaths = new List<string>();
			
			searchPaths.AddRange(module.Process.Options.SymbolsSearchPaths);
			
			string modulePath = module.FullPath;
			while (true) {
				// Get parent directory
				int index = modulePath.LastIndexOf('\\');
				if (index == -1) break;
				modulePath = modulePath.Substring(0, index);
				// Add the directory to search path list
				if (modulePath.Length == 2 && modulePath[1] == ':') {
					searchPaths.Add(modulePath + '\\');
				} else {
					searchPaths.Add(modulePath);
				}
			}
			
			List<string> filenames = new List<string>();
			string filename = symDoc.URL;
			while (true) {
				// Remove start of the path
				int index = filename.IndexOf('\\');
				if (index == -1) break;
				filename = filename.Substring(index + 1);
				// Add the filename as candidate
				filenames.Add(filename);
			}
			
			List<string> candidates = new List<string>();
			foreach(string path in searchPaths) {
				foreach(string name in filenames) {
					candidates.Add(Path.Combine(path, name));
				}
			}
			
			foreach(string candiate in candidates) {
				if (File.Exists(candiate)) {
					return candiate;
				}
			}
			
			return symDoc.URL;
		}
		
		/// <summary>
		/// 'ILStart &lt;= ILOffset &lt;= ILEnd' and this range includes at least
		/// the returned area of source code. (May incude some extra compiler generated IL too)
		/// </summary>
		internal static SourcecodeSegment Resolve(Module module, ICorDebugFunction corFunction, uint offset)
		{
			ISymUnmanagedReader symReader = module.SymReader;
			if (symReader == null) return null; // No symbols
			
			ISymUnmanagedMethod symMethod;
			try {
				symMethod = symReader.GetMethod(corFunction.Token);
			} catch (COMException) {
				// Can not find the method
				// eg. Compiler generated constructors are not in symbol store
				return null;
			}
			if (symMethod == null) return null;
			
			uint sequencePointCount = symMethod.SequencePointCount;
			SequencePoint[] sequencePoints = symMethod.SequencePoints;
			
			// Get i for which: offsets[i] <= offset < offsets[i + 1]
			// or fallback to first element if  offset < offsets[0]
			for (int i = (int)sequencePointCount - 1; i >= 0; i--) { // backwards
				if (sequencePoints[i].Offset <= offset || i == 0) {
					// Set inforamtion about current IL range
					int codeSize = (int)corFunction.ILCode.Size;
					
					int ilStart = (int)sequencePoints[i].Offset;
					int ilEnd = (i + 1 < sequencePointCount) ? (int)sequencePoints[i+1].Offset : codeSize;
					
					// 0xFeeFee means "code generated by compiler"
					// If we are in generated sequence use to closest real one instead,
					// extend the ILStart and ILEnd to include the 'real' sequence
					
					// Look ahead for 'real' sequence
					while (i + 1 < sequencePointCount && sequencePoints[i].Line == 0xFeeFee) {
						i++;
						ilEnd = (i + 1 < sequencePointCount) ? (int)sequencePoints[i+1].Offset : codeSize;
					}
					// Look back for 'real' sequence
					while (i - 1 >= 0 && sequencePoints[i].Line == 0xFeeFee) {
						i--;
						ilStart = (int)sequencePoints[i].Offset;
					}
					// Wow, there are no 'real' sequences
					if (sequencePoints[i].Line == 0xFeeFee) {
						return null;
					}
					
					List<int> stepRanges = new List<int>();
					for (int j = 0; j < sequencePointCount; j++) {
						// Step over compiler generated sequences and current statement
						// 0xFeeFee means "code generated by compiler"
						if (sequencePoints[j].Line == 0xFeeFee || j == i) {
							// Add start offset or remove last end (to connect two ranges into one)
							if (stepRanges.Count > 0 && stepRanges[stepRanges.Count - 1] == sequencePoints[j].Offset) {
								stepRanges.RemoveAt(stepRanges.Count - 1);
							} else {
								stepRanges.Add((int)sequencePoints[j].Offset);
							}
							// Add end offset | handle last sequence point
							if (j + 1 < sequencePointCount) {
								stepRanges.Add((int)sequencePoints[j+1].Offset);
							} else {
								stepRanges.Add(codeSize);
							}
						}
					}
					
					SourcecodeSegment segment = new SourcecodeSegment();
					segment.module        = module;
					segment.filename      = GetFilenameFromSymDocument(module, sequencePoints[i].Document);
					segment.checkSum      = sequencePoints[i].Document.CheckSum;
					segment.startLine     = (int)sequencePoints[i].Line;
					segment.startColumn   = (int)sequencePoints[i].Column;
					segment.endLine       = (int)sequencePoints[i].EndLine;
					segment.endColumn     = (int)sequencePoints[i].EndColumn;
					segment.corFunction   = corFunction;
					segment.ilStart       = ilStart;
					segment.ilEnd         = ilEnd;
					segment.stepRanges    = stepRanges.ToArray();
					
					// VB.NET sometimes produces temporary files which it then deletes
					// (eg 17d14f5c-a337-4978-8281-53493378c1071.vb)
					string filename = Path.GetFileName(segment.filename);
					if (filename.Length == 40 && filename.EndsWith(".vb")) {
						bool guidName = true;
						foreach(char c in filename.Substring(0, filename.Length - 3)) {
							if (('0' <= c && c <= '9') ||
							    ('a' <= c && c <= 'f') ||
							    ('A' <= c && c <= 'F') ||
							    (c == '-'))
							{
								guidName = true;
							} else {
								guidName = false;
								break;
							}
						}
						if (guidName)
							return null;
					}
					
					return segment;
				}
			}
			return null;
		}
		
		public override string ToString()
		{
			return string.Format("{0}:{1},{2}-{3},{4}", Path.GetFileName(this.Filename), this.startLine, this.startColumn, this.endLine, this.endColumn);
		}
	}
}
