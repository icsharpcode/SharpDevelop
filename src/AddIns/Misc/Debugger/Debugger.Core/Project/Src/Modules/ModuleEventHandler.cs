// <file>
//     <owner name="David Srbecký" email="dsrbecky@post.cz"/>
// </file>

using System;

namespace DebuggerLibrary 
{	
	public delegate void ModuleEventHandler (object sender, ModuleEventArgs e);
	
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
