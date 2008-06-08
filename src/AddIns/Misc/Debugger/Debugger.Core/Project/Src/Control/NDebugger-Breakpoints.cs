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
		public event EventHandler<BreakpointEventArgs> BreakpointStateChanged;
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

		protected virtual void OnBreakpointStateChanged(object sender, BreakpointEventArgs e)
		{
			if (BreakpointStateChanged != null) {
				BreakpointStateChanged(this, new BreakpointEventArgs(e.Breakpoint));
			}
		}

		protected virtual void OnBreakpointHit(object sender, BreakpointEventArgs e)
		{
			if (BreakpointHit != null) {
				BreakpointHit(this, new BreakpointEventArgs(e.Breakpoint));
			}
		}

		public IList<Breakpoint> Breakpoints {
			get {
				return breakpointCollection.AsReadOnly();
			}
		}

		internal Breakpoint GetBreakpoint(ICorDebugBreakpoint corBreakpoint)
		{
			foreach(Breakpoint breakpoint in breakpointCollection) {
				if (breakpoint.Equals(corBreakpoint)) {
					return breakpoint;
				}
			}
			
			throw new DebuggerException("Breakpoint is not in collection");
		}
		
		internal Breakpoint AddBreakpoint(Breakpoint breakpoint)  
		{
			breakpointCollection.Add(breakpoint);
			
			foreach(Process process in this.Processes) {
				foreach(Module module in process.Modules) {
					breakpoint.SetBreakpoint(module);
				}
			}
			breakpoint.Changed += new EventHandler<BreakpointEventArgs>(OnBreakpointStateChanged);
			breakpoint.Hit += new EventHandler<BreakpointEventArgs>(OnBreakpointHit);
			
			OnBreakpointAdded(breakpoint);
			
			return breakpoint;
		}
		
		public Breakpoint AddBreakpoint(string filename, int line)
		{
			return AddBreakpoint(new SourcecodeSegment(filename, line), true);
		}
		
		public Breakpoint AddBreakpoint(SourcecodeSegment segment, bool breakpointEnabled)
		{
			return AddBreakpoint(new Breakpoint(this, segment, breakpointEnabled));
		}
		
		public void RemoveBreakpoint(Breakpoint breakpoint)  
		{
			breakpoint.Changed -= new EventHandler<BreakpointEventArgs>(OnBreakpointStateChanged);
			breakpoint.Hit -= new EventHandler<BreakpointEventArgs>(OnBreakpointHit);
			
			breakpoint.Enabled = false;
			breakpointCollection.Remove( breakpoint );
			OnBreakpointRemoved( breakpoint);
		}

		public void ResetBreakpoints()
		{
			foreach (Breakpoint b in breakpointCollection) {
				b.MarkUnset();
			}
		}
		
		public void DeactivateBreakpoints()
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
