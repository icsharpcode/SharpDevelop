// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

using System;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Services;

using Debugger;

namespace ICSharpCode.SharpDevelop.Gui.Pads
{
	public abstract class DebuggerPad: AbstractPadContent
	{
		protected WindowsDebugger debugger;
		protected NDebugger debuggerCore;
		
		public DebuggerPad()
		{
			debugger = (WindowsDebugger)DebuggerService.CurrentDebugger;
			
			InitializeComponents();
			
			if (debugger.ServiceInitialized) {
				InitializeDebugger();
			} else {
				debugger.Initialize += delegate {
					InitializeDebugger();
				};
			}
		}
		
		protected virtual void InitializeComponents()
		{
			
		}
		
		void InitializeDebugger()
		{
			debuggerCore = debugger.DebuggerCore;

			RegisterDebuggerEvents();
			
			RefreshPad();
		}
		
		protected virtual void RegisterDebuggerEvents()
		{
			
		}
		
		public virtual void RefreshPad()
		{
			
		}
	}
}
