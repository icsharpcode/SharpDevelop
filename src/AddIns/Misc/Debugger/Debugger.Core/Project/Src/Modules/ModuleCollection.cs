// <file>
//     <owner name="David Srbecký" email="dsrbecky@post.cz"/>
// </file>

using System;
using System.Collections;

using DebuggerInterop.Core;

namespace DebuggerLibrary
{
	public class ModuleCollection: ReadOnlyCollectionBase
	{
		int lastAssignedOrderOfLoading= 0;

		public event ModuleEventHandler ModuleAdded;

		private void OnModuleAdded(Module module)
		{
			if (ModuleAdded != null)
				ModuleAdded(this, new ModuleEventArgs(module));
		}


		public event ModuleEventHandler ModuleRemoved;

		private void OnModuleRemoved(Module module)
		{
			if (ModuleRemoved != null)
				ModuleRemoved(this, new ModuleEventArgs(module));
		}


		public Module this[int index] {
			get  {
				return (Module) InnerList[index];
			}
		}

		public Module this[string filename] {
			get {
				foreach(Module module in InnerList)
					if (module.Filename == filename)
						return module;

				throw new UnableToGetPropertyException(this, "this[string]", "Module \"" + filename + "\" is not in collection");
			}
		}

		internal Module this[ICorDebugModule corModule]  
		{
			get  
			{
				foreach(Module module in InnerList)
					if (module.CorModule == corModule)
						return module;

				throw new UnableToGetPropertyException(this, "this[ICorDebugModule]", "Module is not in collection");
			}
		}

		internal void Clear()
		{
			foreach (Module m in InnerList) {
 				OnModuleRemoved(m);
			}
			InnerList.Clear();
			lastAssignedOrderOfLoading = 0;
		}

		internal void Add(Module module)
		{
			System.Diagnostics.Trace.Assert(module != null);
			if (module != null)
			{
				module.OrderOfLoading = lastAssignedOrderOfLoading;
				lastAssignedOrderOfLoading++;
				InnerList.Add(module);
				OnModuleAdded(module);
			}
		}

		internal void Add(ICorDebugModule corModule)
		{
			System.Diagnostics.Trace.Assert(corModule != null);
			if (corModule != null)
				Add(new Module(corModule));
		}

		internal void Remove(Module module)
		{
			InnerList.Remove(module);
			OnModuleRemoved (module);
		}

		internal void Remove(ICorDebugModule corModule)
		{
			Remove(this[corModule]);
		}
	}
}
