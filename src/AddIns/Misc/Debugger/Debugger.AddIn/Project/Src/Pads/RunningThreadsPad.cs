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
using System.ComponentModel;
using System.IO;
using System.Diagnostics;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Services;

using Debugger;

namespace ICSharpCode.SharpDevelop.Gui.Pads
{
	public partial class RunningThreadsPad : DebuggerPad
	{
		ListView  runningThreadsList;
		Debugger.Process debuggedProcess;
		
		ColumnHeader id          = new ColumnHeader();
		ColumnHeader name        = new ColumnHeader();
		ColumnHeader location    = new ColumnHeader();
		ColumnHeader priority    = new ColumnHeader();
		ColumnHeader breaked     = new ColumnHeader();
		
		public override Control Control {
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
		}
		
		public override void RedrawContent()
		{
			id.Text       = ResourceService.GetString("Global.ID");
			name.Text     = ResourceService.GetString("Global.Name");
			location.Text = ResourceService.GetString("AddIns.HtmlHelp2.Location");
			priority.Text = ResourceService.GetString("MainWindow.Windows.Debug.Threads.Priority");
			breaked.Text  = ResourceService.GetString("MainWindow.Windows.Debug.Threads.Frozen");
		}
		

		protected override void SelectProcess(Debugger.Process process)
		{
			if (debuggedProcess != null) {
				debuggedProcess.DebuggeeStateChanged -= debuggedProcess_DebuggeeStateChanged;
				debuggedProcess.ThreadStarted        -= debuggedProcess_ThreadStarted;
				debuggedProcess.ThreadStateChanged   -= debuggedProcess_ThreadStateChanged;
				debuggedProcess.ThreadExited         -= debuggedProcess_ThreadExited;
			}
			debuggedProcess = process;
			if (debuggedProcess != null) {
				debuggedProcess.DebuggeeStateChanged += debuggedProcess_DebuggeeStateChanged;
				debuggedProcess.ThreadStarted        += debuggedProcess_ThreadStarted;
				debuggedProcess.ThreadStateChanged   += debuggedProcess_ThreadStateChanged;
				debuggedProcess.ThreadExited         += debuggedProcess_ThreadExited;
			}
			runningThreadsList.Clear();
			RefreshPad();
		}
		
		void debuggedProcess_DebuggeeStateChanged(object sender, ProcessEventArgs e)
		{
			RefreshPad();
		}
		
		void debuggedProcess_ThreadStarted(object sender, ThreadEventArgs e)
		{
			AddThread(e.Thread);
		}
		
		void debuggedProcess_ThreadStateChanged(object sender, ThreadEventArgs e)
		{
			RefreshThread(e.Thread);
		}
		
		void debuggedProcess_ThreadExited(object sender, ThreadEventArgs e)
		{
			RemoveThread(e.Thread);
		}
		
		public override void RefreshPad()
		{
			if (debuggedProcess != null) {
				foreach (Thread t in debuggedProcess.Threads) {
					RefreshThread(t);
				}
			}
		}

		void RunningThreadsListItemActivate(object sender, EventArgs e)
		{
			if (debuggedProcess.IsPaused) {
				if (debuggedProcess != null) {
					debuggedProcess.SelectedThread = (Thread)(runningThreadsList.SelectedItems[0].Tag);
					debuggedProcess.OnDebuggeeStateChanged(); // Force refresh of pads
				}
			} else {
				MessageService.ShowMessage("${res:MainWindow.Windows.Debug.Threads.CannotSwitchWhileRunning}", "${res:MainWindow.Windows.Debug.Threads.ThreadSwitch}");
			}
		}		
		
		void AddThread(Thread thread)
		{
			runningThreadsList.Items.Add(new ListViewItem(thread.ID.ToString()));
			RefreshThread(thread);
		}
		
		void RefreshThread(Thread thread)
		{
			foreach (ListViewItem item in runningThreadsList.Items) {
				if (thread.ID.ToString() == item.Text) {
					item.SubItems.Clear();
					item.Text = thread.ID.ToString();
					item.Tag = thread;
					item.SubItems.Add(thread.Name);
					Function location = null;
					if (thread.Process.IsPaused) {
						location = thread.LastFunctionWithLoadedSymbols;
						if (location == null) {
							location = thread.LastFunction;
						}
					}
					if (location != null) {
						item.SubItems.Add(location.Name);
					} else {
						item.SubItems.Add(ResourceService.GetString("Global.NA"));
					}
					switch (thread.Priority) {
						case System.Threading.ThreadPriority.Highest:
							item.SubItems.Add(ResourceService.GetString("MainWindow.Windows.Debug.Threads.Priority.Highest"));
							break;
						case System.Threading.ThreadPriority.AboveNormal:
							item.SubItems.Add(ResourceService.GetString("MainWindow.Windows.Debug.Threads.Priority.AboveNormal"));
							break;
						case System.Threading.ThreadPriority.Normal:
							item.SubItems.Add(ResourceService.GetString("MainWindow.Windows.Debug.Threads.Priority.Normal"));
							break;
						case System.Threading.ThreadPriority.BelowNormal:
							item.SubItems.Add(ResourceService.GetString("MainWindow.Windows.Debug.Threads.Priority.BelowNormal"));
							break;
						case System.Threading.ThreadPriority.Lowest:
							item.SubItems.Add(ResourceService.GetString("MainWindow.Windows.Debug.Threads.Priority.Lowest"));
							break;
						default:
							item.SubItems.Add(thread.Priority.ToString());
							break;
					}
					item.SubItems.Add(ResourceService.GetString(thread.Suspended ? "Global.Yes" : "Global.No"));
					return;
				}
			}
			AddThread(thread);
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
