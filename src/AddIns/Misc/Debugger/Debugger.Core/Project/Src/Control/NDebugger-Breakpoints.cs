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
		BreakpointCollection breakpoints;
		
		public BreakpointCollection Breakpoints {
			get { return breakpoints; }
		}
	}
	
	public class BreakpointCollection: CollectionWithEvents<Breakpoint>
	{
		public event EventHandler<CollectionItemEventArgs<Breakpoint>> Hit;
		
		protected internal void OnHit(Breakpoint item)
		{
			if (Hit != null) {
				Hit(this, new CollectionItemEventArgs<Breakpoint>(item));
			}
		}
		
		public BreakpointCollection(NDebugger debugger):base(debugger)
		{
			
		}
		
		internal Breakpoint Get(ICorDebugBreakpoint corBreakpoint)
		{
			foreach (Breakpoint breakpoint in this) {
				if (breakpoint.IsOwnerOf(corBreakpoint)) {
					return breakpoint;
				}
			}
			return null;
		}
		
		public new void Add(Breakpoint breakpoint)
		{
			base.Add(breakpoint);
		}
		
		public Breakpoint Add(string filename, int line)
		{
			Breakpoint breakpoint = new Breakpoint(this.Debugger, filename, null, line, 0, true);
			Add(breakpoint);
			return breakpoint;
		}
		
		public Breakpoint Add(string fileName, byte[] checkSum, int line, int column, bool enabled)
		{
			Breakpoint breakpoint = new Breakpoint(this.Debugger, fileName, checkSum, line, column, enabled);
			Add(breakpoint);
			return breakpoint;
		}
		
		protected override void OnAdded(Breakpoint breakpoint)
		{
			foreach(Process process in this.Debugger.Processes) {
				foreach(Module module in process.Modules) {
					breakpoint.SetBreakpoint(module);
				}
			}
			
			base.OnAdded(breakpoint);
		}
		
		public new void Remove(Breakpoint breakpoint)
		{
			base.Remove(breakpoint);
		}
		
		protected override void OnRemoved(Breakpoint breakpoint)
		{
			breakpoint.Deactivate();
			
			base.OnRemoved(breakpoint);
		}
		
		internal void SetInModule(Module module) 
		{
			// This is in case that the client modifies the collection as a response to set breakpoint
			// NB: If client adds new breakpoint, it will be set directly as a result of his call, not here (because module is already loaded)
			List<Breakpoint> collection = new List<Breakpoint>();
			collection.AddRange(this);
			
			foreach (Breakpoint b in collection) {
				b.SetBreakpoint(module);
			}
		}
	}
}
