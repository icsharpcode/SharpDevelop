// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the BSD license (for details please see \src\AddIns\Debugger\Debugger.AddIn\license.txt)

using System;
using System.Threading;
using System.Windows.Forms;
using Debugger;
using Debugger.AddIn.TreeModel;
using ICSharpCode.Core;
using Exception=System.Exception;
using Thread=Debugger.Thread;

namespace ICSharpCode.SharpDevelop.Gui.Pads
{
	public partial class RunningThreadsPad : DebuggerPad
	{
		ListView  runningThreadsList;
		Process debuggedProcess;
		
		ColumnHeader id          = new ColumnHeader();
		ColumnHeader name        = new ColumnHeader();
		ColumnHeader location    = new ColumnHeader();
		ColumnHeader priority    = new ColumnHeader();
		ColumnHeader breaked     = new ColumnHeader();
		
		public override object Control {
			get {
				return runningThreadsList;
			}
		}
			
		protected override void InitializeComponents()
		{
			runningThreadsList = new ListView();
			runningThreadsList.FullRowSelect = true;
			runningThreadsList.AutoArrange = true;
			runningThreadsList.Alignment   = ListViewAlignment.Left;
			runningThreadsList.View = View.Details;
			runningThreadsList.Dock = DockStyle.Fill;
			runningThreadsList.GridLines  = false;
			runningThreadsList.Activation = ItemActivation.OneClick;
			runningThreadsList.Columns.AddRange(new ColumnHeader[] {id, name, location, priority, breaked} );
			runningThreadsList.ContextMenuStrip = CreateContextMenuStrip();
			runningThreadsList.ItemActivate += new EventHandler(RunningThreadsListItemActivate);
			id.Width = 100;
			name.Width = 300;
			location.Width = 250;
			priority.Width = 120;
			breaked.Width = 80;
			
			RedrawContent();
			ResourceService.LanguageChanged += delegate { RedrawContent(); };
		}
		
		public void RedrawContent()
		{
			id.Text       = ResourceService.GetString("Global.ID");
			name.Text     = ResourceService.GetString("Global.Name");
			location.Text = ResourceService.GetString("AddIns.HtmlHelp2.Location");
			priority.Text = ResourceService.GetString("MainWindow.Windows.Debug.Threads.Priority");
			breaked.Text  = ResourceService.GetString("MainWindow.Windows.Debug.Threads.Frozen");
		}
		

		protected override void SelectProcess(Process process)
		{
			if (debuggedProcess != null) {
				debuggedProcess.Paused               -= debuggedProcess_Paused;
				debuggedProcess.Threads.Added        -= debuggedProcess_ThreadStarted;
			}
			debuggedProcess = process;
			if (debuggedProcess != null) {
				debuggedProcess.Paused               += debuggedProcess_Paused;
				debuggedProcess.Threads.Added        += debuggedProcess_ThreadStarted;
			}
			runningThreadsList.Items.Clear();
			RefreshPad();
		}
		
		void debuggedProcess_Paused(object sender, ProcessEventArgs e)
		{
			RefreshPad();
		}
		
		void debuggedProcess_ThreadStarted(object sender, CollectionItemEventArgs<Thread> e)
		{
			AddThread(e.Item);
		}
		
		public override void RefreshPad()
		{
			if (debuggedProcess == null || debuggedProcess.IsRunning) {
				runningThreadsList.Items.Clear();
				return;
			}
			
			using(new PrintTimes("Threads refresh")) {
				try {
					foreach (Thread t in debuggedProcess.Threads) {
						if (debuggedProcess.IsPaused) {
							Utils.DoEvents(debuggedProcess);
						}
						RefreshThread(t);
					}
				} catch(AbortedBecauseDebuggeeResumedException) {
				} catch(Exception) {
					if (debuggedProcess == null || debuggedProcess.HasExited) {
						// Process unexpectedly exited
					} else {
						throw;
					}
				}
			}
		}

		void RunningThreadsListItemActivate(object sender, EventArgs e)
		{
			if (debuggedProcess.IsPaused) {
				if (debuggedProcess != null) {
					debuggedProcess.SelectedThread = (Thread)(runningThreadsList.SelectedItems[0].Tag);
					debuggedProcess.OnPaused(); // Force refresh of pads
				}
			} else {
				MessageService.ShowMessage("${res:MainWindow.Windows.Debug.Threads.CannotSwitchWhileRunning}", "${res:MainWindow.Windows.Debug.Threads.ThreadSwitch}");
			}
		}		
		
		void AddThread(Thread thread)
		{
			runningThreadsList.Items.Add(new ListViewItem(thread.ID.ToString()));
			RefreshThread(thread);
			thread.NameChanged += delegate {
				RefreshThread(thread);
			};
			thread.Exited += delegate {
				RemoveThread(thread);
			};
		}
		
		ListViewItem FindItem(Thread thread)
		{
			foreach (ListViewItem item in runningThreadsList.Items) {
				if (item.Text == thread.ID.ToString()) {
					return item;
				}
			}
			return null;
		}
		
		void RefreshThread(Thread thread)
		{
			ListViewItem item = FindItem(thread);
			if (item == null) {
				AddThread(thread);
				return;
			}
			item.SubItems.Clear();
			item.Text = thread.ID.ToString();
			item.Tag = thread;
			item.SubItems.Add(thread.Name);
			StackFrame location = null;
			if (thread.Process.IsPaused) {
				location = thread.MostRecentStackFrame;
			}
			if (location != null) {
				item.SubItems.Add(location.MethodInfo.Name);
			} else {
				item.SubItems.Add(ResourceService.GetString("Global.NA"));
			}
			switch (thread.Priority) {
				case ThreadPriority.Highest:
					item.SubItems.Add(ResourceService.GetString("MainWindow.Windows.Debug.Threads.Priority.Highest"));
					break;
				case ThreadPriority.AboveNormal:
					item.SubItems.Add(ResourceService.GetString("MainWindow.Windows.Debug.Threads.Priority.AboveNormal"));
					break;
				case ThreadPriority.Normal:
					item.SubItems.Add(ResourceService.GetString("MainWindow.Windows.Debug.Threads.Priority.Normal"));
					break;
				case ThreadPriority.BelowNormal:
					item.SubItems.Add(ResourceService.GetString("MainWindow.Windows.Debug.Threads.Priority.BelowNormal"));
					break;
				case ThreadPriority.Lowest:
					item.SubItems.Add(ResourceService.GetString("MainWindow.Windows.Debug.Threads.Priority.Lowest"));
					break;
				default:
					item.SubItems.Add(thread.Priority.ToString());
					break;
			}
			item.SubItems.Add(ResourceService.GetString(thread.Suspended ? "Global.Yes" : "Global.No"));
		}
		
		void RemoveThread(Thread thread)
		{
			foreach (ListViewItem item in runningThreadsList.Items) {
				if (thread.ID.ToString() == item.Text) {
					item.Remove();
				}
			}
		}
	}
}
