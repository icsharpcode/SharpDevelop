// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Runtime.InteropServices;
using System.Collections.Generic;

using DebuggerInterop.Core;
using DebuggerInterop.MetaData;

namespace DebuggerLibrary
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
				BreakpointAdded(this, new BreakpointEventArgs(this, breakpoint));
			}
		}

		protected virtual void OnBreakpointRemoved(Breakpoint breakpoint)
		{
			if (BreakpointRemoved != null) {
				BreakpointRemoved(this, new BreakpointEventArgs(this, breakpoint));
			}
		}

		protected virtual void OnBreakpointStateChanged(object sender, BreakpointEventArgs e)
		{
			if (BreakpointStateChanged != null) {
				BreakpointStateChanged(this, new BreakpointEventArgs(this, e.Breakpoint));
			}
		}

		protected virtual void OnBreakpointHit(object sender, BreakpointEventArgs e)
		{
			if (BreakpointHit != null) {
				BreakpointHit(this, new BreakpointEventArgs(this, e.Breakpoint));
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
				if (breakpoint == corBreakpoint) {
					return breakpoint;
				}
			}

			throw new DebuggerException("Breakpoint is not in collection");
		}

		internal Breakpoint AddBreakpoint(Breakpoint breakpoint)  
		{
			breakpointCollection.Add(breakpoint);

			breakpoint.SetBreakpoint();
			breakpoint.BreakpointStateChanged += new EventHandler<BreakpointEventArgs>(OnBreakpointStateChanged);
			breakpoint.BreakpointHit += new EventHandler<BreakpointEventArgs>(OnBreakpointHit);

			OnBreakpointAdded(breakpoint);

			return breakpoint;
		}

		public Breakpoint AddBreakpoint(SourcecodeSegment segment, bool breakpointEnabled)
		{
			return AddBreakpoint(new Breakpoint(this, segment, breakpointEnabled));
		}

		public void RemoveBreakpoint(Breakpoint breakpoint)  
		{
			breakpoint.BreakpointStateChanged -= new EventHandler<BreakpointEventArgs>(OnBreakpointStateChanged);
			breakpoint.BreakpointHit -= new EventHandler<BreakpointEventArgs>(OnBreakpointHit);
	
            breakpoint.Enabled = false;
			breakpointCollection.Remove( breakpoint );
			OnBreakpointRemoved( breakpoint);
		}

		public void ResetBreakpoints()
		{
			foreach (Breakpoint b in breakpointCollection) {
				b.ResetBreakpoint();
			}
		}

		public void ClearBreakpoints()
		{
			foreach (Breakpoint b in breakpointCollection) {
				OnBreakpointRemoved(b);
			}
			breakpointCollection.Clear();
		}

		internal void SetBreakpointsInModule(object sender, ModuleEventArgs e) 
		{
			// This is in case that the client modifies the collection as a response to set breakpoint
			// NB: If client adds new breakpoint, it will be set directly as a result of his call, not here (because module is already loaded)
			List<Breakpoint> collection = new List<Breakpoint>();
			collection.AddRange(breakpointCollection);
			
			foreach (Breakpoint b in collection) {
				b.SetBreakpoint();
			}
		}
	}
}
