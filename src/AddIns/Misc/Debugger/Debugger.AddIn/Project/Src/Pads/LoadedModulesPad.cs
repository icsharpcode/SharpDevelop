// <file>
//     <owner name="David Srbecký" email="dsrbecky@post.cz"/>
//	   <owner name="Mike Krueger" email="mike@icsharpcode.net"/>
// </file>

using System;
using System.Windows.Forms;
using System.Drawing;
using System.CodeDom.Compiler;
using System.Collections;
using System.IO;
using System.Diagnostics;
//using ICSharpCode.Core.Services;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Services;
//using ICSharpCode.Core.Properties;

using DebuggerLibrary;

namespace ICSharpCode.SharpDevelop.Gui.Pads
{
	public class LoadedModulesPad : AbstractPadContent
	{
		ListView  loadedModulesList;
		
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
		
		public LoadedModulesPad() //: base("${res:MainWindow.Windows.Debug.Modules}", null)
		{
			InitializeComponents();
		}
		
		void InitializeComponents()
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

			NDebugger.Instance.ModuleLoaded += new DebuggerLibrary.ModuleEventHandler(AddModule);
			NDebugger.Instance.ModuleUnloaded += new DebuggerLibrary.ModuleEventHandler(RemoveModule);

			RedrawContent();
		}
		
		public override void RedrawContent()
		{
			
			name.Text         = StringParser.Parse("${res:MainWindow.Windows.Debug.NameColumn}");
			address.Text      = StringParser.Parse("${res:MainWindow.Windows.Debug.AddressColumn}");
			path.Text         = StringParser.Parse("${res:MainWindow.Windows.Debug.PathColumn}");
			order.Text        = StringParser.Parse("${res:MainWindow.Windows.Debug.OrderColumn}");
			version.Text      = StringParser.Parse("${res:MainWindow.Windows.Debug.VersionColumn}");
			program.Text      = StringParser.Parse("${res:MainWindow.Windows.Debug.ProgramColumn}");
			timestamp.Text    = StringParser.Parse("${res:MainWindow.Windows.Debug.TimestampColumn}");
			information.Text  = StringParser.Parse("${res:MainWindow.Windows.Debug.InformationColumn}");

            loadedModulesList.Items.Clear();
            foreach(Module m in NDebugger.Instance.Modules) {
                AddModule(this, new ModuleEventArgs(m));
            }
		}

		void AddModule(object sender, ModuleEventArgs e) 
		{
			ListViewItem newItem = new ListViewItem(new string[] {e.Module.Filename,
			                                                      String.Format("{0:X8}", e.Module.BaseAdress),
			                                                      e.Module.DirectoryName,
			                                                      e.Module.OrderOfLoading.ToString(),
			                                                      "",
			                                                      "",
			                                                      "",
			                                                      e.Module.SymbolsLoaded.ToString(), //StringParser.Parse(m.symbolsLoaded ? "${res:MainWindow.Windows.Debug.HasSymbols}" : "${res:MainWindow.Windows.Debug.HasNoSymbols}")
			                                                     });
			newItem.Tag = e.Module;
			loadedModulesList.Items.Add(newItem);
		}

		void RemoveModule(object sender, ModuleEventArgs e) 
		{
			foreach (ListViewItem item in loadedModulesList.Items) {
				if (item.Tag == e.Module) {
					item.Remove();
				}
			}
		}
	}
}
