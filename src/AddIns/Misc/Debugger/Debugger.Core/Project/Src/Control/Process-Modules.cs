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
	public partial class Process
	{
		int lastAssignedModuleOrderOfLoading= 0;

		List<Module> moduleCollection = new List<Module>();

		public event EventHandler<ModuleEventArgs> ModuleLoaded;
		public event EventHandler<ModuleEventArgs> ModuleUnloaded;
		public event EventHandler<ModuleEventArgs> ModuleSymbolsLoaded;

		protected void OnModuleLoaded(Module module)
		{
			if (ModuleLoaded != null) {
				ModuleLoaded(this, new ModuleEventArgs(module));
			}
		}

		protected void OnModuleUnloaded(Module module)
		{
			if (ModuleUnloaded != null) {
				ModuleUnloaded(this, new ModuleEventArgs(module));
			}
		}
		
		internal virtual void OnModuleSymbolsLoaded(ModuleEventArgs e)
		{
			if (ModuleSymbolsLoaded != null) {
				ModuleSymbolsLoaded(this, e);
			}
		}

		public IList<Module> Modules { 
			get{ 
				return moduleCollection.AsReadOnly();
			} 
		}

		public Module GetModule(string filename) 
		{
			foreach(Module module in moduleCollection) {
				if (module.Filename == filename) {
					return module;
				}
			}

			throw new DebuggerException("Module \"" + filename + "\" is not in collection");
		}

		internal Module GetModule(ICorDebugModule corModule) 
		{
			foreach(Module module in moduleCollection) {
				if (module.CorModule == corModule) {
					return module;
				}
			}

			throw new DebuggerException("Module is not in collection");
		}

		internal void AddModule(Module module)
		{
			module.OrderOfLoading = lastAssignedModuleOrderOfLoading;
			lastAssignedModuleOrderOfLoading++;
			moduleCollection.Add(module);
			debugger.SetBreakpointsInModule(module);
			OnModuleLoaded(module);
		}

		internal void AddModule(ICorDebugModule corModule)
		{
			AddModule(new Module(this, corModule));
		}

		internal void RemoveModule(Module module)
		{
			moduleCollection.Remove(module);
			OnModuleUnloaded(module);
			module.Dispose();
		}

		internal void RemoveModule(ICorDebugModule corModule)
		{
			RemoveModule(GetModule(corModule));
		}

		internal void ClearModules()
		{
			while(moduleCollection.Count > 0) {
				RemoveModule(moduleCollection[0]);
			}
			lastAssignedModuleOrderOfLoading = 0;
		}
	}
}
