// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the BSD license (for details please see \src\AddIns\Debugger\Debugger.AddIn\license.txt)

using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Threading;
using Debugger;
using Debugger.AddIn.Pads.Controls;
using Debugger.AddIn.Pads.ParallelPad;
using Debugger.AddIn.TreeModel;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Debugging;
using ICSharpCode.SharpDevelop.Services;
using Exception = System.Exception;
using Thread = Debugger.Thread;

namespace ICSharpCode.SharpDevelop.Gui.Pads
{
	public partial class RunningThreadsPad : AbstractPadContent
	{
		DockPanel panel;
		ListView runningThreadsList;
		ObservableCollection<ThreadModel> runningThreads;
		
		public override object Control {
			get { return panel; }
		}
		
		public RunningThreadsPad()
		{
			this.panel = new DockPanel();
			runningThreads = new ObservableCollection<ThreadModel>();
			runningThreadsList = new ListView();
			runningThreadsList.ContextMenu = CreateContextMenuStrip();
			runningThreadsList.MouseDoubleClick += RunningThreadsListItemActivate;
			runningThreadsList.ItemsSource = runningThreads;
			runningThreadsList.View = new GridView();
			panel.Children.Add(runningThreadsList);
			
			RedrawContent();
			ResourceService.LanguageChanged += delegate { RedrawContent(); };
			
			WindowsDebugger.RefreshingPads += RefreshPad;
			WindowsDebugger.RefreshPads();
		}
		
		public void RedrawContent()
		{
			runningThreadsList.ClearColumns();
			runningThreadsList.AddColumn(ResourceService.GetString("Global.ID"),
			                             new Binding { Path = new PropertyPath("ID") },
			                             100);
			runningThreadsList.AddColumn(ResourceService.GetString("Global.Name"),
			                             new Binding { Path = new PropertyPath("Name") },
			                             300);
			runningThreadsList.AddColumn(ResourceService.GetString("AddIns.HtmlHelp2.Location"),
			                             new Binding { Path = new PropertyPath("Location") },
			                             250);
			runningThreadsList.AddColumn(ResourceService.GetString("MainWindow.Windows.Debug.Threads.Priority"),
			                             new Binding { Path = new PropertyPath("Priority") },
			                             120);
			runningThreadsList.AddColumn(ResourceService.GetString("MainWindow.Windows.Debug.Threads.Frozen"),
			                             new Binding { Path = new PropertyPath("Frozen") },
			                             80);
		}

		protected void RefreshPad(object sender, DebuggerEventArgs dbg)
		{
			Process debuggedProcess = dbg.Process;
			
			if (debuggedProcess == null || debuggedProcess.IsRunning) {
				runningThreads.Clear();
				return;
			}
			
			LoggingService.Info("Threads refresh");
			
			debuggedProcess.EnqueueForEach(
				Dispatcher.CurrentDispatcher,
				debuggedProcess.Threads.ToList(),
				t => AddThread(t)
			);
		}

		void RunningThreadsListItemActivate(object sender, EventArgs e)
		{
			ThreadModel obj = runningThreadsList.SelectedItems[0] as ThreadModel;
			Thread thread = obj.Thread;
			Process debuggedProcess = thread.Process;
			
			if (debuggedProcess != null) {
				if (debuggedProcess.IsPaused) {
					
					// check for options - if these options are enabled, selecting the frame should not continue
					if ((thread.MostRecentStackFrame == null || !thread.MostRecentStackFrame.HasSymbols) &&
					    !DebuggingOptions.Instance.DecompileCodeWithoutSymbols) {
						MessageService.ShowMessage("${res:MainWindow.Windows.Debug.Threads.CannotSwitchWithoutDecompiledCodeOptions}",
						                           "${res:MainWindow.Windows.Debug.Threads.ThreadSwitch}");
						return;
					}
					
					debuggedProcess.SelectedThread = thread;
					debuggedProcess.SelectedThread.SelectedStackFrame = debuggedProcess.SelectedThread.MostRecentStackFrame;
					if (debuggedProcess.SelectedThread.SelectedStackFrame != null) {
						WindowsDebugger.RefreshPads();
					} else {
						MessageService.ShowMessage("${res:MainWindow.Windows.Debug.Threads.CannotSwitchOnNAFrame}", "${res:MainWindow.Windows.Debug.Threads.ThreadSwitch}");
					}
			} else {
				MessageService.ShowMessage("${res:MainWindow.Windows.Debug.Threads.CannotSwitchWhileRunning}", "${res:MainWindow.Windows.Debug.Threads.ThreadSwitch}");
			}
		}
		}
		
		void AddThread(Thread thread)
		{
			if (thread == null) return;
			
			// remove the object if exists
			RemoveThread(thread);
			
			ThreadModel obj = new ThreadModel(thread);
			
			runningThreads.Add(obj);
		}
		
		void RemoveThread(Thread thread)
		{
			if (thread == null)
				return;
			
			runningThreads.RemoveWhere(model => model.Thread == thread);
		}
	}
}