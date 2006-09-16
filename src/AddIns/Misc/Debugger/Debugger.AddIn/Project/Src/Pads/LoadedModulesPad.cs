// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Windows.Forms;
using System.Drawing;
using System.CodeDom.Compiler;
using System.Collections;
using System.IO;
using System.Diagnostics;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Services;

using Debugger;

namespace ICSharpCode.SharpDevelop.Gui.Pads
{
	public class LoadedModulesPad : DebuggerPad
	{
		ListView  loadedModulesList;
		Debugger.Process debuggedProcess;
		
		ColumnHeader name        = new ColumnHeader();
		ColumnHeader address     = new ColumnHeader();
		ColumnHeader path        = new ColumnHeader();
		ColumnHeader order       = new ColumnHeader();
		ColumnHeader version     = new ColumnHeader();
		ColumnHeader program     = new ColumnHeader();
		ColumnHeader timestamp   = new ColumnHeader();
		ColumnHeader information = new ColumnHeader();
			
		
		public override Control Control {
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
		}
		
		public override void RedrawContent()
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
		
		
		protected override void SelectProcess(Debugger.Process process)
		{
			if (debuggedProcess != null) {
				debuggedProcess.ModuleLoaded -= debuggedProcess_ModuleLoaded;
				debuggedProcess.ModuleUnloaded -= debuggedProcess_ModuleUnloaded;
			}
			debuggedProcess = process;
			if (debuggedProcess != null) {
				debuggedProcess.ModuleLoaded += debuggedProcess_ModuleLoaded;
				debuggedProcess.ModuleUnloaded += debuggedProcess_ModuleUnloaded;
			}
			RefreshPad();
		}
		
		void debuggedProcess_ModuleLoaded(object sender, ModuleEventArgs e)
		{
			AddModule(e.Module);
		}
		
		void debuggedProcess_ModuleUnloaded(object sender, ModuleEventArgs e)
		{
			RemoveModule(e.Module);
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
			ListViewItem newItem = new ListViewItem(new string[] {module.Filename,
			                                                      String.Format("{0:X8}", module.BaseAdress),
			                                                      module.DirectoryName,
			                                                      module.OrderOfLoading.ToString(),
			                                                      "",
			                                                      "",
			                                                      "",
			                                                      StringParser.Parse(module.SymbolsLoaded ? "${res:MainWindow.Windows.Debug.Modules.HasSymbols}" : "${res:MainWindow.Windows.Debug.Modules.HasNoSymbols}")
			                                                     });
			newItem.Tag = module;
			loadedModulesList.Items.Add(newItem);
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
