// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the BSD license (for details please see \src\AddIns\Debugger\Debugger.AddIn\license.txt)

using Debugger;
using ICSharpCode.SharpDevelop.Debugging;
using ICSharpCode.SharpDevelop.Services;

namespace ICSharpCode.SharpDevelop.Gui.Pads
{
	public abstract class DebuggerPad: AbstractPadContent
	{
		protected WindowsDebugger debugger;
		
		public DebuggerPad()
		{
			debugger = (WindowsDebugger)DebuggerService.CurrentDebugger;
			
			InitializeComponents();
			
			debugger.ProcessSelected += delegate(object sender, ProcessEventArgs e) {
				SelectProcess(e.Process);
			};
			SelectProcess(debugger.DebuggedProcess);
		}
		
		protected virtual void InitializeComponents()
		{
			
		}
		
		protected virtual void SelectProcess(Process process)
		{
			
		}
		
		public virtual void RefreshPad()
		{
			
		}
	}
}
