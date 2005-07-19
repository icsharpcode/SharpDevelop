// <file>
//     <owner name="David Srbecký" email="dsrbecky@post.cz"/>
// </file>

using System;

namespace DebuggerLibrary 
{	
	[Serializable]
	public class ModuleEventArgs : DebuggerEventArgs
	{
		Module module;
		
		public Module Module {
			get {
				return module;
			}
		}
		
		public ModuleEventArgs(NDebugger debugger, Module module): base(debugger)
		{
			this.module = module;
		}
	}
}
