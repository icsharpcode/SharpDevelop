// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Dynamic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

using Debugger;
using ICSharpCode.Core.Presentation;
using Debugger.AddIn.Pads.Controls;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Services;
using ICSharpCode.SharpDevelop.Workbench;

namespace ICSharpCode.SharpDevelop.Gui.Pads
{
	public class LoadedModulesPad : AbstractPadContent
	{
		ListView listView;
		
		public override object Control {
			get { return listView; }
		}
		
		public LoadedModulesPad()
		{
			var res = new CommonResources();
			res.InitializeComponent();
			
			listView = new ListView();
			listView.View = (GridView)res["loadedModulesGridView"];
			listView.SetValue(GridViewColumnAutoSize.AutoWidthProperty, "50%;70;50%;35;120");
			
			WindowsDebugger.RefreshingPads += RefreshPad;
			RefreshPad();
		}
		
		void RefreshPad()
		{
			Process process = WindowsDebugger.CurrentProcess;
			List<ModuleItem> loadedModules = new List<ModuleItem>();
			if (process != null) {
				foreach(Module module in process.Modules) {
					loadedModules.Add(new ModuleItem(module));
				}
			}
			listView.ItemsSource = loadedModules;
		}
	}
	
	public class ModuleItem
	{
		public string Name { get; private set; }
		public string Address { get; private set; }
		public string Path { get; private set; }
		public string Order { get; private set; }
		public string Symbols { get; private set; }
		
		public ModuleItem(Module module)
		{
			this.Name = module.Name;
			this.Address = string.Format("{0:X8}", module.BaseAdress);
			if (module.IsDynamic) {
				this.Path = StringParser.Parse("${res:MainWindow.Windows.Debug.Modules.DynamicModule}");
			} else if (module.IsInMemory) {
				this.Path = StringParser.Parse("${res:MainWindow.Windows.Debug.Modules.InMemoryModule}");
			} else {
				this.Path = module.FullPath;
			}
			this.Order = module.OrderOfLoading.ToString();
			this.Symbols = module.HasSymbols ? StringParser.Parse("${res:MainWindow.Windows.Debug.Modules.HasSymbols}") : StringParser.Parse("${res:MainWindow.Windows.Debug.Modules.HasNoSymbols}");
		}
	}
}
