// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

using Debugger.Interop.CorDebug;

namespace Debugger
{
	public class Breakpoint: DebuggerObject
	{
		bool enabled;
		List<ICorDebugFunctionBreakpoint> corBreakpoints = new List<ICorDebugFunctionBreakpoint>();
		
		[Tests.Ignore]
		public string FileName { get; private set; }
		public int Line { get; set; }
		public int Column { get; set; }
		public byte[] Checksum { get; private set; }
		
		public bool IsEnabled {
			get { return enabled; }
			set {
				enabled = value;
				foreach(ICorDebugFunctionBreakpoint corBreakpoint in corBreakpoints) {
					try {
						corBreakpoint.Activate(enabled ? 1 : 0);
					} catch(COMException e) {
						// Sometimes happens, but we had not repro yet.
						// 0x80131301: Process was terminated.
						if ((uint)e.ErrorCode == 0x80131301)
							continue;
						throw;
					}
				}
			}
		}
		
		public bool IsSet {
			get { return corBreakpoints.Count > 0; }
		}
		
		internal Breakpoint()
		{
		}
		
		internal Breakpoint(string fileName, int line, int column, bool enabled)
		{
			this.FileName = fileName;
			this.Line = line;
			this.Column = column;
			this.enabled = enabled;
		}
		
		internal bool IsOwnerOf(ICorDebugBreakpoint breakpoint)
		{
			foreach(ICorDebugFunctionBreakpoint corFunBreakpoint in corBreakpoints) {
				if (((ICorDebugBreakpoint)corFunBreakpoint).Equals(breakpoint)) return true;
			}
			return false;
		}
		
		internal void NotifyDebuggerTerminated()
		{
			corBreakpoints.Clear();
		}
		
		public void SetBreakpoint(Module module)
		{
			foreach(var symbolSource in module.Process.Debugger.SymbolSources) {
				foreach (var seq in symbolSource.GetSequencePoints(module, this.FileName, this.Line, this.Column)) {
					ICorDebugFunction corFunction = module.CorModule.GetFunctionFromToken(seq.MethodDefToken);
					ICorDebugFunctionBreakpoint corBreakpoint = corFunction.GetILCode().CreateBreakpoint((uint)seq.ILOffset);
					corBreakpoint.Activate(enabled ? 1 : 0);
					corBreakpoints.Add(corBreakpoint);
				}
			}
		}
	}
}
