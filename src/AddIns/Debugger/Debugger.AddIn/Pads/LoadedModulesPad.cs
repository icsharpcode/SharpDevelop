// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the BSD license (for details please see \src\AddIns\Debugger\Debugger.AddIn\license.txt)

using System;
using System.Windows.Forms;
using Debugger;
using ICSharpCode.Core;

namespace ICSharpCode.SharpDevelop.Gui.Pads
{
	public class LoadedModulesPad : DebuggerPad
	{
		ListView  loadedModulesList;
		Process debuggedProcess;
		
		ColumnHeader name        = new ColumnHeader();
		ColumnHeader address     = new ColumnHeader();
		ColumnHeader path        = new ColumnHeader();
		ColumnHeader order       = new ColumnHeader();
		ColumnHeader version     = new ColumnHeader();
		ColumnHeader program     = new ColumnHeader();
		ColumnHeader timestamp   = new ColumnHeader();
		ColumnHeader information = new ColumnHeader();
			
		
		public override object Control {
			get {
				return loadedModulesList;
			}
		}

		protected override void InitializeComponents()
		{
			loadedModulesList = new ListView();
			loadedModulesList.FullRowSelect = true;
			loadedModulesList.AutoArrange = true;
			loadedModulesList.Alignment   = ListViewAlignment.Left;
			loadedModulesList.View = View.Details;
			loadedModulesList.Dock = DockStyle.Fill;
			loadedModulesList.GridLines  = false;
			loadedModulesList.Activation = ItemActivation.OneClick;
			loadedModulesList.Columns.AddRange(new ColumnHeader[] {name, address, path, order, version, program, timestamp, information} );
			name.Width = 250;
			address.Width = 100;
			path.Width = 250;
			order.Width = 80;
			version.Width = 0;//50;
			program.Width = 0;//90;
			timestamp.Width = 0;//80;
			information.Width = 130;

			RedrawContent();
			ResourceService.LanguageChanged += delegate { RedrawContent(); };
		}
		
		public void RedrawContent()
		{
			name.Text         = StringParser.Parse("${res:Global.Name}");
			address.Text      = StringParser.Parse("${res:MainWindow.Windows.Debug.Modules.AddressColumn}");
			path.Text         = StringParser.Parse("${res:Global.Path}");
			order.Text        = StringParser.Parse("${res:MainWindow.Windows.Debug.Modules.OrderColumn}");
			version.Text      = StringParser.Parse("${res:MainWindow.Windows.Debug.Modules.VersionColumn}");
			program.Text      = StringParser.Parse("${res:MainWindow.Windows.Debug.Modules.ProgramColumn}");
			timestamp.Text    = StringParser.Parse("${res:MainWindow.Windows.Debug.Modules.TimestampColumn}");
			information.Text  = StringParser.Parse("${res:MainWindow.Windows.Debug.Modules.SymbolsColumn}");
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
			loadedModulesList.Items.Clear();
			if (debuggedProcess != null) {
				foreach(Module module in debuggedProcess.Modules) {
					AddModule(module);
				}
			}
		}
		
		void AddModule(Module module)
		{
			ListViewItem newItem = new ListViewItem();
			newItem.Tag = module;
			RefreshItem(newItem);
			module.SymbolsUpdated += delegate { RefreshItem(newItem); };
			loadedModulesList.Items.Add(newItem);
		}
		
		void RefreshItem(ListViewItem item)
		{
			Module module = (Module)item.Tag;
			item.SubItems.Clear();
			item.SubItems.AddRange(
				new string[] {
					module.Name,
					String.Format("{0:X8}", module.BaseAdress),
					module.IsDynamic ? "(dynamic)" : module.IsInMemory ? "(in memory)" : module.FullPath,
					module.OrderOfLoading.ToString(),
					"",
					"",
					"",
					StringParser.Parse(module.HasSymbols ? "${res:MainWindow.Windows.Debug.Modules.HasSymbols}" : "${res:MainWindow.Windows.Debug.Modules.HasNoSymbols}")
				}
			);
			item.SubItems.RemoveAt(0);
		}

		void RemoveModule(Module module)
		{
			foreach (ListViewItem item in loadedModulesList.Items) {
				if (item.Tag == module) {
					item.Remove();
				}
			}
		}
	}
}
