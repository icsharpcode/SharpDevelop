// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

using System;
using Debugger.Wrappers.CorDebug;

namespace Debugger
{
	public class ModuleCollection: CollectionWithEvents<Module>
	{
		public ModuleCollection(NDebugger debugger):base (debugger) {}
		
		int lastAssignedModuleOrderOfLoading = 0;

		public Module this[string filename] {
			get {
				foreach(Module module in this) {
					if (module.Filename == filename) {
						return module;
					}
				}
				throw new DebuggerException("Module \"" + filename + "\" is not in collection");
			}
		}

		internal Module this[ICorDebugModule corModule] {
			get {
				foreach(Module module in this) {
					if (module.CorModule == corModule) {
						return module;
					}
				}
				throw new DebuggerException("Module is not in collection");
			}
		}
		
		protected override void OnAdded(Module module)
		{
			module.OrderOfLoading = lastAssignedModuleOrderOfLoading;
			lastAssignedModuleOrderOfLoading++;
			this.Debugger.Breakpoints.SetInModule(module);
			base.OnAdded(module);
		}
		
		protected override void OnRemoved(Module module)
		{
			base.OnRemoved(module);
			module.Dispose();
		}
	}
}
