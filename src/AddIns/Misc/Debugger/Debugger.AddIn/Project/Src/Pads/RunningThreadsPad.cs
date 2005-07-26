// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
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

using DebuggerLibrary;

namespace ICSharpCode.SharpDevelop.Gui.Pads
{
	public class RunningThreadsPad : AbstractPadContent
	{
		WindowsDebugger debugger;
		NDebugger debuggerCore;

		ListView  runningThreadsList;
		
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
		
		public RunningThreadsPad() //: base("${res:MainWindow.Windows.Debug.Threads}", null)
		{
			InitializeComponents();
		}
			
		void InitializeComponents()
		{
			debugger = (WindowsDebugger)DebuggerService.CurrentDebugger;
			
			runningThreadsList = new ListView();
			runningThreadsList.FullRowSelect = true;
			runningThreadsList.AutoArrange = true;
			runningThreadsList.Alignment   = ListViewAlignment.Left;
			runningThreadsList.View = View.Details;
			runningThreadsList.Dock = DockStyle.Fill;
			runningThreadsList.GridLines  = false;
			runningThreadsList.Activation = ItemActivation.OneClick;
			runningThreadsList.Columns.AddRange(new ColumnHeader[] {id, name, location, priority, breaked} );
			runningThreadsList.ItemActivate += new EventHandler(RunningThreadsListItemActivate);
			id.Width = 100;
			name.Width = 300;
			location.Width = 250;
			priority.Width = 120;
			breaked.Width = 80;
			
			RedrawContent();

			if (debugger.ServiceInitialized) {
				InitializeDebugger();
			} else {
				debugger.Initialize += delegate {
					InitializeDebugger();
				};
			}
		}

		public void InitializeDebugger()
		{
			debuggerCore = debugger.DebuggerCore;

			debuggerCore.DebuggingPaused += new EventHandler<DebuggingPausedEventArgs>(OnDebuggingPaused);
			debuggerCore.ThreadStarted += new EventHandler<ThreadEventArgs>(ThreadStarted);
			debuggerCore.ThreadStateChanged += new EventHandler<ThreadEventArgs>(ThreadStateChanged);
			debuggerCore.ThreadExited += new EventHandler<ThreadEventArgs>(ThreadExited);

			RefreshList();
		}
		
		public override void RedrawContent()
		{
			id.Text          = "ID";
			name.Text        = "Name";
			location.Text    = "Location";
			priority.Text    = "Priority";
			breaked.Text     = "Breaked";
		}
		
		void RefreshList()
		{
			foreach (Thread t in debuggerCore.Threads) {
				RefreshThread(t);
			}
		}

		void RunningThreadsListItemActivate(object sender, EventArgs e)
		{
			if (debuggerCore.IsPaused) {
				if (debuggerCore.CurrentProcess != null) {
					debuggerCore.CurrentProcess.SetCurrentThread((Thread)(runningThreadsList.SelectedItems[0].Tag));
				}
			} else {
				MessageBox.Show("You can not switch threads while the debugger is running.", "Thread switch");
			}
		}

		void OnDebuggingPaused(object sender, DebuggingPausedEventArgs e)
		{
			RefreshList();
		}

		void ThreadStarted(object sender, ThreadEventArgs e)
		{
			AddThread(e.Thread);
		}

		void ThreadStateChanged(object sender, ThreadEventArgs e)
		{
			RefreshThread(e.Thread);
		}
		
		void ThreadExited(object sender, ThreadEventArgs e)
		{
			RemoveThread(e.Thread);
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
					Function location;
					location = thread.LastFunctionWithLoadedSymbols;
					if (location == null) {
						location = thread.LastFunction;
					}
					if (location != null) {
						item.SubItems.Add(location.Name);
					} else {
						item.SubItems.Add("N/A");
					}
					item.SubItems.Add(thread.Priority.ToString());
					item.SubItems.Add(thread.Suspended.ToString());
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
