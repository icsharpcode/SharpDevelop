// <file>
//     <owner name="David Srbecký" email="dsrbecky@post.cz"/>
// </file>

using System;
using System.Runtime.InteropServices;
using System.Collections;

using DebuggerInterop.Core;
using DebuggerInterop.MetaData;

namespace DebuggerLibrary
{
	public class BreakpointCollection: CollectionBase
	{
		internal BreakpointCollection()
		{
			NDebugger.Modules.ModuleAdded += new ModuleEventHandler(SetBreakpointsInModule);
		}

		public event BreakpointEventHandler BreakpointAdded;

		private void OnBreakpointAdded(Breakpoint breakpoint)
		{
			breakpoint.BreakpointStateChanged += new BreakpointEventHandler(OnBreakpointStateChanged);
			breakpoint.BreakpointHit += new BreakpointEventHandler(OnBreakpointHit);
			if (BreakpointAdded != null)
				BreakpointAdded(this, new BreakpointEventArgs(breakpoint));
		}


		public event BreakpointEventHandler BreakpointRemoved;

		private void OnBreakpointRemoved(Breakpoint breakpoint)
		{
			breakpoint.BreakpointStateChanged -= new BreakpointEventHandler(OnBreakpointStateChanged);
			breakpoint.BreakpointHit -= new BreakpointEventHandler(OnBreakpointHit);
			if (BreakpointRemoved != null)
				BreakpointRemoved(this, new BreakpointEventArgs(breakpoint));
		}


		public event BreakpointEventHandler BreakpointStateChanged;

		private void OnBreakpointStateChanged(object sender, BreakpointEventArgs e)
		{
			if (BreakpointStateChanged != null)
				BreakpointStateChanged(this, new BreakpointEventArgs(e.Breakpoint));
		}


		public event BreakpointEventHandler BreakpointHit;

		private void OnBreakpointHit(object sender, BreakpointEventArgs e)
		{
			if (BreakpointHit != null)
				BreakpointHit(this, new BreakpointEventArgs(e.Breakpoint));
		}


		public Breakpoint this[int index]  
		{
			get  
			{
				return( (Breakpoint) List[index] );
			}
			set  
			{
				Breakpoint oldValue = (Breakpoint)List[index];
				List[index] = value;
				OnBreakpointRemoved( oldValue );
				OnBreakpointAdded( value );
			}
		}

		internal Breakpoint this[ICorDebugBreakpoint corBreakpoint]  
		{
			get  
			{
				foreach(Breakpoint breakpoint in InnerList)
					if (breakpoint == corBreakpoint)
						return breakpoint;

				throw new UnableToGetPropertyException(this, "this[ICorDebugBreakpoint]", "Breakpoint is not in collection");
			}
		}

		public int Add(Breakpoint breakpoint)  
		{
			System.Diagnostics.Trace.Assert(breakpoint != null);
			if (breakpoint != null)
			{
				int retVal = List.Add(breakpoint);
				breakpoint.SetBreakpoint();
				OnBreakpointAdded(breakpoint);
				return retVal;
			} else {
				return -1;
			}
		}

		public int Add(SourcecodeSegment segment)
		{
			return Add(new Breakpoint(segment));
		}

		public int Add(int line)
		{
			return Add(new Breakpoint(line));
		}

		public int Add(string sourceFilename, int line)
		{
			return Add(new Breakpoint(sourceFilename, line));
		}

		public int Add(string sourceFilename, int line, int column)
		{
			return Add(new Breakpoint(sourceFilename, line, column));
		}

		public int IndexOf( Breakpoint breakpoint )  
		{
			return( List.IndexOf( breakpoint ) );
		}

		public void Insert( int index, Breakpoint breakpoint )  
		{
			System.Diagnostics.Trace.Assert(breakpoint != null);
			if (breakpoint != null)
			{
				List.Insert( index, breakpoint );
				OnBreakpointAdded(breakpoint);
			}
		}

		public void Remove( Breakpoint breakpoint )  
		{
            breakpoint.Enabled = false;
			List.Remove( breakpoint );
			OnBreakpointRemoved( breakpoint);
		}

		public bool Contains( Breakpoint breakpoint )  
		{
			return( List.Contains( breakpoint ) );
		}

		private void SetBreakpointsInModule(object sender, ModuleEventArgs e) 
		{
            foreach (Breakpoint b in InnerList) {
				b.SetBreakpoint();
			}
		}
	}
}
