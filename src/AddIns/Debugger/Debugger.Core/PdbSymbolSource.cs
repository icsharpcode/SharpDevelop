// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using Debugger;
using Debugger.Interop.CorDebug;
using Debugger.Interop.CorSym;
using ICSharpCode.NRefactory.TypeSystem;

namespace Debugger
{
	public class SequencePoint
	{
		public uint MethodDefToken { get; set; }
		public ILRange[] ILRanges { get; set; }
		public int ILOffset { get { return ILRanges.Length > 0 ? ILRanges[0].From : 0; } }
		
		public string Filename { get; set; }
		public byte[] FileCheckSum { get; set; }
		public int StartLine { get; set; }
		public int StartColumn { get; set; }
		public int EndLine { get; set; }
		public int EndColumn { get; set; }
		
		public override string ToString()
		{
			return string.Format("{0}:{1},{2}-{3},{4}",
			                     Path.GetFileName(this.Filename ?? string.Empty),
			                     this.StartLine, this.StartColumn,
			                     this.EndLine, this.EndColumn);
		}
		
		public bool ContainsLocation(int line, int column = 0)
		{
			if (column == 0)
				return line >= StartLine && line <= EndLine;
			return (StartLine < line || (StartLine == line && StartColumn <= column))
				&& (line < EndLine || (line == EndLine && column <= EndColumn));
		}
	}
	
	public struct ILRange
	{
		public int From;
		public int To;
		
		public ILRange(int from, int to)
		{
			this.From = from;
			this.To = to;
		}
		
		public override string ToString()
		{
			return string.Format("{0:X2}-{1:X2}", From, To);
		}
	}
	
	public class ILLocalVariable
	{
		public int Index { get; set; }
		public IType Type { get; set; }
		public string Name { get; set; }
		public bool IsCompilerGenerated { get; set; }
		public ILRange[] ILRanges { get; set; }
	}
	
	public interface ISymbolSource
	{
		/// <summary> Use this symbol store handle the method </summary>
		bool Handles(IMethod method);
		
		/// <summary> Does this method correspond to any source code? </summary>
		bool IsCompilerGenerated(IMethod method);
		
		/// <summary> Find sequence point by IL offset </summary>
		SequencePoint GetSequencePoint(IMethod method, int iloffset);
		
		/// <summary> Find sequence points by source code location. Might find multiple methods at one location (lambda expressions, etc.) </summary>
		/// <remarks> Only source files corresponding to the given module are searched </remarks>
		IEnumerable<SequencePoint> GetSequencePoints(Module module, string filename, int line, int column);
		
		/// <summary> Get IL ranges that should be always stepped over by the debugger </summary>
		/// <remarks> This is used for compiler generated code </remarks>
		IEnumerable<ILRange> GetIgnoredILRanges(IMethod method);
		
		/// <summary> Get local variable metadata </summary>
		IEnumerable<ILLocalVariable> GetLocalVariables(IMethod method);
	}
	
	public class PdbSymbolSource : ISymbolSource
	{
		public bool Handles(IMethod method)
		{
			return method.ParentAssembly.GetModule().HasSymbols
				&& !IsCompilerGenerated(method)
				&& GetSequencePoint(method, 0) != null;
		}
		
		public bool IsCompilerGenerated(IMethod method)
		{
			return method.GetSymMethod() == null;
		}
		
		/// <summary>
		/// Get absolute source code path for the specified document.
		/// If the file is not found, some effort will be made to locate it.
		/// (i.e. to handle case when the directory is moved after compilation)
		/// One document maps exactly to one disk file.
		/// </summary>
		/// <remarks>
		/// Be careful to use this in the same way in both mapping directinons so that
		/// setting breakpoints (src->IL) and stepping (IL->src) work consistently.
		/// </remarks>
		static string GetSourceCodePath(Process process, string origPath)
		{
			var paths = RelocatePath(process.Filename, origPath).Distinct();
			var path = paths.FirstOrDefault(f => File.Exists(f));
			return path;
		}
		
