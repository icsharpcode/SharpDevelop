// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the BSD license (for details please see \src\AddIns\Debugger\Debugger.AddIn\license.txt)

using System;
using System.Collections.ObjectModel;
using System.Dynamic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Debugger;
using Debugger.AddIn.Pads.Controls;
using Debugger.AddIn.Pads.ParallelPad;
using ICSharpCode.Core;

namespace ICSharpCode.SharpDevelop.Gui.Pads
{
	public class LoadedModulesPad : DebuggerPad
	{
		ListView loadedModulesList;
		Process debuggedProcess;
		ObservableCollection<ModuleModel> loadedModules;
		
		protected override void InitializeComponents()
		{
			loadedModulesList = new ListView();
			loadedModules = new ObservableCollection<ModuleModel>();
			loadedModulesList.ItemsSource = loadedModules;
			loadedModulesList.View = new GridView();
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
			InvalidatePad();
		}
		
		void debuggedProcess_ModuleLoaded(object sender, CollectionItemEventArgs<Module> e)
		{
			AddModule(e.Item);
		}
		
		void debuggedProcess_ModuleUnloaded(object sender, CollectionItemEventArgs<Module> e)
		{
			RemoveModule(e.Item);
		}
		
		protected override void RefreshPad()
		{
			loadedModules.Clear();
			if (debuggedProcess != null) {
				foreach(Module module in debuggedProcess.Modules) {
					AddModule(module);
				}
			}
		}
		
		void AddModule(Module module)
		{
			loadedModules.Add(new ModuleModel(module));
		}

		void RemoveModule(Module module)
		{
			loadedModules.RemoveWhere(model => model.Module == module);
		}
	}
	
	static class ListViewExtensions
	{
		public static void ClearColumns(this ListView view)
		{
			if (view == null)
				throw new ArgumentNullException("view");
			if (view.View is GridView)
				((GridView)view.View).Columns.Clear();
		}
		
		public static void AddColumn(this ListView view, string header, Binding binding, double width)
		{
			if (view == null)
				throw new ArgumentNullException("view");
			if (view.View is GridView) {
				GridViewColumn column = new GridViewColumn {
					Width = width,
					DisplayMemberBinding = binding,
					Header = header };
				((GridView)view.View).Columns.Add(column);
			}
		}
	}
}