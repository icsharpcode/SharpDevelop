// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>
#region License
//  
//  Copyright (c) 2007, ic#code
//  
//  All rights reserved.
//  
//  Redistribution  and  use  in  source  and  binary  forms,  with  or without
//  modification, are permitted provided that the following conditions are met:
//  
//  1. Redistributions  of  source code must retain the above copyright notice,
//     this list of conditions and the following disclaimer.
//  
//  2. Redistributions  in  binary  form  must  reproduce  the  above copyright
//     notice,  this  list  of  conditions  and the following disclaimer in the
//     documentation and/or other materials provided with the distribution.
//  
//  3. Neither the name of the ic#code nor the names of its contributors may be
//     used  to  endorse or promote products derived from this software without
//     specific prior written permission.
//  
//  THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS"
//  AND  ANY  EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE
//  IMPLIED  WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE
//  ARE  DISCLAIMED.   IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE
//  LIABLE  FOR  ANY  DIRECT,  INDIRECT,  INCIDENTAL,  SPECIAL,  EXEMPLARY,  OR
//  CONSEQUENTIAL  DAMAGES  (INCLUDING,  BUT  NOT  LIMITED  TO,  PROCUREMENT OF
//  SUBSTITUTE  GOODS  OR  SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS
//  INTERRUPTION)  HOWEVER  CAUSED  AND  ON ANY THEORY OF LIABILITY, WHETHER IN
//  CONTRACT,  STRICT  LIABILITY,  OR  TORT (INCLUDING NEGLIGENCE OR OTHERWISE)
//  ARISING  IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE
//  POSSIBILITY OF SUCH DAMAGE.
//  
#endregion

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
		
		
		protected override void SelectProcess(Process process)
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
			ListViewItem newItem = new ListViewItem();
			newItem.Tag = module;
			RefreshItem(newItem);
			module.SymbolsLoaded += delegate { RefreshItem(newItem); };
			loadedModulesList.Items.Add(newItem);
		}
		
		void RefreshItem(ListViewItem item)
		{
			Module module = (Module)item.Tag;
			item.SubItems.Clear();
			item.SubItems.AddRange(
				new string[] {
					GetModuleFileNameOrName(module),
					String.Format("{0:X8}", module.BaseAdress),
					module.DirectoryName,
					module.OrderOfLoading.ToString(),
					"",
					"",
					"",
					StringParser.Parse(module.HasSymbols ? "${res:MainWindow.Windows.Debug.Modules.HasSymbols}" : "${res:MainWindow.Windows.Debug.Modules.HasNoSymbols}")
				}
			);
			item.SubItems.RemoveAt(0);
		}
		
		string GetModuleFileNameOrName(Module module)
		{
			string fileName = module.Filename;
			if (String.IsNullOrEmpty(fileName)) {
				return module.CorModule.Name;
			}
			return module.Filename;
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
