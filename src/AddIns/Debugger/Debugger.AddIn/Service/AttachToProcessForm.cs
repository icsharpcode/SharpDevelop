// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the BSD license (for details please see \src\AddIns\Debugger\Debugger.AddIn\license.txt)

using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Forms;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Debugging;
using ICSharpCode.SharpDevelop.Gui;

namespace ICSharpCode.SharpDevelop.Services
{
	public class AttachToProcessForm : AbstractAttachToProcessForm
	{
		class ProcessListViewItem : ListViewItem
		{
			Process process;
			bool managed;
			
			public ProcessListViewItem(Process process, WindowsDebugger debugger)
			{
				this.process = process;
				try {
					var modules = process.Modules.Cast<ProcessModule>().Where(
						m => m.ModuleName.StartsWith("mscor", StringComparison.InvariantCultureIgnoreCase));

					managed = modules.Count() > 0;
				} catch { }
				
				string fileName = Path.GetFileName(process.MainModule.FileName);
				Text = fileName;
				SubItems.Add(process.Id.ToString());
				SubItems.Add(process.MainWindowTitle);
				SubItems.Add(GetManagedString(managed));
			}
			
			public Process Process {
				get { return process; }
			}
			
			public bool IsManaged {
				get { return managed; }
			}
			
			static string GetManagedString(bool managed)
			{
				if (managed) {
					return StringParser.Parse("${res:ICSharpCode.SharpDevelop.Gui.Dialogs.AttachToProcessForm.Managed}");
				}
				return String.Empty;
			}
		}
		
		protected override void RefreshProcessList(ListView listView, bool showNonManaged)
		{
			listView.Items.Clear();
			WindowsDebugger debugger = (WindowsDebugger)DebuggerService.CurrentDebugger;
			Process currentProcess = Process.GetCurrentProcess();
			foreach (Process process in Process.GetProcesses()) {
				try {
					if (process.HasExited) continue;
					// Prevent attaching to our own process.
					if (currentProcess.Id != process.Id) {
						ProcessListViewItem item = new ProcessListViewItem(process, debugger);
						if (showNonManaged || item.IsManaged) {
							item.Tag = process;
							listView.Items.Add(item);
						}
					}
				} catch (Win32Exception) {
					// Do nothing.
				}
			}
			listView.Sort();
		}
	}
}
