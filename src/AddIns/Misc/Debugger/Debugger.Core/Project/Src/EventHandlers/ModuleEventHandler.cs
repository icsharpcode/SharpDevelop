// <file>
//     <owner name="David Srbecký" email="dsrbecky@post.cz"/>
// </file>

using System;

namespace DebuggerLibrary 
{	
	public delegate void ModuleEventHandler (object sender, ModuleEventArgs e);
	
	public class ModuleEventArgs : System.EventArgs 
	{
		Module module;
		
		public Module Module {
			get {
				return module;
			}
		}
		
		public ModuleEventArgs(Module module)
		{
			this.module = module;
		}
	}
}
