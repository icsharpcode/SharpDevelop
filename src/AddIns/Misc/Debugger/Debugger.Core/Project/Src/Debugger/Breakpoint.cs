// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

using Debugger.Wrappers.CorDebug;

namespace Debugger
{
	public class Breakpoint: DebuggerObject
	{
		NDebugger debugger;
		
		string fileName;
		byte[] checkSum;
		int    line;
		int    column;
		bool   enabled;

		SourcecodeSegment originalLocation;
		
		List<ICorDebugFunctionBreakpoint> corBreakpoints = new List<ICorDebugFunctionBreakpoint>();
		
		public event EventHandler<BreakpointEventArgs> Hit;
		public event EventHandler<BreakpointEventArgs> Set;
		
		[Debugger.Tests.Ignore]
		public NDebugger Debugger {
			get { return debugger; }
		}
		
		public string FileName {
			get { return fileName; }
		}
		
		public byte[] CheckSum {
			get { return checkSum; }
		}
		
		public int Line {
			get { return line; }
			set { line = value; }
		}
		
		public int Column {
			get { return column; }
		}
		
		public bool Enabled {
			get { return enabled; }
			set {
				enabled = value;
				foreach(ICorDebugFunctionBreakpoint corBreakpoint in corBreakpoints) {
					corBreakpoint.Activate(enabled ? 1 : 0);
				}
			}
		}
		
		public SourcecodeSegment OriginalLocation {
			get { return originalLocation; }
		}
		
		public bool IsSet { 
			get { 
				return corBreakpoints.Count > 0;
			}
		}
		
		protected virtual void OnHit(BreakpointEventArgs e)
		{
			if (Hit != null) {
				Hit(this, e);
			}
		}
		
		internal void NotifyHit()
		{
			OnHit(new BreakpointEventArgs(this));
			debugger.OnBreakpointHit(this);
		}
		
		protected virtual void OnSet(BreakpointEventArgs e)
		{
			if (Set != null) {
				Set(this, e);
			}
		}
		
		public Breakpoint(NDebugger debugger, string fileName, byte[] checkSum, int line, int column, bool enabled)
		{
			this.debugger = debugger;
			this.fileName = fileName;
			this.checkSum = checkSum;
			this.line = line;
			this.column = column;
			this.enabled = enabled;
		}
		
		internal bool IsOwnerOf(ICorDebugBreakpoint breakpoint) 
		{
			foreach(ICorDebugFunctionBreakpoint corFunBreakpoint in corBreakpoints) {
				if (corFunBreakpoint.CastTo<ICorDebugBreakpoint>().Equals(breakpoint)) return true;
			}
			return false;
		}
		
		internal void Deactivate()
		{
			foreach(ICorDebugFunctionBreakpoint corBreakpoint in corBreakpoints) {
				#if DEBUG
					// Get repro
					corBreakpoint.Activate(0);
				#else
					try {
						corBreakpoint.Activate(0);
					} catch(COMException e) {
						// Sometimes happens, but we had not repro yet.
						// 0x80131301: Process was terminated.
						if ((uint)e.ErrorCode == 0x80131301)
							continue;
						throw;
					}
				#endif
			}
			corBreakpoints.Clear();
		}
		
		internal void MarkAsDeactivated()
		{
			corBreakpoints.Clear();
		}
		
		internal bool SetBreakpoint(Module module)
		{
			SourcecodeSegment segment = SourcecodeSegment.Resolve(module, FileName, CheckSum, Line, Column);
			if (segment == null) return false;
			
			originalLocation = segment;
			
			ICorDebugFunctionBreakpoint corBreakpoint = segment.CorFunction.ILCode.CreateBreakpoint((uint)segment.ILStart);
			corBreakpoint.Activate(enabled ? 1 : 0);
			
			corBreakpoints.Add(corBreakpoint);
			
			OnSet(new BreakpointEventArgs(this));
			
			return true;
		}
		
		/// <summary> Remove this breakpoint </summary>
		public void Remove()
		{
			debugger.RemoveBreakpoint(this);
		}
	}
	
	[Serializable]
	public class BreakpointEventArgs : DebuggerEventArgs
	{
		Breakpoint breakpoint;
		
		public Breakpoint Breakpoint {
			get {
				return breakpoint;
			}
		}
		
		public BreakpointEventArgs(Breakpoint breakpoint): base(breakpoint.Debugger)
		{
			this.breakpoint = breakpoint;
		}
	}
}
