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
					managed = process.Modules
						.OfType<ProcessModule>()
						.Any(m => m.ModuleName.StartsWith("mscor", StringComparison.InvariantCultureIgnoreCase));
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
			WindowsDebugger debugger = (WindowsDebugger)SD.Debugger;
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
