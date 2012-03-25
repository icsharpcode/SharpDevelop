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
using ICSharpCode.SharpDevelop.Services;

namespace ICSharpCode.SharpDevelop.Gui.Pads
{
	public class LoadedModulesPad : AbstractPadContent
	{
		DockPanel panel;
		ListView loadedModulesList;
		ObservableCollection<ModuleModel> loadedModules;
		
		public override object Control {
			get { return panel; }
		}
		
		public LoadedModulesPad()
		{
			this.panel = new DockPanel();
			loadedModulesList = new ListView();
			loadedModules = new ObservableCollection<ModuleModel>();
			loadedModulesList.ItemsSource = loadedModules;
			loadedModulesList.View = new GridView();
			panel.Children.Add(loadedModulesList);
			RedrawContent();
			ResourceService.LanguageChanged += delegate { RedrawContent(); };
			
			WindowsDebugger.RefreshingPads += RefreshPad;
			RefreshPad();
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
		
		void RefreshPad()
		{
			Process process = WindowsDebugger.CurrentProcess;
			loadedModules.Clear();
			if (process != null) {
				foreach(Module module in process.Modules) {
					loadedModules.Add(new ModuleModel(module));
				}
			}
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