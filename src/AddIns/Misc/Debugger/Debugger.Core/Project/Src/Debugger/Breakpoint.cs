// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

using System;
using Debugger.Wrappers.CorDebug;

namespace Debugger
{
	public class Breakpoint: DebuggerObject
	{
		NDebugger debugger;

		SourcecodeSegment sourcecodeSegment;
		
		bool hadBeenSet = false;
		bool enabled = true;
		ICorDebugFunctionBreakpoint corBreakpoint;
		
		[Debugger.Tests.Ignore]
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
				OnChanged();
			}
		}
		
		public bool Enabled {
			get {
				return enabled;
			}
			set	{
				enabled = value;
				if (HadBeenSet) {
					corBreakpoint.Activate(enabled?1:0);
				}
				OnChanged();
			}
		}
		

		public event EventHandler<BreakpointEventArgs> Changed;

		protected void OnChanged()
		{
			if (Changed != null) {
				Changed(this, new BreakpointEventArgs(this));
			}
		}

		public event EventHandler<BreakpointEventArgs> Hit;

		internal void OnHit()
		{
			if (Hit != null) {
				Hit(this, new BreakpointEventArgs(this));
			}
		}

		internal Breakpoint(NDebugger debugger, SourcecodeSegment sourcecodeSegment, bool enabled)
		{
			this.debugger = debugger;
			this.sourcecodeSegment = sourcecodeSegment;
			this.enabled = enabled;
		}
		
		internal bool Equals(ICorDebugFunctionBreakpoint obj) 
		{
			return corBreakpoint == obj;
		}
		
		public override bool Equals(object obj) 
		{
			return base.Equals(obj) || (corBreakpoint != null && corBreakpoint.Equals(obj));
		}
		
		public override int GetHashCode() 
		{
			return base.GetHashCode();
		}
		
		internal void MarkUnset()
		{
			HadBeenSet = false;
		}
		
		public bool SetBreakpoint(Module module)
		{
			ICorDebugFunction corFunction;
			int ilOffset;
			if (!sourcecodeSegment.GetFunctionAndOffset(module, false, out corFunction, out ilOffset)) {
				return false;
			}
			
			corBreakpoint = corFunction.ILCode.CreateBreakpoint((uint)ilOffset);
			
			hadBeenSet = true;
			corBreakpoint.Activate(enabled?1:0);
			
			OnChanged();
			
			return true;
		}
	}
}
