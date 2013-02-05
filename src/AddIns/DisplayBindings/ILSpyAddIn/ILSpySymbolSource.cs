using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Debugger;
using ICSharpCode.Core;
using ICSharpCode.Decompiler;
using ICSharpCode.NRefactory;
using ICSharpCode.NRefactory.Documentation;
using ICSharpCode.NRefactory.TypeSystem;

namespace ICSharpCode.ILSpyAddIn
{
	public class ILSpySymbolSource : ISymbolSource
	{
		public static MethodDebugSymbols GetSymbols(IMethod method)
		{
			// Use the non-specialised method definition to look up decompiled symbols
			var id = IdStringProvider.GetIdString(method.MemberDefinition);
			var content = DecompiledViewContent.Get(method);
			if (content != null && content.DebugSymbols.ContainsKey(id)) {
				return content.DebugSymbols[id];
			}
			return null;
		}
		
		public Debugger.SequencePoint GetSequencePoint(IMethod method, int iloffset)
		{
			var symbols = GetSymbols(method);
			if (symbols == null)
				return null;
			
			var content = DecompiledViewContent.Get(method);
			var seq = symbols.SequencePoints.FirstOrDefault(p => p.ILRanges.Any(r => r.From <= iloffset && iloffset < r.To));
			if (seq == null)
				seq = symbols.SequencePoints.FirstOrDefault(p => iloffset <= p.ILOffset);
			if (seq != null)
				return seq.ToDebugger(symbols, content.VirtualFileName);
			return null;
		}
		
		public Debugger.SequencePoint GetSequencePoint(Module module, string filename, int line, int column)
		{
			var content = DecompiledViewContent.Get(new FileName(filename));
			if (content == null)
				return null;
			if (!FileUtility.IsEqualFileName(module.FullPath, content.AssemblyFile))
				return null;
			
			TextLocation loc = new TextLocation(line, column);
			foreach(var symbols in content.DebugSymbols.Values.Where(s => s.StartLocation <= loc && loc <= s.EndLocation)) {
				Decompiler.SequencePoint seq = null;
				if (column != 0)
					seq = symbols.SequencePoints.FirstOrDefault(p => p.StartLocation <= loc && loc <= p.EndLocation);
				if (seq == null)
					seq = symbols.SequencePoints.FirstOrDefault(p => line <= p.StartLocation.Line);
				if (seq != null)
					return seq.ToDebugger(symbols, content.VirtualFileName);
			}
			return null;
		}
		
		public bool HasSymbols(IMethod method)
		{
			var symbols = GetSymbols(method);
			return symbols != null && symbols.SequencePoints.Any();
		}
		
		public IEnumerable<ILRange> GetIgnoredILRanges(IMethod method)
		{
			var symbols = GetSymbols(method);
			if (symbols == null)
				return new ILRange[] { };
			
			int codesize = symbols.CecilMethod.Body.CodeSize;
			var inv = ICSharpCode.Decompiler.ILAst.ILRange.Invert(symbols.SequencePoints.SelectMany(s => s.ILRanges), codesize);
			return inv.Select(r => new ILRange(r.From, r.To));
		}
		
		public IEnumerable<ILLocalVariable> GetLocalVariables(IMethod method)
		{
			var symbols = GetSymbols(method);
			if (symbols == null)
				return null;
			
			return symbols.LocalVariables.Select(v => new Debugger.ILLocalVariable() {
					Index = v.OriginalVariable.Index,
					Type = method.Compilation.FindType(KnownTypeCode.Object), // TODO
				Name = v.Name,
				IsCompilerGenerated = false,
				ILRanges = new [] { new Debugger.ILRange(0, int.MaxValue) }
			});
		}
	}
	
	static class ILSpySymbolSourceExtensions
	{
		public static Debugger.SequencePoint ToDebugger(this ICSharpCode.Decompiler.SequencePoint seq, ICSharpCode.Decompiler.MethodDebugSymbols symbols, string filename)
		{
			return new Debugger.SequencePoint() {
				MethodDefToken = symbols.CecilMethod.MetadataToken.ToUInt32(),
				ILRanges = seq.ILRanges.Select(r => new ILRange(r.From, r.To)).ToArray(),
				Filename = filename,
				StartLine = seq.StartLocation.Line,
				StartColumn = seq.StartLocation.Column,
				EndLine = seq.EndLocation.Line,
				EndColumn = seq.EndLocation.Column,
			};
		}
	}
}