		/// <summary>
		/// Suggest posible new locations for original path.
		/// </summary>
		static IEnumerable<string> RelocatePath(string basePath, string origPath)
		{
			if (!string.IsNullOrEmpty(origPath)) {
				if (Path.IsPathRooted(origPath)) {
					// Try without relocating
					yield return origPath;
					
					string[] baseParts = basePath.Split('\\');
					string[] origParts = origPath.Split('\\');
					
					// Join the paths at some point (joining directry must match)
					for (int i = 0; i < baseParts.Length; i++) {
						for (int j = 0; j < origParts.Length; j++) {
							if (!string.IsNullOrEmpty(baseParts[i]) && string.Equals(baseParts[i], origParts[j], StringComparison.OrdinalIgnoreCase)) {
								// Join the paths
								string[] joinedDirs = new string[i + (origParts.Length - j)];
								Array.Copy(baseParts, joinedDirs, i);
								Array.Copy(origParts, j, joinedDirs, i, origParts.Length - j);
								string joined = string.Join(@"\", joinedDirs);
								yield return joined;
							}
						}
					}
				} else {
					if (origPath.StartsWith(@".\"))
						origPath = origPath.Substring(2);
					if (origPath.StartsWith(@"\"))
						origPath = origPath.Substring(1);
					
					// Try 0, 1 and 2 levels above the base path
					string dir = basePath;
					for(int i = 0; i <= 2; i++) {
						dir = Path.GetDirectoryName(dir);
						if (!string.IsNullOrEmpty(dir))
							yield return Path.Combine(dir, origPath);
					}
				}
			}
		}
		
		public SequencePoint GetSequencePoint(IMethod method, int iloffset)
		{
			var symMethod = method.GetSymMethod();
			if (symMethod == null)
				return null;
			
			// 0xFEEFEE means "code generated by compiler"
			int codeSize = (int)method.ToCorFunction().GetILCode().GetSize();
			var sequencePoints = symMethod.GetSequencePoints(codeSize);
			var realSeqPoints = sequencePoints.Where(p => p.StartLine != 0xFEEFEE);
			
			// Find point for which (ilstart <= iloffset < ilend) or fallback to the next valid sequence point
			var sequencePoint = realSeqPoints.FirstOrDefault(p => p.ILRanges.Any(r => r.From <= iloffset && iloffset < r.To))
				?? realSeqPoints.FirstOrDefault(p => iloffset <= p.ILOffset);
			
			if (sequencePoint != null) {
				// VB.NET sometimes produces temporary files which it then deletes
				// (eg 17d14f5c-a337-4978-8281-53493378c1071.vb)
				string name = Path.GetFileName(sequencePoint.Filename);
				if (name.Length == 40 && name.EndsWith(".vb", StringComparison.OrdinalIgnoreCase)) {
					if (name.Substring(0, name.Length - 3).All(c => ('0' <= c && c <= '9') || ('a' <= c && c <= 'f') || ('A' <= c && c <= 'F') || (c == '-'))) {
						return null;
					}
				}
				

				sequencePoint.Filename = GetSourceCodePath(method.ParentAssembly.GetModule().Process, sequencePoint.Filename);
			}
			
			return sequencePoint;
		}
		
		public IEnumerable<SequencePoint> GetSequencePoints(Module module, string filename, int line, int column)
		{
			// Do not use ISymUnmanagedReader.GetDocument!  It is broken if two files have the same name
			// Do not use ISymUnmanagedMethod.GetOffset!  It sometimes returns negative offset
			
			ISymUnmanagedReader symReader = module.SymReader;
			if (symReader == null)
				yield break; // No symbols
			
			// Find ISymUnmanagedDocument which excactly matches the filename.
			var symDoc = module.SymDocuments.FirstOrDefault(d => string.Equals(filename, d.GetURL(), StringComparison.OrdinalIgnoreCase));
			
			// Find the file even if the symbol is relative or if the file was moved
			var symDocs = module.SymDocuments.Where(d => string.Equals(Path.GetFileName(filename), Path.GetFileName(d.GetURL()), StringComparison.OrdinalIgnoreCase));
			symDoc = symDoc ?? symDocs.FirstOrDefault(d => string.Equals(GetSourceCodePath(module.Process, d.GetURL()), filename, StringComparison.OrdinalIgnoreCase));
			if (symDoc == null) yield break; // Document not found
			
			ISymUnmanagedMethod[] symMethods;
			try {
				uint validLine = symDoc.FindClosestLine((uint)line);
				symMethods = symReader.GetMethodsFromDocumentPosition(symDoc, validLine, (uint)column);
				if (validLine > 0)
					line = (int)validLine;
			} catch {
				yield break; //Not found
			}
			
			foreach (ISymUnmanagedMethod symMethod in symMethods) {
				var corFunction = module.CorModule.GetFunctionFromToken(symMethod.GetToken());
				int codeSize = (int)corFunction.GetILCode().GetSize();
				var seqPoints = symMethod.GetSequencePoints(codeSize).Where(s => s.StartLine != 0xFEEFEE).OrderBy(s => s.StartLine).ToArray();
				SequencePoint seqPoint = FindNearestMatchingSequencePoint(seqPoints, line, column);
				if (seqPoint != null)
					yield return seqPoint;
			}
		}
		
		SequencePoint FindNearestMatchingSequencePoint(SequencePoint[] seqPoints, int line, int column)
		{
			for (int i = 0; i < seqPoints.Length; i++) {
				var s = seqPoints[i];
				if (s.ContainsLocation(line, column))
					return s;
				if (s.StartLine > line || (s.StartLine == line && s.StartColumn > column))
					return s;
			}
			return null;
		}
		
		public IEnumerable<ILRange> GetIgnoredILRanges(IMethod method)
		{
			var symMethod = method.GetSymMethod();
			if (symMethod == null)
				return null;
			
			int codeSize = (int)method.ToCorFunction().GetILCode().GetSize();
			return symMethod.GetSequencePoints(codeSize).Where(p => p.StartLine == 0xFEEFEE).SelectMany(p => p.ILRanges).ToList();
		}
		
		public IEnumerable<ILLocalVariable> GetLocalVariables(IMethod method)
		{
			List<ILLocalVariable> vars = new List<ILLocalVariable>();
			var symMethod = method.GetSymMethod();
			if (symMethod == null)
				return vars;
			
			Stack<ISymUnmanagedScope> scopes = new Stack<ISymUnmanagedScope>();
			scopes.Push(symMethod.GetRootScope());
			while(scopes.Count > 0) {
				ISymUnmanagedScope scope = scopes.Pop();
				foreach (ISymUnmanagedVariable symVar in scope.GetLocals()) {
					int index = (int)symVar.GetAddressField1();
					vars.Add(new ILLocalVariable() {
					         	Index = index,
					         	Type = method.GetLocalVariableType(index),
					         	Name = symVar.GetName(),
					         	IsCompilerGenerated = (symVar.GetAttributes() & 1) == 1,
					         	// symVar also has Get*Offset methods, but the are not implemented
					         	ILRanges = new [] { new ILRange() { From = (int)scope.GetStartOffset(), To = (int)scope.GetEndOffset() } }
					         });
				}
				foreach(ISymUnmanagedScope childScope in scope.GetChildren()) {
					scopes.Push(childScope);
				}
			}
			return vars;
		}
	}
	
	static class PDBSymbolSourceExtensions
	{
		public static ISymUnmanagedMethod GetSymMethod(this IMethod method)
		{
			var module = method.ParentAssembly.GetModule();
			if (module.SymReader == null) {
				return null;
			}
			try {
				return module.SymReader.GetMethod(method.GetMetadataToken());
			} catch (System.Runtime.InteropServices.COMException) {
				// Can not find the method
				// eg. Compiler generated constructors are not in symbol store
				return null;
			}
		}
	}
}
