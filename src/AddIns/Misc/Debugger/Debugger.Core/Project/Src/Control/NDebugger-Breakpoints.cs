// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using Debugger.Wrappers.CorDebug;

namespace Debugger
{
	public partial class NDebugger
	{
		List<Breakpoint> breakpointCollection = new List<Breakpoint>();
		
		public event EventHandler<BreakpointEventArgs> BreakpointAdded;
		public event EventHandler<BreakpointEventArgs> BreakpointRemoved;
		public event EventHandler<BreakpointEventArgs> BreakpointHit;
		
		protected virtual void OnBreakpointAdded(Breakpoint breakpoint)
		{
			if (BreakpointAdded != null) {
				BreakpointAdded(this, new BreakpointEventArgs(breakpoint));
			}
		}
		
		protected virtual void OnBreakpointRemoved(Breakpoint breakpoint)
		{
			if (BreakpointRemoved != null) {
				BreakpointRemoved(this, new BreakpointEventArgs(breakpoint));
			}
		}
		
		protected internal virtual void OnBreakpointHit(Breakpoint breakpoint)
		{
			if (BreakpointHit != null) {
				BreakpointHit(this, new BreakpointEventArgs(breakpoint));
			}
		}
		
		public IList<Breakpoint> Breakpoints {
			get {
				return breakpointCollection.AsReadOnly();
			}
		}

		internal Breakpoint GetBreakpoint(ICorDebugBreakpoint corBreakpoint)
		{
			foreach (Breakpoint breakpoint in this.Breakpoints) {
				if (breakpoint.IsOwnerOf(corBreakpoint)) {
					return breakpoint;
				}
			}
			return null;
		}
		
		public Breakpoint AddBreakpoint(Breakpoint breakpoint)  
		{
			breakpointCollection.Add(breakpoint);
			
			foreach(Process process in this.Processes) {
				foreach(Module module in process.Modules) {
					breakpoint.SetBreakpoint(module);
				}
			}
			
			OnBreakpointAdded(breakpoint);
			
			return breakpoint;
		}
		
		public Breakpoint AddBreakpoint(string filename, int line)
		{
			return AddBreakpoint(new Breakpoint(this, filename, null, line, 0, true));
		}
		
		public Breakpoint AddBreakpoint(string fileName, byte[] checkSum, int line, int column, bool enabled)
		{
			return AddBreakpoint(new Breakpoint(this, fileName, checkSum, line, column, enabled));
		}
		
		public void RemoveBreakpoint(Breakpoint breakpoint)  
		{
			if (breakpointCollection.Contains(breakpoint)) {
				breakpoint.Deactivate();
				breakpointCollection.Remove(breakpoint);
				OnBreakpointRemoved(breakpoint);
			}
		}

		void MarkBreakpointsAsDeactivated()
		{
			foreach (Breakpoint b in breakpointCollection) {
				b.MarkAsDeactivated();
			}
		}
		
		void DeactivateBreakpoints()
		{
			foreach (Breakpoint b in breakpointCollection) {
				b.Deactivate();
			}
		}

		public void ClearBreakpoints()
		{
			foreach (Breakpoint b in breakpointCollection) {
				OnBreakpointRemoved(b);
			}
			breakpointCollection.Clear();
		}

		internal void SetBreakpointsInModule(Module module) 
		{
			// This is in case that the client modifies the collection as a response to set breakpoint
			// NB: If client adds new breakpoint, it will be set directly as a result of his call, not here (because module is already loaded)
			List<Breakpoint> collection = new List<Breakpoint>();
			collection.AddRange(breakpointCollection);
			
			foreach (Breakpoint b in collection) {
				b.SetBreakpoint(module);
			}
		}
	}
}
