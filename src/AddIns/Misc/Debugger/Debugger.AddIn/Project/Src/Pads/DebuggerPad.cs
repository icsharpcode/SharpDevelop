// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

using System;
using Debugger;
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
		
		protected virtual void SelectProcess(Debugger.Process process)
		{
			
		}
		
		public virtual void RefreshPad()
		{
			
		}
	}
}
