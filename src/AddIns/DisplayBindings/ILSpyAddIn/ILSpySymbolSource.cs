using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Debugger;
using ICSharpCode.Core;
using ICSharpCode.Decompiler;
using ICSharpCode.NRefactory;
using ICSharpCode.NRefactory.Documentation;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.SharpDevelop;

namespace ICSharpCode.ILSpyAddIn
{
	public class ILSpySymbolSource : ISymbolSource
	{
		public bool Handles(IMethod method)
		{
			return !SD.Debugger.Options.EnableJustMyCode;
		}
		
		public bool IsCompilerGenerated(IMethod method)
		{
			return false;
		}
		
		public static ILSpyUnresolvedFile GetSymbols(IMethod method)
		{
			var typeName = DecompiledTypeReference.FromTypeDefinition(method.DeclaringTypeDefinition);
			if (typeName == null) return null;
			SD.Log.DebugFormatted("GetSymbols for: {0}", typeName.ToFileName());
			// full parse info required to make ParserService caching possible...
			return SD.ParserService.Parse(typeName.ToFileName()).UnresolvedFile as ILSpyUnresolvedFile;
		}
		
		public Debugger.SequencePoint GetSequencePoint(IMethod method, int iloffset)
		{
			string id = IdStringProvider.GetIdString(method.MemberDefinition);
			var file = GetSymbols(method);
			if (file == null || !file.DebugSymbols.ContainsKey(id))
				return null;
			var symbols = file.DebugSymbols[id];
			var seqs = symbols.SequencePoints;
			var seq = seqs.FirstOrDefault(p => p.ILRanges.Any(r => r.From <= iloffset && iloffset < r.To));
			if (seq == null)
				seq = seqs.FirstOrDefault(p => iloffset <= p.ILOffset);
			if (seq != null) {
				// Use the widest sequence point containing the IL offset
				iloffset = seq.ILOffset;
				seq = seqs.Where(p => p.ILRanges.Any(r => r.From <= iloffset && iloffset < r.To))
					.OrderByDescending(p => p.ILRanges.Last().To - p.ILRanges.First().From)
					.FirstOrDefault();
				return seq.ToDebugger(symbols, file.FileName);
			}
			return null;
		}
		
		public IEnumerable<Debugger.SequencePoint> GetSequencePoints(Module module, string filename, int line, int column)
		{
			var name = DecompiledTypeReference.FromFileName(filename);
			if (name == null || !FileUtility.IsEqualFileName(module.FullPath, name.AssemblyFile))
				yield break;
			
			var file = SD.ParserService.ParseFile(name.ToFileName()) as ILSpyUnresolvedFile;
			if (file == null)
				yield break;
			
			TextLocation loc = new TextLocation(line, column);
			foreach(var symbols in file.DebugSymbols.Values.Where(s => s.StartLocation <= loc && loc <= s.EndLocation)) {
				Decompiler.SequencePoint seq = null;
				if (column != 0)
					seq = symbols.SequencePoints.FirstOrDefault(p => p.StartLocation <= loc && loc <= p.EndLocation);
				if (seq == null)
					seq = symbols.SequencePoints.FirstOrDefault(p => line <= p.StartLocation.Line);
				if (seq != null)
					yield return seq.ToDebugger(symbols, filename);
			}
		}
		
		public IEnumerable<ILRange> GetIgnoredILRanges(IMethod method)
		{
			string id = IdStringProvider.GetIdString(method.MemberDefinition);
			var file = GetSymbols(method);
			
			if (file == null || !file.DebugSymbols.ContainsKey(id))
				return new ILRange[] { };
			
			var symbols = file.DebugSymbols[id];
			int codesize = symbols.CecilMethod.Body.CodeSize;
			var inv = ICSharpCode.Decompiler.ILAst.ILRange.Invert(symbols.SequencePoints.SelectMany(s => s.ILRanges), codesize);
			return inv.Select(r => new ILRange(r.From, r.To));
		}
		
		public IEnumerable<ILLocalVariable> GetLocalVariables(IMethod method)
		{
			string id = IdStringProvider.GetIdString(method.MemberDefinition);
			var file = GetSymbols(method);
			
			if (file == null || !file.DebugSymbols.ContainsKey(id))
				return Enumerable.Empty<ILLocalVariable>();
			
			var symbols = file.DebugSymbols[id];
			
			var context = new SimpleTypeResolveContext(method);
			var loader = new CecilLoader();
			
			return symbols.LocalVariables.Where(v => v.OriginalVariable != null).Select(
				v => new Debugger.ILLocalVariable() {
					Index = v.OriginalVariable.Index,
					Type = loader.ReadTypeReference(v.Type).Resolve(context).AcceptVisitor(method.Substitution),
					Name = v.Name,
					IsCompilerGenerated = false,
					ILRanges = new [] { new ILRange(0, int.MaxValue) }
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
