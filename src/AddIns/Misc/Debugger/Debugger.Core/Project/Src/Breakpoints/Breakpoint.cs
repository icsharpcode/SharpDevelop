// <file>
//     <owner name="David Srbecký" email="dsrbecky@post.cz"/>
// </file>

using System;
using System.Diagnostics.SymbolStore;
using System.Collections;
using System.Runtime.InteropServices;

using DebuggerInterop.Core;

namespace DebuggerLibrary
{
	public class Breakpoint
	{
		readonly SourcecodeSegment sourcecodeSegment;
		
		bool hadBeenSet = false;
		bool enabled = true;
		ICorDebugFunctionBreakpoint corBreakpoint;
		IntPtr pBreakpoint;
		object tag;
		
		public SourcecodeSegment SourcecodeSegment {
			get {
				return sourcecodeSegment;
			}
		}

		public object Tag {
			get {
				return tag;
			}
			set {
				tag = value;
			}
		}
		
		public bool HadBeenSet { 
			get { 
				return hadBeenSet;
			}
			internal set {
				hadBeenSet = value;
			}
		}
		
		public bool Enabled	{
			get {
				if (HadBeenSet)
				{
					int active;
					corBreakpoint.IsActive(out active);
					enabled = (active == 1);
				}
				return enabled;
			}
			set	{
				enabled = value;
				if (HadBeenSet)
				{
					corBreakpoint.Activate(enabled?1:0);
				}
				OnBreakpointStateChanged();
			}
		}
		

		public event BreakpointEventHandler BreakpointStateChanged;

		internal void OnBreakpointStateChanged()
		{
			if (BreakpointStateChanged != null)
				BreakpointStateChanged(this, new BreakpointEventArgs(this));
		}

		public event BreakpointEventHandler BreakpointHit;

		internal void OnBreakpointHit()
		{
			if (BreakpointHit != null)
				BreakpointHit(this, new BreakpointEventArgs(this));
		}

		public Breakpoint(SourcecodeSegment segment)
		{
			sourcecodeSegment = segment;
		}

		public Breakpoint(int line)
		{
			sourcecodeSegment = new SourcecodeSegment();
			sourcecodeSegment.StartLine = line;
		}

		public Breakpoint(string sourceFilename, int line)
		{
			sourcecodeSegment = new SourcecodeSegment();
			sourcecodeSegment.SourceFullFilename = sourceFilename;
			sourcecodeSegment.StartLine = line;
		}

		public Breakpoint(string sourceFilename, int line, int column)
		{
			sourcecodeSegment = new SourcecodeSegment();
			sourcecodeSegment.SourceFullFilename = sourceFilename;
			sourcecodeSegment.StartLine = line;
			sourcecodeSegment.StartColumn = column;
		}

		public Breakpoint(string sourceFilename, int line, int column, bool enabled)
		{
			sourcecodeSegment = new SourcecodeSegment();
			sourcecodeSegment.SourceFullFilename = sourceFilename;
			sourcecodeSegment.StartLine = line;
			sourcecodeSegment.StartColumn = column;
			this.enabled = enabled;
		}
		
		internal bool Equals(IntPtr ptr) 
		{
			return pBreakpoint == ptr;
		}
				
		internal bool Equals(ICorDebugFunctionBreakpoint obj) 
		{
			return corBreakpoint == obj;
		}
		
		public override bool Equals(object obj) 
		{
			return base.Equals(obj) || corBreakpoint == obj;
		}
		
		public override int GetHashCode() 
		{
			return base.GetHashCode();
		}
		
		internal unsafe void ResetBreakpoint() //TODO
		{
			hadBeenSet = false;
			OnBreakpointStateChanged();
		}
		
		
		internal unsafe void SetBreakpoint()
		{
			if (hadBeenSet) {
				return;
			}

			SourcecodeSegment seg = sourcecodeSegment;

			Module           module     = null;
			ISymbolReader    symReader  = null;
			ISymbolDocument  symDoc     = null;

			// Try to get doc from seg.moduleFilename
			if (seg.ModuleFilename != null)
			{
				try 
				{
					module = NDebugger.Instance.GetModule(seg.ModuleFilename);
					symReader = NDebugger.Instance.GetModule(seg.ModuleFilename).SymReader;
					symDoc = symReader.GetDocument(seg.SourceFullFilename,Guid.Empty,Guid.Empty,Guid.Empty);
				}
				catch {}
			}

			// search all modules
			if (symDoc == null) {
				foreach (Module m in NDebugger.Instance.Modules) {
					module    = m;
					symReader = m.SymReader;
					if (symReader == null) {
						continue;
					}

					symDoc = symReader.GetDocument(seg.SourceFullFilename,Guid.Empty,Guid.Empty,Guid.Empty);

					if (symDoc != null) {
						break;
					}
				}
			}

			if (symDoc == null) {
				//throw new Exception("Failed to add breakpoint - (module not loaded? wrong sourceFilename?)");
				return;
			}

			int validStartLine;
			validStartLine = symDoc.FindClosestLine(seg.StartLine);
			if (validStartLine != seg.StartLine) {
				seg.StartLine = validStartLine;
				seg.EndLine = validStartLine;
				seg.StartColumn = 0;
				seg.EndColumn = 0;
			}

			ISymbolMethod symMethod;
			symMethod = symReader.GetMethodFromDocumentPosition(symDoc, seg.StartLine, seg.StartColumn);
			
			int corInstructionPtr = symMethod.GetOffset(symDoc, seg.StartLine, seg.StartColumn);

			ICorDebugFunction corFunction;
			module.CorModule.GetFunctionFromToken((uint)symMethod.Token.GetToken(), out corFunction);

			ICorDebugCode code;
			corFunction.GetILCode(out code);

			code.CreateBreakpoint((uint)corInstructionPtr, out corBreakpoint);
			
			hadBeenSet = true;
			corBreakpoint.Activate(enabled?1:0);
			pBreakpoint = Marshal.GetComInterfaceForObject(corBreakpoint, typeof(ICorDebugFunctionBreakpoint));
			OnBreakpointStateChanged();
			
		}
	}
}
