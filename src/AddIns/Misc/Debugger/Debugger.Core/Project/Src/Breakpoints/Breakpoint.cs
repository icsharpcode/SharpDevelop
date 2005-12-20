// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Diagnostics.SymbolStore;
using System.Collections;
using System.Runtime.InteropServices;

using Debugger.Wrappers.CorDebug;

namespace Debugger
{
	public class Breakpoint: RemotingObjectBase
	{
		NDebugger debugger;

		SourcecodeSegment sourcecodeSegment;
		
		bool hadBeenSet = false;
		bool enabled = true;
		ICorDebugFunctionBreakpoint corBreakpoint;
		IntPtr pBreakpoint;
		
		public NDebugger Debugger {
			get {
				return debugger;
			}
		}
		
		public SourcecodeSegment SourcecodeSegment {
			get {
				return sourcecodeSegment;
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
				if (HadBeenSet) {
					int active;
					corBreakpoint.IsActive(out active);
					enabled = (active == 1);
				}
				return enabled;
			}
			set	{
				enabled = value;
				if (HadBeenSet) {
					corBreakpoint.Activate(enabled?1:0);
				}
				OnBreakpointStateChanged();
			}
		}
		

		public event EventHandler<BreakpointEventArgs> BreakpointStateChanged;

		internal void OnBreakpointStateChanged()
		{
			if (BreakpointStateChanged != null)
				BreakpointStateChanged(this, new BreakpointEventArgs(this));
		}

		public event EventHandler<BreakpointEventArgs> BreakpointHit;

		internal void OnBreakpointHit()
		{
			if (BreakpointHit != null)
				BreakpointHit(this, new BreakpointEventArgs(this));
		}

		internal Breakpoint(NDebugger debugger, SourcecodeSegment sourcecodeSegment, bool enabled)
		{
			this.debugger = debugger;
			this.sourcecodeSegment = sourcecodeSegment;
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
			return base.Equals(obj) || corBreakpoint == (obj as ICorDebugFunctionBreakpoint);
		}
		
		public override int GetHashCode() 
		{
			return base.GetHashCode();
		}
		
		internal unsafe void ResetBreakpoint()
		{
			hadBeenSet = false;
			OnBreakpointStateChanged();
		}
		
		
		internal unsafe bool SetBreakpoint()
		{
			if (hadBeenSet) {
				return true;
			}

			ICorDebugFunction corFunction;
			int ilOffset;
			if (!sourcecodeSegment.GetFunctionAndOffset(debugger, true, out corFunction, out ilOffset)) {
				return false;
			}

			ICorDebugCode code;
			corFunction.GetILCode(out code);

			code.CreateBreakpoint((uint)ilOffset, out corBreakpoint);
			
			hadBeenSet = true;
			corBreakpoint.Activate(enabled?1:0);
			pBreakpoint = Marshal.GetComInterfaceForObject(corBreakpoint.WrappedObject, typeof(Debugger.Interop.CorDebug.ICorDebugFunctionBreakpoint));
			
			OnBreakpointStateChanged();
			
			return true;
		}
	}
}
