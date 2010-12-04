// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the BSD license (for details please see \src\AddIns\Debugger\Debugger.AddIn\license.txt)

using System;
using System.Dynamic;
using System.Windows;
using System.Windows.Data;

using Debugger;
using Debugger.AddIn.Pads.Controls;
using ICSharpCode.Core;

namespace ICSharpCode.SharpDevelop.Gui.Pads
{
	public class LoadedModulesPad : DebuggerPad
	{
		SimpleListViewControl  loadedModulesList;
		Process debuggedProcess;
		
		protected override void InitializeComponents()
		{
			loadedModulesList = new SimpleListViewControl();
			panel.Children.Add(loadedModulesList);
			RedrawContent();
			ResourceService.LanguageChanged += delegate { RedrawContent(); };
		}
		
		public void RedrawContent()
		{
			loadedModulesList.ClearColumns();
			loadedModulesList.AddColumn(StringParser.Parse("${res:Global.Name}"),
			                            new Binding { Path = new PropertyPath("Name") }, 250);
			loadedModulesList.AddColumn(StringParser.Parse("${res:MainWindow.Windows.Debug.Modules.AddressColumn}"),
			                            new Binding { Path = new PropertyPath("Address") }, 100);
			loadedModulesList.AddColumn(StringParser.Parse("${res:Global.Path}"),
			                            new Binding { Path = new PropertyPath("Path") }, 250);
			loadedModulesList.AddColumn(StringParser.Parse("${res:MainWindow.Windows.Debug.Modules.OrderColumn}"),
			                            new Binding { Path = new PropertyPath("Order") }, 80);
			loadedModulesList.AddColumn(StringParser.Parse("${res:MainWindow.Windows.Debug.Modules.SymbolsColumn}"),
			                            new Binding { Path = new PropertyPath("Symbols") }, 130);
		}
		
		protected override void SelectProcess(Process process)
		{
			if (debuggedProcess != null) {
				debuggedProcess.Modules.Added -= debuggedProcess_ModuleLoaded;
				debuggedProcess.Modules.Removed -= debuggedProcess_ModuleUnloaded;
			}
			debuggedProcess = process;
			if (debuggedProcess != null) {
				debuggedProcess.Modules.Added += debuggedProcess_ModuleLoaded;
				debuggedProcess.Modules.Removed += debuggedProcess_ModuleUnloaded;
			}
			RefreshPad();
		}
		
		void debuggedProcess_ModuleLoaded(object sender, CollectionItemEventArgs<Module> e)
		{
			AddModule(e.Item);
		}
		
		void debuggedProcess_ModuleUnloaded(object sender, CollectionItemEventArgs<Module> e)
		{
			RemoveModule(e.Item);
		}
		
		public override void RefreshPad()
		{
			loadedModulesList.ItemCollection.Clear();
			if (debuggedProcess != null) {
				foreach(Module module in debuggedProcess.Modules) {
					AddModule(module);
				}
			}
		}
		
		void AddModule(Module module)
		{
			dynamic obj = new ExpandoObject();
			obj.Tag = module;
			RefreshItem(obj);
			module.SymbolsUpdated += delegate { RefreshItem(obj); };
			loadedModulesList.ItemCollection.Add(obj);
		}
		
		void RefreshItem(ExpandoObject obj)
		{
			dynamic item = obj;
			Module module = (Module)item.Tag;
			item.Name = module.Name;
			item.Address = String.Format("{0:X8}", module.BaseAdress);
			item.Path = module.IsDynamic ? "(dynamic)" : module.IsInMemory ? "(in memory)" : module.FullPath;
			item.Order = module.OrderOfLoading.ToString();
			item.Symbols = StringParser.Parse(module.HasSymbols ? "${res:MainWindow.Windows.Debug.Modules.HasSymbols}" : "${res:MainWindow.Windows.Debug.Modules.HasNoSymbols}");
		}

		void RemoveModule(Module module)
		{
			foreach (dynamic item in loadedModulesList.ItemCollection) {
				if (item.Tag == module) {
					loadedModulesList.ItemCollection.Remove(item);
					break;
				}
			}
		}
	}
}